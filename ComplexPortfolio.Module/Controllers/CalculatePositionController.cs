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
            double currentProfitTotal = 0;
            double prevPrice = 0;
            List<TickerDayDatum> currencyDayDataList = null;
            if(position.Ticker.Currency != null) {
                currencyDayDataList = objectSpace.GetObjects<TickerDayDatum>(new BinaryOperator("Ticker.Name", position.Ticker.Currency.Name)).ToList();
            }
            foreach(var calcData in calcDataList) {
                calcData.Label = position.Label;
                var startSharesCount = currentSharesCount;
                var transactions = position.Transactions.Where(x => x.Date == calcData.Date).ToList();

                var transactionInShares = 0;
                var transactionOutShares = 0;
                double transactionProfit = 0;
                foreach(var trans in transactions) {
                    if(trans.Direction == TransactionDirectionEnum.Buy) {
                        transactionInShares += trans.Amount;
                        transactionProfit += trans.Amount * (calcData.Price - trans.Price);
                    } else {
                        transactionOutShares += trans.Amount;
                        transactionProfit += trans.Amount * (trans.Price - prevPrice);
                    }
                }
                var baseShares = startSharesCount - transactionOutShares;
                calcData.SharesCount = startSharesCount + transactionInShares - transactionOutShares;
                calcData.Value = calcData.SharesCount * calcData.Price;

                var baseProfit = baseShares * (calcData.Price - prevPrice);
                calcData.Profit = baseProfit + transactionProfit;
                calcData.ProfitTotal = currentProfitTotal + calcData.Profit;
                currentSharesCount = calcData.SharesCount;
                currentProfitTotal = calcData.ProfitTotal;
                prevPrice = calcData.Price;
                double currencyValue;
                if(position.Ticker.Currency != null) {
                    currencyValue = currencyDayDataList.Where(x => x.Date == calcData.Date).First().Close;
                    calcData.Value = calcData.Value * currencyValue;
                    calcData.Profit = calcData.Profit * currencyValue;
                    calcData.ProfitTotal = calcData.ProfitTotal * currencyValue;
                }
            }
            position.CalculateData = calcDataList;
            position.Summary = CalculatePositionSummary(position.Transactions.OrderBy(x => x.Date).ToList(), position.Ticker);
        }

        public PositionSummary CalculatePositionSummary(List<Transaction> transactions, ITicker ticker) {
            var summary = new PositionSummary();


            var inputPosition = new List<Tuple<int, double>>();
            double fixedProfit = 0;
            foreach(var trans in transactions) {
                switch(trans.Direction) {
                    case TransactionDirectionEnum.Buy:
                        inputPosition.Add(new Tuple<int, double>(trans.Amount, trans.Price));
                        break;
                    case TransactionDirectionEnum.Sell:
                        var transAmount = trans.Amount;
                        while(transAmount > 0) {
                            var firstInput = inputPosition[0];
                            var amount = firstInput.Item1;
                            var price = firstInput.Item2;
                            if(transAmount >= amount) {
                                inputPosition.Remove(firstInput);
                                transAmount -= amount;
                                fixedProfit += (amount * (trans.Price - price));
                            } else {
                                var newAmount = amount - transAmount;
                                inputPosition[0] = new Tuple<int, double>(newAmount, price);
                                fixedProfit += (transAmount * (trans.Price - price));
                                transAmount = 0;

                            }
                        }
                        break;
                }
            }
            summary.SharesCount = inputPosition.Sum(x => x.Item1);
            summary.InputValue = inputPosition.Sum(x => x.Item1 * x.Item2);
            summary.AveragePrice = summary.InputValue / summary.SharesCount;
            summary.FixedProfit = fixedProfit;

            if(ticker.DayData != null && ticker.DayData.Count > 0) {
                var maxDate = ticker.DayData.Max(x => x.Date);
                var lastPrice = ticker.DayData.Where(x => x.Date == maxDate).First().Close;
                double _lastRubPrice = 0;
                if(ticker.Currency != null) {
                    var _lastCurrencyPrice = ticker.Currency.DayData.Where(x => x.Date == maxDate).FirstOrDefault().Close;
                    _lastRubPrice = lastPrice * _lastCurrencyPrice;
                } else {
                    _lastRubPrice = lastPrice;
                }
                summary.LastPrice = lastPrice;
                summary.LastPriceRub = _lastRubPrice;
                summary.CurrentValue = _lastRubPrice * summary.SharesCount;
            }
            return summary;
        }

        public void PopulateCalcDataWithTransactionsData(CalcPositionDatum dayData, List<Transaction> transactions, double currencyValue) {
            foreach(var trans in transactions) {
                if(trans.Direction == TransactionDirectionEnum.Buy) {
                    dayData.SharesCount += trans.Amount;
                    dayData.Value += (trans.Amount * trans.Price * currencyValue);
                } else {
                    dayData.SharesCount -= trans.Amount;
                    dayData.Value -= (trans.Amount * trans.Price * currencyValue);
                }
            }
        }
    }


}
