using ComplexPortfolio.Module.BusinessObjects;
using ComplexPortfolio.Module.Controllers;
using DevExpress.ExpressApp;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComplexPortfolio.Module.Tests {
    [TestFixture]
    public class AddPricesFromFilesControllerTests {
        [Test]
        public void GetPriceFromLine_firstLine(){
            //arrange
            var cont = new AddPricesFromFilesController();
            var line = "<TICKER>;<PER>;<DATE>;<TIME>;<OPEN>;<HIGH>;<LOW>;<CLOSE>;<VOL>";
            var osMoq = new Mock<IObjectSpace>();
            osMoq.Setup(x => x.CreateObject<TickerPrice>()).Returns(new TickerPrice());
            //act

            var res = cont.GetPriceFromLine(line, osMoq.Object);
            //assert
            Assert.AreEqual(null, res);
        }

        [Test]
        public void GetPriceFromLine_normal() {
            //arrange
            var cont = new AddPricesFromFilesController();
            var line = "FXGD;D;20200109;000000;659.8000000;660.0000000;650.0000000;654.8000000;92468";
            var osMoq = new Mock<IObjectSpace>();
            osMoq.Setup(x => x.CreateObject<TickerPrice>()).Returns(new TickerPrice());
            //act

            var res = cont.GetPriceFromLine(line, osMoq.Object);
            //assert
            Assert.AreEqual("FXGD", res.Ticker.Name); 
            Assert.AreEqual(new DateTime(2020,01,09), res.Date); 
            Assert.AreEqual(659.8, res.Open); 
            Assert.AreEqual(660, res.High); 
            Assert.AreEqual(650, res.Low); 
            Assert.AreEqual(654.8, res.Close); 
            Assert.AreEqual(92468, res.Volume); 
        }

    }
}
