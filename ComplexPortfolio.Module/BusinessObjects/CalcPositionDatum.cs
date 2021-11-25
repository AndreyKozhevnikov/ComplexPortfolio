using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComplexPortfolio.Module.BusinessObjects {
    [DomainComponent]
    [DebuggerDisplay("Date - {Date}")]
    public class CalcPositionDatum : NonPersistentLiteObject {

        string label;
        string tickerName;
        double valueTotal;
        double valueDiff;
        double _value;
        double price;
        int currentSharesCount;
        DateTime date;

        public CalcPositionDatum(TickerDayDatum _dayData) {
            this.price = _dayData.Close;
            this.date = _dayData.Date;
            this.tickerName = _dayData.Ticker.Name;

        }
        public CalcPositionDatum(TickerDayDatum _dayData, string _label) : this(_dayData) {
            this.label = _label;
        }

        public DateTime Date {
            get => date;
            set => date = value;
        }

        public int SharesCount {
            get => currentSharesCount;
            set => currentSharesCount = value;
        }

        public double Price {
            get => price;
            set => price = value;
        }

        public double Value {
            get => _value;
            set => _value = value;
        }

        public double ValueDiff {
            get => valueDiff;
            set => valueDiff = value;
        }

        public double ValueDiffTotal {
            get => valueTotal;
            set => valueTotal = value;
        }


        [Size(SizeAttribute.DefaultStringMappingFieldSize)]
        public string TickerName {
            get => tickerName;
            set => tickerName = value;
        }




        [Size(SizeAttribute.DefaultStringMappingFieldSize)]
        public string Label {
            get => label;
            set => label = value;
        }



    }
}
