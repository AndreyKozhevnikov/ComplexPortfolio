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
            var pos = new Mock<IPosition>();
            pos.Setup(x => x.Label).Returns("TestLabel");
            var ticker = new Mock<ITicker>();
            //  ticker.Setup(x=>x.n)
            //new Ticker() { Name = "FXRL" };
            pos.Setup(x => x.Ticker).Returns(ticker.Object);
            var trans1 = new Transaction(new DateTime(2020, 8, 18), 5, 3190, TransactionDirectionEnum.Buy);
            var trans2 = new Transaction(new DateTime(2020, 8, 18), 1, 3195, TransactionDirectionEnum.Buy);
            var trans3 = new Transaction(new DateTime(2020, 8, 19), 1, 3160, TransactionDirectionEnum.Buy);
            var posTrans = new List<Transaction>();
            posTrans.Add(trans1);
            posTrans.Add(trans2);
            posTrans.Add(trans3);

            pos.Setup(x => x.Transactions).Returns(posTrans);
            //pos.Transactions.Add(trans1);
            //pos.Transactions.Add(trans2);
            //pos.Transactions.Add(trans3);


            var dayDataList = new List<ITickerDayDatum>();

            dayDataList.Add(CreateDayDatum(ticker, new DateTime(2020, 8, 17), 3180));
            dayDataList.Add(CreateDayDatum(ticker, new DateTime(2020, 8, 18), 3184));
            dayDataList.Add(CreateDayDatum(ticker, new DateTime(2020, 8, 19), 3157.5));
            dayDataList.Add(CreateDayDatum(ticker, new DateTime(2020, 8, 20), 3155));
            dayDataList.Add(CreateDayDatum(ticker, new DateTime(2020, 8, 22), 3180));

            ticker.Setup(x => x.DayData).Returns(dayDataList);

            // osMock.Setup(x => x.GetObjects<TickerDayDatum>(new BinaryOperator("Ticker.Name", "FXRL"))).Returns(dayDataList);

            //act
            var res = cnt.CalculatePositionData(pos.Object);
            //assert
            Assert.AreEqual(4, res.Count);
            Assert.AreEqual(7, res[3].SharesCount);
            Assert.AreEqual(3180, res[3].Price);
            Assert.AreEqual(22260, res[3].Value);
            Assert.AreEqual(175, res[3].Profit);
            Assert.AreEqual(-45, res[3].ProfitTotal);
            Assert.AreEqual("TestLabel", res[3].Label);

        }

        ITickerDayDatum CreateDayDatum(Mock<ITicker> ticker, DateTime date, double close) {
            var dayMock = new Mock<ITickerDayDatum>();
            dayMock.Setup(x => x.Close).Returns(close);
            dayMock.Setup(x => x.Date).Returns(date);
            if(ticker != null) {
                dayMock.Setup(x => x.Ticker).Returns(ticker.Object);
            } else {
                dayMock.Setup(x => x.Ticker).Returns(new Mock<ITicker>().Object);
            }
            return dayMock.Object;
        }

        [Test]
        public void CalculatePosition_sell() {
            //arrange
            var cnt = new CalculatePositionController();
            var pos = new Mock<IPosition>();
            pos.Setup(x => x.Label).Returns("TestLabel");
            var ticker = new Mock<ITicker>();
            pos.Setup(x => x.Ticker).Returns(ticker.Object);
            var trans1 = new Transaction(new DateTime(2021, 11, 16), 1, 98, TransactionDirectionEnum.Buy);
            var trans2 = new Transaction(new DateTime(2021, 11, 18), 2, 117, TransactionDirectionEnum.Buy);
            var trans3 = new Transaction(new DateTime(2021, 11, 19), 1, 134, TransactionDirectionEnum.Sell);
            var trans4 = new Transaction(new DateTime(2021, 11, 20), 2, 138, TransactionDirectionEnum.Sell);
            var posTrans = new List<Transaction>();
            posTrans.Add(trans1);
            posTrans.Add(trans2);
            posTrans.Add(trans3);
            posTrans.Add(trans4);

            pos.Setup(x => x.Transactions).Returns(posTrans);
            var dayDataList = new List<ITickerDayDatum>();
            dayDataList.Add(CreateDayDatum(ticker, new DateTime(2021, 11, 15), 90));
            dayDataList.Add(CreateDayDatum(ticker, new DateTime(2021, 11, 16), 100));
            dayDataList.Add(CreateDayDatum(ticker, new DateTime(2021, 11, 17), 110));
            dayDataList.Add(CreateDayDatum(ticker, new DateTime(2021, 11, 18), 120));
            dayDataList.Add(CreateDayDatum(ticker, new DateTime(2021, 11, 19), 130));
            dayDataList.Add(CreateDayDatum(ticker, new DateTime(2021, 11, 20), 140));
            dayDataList.Add(CreateDayDatum(ticker, new DateTime(2021, 11, 21), 150));

            ticker.Setup(x => x.DayData).Returns(dayDataList);

            //act
            var res = cnt.CalculatePositionData(pos.Object);
            //assert
            Assert.AreEqual(6, res.Count);

            Assert.AreEqual(1, res[0].SharesCount);
            Assert.AreEqual(100, res[0].Value);
            Assert.AreEqual(2, res[0].Profit);
            Assert.AreEqual(2, res[0].ProfitTotal);

            Assert.AreEqual(1, res[1].SharesCount);
            Assert.AreEqual(110, res[1].Value);
            Assert.AreEqual(10, res[1].Profit);
            Assert.AreEqual(12, res[1].ProfitTotal);

            Assert.AreEqual(3, res[2].SharesCount);
            Assert.AreEqual(360, res[2].Value);
            Assert.AreEqual(16, res[2].Profit);
            Assert.AreEqual(28, res[2].ProfitTotal);

            Assert.AreEqual(2, res[3].SharesCount);
            Assert.AreEqual(260, res[3].Value);
            Assert.AreEqual(34, res[3].Profit);
            Assert.AreEqual(62, res[3].ProfitTotal);

            Assert.AreEqual(0, res[4].SharesCount);
            Assert.AreEqual(0, res[4].Value);
            Assert.AreEqual(16, res[4].Profit);
            Assert.AreEqual(78, res[4].ProfitTotal);



            Assert.AreEqual("TestLabel", res[3].Label);

        }

        [Test]
        public void CalculatePosition_sellv2() {
            //arrange
            var cnt = new CalculatePositionController();
            var pos = new Mock<IPosition>();
            pos.Setup(x => x.Label).Returns("TestLabel");
            var ticker = new Mock<ITicker>();
            pos.Setup(x => x.Ticker).Returns(ticker.Object);
            var trans1 = new Transaction(new DateTime(2021, 11, 16), 3, 10, TransactionDirectionEnum.Buy);
            var trans2 = new Transaction(new DateTime(2021, 11, 17), 1, 14, TransactionDirectionEnum.Buy);
            var trans3 = new Transaction(new DateTime(2021, 11, 20), 1, 28, TransactionDirectionEnum.Sell);

            var posTrans = new List<Transaction>();
            posTrans.Add(trans1);
            posTrans.Add(trans2);
            posTrans.Add(trans3);
            pos.Setup(x => x.Transactions).Returns(posTrans);

            var dayDataList = new List<ITickerDayDatum>();
            dayDataList.Add(CreateDayDatum(ticker, new DateTime(2021, 11, 15), 5));
            dayDataList.Add(CreateDayDatum(ticker, new DateTime(2021, 11, 16), 10));
            dayDataList.Add(CreateDayDatum(ticker, new DateTime(2021, 11, 17), 15));
            dayDataList.Add(CreateDayDatum(ticker, new DateTime(2021, 11, 18), 20));
            dayDataList.Add(CreateDayDatum(ticker, new DateTime(2021, 11, 19), 17));
            dayDataList.Add(CreateDayDatum(ticker, new DateTime(2021, 11, 20), 25));
            dayDataList.Add(CreateDayDatum(ticker, new DateTime(2021, 11, 21), 20));

            ticker.Setup(x => x.DayData).Returns(dayDataList);

            //act
            var res = cnt.CalculatePositionData(pos.Object);
            //assert
            Assert.AreEqual(6, res.Count);
            Assert.AreEqual(3, res[0].SharesCount);
            Assert.AreEqual(30, res[0].Value);
            Assert.AreEqual(0, res[0].Profit);
            Assert.AreEqual(0, res[0].ProfitTotal);

            Assert.AreEqual(4, res[1].SharesCount);
            Assert.AreEqual(60, res[1].Value);
            Assert.AreEqual(16, res[1].Profit);
            Assert.AreEqual(16, res[1].ProfitTotal);

            Assert.AreEqual(4, res[2].SharesCount);
            Assert.AreEqual(80, res[2].Value);
            Assert.AreEqual(20, res[2].Profit);
            Assert.AreEqual(36, res[2].ProfitTotal);

            Assert.AreEqual(4, res[3].SharesCount);
            Assert.AreEqual(68, res[3].Value);
            Assert.AreEqual(-12, res[3].Profit);
            Assert.AreEqual(24, res[3].ProfitTotal);

            Assert.AreEqual(3, res[4].SharesCount);
            Assert.AreEqual(75, res[4].Value);
            Assert.AreEqual(35, res[4].Profit);
            Assert.AreEqual(59, res[4].ProfitTotal);

            Assert.AreEqual(3, res[5].SharesCount);
            Assert.AreEqual(60, res[5].Value);
            Assert.AreEqual(-15, res[5].Profit);
            Assert.AreEqual(44, res[5].ProfitTotal);

        }

        [Test]
        public void CalculatePosition_sellv3() {
            //arrange
            var cnt = new CalculatePositionController();
            var pos = new Mock<IPosition>();
            pos.Setup(x => x.Label).Returns("TestLabel");
            var ticker = new Mock<ITicker>();
            pos.Setup(x => x.Ticker).Returns(ticker.Object);
            var trans0 = new Transaction(new DateTime(2021, 11, 15), 3, 5, TransactionDirectionEnum.Buy);
            var trans1 = new Transaction(new DateTime(2021, 11, 16), 1, 8, TransactionDirectionEnum.Buy);
            var trans2 = new Transaction(new DateTime(2021, 11, 16), 2, 7, TransactionDirectionEnum.Sell);
            var posTrans = new List<Transaction>();
            posTrans.Add(trans0);
            posTrans.Add(trans1);
            posTrans.Add(trans2);
            pos.Setup(x => x.Transactions).Returns(posTrans);

            var dayDataList = new List<ITickerDayDatum>();
            dayDataList.Add(CreateDayDatum(ticker, new DateTime(2021, 11, 15), 5));
            dayDataList.Add(CreateDayDatum(ticker, new DateTime(2021, 11, 16), 10));

            ticker.Setup(x => x.DayData).Returns(dayDataList);

            //act
            var res = cnt.CalculatePositionData(pos.Object);
            //assert
            Assert.AreEqual(2, res.Count);

            Assert.AreEqual(2, res[1].SharesCount);
            Assert.AreEqual(20, res[1].Value);
            Assert.AreEqual(11, res[1].Profit);
            Assert.AreEqual(11, res[1].ProfitTotal);
        }


        [Test]
        public void CalculatePosition_US() {
            //arrange
            var cnt = new CalculatePositionController();
            var pos = new Mock<IPosition>();
            var usTicker = new Mock<ITicker>();// { Name = "US", IsCurrency = true };
            usTicker.Setup(x => x.Name).Returns("US");

            var ticker = new Mock<ITicker>();// { Name = "TSPX", Currency = usTicker };
            ticker.Setup(x => x.Currency).Returns(usTicker.Object);
            pos.Setup(x => x.Ticker).Returns(ticker.Object);
            var posTrans = new List<Transaction>();
            var trans1 = new Transaction(new DateTime(2020, 8, 18), 5, 100, TransactionDirectionEnum.Buy);

            posTrans.Add(trans1);
            pos.Setup(x => x.Transactions).Returns(posTrans);


            var dayDataList = new List<ITickerDayDatum>();
            dayDataList.Add(CreateDayDatum(ticker, new DateTime(2020, 8, 17), 95));
            dayDataList.Add(CreateDayDatum(ticker, new DateTime(2020, 8, 18), 110));
            dayDataList.Add(CreateDayDatum(ticker, new DateTime(2020, 8, 19), 120));
            dayDataList.Add(CreateDayDatum(ticker, new DateTime(2020, 8, 20), 130));
            dayDataList.Add(CreateDayDatum(ticker, new DateTime(2020, 8, 22), 140));

            var usDayDataList = new List<ITickerDayDatum>();
            usDayDataList.Add(CreateDayDatum(usTicker, new DateTime(2020, 8, 17), 50));
            usDayDataList.Add(CreateDayDatum(usTicker, new DateTime(2020, 8, 18), 60));
            usDayDataList.Add(CreateDayDatum(usTicker, new DateTime(2020, 8, 19), 70));
            usDayDataList.Add(CreateDayDatum(usTicker, new DateTime(2020, 8, 20), 80));
            usDayDataList.Add(CreateDayDatum(usTicker, new DateTime(2020, 8, 22), 90));

            ticker.Setup(x => x.DayData).Returns(dayDataList);
            usTicker.Setup(x => x.DayData).Returns(usDayDataList);

            //act
            var res = cnt.CalculatePositionData(pos.Object);
            //assert
            Assert.AreEqual(4, res.Count);
            Assert.AreEqual(5, res[3].SharesCount);
            Assert.AreEqual(140, res[3].Price);
            Assert.AreEqual(63000, res[3].Value);
            Assert.AreEqual(4500, res[3].Profit);
            Assert.AreEqual(18000, res[3].ProfitTotal);

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

            var calcData = new CalcPositionDatum(CreateDayDatum(null, new DateTime(2020, 8, 20), 0));
            calcData.SharesCount = 3;
            calcData.Value = 1000;
            //act
            cnt.PopulateCalcDataWithTransactionsData(calcData, lst, 1);
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

            var calcData = new CalcPositionDatum(CreateDayDatum(null, new DateTime(2020, 8, 20), 0));
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
            var pos = new Mock<IPosition>();
            pos.Setup(x => x.Transactions).Returns(lst);
            pos.Setup(x => x.Ticker).Returns(new Mock<ITicker>().Object);
            //act
            var res = cnt.CalculatePositionSummary(pos.Object);
            //assert
            Assert.AreEqual(4, res.SharesCount);
        }




        [Test]
        public void CalculatePositionSummary_LastPrice() {
            //arrange
            var cnt = new CalculatePositionController();

            var ticker = new Mock<ITicker>();
            var myDayDataList = new List<ITickerDayDatum>();
            var d1 = CreateDayDatum(null, new DateTime(2022, 1, 1), 15);
            var d2 = CreateDayDatum(null, new DateTime(2022, 1, 2), 16);
            myDayDataList.Add(d1);
            myDayDataList.Add(d2);
            ticker.Setup(x => x.DayData).Returns(myDayDataList);
            //var trans1 = new Transaction(new DateTime(2020, 8, 18), 5, 3190, TransactionDirectionEnum.Buy);
            //var trans2 = new Transaction(new DateTime(2020, 8, 18), 1, 3195, TransactionDirectionEnum.Sell);
            var lst = new List<Transaction>();
            //lst.Add(trans1);
            //lst.Add(trans2);
            var pos = new Mock<IPosition>();
            pos.Setup(x => x.Transactions).Returns(lst);
            pos.Setup(x => x.Ticker).Returns(ticker.Object);
            //act
            var res = cnt.CalculatePositionSummary(pos.Object);
            //assert
            Assert.AreEqual(16, res.LastPrice);
        }

        [Test]
        public void CalculatePositionSummary_LastPriceRub() {
            //arrange
            var cnt = new CalculatePositionController();

            var ticker = new Mock<ITicker>();

            var myDayDataList = new List<ITickerDayDatum>();
            var d1 = CreateDayDatum(null, new DateTime(2022, 1, 1), 25);
            var d2 = CreateDayDatum(null, new DateTime(2022, 1, 2), 35);
            myDayDataList.Add(d1);
            myDayDataList.Add(d2);
            ticker.Setup(x => x.DayData).Returns(myDayDataList);


            var currency = new Mock<ITicker>();
            var myCurrencyDataList = new List<ITickerDayDatum>();
            var dc1 = CreateDayDatum(null, new DateTime(2022, 1, 1), 10);
            var dc2 = CreateDayDatum(null, new DateTime(2022, 1, 2), 20);
            myCurrencyDataList.Add(dc1);
            myCurrencyDataList.Add(dc2);
            currency.Setup(x => x.DayData).Returns(myCurrencyDataList);
            ticker.Setup(x => x.Currency).Returns(currency.Object);

            var lst = new List<Transaction>();
            var pos = new Mock<IPosition>();
            pos.Setup(x => x.Transactions).Returns(lst);
            pos.Setup(x => x.Ticker).Returns(ticker.Object);
            //act
            var res = cnt.CalculatePositionSummary(pos.Object);
            //assert
            Assert.AreEqual(700, res.LastPriceRub);
        }

        [Test]
        public void CalculatePositionSummary_LastPriceRub_NoCurrency() {
            //arrange
            var cnt = new CalculatePositionController();

            var ticker = new Mock<ITicker>();

            var myDayDataList = new List<ITickerDayDatum>();
            var d1 = CreateDayDatum(null, new DateTime(2022, 1, 1), 55);
            var d2 = CreateDayDatum(null, new DateTime(2022, 1, 2), 45);
            myDayDataList.Add(d1);
            myDayDataList.Add(d2);
            ticker.Setup(x => x.DayData).Returns(myDayDataList);




            var lst = new List<Transaction>();
            var pos = new Mock<IPosition>();
            pos.Setup(x => x.Transactions).Returns(lst);
            pos.Setup(x => x.Ticker).Returns(ticker.Object);
            //act
            var res = cnt.CalculatePositionSummary(pos.Object);
            //assert
            Assert.AreEqual(45, res.LastPriceRub);
        }

        [Test]
        public void CalculatePositionSummary_LastPrice_Blocked() {
            //arrange
            var cnt = new CalculatePositionController();

            var ticker = new Mock<ITicker>();

            var myDayDataList = new List<ITickerDayDatum>();
            var d1 = CreateDayDatum(null, new DateTime(2022, 1, 1), 15);
            var d2 = CreateDayDatum(null, new DateTime(2022, 1, 2), 16);
            myDayDataList.Add(d1);
            myDayDataList.Add(d2);
            ticker.Setup(x => x.DayData).Returns(myDayDataList);
            ticker.Setup(x => x.IsBlocked).Returns(true);
            //var trans1 = new Transaction(new DateTime(2020, 8, 18), 5, 3190, TransactionDirectionEnum.Buy);
            //var trans2 = new Transaction(new DateTime(2020, 8, 18), 1, 3195, TransactionDirectionEnum.Sell);
            var lst = new List<Transaction>();
            //lst.Add(trans1);
            //lst.Add(trans2);
            var pos = new Mock<IPosition>();
            pos.Setup(x => x.Transactions).Returns(lst);
            pos.Setup(x => x.Ticker).Returns(ticker.Object);
            //act
            var res = cnt.CalculatePositionSummary(pos.Object);
            //assert
            Assert.AreEqual(0, res.LastPrice);
        }


        [Test]
        public void CalculatePositionSummary_CurrentValue() {
            //arrange
            var cnt = new CalculatePositionController();

            var ticker = new Mock<ITicker>();

            var myDayDataList = new List<ITickerDayDatum>();
            var d1 = CreateDayDatum(null, new DateTime(2022, 1, 1), 55);
            var d2 = CreateDayDatum(null, new DateTime(2022, 1, 2), 80);
            myDayDataList.Add(d1);
            myDayDataList.Add(d2);
            ticker.Setup(x => x.DayData).Returns(myDayDataList);




            var lst = new List<Transaction>();
            var trans1 = new Transaction(new DateTime(2020, 8, 18), 15, 3190, TransactionDirectionEnum.Buy);
            var trans2 = new Transaction(new DateTime(2020, 8, 18), 11, 3195, TransactionDirectionEnum.Sell);
            lst.Add(trans1);
            lst.Add(trans2);
            var pos = new Mock<IPosition>();
            pos.Setup(x => x.Transactions).Returns(lst);
            pos.Setup(x => x.Ticker).Returns(ticker.Object);
            //act
            var res = cnt.CalculatePositionSummary(pos.Object);
            //assert
            Assert.AreEqual(320, res.CurrentValue);
        }
        [Test]
        public void CalculatePositionSummary_InputValue() {
            //arrange
            var cnt = new CalculatePositionController();

            var ticker = new Mock<ITicker>();

            var myDayDataList = new List<ITickerDayDatum>();
            var d1 = CreateDayDatum(null, new DateTime(2022, 1, 1), 55);
            var d2 = CreateDayDatum(null, new DateTime(2022, 1, 2), 80);
            myDayDataList.Add(d1);
            myDayDataList.Add(d2);
            ticker.Setup(x => x.DayData).Returns(myDayDataList);




            var lst = new List<Transaction>();
            var trans1 = new Transaction(new DateTime(2020, 8, 18), 15, 3190, TransactionDirectionEnum.Buy);
            var trans2 = new Transaction(new DateTime(2020, 8, 18), 11, 3195, TransactionDirectionEnum.Sell);
            lst.Add(trans1);
            lst.Add(trans2);
            var pos = new Mock<IPosition>();
            pos.Setup(x => x.Transactions).Returns(lst);
            pos.Setup(x => x.Ticker).Returns(ticker.Object);
            //act
            var res = cnt.CalculatePositionSummary(pos.Object);
            //assert
            Assert.AreEqual(12760, res.InputValue);
        }


        [Test]
        public void CalculatePositionSummary_InputValue_1() {
            //arrange
            var cnt = new CalculatePositionController();

            var ticker = new Mock<ITicker>();

            var myDayDataList = new List<ITickerDayDatum>();
            var d1 = CreateDayDatum(null, new DateTime(2022, 1, 1), 55);
            var d2 = CreateDayDatum(null, new DateTime(2022, 1, 2), 80);
            myDayDataList.Add(d1);
            myDayDataList.Add(d2);
            ticker.Setup(x => x.DayData).Returns(myDayDataList);




            var lst = new List<Transaction>();
            var trans1 = new Transaction(new DateTime(2020, 8, 18), 10, 10, TransactionDirectionEnum.Buy);
            var trans2 = new Transaction(new DateTime(2020, 8, 19), 5, 15, TransactionDirectionEnum.Sell);
            var trans3 = new Transaction(new DateTime(2020, 8, 20), 2, 20, TransactionDirectionEnum.Sell);
            var trans4 = new Transaction(new DateTime(2020, 8, 21), 10, 25, TransactionDirectionEnum.Buy);
            lst.Add(trans1);
            lst.Add(trans2);
            lst.Add(trans3);
            lst.Add(trans4);
            var pos = new Mock<IPosition>();
            pos.Setup(x => x.Transactions).Returns(lst);
            pos.Setup(x => x.Ticker).Returns(ticker.Object);
            //act
            var res = cnt.CalculatePositionSummary(pos.Object);
            //assert
            Assert.AreEqual(280, res.InputValue);
        }

        [Test]
        public void CalculatePositionSummary_InputValue_2() {
            //arrange
            var cnt = new CalculatePositionController();

            var ticker = new Mock<ITicker>();

            var myDayDataList = new List<ITickerDayDatum>();
            var d1 = CreateDayDatum(null, new DateTime(2022, 1, 1), 55);
            var d2 = CreateDayDatum(null, new DateTime(2022, 1, 2), 80);
            myDayDataList.Add(d1);
            myDayDataList.Add(d2);
            ticker.Setup(x => x.DayData).Returns(myDayDataList);




            var lst = new List<Transaction>();
            var trans1 = new Transaction(new DateTime(2020, 8, 18), 10, 10, TransactionDirectionEnum.Buy);
            var trans2 = new Transaction(new DateTime(2020, 8, 19), 5, 15, TransactionDirectionEnum.Sell);
            var trans3 = new Transaction(new DateTime(2020, 8, 20), 5, 20, TransactionDirectionEnum.Buy);
            var trans4 = new Transaction(new DateTime(2020, 8, 21), 8, 5, TransactionDirectionEnum.Sell);
            var trans5 = new Transaction(new DateTime(2020, 8, 21), 5, 6, TransactionDirectionEnum.Buy);
            lst.Add(trans1);
            lst.Add(trans2);
            lst.Add(trans3);
            lst.Add(trans4);
            lst.Add(trans5);
            var pos = new Mock<IPosition>();
            pos.Setup(x => x.Transactions).Returns(lst);
            pos.Setup(x => x.Ticker).Returns(ticker.Object);
            //act
            var res = cnt.CalculatePositionSummary(pos.Object);
            //assert
            Assert.AreEqual(70, res.InputValue);
            Assert.AreEqual(7, res.SharesCount);
            Assert.AreEqual(560, res.CurrentValue);
            Assert.AreEqual(10, res.AveragePrice);
            Assert.AreEqual(-45, res.FixedProfit);
            Assert.AreEqual(490, res.VirtualProfit);
            //Assert.AreEqual(7, res.VirtualProfitPercent);
            Assert.AreEqual(445, res.TotalProfit);
            Assert.AreEqual(6.36, Math.Round(res.TotalProfitPercent, 2));
        }

        [Test]
        public void CalculatePositionSummary_InputValue_2_Currency() {
            //arrange
            var cnt = new CalculatePositionController();

            var ticker = new Mock<ITicker>();

            var myDayDataList = new List<ITickerDayDatum>();
            var d1 = CreateDayDatum(null, new DateTime(2022, 1, 1), 55);
            var d2 = CreateDayDatum(null, new DateTime(2022, 1, 2), 80);
            myDayDataList.Add(d1);
            myDayDataList.Add(d2);
            ticker.Setup(x => x.DayData).Returns(myDayDataList);
            var currency = new Mock<ITicker>();
            var myCurrencyDataList = new List<ITickerDayDatum>();
            var dc1 = CreateDayDatum(null, new DateTime(2022, 1, 1), 10);
            var dc2 = CreateDayDatum(null, new DateTime(2022, 1, 2), 30);
            myCurrencyDataList.Add(dc1);
            myCurrencyDataList.Add(dc2);
            currency.Setup(x => x.DayData).Returns(myCurrencyDataList);
            ticker.Setup(x => x.Currency).Returns(currency.Object);



            var lst = new List<Transaction>();
            var trans1 = new Transaction(new DateTime(2020, 8, 18), 10, 10, TransactionDirectionEnum.Buy);
            var trans2 = new Transaction(new DateTime(2020, 8, 19), 5, 15, TransactionDirectionEnum.Sell);
            var trans3 = new Transaction(new DateTime(2020, 8, 20), 5, 20, TransactionDirectionEnum.Buy);
            var trans4 = new Transaction(new DateTime(2020, 8, 21), 8, 5, TransactionDirectionEnum.Sell);
            var trans5 = new Transaction(new DateTime(2020, 8, 21), 5, 6, TransactionDirectionEnum.Buy);
            lst.Add(trans1);
            lst.Add(trans2);
            lst.Add(trans3);
            lst.Add(trans4);
            lst.Add(trans5);
            var pos = new Mock<IPosition>();
            pos.Setup(x => x.Transactions).Returns(lst);
            pos.Setup(x => x.Ticker).Returns(ticker.Object);
            //act
            var res = cnt.CalculatePositionSummary(pos.Object);
            //assert
            Assert.AreEqual(2100, res.InputValue);
            Assert.AreEqual(7, res.SharesCount);
            Assert.AreEqual(16800, res.CurrentValue);
            Assert.AreEqual(300, res.AveragePrice);
            Assert.AreEqual(-1350, res.FixedProfit);
            Assert.AreEqual(14700, res.VirtualProfit);
            //Assert.AreEqual(7, res.VirtualProfitPercent);
            Assert.AreEqual(13350, res.TotalProfit);
            Assert.AreEqual(6.36, Math.Round(res.TotalProfitPercent, 2));
        }

        [Test]
        public void CalculatePositionSummary_InputValue_2_Currency_noValue() {
            //arrange
            var cnt = new CalculatePositionController();

            var ticker = new Mock<ITicker>();

            var myDayDataList = new List<ITickerDayDatum>();
            var d1 = CreateDayDatum(null, new DateTime(2022, 1, 1), 55);
            var d2 = CreateDayDatum(null, new DateTime(2022, 1, 2), 80);
            myDayDataList.Add(d1);
            myDayDataList.Add(d2);
            ticker.Setup(x => x.DayData).Returns(myDayDataList);
            var currency = new Mock<ITicker>();
            var myCurrencyDataList = new List<ITickerDayDatum>();
            var dc1 = CreateDayDatum(null, new DateTime(2022, 1, 1), 20);
            var dc2 = CreateDayDatum(null, new DateTime(2022, 1, 3), 30);
            myCurrencyDataList.Add(dc1);
            myCurrencyDataList.Add(dc2);
            currency.Setup(x => x.DayData).Returns(myCurrencyDataList);
            ticker.Setup(x => x.Currency).Returns(currency.Object);



            var lst = new List<Transaction>();
            var trans1 = new Transaction(new DateTime(2020, 8, 18), 10, 10, TransactionDirectionEnum.Buy);
            var trans2 = new Transaction(new DateTime(2020, 8, 19), 5, 15, TransactionDirectionEnum.Sell);
            var trans3 = new Transaction(new DateTime(2020, 8, 20), 5, 20, TransactionDirectionEnum.Buy);
            var trans4 = new Transaction(new DateTime(2020, 8, 21), 8, 5, TransactionDirectionEnum.Sell);
            var trans5 = new Transaction(new DateTime(2020, 8, 21), 5, 6, TransactionDirectionEnum.Buy);
            lst.Add(trans1);
            lst.Add(trans2);
            lst.Add(trans3);
            lst.Add(trans4);
            lst.Add(trans5);
            var pos = new Mock<IPosition>();
            pos.Setup(x => x.Transactions).Returns(lst);
            pos.Setup(x => x.Ticker).Returns(ticker.Object);
            //act
            var res = cnt.CalculatePositionSummary(pos.Object);
            //assert
            Assert.AreEqual(1400, res.InputValue);
            Assert.AreEqual(7, res.SharesCount);
            Assert.AreEqual(11200, res.CurrentValue);
            Assert.AreEqual(200, res.AveragePrice);
            Assert.AreEqual(-900, res.FixedProfit);
            Assert.AreEqual(9800, res.VirtualProfit);
            //Assert.AreEqual(7, res.VirtualProfitPercent);
            Assert.AreEqual(8900, res.TotalProfit);
            Assert.AreEqual(6.36, Math.Round(res.TotalProfitPercent, 2));
        }

        [Test]
        public void GetLastCurrencyPrice() {
            //arrange
            var cnt = new CalculatePositionController();
            var myCurrencyDataList = new List<ITickerDayDatum>();
            var dc1 = CreateDayDatum(null, new DateTime(2022, 1, 1), 20);
            var dc2 = CreateDayDatum(null, new DateTime(2022, 1, 2), 30);
            myCurrencyDataList.Add(dc1);
            myCurrencyDataList.Add(dc2);
            var maxDate = new DateTime(2022, 1, 2);
            //act
            var res = cnt.GetLastCurrencyPrice(myCurrencyDataList, maxDate);
            //assert
            Assert.AreEqual(30, res);
        }
        [Test]
        public void GetLastCurrencyPrice_2() {
            //arrange
            var cnt = new CalculatePositionController();
            var myCurrencyDataList = new List<ITickerDayDatum>();
            var dc1 = CreateDayDatum(null, new DateTime(2022, 1, 1), 20);
            var dc2 = CreateDayDatum(null, new DateTime(2022, 1, 3), 30);
            myCurrencyDataList.Add(dc1);
            myCurrencyDataList.Add(dc2);
            var maxDate = new DateTime(2022, 1, 2);
            //act
            var res = cnt.GetLastCurrencyPrice(myCurrencyDataList, maxDate);
            //assert
            Assert.AreEqual(20, res);
        }
        [Test]
        public void GetLastCurrencyPrice_3() {
            //arrange
            var cnt = new CalculatePositionController();
            var myCurrencyDataList = new List<ITickerDayDatum>();
            var dc1 = CreateDayDatum(null, new DateTime(2022, 1, 11), 20);
            var dc2 = CreateDayDatum(null, new DateTime(2022, 1, 12), 25);
            var dc3 = CreateDayDatum(null, new DateTime(2022, 1, 30), 30);
            myCurrencyDataList.Add(dc1);
            myCurrencyDataList.Add(dc2);
            myCurrencyDataList.Add(dc3);
            var maxDate = new DateTime(2022, 1, 20);
            //act
            var res = cnt.GetLastCurrencyPrice(myCurrencyDataList, maxDate);
            //assert
            Assert.AreEqual(25, res);
        }
        [Test]
        [Ignore("todo")]
        public void CalculatePositionSummary_InputValue_3_EmptyPosition() {
            //arrange
            var cnt = new CalculatePositionController();

            var ticker = new Mock<ITicker>();

            var myDayDataList = new List<ITickerDayDatum>();
            var d1 = CreateDayDatum(null, new DateTime(2022, 1, 1), 55);
            var d2 = CreateDayDatum(null, new DateTime(2022, 1, 2), 80);
            myDayDataList.Add(d1);
            myDayDataList.Add(d2);
            ticker.Setup(x => x.DayData).Returns(myDayDataList);

            var lst = new List<Transaction>();
            var trans1 = new Transaction(new DateTime(2020, 8, 18), 10, 10, TransactionDirectionEnum.Buy);
            var trans2 = new Transaction(new DateTime(2020, 8, 19), 10, 15, TransactionDirectionEnum.Sell);
            lst.Add(trans1);
            lst.Add(trans2);
            var pos = new Mock<IPosition>();
            pos.Setup(x => x.Transactions).Returns(lst);
            pos.Setup(x => x.Ticker).Returns(ticker.Object);
            //act
            var res = cnt.CalculatePositionSummary(pos.Object);
            //assert

            Assert.AreEqual(6.36, Math.Round(res.TotalProfitPercent, 2)); //for now it is infinity
        }
    }
}
