using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComplexPortfolio.Module.HelpClasses {
    public class ProportionData {
        public ProportionData(string _ticker,decimal _price, int _currenAmount,double _targetProportion) {
            Ticker = _ticker;
            Price = _price;
            CurrentAmount = _currenAmount;
            TargetProportion = _targetProportion;
        }
        public string Ticker { get; set; }
        public decimal Price { get; set; }
        public double TargetProportion { get; set; }
        public int CurrentAmount { get; set; }
    }
}
