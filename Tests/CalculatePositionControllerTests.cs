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
            Assert.AreEqual(175, pos.CalculateData[3].ValueDiff);
            Assert.AreEqual(-45, pos.CalculateData[3].ValueDiffTotal);

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
            cnt.PopulateCalcDataWithTransactionsData(calcData, lst);
            //assert
            Assert.AreEqual(20145, calcData.Value);
            Assert.AreEqual(9, calcData.SharesCount);

        }
    }
}
