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
            var exportTradesAction = new SimpleAction(this, "ImportTrades", PredefinedCategory.Edit);
            exportTradesAction.Execute += ImportTradesAction_Execute;
        }

        private void ImportTradesAction_Execute(object sender, SimpleActionExecuteEventArgs e) {
            var fileName = @"c:\Dropbox\Stocks\IdealETF\IdealETF.xlsx";
            var wb = new Workbook();
            wb.LoadDocument(fileName);
            Worksheet ws = wb.Worksheets[0];
            var os = Application.CreateObjectSpace(typeof(Ticker));
            var port = os.FindObject<Portfolio>(new BinaryOperator(nameof(Portfolio.Name), "IdealETF(funny)"));
            if(port == null) {
                return;
            }
            var acc = os.FindObject<Account>(new BinaryOperator(nameof(Account.Name), "IdealETF(Funny)"));

            for(int i = 1; i < 43; i++) {//!!!!!


                var tickerName = ws.Cells[i, 1].Value.TextValue;
                var ticker = os.FindObject<Ticker>(new BinaryOperator(nameof(Ticker.Name), tickerName));
                if(ticker == null) {
                    ticker = os.CreateObject<Ticker>();
                    ticker.Name = tickerName;
                    os.CommitChanges();
                }
                var position = os.FindObject<Position>(GroupOperator.And(new BinaryOperator(nameof(Position.Ticker), ticker), new BinaryOperator(nameof(Position.Portfolio), port)));
                if(position == null) {
                    position = os.CreateObject<Position>();
                    position.Ticker = ticker;
                    position.Comment = "CommonImport";
                    position.Portfolio = port;
                    os.CommitChanges();
                }
                var transaction = os.CreateObject<Transaction>();
                transaction.Position = position;
                transaction.Date =ws.Cells[i, 0].Value.DateTimeValue;
                transaction.Amount =(int) ws.Cells[i, 2].Value.NumericValue;
                transaction.Price =(double) ws.Cells[i, 3].Value.NumericValue;
                transaction.Comment = ws.Cells[i, 6].Value.TextValue;
                transaction.Account = acc;
                
            }
            os.CommitChanges();

        }
    }
}
