using ComplexPortfolio.Module.BusinessObjects;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.Persistent.Base;
using DevExpress.Spreadsheet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tinkoff.Trading.OpenApi.Network;

namespace ComplexPortfolio.Module.Controllers {

    public class UpdateAllTickersController : ObjectViewController<ListView, Ticker> {
        public UpdateAllTickersController() {
            var updateDataAction = new SimpleAction(this, "updateData", PredefinedCategory.Edit);
            updateDataAction.Execute += updateDataAction_Execute;

            var reloadAllDataAction = new SimpleAction(this, "reloadAllData", PredefinedCategory.Edit);
            reloadAllDataAction.Execute += reloadAllDataAction_Execute;
        }
        private async void reloadAllDataAction_Execute(object sender, SimpleActionExecuteEventArgs e) {
            var os = Application.CreateObjectSpace(typeof(TickerDayDatum));
            var tickers = os.GetObjects<Ticker>();
            var cnt = new UpdateOneTickerController();
            foreach(var ticker in tickers) {
                cnt.ReloadAllDataForTicker(ticker, os);
            }
        }

        private async void updateDataAction_Execute(object sender, SimpleActionExecuteEventArgs e) {

            var staticStartDate = new DateTime(DateTime.Today.Year, 2, 1);
            var d1 = staticStartDate;
            var d2 = DateTime.Today;//.AddDays(-1);
            var os = Application.CreateObjectSpace(typeof(TickerDayDatum));

            var tickers = os.GetObjects<Ticker>();
            var dataLoader = new GetTickerDataLoader();
            var tickerFactory = new TickerDayDataFactory();
            foreach(var ticker in tickers) {
                HashSet<DateTime> existingDataDates = new HashSet<DateTime>(ticker.DayData.Select(x => x.Date));
                if(existingDataDates.Count > 0) {
                    d1 = existingDataDates.Max();
                }
                var candles = await dataLoader.GetTickerData(ticker.Name, d1, d2);
                // var candles = await dataLoader.GetTickerYearData(ticker.Name,2020);
                foreach(var c in candles) {
                    tickerFactory.CreateTickerDayDataFromCandle(ticker, c, existingDataDates, os, DateTime.Today);
                }
                os.CommitChanges();
                d1 = staticStartDate;
            }


            //test
            //using(Workbook workbook = new Workbook()) {
            //    Worksheet ws = workbook.Worksheets[0];
            //    workbook.BeginUpdate();
            //    var currentRow = 1;
            //    foreach(var c in data.Candles) {
            //        ws.Cells[currentRow, 1].Value = c.Time;
            //        ws.Cells[currentRow, 2].Value = c.Open;
            //        ws.Cells[currentRow, 3].Value = c.High;
            //        ws.Cells[currentRow, 4].Value = c.Low;
            //        ws.Cells[currentRow, 5].Value = c.Close;
            //        currentRow++;
            //    }
            //    workbook.EndUpdate();
            //    string date = DateTime.Now.ToString("yyyy_MM_dd");
            //    string fileName = string.Format(@"c:\temp\shares\TestDocApi{0}.xlsx", date);
            //    workbook.SaveDocument(fileName, DocumentFormat.Xlsx);
            //}

        }
    }
}
