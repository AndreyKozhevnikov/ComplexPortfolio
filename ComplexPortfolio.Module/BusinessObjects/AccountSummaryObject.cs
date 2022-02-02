using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;

namespace ComplexPortfolio.Module.BusinessObjects {
    [DomainComponent]
    public class AccountSummaryObject : NonPersistentBaseObject {
        public AccountSummaryObject(ITicker _ticker) {
            Ticker = _ticker;
        }

        public ITicker Ticker{ get; set; }
        public int Amount{ get; set; }
        public double LastPrice{ get; set; }
        public double CurrentValue{ get; set; }
    }
}
