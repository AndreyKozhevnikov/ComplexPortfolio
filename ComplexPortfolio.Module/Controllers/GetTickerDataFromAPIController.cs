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
using Tinkoff.Trading.OpenApi.Models;
using Tinkoff.Trading.OpenApi.Network;

namespace ComplexPortfolio.Module.Controllers {
    public class GetTickerDataFromAPIController : ObjectViewController<ListView, Ticker> {
        public GetTickerDataFromAPIController() {
            var getDataAction = new SimpleAction(this, "GetAllDataFromAPI", PredefinedCategory.Edit);
            getDataAction.Execute += GetDataAction_Execute;
        }

        public void CreateTickerDayDataFromCandle(Ticker ticker, CandlePayload candle, HashSet<DateTime> existingDataDates, IObjectSpace os) {
            if(existingDataDates.Contains(candle.Time.Date)) {
                return;
            }
            var dayData = os.CreateObject<TickerDayDatum>();
            dayData.Ticker = ticker;
            dayData.Date = candle.Time.Date;
            dayData.Open = candle.Open;
            dayData.High = candle.High;
            dayData.Low = candle.Low;
            dayData.Close = candle.Close;
            dayData.Volume = (double)candle.Volume;
            existingDataDates.Add(dayData.Date);
        }
        private async void GetDataAction_Execute(object sender, SimpleActionExecuteEventArgs e) {
            var fileTokenName = @"c:\Skt\ttoken.txt";
            var file = new System.IO.StreamReader(fileTokenName);
            var token = file.ReadLine();
            file.Close();
            // для работы в песочнице используйте GetSandboxConnection
            var connection = ConnectionFactory.GetSandboxConnection(token);
            var context = connection.Context;

            // вся работа происходит асинхронно через объект контекста
            //  await context.RegisterAsync(Tinkoff.Trading.OpenApi.Models.BrokerAccountType.Tinkoff);
            //var portfolio = await context.PortfolioAsync();
            //var d1 = new DateTime(2020, 7, 1);
            //var d2 = new DateTime(2021, 6, 30);
            var d1 = new DateTime(2021, 7, 1);
            var d2 = DateTime.Today.AddDays(-1);
            var os = Application.CreateObjectSpace(typeof(TickerDayDatum));

            var tickers = os.GetObjects<Ticker>();
            foreach(var ticker in tickers) {
                HashSet<DateTime> existingDataDates = new HashSet<DateTime>(ticker.DayData.Select(x => x.Date));
                if(existingDataDates.Count > 0) {
                    d1 = existingDataDates.Max();
                }
                var figiAccs = await context.MarketSearchByTickerAsync(ticker.Name);
               // var test=await context.ma
                if(figiAccs.Instruments.Count == 0) {
                    continue;
                }
                var figi = figiAccs.Instruments[0].Figi;
                var data = await context.MarketCandlesAsync(figi, d1, d2, Tinkoff.Trading.OpenApi.Models.CandleInterval.Day);
                foreach(var c in data.Candles) {
                    CreateTickerDayDataFromCandle(ticker, c, existingDataDates, os);
                }
                os.CommitChanges();
                d1 = new DateTime(2021, 7, 1);
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
