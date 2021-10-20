using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComplexPortfolio.Module.HelpClasses {
    public class ProportionCalculator {

        public Dictionary<string, int> Calculate(double currentSum, List<ProportionData> currentProportions, double sumToSpend) {
            var result = new Dictionary<string, int>();
            double alreadySpent = 0;
            foreach(var d in currentProportions) {
                var currentValue = d.CurrentAmount * d.Price;
                var targetValue = (currentSum + sumToSpend) * d.TargetProportion;
                var sumToSpendForShare = Math.Min(targetValue - currentValue, sumToSpend);
                var res = Math.Floor(sumToSpendForShare / d.Price);
                if(res > 0) {
                    result[d.Ticker] = (int)res;
                } else {
                    result[d.Ticker] = 0;
                }
                alreadySpent = alreadySpent + result[d.Ticker] * d.Price;
            }
            var leftOver = sumToSpend - alreadySpent;
            var minPrice = currentProportions.Min(x => x.Price);
            if(minPrice < leftOver) {
                var additionalShares =(int) Math.Floor(leftOver / minPrice);
                var ticker = currentProportions.Where(x => x.Price == minPrice).First().Ticker;
                result[ticker] = result[ticker] + additionalShares;
            }
            return result;
        }
    }
}
