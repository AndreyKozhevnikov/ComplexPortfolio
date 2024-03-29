﻿using ComplexPortfolio.Module.BusinessObjects;
using ComplexPortfolio.Module.Controllers;
using DevExpress.ExpressApp;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tinkoff.Trading.OpenApi.Models;

namespace Tests {
    [TestFixture]
    public class GetTickerDataFromAPIControllerTests {
        [Test]
        public void CreateTickerDayDataFromCandle_Exists() {
            //arrange
            var cnt = new TickerDayDataFactory();
            var d1 = new DateTime(2021, 10, 1);
            var osMock = new Mock<IObjectSpace>();
            osMock.Setup(x => x.CreateObject<TickerDayDatum>()).Returns(new TickerDayDatum());
            var ticker = new Ticker() { Name = "TestTicker" };
            var candle = new CandlePayload(0, 0, 0, 0, 0, d1, CandleInterval.Day, "test");
            HashSet<DateTime> existingDates = new HashSet<DateTime>();
            existingDates.Add(d1);
            //act

            cnt.CreateTickerDayDataFromCandle(ticker, candle, existingDates, osMock.Object,new DateTime(2021,10,20));
            //assert

            osMock.Verify(x => x.CreateObject<TickerDayDatum>(), Times.Never);
            Assert.AreEqual(1, existingDates.Count);
        }
        [Test]
        public void CreateTickerDayDataFromCandle_Exists_DiffTime() {
            //arrange
            var cnt = new TickerDayDataFactory();
            var d1 = new DateTime(2021, 10, 1);
            var d11 = new DateTime(2021, 10, 1, 15, 0, 0);
            var osMock = new Mock<IObjectSpace>();
            osMock.Setup(x => x.CreateObject<TickerDayDatum>()).Returns(new TickerDayDatum());
            var ticker = new Ticker() { Name = "TestTicker" };
            var candle = new CandlePayload(0, 0, 0, 0, 0, d11, CandleInterval.Day, "test");
            HashSet<DateTime> existingDates = new HashSet<DateTime>();
            existingDates.Add(d1);
            //act

            cnt.CreateTickerDayDataFromCandle(ticker, candle, existingDates, osMock.Object, new DateTime(2021, 10, 20));
            //assert

            osMock.Verify(x => x.CreateObject<TickerDayDatum>(), Times.Never);
            Assert.AreEqual(1, existingDates.Count);
        }
        [Test]
        public void CreateTickerDayDataFromCandle_DoesntExists() {
            //arrange
            var cnt = new TickerDayDataFactory();
            var osMock = new Mock<IObjectSpace>();
            osMock.Setup(x => x.CreateObject<TickerDayDatum>()).Returns(new TickerDayDatum());
            var d1 = new DateTime(2021, 10, 2);
            var ticker = new Ticker() { Name = "TestTicker" };
            var candle = new CandlePayload(0, 0, 0, 0, 0,d1, CandleInterval.Day, "test");
            HashSet<DateTime> existingDates = new HashSet<DateTime>();
            existingDates.Add(new DateTime(2021, 10, 1));
            //act

            cnt.CreateTickerDayDataFromCandle(ticker, candle, existingDates, osMock.Object, new DateTime(2021, 10, 20));
            //assert

            osMock.Verify(x => x.CreateObject<TickerDayDatum>(), Times.Once);
            Assert.AreEqual(2, existingDates.Count);
            Assert.AreEqual(true, existingDates.Contains(d1));
        }
        [Test]
        public void CreateTickerDayDataFromCandle_Today() {
            //arrange
            var cnt = new TickerDayDataFactory();
            var osMock = new Mock<IObjectSpace>();
            osMock.Setup(x => x.CreateObject<TickerDayDatum>()).Returns(new TickerDayDatum());
            var d1 = new DateTime(2021, 10, 20);
            var ticker = new Ticker() { Name = "TestTicker" };
            var candle = new CandlePayload(0, 0, 0, 0, 0, d1, CandleInterval.Day, "test");
            HashSet<DateTime> existingDates = new HashSet<DateTime>();
            existingDates.Add(new DateTime(2021, 10, 1));
            //act

            cnt.CreateTickerDayDataFromCandle(ticker, candle, existingDates, osMock.Object, new DateTime(2021, 10, 20));
            //assert

            osMock.Verify(x => x.CreateObject<TickerDayDatum>(), Times.Never);
            Assert.AreEqual(1, existingDates.Count);
            Assert.AreEqual(false, existingDates.Contains(d1));
        }
    }
}
