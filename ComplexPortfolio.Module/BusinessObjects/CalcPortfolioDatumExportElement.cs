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
        public CalcPortfolioDatumExportElement(DateTime _date, int valuesCount):this(_date) {
            Values = new double?[valuesCount];
        }
        public DateTime Date { get; set; }
        public double?[] Values { get; set; }
        public double SumValue { get; set; }
    }

}
