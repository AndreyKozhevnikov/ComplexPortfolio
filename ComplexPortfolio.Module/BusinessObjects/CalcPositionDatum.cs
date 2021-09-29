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
    public class CalcPositionDatum: NonPersistentLiteObject {

        string tickerName;
        decimal valueTotal;
        decimal valueDiff;
        decimal _value;
        decimal price;
        int currentSharesCount;
        DateTime date;

        public CalcPositionDatum(TickerDayData _dayData) {
            this.price = _dayData.Close;
            this.date = _dayData.Date;
            this.tickerName = _dayData.Ticker.Name;
        }

        public DateTime Date {
            get => date;
            set => date = value;
        }

        public int SharesCount {
            get => currentSharesCount;
            set => currentSharesCount = value;
        }

        public decimal Price {
            get => price;
            set => price = value;
        }

        public decimal Value {
            get => _value;
            set => _value = value;
        }

        public decimal ValueDiff {
            get => valueDiff;
            set => valueDiff = value;
        }

        public decimal ValueDiffTotal {
            get => valueTotal;
            set => valueTotal = value;
        }

        
        [Size(SizeAttribute.DefaultStringMappingFieldSize)]
        public string TickerName {
            get => tickerName;
            set => tickerName = value;
        }

    }
}
