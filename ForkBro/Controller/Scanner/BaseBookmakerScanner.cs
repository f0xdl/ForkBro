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
	public class BookmakerScanner : IWorker
	{
		public IBookmakerEvent[] events;
		IBookmakerScanner scanner;
		object idEventsLock;
		private Func<int> eventId;

		//IBookmakerScanner
		//BaseHttpRequest IBookmakerScanner.httpClient { get; set; }
		//TODO реализовать интерфейс IBookmakerScanner для favbet

		public BookmakerScanner(int countEvents = 100)
		{
			events = new IBookmakerEvent[countEvents];
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

		//Bookmaker event operation
		int GetEventID()
		{
			int id;
			lock (idEventsLock)
				try
				{
					id = Array.FindIndex(events, x => x.status == EStatusEvent.Undefined || x.status == EStatusEvent.Over);
				}
				catch (ArgumentNullException) { 
					throw new Exception("Список событий в сканере переполнен![" + scanner.BookmakerName + "]");
				}
			return id;
		}//Получить свободный элемент в массиве
		public bool GameExists(int betEventId) => events.Count(x => betEventId == x.EventID) > 0;//TODO Check result
		public ref IBookmakerEvent AddEvent(int idBetEvent)
		{
			int eventId;

			if (!GameExists(idBetEvent))
			{
				eventId = GetEventID();
				IBookmakerEvent _event = scanner.newEvent();
				_event.EventID = idBetEvent;
				events[eventId] = _event; 
			}
			else
				throw new Exception("Данное событие уже существует!");

			return ref events[eventId]; //возвращаем ссылку на ивент при удачном добавлении элемента
		}
		public void RemoveEvent(int idBetEvent)
		{
			foreach (var item in events.Where(x => x.EventID == idBetEvent))
				item.status = EStatusEvent.Over;
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
					await Task.Run(() => Parallel.ForEach(events, x => UpdateGameData(x.EventID)));
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
