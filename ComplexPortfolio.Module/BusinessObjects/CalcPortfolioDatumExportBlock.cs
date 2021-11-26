using DevExpress.ExpressApp;
using System;
using System.Collections.Generic;

namespace ComplexPortfolio.Module.BusinessObjects {
    public class CalcPortfolioDatumExportBlock : NonPersistentLiteObject {
        public DateTime Date{ get; set; }
        public List<string> Names { get; set; }
        public Dictionary<string, double> Values { get; set; }
        public Dictionary<string, double> DiffValues { get; set; }
    }
}
