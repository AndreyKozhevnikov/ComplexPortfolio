using ComplexPortfolio.Module.BusinessObjects;
using ComplexPortfolio.Module.Controllers;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests {
    [TestFixture]
    public class CalculatePositionControllerTests {
        [Test]
        public void CalculatePosition() {
            //arrange
            var cnt = new CalculatePositionController();
            var pos = new Position();
            pos.Label = "TestLabel";
            var ticker = new Ticker() { Name = "FXRL" };
            pos.Ticker = ticker;
            var trans1 = new Transaction(new DateTime(2020, 8, 18), 5, 3190, TransactionDirectionEnum.Buy);
            var trans2 = new Transaction(new DateTime(2020, 8, 18), 1, 3195, TransactionDirectionEnum.Buy);
            var trans3 = new Transaction(new DateTime(2020, 8, 19), 1, 3160, TransactionDirectionEnum.Buy);
            pos.Transactions.Add(trans1);
            pos.Transactions.Add(trans2);
            pos.Transactions.Add(trans3);

            var osMock = new Mock<IObjectSpace>();

            var dayDataList = new List<TickerDayDatum>();
            dayDataList.Add(new TickerDayDatum(ticker, new DateTime(2020, 8, 17), 3180));
            dayDataList.Add(new TickerDayDatum(ticker, new DateTime(2020, 8, 18), 3184));
            dayDataList.Add(new TickerDayDatum(ticker, new DateTime(2020, 8, 19), 3157.5));
            dayDataList.Add(new TickerDayDatum(ticker, new DateTime(2020, 8, 20), 3155));
            dayDataList.Add(new TickerDayDatum(ticker, new DateTime(2020, 8, 22), 3180));

            osMock.Setup(x => x.GetObjects<TickerDayDatum>(new BinaryOperator("Ticker.Name", "FXRL"))).Returns(dayDataList);

            //act
            cnt.CalculatePosition(pos, osMock.Object);
            //assert
            Assert.AreEqual(4, pos.CalculateData.Count);
            Assert.AreEqual(7, pos.CalculateData[3].SharesCount);
            Assert.AreEqual(3180, pos.CalculateData[3].Price);
            Assert.AreEqual(22260, pos.CalculateData[3].Value);
            Assert.AreEqual(175, pos.CalculateData[3].Profit);
            Assert.AreEqual(-45, pos.CalculateData[3].ProfitTotal);
            Assert.AreEqual("TestLabel", pos.CalculateData[3].Label);

        }

        [Test]
        public void CalculatePosition_sell() {
            //arrange
            var cnt = new CalculatePositionController();
            var pos = new Position();
            pos.Label = "TestLabel";
            var ticker = new Ticker() { Name = "FXRL" };
            pos.Ticker = ticker;
            var trans1 = new Transaction(new DateTime(2021, 11, 16), 1, 98, TransactionDirectionEnum.Buy);
            var trans2 = new Transaction(new DateTime(2021, 11, 18), 2, 117, TransactionDirectionEnum.Buy);
            var trans3 = new Transaction(new DateTime(2021, 11, 19), 1, 134, TransactionDirectionEnum.Sell );
            var trans4 = new Transaction(new DateTime(2021, 11, 20), 2, 138, TransactionDirectionEnum.Sell);
            pos.Transactions.Add(trans1);
            pos.Transactions.Add(trans2);
            pos.Transactions.Add(trans3);
            pos.Transactions.Add(trans4);

            var osMock = new Mock<IObjectSpace>();

            var dayDataList = new List<TickerDayDatum>();
            dayDataList.Add(new TickerDayDatum(ticker, new DateTime(2021, 11, 15), 90));
            dayDataList.Add(new TickerDayDatum(ticker, new DateTime(2021, 11, 16), 100));
            dayDataList.Add(new TickerDayDatum(ticker, new DateTime(2021, 11, 17), 110));
            dayDataList.Add(new TickerDayDatum(ticker, new DateTime(2021, 11, 18), 120));
            dayDataList.Add(new TickerDayDatum(ticker, new DateTime(2021, 11, 19), 130));
            dayDataList.Add(new TickerDayDatum(ticker, new DateTime(2021, 11, 20), 140));
            dayDataList.Add(new TickerDayDatum(ticker, new DateTime(2021, 11, 21), 150));

            osMock.Setup(x => x.GetObjects<TickerDayDatum>(new BinaryOperator("Ticker.Name", "FXRL"))).Returns(dayDataList);

            //act
            cnt.CalculatePosition(pos, osMock.Object);
            //assert
            Assert.AreEqual(6, pos.CalculateData.Count);
            
            Assert.AreEqual(1, pos.CalculateData[0].SharesCount);
            Assert.AreEqual(100, pos.CalculateData[0].Value);
            Assert.AreEqual(2, pos.CalculateData[0].Profit);
            Assert.AreEqual(2, pos.CalculateData[0].ProfitTotal);

            Assert.AreEqual(1, pos.CalculateData[1].SharesCount);
            Assert.AreEqual(110, pos.CalculateData[1].Value);
            Assert.AreEqual(10, pos.CalculateData[1].Profit);
            Assert.AreEqual(12, pos.CalculateData[1].ProfitTotal);

            Assert.AreEqual(3, pos.CalculateData[2].SharesCount);
            Assert.AreEqual(360, pos.CalculateData[2].Value);
            Assert.AreEqual(16, pos.CalculateData[2].Profit);
            Assert.AreEqual(28, pos.CalculateData[2].ProfitTotal);

            Assert.AreEqual(2, pos.CalculateData[3].SharesCount);
            Assert.AreEqual(260, pos.CalculateData[3].Value);
            Assert.AreEqual(34, pos.CalculateData[3].Profit);
            Assert.AreEqual(62, pos.CalculateData[3].ProfitTotal);

            Assert.AreEqual(0, pos.CalculateData[4].SharesCount);
            Assert.AreEqual(0, pos.CalculateData[4].Value);
            Assert.AreEqual(16, pos.CalculateData[4].Profit);
            Assert.AreEqual(78, pos.CalculateData[4].ProfitTotal);



            Assert.AreEqual("TestLabel", pos.CalculateData[3].Label);

        }
        [Test]
        public void CalculatePosition_sellv2() {
            //arrange
            var cnt = new CalculatePositionController();
            var pos = new Position();
            pos.Label = "TestLabel";
            var ticker = new Ticker() { Name = "FXRL" };
            pos.Ticker = ticker;
            var trans1 = new Transaction(new DateTime(2021, 11, 16), 3, 10, TransactionDirectionEnum.Buy);
            var trans2 = new Transaction(new DateTime(2021, 11, 17), 1, 14, TransactionDirectionEnum.Buy);
            var trans3 = new Transaction(new DateTime(2021, 11, 20), 1, 28, TransactionDirectionEnum.Sell);
            pos.Transactions.Add(trans1);
            pos.Transactions.Add(trans2);
            pos.Transactions.Add(trans3);

            var osMock = new Mock<IObjectSpace>();

            var dayDataList = new List<TickerDayDatum>();
            dayDataList.Add(new TickerDayDatum(ticker, new DateTime(2021, 11, 15), 5));
            dayDataList.Add(new TickerDayDatum(ticker, new DateTime(2021, 11, 16), 10));
            dayDataList.Add(new TickerDayDatum(ticker, new DateTime(2021, 11, 17), 15));
            dayDataList.Add(new TickerDayDatum(ticker, new DateTime(2021, 11, 18), 20));
            dayDataList.Add(new TickerDayDatum(ticker, new DateTime(2021, 11, 19), 17));
            dayDataList.Add(new TickerDayDatum(ticker, new DateTime(2021, 11, 20), 25));
            dayDataList.Add(new TickerDayDatum(ticker, new DateTime(2021, 11, 21), 20));

            osMock.Setup(x => x.GetObjects<TickerDayDatum>(new BinaryOperator("Ticker.Name", "FXRL"))).Returns(dayDataList);

            //act
            cnt.CalculatePosition(pos, osMock.Object);
            //assert
            Assert.AreEqual(6, pos.CalculateData.Count);
            Assert.AreEqual(3, pos.CalculateData[0].SharesCount);
            Assert.AreEqual(30, pos.CalculateData[0].Value);
            Assert.AreEqual(0, pos.CalculateData[0].Profit);
            Assert.AreEqual(0, pos.CalculateData[0].ProfitTotal);

            Assert.AreEqual(4, pos.CalculateData[1].SharesCount);
            Assert.AreEqual(60, pos.CalculateData[1].Value);
            Assert.AreEqual(16, pos.CalculateData[1].Profit);
            Assert.AreEqual(16, pos.CalculateData[1].ProfitTotal);

            Assert.AreEqual(4, pos.CalculateData[2].SharesCount);
            Assert.AreEqual(80, pos.CalculateData[2].Value);
            Assert.AreEqual(20, pos.CalculateData[2].Profit);
            Assert.AreEqual(36, pos.CalculateData[2].ProfitTotal);

            Assert.AreEqual(4, pos.CalculateData[3].SharesCount);
            Assert.AreEqual(68, pos.CalculateData[3].Value);
            Assert.AreEqual(-12, pos.CalculateData[3].Profit);
            Assert.AreEqual(24, pos.CalculateData[3].ProfitTotal);

            Assert.AreEqual(3, pos.CalculateData[4].SharesCount);
            Assert.AreEqual(75, pos.CalculateData[4].Value);
            Assert.AreEqual(35, pos.CalculateData[4].Profit);
            Assert.AreEqual(59, pos.CalculateData[4].ProfitTotal);

            Assert.AreEqual(3, pos.CalculateData[5].SharesCount);
            Assert.AreEqual(60, pos.CalculateData[5].Value);
            Assert.AreEqual(-15, pos.CalculateData[5].Profit);
            Assert.AreEqual(44, pos.CalculateData[5].ProfitTotal);

        }

        [Test]
        public void CalculatePosition_sellv3() {
            //arrange
            var cnt = new CalculatePositionController();
            var pos = new Position();
            pos.Label = "TestLabel";
            var ticker = new Ticker() { Name = "FXRL" };
            pos.Ticker = ticker;
            var trans0 = new Transaction(new DateTime(2021, 11, 15), 3, 5, TransactionDirectionEnum.Buy);
            var trans1 = new Transaction(new DateTime(2021, 11, 16), 1, 8, TransactionDirectionEnum.Buy);
            var trans2 = new Transaction(new DateTime(2021, 11, 16), 2, 7, TransactionDirectionEnum.Sell);
            pos.Transactions.Add(trans0);
            pos.Transactions.Add(trans1);
            pos.Transactions.Add(trans2);

            var osMock = new Mock<IObjectSpace>();

            var dayDataList = new List<TickerDayDatum>();
            dayDataList.Add(new TickerDayDatum(ticker, new DateTime(2021, 11, 15), 5));
            dayDataList.Add(new TickerDayDatum(ticker, new DateTime(2021, 11, 16), 10));

            osMock.Setup(x => x.GetObjects<TickerDayDatum>(new BinaryOperator("Ticker.Name", "FXRL"))).Returns(dayDataList);

            //act
            cnt.CalculatePosition(pos, osMock.Object);
            //assert
            Assert.AreEqual(2, pos.CalculateData.Count);

            Assert.AreEqual(2, pos.CalculateData[1].SharesCount);
            Assert.AreEqual(20, pos.CalculateData[1].Value);
            Assert.AreEqual(11, pos.CalculateData[1].Profit);
            Assert.AreEqual(11, pos.CalculateData[1].ProfitTotal);
        }


        [Test]
        public void CalculatePosition_US() {
            //arrange
            var cnt = new CalculatePositionController();
            var pos = new Position();
            var usTicker = new Ticker() { Name = "US", IsCurrency = true };
            var ticker = new Ticker() { Name = "TSPX", Currency = usTicker };
            pos.Ticker = ticker;
            var trans1 = new Transaction(new DateTime(2020, 8, 18), 5, 100, TransactionDirectionEnum.Buy);

            pos.Transactions.Add(trans1);


            var osMock = new Mock<IObjectSpace>();

            var dayDataList = new List<TickerDayDatum>();
            dayDataList.Add(new TickerDayDatum(ticker, new DateTime(2020, 8, 17), 95));
            dayDataList.Add(new TickerDayDatum(ticker, new DateTime(2020, 8, 18), 110));
            dayDataList.Add(new TickerDayDatum(ticker, new DateTime(2020, 8, 19), 120));
            dayDataList.Add(new TickerDayDatum(ticker, new DateTime(2020, 8, 20), 130));
            dayDataList.Add(new TickerDayDatum(ticker, new DateTime(2020, 8, 22), 140));

            var usDayDataList = new List<TickerDayDatum>();
            usDayDataList.Add(new TickerDayDatum(usTicker, new DateTime(2020, 8, 17), 50));
            usDayDataList.Add(new TickerDayDatum(usTicker, new DateTime(2020, 8, 18), 60));
            usDayDataList.Add(new TickerDayDatum(usTicker, new DateTime(2020, 8, 19), 70));
            usDayDataList.Add(new TickerDayDatum(usTicker, new DateTime(2020, 8, 20), 80));
            usDayDataList.Add(new TickerDayDatum(usTicker, new DateTime(2020, 8, 22), 90));

            osMock.Setup(x => x.GetObjects<TickerDayDatum>(new BinaryOperator("Ticker.Name", "TSPX"))).Returns(dayDataList);
            osMock.Setup(x => x.GetObjects<TickerDayDatum>(new BinaryOperator("Ticker.Name", "US"))).Returns(usDayDataList);

            //act
            cnt.CalculatePosition(pos, osMock.Object);
            //assert
            Assert.AreEqual(4, pos.CalculateData.Count);
            Assert.AreEqual(5, pos.CalculateData[3].SharesCount);
            Assert.AreEqual(140, pos.CalculateData[3].Price);
            Assert.AreEqual(63000, pos.CalculateData[3].Value);
            Assert.AreEqual(4500, pos.CalculateData[3].Profit);
            Assert.AreEqual(18000, pos.CalculateData[3].ProfitTotal);

        }

        [Test]
        public void PopulateCalcDataWithTransactionsData() {
            //arrange
            var cnt = new CalculatePositionController();
            var trans1 = new Transaction(new DateTime(2020, 8, 18), 5, 3190, TransactionDirectionEnum.Buy);
            var trans2 = new Transaction(new DateTime(2020, 8, 18), 1, 3195, TransactionDirectionEnum.Buy);
            var lst = new List<Transaction>();
            lst.Add(trans1);
            lst.Add(trans2);

            var calcData = new CalcPositionDatum(new TickerDayDatum(new Ticker() { Name = "Test1" }, new DateTime(2020, 8, 20), 0));
            calcData.SharesCount = 3;
            calcData.Value = 1000;
            //act
            cnt.PopulateCalcDataWithTransactionsData(calcData, lst,1);
            //assert
            Assert.AreEqual(20145, calcData.Value);
            Assert.AreEqual(9, calcData.SharesCount);

        }

        [Test]
        public void PopulateCalcDataWithTransactionsData_sell() {
            //arrange
            var cnt = new CalculatePositionController();
            var trans1 = new Transaction(new DateTime(2020, 8, 18), 5, 3190, TransactionDirectionEnum.Buy);
            var trans2 = new Transaction(new DateTime(2020, 8, 18), 1, 3195, TransactionDirectionEnum.Sell);
            var lst = new List<Transaction>();
            lst.Add(trans1);
            lst.Add(trans2);

            var calcData = new CalcPositionDatum(new TickerDayDatum(new Ticker() { Name = "Test1" }, new DateTime(2020, 8, 20), 0));
            calcData.SharesCount = 3;
            calcData.Value = 1000;
            //act
            cnt.PopulateCalcDataWithTransactionsData(calcData, lst, 1);
            //assert
            Assert.AreEqual(13755, calcData.Value);
            Assert.AreEqual(7, calcData.SharesCount);

        }
        [Test]
        public void CalculatePositionSummary_SharesCount() {
            //arrange
            var cnt = new CalculatePositionController();
           
            var trans1 = new Transaction(new DateTime(2020, 8, 18), 5, 3190, TransactionDirectionEnum.Buy);
            var trans2 = new Transaction(new DateTime(2020, 8, 18), 1, 3195, TransactionDirectionEnum.Sell);
            var lst = new List<Transaction>();
            lst.Add(trans1);
            lst.Add(trans2);
            //act
            var res = cnt.CalculatePositionSummary(lst);
            //assert
            Assert.AreEqual(4, res.SharesCount);


        }
    }
}
