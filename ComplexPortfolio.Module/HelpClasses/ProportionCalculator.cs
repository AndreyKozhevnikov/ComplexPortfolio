using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComplexPortfolio.Module.HelpClasses {
    public class ProportionCalculator {

        public Dictionary<string, int> Calculate(double currentSum, List<ProportionData> currentProportions, double sumToSpend) {
            foreach(var d in currentProportions) {
                var currentValue = d.CurrentAmount * d.Price;
                var currentProportion = currentValue / currentSum;
                var sumToSpendForShare = (currentSum + sumToSpend) * d.TargetProportion;
                var result =Math.Floor(sumToSpendForShare / d.Price);
            }
            return null;
        }
    }
}
