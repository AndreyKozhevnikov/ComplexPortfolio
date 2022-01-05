using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;

namespace ComplexPortfolio.Module.BusinessObjects {
    [DomainComponent]
    public class PositionSummary : NonPersistentBaseObject {
        public int SharesCount { get; set; }
        public double LastPrice { get; set; }
        public double LastPriceRub { get; set; }
        public double CurrentValue { get; set; }
        public double InputValue { get; set; }
        public double FixedProfit { get; set; }
        public double AveragePrice { get; set; }
        public double VirtualProfit{ get; set; }
        public double VirtualProfitPercent{ get; set; }
        public double TotalProfit { get; set; }

    }
}
