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

    public interface ITicker {
        List<ITickerDayDatum> DayData { get; }
        ITicker Currency { get; }
        string Name { get; }
        bool IsBlocked { get; }
    }

    [DebuggerDisplay("Name - {Name}")]
    [DefaultClassOptions]
    public class Ticker : BaseObject, ITicker {
        public Ticker(Session session) : base(session) {
        }

        public Ticker() {

        }
        bool isBlocked;
        bool isCurrency;
        Ticker _currency;
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
        public XPCollection<TickerDayDatum> DayData {
            get {
                return GetCollection<TickerDayDatum>(nameof(DayData));
            }
        }
        [Association]
        public XPCollection<Position> Positions {
            get {
                return GetCollection<Position>(nameof(Positions));
            }
        }


        public bool IsCurrency {
            get => isCurrency;
            set => SetPropertyValue(nameof(IsCurrency), ref isCurrency, value);
        }
        [DataSourceCriteria("IsCurrency")]
        public Ticker Currency {
            get => _currency;
            set => SetPropertyValue(nameof(Currency), ref _currency, value);
        }


        public bool IsBlocked {
            get => isBlocked;
            set => SetPropertyValue(nameof(IsBlocked), ref isBlocked, value);
        }

        List<ITickerDayDatum> ITicker.DayData => DayData.Cast<ITickerDayDatum>().ToList();

        ITicker ITicker.Currency => Currency;
    }
}
