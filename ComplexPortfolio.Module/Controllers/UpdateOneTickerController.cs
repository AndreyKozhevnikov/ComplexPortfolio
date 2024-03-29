﻿using ComplexPortfolio.Module.BusinessObjects;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.Persistent.Base;
using System;
using System.Collections.Generic;

namespace ComplexPortfolio.Module.Controllers {
    public class UpdateOneTickerController : ObjectViewController<DetailView, Ticker> {
        public UpdateOneTickerController() {
            var reloadAction = new SimpleAction(this, "ReloadData", PredefinedCategory.Edit);
            reloadAction.Execute += ReloadAction_Execute;

            var testAction = new SimpleAction(this, "Test", PredefinedCategory.Edit);
            testAction.Execute += TestAction_Execute;
        }

        private async void TestAction_Execute(object sender, SimpleActionExecuteEventArgs e) {
            var dataLoader = new GetTickerDataLoader();

            var res =await dataLoader.GetTickerData("tspx", DateTime.Today.AddDays(-20), DateTime.Today);
        }

        private void ReloadAction_Execute(object sender, SimpleActionExecuteEventArgs e) {
            var os = Application.CreateObjectSpace(typeof(Ticker));
            var ticker = os.GetObject((Ticker)View.CurrentObject);
            ReloadAllDataForTicker(ticker, os);
        }

        public async void ReloadAllDataForTicker(Ticker ticker, IObjectSpace os) {
            os.Delete(ticker.DayData);
            os.CommitChanges();
            //todo
            var dataLoader = new GetTickerDataLoader();
            var tickerFactory = new TickerDayDataFactory();
            var workYear = 2020;
            HashSet<DateTime> existingDates = new HashSet<DateTime>();
            while(workYear <= DateTime.Today.Year) {
                var candles = await dataLoader.GetTickerYearData(ticker.Name, workYear);
                if(candles == null) {
                    return;
                }
                foreach(var c in candles) {
                    tickerFactory.CreateTickerDayDataFromCandle(ticker, c, existingDates, os, DateTime.Today);
                }
                workYear++;
            }
            os.CommitChanges();
        }
    }
}
