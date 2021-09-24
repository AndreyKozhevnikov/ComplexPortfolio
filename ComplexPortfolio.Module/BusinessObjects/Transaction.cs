using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComplexPortfolio.Module.BusinessObjects {
    public class Transaction : BaseObject {
        public Transaction(Session session) : base(session) {
        }

        public Transaction() {
        }

        string comment;
        int amount;
        decimal price;
        string propertyName;
        DateTime transationDate;
        TransactionDirectionEnum direction;

        public TransactionDirectionEnum Direction {
            get => direction;
            set => SetPropertyValue(nameof(Direction), ref direction, value);
        }

        public DateTime TransationDate {
            get => transationDate;
            set => SetPropertyValue(nameof(TransationDate), ref transationDate, value);
        }
        public decimal Price {
            get => price;
            set => SetPropertyValue(nameof(Price), ref price, value);
        }
        public int Amount {
            get => amount;
            set => SetPropertyValue(nameof(Amount), ref amount, value);
        }
        
        [Size(SizeAttribute.DefaultStringMappingFieldSize)]
        public string Comment {
            get => comment;
            set => SetPropertyValue(nameof(Comment), ref comment, value);
        }
        Position _position;
        [Association]
        public Position Position {
            get {
                return _position;
            }
            set {
                SetPropertyValue(nameof(Position), ref _position, value);
            }
        }

    }
}
