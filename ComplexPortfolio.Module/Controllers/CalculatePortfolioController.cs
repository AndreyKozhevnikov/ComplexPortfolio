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
    public class CalculatePortfolioController : ViewController {
        public CalculatePortfolioController() {
            TargetObjectType = typeof(Portfolio);
            var exportToExcelAction = new SimpleAction(this, "ExportToExcel", PredefinedCategory.Edit);
            exportToExcelAction.SelectionDependencyType = SelectionDependencyType.RequireMultipleObjects;
            exportToExcelAction.Execute += ExportToExcelAction_Execute;

            var calcPosition = new SimpleAction(this, "calcPosition", PredefinedCategory.Edit);
            calcPosition.SelectionDependencyType = SelectionDependencyType.RequireMultipleObjects;
            calcPosition.Execute += CalcPosition_Execute;
        }

        private void CalcPosition_Execute(object sender, SimpleActionExecuteEventArgs e) {
            var os = Application.CreateObjectSpace(typeof(Portfolio));
            foreach(var port in View.SelectedObjects) {
                CalculatePositionsForPortfolio((Portfolio)port, os);
            }
        }

        void CalculatePositionsForPortfolio(Portfolio port, IObjectSpace os) {
            var cnt = new CalculatePositionController();
            foreach(var position in port.Positions) {
                cnt.CalculatePosition(position, os);
            }
        }

        public List<CalcPortfolioDatum> CalculatePortfolioDataList(List<Position> positions) {
            DateTime startDate = DateTime.Today;
            DateTime finishDate = DateTime.MinValue;
            var positionsWithData = positions.Where(x => x.CalculateData.Count > 0);
            foreach(var p in positionsWithData) {

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
                foreach(var p in positionsWithData) {
                    var positionData = p.CalculateData.Where(x => x.Date == startDate).FirstOrDefault();
                    if(positionData != null) {
                        tmpList.Add(positionData);
                    }
                }
                if(tmpList.Count > 0) {
                    var portfolioData = CalculateSinglePorfolioDatum(tmpList);
                    result.Add(portfolioData);
                }
                startDate = startDate.AddDays(1);
            }
            return result;
        }

        public CalcPortfolioDatum CalculateSinglePorfolioDatum(List<CalcPositionDatum> positionList) {
            CalcPortfolioDatum datum = new CalcPortfolioDatum(positionList[0].Date);
            datum.SumTotalValues = new Dictionary<string, double>();
            datum.SumDiffTotalValues = new Dictionary<string, double>();
            datum.SumTotalValuesLabels = new Dictionary<string, double>();
            datum.SumDiffTotalValuesLabels = new Dictionary<string, double>();
            bool hasLabels = true;
            foreach(var p in positionList) {
                datum.SumTotalValues[p.TickerName] = p.Value;
                datum.SumDiffTotalValues[p.TickerName] = p.ProfitTotal;

                if(string.IsNullOrEmpty(p.Label) || !hasLabels) {
                    hasLabels = false;
                    datum.SumTotalValuesLabels.Clear();
                    datum.SumDiffTotalValuesLabels.Clear();
                    continue;
                }
                if(datum.SumTotalValuesLabels.ContainsKey(p.Label)) {
                    datum.SumTotalValuesLabels[p.Label] += p.Value;
                    datum.SumDiffTotalValuesLabels[p.Label] += p.ProfitTotal;
                } else {
                    datum.SumTotalValuesLabels[p.Label] = p.Value;
                    datum.SumDiffTotalValuesLabels[p.Label] = p.ProfitTotal;
                }
            }
            return datum;
        }
        public List<CalcPortfolioDatumExportBlock> CreateExportBlocksFromData(List<CalcPortfolioDatum> calcPortData) {

            var tickerNames = GetAllTickersFromCalcPortfolioData(calcPortData);
            var labelNames = GetAllLabelsFromCalcPortfolioData(calcPortData);

            var tickersBlock = new CalcPortfolioDatumExportBlock("Tickers");
            tickersBlock.Names = tickerNames;
            var tickersDiffBlock = new CalcPortfolioDatumExportBlock("TickersDiff");
            tickersDiffBlock.Names = tickerNames;

            var labelsBlock = new CalcPortfolioDatumExportBlock("Labels");
            labelsBlock.Names = labelNames;
            var labelsDiffBlock = new CalcPortfolioDatumExportBlock("LabelsDiff");
            labelsDiffBlock.Names = labelNames;

            bool hasLabels = labelNames.Count > 0;

            foreach(var calcData in calcPortData) {
                var tickerElement = new CalcPortfolioDatumExportElement(calcData.Date, tickerNames.Count);

                tickerElement.SumValue = calcData.SumTotal;
                foreach(var position in calcData.SumTotalValues) {
                    var ind = tickerNames.IndexOf(position.Key);
                    tickerElement.Values[ind] = position.Value;
                }
                tickersBlock.Elements.Add(tickerElement);

                var tickerDiffElement = new CalcPortfolioDatumExportElement(calcData.Date, tickerNames.Count);
                tickerDiffElement.SumValue = calcData.SumDiffTotal;
                foreach(var position in calcData.SumDiffTotalValues) {
                    var ind = tickerNames.IndexOf(position.Key);
                    tickerDiffElement.Values[ind] = position.Value;
                }
                tickersDiffBlock.Elements.Add(tickerDiffElement);

                if(hasLabels) {
                    var labelElement = new CalcPortfolioDatumExportElement(calcData.Date, labelNames.Count);
                    labelElement.SumValue = calcData.SumTotal;
                    foreach(var position in calcData.SumTotalValuesLabels) {
                        var ind = labelNames.IndexOf(position.Key);
                        labelElement.Values[ind] = position.Value;
                    }
                    labelsBlock.Elements.Add(labelElement);

                    var labelDiffElement = new CalcPortfolioDatumExportElement(calcData.Date, labelNames.Count);
                    labelDiffElement.SumValue = calcData.SumDiffTotal;
                    foreach(var position in calcData.SumDiffTotalValuesLabels) {
                        var ind = labelNames.IndexOf(position.Key);
                        labelDiffElement.Values[ind] = position.Value;
                    }
                    labelsDiffBlock.Elements.Add(labelDiffElement);
                }
            }
            var result = new List<CalcPortfolioDatumExportBlock>();
            result.Add(tickersBlock);
            result.Add(tickersDiffBlock);
            if(hasLabels) {
                result.Add(labelsBlock);
                result.Add(labelsDiffBlock);
            }
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
                c = ExportToExcelOneBlock(wsWorker, block, c, 7);
                c += 2;
            }
        }



        public int ExportToExcelOneBlock(IWorkSheetWorker wsWorker, CalcPortfolioDatumExportBlock block, int startColumn, int startRow) {
            var currentColumn = startColumn + 1;
            var currentRow = startRow;
            wsWorker.SetCellValue(currentRow, currentColumn, block.Prefix);
            currentColumn++;
            foreach(var name in block.Names) {
                wsWorker.SetCellValue(currentRow, currentColumn, name);
                currentColumn++;
            }


            foreach(var el in block.Elements) {
                currentColumn = startColumn;
                currentRow++;
                wsWorker.SetCellValue(currentRow, currentColumn, el.Date);
                currentColumn++;
                wsWorker.SetCellValue(currentRow, currentColumn, el.SumValue);
                foreach(var val in el.Values) {
                    currentColumn++;
                    wsWorker.SetCellValue(currentRow, currentColumn, val);
                }
            }
            return currentColumn;
        }

        void ExportPortfolio(Portfolio port, IObjectSpace os) {
            CalculatePositionsForPortfolio(port, os);
            var resultToPrint = CalculatePortfolioDataList(port.Positions.ToList());
            using(Workbook workbook = new Workbook()) {

                var wsWorker = new WorkSheetWorker();
                wsWorker.CurrentWorkSheet = workbook.Worksheets[0];
                workbook.BeginUpdate();
                ExportToExcel(wsWorker, resultToPrint);

                workbook.EndUpdate();
                string date = DateTime.Now.ToString("yyyy_MM_dd");
                string fileName = string.Format(@"c:\temp\{0}-{1}.xlsx", port.Name, date);
                workbook.SaveDocument(fileName, DocumentFormat.Xlsx);
            }
        }

        private void ExportToExcelAction_Execute(object sender, SimpleActionExecuteEventArgs e) {
            var os = Application.CreateObjectSpace(typeof(Portfolio));
            foreach(var port in View.SelectedObjects) {
                ExportPortfolio((Portfolio)port, os);
            }
        }
    }
}

