﻿using ComplexPortfolio.Module.BusinessObjects;
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
            var tickerDayData1 = new TickerDayDatum(new Ticker() { Name = "test1" }, d1, 0);
            var tickerDayData2 = new TickerDayDatum(new Ticker() { Name = "test1" }, d2, 0);
            var tickerDayData3 = new TickerDayDatum(new Ticker() { Name = "test1" }, d3, 0);
            var calcData11 = new CalcPositionDatum(tickerDayData1);
            var calcData12 = new CalcPositionDatum(tickerDayData2);
            position1.CalculateData = new List<CalcPositionDatum>();
            position1.CalculateData.Add(calcData11);
            position1.CalculateData.Add(calcData12);

            var position2 = new Position();
            var calcData21 = new CalcPositionDatum(tickerDayData2);
            var calcData22 = new CalcPositionDatum(tickerDayData3);
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
            Assert.AreEqual(1, res[0].PositionData.Count);
            Assert.AreEqual(2, res[1].PositionData.Count);
            Assert.AreEqual(1, res[2].PositionData.Count);
            Assert.AreEqual(d1, res[0].Date);
            Assert.AreEqual(d2, res[1].Date);
            Assert.AreEqual(d3, res[2].Date);
        }

        [Test]
        public void CalculateSinglePorfolioDatumTest() {
            //arrange
            var cnt = new CalculatePortfolioController();
            var d1 = new DateTime(2020, 8, 20);

            var tickerDayData1 = new TickerDayDatum(new Ticker() { Name = "FXRB" }, d1, 1767);
            var position1 = new CalcPositionDatum(tickerDayData1);
            position1.SharesCount = 10;
            position1.Value = 17670;
            position1.ValueDiff = 0;
            position1.ValueDiffTotal = 160;

            var tickerDayData2 = new TickerDayDatum(new Ticker() { Name = "FXRL" }, d1, 3113);
            var position2 = new CalcPositionDatum(tickerDayData2);
            position2.SharesCount = 6;
            position2.Value = 18678;
            position2.ValueDiff = -366;
            position2.ValueDiffTotal = 72;

            var tickerDayData3 = new TickerDayDatum(new Ticker() { Name = "FXGD" }, d1, 986.8m);
            var position3 = new CalcPositionDatum(tickerDayData3);
            position3.SharesCount = 17;
            position3.Value = 16775.6m;
            position3.ValueDiff = 85;
            position3.ValueDiffTotal = -887.4m;

            var datum = new CalcPortfolioDatum(d1);
            datum.PositionData = new List<CalcPositionDatum>();
            datum.PositionData.Add(position1);
            datum.PositionData.Add(position2);
            datum.PositionData.Add(position3);
            //act
            cnt.CalculateSinglePorfolioDatum(datum);
            //assert
            var lst1 = new Dictionary<string, decimal> { { "FXRB", 17670 }, { "FXRL", 18678 }, { "FXGD", 16775.6m } };
            var lst2 = new Dictionary<string, decimal> { { "FXRB", 160 }, { "FXRL", 72 }, { "FXGD", -887.4m } };
            var lst3 = new List<String>() { "FXRB", "FXRL", "FXGD" };

            Assert.AreEqual(53123.6, datum.SumTotal);
            Assert.AreEqual(-655.4, datum.SumDiffTotal);
            Assert.AreEqual(lst1, datum.SumTotalValues);
            Assert.AreEqual(lst2, datum.SumDiffTotalValues);
            Assert.AreEqual(lst3, datum.Tickers);

        }

        [Test]
        public void ExportToExcel() {
            // arrange
            var cnt = new CalculatePortfolioController();

            var wsWorkerMock = new Mock<IWorkSheetWorker>(MockBehavior.Strict);
            wsWorkerMock.Setup(x => x.SetCellValue(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CellValue>()));

            var d1 = new DateTime(2020, 8, 20);
            var d2 = new DateTime(2020, 8, 21);
            var d3 = new DateTime(2020, 8, 22);


            var data1 = new CalcPortfolioDatum(d1);

            data1.SumTotalValues = new Dictionary<string, decimal> { { "FXGD", 3 }, { "FXRL", 7 } };
            data1.SumDiffTotalValues = new Dictionary<string, decimal> { { "FXGD", 10 }, { "FXRL", 5 } };

            var data2 = new CalcPortfolioDatum(d1);
            data2.SumTotalValues = new Dictionary<string, decimal> { { "FXGD", 10 }, { "FXRL", 4 }, { "FXRB", 6 } };
            data2.SumDiffTotalValues = new Dictionary<string, decimal> { { "FXGD", 13 }, { "FXRL", 7 }, { "FXRB", 5 } };

            var data3 = new CalcPortfolioDatum(d1);
            data3.SumTotalValues = new Dictionary<string, decimal> { { "FXRL", 17 }, { "FXRB", 13 } };
            data3.SumDiffTotalValues = new Dictionary<string, decimal> { { "FXRL", 23 }, { "FXRB", 12 } };

            List<CalcPortfolioDatum> calcData = new List<CalcPortfolioDatum>();
            calcData.Add(data1);
            calcData.Add(data1);
            calcData.Add(data1);





            //act
            cnt.ExportToExcel(wsWorkerMock.Object, calcData);
            //assert
            wsWorkerMock.Verify(x => x.SetCellValue(7, 2, "SumTotal"), Times.Once());
            wsWorkerMock.Verify(x => x.SetCellValue(7, 3, "FXGD"), Times.Once());
            wsWorkerMock.Verify(x => x.SetCellValue(7, 4, "FXRL"), Times.Once());
            wsWorkerMock.Verify(x => x.SetCellValue(7, 5, "FXRB"), Times.Once());

            wsWorkerMock.Verify(x => x.SetCellValue(7, 7, "SumDiffTotal"), Times.Once());
            wsWorkerMock.Verify(x => x.SetCellValue(7, 8, "FXGD"), Times.Once());
            wsWorkerMock.Verify(x => x.SetCellValue(7, 9, "FXRL"), Times.Once());
            wsWorkerMock.Verify(x => x.SetCellValue(7, 10, "FXRB"), Times.Once());

            wsWorkerMock.Verify(x => x.SetCellValue(8, 1, new DateTime(2020, 8, 20)), Times.Once());

            wsWorkerMock.Verify(x => x.SetCellValue(8, 2, 10), Times.Once());
            wsWorkerMock.Verify(x => x.SetCellValue(8, 3, 3), Times.Once());
            wsWorkerMock.Verify(x => x.SetCellValue(8, 4, 7), Times.Once());

            wsWorkerMock.Verify(x => x.SetCellValue(8, 7, 15), Times.Once());
            wsWorkerMock.Verify(x => x.SetCellValue(8, 8, 10), Times.Once());
            wsWorkerMock.Verify(x => x.SetCellValue(8, 9, 5), Times.Once());

            wsWorkerMock.Verify(x => x.SetCellValue(9, 1, new DateTime(2020, 8, 21)), Times.Once());

            wsWorkerMock.Verify(x => x.SetCellValue(9, 2, 20), Times.Once());
            wsWorkerMock.Verify(x => x.SetCellValue(9, 3, 10), Times.Once());
            wsWorkerMock.Verify(x => x.SetCellValue(9, 4, 4), Times.Once());
            wsWorkerMock.Verify(x => x.SetCellValue(9, 5, 6), Times.Once());

            wsWorkerMock.Verify(x => x.SetCellValue(9, 7, 25), Times.Once());
            wsWorkerMock.Verify(x => x.SetCellValue(9, 8, 13), Times.Once());
            wsWorkerMock.Verify(x => x.SetCellValue(9, 9, 7), Times.Once());
            wsWorkerMock.Verify(x => x.SetCellValue(9, 10, 5), Times.Once());

            wsWorkerMock.Verify(x => x.SetCellValue(10, 1, new DateTime(2020, 8, 22)), Times.Once());

            wsWorkerMock.Verify(x => x.SetCellValue(10, 2, 20), Times.Once());
            wsWorkerMock.Verify(x => x.SetCellValue(10, 4, 17), Times.Once());
            wsWorkerMock.Verify(x => x.SetCellValue(10, 5, 13), Times.Once());

            wsWorkerMock.Verify(x => x.SetCellValue(10, 7, 35), Times.Once());
            wsWorkerMock.Verify(x => x.SetCellValue(10, 9, 23), Times.Once());
            wsWorkerMock.Verify(x => x.SetCellValue(10, 10, 12), Times.Once());

        }
        [Test]
        public void GetAllTickersFromCalcPortfolioData() {
            //arrange
            var cnt = new CalculatePortfolioController();
            var expectedRes = new Dictionary<string, decimal>();
            expectedRes.Add("FXCN", 0);
            expectedRes.Add("FXGD", 0);
            expectedRes.Add("FXRL", 0);
            expectedRes.Add("FXUS", 0);

            var ticker1 = new Ticker() { Name = "FXGD" };
            var ticker2 = new Ticker() { Name = "FXRL" };
            var ticker3 = new Ticker() { Name = "FXUS" };
            var ticker4 = new Ticker() { Name = "FXCN" };



            var c1 = new CalcPortfolioDatum(DateTime.Now);
            var t11 = new TickerDayDatum(ticker1, DateTime.Today, 0);
            var p11 = new CalcPositionDatum(t11);
            c1.PositionData = new List<CalcPositionDatum>() { p11 };
            cnt.CalculateSinglePorfolioDatum(c1);


            var c2 = new CalcPortfolioDatum(DateTime.Now);
            var t21 = new TickerDayDatum(ticker1, DateTime.Today, 0);
            var p21 = new CalcPositionDatum(t21);
            var t22 = new TickerDayDatum(ticker2, DateTime.Today, 0);
            var p22 = new CalcPositionDatum(t22);
            c2.PositionData = new List<CalcPositionDatum>() { p21, p22 };
            cnt.CalculateSinglePorfolioDatum(c2);

            var c3 = new CalcPortfolioDatum(DateTime.Now);
            var t31 = new TickerDayDatum(ticker2, DateTime.Today, 0);
            var p31 = new CalcPositionDatum(t31);
            var t32 = new TickerDayDatum(ticker3, DateTime.Today, 0);
            var p32 = new CalcPositionDatum(t32);
            c3.PositionData = new List<CalcPositionDatum>() { p31, p32 };
            cnt.CalculateSinglePorfolioDatum(c3);

            var c4 = new CalcPortfolioDatum(DateTime.Now);
            var t41 = new TickerDayDatum(ticker3, DateTime.Today, 0);
            var p41 = new CalcPositionDatum(t41);
            var t42 = new TickerDayDatum(ticker4, DateTime.Today, 0);
            var p42 = new CalcPositionDatum(t42);
            c4.PositionData = new List<CalcPositionDatum>() { p41, p42 };
            cnt.CalculateSinglePorfolioDatum(c4);

            var c5 = new CalcPortfolioDatum(DateTime.Now);
            var t51 = new TickerDayDatum(ticker4, DateTime.Today, 0);
            var p51 = new CalcPositionDatum(t51);
            c5.PositionData = new List<CalcPositionDatum>() { p51 };
            cnt.CalculateSinglePorfolioDatum(c5);

            var inputList = new List<CalcPortfolioDatum>() { c1, c2, c3, c4, c5 };

         //   var resultToPrint = cnt.CalculatePortfolioDataList(portfolio.Positions.ToList());
            //act
            var res = cnt.GetAllTickersFromCalcPortfolioData(inputList);
            //assert
            Assert.AreEqual(expectedRes, res);

        }
    }
}
