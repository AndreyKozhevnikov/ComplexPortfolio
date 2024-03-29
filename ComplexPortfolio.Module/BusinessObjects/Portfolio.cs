﻿using DevExpress.Persistent.Base;
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


        bool isVirtual;
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

        
        public bool IsVirtual {
            get => isVirtual;
            set => SetPropertyValue(nameof(IsVirtual), ref isVirtual, value);
        }

        PortfolioSummary _summary;
        [NonPersistent]
        public PortfolioSummary Summary {
            get {
                return _summary;
            }
            set {
                SetPropertyValue(nameof(Summary), ref _summary, value);
            }
        }

        Account _defaultAccount;
        public Account DefaultAccount {
            get {
                return _defaultAccount;
            }
            set {
                SetPropertyValue(nameof(DefaultAccount), ref _defaultAccount, value);
            }
        }

    }
}
