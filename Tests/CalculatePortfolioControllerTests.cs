using ComplexPortfolio.Module.BusinessObjects;
using ComplexPortfolio.Module.Controllers;
using ComplexPortfolio.Module.HelpClasses;
using DevExpress.Spreadsheet;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests {
    [TestFixture]
    public class CalculatePortfolioControllerTests {
        [Test]
        public void CalculatePortfolio() {
            //arrange
            var cnt = new CalculatePortfolioController();
            var position1 = new Position();
            var d1 = new DateTime(2021, 8, 19);
            var d2 = new DateTime(2021, 8, 21);
            var d3 = new DateTime(2021, 8, 22);
            var tickerDayData1 = new TickerDayDatum(new Ticker() { Name = "test1" }, d1, 11);
            var tickerDayData2 = new TickerDayDatum(new Ticker() { Name = "test1" }, d2, 22);
            var tickerDayData3 = new TickerDayDatum(new Ticker() { Name = "test2" }, d2, 33);
            var tickerDayData4 = new TickerDayDatum(new Ticker() { Name = "test2" }, d3, 33);
            var calcData11 = new CalcPositionDatum(tickerDayData1);
            var calcData12 = new CalcPositionDatum(tickerDayData2);
            position1.CalculateData = new List<CalcPositionDatum>();
            position1.CalculateData.Add(calcData11);
            position1.CalculateData.Add(calcData12);

            var position2 = new Position();
            var calcData21 = new CalcPositionDatum(tickerDayData3);
            var calcData22 = new CalcPositionDatum(tickerDayData4);
            position2.CalculateData = new List<CalcPositionDatum>();
            position2.CalculateData.Add(calcData21);
            position2.CalculateData.Add(calcData22);

            var positions = new List<Position>();
            positions.Add(position1);
            positions.Add(position2);
            //act
            var res = cnt.CalculatePortfolioDataList(positions);
            //assert
            Assert.AreEqual(3, res.Count);
            Assert.AreEqual(1, res[0].SumTotalValues.Count); //check
            Assert.AreEqual(2, res[1].SumTotalValues.Count);
            Assert.AreEqual(1, res[2].SumTotalValues.Count);
            Assert.AreEqual(d1, res[0].Date);
            Assert.AreEqual(d2, res[1].Date);
            Assert.AreEqual(d3, res[2].Date);
        }

        [Test]
        public void CalculatePortfolio_NoPositionData() {
            //scenario: a position opened today. As we got data for today-1 only - there is no position data for this position
            //arrange
            var cnt = new CalculatePortfolioController();
            var position1 = new Position();
            var d1 = new DateTime(2021, 8, 19);
            var d2 = new DateTime(2021, 8, 21);
            var d3 = new DateTime(2021, 8, 22);
            var tickerDayData1 = new TickerDayDatum(new Ticker() { Name = "test1" }, d1, 11);
            var tickerDayData2 = new TickerDayDatum(new Ticker() { Name = "test1" }, d2, 22);
            var tickerDayData3 = new TickerDayDatum(new Ticker() { Name = "test2" }, d2, 33);
            var tickerDayData4 = new TickerDayDatum(new Ticker() { Name = "test2" }, d3, 33);
            var calcData11 = new CalcPositionDatum(tickerDayData1);
            var calcData12 = new CalcPositionDatum(tickerDayData2);
            position1.CalculateData = new List<CalcPositionDatum>();
            position1.CalculateData.Add(calcData11);
            position1.CalculateData.Add(calcData12);

            var position2 = new Position();
            var calcData21 = new CalcPositionDatum(tickerDayData3);
            var calcData22 = new CalcPositionDatum(tickerDayData4);
            position2.CalculateData = new List<CalcPositionDatum>();
            position2.CalculateData.Add(calcData21);
            position2.CalculateData.Add(calcData22);

            var position3 = new Position();
            position3.CalculateData = new List<CalcPositionDatum>();

            var positions = new List<Position>();
            positions.Add(position1);
            positions.Add(position2);
            positions.Add(position3);
            //act
            //assert
            Assert.DoesNotThrow(()=> cnt.CalculatePortfolioDataList(positions));
           
            
        }

        List<CalcPositionDatum> CreatePlainPositions() {
            var d1 = new DateTime(2020, 8, 20);

            var tickerDayData1 = new TickerDayDatum(new Ticker() { Name = "FXRB" }, d1, 0);
            var position1 = new CalcPositionDatum(tickerDayData1);
            position1.Value = 17670;
            position1.Profit = 0;
            position1.ProfitTotal = 160;

            var tickerDayData2 = new TickerDayDatum(new Ticker() { Name = "FXRL" }, d1, 0);
            var position2 = new CalcPositionDatum(tickerDayData2);
            position2.Value = 18678;
            position2.Profit = -366;
            position2.ProfitTotal = 72;

            var tickerDayData3 = new TickerDayDatum(new Ticker() { Name = "FXGD" }, d1, 0);
            var position3 = new CalcPositionDatum(tickerDayData3);
            position3.Value = 16775.6;
            position3.Profit = 85;
            position3.ProfitTotal = -887.4;


            var list = new List<CalcPositionDatum>();
            list.Add(position1);
            list.Add(position2);
            list.Add(position3);
            return list;
        }

        List<List<CalcPositionDatum>> CreateSeveralDatumWithMissedTickers() {
            var res = new List<List<CalcPositionDatum>>();


            var d1 = new DateTime(2020, 8, 19);

            var tickerDayData1 = new TickerDayDatum(new Ticker() { Name = "FXRB" }, d1, 0);
            var position1 = new CalcPositionDatum(tickerDayData1);
            position1.Value = 111;
            position1.Profit = 0;
            position1.ProfitTotal = 10;

            var tickerDayData2 = new TickerDayDatum(new Ticker() { Name = "FXRL" }, d1, 0);
            var position2 = new CalcPositionDatum(tickerDayData2);
            position2.Value = 222;
            position2.Profit = -366;
            position2.ProfitTotal = 20;


            var r0 = new List<CalcPositionDatum>();
            r0.Add(position1);
            r0.Add(position2);

            res.Add(r0);

            var r1 = CreatePlainPositions();
            res.Add(r1);
            return res;
        }


        [Test]
        public void CalculateSinglePorfolioDatumTest() {
            //arrange
            var cnt = new CalculatePortfolioController();
            var list = CreatePlainPositions();
            //act
            var datum = cnt.CalculateSinglePorfolioDatum(list);
            //assert
            var lst1 = new Dictionary<string, double> { { "FXRB", 17670 }, { "FXRL", 18678 }, { "FXGD", 16775.6 } };
            var lst2 = new Dictionary<string, double> { { "FXRB", 160 }, { "FXRL", 72 }, { "FXGD", -887.4 } };
            var lst3 = new List<String>() { "FXRB", "FXRL", "FXGD" };

            Assert.AreEqual(53123.6, datum.SumTotal);
            Assert.AreEqual(-655.4, datum.SumDiffTotal);
            Assert.AreEqual(lst1, datum.SumTotalValues);
            Assert.AreEqual(lst2, datum.SumDiffTotalValues);
            Assert.AreEqual(lst3, datum.Tickers);

        }
        [Test]
        public void CreateExportBlocksFromPortfolioDatum() {
            //arrange
            var cnt = new CalculatePortfolioController();
            var list = CreatePlainPositions();
            var datum = cnt.CalculateSinglePorfolioDatum(list);
            var calcList = new List<CalcPortfolioDatum>();
            calcList.Add(datum);

            CalcPortfolioDatumExportBlock ticketsBlock = new CalcPortfolioDatumExportBlock("Tickers");
            ticketsBlock.Names = new List<string> { "FXGD", "FXRB", "FXRL" };
            var exportElement = new CalcPortfolioDatumExportElement(new DateTime(2020, 8, 20));
            exportElement.Values = new double?[] { 16775.6, 17670, 18678 };
            exportElement.SumValue = 53123.6;
            ticketsBlock.Elements.Add(exportElement);

            CalcPortfolioDatumExportBlock ticketsDiffBlock = new CalcPortfolioDatumExportBlock("TickersDiff");
            ticketsDiffBlock.Names = new List<string> { "FXGD", "FXRB", "FXRL" };
            var exportElementDiff = new CalcPortfolioDatumExportElement(new DateTime(2020, 8, 20));
            exportElementDiff.Values = new double?[] { -887.4, 160, 72 };
            exportElementDiff.SumValue = -655.4;
            ticketsDiffBlock.Elements.Add(exportElementDiff);


            var expectedRes = new List<CalcPortfolioDatumExportBlock>() { ticketsBlock, ticketsDiffBlock };

            //act
            var res = cnt.CreateExportBlocksFromData(calcList);
            //assert
            Assert.AreEqual(expectedRes, res);
        }

        [Test]
        public void CreateExportBlocksFromPortfolioDatum_Labels() {
            //arrange
            var cnt = new CalculatePortfolioController();

            var datum = new CalcPortfolioDatum(new DateTime(2020, 8, 20));
            datum.SumTotalValues = new Dictionary<string, double>() { { "TSPX", 10 }, { "TMOS", 20 }, { "FXUS", 5 } };
            datum.SumDiffTotalValues = new Dictionary<string, double>() { { "TSPX", 15 }, { "TMOS", 25 }, { "FXUS", 8 } };

            datum.SumTotalValuesLabels = new Dictionary<string, double>() { { "RUS", 20 }, { "US", 15 } };
            datum.SumDiffTotalValuesLabels = new Dictionary<string, double>() { { "RUS", 25 }, { "US", 23 } };

            var calcList = new List<CalcPortfolioDatum>();
            calcList.Add(datum);

            CalcPortfolioDatumExportBlock ticketsBlock = new CalcPortfolioDatumExportBlock("Tickers");
            ticketsBlock.Names = new List<string> { "FXUS", "TMOS", "TSPX" };
            var exportElement = new CalcPortfolioDatumExportElement(new DateTime(2020, 8, 20));
            exportElement.Values = new double?[] { 5, 20, 10 };
            exportElement.SumValue = 35;
            ticketsBlock.Elements.Add(exportElement);

            CalcPortfolioDatumExportBlock ticketsDiffBlock = new CalcPortfolioDatumExportBlock("TickersDiff");
            ticketsDiffBlock.Names = new List<string> { "FXUS", "TMOS", "TSPX" };
            var exportElementDiff = new CalcPortfolioDatumExportElement(new DateTime(2020, 8, 20));
            exportElementDiff.Values = new double?[] { 8, 25, 15 };
            exportElementDiff.SumValue = 48;
            ticketsDiffBlock.Elements.Add(exportElementDiff);

            CalcPortfolioDatumExportBlock labelsBlock = new CalcPortfolioDatumExportBlock("Labels");
            labelsBlock.Names = new List<string> { "RUS", "US" };
            var exportElementLabel = new CalcPortfolioDatumExportElement(new DateTime(2020, 8, 20));
            exportElementLabel.Values = new double?[] { 20, 15 };
            exportElementLabel.SumValue = 35;
            labelsBlock.Elements.Add(exportElementLabel);

            CalcPortfolioDatumExportBlock labelsDiffBlock = new CalcPortfolioDatumExportBlock("LabelsDiff");
            labelsDiffBlock.Names = new List<string> { "RUS", "US" };
            var exportElementLabelDiff = new CalcPortfolioDatumExportElement(new DateTime(2020, 8, 20));
            exportElementLabelDiff.Values = new double?[] { 25, 23 };
            exportElementLabelDiff.SumValue = 48;
            labelsDiffBlock.Elements.Add(exportElementLabelDiff);


            var expectedRes = new List<CalcPortfolioDatumExportBlock>() { ticketsBlock, ticketsDiffBlock, labelsBlock, labelsDiffBlock };

            //act
            var res = cnt.CreateExportBlocksFromData(calcList);
            //assert
            Assert.AreEqual(expectedRes, res);
        }



        [Test]
        public void CreateExportBlocksFromPortfolioDatum_SomeTickersMissed() {
            //arrange
            var cnt = new CalculatePortfolioController();
            var calcList = CreateSeveralDatumWithMissedTickers();

            CalcPortfolioDatumExportBlock ticketsBlock = new CalcPortfolioDatumExportBlock("Tickers");
            ticketsBlock.Names = new List<string> { "FXGD", "FXRB", "FXRL" };

            var exportElement19 = new CalcPortfolioDatumExportElement(new DateTime(2020, 8, 19));
            exportElement19.Values = new double?[] { null, 111, 222 };
            exportElement19.SumValue = 333;

            var exportElement20 = new CalcPortfolioDatumExportElement(new DateTime(2020, 8, 20));
            exportElement20.Values = new double?[] { 16775.6, 17670, 18678 };
            exportElement20.SumValue = 53123.6;

            ticketsBlock.Elements.Add(exportElement19);
            ticketsBlock.Elements.Add(exportElement20);

            CalcPortfolioDatumExportBlock ticketsDiffBlock = new CalcPortfolioDatumExportBlock("TickersDiff");
            ticketsDiffBlock.Names = new List<string> { "FXGD", "FXRB", "FXRL" };

            var exportElementDiff19 = new CalcPortfolioDatumExportElement(new DateTime(2020, 8, 19));
            exportElementDiff19.Values = new double?[] { null, 10, 20 };
            exportElementDiff19.SumValue = 30;

            var exportElementDiff20 = new CalcPortfolioDatumExportElement(new DateTime(2020, 8, 20));
            exportElementDiff20.Values = new double?[] { -887.4, 160, 72 };
            exportElementDiff20.SumValue = -655.4;

            ticketsDiffBlock.Elements.Add(exportElementDiff19);
            ticketsDiffBlock.Elements.Add(exportElementDiff20);


            var expectedRes = new List<CalcPortfolioDatumExportBlock>() { ticketsBlock, ticketsDiffBlock };
            List<CalcPortfolioDatum> list = new List<CalcPortfolioDatum>();
            foreach(var c in calcList) {
                var l = cnt.CalculateSinglePorfolioDatum(c);
                list.Add(l);
            }

            //act
            var res = cnt.CreateExportBlocksFromData(list);
            //assert
            Assert.AreEqual(expectedRes, res);
        }



        [Test]
        public void CalculateSinglePorfolioDatumTest_Labels() {

            //todo: print labels
            //arrange
            var cnt = new CalculatePortfolioController();
            var d1 = new DateTime(2020, 8, 20);

            var tickerDayData1 = new TickerDayDatum(new Ticker() { Name = "VTBX" }, d1, 0);
            var position1 = new CalcPositionDatum(tickerDayData1, "RUS");
            position1.Value = 17670;
            position1.Profit = 0;
            position1.ProfitTotal = 160;

            var tickerDayData2 = new TickerDayDatum(new Ticker() { Name = "TSPX" }, d1, 0);
            var position2 = new CalcPositionDatum(tickerDayData2, "US");
            position2.Value = 18678;
            position2.Profit = -366;
            position2.ProfitTotal = 72;

            var tickerDayData3 = new TickerDayDatum(new Ticker() { Name = "TMOS" }, d1, 0);
            var position3 = new CalcPositionDatum(tickerDayData3, "RUS");
            position3.Value = 16775.6;
            position3.Profit = 85;
            position3.ProfitTotal = -887.4;

            var tickerDayData4 = new TickerDayDatum(new Ticker() { Name = "FXUS" }, d1, 0);
            var position4 = new CalcPositionDatum(tickerDayData4, "US");
            position4.Value = 8459.6;
            position4.Profit = 234;
            position4.ProfitTotal = -568.4;



            var list = new List<CalcPositionDatum>();
            list.Add(position1);
            list.Add(position2);
            list.Add(position3);
            list.Add(position4);
            //act
            var datum = cnt.CalculateSinglePorfolioDatum(list);
            //assert
            var sumTotalValues = new Dictionary<string, double> { { "VTBX", 17670 }, { "TSPX", 18678 }, { "TMOS", 16775.6 }, { "FXUS", 8459.6 } };
            var sumDiffTotalValues = new Dictionary<string, double> { { "VTBX", 160 }, { "TSPX", 72 }, { "TMOS", -887.4 }, { "FXUS", -568.4 } };
            var tickers = new List<String>() { "VTBX", "TSPX", "TMOS", "FXUS" };

            var sumTotalValuesLabels = new Dictionary<string, double> { { "RUS", 34445.6 }, { "US", 27137.6 } };
            var sumDiffTotalValuesLabels = new Dictionary<string, double> { { "RUS", -727.4 }, { "US", -496.4 } };
            var labels = new List<String>() { "RUS", "US" };

            Assert.AreEqual(61583.2, datum.SumTotal);
            Assert.AreEqual(-1223.8, datum.SumDiffTotal);

            Assert.AreEqual(sumTotalValues, datum.SumTotalValues);
            Assert.AreEqual(sumDiffTotalValues, datum.SumDiffTotalValues);
            Assert.AreEqual(tickers, datum.Tickers);

            Assert.AreEqual(sumTotalValuesLabels, datum.SumTotalValuesLabels);
            Assert.AreEqual(sumDiffTotalValuesLabels, datum.SumDiffTotalValuesLabels);
            Assert.AreEqual(labels, datum.Labels);


        }

        [Test]
        public void CalculateSinglePorfolioDatumTest_Labels_NotAllHasLabels() {


            //arrange
            var cnt = new CalculatePortfolioController();
            var d1 = new DateTime(2020, 8, 20);

            var tickerDayData1 = new TickerDayDatum(new Ticker() { Name = "VTBX" }, d1, 0);
            var position1 = new CalcPositionDatum(tickerDayData1, "RUS");
            position1.Value = 17670;
            position1.Profit = 0;
            position1.ProfitTotal = 160;

            var tickerDayData2 = new TickerDayDatum(new Ticker() { Name = "TSPX" }, d1, 0);
            var position2 = new CalcPositionDatum(tickerDayData2);
            position2.Value = 18678;
            position2.Profit = -366;
            position2.ProfitTotal = 72;

            var tickerDayData3 = new TickerDayDatum(new Ticker() { Name = "TMOS" }, d1, 0);
            var position3 = new CalcPositionDatum(tickerDayData3, "RUS");
            position3.Value = 16775.6;
            position3.Profit = 85;
            position3.ProfitTotal = -887.4;

            var tickerDayData4 = new TickerDayDatum(new Ticker() { Name = "FXUS" }, d1, 0);
            var position4 = new CalcPositionDatum(tickerDayData4, "US");
            position4.Value = 8459.6;
            position4.Profit = 234;
            position4.ProfitTotal = -568.4;


            var list = new List<CalcPositionDatum>();
            list.Add(position1);
            list.Add(position2);
            list.Add(position3);
            list.Add(position4);
            //act
            var datum = cnt.CalculateSinglePorfolioDatum(list);
            //assert
            var sumTotalValues = new Dictionary<string, double> { { "VTBX", 17670 }, { "TSPX", 18678 }, { "TMOS", 16775.6 }, { "FXUS", 8459.6 } };
            var sumDiffTotalValues = new Dictionary<string, double> { { "VTBX", 160 }, { "TSPX", 72 }, { "TMOS", -887.4 }, { "FXUS", -568.4 } };
            var tickers = new List<String>() { "VTBX", "TSPX", "TMOS", "FXUS" };

            var sumTotalValuesLabels = new Dictionary<string, double>();
            var sumDiffTotalValuesLabels = new Dictionary<string, double>();
            var labels = new List<String>();

            Assert.AreEqual(61583.2, datum.SumTotal);
            Assert.AreEqual(-1223.8, datum.SumDiffTotal);

            Assert.AreEqual(sumTotalValues, datum.SumTotalValues);
            Assert.AreEqual(sumDiffTotalValues, datum.SumDiffTotalValues);
            Assert.AreEqual(tickers, datum.Tickers);

            Assert.AreEqual(sumTotalValuesLabels, datum.SumTotalValuesLabels);
            Assert.AreEqual(sumDiffTotalValuesLabels, datum.SumDiffTotalValuesLabels);
            Assert.AreEqual(labels, datum.Labels);


        }
        [Test]

        public void ExportToExcel_OneBlock() {
            //arrange
            var cnt = new CalculatePortfolioController();
            var wsWorkerMock = new Mock<IWorkSheetWorker>();
            var exportBlock = new CalcPortfolioDatumExportBlock("TestPrefix");
            exportBlock.Names = new List<string>() { "Test1", "Test2" };
            var el = new CalcPortfolioDatumExportElement(new DateTime(2020, 8, 15));
            el.SumValue = 33;
            el.Values = new double?[] { 11, 22 };

            var el2 = new CalcPortfolioDatumExportElement(new DateTime(2020, 8, 16));
            el2.SumValue = 99;
            el2.Values = new double?[] { 44, 55 };
            exportBlock.Elements.Add(el);
            exportBlock.Elements.Add(el2);
            //act
            var finishColumn = cnt.ExportToExcelOneBlock(wsWorkerMock.Object, exportBlock, 1, 1);
            //assert
            wsWorkerMock.Verify(x => x.SetCellValue(1, 2, "TestPrefix"), Times.Once());
            wsWorkerMock.Verify(x => x.SetCellValue(1, 3, "Test1"), Times.Once());
            wsWorkerMock.Verify(x => x.SetCellValue(1, 4, "Test2"), Times.Once());

            wsWorkerMock.Verify(x => x.SetCellValue(2, 1, new DateTime(2020, 8, 15)), Times.Once());
            wsWorkerMock.Verify(x => x.SetCellValue(2, 2, 33), Times.Once());
            wsWorkerMock.Verify(x => x.SetCellValue(2, 3, 11), Times.Once());
            wsWorkerMock.Verify(x => x.SetCellValue(2, 4, 22), Times.Once());


            wsWorkerMock.Verify(x => x.SetCellValue(3, 1, new DateTime(2020, 8, 16)), Times.Once());
            wsWorkerMock.Verify(x => x.SetCellValue(3, 2, 99), Times.Once());
            wsWorkerMock.Verify(x => x.SetCellValue(3, 3, 44), Times.Once());
            wsWorkerMock.Verify(x => x.SetCellValue(3, 4, 55), Times.Once());

            Assert.AreEqual(4, finishColumn);
        }



        [Test]
        public void ExportToExcel() {
            // arrange
            var cnt = new CalculatePortfolioController();

            var wsWorkerMock = new Mock<IWorkSheetWorker>();

            var d1 = new DateTime(2020, 8, 20);
            var d2 = new DateTime(2020, 8, 21);
            var d3 = new DateTime(2020, 8, 22);


            var data1 = new CalcPortfolioDatum(d1);

            data1.SumTotalValues = new Dictionary<string, double> { { "FXGD", 3 }, { "FXRB", 7 } };
            data1.SumDiffTotalValues = new Dictionary<string, double> { { "FXGD", 10 }, { "FXRB", 5 } };

            var data2 = new CalcPortfolioDatum(d2);
            data2.SumTotalValues = new Dictionary<string, double> { { "FXGD", 10 }, { "FXRB", 4 }, { "FXRL", 6 } };
            data2.SumDiffTotalValues = new Dictionary<string, double> { { "FXGD", 13 }, { "FXRB", 7 }, { "FXRL", 5 } };

            var data3 = new CalcPortfolioDatum(d3);
            data3.SumTotalValues = new Dictionary<string, double> { { "FXRB", 17 }, { "FXRL", 13 } };
            data3.SumDiffTotalValues = new Dictionary<string, double> { { "FXRB", 23 }, { "FXRL", 12 } };

            List<CalcPortfolioDatum> calcData = new List<CalcPortfolioDatum>();
            calcData.Add(data1);
            calcData.Add(data2);
            calcData.Add(data3);





            //act
            cnt.ExportToExcel(wsWorkerMock.Object, calcData);
            //assert
            wsWorkerMock.Verify(x => x.SetCellValue(7, 2, "Tickers"), Times.Once());
            wsWorkerMock.Verify(x => x.SetCellValue(7, 3, "FXGD"), Times.Once());
            wsWorkerMock.Verify(x => x.SetCellValue(7, 4, "FXRB"), Times.Once());
            wsWorkerMock.Verify(x => x.SetCellValue(7, 5, "FXRL"), Times.Once());



            wsWorkerMock.Verify(x => x.SetCellValue(7, 8, "TickersDiff"), Times.Once());
            wsWorkerMock.Verify(x => x.SetCellValue(7, 9, "FXGD"), Times.Once());
            wsWorkerMock.Verify(x => x.SetCellValue(7, 10, "FXRB"), Times.Once());
            wsWorkerMock.Verify(x => x.SetCellValue(7, 11, "FXRL"), Times.Once());

            wsWorkerMock.Verify(x => x.SetCellValue(8, 1, new DateTime(2020, 8, 20)), Times.Once());

            wsWorkerMock.Verify(x => x.SetCellValue(8, 2, 10), Times.Once());
            wsWorkerMock.Verify(x => x.SetCellValue(8, 3, 3), Times.Once());
            wsWorkerMock.Verify(x => x.SetCellValue(8, 4, 7), Times.Once());

            wsWorkerMock.Verify(x => x.SetCellValue(8, 7, new DateTime(2020, 8, 20)), Times.Once());

            wsWorkerMock.Verify(x => x.SetCellValue(8, 8, 15), Times.Once());
            wsWorkerMock.Verify(x => x.SetCellValue(8, 9, 10), Times.Once());
            wsWorkerMock.Verify(x => x.SetCellValue(8, 10, 5), Times.Once());

            wsWorkerMock.Verify(x => x.SetCellValue(9, 1, new DateTime(2020, 8, 21)), Times.Once());

            wsWorkerMock.Verify(x => x.SetCellValue(9, 2, 20), Times.Once());
            wsWorkerMock.Verify(x => x.SetCellValue(9, 3, 10), Times.Once());
            wsWorkerMock.Verify(x => x.SetCellValue(9, 4, 4), Times.Once());
            wsWorkerMock.Verify(x => x.SetCellValue(9, 5, 6), Times.Once());

            wsWorkerMock.Verify(x => x.SetCellValue(9, 7, new DateTime(2020, 8, 21)), Times.Once());

            wsWorkerMock.Verify(x => x.SetCellValue(9, 8, 25), Times.Once());
            wsWorkerMock.Verify(x => x.SetCellValue(9, 9, 13), Times.Once());
            wsWorkerMock.Verify(x => x.SetCellValue(9, 10, 7), Times.Once());
            wsWorkerMock.Verify(x => x.SetCellValue(9, 11, 5), Times.Once());

            wsWorkerMock.Verify(x => x.SetCellValue(10, 1, new DateTime(2020, 8, 22)), Times.Once());

            wsWorkerMock.Verify(x => x.SetCellValue(10, 2, 30), Times.Once());
            wsWorkerMock.Verify(x => x.SetCellValue(10, 4, 17), Times.Once());
            wsWorkerMock.Verify(x => x.SetCellValue(10, 5, 13), Times.Once());

            wsWorkerMock.Verify(x => x.SetCellValue(10, 7, new DateTime(2020, 8, 22)), Times.Once());

            wsWorkerMock.Verify(x => x.SetCellValue(10, 8, 35), Times.Once());
            wsWorkerMock.Verify(x => x.SetCellValue(10, 10, 23), Times.Once());
            wsWorkerMock.Verify(x => x.SetCellValue(10, 11, 12), Times.Once());

        }






        [Test]
        public void GetAllTickersFromCalcPortfolioData() {
            //arrange
            var cnt = new CalculatePortfolioController();
            var expectedRes = new List<string>();
            expectedRes.Add("FXCN");
            expectedRes.Add("FXGD");
            expectedRes.Add("FXRL");
            expectedRes.Add("FXUS");

            var ticker1 = new Ticker() { Name = "FXGD" };
            var ticker2 = new Ticker() { Name = "FXRL" };
            var ticker3 = new Ticker() { Name = "FXUS" };
            var ticker4 = new Ticker() { Name = "FXCN" };



            var t11 = new TickerDayDatum(ticker1, DateTime.Today, 0);
            var p11 = new CalcPositionDatum(t11);
            var list1 = new List<CalcPositionDatum>() { p11 };
            var c1 = cnt.CalculateSinglePorfolioDatum(list1);



            var t21 = new TickerDayDatum(ticker1, DateTime.Today, 0);
            var p21 = new CalcPositionDatum(t21);
            var t22 = new TickerDayDatum(ticker2, DateTime.Today, 0);
            var p22 = new CalcPositionDatum(t22);
            var list2 = new List<CalcPositionDatum>() { p21, p22 };
            var c2 = cnt.CalculateSinglePorfolioDatum(list2);

            var t31 = new TickerDayDatum(ticker2, DateTime.Today, 0);
            var p31 = new CalcPositionDatum(t31);
            var t32 = new TickerDayDatum(ticker3, DateTime.Today, 0);
            var p32 = new CalcPositionDatum(t32);
            var list3 = new List<CalcPositionDatum>() { p31, p32 };
            var c3 = cnt.CalculateSinglePorfolioDatum(list3);

            var t41 = new TickerDayDatum(ticker3, DateTime.Today, 0);
            var p41 = new CalcPositionDatum(t41);
            var t42 = new TickerDayDatum(ticker4, DateTime.Today, 0);
            var p42 = new CalcPositionDatum(t42);
            var list4 = new List<CalcPositionDatum>() { p41, p42 };
            var c4 = cnt.CalculateSinglePorfolioDatum(list4);

            var t51 = new TickerDayDatum(ticker4, DateTime.Today, 0);
            var p51 = new CalcPositionDatum(t51);
            var list5 = new List<CalcPositionDatum>() { p51 };
            var c5 = cnt.CalculateSinglePorfolioDatum(list5);

            var inputList = new List<CalcPortfolioDatum>() { c1, c2, c3, c4, c5 };

            //act
            var res = cnt.GetAllTickersFromCalcPortfolioData(inputList);
            //assert
            Assert.AreEqual(expectedRes, res);

        }

        [Test]
        public void GetAllLabelsFromCalcPortfolioData() {
            //arrange
            var cnt = new CalculatePortfolioController();
            var expectedRes = new List<string>();
            expectedRes.Add("DM");
            expectedRes.Add("RUS");
            expectedRes.Add("US");


            var ticker1 = new Ticker() { Name = "FXUS" };
            var ticker2 = new Ticker() { Name = "FXRL" };
            var ticker3 = new Ticker() { Name = "TSPX" };
            var ticker4 = new Ticker() { Name = "FXCN" };



            var t11 = new TickerDayDatum(ticker1, DateTime.Today, 0);
            var p11 = new CalcPositionDatum(t11, "US");
            var list1 = new List<CalcPositionDatum>() { p11 };
            var c1 = cnt.CalculateSinglePorfolioDatum(list1);


            var t21 = new TickerDayDatum(ticker1, DateTime.Today, 0);
            var p21 = new CalcPositionDatum(t21, "US");
            var t22 = new TickerDayDatum(ticker2, DateTime.Today, 0);
            var p22 = new CalcPositionDatum(t22, "RUS");
            var list2 = new List<CalcPositionDatum>() { p21, p22 };
            var c2 = cnt.CalculateSinglePorfolioDatum(list2);

            var t31 = new TickerDayDatum(ticker2, DateTime.Today, 0);
            var p31 = new CalcPositionDatum(t31, "RUS");
            var t32 = new TickerDayDatum(ticker3, DateTime.Today, 0);
            var p32 = new CalcPositionDatum(t32, "US");
            var list3 = new List<CalcPositionDatum>() { p31, p32 };
            var c3 = cnt.CalculateSinglePorfolioDatum(list3);

            var t41 = new TickerDayDatum(ticker3, DateTime.Today, 0);
            var p41 = new CalcPositionDatum(t41, "US");
            var t42 = new TickerDayDatum(ticker4, DateTime.Today, 0);
            var p42 = new CalcPositionDatum(t42, "DM");
            var list4 = new List<CalcPositionDatum>() { p41, p42 };
            var c4 = cnt.CalculateSinglePorfolioDatum(list4);

            var t51 = new TickerDayDatum(ticker4, DateTime.Today, 0);
            var p51 = new CalcPositionDatum(t51, "DM");
            var list5 = new List<CalcPositionDatum>() { p51 };
            var c5 = cnt.CalculateSinglePorfolioDatum(list5);

            var inputList = new List<CalcPortfolioDatum>() { c1, c2, c3, c4, c5 };

            //act
            var res = cnt.GetAllLabelsFromCalcPortfolioData(inputList);
            //assert
            Assert.AreEqual(expectedRes, res);

        }

        [Test]
        public void CalculatePortfolioSummary() {
            //arrange
            var pos1 = new PositionSummary();
            pos1.InputValue = 20;
            pos1.CurrentValue = 50;
            pos1.TotalProfit = 30;
            
            var pos2 = new PositionSummary();
            pos2.InputValue = 30;
            pos2.CurrentValue = 150;
            pos2.TotalProfit = 120;
            var lst = new List<PositionSummary>() { pos1, pos2 };
            var cnt = new CalculatePortfolioController();
            //act
            var res = cnt.CalculatePortfolioSummary(lst);
            //assert
            Assert.AreEqual(200, res.CurrentValue);
            Assert.AreEqual(150, res.TotalProfit);
            Assert.AreEqual(3, res.TotalProfitPercent);

        }
    }
}
