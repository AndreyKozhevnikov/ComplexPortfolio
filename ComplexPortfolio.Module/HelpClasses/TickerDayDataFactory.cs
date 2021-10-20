using ComplexPortfolio.Module.BusinessObjects;
using DevExpress.ExpressApp;
using System;
using System.Collections.Generic;
using Tinkoff.Trading.OpenApi.Models;

namespace ComplexPortfolio.Module.Controllers {
    public class TickerDayDataFactory {
        public void CreateTickerDayDataFromCandle(Ticker ticker, CandlePayload candle, HashSet<DateTime> existingDataDates, IObjectSpace os, DateTime today) {
            if(existingDataDates.Contains(candle.Time.Date) || candle.Time.Date == today) {
                return;
            }
            var dayData = os.CreateObject<TickerDayDatum>();
            dayData.Ticker = ticker;
            dayData.Date = candle.Time.Date;
            dayData.Open = (double)candle.Open;
            dayData.High = (double)candle.High;
            dayData.Low = (double)candle.Low;
            dayData.Close = (double)candle.Close;
            dayData.Volume = (double)candle.Volume;
            existingDataDates.Add(dayData.Date);
        }
    }
}
