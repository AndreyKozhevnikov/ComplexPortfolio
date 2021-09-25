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
    public class AddPricesFromFilesControllerTests {
        [Test]
        public void GetPriceFromLine_firstLine_noPrices(){
            //arrange
            var cont = new AddDayDataFromFilesController();
            var line = "<TICKER>;<PER>;<DATE>;<TIME>;<OPEN>;<HIGH>;<LOW>;<CLOSE>;<VOL>";
            var osMoq = new Mock<IObjectSpace>();
            osMoq.Setup(x => x.CreateObject<TickerDayData>()).Returns(new TickerDayData());
            
            //act

            var res = cont.ProcessLine(line, osMoq.Object,new List<TickerDayData>(),new List<Ticker>());
            //assert
            Assert.AreEqual(null, res);
        }

        [Test]
        public void GetPriceFromLine_normalline_noticker() {
            //arrange
            var cont = new AddDayDataFromFilesController();
            var line = "FXGD;D;20200109;000000;659.8000000;660.0000000;650.0000000;654.8000000;92468";
            var osMoq = new Mock<IObjectSpace>();
            osMoq.Setup(x => x.CreateObject<TickerDayData>()).Returns(new TickerDayData());
            osMoq.Setup(x => x.CreateObject<Ticker>()).Returns(new Ticker());
            
            //act

            var res = cont.ProcessLine(line, osMoq.Object, new List<TickerDayData>(), new List<Ticker>());
            //assert
            Assert.AreEqual("FXGD", res.Ticker.Name); 
            Assert.AreEqual(new DateTime(2020,01,09), res.Date); 
            Assert.AreEqual(659.8, res.Open); 
            Assert.AreEqual(660, res.High); 
            Assert.AreEqual(650, res.Low); 
            Assert.AreEqual(654.8, res.Close); 
            Assert.AreEqual(92468, res.Volume);
            osMoq.Verify(x => x.CreateObject<Ticker>(), Times.Once);
        }

        [Test]
        public void GetPriceFromLine_normalline_tickerexists() {
            //arrange
            var cont = new AddDayDataFromFilesController();
            var line = "FXGD;D;20200109;000000;659.8000000;660.0000000;650.0000000;654.8000000;92468";
            var osMoq = new Mock<IObjectSpace>();
            osMoq.Setup(x => x.CreateObject<TickerDayData>()).Returns(new TickerDayData());
            osMoq.Setup(x => x.CreateObject<Ticker>()).Returns(new Ticker());
            var existingTickers = new List<Ticker>();
            existingTickers.Add(new Ticker() { Name = "FXGD" });

            //act

            var res = cont.ProcessLine(line, osMoq.Object, new List<TickerDayData>(), existingTickers);
            //assert
            Assert.AreEqual("FXGD", res.Ticker.Name);
            Assert.AreEqual(new DateTime(2020, 01, 09), res.Date);
            Assert.AreEqual(659.8, res.Open);
            Assert.AreEqual(660, res.High);
            Assert.AreEqual(650, res.Low);
            Assert.AreEqual(654.8, res.Close);
            Assert.AreEqual(92468, res.Volume);
            osMoq.Verify(x => x.CreateObject<Ticker>(), Times.Never);
        }

        [Test]
        public void GetPriceFromLine_normalline_tickerexists_PriceExist() {
            //arrange
            var cont = new AddDayDataFromFilesController();
            var line = "FXGD;D;20200109;000000;659.8000000;660.0000000;650.0000000;654.8000000;92468";
            var osMoq = new Mock<IObjectSpace>();
            osMoq.Setup(x => x.CreateObject<TickerDayData>()).Returns(new TickerDayData());
            osMoq.Setup(x => x.CreateObject<Ticker>()).Returns(new Ticker());
            var existingTickers = new List<Ticker>();
            existingTickers.Add(new Ticker() { Name = "FXGD" });
            var existingPrices = new List<TickerDayData>();
            existingPrices.Add(new TickerDayData() {Ticker= new Ticker() { Name = "FXGD" }, Date = new DateTime(2020, 1, 9) });
            //act

            var res = cont.ProcessLine(line, osMoq.Object, existingPrices, existingTickers);
            //assert
            Assert.AreEqual(null, res);
           
            osMoq.Verify(x => x.CreateObject<Ticker>(), Times.Never);
        }

        [Test]
        public void GetPriceFromLine_createNewTickerOnlyOnce() {
            //arrange
            var cont = new AddDayDataFromFilesController();
            var line = "FXGD;D;20200109;000000;659.8000000;660.0000000;650.0000000;654.8000000;92468";
            var osMoq = new Mock<IObjectSpace>();
            osMoq.Setup(x => x.CreateObject<TickerDayData>()).Returns(new TickerDayData());
            osMoq.Setup(x => x.CreateObject<Ticker>()).Returns(new Ticker());
            var existingTickers = new List<Ticker>();
          //  existingTickers.Add(new Ticker() { Name = "FXGD" });
            var existingPrices = new List<TickerDayData>();
            existingPrices.Add(new TickerDayData() { Ticker = new Ticker() { Name = "FXGD" }, Date = new DateTime(2020, 1, 9) });
            //act

            var res = cont.ProcessLine(line, osMoq.Object, existingPrices, existingTickers);
            var res2 = cont.ProcessLine(line, osMoq.Object, existingPrices, existingTickers);
            //assert
            Assert.AreEqual(null, res);

            osMoq.Verify(x => x.CreateObject<Ticker>(), Times.Once);
        }

    }
}
