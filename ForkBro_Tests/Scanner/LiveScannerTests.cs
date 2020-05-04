using Microsoft.VisualStudio.TestTools.UnitTesting;
using ForkBro.Scanner;
using System;
using System.Collections.Generic;
using System.Text;
using ForkBro.Mediator;
using ForkBro.Scanner.EventLinks;
using ForkBro.Common;
using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using ForkBro.Configuration;
using System.Threading;
using System.Diagnostics;

namespace ForkBro.Tests
{
    [TestClass()]
    public class LiveScannerTests
    {
        [TestMethod()]
        public void StartLiveScanner()
        {
            //Виды спорта для сканирования
            List<Sport> lSports = new List<Sport>();
            //--Выбор определённых видов спорта
            //lSports.Add(Sport.Basketball);
            //lSports.Add(Sport.Tennis);
            //--Выбор всех видов спорта
            foreach (Sport sport in Enum.GetValues(typeof(Sport)))
                lSports.Add(sport);
            lSports.ToArray();

            Bookmaker[] bookmakers = new Bookmaker[] { Bookmaker._1xbet, Bookmaker._favbet };
            CancellationTokenSource tokenSource = new CancellationTokenSource();
            CancellationToken cancellation = tokenSource.Token;

            LiveScanner liveScanner = LiveScannerInit(
                lSports.ToArray(),
               bookmakers,
                out TestScannerMediator mediator, 
                out ILogger<LiveScanner> logger);
            //Запуск
            liveScanner.StartAsync(cancellation);
            //Ожидание выполнения первой иттерации
            while (mediator.lastDtUpdate == DateTime.MinValue)
                Thread.Sleep(0);
            //Прекращение сканирования
            tokenSource.Cancel();

            //Логгирование результатов
            List<Bookmaker> BMresult = new List<Bookmaker>();
            logger.LogInformation($"Count Links = {mediator.Links.Count}");
            foreach (var item in mediator.Links)
            {
                if (!BMresult.Contains(item.Bookmaker))
                    BMresult.Add(item.Bookmaker);

                logger.LogInformation($"BM={item.Bookmaker.ToString()}, " +
                    $"Sport={item.Sport.ToString()}, " +
                    $"ID={item.Id}, " +
                    //$"Status={item.Status.ToString()}"
                    "");
            }

            //Assert
            Assert.IsTrue(bookmakers.Length == BMresult.Count);
        }

        public LiveScanner LiveScannerInit(Sport[] sports, Bookmaker[] BMs,out TestScannerMediator mediator,out ILogger<LiveScanner> logger) 
        {
            //Объявление необходимых переменных
            mediator = new TestScannerMediator();
            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddConsole();
            });
            logger = loggerFactory.CreateLogger<LiveScanner>();

            ISetting settings = new AppSettings();
            settings.LiveScanRepeat = 1000;
            settings.Companies = new List<BookmakersProp>();

            settings.TrackedSports = sports;

            foreach (var bm in BMs)
                settings.Companies.Add(new BookmakersProp()
                {
                    enable = true,
                    id = bm,
                    repeat = 1000,
                });

            return new LiveScanner(logger, mediator, settings);
        }
    }

    public class TestScannerMediator : IScannerMediator
    {
        public ConcurrentQueue<IEventLink> Links;
        public int ScannerDelay { get; set; }
        public DateTime lastDtUpdate;
        public TestScannerMediator()
        {
            Links = new ConcurrentQueue<IEventLink>();
            ScannerDelay = (int)TimeSpan.FromMinutes(1).TotalMilliseconds;
            lastDtUpdate = DateTime.MinValue;
        }
        
        public void EventEnqueue(IEventLink link) => Links.Enqueue(link);
        public Bookmaker[] GetBookmakers() => new Bookmaker[] { Bookmaker._1xbet, Bookmaker._favbet };
        public void UpdateScannerStatus() => lastDtUpdate = DateTime.Now; 

    }
}