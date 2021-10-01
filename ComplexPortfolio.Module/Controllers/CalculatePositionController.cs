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

            var calcDataList = dayDataList.Where(x=>x.Date>=firstTransactionDay).OrderBy(x=>x.Date).Select(x=>new CalcPositionDatum(x)).ToList();

            int currentSharesCount = 0;
            decimal currentValue = 0;
            decimal currentValueTotal = 0;

            foreach(var calcData in calcDataList) {
                calcData.SharesCount = currentSharesCount;
                calcData.Value = calcData.Price*calcData.SharesCount;
                calcData.ValueDiff = calcData.Value - currentValue;
                calcData.ValueDiffTotal = currentValueTotal+calcData.ValueDiff;
                var transactions = position.Transactions.Where(x => x.Date == calcData.Date).ToList();
                PopulateCalcDataWithTransactionsData(calcData, transactions);

                currentSharesCount = calcData.SharesCount;
                currentValue = calcData.Value;
                currentValueTotal = calcData.ValueDiffTotal;
            }
            position.CalculateData = calcDataList;
        }

        public void PopulateCalcDataWithTransactionsData(CalcPositionDatum dayData, List<Transaction> transactions) {
            foreach(var trans in transactions) {
                dayData.SharesCount += trans.Amount;
                dayData.Value += (trans.Amount * trans.Price);
            }
        }
    }


}
