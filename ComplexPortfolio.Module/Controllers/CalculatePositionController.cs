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
            CalculatePosition(this.ViewCurrentObject);
        }

        public List<CalcPositionDatum> CalculatePositionData(IPosition position) {
            var dayDataList = position.Ticker.DayData;

            var firstTransactionDay = position.Transactions.Min(x => x.Date);

            var calcDataList = dayDataList.Where(x => x.Date >= firstTransactionDay).OrderBy(x => x.Date).Select(x => new CalcPositionDatum(x)).ToList();

            int currentSharesCount = 0;
            double currentProfitTotal = 0;
            double prevPrice = 0;
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
                double tmpCurrencyValue = 0;
                if(position.Ticker.Currency != null) {
                    var currCurrencyValue = position.Ticker.Currency.DayData.Where(x => x.Date == calcData.Date).FirstOrDefault();
                    if(currCurrencyValue != null) {
                        currencyValue = currCurrencyValue.Close;
                        tmpCurrencyValue = currCurrencyValue.Close;
                    } else {
                        currencyValue = tmpCurrencyValue;
                    }
                    calcData.Value = calcData.Value * currencyValue;
                    calcData.Profit = calcData.Profit * currencyValue;
                    calcData.ProfitTotal = calcData.ProfitTotal * currencyValue;
                }
            }
            return calcDataList;
        }


        public void CalculatePosition(IPosition position) {
            position.CalculateData = CalculatePositionData(position);
            position.Summary = CalculatePositionSummary(position);
        }

        public PositionSummary CalculatePositionSummary(IPosition position) {

            var transactions = position.Transactions.OrderBy(x => x.Date).ToList();
            var ticker = position.Ticker;

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
            summary.FixedProfit = fixedProfit;
            summary.SharesCount = inputPosition.Sum(x => x.Item1);
            summary.InputValue = inputPosition.Sum(x => x.Item1 * x.Item2);
            summary.AveragePrice = summary.InputValue / summary.SharesCount;

            double lastCurrencyPrice = 1;
            DateTime maxDate = DateTime.MinValue;
            if(ticker.DayData != null && ticker.DayData.Count > 0) {//todo: only for tests? remove?
                maxDate = ticker.DayData.Max(x => x.Date);
                var lastPrice = ticker.DayData.Where(x => x.Date == maxDate).First().Close;
                summary.LastPrice = lastPrice;
            }
            if(ticker.IsBlocked) {
                summary.LastPrice = 0;
            }
            summary.LastPriceRub = summary.LastPrice;
            summary.CurrentValue = summary.LastPrice * summary.SharesCount;
            if(ticker.Currency != null) {
                lastCurrencyPrice = GetLastCurrencyPrice(ticker.Currency.DayData, maxDate);
                summary.LastPriceRub = summary.LastPrice * lastCurrencyPrice;
                summary.FixedProfit = summary.FixedProfit * lastCurrencyPrice;
                summary.InputValue = summary.InputValue * lastCurrencyPrice;
                summary.AveragePrice = summary.AveragePrice * lastCurrencyPrice;
                summary.CurrentValue = summary.CurrentValue * lastCurrencyPrice;
            }
            summary.VirtualProfit = summary.CurrentValue - summary.InputValue;

            summary.TotalProfit = summary.FixedProfit + summary.VirtualProfit;
            summary.TotalProfitPercent = summary.TotalProfit / summary.InputValue;
            return summary;
        }

        public double GetLastCurrencyPrice(List<ITickerDayDatum> data, DateTime maxDate) {
            var tmpDate = data.Where(x => x.Date == maxDate).FirstOrDefault();
            double result = 0;
            if(tmpDate != null) {
                result = tmpDate.Close;
            } else {
                var sorted = data.OrderBy(x => x.Date).ToList();
                DateTime candidateDate=maxDate;
                ITickerDayDatum candidateResult;
                do {
                    candidateDate = candidateDate.AddDays(-1);
                    candidateResult = data.Where(x => x.Date == candidateDate).FirstOrDefault();
                } while(candidateResult == null);
                result = candidateResult.Close;
            }
            return result;

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
