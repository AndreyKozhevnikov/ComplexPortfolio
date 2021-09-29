using DevExpress.ExpressApp;
using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComplexPortfolio.Module.BusinessObjects {
    public class CalcPortfolioDatum : NonPersistentLiteObject {
        public CalcPortfolioDatum(DateTime _date) {
            this.date = _date;
        }

        decimal sumDiffTotal;
        decimal sumTotal;
        DateTime date;
        public DateTime Date {
            get => date;
            set => date = value;
        }

        public decimal SumTotal {
            get => sumTotal;
            set => SetPropertyValue(ref sumTotal, value);
        }

        public decimal SumDiffTotal {
            get => sumDiffTotal;
            set => SetPropertyValue(ref sumDiffTotal, value);
        }

        public List<CalcPositionDatum> PositionData { get; set; }
        public List<string> Tickers { get; set; }
        public List<decimal> SumTotalValues { get; set; }
        public List<decimal> SumDiffTotalValues { get; set; }

        public void CalculateData() {

        }

    }
}
