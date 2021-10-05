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

        decimal sumDiffTotal;
        // decimal sumTotal=-1;
        DateTime date;
        public DateTime Date {
            get => date;
            set => date = value;
        }

        public decimal SumTotal {
            get {
                return SumTotalValues.Sum(x => x.Item2);
            }
        }

        public decimal SumDiffTotal {
            get {
                return SumDiffTotalValues.Sum(x => x.Item2);
            }
        }

        public List<CalcPositionDatum> PositionData { get; set; }
        public List<string> Tickers {
            get {
                return SumTotalValues.Select(x => x.Item1).ToList();
            }
        }
        public List<Tuple<string, decimal>> SumTotalValues { get; set; }
        public List<Tuple<string, decimal>> SumDiffTotalValues { get; set; }



    }
}
