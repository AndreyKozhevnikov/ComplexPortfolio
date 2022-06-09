using ComplexPortfolio.Module.BusinessObjects;
using ComplexPortfolio.Module.Controllers;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests {
    [TestFixture]
    public class CalculateAccountControllerTests {

        [Test]
        public void CalculateAccountSummary() {
            //arrange
            var cnt = new CalculateAccountController();

            var p1 = new Mock<IPosition>();
            var tick1 = new Mock<ITicker>();
            tick1.Setup(x => x.Name).Returns("Ticker1");
            var dayData1 = new Mock<ITickerDayDatum>();
            dayData1.Setup(x => x.Date).Returns(new DateTime(2022, 6, 9));
            dayData1.Setup(x => x.Close).Returns(10);
            var listDayData1 = new List<ITickerDayDatum>();
            listDayData1.Add(dayData1.Object);
            tick1.Setup(x => x.DayData).Returns(listDayData1);
            p1.Setup(x => x.Ticker).Returns(tick1.Object);

            var p2 = new Mock<IPosition>();
            var tick2 = new Mock<ITicker>();
            var dayData2 = new Mock<ITickerDayDatum>();
            dayData2.Setup(x => x.Date).Returns(new DateTime(2022, 6, 9));
            var listDayData2 = new List<ITickerDayDatum>();
            listDayData2.Add(dayData2.Object);
            dayData2.Setup(x => x.Close).Returns(20);
            tick2.Setup(x => x.Name).Returns("Ticker2");
            tick2.Setup(x => x.DayData).Returns(listDayData2);
            p2.Setup(x => x.Ticker).Returns(tick2.Object);

            var t1 = new Mock<ITransaction>();// (new DateTime(2022, 2, 1), 4, 50, TransactionDirectionEnum.Buy, p1.Object);
            t1.Setup(x => x.Date).Returns(new DateTime(2022, 2, 1));
            t1.Setup(x => x.Amount).Returns(4);
            t1.Setup(x => x.Price).Returns(50);
            t1.Setup(x => x.Direction).Returns(TransactionDirectionEnum.Buy);
            t1.Setup(x => x.Position).Returns(p1.Object);

            var t2 = new Mock<ITransaction>();// (new DateTime(2022, 2, 1), 4, 50, TransactionDirectionEnum.Buy, p1.Object);
            t2.Setup(x => x.Date).Returns(new DateTime(2022, 2, 2));
            t2.Setup(x => x.Amount).Returns(9);
            t2.Setup(x => x.Price).Returns(70);
            t2.Setup(x => x.Direction).Returns(TransactionDirectionEnum.Buy);
            t2.Setup(x => x.Position).Returns(p1.Object);

            var t3 = new Mock<ITransaction>();// (new DateTime(2022, 2, 1), 4, 50, TransactionDirectionEnum.Buy, p1.Object);
            t3.Setup(x => x.Date).Returns(new DateTime(2022, 2, 1));
            t3.Setup(x => x.Amount).Returns(8);
            t3.Setup(x => x.Price).Returns(30);
            t3.Setup(x => x.Direction).Returns(TransactionDirectionEnum.Buy);
            t3.Setup(x => x.Position).Returns(p2.Object);

            var t4 = new Mock<ITransaction>();// (new DateTime(2022, 2, 1), 4, 50, TransactionDirectionEnum.Buy, p1.Object);
            t4.Setup(x => x.Date).Returns(new DateTime(2022, 2, 2));
            t4.Setup(x => x.Amount).Returns(2);
            t4.Setup(x => x.Price).Returns(45);
            t4.Setup(x => x.Direction).Returns(TransactionDirectionEnum.Sell);
            t4.Setup(x => x.Position).Returns(p2.Object);



            var list = new List<ITransaction>();
            list.Add(t1.Object);
            list.Add(t2.Object);
            list.Add(t3.Object);
            list.Add(t4.Object);
            //act
            var res = cnt.CalculateAccountSummary(list);
            //assert
            Assert.AreEqual(2, res.TickersData.Count);
            
            Assert.AreEqual("Ticker1", res.TickersData[0].Ticker.Name);
            Assert.AreEqual(13, res.TickersData[0].Amount);
            Assert.AreEqual(10, res.TickersData[0].LastPrice);
            Assert.AreEqual(130, res.TickersData[0].CurrentValue);

            Assert.AreEqual("Ticker2", res.TickersData[1].Ticker.Name);
            Assert.AreEqual(6, res.TickersData[1].Amount);
            Assert.AreEqual(20, res.TickersData[1].LastPrice);
            Assert.AreEqual(120, res.TickersData[1].CurrentValue);


        }
    }
}
