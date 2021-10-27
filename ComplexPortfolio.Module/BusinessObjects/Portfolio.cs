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
    [DebuggerDisplay("Name - {Name}")]
    public class Portfolio : BaseObject {
        public Portfolio(Session session) : base(session) {
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

        [Association]
        public XPCollection<Position> Positions {
            get {
                return GetCollection<Position>(nameof(Positions));
            }
        }

        [PersistentAlias("Positions.Sum(CurrentValue)")]
        public double CurrentValue {
            get { return Convert.ToDouble (EvaluateAlias(nameof(CurrentValue))); }
        }

        [PersistentAlias("Positions.Sum(ValueChangeSum)")]
        public double ValueChange {
            get { return Convert.ToDouble(EvaluateAlias(nameof(ValueChange))); }
        }
        [PersistentAlias("ValueChange/(CurrentValue-ValueChange)")]
        public double ValueChangePercent {
            get { return Convert.ToDouble(EvaluateAlias(nameof(ValueChangePercent))); }
        }
    }
}
