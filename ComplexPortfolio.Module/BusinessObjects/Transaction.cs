using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComplexPortfolio.Module.BusinessObjects {
    public interface ITransaction {
        public DateTime Date { get; set; }
        public double Price { get; set; }
        public int Amount { get; set; }
        public IPosition Position { get; }
        public TransactionDirectionEnum Direction { get; }
    }

    [DefaultClassOptions]
    public class Transaction : BaseObject, ITransaction {
        public Transaction(Session session) : base(session) {
        }
        public override void AfterConstruction() {
            base.AfterConstruction();
            this.Date = DateTime.Today;
        }

        public Transaction(DateTime _transactionDate, int _amount, double _price, TransactionDirectionEnum _direction) {
            this.transationDate = _transactionDate;
            this.amount = _amount;
            this.price = _price;
            this.direction = _direction;
        }


        string comment;
        int amount;
        double price;

        DateTime transationDate;
        TransactionDirectionEnum direction;

        public TransactionDirectionEnum Direction {
            get => direction;
            set => SetPropertyValue(nameof(Direction), ref direction, value);
        }

        public DateTime Date {
            get => transationDate;
            set => SetPropertyValue(nameof(Date), ref transationDate, value);
        }
        public double Price {
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

                bool modified = SetPropertyValue(nameof(Position), ref _position, value);
                if(!IsLoading && !IsSaving && value != null && modified && this.Account == null) {
                    this.Account = value.Portfolio.DefaultAccount;
                }
            }
        }

        Account _account;
        [Association]
        public Account Account {
            get {
                return _account;
            }
            set {
                SetPropertyValue(nameof(Account), ref _account, value);
            }
        }

        [NonPersistent]
        public double CommonSum {
            get => Amount * Price;
            set => Price = value / Amount;
        }

        IPosition ITransaction.Position { get => Position; }
    }
}
