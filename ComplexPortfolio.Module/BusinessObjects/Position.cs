using DevExpress.ExpressApp.ConditionalAppearance;
using DevExpress.ExpressApp.DC;
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
    [DebuggerDisplay("Ticker - {Ticker.Name}")]
    [Appearance("RedPriceObject", AppearanceItemType = "ViewItem", TargetItems = "Ticker", Criteria = "!AllowEdit", Context = "DetailView", Enabled = false)]
    [XafDefaultProperty(nameof(Comment))]
    public class Position : BaseObject {
        public Position(Session session) : base(session) {

        }
        protected override void OnChanged(string propertyName, object oldValue, object newValue) {
            base.OnChanged(propertyName, oldValue, newValue);
            if(propertyName == nameof(Position.Ticker) && newValue != null) {
                ((Ticker)newValue).Changed += Position_Changed;
                CalculateLastPrice(true);
            }

        }

        private void Position_Changed(object sender, ObjectChangeEventArgs e) {
            if(e.PropertyName == nameof(Ticker.DayData)) {
                ((XPCollection<TickerDayDatum>)e.NewValue).CollectionChanged += Position_CollectionChanged;
                CalculateLastPrice(true);
            }

        }

        private void Position_CollectionChanged(object sender, XPCollectionChangedEventArgs e) {
            CalculateLastPrice(true);
        }

        public Position() {

        }


        string label;
        bool allowEdit;
        string comment;
        Ticker ticker;
        private List<CalcPositionDatum> calculateData;
        [Association]

        public Ticker Ticker {
            get => ticker;
            set => SetPropertyValue(nameof(Ticker), ref ticker, value);
        }

        [VisibleInListView(false)]
        public bool AllowEdit {
            get => allowEdit;
            set => SetPropertyValue(nameof(AllowEdit), ref allowEdit, value);
        }


        [Size(SizeAttribute.DefaultStringMappingFieldSize)]
        public string Comment {
            get => comment;
            set => SetPropertyValue(nameof(Comment), ref comment, value);
        }

        [Association]
        public XPCollection<Transaction> Transactions {
            get {
                return GetCollection<Transaction>(nameof(Transactions));
            }
        }

        Portfolio _portfolio;
        [Association]
        public Portfolio Portfolio {
            get {
                return _portfolio;
            }
            set {
                SetPropertyValue(nameof(Portfolio), ref _portfolio, value);
            }
        }


        [Size(SizeAttribute.DefaultStringMappingFieldSize)]
        public string Label {
            get => label;
            set => SetPropertyValue(nameof(Label), ref label, value);
        }
        [PersistentAlias("Transactions.Sum(Amount)")]
        public int SharesCount {
            get { return Convert.ToInt32(EvaluateAlias(nameof(SharesCount))); }
        }
        [PersistentAlias("Transactions.Sum(Amount*Price)/Transactions.Sum(Amount)")]
        public double AveragePrice {
            get { return Convert.ToDouble(EvaluateAlias(nameof(AveragePrice))); }
        }
        // [PersistentAlias("AveragePrice*LastPriceRub")]
        public double AveragePriceRub {
            get {
                if(Ticker == null) {
                    return 0;
                }
                if(Ticker.Currency != null) {
                    return AveragePrice * _lastCurrencyPrice;
                } else {
                    return AveragePrice;
                }
            }
        }

        [PersistentAlias("LastPrice*SharesCount")]
        public double CurrentValue {
            get { return Convert.ToDouble(EvaluateAlias(nameof(CurrentValue))); }
        }

        [PersistentAlias("LastPriceRub*SharesCount")]
        public double CurrentValueRub {
            get { return Convert.ToDouble(EvaluateAlias(nameof(CurrentValueRub))); }
        }
        [PersistentAlias("AveragePrice*SharesCount")]
        public double InputValue {
            get { return Convert.ToDouble(EvaluateAlias(nameof(InputValue))); }
        }
        [PersistentAlias("AveragePriceRub*SharesCount")]
        public double InputValueRub {
            get { return Convert.ToDouble(EvaluateAlias(nameof(InputValueRub))); }
        }

        [PersistentAlias("CurrentValue - InputValue")]
        public double ValueChangeSum {
            get { return Convert.ToDouble(EvaluateAlias(nameof(ValueChangeSum))); }
        }
        [PersistentAlias("CurrentValueRub - InputValueRub")]
        public double ValueChangeSumRub {
            get { return Convert.ToDouble(EvaluateAlias(nameof(ValueChangeSumRub))); }
        }
        [PersistentAlias("ValueChangeSum/InputValue")]
        public double ValueChangePercent {
            get { return Convert.ToDouble(EvaluateAlias(nameof(ValueChangePercent))); }
        }
        [PersistentAlias("ValueChangeSumRub/InputValueRub")]
        public double ValueChangePercentRub {
            get { return Convert.ToDouble(EvaluateAlias(nameof(ValueChangePercentRub))); }
        }


        double _lastPrice;
        bool _isLastPriceCalculated = false;
        public double LastPrice {
            get {
                CalculateLastPrice(true);
                return _lastPrice;

            }
        }


        double _lastRubPrice;
        double _lastCurrencyPrice;
        public double LastPriceRub {
            get {
                CalculateLastPrice(true);
                return _lastRubPrice;
            }
        }
        void CalculateLastPrice(bool ignore) {
            if(_isLastPriceCalculated && !ignore) {
                return;
            }
            if(ticker == null || ticker.DayData == null || ticker.DayData.Count == 0) {
                return;
            }
            var maxDate = Ticker.DayData.Max(x => x.Date);
            var lastPrice = Ticker.DayData.Where(x => x.Date == maxDate).First().Close;
            if(Ticker.Currency != null) {
                _lastCurrencyPrice = Ticker.Currency.DayData.Where(x => x.Date == maxDate).FirstOrDefault().Close;
                _lastRubPrice = lastPrice * _lastCurrencyPrice;
            } else {
                _lastRubPrice = lastPrice;
            }
            _lastPrice = lastPrice;
            _isLastPriceCalculated = true;
        }

        public List<CalcPositionDatum> CalculateData { get => calculateData; set => SetPropertyValue(nameof(CalculateData), ref calculateData, value); }
    }
}
