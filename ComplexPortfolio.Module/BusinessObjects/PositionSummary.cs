using DevExpress.ExpressApp.DC;

namespace ComplexPortfolio.Module.BusinessObjects {
    [DomainComponent]
    public class PositionSummary {
        public int SharesCount { get; set; }
        public double LastPrice { get; set; }
        public double LastPriceRub { get; set; }
        public double CurrentValue{ get; set; }
        
    }
}
