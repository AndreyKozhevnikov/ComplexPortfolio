using ComplexPortfolio.Module.BusinessObjects;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.Persistent.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComplexPortfolio.Module.Controllers {
    public class CalculatePositionController : ObjectViewController<DetailView, Position> {
        public CalculatePositionController() {
            var calculateAction = new SimpleAction(this, "Calculate", PredefinedCategory.Edit);
            calculateAction.Execute += AddDayDataAction_Execute;
        }

        private void AddDayDataAction_Execute(object sender, SimpleActionExecuteEventArgs e) {
            CalculatePosition(this.ViewCurrentObject, this.ObjectSpace);
        }

        public void CalculatePosition(Position position, IObjectSpace objectSpace) {
            var dayDataList = objectSpace.GetObjects<TickerDayDatum>(new BinaryOperator("Ticker.Name", position.Ticker.Name)).ToList();

            var firstTransactionDay = position.Transactions.Min(x => x.Date);

            var calcDataList = dayDataList.Where(x => x.Date >= firstTransactionDay).OrderBy(x => x.Date).Select(x => new CalcPositionDatum(x)).ToList();

            int currentSharesCount = 0;
            double currentValue = 0;
            double currentValueTotal = 0;
            List<TickerDayDatum> currencyDayDataList = null;
            if(position.Ticker.Currency != null) {
                currencyDayDataList = objectSpace.GetObjects<TickerDayDatum>(new BinaryOperator("Ticker.Name", position.Ticker.Currency.Name)).ToList();
            }
            foreach(var calcData in calcDataList) {
                calcData.Label = position.Label;
                calcData.SharesCount = currentSharesCount;
                calcData.Value = calcData.Price * calcData.SharesCount;
                double currencyValue;
                if(position.Ticker.Currency != null) {
                    currencyValue = currencyDayDataList.Where(x => x.Date == calcData.Date).First().Close;

                } else {
                    currencyValue = 1;
                }
                calcData.Value = calcData.Value * currencyValue;
                calcData.ValueDiff = calcData.Value - currentValue;
                calcData.ValueDiffTotal = currentValueTotal + calcData.ValueDiff;
                var transactions = position.Transactions.Where(x => x.Date == calcData.Date).ToList();
                PopulateCalcDataWithTransactionsData(calcData, transactions, currencyValue);
                // calcData.Value = calcData.Value * currencyValue;
                currentSharesCount = calcData.SharesCount;
                currentValue = calcData.Value;
                currentValueTotal = calcData.ValueDiffTotal;
            }
            position.CalculateData = calcDataList;
        }

        public void PopulateCalcDataWithTransactionsData(CalcPositionDatum dayData, List<Transaction> transactions, double currencyValue) {
            foreach(var trans in transactions) {
                dayData.SharesCount += trans.Amount;
                dayData.Value += (trans.Amount * trans.Price * currencyValue);
            }
        }
    }


}
