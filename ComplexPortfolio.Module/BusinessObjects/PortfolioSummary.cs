using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;

namespace ComplexPortfolio.Module.BusinessObjects {
    [DomainComponent]
    public class PortfolioSummary:NonPersistentBaseObject {
        public double CurrentValue{ get; set; }
        public double TotalProfit{ get; set; }
        public double TotalProfitPercent { get; set; }
    }
}
