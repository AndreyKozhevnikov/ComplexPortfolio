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
            this.Summary = new PositionSummary();
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
     



       

        public List<CalcPositionDatum> CalculateData {
            get => calculateData; 
            set => SetPropertyValue(nameof(CalculateData), ref calculateData, value);
        }
        [NonPersistent]
        public PositionSummary Summary{ get; set; }
    }
}
