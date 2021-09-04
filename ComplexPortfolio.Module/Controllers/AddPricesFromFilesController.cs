using ComplexPortfolio.Module.BusinessObjects;
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
   public class AddPricesFromFilesController:ObjectViewController<ListView,TickerPrice> {
        public AddPricesFromFilesController() {
            var addPricesAction = new SimpleAction(this, "AddPricesAction", PredefinedCategory.Edit);
            addPricesAction.Execute += AddPricesAction_Execute; ;
        }

        private void AddPricesAction_Execute(object sender, SimpleActionExecuteEventArgs e) {
            var folder = @"c:\temp\shares";
            string[] fileEntries = Directory.GetFiles(folder);
            foreach (var fileName in fileEntries) {
                StreamReader file =    new StreamReader(fileName);
                string line;
                while((line= file.ReadLine()) != null) {

                }

            }
        }
      public  TickerPrice GetPriceFromLine(string line,IObjectSpace os) {
            return null;
        }
    }
}
