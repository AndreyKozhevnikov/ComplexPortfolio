using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using System.Collections.Generic;
using System.Linq;

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

        public override bool Equals(object obj) {
            var otherCalc = obj as CalcPortfolioDatumExportBlock;
            if(otherCalc != null) {
                if(Prefix != otherCalc.Prefix) {
                    return false;
                }
                if(!Names.SequenceEqual(otherCalc.Names)) {
                    return false;
                }
                if(Elements.Count != otherCalc.Elements.Count) {
                    return false;
                }
                foreach(var el in Elements) {
                    var otherEl = otherCalc.Elements.Where(x => x.Date == el.Date).FirstOrDefault();
                    if(otherEl == null) {
                        return false;
                    }
                    if(el.SumValue != otherEl.SumValue) {
                        return false;
                    }
                    if(!el.Values.SequenceEqual(otherEl.Values)) {
                        return false;
                    }
                }
                return true;
            }
            return base.Equals(obj);
        }

    }

}
