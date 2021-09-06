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
    [DebuggerDisplay("Name - {Name}")]
    [DefaultClassOptions]
    public class Ticker : BaseObject {
        public Ticker(Session session) : base(session) {
        }

        public Ticker() {

        }
        string description;
        string name;

        [Size(SizeAttribute.DefaultStringMappingFieldSize)]
        public string Name {
            get => name;
            set => SetPropertyValue(nameof(Name), ref name, value);
        }
        
        [Size(SizeAttribute.DefaultStringMappingFieldSize)]
        public string Description {
            get => description;
            set => SetPropertyValue(nameof(Description), ref description, value);
        }
    }
}
