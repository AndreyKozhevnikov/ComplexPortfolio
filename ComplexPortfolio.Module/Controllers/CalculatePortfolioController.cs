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
        public List<CalcPortfolioDatumExportBlock> CreateExportBlocksFromData(List<CalcPortfolioDatum> calcPortData) {
            
            var tickerNames = GetAllTickersFromCalcPortfolioData(calcPortData);
            var labelNames = GetAllLabelsFromCalcPortfolioData(calcPortData);

            var tickersBlock = new CalcPortfolioDatumExportBlock("Tickers");
            tickersBlock.Names = tickerNames;
            var tickersDiffBlock = new CalcPortfolioDatumExportBlock("TickersDiff");
            tickersDiffBlock.Names = tickerNames;

            var labelsBlock = new CalcPortfolioDatumExportBlock("Labels");
            var labelsDiffBlock = new CalcPortfolioDatumExportBlock("LabelsDiff");

            foreach(var calcData in calcPortData) {
                var tickerElement = new CalcPortfolioDatumExportElement(calcData.Date);
                tickerElement.SumValue = calcData.SumTotal;
                foreach(var position in calcData.SumTotalValues) {
                    var ind = tickerNames.IndexOf(position.Key);
                    tickerElement.Values[ind] = position.Value;
                }
                tickersBlock.Elements.Add(tickerElement);

                var tickerDiffElement = new CalcPortfolioDatumExportElement(calcData.Date);
                tickerDiffElement.SumValue = calcData.SumDiffTotal;
                foreach(var position in calcData.SumDiffTotalValues) {
                    var ind = tickerNames.IndexOf(position.Key);
                    tickerDiffElement.Values[ind] = position.Value;
                }
                tickersDiffBlock.Elements.Add(tickerDiffElement);


                //exportDatum.Date = calcData.Date;
                //exportDatum.Names = tickers;
                //exportDatum.Values = calcData.SumTotalValues;
                //exportDatum.DiffValues = calcData.SumDiffTotalValues;
                //exportData.Add(exportDatum);
            }
            var result = new List<CalcPortfolioDatumExportBlock>();
            result.Add(tickersBlock);
            result.Add(tickersDiffBlock);
            return result;
        }
        public List<string> GetAllLabelsFromCalcPortfolioData(List<CalcPortfolioDatum> calcPortData) {
            var resultList = new List<string>();
            foreach(var datum in calcPortData) {
                resultList = resultList.Concat(datum.Labels).ToList();
            }
            var uniqueList = resultList.Distinct<string>().OrderBy(x => x).ToList();
            return uniqueList;
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
            List<CalcPortfolioDatumExportBlock> blocks = CreateExportBlocksFromData(calcPortData);

            var c = 1;
            foreach(var block in blocks) {
                c = ExportToExcelOneBlock(wsWorker, block, c);
                c += 2;
            }
        }



        public int ExportToExcelOneBlock(IWorkSheetWorker wsWorker, CalcPortfolioDatumExportBlock block, int startColumn) {
            //    var currentColumn = startColumn;
            //    var currentRow = startRow;
            //    wsWorker.SetCellValue(currentRow, currentColumn, "SumTotal");
            //    currentColumn++;
            //    List<string> tickers = exportList[0].Names;
            //    foreach(var tickerName in tickers) {
            //        wsWorker.SetCellValue(currentRow, currentColumn, tickerName);
            //        currentColumn++;
            //    }
            //    currentColumn += 2;
            //    wsWorker.SetCellValue(currentRow, currentColumn, "SumDiffTotal");
            //    currentColumn++;
            //    foreach(var tickerName in tickers) {
            //        wsWorker.SetCellValue(currentRow, currentColumn, tickerName);
            //        currentColumn++;
            //    }

            //    foreach(var calcPortDatum in exportList) {
            //        currentColumn = 1;
            //        currentRow++;
            //        wsWorker.SetCellValue(currentRow, currentColumn, calcPortDatum.Date);
            //        currentColumn++;
            //        wsWorker.SetCellValue(currentRow, currentColumn, calcPortDatum.Values.Sum(x => x.Value));
            //        foreach(var sumTolalValue in calcPortDatum.Values) {
            //            var offSet = tickers.IndexOf(sumTolalValue.Key);
            //            var thisColumn = currentColumn + offSet + 1;
            //            wsWorker.SetCellValue(currentRow, thisColumn, sumTolalValue.Value);
            //        }
            //        currentColumn = currentColumn + tickers.Count + 2;
            //        wsWorker.SetCellValue(currentRow, currentColumn, calcPortDatum.Date);
            //        currentColumn++;
            //        wsWorker.SetCellValue(currentRow, currentColumn, calcPortDatum.DiffValues.Sum(x => x.Value));
            //        foreach(var sumDiffTolalValue in calcPortDatum.DiffValues) {
            //            var offSet = tickers.IndexOf(sumDiffTolalValue.Key);
            //            var thisColumn = currentColumn + offSet + 1;
            //            wsWorker.SetCellValue(currentRow, thisColumn, sumDiffTolalValue.Value);
            //        }
            //    }
            //    return currentColumn + tickers.Count;
            return 0;
        }
        //public int ExportToExcel_Tickers(IWorkSheetWorker wsWorker, List<CalcPortfolioDatum> calcPortData) {

        //    var tickers = GetAllTickersFromCalcPortfolioData(calcPortData);
        //    var exportData = new List<CalcPortfolioDatumExportBlock>();
        //    foreach(var calcData in calcPortData) {
        //        var exportDatum = new CalcPortfolioDatumExportBlock();
        //        exportDatum.Date = calcData.Date;
        //        exportDatum.Names = tickers;
        //        exportDatum.Values = calcData.SumTotalValues;
        //        exportDatum.DiffValues = calcData.SumDiffTotalValues;
        //        exportData.Add(exportDatum);
        //    }
        //    return ExportToExcelSeveralBlocks(wsWorker, exportData, 7, 2);
        //}


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

