using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComplexPortfolio.Module.BusinessObjects {
    [DefaultClassOptions]
    [DebuggerDisplay("Name - {Name}")]
    public class Account : BaseObject {
        public Account(Session session) : base(session) {
        }

        string name;

        [Size(SizeAttribute.DefaultStringMappingFieldSize)]
        public string Name {
            get => name;
            set => SetPropertyValue(nameof(Name), ref name, value);
        }

        [Association]
        public XPCollection<Transaction> Transactions {
            get {
                return GetCollection<Transaction>(nameof(Transactions));
            }
        }

        AccountSummary summary;
        public AccountSummary Summary { get => summary; set => SetPropertyValue(nameof(Summary), ref summary, value); }
    }
}
