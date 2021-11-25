using DevExpress.ExpressApp;
using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComplexPortfolio.Module.BusinessObjects {
    [DebuggerDisplay("Date - {Date}")]
    public class CalcPortfolioDatum : NonPersistentLiteObject {
        public CalcPortfolioDatum(DateTime _date) {
            this.date = _date;
        }

        DateTime date;
        public DateTime Date {
            get => date;
            set => date = value;
        }

        public double SumTotal {
            get {
                return SumTotalValues.Sum(x => x.Value);
            }
        }

        public double SumDiffTotal {
            get {
                return SumDiffTotalValues.Sum(x => x.Value);
            }
        }

        public List<CalcPositionDatum> PositionData { get; set; }
        public List<string> Tickers {
            get {
                return SumTotalValues.Select(x => x.Key).ToList();
            }
        }
        public List<string> Labels {
            get {
                return SumTotalValuesLabels.Select(x => x.Key).ToList();
            }
        }
        public Dictionary<string, double> SumTotalValues { get; set; }
        public Dictionary<string, double> SumDiffTotalValues { get; set; }

        public Dictionary<string, double> SumTotalValuesLabels { get; set; }
        public Dictionary<string, double> SumDiffTotalValuesLabels { get; set; }

    }
}
