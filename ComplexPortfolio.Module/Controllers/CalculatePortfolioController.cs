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

namespace ComplexPortfolio.Module.Controllers {
    public class CalculatePortfolioController : ObjectViewController<DetailView, Portfolio> {
        public CalculatePortfolioController() {
            var exportToExcelAction = new SimpleAction(this, "ExportToExcel", PredefinedCategory.Edit);
            exportToExcelAction.Execute += ExportToExcelAction_Execute;
        }

        private void ExportToExcelAction_Execute(object sender, SimpleActionExecuteEventArgs e) {
            var portfolio = this.ViewCurrentObject;
            var cnt = new CalculatePositionController();
            var os = Application.CreateObjectSpace(typeof(Position));
            using(Workbook workbook = new Workbook()) {
                // Access the first worksheet in the workbook.

                Worksheet worksheet = workbook.Worksheets[0];
                workbook.BeginUpdate();
                var currentColumn = 1;
                foreach(var position in portfolio.Positions) {
                    cnt.CalculatePosition(position, os);
                    var currentRow = 5;
                    worksheet.Cells[currentRow, currentColumn].Value = position.Ticker.Name;
                    currentRow++;
                    foreach(var calcData in position.CalculateData) {
                        worksheet.Cells[currentRow,currentColumn].Value = calcData.Date;
                        worksheet.Cells[currentRow,currentColumn+1].Value = calcData.Price;
                        worksheet.Cells[currentRow,currentColumn+2].Value = calcData.SharesCount;
                        worksheet.Cells[currentRow,currentColumn+3].Value = calcData.Value;
                        worksheet.Cells[currentRow,currentColumn+4].Value = calcData.ValueDiff;
                        worksheet.Cells[currentRow,currentColumn+5].Value = calcData.ValueTotal;
                        currentRow++;
                    }
                    currentColumn += 8;
                }

                workbook.EndUpdate();
                workbook.SaveDocument(@"c:\temp\shares\TestDoc.xlsx", DocumentFormat.Xlsx);
            }
        }
    }
}
