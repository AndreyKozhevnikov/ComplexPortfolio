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
   public class GetTickerDataFromAPIController: ObjectViewController<ListView,Ticker> {
        public GetTickerDataFromAPIController() {
            var getDataAction = new SimpleAction(this, "GetDataAPI", PredefinedCategory.Edit);
            getDataAction.Execute += GetDataAction_Execute;
        }

        private async void GetDataAction_Execute(object sender, SimpleActionExecuteEventArgs e) {
            var fileTokenName = @"c:\Skt\ttoken.txt";
            var file =     new System.IO.StreamReader(fileTokenName);
            var token = file.ReadLine();
            file.Close();
            // для работы в песочнице используйте GetSandboxConnection
            var connection = ConnectionFactory.GetSandboxConnection(token);
            var context = connection.Context;

            // вся работа происходит асинхронно через объект контекста
          //  await context.RegisterAsync(Tinkoff.Trading.OpenApi.Models.BrokerAccountType.Tinkoff);
            var portfolio = await context.PortfolioAsync();
            var figiAcc = await context.MarketSearchByTickerAsync("FXGD");
            var figi = figiAcc.Instruments[0].Figi;
            var data = await context.MarketCandlesAsync(figi, new DateTime(2021, 1, 1), DateTime.Today, Tinkoff.Trading.OpenApi.Models.CandleInterval.Day);
            

            //test
            using(Workbook workbook = new Workbook()) {
                Worksheet ws = workbook.Worksheets[0];
                workbook.BeginUpdate();
                var currentRow = 1;
                foreach(var c in data.Candles) {
                    ws.Cells[currentRow, 1].Value = c.Time;
                    ws.Cells[currentRow, 2].Value = c.Open;
                    ws.Cells[currentRow, 3].Value = c.High;
                    ws.Cells[currentRow, 4].Value = c.Low;
                    ws.Cells[currentRow, 5].Value = c.Close;
                    currentRow++;
                }
                workbook.EndUpdate();
                string date = DateTime.Now.ToString("yyyy_MM_dd");
                string fileName = string.Format(@"c:\temp\shares\TestDocApi{0}.xlsx", date);
                workbook.SaveDocument(fileName, DocumentFormat.Xlsx);
            }

            }
    }
}
