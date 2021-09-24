using ComplexPortfolio.Module.BusinessObjects;
using ComplexPortfolio.Module.Controllers;
using ComplexPortfolio.Module.HelpClasses;
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
            var trans1 = new Transaction(new DateTime(2020, 8, 18), 6, 3190, TransactionDirectionEnum.Buy);
            var trans2 = new Transaction(new DateTime(2020, 8, 19), 1, 3160, TransactionDirectionEnum.Buy);
            pos.Transactions.Add(trans1);
            pos.Transactions.Add(trans2);

            var osMock = new Mock<IObjectSpace>();

            var dayDataList = new List<TickerDayData>();
            dayDataList.Add(new TickerDayData(ticker, new DateTime(2020, 8, 18), 3184));
            dayDataList.Add(new TickerDayData(ticker, new DateTime(2020, 8, 19), 3157.5m));
            dayDataList.Add(new TickerDayData(ticker, new DateTime(2020, 8, 20), 3155));

            osMock.Setup(x => x.GetObjects<TickerDayData>(new BinaryOperator("Ticker.Name", "FXRL"))).Returns(dayDataList);


            var parameterProvider = new DebugParametersProvider();
            parameterProvider.CurrentDate = new DateTime(2020, 8, 20);
            //act
            cnt.CalculatePosition(pos, osMock.Object, parameterProvider);
            //assert
            Assert.AreEqual(3, pos.CalculateData.Count);
            Assert.AreEqual(7, pos.CalculateData[2].CurrentSharesCount);
            Assert.AreEqual(3155, pos.CalculateData[2].Price);
            Assert.AreEqual(22085, pos.CalculateData[2].Value);
            Assert.AreEqual(17.5, pos.CalculateData[2].ValueDiff);
            Assert.AreEqual(215, pos.CalculateData[2].ValueDiffTotal);

        }
    }
}
