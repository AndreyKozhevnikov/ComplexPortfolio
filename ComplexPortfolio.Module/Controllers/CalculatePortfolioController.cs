using ComplexPortfolio.Module.BusinessObjects;
using ComplexPortfolio.Module.HelpClasses;
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

        public List<CalcPortfolioDatum> CalculatePortfolioDataList(List<Position> positions) {
            DateTime startDate = DateTime.Today;
            DateTime finishDate = DateTime.MinValue;
            foreach(var p in positions) {
                var positionStartDate = p.CalculateData.Min(x => x.Date);
                if(positionStartDate < startDate) {
                    startDate = positionStartDate;
                }
                var positionFinishDate = p.CalculateData.Max(x => x.Date);
                if(positionFinishDate > finishDate) {
                    finishDate = positionFinishDate;
                }
            }
            List<CalcPortfolioDatum> result = new List<CalcPortfolioDatum>();
            while(startDate <= finishDate) {
                List<CalcPositionDatum> tmpList = new List<CalcPositionDatum>();
                foreach(var p in positions) {
                    var positionData = p.CalculateData.Where(x => x.Date == startDate).FirstOrDefault();
                    if(positionData != null) {
                        tmpList.Add(positionData);
                    }
                }
                if(tmpList.Count > 0) {
                    var portfolioData = new CalcPortfolioDatum(startDate);
                    portfolioData.PositionData = tmpList;
                    CalculateSinglePorfolioDatum(portfolioData);
                    result.Add(portfolioData);
                }
                startDate = startDate.AddDays(1);
            }
            return result;
        }

        public void CalculateSinglePorfolioDatum(CalcPortfolioDatum datum) {
            datum.SumTotalValues = new List<Tuple<string, decimal>>();
            datum.SumDiffTotalValues = new List<Tuple<string, decimal>>();
            foreach(var p in datum.PositionData) {
                datum.SumTotalValues.Add(new Tuple<string,decimal>(p.TickerName, p.Value));
                datum.SumDiffTotalValues.Add(new Tuple<string, decimal>(p.TickerName, p.ValueDiffTotal));
            }
        }

        public void ExportToExcel(IWorkSheetWorker wsWorker, List<CalcPortfolioDatum> calcPortData) {

            var currentColumn = 1;
            var currentRow = 6;
            wsWorker.SetCellValue(currentRow, currentColumn, "SumTotal");
            currentColumn++;
            foreach(var tickerName in calcPortData[0].Tickers) {
                wsWorker.SetCellValue(currentRow, currentColumn, tickerName);
                currentColumn++;
            }
            currentColumn++;
            wsWorker.SetCellValue(currentRow, currentColumn, "SumDiffTotal");
            currentColumn++;
            foreach(var tickerName in calcPortData[0].Tickers) {
                wsWorker.SetCellValue(currentRow, currentColumn, tickerName);
                currentColumn++;
            }

            foreach(var calcPortDatum in calcPortData) {
                currentColumn = 0;
                currentRow++;
                wsWorker.SetCellValue(currentRow, currentColumn, calcPortDatum.Date);
                currentColumn++;
                wsWorker.SetCellValue(currentRow, currentColumn, calcPortDatum.SumTotal);
                foreach(var sumTolalValue in calcPortDatum.SumTotalValues) {
                    currentColumn++;
                    wsWorker.SetCellValue(currentRow, currentColumn, sumTolalValue.Item2);
                }
                currentColumn++;
                currentColumn++;
                wsWorker.SetCellValue(currentRow, currentColumn, calcPortDatum.SumDiffTotal);
                foreach(var sumDiffTolalValue in calcPortDatum.SumDiffTotalValues) {
                    currentColumn++;
                    wsWorker.SetCellValue(currentRow, currentColumn, sumDiffTolalValue.Item2);
                }
            }
        }

        private void ExportToExcelAction_Execute(object sender, SimpleActionExecuteEventArgs e) {
            var portfolio = this.ViewCurrentObject;
            var cnt = new CalculatePositionController();
            var os = Application.CreateObjectSpace(typeof(Position));

            foreach(var position in portfolio.Positions) {
                cnt.CalculatePosition(position, os);
            }

            var resultToPrint = CalculatePortfolioDataList(portfolio.Positions.ToList());
            using(Workbook workbook = new Workbook()) {

                var wsWorker = new WorkSheetWorker();
                wsWorker.CurrentWorkSheet = workbook.Worksheets[0];
                workbook.BeginUpdate();
                ExportToExcel(wsWorker, resultToPrint);

                workbook.EndUpdate();
                string date = DateTime.Now.ToString("yyyy_MM_dd");
                string fileName = string.Format(@"c:\temp\shares\TestDoc{0}.xlsx", date);
                workbook.SaveDocument(fileName, DocumentFormat.Xlsx);
            }

        }
    }
}

