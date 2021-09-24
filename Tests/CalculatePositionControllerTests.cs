using ComplexPortfolio.Module.BusinessObjects;
using ComplexPortfolio.Module.Controllers;
using ComplexPortfolio.Module.HelpClasses;
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
    public  class CalculatePositionControllerTests {
        [Test]
        public void CalculatePosition() {
            //arrange
            var cnt = new CalculatePositionController();
            var pos = new Position();
            var trans = new Transaction();
            trans.TransationDate = new DateTime(2021, 9, 23);
            pos.Transactions.Add(trans);
            var osMock= new Mock<IObjectSpace>();
            var parameterProvider = new DebugParametersProvider();
            parameterProvider.CurrentDate = new DateTime(2021, 9, 24);
            //act
            cnt.CalculatePosition(pos, osMock.Object, parameterProvider);
            //assert
            Assert.AreEqual(2, pos.CalculateData.Count);

        }
    }
}
