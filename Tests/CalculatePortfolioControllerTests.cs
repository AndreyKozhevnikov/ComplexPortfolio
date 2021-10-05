using ComplexPortfolio.Module.BusinessObjects;
using ComplexPortfolio.Module.Controllers;
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
            var tickerDayData1 = new TickerDayDatum(new Ticker(), d1, 0);
            var tickerDayData2 = new TickerDayDatum(new Ticker(), d2, 0);
            var tickerDayData3 = new TickerDayDatum(new Ticker(), d3, 0);
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

        Tuple<string, decimal> CreateTuple(string ticker, decimal value) {
            return new Tuple<string, decimal>(ticker, value);
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
            var lst1 = new List<Tuple<string, decimal>>() { CreateTuple("FXRB", 17670), CreateTuple("FXRL", 18678), CreateTuple("FXGD", 16775.6m) };
            var lst2 = new List<Tuple<string, decimal>>() { CreateTuple("FXRB", 160), CreateTuple("FXRL", 72), CreateTuple("FXGD", -887.4m) };
            var lst3 = new List<String>() { "FXRB", "FXRL", "FXGD" };

            Assert.AreEqual(53123.6, datum.SumTotal);
            Assert.AreEqual(-655.4, datum.SumDiffTotal);
            Assert.AreEqual(lst1, datum.SumTotalValues);
            Assert.AreEqual(lst2, datum.SumDiffTotalValues);
            Assert.AreEqual(lst3, datum.Tickers);

        }

        [Test]
        public void ExportToExcel() {
            //arrange
            //var cnt = new CalculatePortfolioController();
            //var d1 = new DateTime(2020, 8, 20);
            //var d2 = new DateTime(2020, 8, 21);
            //var d3 = new DateTime(2020, 8, 22);


            //var data1 = new CalcPortfolioDatum(d1);
            //data1.Tickers = new List<string> { "GD", "RL" };
            //data1.SumTotal = 10;
            //data1.SumTotalValues = new List<decimal> { 3, 7 };
            //data1.SumDiffTotal = 15;
            //data1.SumDiffTotalValues = new List<decimal> { 10, 5 };

            //var data2 = new CalcPortfolioDatum(d1);
            //data2.Tickers = new List<string> { "GD", "RL", "RB" };
            //data2.SumTotal = 20;
            //data2.SumTotalValues = new List<decimal> { 10, 4, 6 };
            //data2.SumDiffTotal = 25;
            //data2.SumDiffTotalValues = new List<decimal> { 13, 7, 5 };

            //var data3 = new CalcPortfolioDatum(d1);
            //data3.Tickers = new List<string> {  "RL","RB" };
            //data3.SumTotal = 30;
            //data3.SumTotalValues = new List<decimal> { 17, 13 };
            //data3.SumDiffTotal = 35;
            //data3.SumDiffTotalValues = new List<decimal> { 23,12 };

            List<CalcPortfolioDatum> calcData = new List<CalcPortfolioDatum>();

            //act

            //assert
        }
    }
}
