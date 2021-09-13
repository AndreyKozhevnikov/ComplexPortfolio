using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComplexPortfolio.Module.BusinessObjects {
    [DefaultClassOptions]
    public class Position : BaseObject {
        public Position(Session session) : base(session) {
        }

        Ticker ticker;
        public Ticker Ticker {
            get => ticker;
            set => SetPropertyValue(nameof(Ticker), ref ticker, value);
        }

        [Association]
        public XPCollection<Transaction> Transactions {
            get {
                return GetCollection<Transaction>(nameof(Transactions));
            }
        }
    }
}
