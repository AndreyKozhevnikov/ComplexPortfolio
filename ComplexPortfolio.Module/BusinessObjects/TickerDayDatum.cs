using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComplexPortfolio.Module.BusinessObjects {
    [DefaultClassOptions]
    [DebuggerDisplay("Ticker-{Ticker.Name},Date-{Date}")]
    public class TickerDayDatum : BaseObject {
        public TickerDayDatum(Session session) : base(session) {
        }

        public TickerDayDatum() {

        }

        public TickerDayDatum(Ticker _ticker, DateTime _date, decimal _close) {
            this.ticker = _ticker;
            this.date = _date;
            this.close = _close;
        }

        double volume;
        decimal close;
        decimal low;
        decimal high;
        decimal open;
        DateTime date;
        Ticker ticker;

        [Size(SizeAttribute.DefaultStringMappingFieldSize)]
        [Association]
        public Ticker Ticker {
            get => ticker;
            set => SetPropertyValue(nameof(Ticker), ref ticker, value);
        }

        public DateTime Date {
            get => date;
            set => SetPropertyValue(nameof(Date), ref date, value);
        }


        public decimal Open {
            get => open;
            set => SetPropertyValue(nameof(Open), ref open, value);
        }

        public decimal High {
            get => high;
            set => SetPropertyValue(nameof(High), ref high, value);
        }

        public decimal Low {
            get => low;
            set => SetPropertyValue(nameof(Low), ref low, value);
        }

        public decimal Close {
            get => close;
            set => SetPropertyValue(nameof(Close), ref close, value);
        }
        
        public double Volume {
            get => volume;
            set => SetPropertyValue(nameof(Volume), ref volume, value);
        }
    }
}
