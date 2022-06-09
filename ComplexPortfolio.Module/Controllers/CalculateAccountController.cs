using ComplexPortfolio.Module.BusinessObjects;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.Persistent.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComplexPortfolio.Module.Controllers {
    public class CalculateAccountController : ViewController {
        public CalculateAccountController() {
            this.TargetObjectType = typeof(Account);
            var calcAccountAction = new SimpleAction(this, "CalcAccount", PredefinedCategory.Edit);
            calcAccountAction.SelectionDependencyType = SelectionDependencyType.RequireMultipleObjects;
            calcAccountAction.Execute += CalcAccountAction_Execute;
        }

        private void CalcAccountAction_Execute(object sender, SimpleActionExecuteEventArgs e) {
            var acc = (Account)View.CurrentObject;
            acc.Summary = CalculateAccountSummary(acc.Transactions.Cast<ITransaction>().ToList());
        }

        public AccountSummary CalculateAccountSummary(IList<ITransaction> transactions) {
            var summ = new AccountSummary();
            foreach(var t in transactions) {
                var tickerData = summ.TickersData.Where(x => x.Ticker.Name == t.Position.Ticker.Name).FirstOrDefault();
                if(tickerData == null) {
                    tickerData = new AccountSummaryObject(t.Position.Ticker);
                    var maxDate = t.Position.Ticker.DayData.Max(x => x.Date);
                    tickerData.LastPrice = t.Position.Ticker.DayData.Where(x => x.Date == maxDate).First().Close;
                    summ.TickersData.Add(tickerData);
                }
                if(t.Direction == TransactionDirectionEnum.Buy) {
                    tickerData.Amount += t.Amount;
                } else {
                    tickerData.Amount -= t.Amount;
                }
                tickerData.CurrentValue = tickerData.Amount * tickerData.LastPrice;
            }

            return summ;
        }
    }
}
