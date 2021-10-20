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
        [Ignore("skip for now")]
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
            proportions.Add(new ProportionData("t1", 1, 600, 0.5));
            proportions.Add(new ProportionData("t2", 2, 200, 0.5));



            var expected = new Dictionary<string, int>();
            expected["t1"] = 0;
            expected["t2"] = 50;
            //act
            var result = calc.Calculate(1000, proportions, 100);
            //assert
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void Calculate_6() {
            //arrange
            var calc = new ProportionCalculator();

            var proportions = new List<ProportionData>();
            proportions.Add(new ProportionData("VTBX", 168.6, 29, 0.2));
            proportions.Add(new ProportionData("FXUS", 58.6, 200, 0.47));
            proportions.Add(new ProportionData("FXDM", 75.5, 74, 0.23));
            proportions.Add(new ProportionData("FXCN", 3160, 0, 0.1));
            



            var expected = new Dictionary<string, int>();
            expected["VTBX"] = 11;
            expected["FXUS"] = 82;
            expected["FXDM"] = 30;
            expected["FXCN"] = 1;
            
            //act
            var result = calc.Calculate(22196.4, proportions, 12136);
            //assert
            Assert.AreEqual(expected, result);
        }
        [Test]//todo
        [Ignore("todo")]
        public void Calculate_7() {
            //arrange
            var calc = new ProportionCalculator();

            var proportions = new List<ProportionData>();
            proportions.Add(new ProportionData("T1", 250, 2, 0.5));
            proportions.Add(new ProportionData("T2", 111, 6, 0.3));
            proportions.Add(new ProportionData("T3", 122, 1, 0.2));


            var expected = new Dictionary<string, int>();
            expected["T1"] = 11;
            expected["T2"] = 82;
            expected["T3"] = 30;

            //act
            var result = calc.Calculate(22196.4, proportions, 12136);
            //assert
            Assert.AreEqual(expected, result);
        }
    }
}
