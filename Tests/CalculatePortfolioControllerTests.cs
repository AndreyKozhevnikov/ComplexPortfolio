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
            var calcData11 = new CalcPositionData(new DateTime(2021, 8, 19));
            var calcData12 = new CalcPositionData(new DateTime(2021, 8, 20));
            position1.CalculateData = new List<CalcPositionData>();
            position1.CalculateData.Add(calcData11);
            position1.CalculateData.Add(calcData12);

            var position2 = new Position();
            var calcData21 = new CalcPositionData(new DateTime(2021, 8, 20));
            var calcData22 = new CalcPositionData(new DateTime(2021, 8, 21));
            position2.CalculateData = new List<CalcPositionData>();
            position2.CalculateData.Add(calcData21);
            position2.CalculateData.Add(calcData22);

            var positions = new List<Position>();
            positions.Add(position1);
            positions.Add(position2);
            //act
            var res = cnt.CalculatePortfolioData(positions);
            //assert
            Assert.AreEqual(3, res.Count);


        }
    }
}
