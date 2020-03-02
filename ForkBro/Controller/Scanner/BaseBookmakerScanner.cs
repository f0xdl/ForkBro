using ForkBro.Controller;
using ForkBro.Controller.Client;
using ForkBro.Model;
using ForkBro.Model.EventModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ForkBro.Controller.Scanner
{
	public class BookmakerScanner : IWorker,IBookmakerScanner
	{
		//private HttpClient client;
		private Dictionary<int, IGame> games;
		IBookmakerScanner scanner;

		//IBookmakerScanner
		//BaseHttpRequest IBookmakerScanner.httpClient { get; set; }
		//TODO реализовать интерфейс IBookmakerScanner для favbet

		public BookmakerScanner()
		{
			games = new Dictionary<int, IGame>();
		}
		public void SetScanner(EBookmakers item)
		{
			switch (item)
			{
				case EBookmakers._1xbet:
					scanner = new Scanner_1xbet(); break;
				case EBookmakers._favbet:
					scanner = new Scanner_1xbet(); break;
				default: break;
			}
		}
		void UpdateGameData(int key) { throw new System.Exception("Not implemented"); }

		//Game operation
		public bool GameExists(BetEvent betEvent) => games.Where(x => betEvent.id == x.Value.EventID).Count() > 0;//TODO Check result
		public long AddGame(BetEvent betEvent)
		{
			throw new System.Exception("Not implemented");
		}
		public long RemoveGame(BetEvent betEvent)
		{
			throw new System.Exception("Not implemented");
		}


		//IWorker
		bool IWorker.IsWork { get; set; }
		Thread IWorker.thread { get; set; }

		public void Start(int updatePeriod) => ((IWorker)this).StartWork(updatePeriod);
		public void Stop(int ms_wait) => ((IWorker)this).StopWork(ms_wait);
		async void IWorker.Work(object delay)
		{
			while (((IWorker)this).IsWork)
				try
				{
					await Task.Run(() => Parallel.ForEach(games, x => UpdateGameData(x.Key)));
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex.Message);
				}
				finally
				{
					Thread.Sleep(TimeSpan.FromMilliseconds((int)delay));
				}
		}
	}
}
