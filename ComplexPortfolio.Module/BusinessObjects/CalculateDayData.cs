using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComplexPortfolio.Module.BusinessObjects {
    [DomainComponent]
    public class CalculateDayData: NonPersistentLiteObject {

        decimal valueDiffTotal;
        decimal valueDiff;
        decimal _value;
        decimal price;
        int currentSharesCount;
        DateTime date;

        public DateTime Date {
            get => date;
            set => date = value;
        }

        public int CurrentSharesCount {
            get => currentSharesCount;
            set => currentSharesCount = value;
        }

        public decimal Price {
            get => price;
            set => price = value;
        }

        public decimal Value {
            get => _value;
            set => _value = value;
        }

        public decimal ValueDiff {
            get => valueDiff;
            set => valueDiff = value;
        }
        
        public decimal ValueDiffTotal {
            get => valueDiffTotal;
            set => valueDiffTotal = value;
        }

    }
}
