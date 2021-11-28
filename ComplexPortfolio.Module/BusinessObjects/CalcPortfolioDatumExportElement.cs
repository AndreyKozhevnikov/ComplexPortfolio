using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using System;
using System.Collections.Generic;

namespace ComplexPortfolio.Module.BusinessObjects {
    [DomainComponent]
    public class CalcPortfolioDatumExportElement : NonPersistentLiteObject {
        public CalcPortfolioDatumExportElement(DateTime _date) {
            Date = _date;
        }
        public DateTime Date { get; set; }
        public List<double?> Values { get; set; }
        public double SumValue { get; set; }
    }

}
