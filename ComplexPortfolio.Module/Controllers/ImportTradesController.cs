using ComplexPortfolio.Module.BusinessObjects;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.Persistent.Base;
using DevExpress.Spreadsheet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComplexPortfolio.Module.Controllers {
    public class ImportTradesController : ObjectViewController<ListView,Transaction> {
        public ImportTradesController() {
            var exportTradesAction = new SimpleAction(this, "ExportTrades", PredefinedCategory.Edit);
            exportTradesAction.Execute += ExportTradesAction_Execute;
        }

        private void ExportTradesAction_Execute(object sender, SimpleActionExecuteEventArgs e) {
            var fileName = @"c:\Dropbox\Stocks\MyTrades.xlsx";
            var wb = new Workbook();
            wb.LoadDocument(fileName);
            Worksheet ws = wb.Worksheets[0];
            var os = Application.CreateObjectSpace(typeof(Ticker));
            for(int i = 2; i < 66; i++) {
                var tickerName = ws.Cells[i, 1].Value.TextValue;
                var ticker = os.FindObject<Ticker>(new BinaryOperator(nameof(Ticker.Name), tickerName));
                if(ticker == null) {
                    ticker = os.CreateObject<Ticker>();
                    ticker.Name = tickerName;
                    os.CommitChanges();
                }
                var position = os.FindObject<Position>(GroupOperator.And(new BinaryOperator(nameof(Position.Ticker), ticker), new BinaryOperator(nameof(Position.Comment), "export")));
                if(position == null) {
                    position = os.CreateObject<Position>();
                    position.Ticker = ticker;
                    position.Comment = "export";
                    os.CommitChanges();
                }
                var transaction = os.CreateObject<Transaction>();
                transaction.Position = position;
                transaction.Date =ws.Cells[i, 0].Value.DateTimeValue;
                transaction.Amount =(int) ws.Cells[i, 2].Value.NumericValue;
                transaction.Price =(decimal) ws.Cells[i, 3].Value.NumericValue;
                transaction.Comment = ws.Cells[i, 6].Value.TextValue;
                
            }
            os.CommitChanges();

        }
    }
}
