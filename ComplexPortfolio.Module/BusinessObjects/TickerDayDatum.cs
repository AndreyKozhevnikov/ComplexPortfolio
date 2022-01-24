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

    public interface ITickerDayDatum {
        double Close{ get; set; }
        DateTime Date{ get; set; }

        ITicker Ticker{ get;  }
    }

    [DefaultClassOptions]
    [DebuggerDisplay("Ticker-{Ticker.Name},Date-{Date}")]
    public class TickerDayDatum : BaseObject, ITickerDayDatum {
        public TickerDayDatum(Session session) : base(session) {
        }

        public TickerDayDatum() { //todo: only for tests? remove.

        }
        public TickerDayDatum(DateTime _date, double _close) {

            this.date = _date;
            this.close = _close;
        }
        public TickerDayDatum(Ticker _ticker, DateTime _date, double _close) : this(_date, _close) {
            this.ticker = _ticker;

        }

        double volume;
        double close;
        double low;
        double high;
        double open;
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


        public double Open {
            get => open;
            set => SetPropertyValue(nameof(Open), ref open, value);
        }

        public double High {
            get => high;
            set => SetPropertyValue(nameof(High), ref high, value);
        }

        public double Low {
            get => low;
            set => SetPropertyValue(nameof(Low), ref low, value);
        }

        public double Close {
            get => close;
            set => SetPropertyValue(nameof(Close), ref close, value);
        }
        
        public double Volume {
            get => volume;
            set => SetPropertyValue(nameof(Volume), ref volume, value);
        }
        ITicker ITickerDayDatum.Ticker { get => Ticker; }
    }
}
