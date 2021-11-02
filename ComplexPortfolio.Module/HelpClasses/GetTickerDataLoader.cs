using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tinkoff.Trading.OpenApi.Models;
using Tinkoff.Trading.OpenApi.Network;

namespace ComplexPortfolio.Module.Controllers {
    public class GetTickerDataLoader {
        public GetTickerDataLoader() {
            var fileTokenName = @"c:\Skt\ttoken.txt";
            var file = new System.IO.StreamReader(fileTokenName);
            var token = file.ReadLine();
            file.Close();
            // для работы в песочнице используйте GetSandboxConnection
            var connection = ConnectionFactory.GetSandboxConnection(token);
            context = connection.Context;
        }

        public async Task<List<CandlePayload>> GetTickerYearData(string tickerName, int year) {
            var d1 = new DateTime(year, 1, 1);
            var d2 = new DateTime(year, 12, 31);
            if(year == DateTime.Today.Year) {
                d2 = DateTime.Today;//.AddDays(-1);
            }
            return await GetTickerData(tickerName, d1, d2);
        }

        SandboxContext context;
        public async Task<List<CandlePayload>> GetTickerData(string tickerName, DateTime startDate, DateTime finishDate) {
            // вся работа происходит асинхронно через объект контекста
            //  await context.RegisterAsync(Tinkoff.Trading.OpenApi.Models.BrokerAccountType.Tinkoff);
            //var portfolio = await context.PortfolioAsync();
            //var d1 = new DateTime(2020, 7, 1);
            //var d2 = new DateTime(2021, 6, 30);
            //var test = await context.MarketCurrenciesAsync();
            //USD000UTSTOM
            var figiAccs = await context.MarketSearchByTickerAsync(tickerName);

            if(figiAccs.Instruments.Count == 0) {
                return null;
            }
            var figi = figiAccs.Instruments[0].Figi;
            var data = await context.MarketCandlesAsync(figi, startDate, finishDate, Tinkoff.Trading.OpenApi.Models.CandleInterval.Day);
            return data.Candles;
        }
    }
}
