using DevExpress.ExpressApp;
using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComplexPortfolio.Module.BusinessObjects {
    public class CalcPortfolioData : NonPersistentLiteObject {
        public CalcPortfolioData(DateTime _date) {
            this.date = _date;
            
        }

        double sumDiffTotal;
        double sumTotal;
        DateTime date;
        public DateTime Date {
            get => date;
            set => date = value;
        }

        public double SumTotal {
            get => sumTotal;
            set => SetPropertyValue(ref sumTotal, value);
        }

        public double SumDiffTotal {
            get => sumDiffTotal;
            set => SetPropertyValue(ref sumDiffTotal, value);
        }

        public List<CalcPositionData> PositionData { get; set; }
        public List<string> Tickers { get; set; }
        public List<double> SumTotalValues { get; set; }
        public List<double> SumDiffTotalValues { get; set; }

        public void CalculateData() {

        }

    }
}
