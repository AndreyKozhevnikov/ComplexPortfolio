using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using System.Collections.Generic;

namespace ComplexPortfolio.Module.BusinessObjects {
    [DomainComponent]
    public class CalcPortfolioDatumExportBlock : NonPersistentLiteObject {
        public CalcPortfolioDatumExportBlock(string _prefix) {
            Prefix = _prefix;
            Elements = new List<CalcPortfolioDatumExportElement>();
        }
        public string Prefix { get; set; }
        public List<string> Names { get; set; }
        public List<CalcPortfolioDatumExportElement> Elements { get; set; }


    }

}
