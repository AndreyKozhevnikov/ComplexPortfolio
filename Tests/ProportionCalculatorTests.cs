using ComplexPortfolio.Module.HelpClasses;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests {
    [TestFixture]
    public class ProportionCalculatorTests {
        [Test]
        public void Calculate_0() {
            //arrange
            var calc = new ProportionCalculator();

            var proportions = new List<ProportionData>();
            proportions.Add(new ProportionData("t1", 1, 0, 0.5));
            proportions.Add(new ProportionData("t2", 2, 0, 0.5));



            var expected = new Dictionary<string, int>();
            expected["t1"] = 50;
            expected["t2"] = 25;
            //act
            var result = calc.Calculate(0, proportions, 100);
            //assert
            Assert.AreEqual(expected, result);
        }
        [Test]
        public void Calculate_1() {
            //arrange
            var calc = new ProportionCalculator();

            var proportions = new List<ProportionData>();
            proportions.Add(new ProportionData("t1", 1, 0, 0.5));
            proportions.Add(new ProportionData("t2", 5, 0, 0.5));



            var expected = new Dictionary<string, int>();
            expected["t1"] = 50;
            expected["t2"] = 10;
            //act
            var result = calc.Calculate(0, proportions, 100);
            //assert
            Assert.AreEqual(expected, result);
        }
        [Test]
        public void Calculate_2() {
            //arrange
            var calc = new ProportionCalculator();

            var proportions = new List<ProportionData>();
            proportions.Add(new ProportionData("t1", 10, 0, 0.5));
            proportions.Add(new ProportionData("t2", 6, 0, 0.3));
            proportions.Add(new ProportionData("t3", 4, 0, 0.2));



            var expected = new Dictionary<string, int>();
            expected["t1"] = 5;
            expected["t2"] = 5;
            expected["t3"] = 5;
            //act
            var result = calc.Calculate(0, proportions, 100);
            //assert
            Assert.AreEqual(expected, result);
        }
        [Test]
        public void Calculate_3_bigPrice() {
            //arrange
            var calc = new ProportionCalculator();

            var proportions = new List<ProportionData>();
            proportions.Add(new ProportionData("t1", 51, 0, 0.5));
            proportions.Add(new ProportionData("t2", 1, 0, 0.5));



            var expected = new Dictionary<string, int>();
            expected["t1"] = 1;
            expected["t2"] = 49;
            //act
            var result = calc.Calculate(0, proportions, 100);
            //assert
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void Calculate_4() {
            //arrange
            var calc = new ProportionCalculator();

            var proportions = new List<ProportionData>();
            proportions.Add(new ProportionData("t1", 1, 60, 0.5));
            proportions.Add(new ProportionData("t2", 2, 20, 0.5));



            var expected = new Dictionary<string, int>();
            expected["t1"] = 40;
            expected["t2"] = 30;
            //act
            var result = calc.Calculate(100, proportions, 100);
            //assert
            Assert.AreEqual(expected, result);
        }
        [Test]
        public void Calculate_5() {
            //arrange
            var calc = new ProportionCalculator();

            var proportions = new List<ProportionData>();
            proportions.Add(new ProportionData("t1", 1, 60, 0.5));
            proportions.Add(new ProportionData("t2", 2, 20, 0.5));



            var expected = new Dictionary<string, int>();
            expected["t1"] = 0;
            expected["t2"] = 50;
            //act
            var result = calc.Calculate(1000, proportions, 100);
            //assert
            Assert.AreEqual(expected, result);
        }
    }
}
