using ComplexPortfolio.Module.BusinessObjects;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.Persistent.Base;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComplexPortfolio.Module.Controllers {
   public class AddDayDataFromFilesController:ObjectViewController<ListView,TickerDayData> {
        public AddDayDataFromFilesController() {
            var addDayDataAction = new SimpleAction(this, "AddPricesAction", PredefinedCategory.Edit);
            addDayDataAction.Execute += AddDayDataAction_Execute; ;
        }

        private void AddDayDataAction_Execute(object sender, SimpleActionExecuteEventArgs e) {
            var folder = @"c:\temp\shares";
            string[] fileEntries = Directory.GetFiles(folder);
            var os = Application.CreateObjectSpace(typeof(TickerDayData));
            var existingPrices = os.GetObjects<TickerDayData>();
            var existingTickers = os.GetObjects<Ticker>();
            foreach (var fileName in fileEntries) {
                StreamReader file = new StreamReader(fileName);
                string line;
                while((line= file.ReadLine()) != null) {
                    GetPriceFromLine(line, os, existingPrices, existingTickers);
                }
            }
            os.CommitChanges();
        }
      public TickerDayData GetPriceFromLine(string line,IObjectSpace os, IList<TickerDayData> existingPrices, IList<Ticker> existingTickers) {
            var cells = line.Split(';');
            if(cells[0].StartsWith('<')) {
                return null;
            }
         
            var tickerName = cells[0];
            var ticker = existingTickers.Where(x=>x.Name == tickerName).FirstOrDefault();
            if(ticker == null) {
                ticker = os.CreateObject<Ticker>();
                ticker.Name = tickerName;
            }
            var dateString = cells[2];
            var year = dateString.Substring(0, 4);
            var month = dateString.Substring(4, 2);
            var date = dateString.Substring(6, 2);
            var dateTime= new DateTime(int.Parse(year), int.Parse(month), int.Parse(date));
            //var existPrice = os.FindObject<TickerPrice>(CriteriaOperator.Parse("Ticker.Name=? and Date=?",tickerName, dateTime));
            var existPrice = existingPrices.Where(x => x.Ticker.Name == tickerName && x.Date == dateTime).FirstOrDefault();
            if(existPrice != null) {
                return null;
            }
            var price = os.CreateObject<TickerDayData>();
            price.Ticker = ticker;

            price.Date = dateTime;
            price.Open =decimal.Parse( cells[4]);
            price.High = decimal.Parse(cells[5]);
            price.Low = decimal.Parse(cells[6]);
            price.Close = decimal.Parse(cells[7]);
            price.Volume = double.Parse(cells[8]);
            return price;
        }
    }
}
