using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp;
using DevExpress.Persistent.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ComplexPortfolio.Module.BusinessObjects;
using DevExpress.Data.Filtering;

namespace ComplexPortfolio.Module.Controllers {

     class SplitTIckersController : ObjectViewController<DetailView, Position> {
        public SplitTIckersController() {
            var splitTransationsAction = new SimpleAction(this, "SplitTransactions", PredefinedCategory.Edit);
            splitTransationsAction.Execute += SplitTransationsAction_Execute; ;
            
        }

        private void SplitTransationsAction_Execute(object sender, SimpleActionExecuteEventArgs e) {
            var pos =(Position) View.CurrentObject;
            foreach(var t in pos.Transactions) {
                t.Price = t.Price / 10;
                t.Amount = t.Amount * 10;
            }
            View.ObjectSpace.CommitChanges();
        }
    }
    class CheckTransationsController : ObjectViewController<ListView, Portfolio> {
        public CheckTransationsController() {
            var checkTransationsAction = new SimpleAction(this, "CheckTransaction", PredefinedCategory.Edit);
            checkTransationsAction.Execute += CheckTransactionsAction_Execute; ;
            checkTransationsAction.SelectionDependencyType = SelectionDependencyType.RequireSingleObject;
        }

        private void CheckTransactionsAction_Execute(object sender, SimpleActionExecuteEventArgs e) {
            var port = (Portfolio)View.SelectedObjects[0];
            foreach(var pos in port.Positions) {
                foreach(var trans in pos.Transactions) {
                    if(trans.Account == null) {
                        var test = 3;
                    }
                }
            }

        }
    }


    class MoveAllTransactionsToAccount : ObjectViewController<ListView, Portfolio> {
        public MoveAllTransactionsToAccount() {
            var moveTransactionsAction = new SimpleAction(this, "MoveTransactions", PredefinedCategory.Edit);
            moveTransactionsAction.Execute += MoveTransactionsAction_Execute; ;
            moveTransactionsAction.SelectionDependencyType = SelectionDependencyType.RequireSingleObject;
        }

        private void MoveTransactionsAction_Execute(object sender, SimpleActionExecuteEventArgs e) {
            var port = (Portfolio)View.SelectedObjects[0];
            var account = View.ObjectSpace.FindObject<Account>(new BinaryOperator(nameof(Account.Name), "Funny"));
            if(account != null) {
                foreach(var p in port.Positions) {
                    foreach(var t in p.Transactions) {
                        if(t.Account == null) {
                            t.Account = account;
                        }
                    }
                }
                View.ObjectSpace.CommitChanges();
            }

        }
    }


    class CopyPortfolioController : ObjectViewController<ListView, Portfolio> {
        public CopyPortfolioController() {
            var copyPortfolioAction = new SimpleAction(this, "CopyPortfolio", PredefinedCategory.Edit);
            copyPortfolioAction.Execute += CopyPortfolioAction_Execute;
            copyPortfolioAction.SelectionDependencyType = SelectionDependencyType.RequireSingleObject;
        }

        private void CopyPortfolioAction_Execute(object sender, SimpleActionExecuteEventArgs e) {
            var initialPortfolio = (Portfolio)View.SelectedObjects[0];
            var os = Application.CreateObjectSpace(typeof(Portfolio));
            var newPortfolio = os.CreateObject<Portfolio>();
            newPortfolio.Name = "NewPortFolio" + DateTime.Now.Millisecond;
            foreach(var pos in initialPortfolio.Positions) {
                var newPos = os.CreateObject<Position>();
                newPos.Comment = "copy " + pos.Ticker.Name;
                newPos.Ticker = os.GetObject(pos.Ticker);
                foreach(var trans in pos.Transactions) {
                    var newTrans = os.CreateObject<Transaction>();
                    newTrans.Direction = trans.Direction;
                    newTrans.Amount = trans.Amount;
                    newTrans.Date = trans.Date;
                    newTrans.Price = trans.Price;
                    newTrans.Comment = "copy";
                    newPos.Transactions.Add(newTrans);
                }
                newPortfolio.Positions.Add(newPos);
            }
            os.CommitChanges();
        }
    }
}
