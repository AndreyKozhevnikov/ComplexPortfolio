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
            var calcData11 = new CalcPositionData(d1);
            var calcData12 = new CalcPositionData(d2);
            position1.CalculateData = new List<CalcPositionData>();
            position1.CalculateData.Add(calcData11);
            position1.CalculateData.Add(calcData12);

            var position2 = new Position();
            var calcData21 = new CalcPositionData(d2);
            var calcData22 = new CalcPositionData(d3);
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
            Assert.AreEqual(1, res[0].PositionData.Count);
            Assert.AreEqual(2, res[1].PositionData.Count);
            Assert.AreEqual(1, res[2].PositionData.Count);
            Assert.AreEqual(d1, res[0].Date);
            Assert.AreEqual(d2, res[1].Date);
            Assert.AreEqual(d3, res[2].Date);
        }
    }
}
