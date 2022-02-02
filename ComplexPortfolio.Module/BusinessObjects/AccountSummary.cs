using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using System.Collections.Generic;

namespace ComplexPortfolio.Module.BusinessObjects {
    [DomainComponent]
    public class AccountSummary : NonPersistentBaseObject {
        public AccountSummary() {
            TickersData = new List<AccountSummaryObject>();
        }
        public double AllValue{ get;set; }
        public List<AccountSummaryObject> TickersData{ get; set; }
    }
}
