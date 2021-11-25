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

            var calculateAction = new SimpleAction(this, "CalculateDayData", PredefinedCategory.Edit);
            calculateAction.Execute += CalculateAction_Execute; ;
        }

        private void CalculateAction_Execute(object sender, SimpleActionExecuteEventArgs e) {
            var portfolio = this.ViewCurrentObject;
            var cnt = new CalculatePositionController();
            var os = Application.CreateObjectSpace(typeof(Position));

            foreach(var position in portfolio.Positions) {
                cnt.CalculatePosition(position, os);
            }
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
            datum.SumTotalValues = new Dictionary<string, double>();
            datum.SumDiffTotalValues = new Dictionary<string, double>();
            datum.SumTotalValuesLabels = new Dictionary<string, double>();
            datum.SumDiffTotalValuesLabels = new Dictionary<string, double>();
            bool hasLabels = true;
            foreach(var p in datum.PositionData) {
                datum.SumTotalValues[p.TickerName] = p.Value;
                datum.SumDiffTotalValues[p.TickerName] = p.ValueDiffTotal;
                if(string.IsNullOrEmpty(p.Label) || !hasLabels) {
                    hasLabels = false;
                    datum.SumTotalValuesLabels.Clear();
                    datum.SumDiffTotalValuesLabels.Clear();
                    continue;
                }
                if(datum.SumTotalValuesLabels.ContainsKey(p.Label)) {
                    datum.SumTotalValuesLabels[p.Label] += p.Value;
                    datum.SumDiffTotalValuesLabels[p.Label] += p.ValueDiffTotal;
                } else {
                    datum.SumTotalValuesLabels[p.Label] = p.Value;
                    datum.SumDiffTotalValuesLabels[p.Label] = p.ValueDiffTotal;
                }
            }
        }

        public List<string> GetAllTickersFromCalcPortfolioData(List<CalcPortfolioDatum> calcPortData) {
            var resultList = new List<string>();
            foreach(var datum in calcPortData) {
                resultList = resultList.Concat(datum.Tickers).ToList();
            }
            var uniqueList = resultList.Distinct<string>().OrderBy(x => x).ToList();
            return uniqueList;
        }
        public void ExportToExcel(IWorkSheetWorker wsWorker, List<CalcPortfolioDatum> calcPortData) {
            ExportToExcel_Tickers(wsWorker, calcPortData);
        }
        public int ExportToExcel_Tickers(IWorkSheetWorker wsWorker, List<CalcPortfolioDatum> calcPortData) {

            var tickers = GetAllTickersFromCalcPortfolioData(calcPortData);
            var currentColumn = 2;
            var currentRow = 7;
            wsWorker.SetCellValue(currentRow, currentColumn, "SumTotal");
            currentColumn++;
            foreach(var tickerName in tickers) {
                wsWorker.SetCellValue(currentRow, currentColumn, tickerName);
                currentColumn++;
            }
            currentColumn += 2;
            wsWorker.SetCellValue(currentRow, currentColumn, "SumDiffTotal");
            currentColumn++;
            foreach(var tickerName in tickers) {
                wsWorker.SetCellValue(currentRow, currentColumn, tickerName);
                currentColumn++;
            }

            foreach(var calcPortDatum in calcPortData) {
                currentColumn = 1;
                currentRow++;
                wsWorker.SetCellValue(currentRow, currentColumn, calcPortDatum.Date);
                currentColumn++;
                wsWorker.SetCellValue(currentRow, currentColumn, calcPortDatum.SumTotal);
                foreach(var sumTolalValue in calcPortDatum.SumTotalValues) {
                    var offSet = tickers.IndexOf(sumTolalValue.Key);
                    var thisColumn = currentColumn + offSet + 1;
                    wsWorker.SetCellValue(currentRow, thisColumn, sumTolalValue.Value);
                }
                currentColumn = currentColumn + tickers.Count + 2;
                wsWorker.SetCellValue(currentRow, currentColumn, calcPortDatum.Date);
                currentColumn++;
                wsWorker.SetCellValue(currentRow, currentColumn, calcPortDatum.SumDiffTotal);
                foreach(var sumDiffTolalValue in calcPortDatum.SumDiffTotalValues) {
                    var offSet = tickers.IndexOf(sumDiffTolalValue.Key);
                    var thisColumn = currentColumn + offSet + 1;
                    wsWorker.SetCellValue(currentRow, thisColumn, sumDiffTolalValue.Value);
                }
            }
            return currentColumn + tickers.Count;
        }

        private void ExportToExcelAction_Execute(object sender, SimpleActionExecuteEventArgs e) {
            CalculateAction_Execute(null, null);
            var portfolio = this.ViewCurrentObject;
            var resultToPrint = CalculatePortfolioDataList(portfolio.Positions.ToList());
            using(Workbook workbook = new Workbook()) {

                var wsWorker = new WorkSheetWorker();
                wsWorker.CurrentWorkSheet = workbook.Worksheets[0];
                workbook.BeginUpdate();
                ExportToExcel(wsWorker, resultToPrint);

                workbook.EndUpdate();
                string date = DateTime.Now.ToString("yyyy_MM_dd");
                string fileName = string.Format(@"c:\temp\{0}-{1}.xlsx", portfolio.Name, date);
                workbook.SaveDocument(fileName, DocumentFormat.Xlsx);
            }

        }
    }
}

