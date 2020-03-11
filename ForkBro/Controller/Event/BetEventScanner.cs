
using ForkBro.Controller.Client;
using ForkBro.Model;
using ForkBro.Model.EventModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ForkBro.Controller.Event
{

	public class BetEventScanner : IWorker
	{
		Dictionary<EBookmakers, List<EventPool>> events;
		Dictionary<EBookmakers, BaseHttpRequest> httpClients;
		Queue<EventPool> changes;

		//Manager methods
		public BetEventScanner()
		{
			events = new Dictionary<EBookmakers, List<EventPool>>();
			changes = new Queue<EventPool>();

		}
		public void UpdateBookmakers(EBookmakers[] bookmakers)
		{
			//Удаление отсутсвующих букмекеров
			foreach (var item in events)
				if (!bookmakers.Contains(item.Key))
					events.Remove(item.Key);

			//Добавление новых букмекеров
			foreach (EBookmakers bm in bookmakers)
				if (!events.ContainsKey(bm))
					events.Add(bm, new List<EventPool>());

			//Добавление парсеров
			httpClients = new Dictionary<EBookmakers, BaseHttpRequest>();
			foreach (var bm in events)
				httpClients[bm.Key] = BaseHttpRequest.GetHttpRequest(bm.Key);
		}
		void GetEventChanges(EBookmakers bookmaker)
		{
			try
			{
				//Выполнить запрос онлайн событий на букмекере
				List<EventPool> newBetEvents = httpClients[bookmaker].GetListEvent();

				//Клонирование текущего списка событий и установка флага обновления
				events[bookmaker].ForEach(x => x.updated = false);

				//Удалить события, которых нет в текущей выборке
				for (int i = 0; i < events[bookmaker].Count; i++)
				{
					for (int n = 0; n < newBetEvents.Count; n++)
						if (newBetEvents[n].id == events[bookmaker][i].id)
						{
							events[bookmaker][i].updated = newBetEvents[n].updated = true;
							break;
						}

					if (!events[bookmaker][i].updated)
						CloseEvent(events[bookmaker][i]);
				}
				//Добавить событие если его нет в старом списке
				for (int n = 0; n < newBetEvents.Count; n++)
					if (!newBetEvents[n].updated)
						AddEvent(newBetEvents[n]);
			}
			catch (Exception ex)
			{
				Console.WriteLine("\n\n" + ex.ToString());
			}
		}

		//IWorker
		bool IWorker.IsWork { get; set; }
		Thread IWorker.thread { get; set; }
		public void ScannerStart(int updatePeriod) => ((IWorker)this).StartWork(updatePeriod);
		public void ScannerStop(int ms_wait) => ((IWorker)this).StopWork(ms_wait);
		async void IWorker.Work(object delay)
		{
			while (((IWorker)this).IsWork)
				try
				{
					await Task.Run(() => Parallel.ForEach(httpClients.Keys, x => GetEventChanges(x)));
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

		//Event action
		public EventPool GetNextEvent() => changes.Dequeue();
		void AddEvent(EventPool ev)
		{
			ev.status = EStatusEvent.New;
			for (int i = 0; i < events[ev.bookmaker].Count; i++)
				if (events[ev.bookmaker][i].id == ev.id)
				{
					ev.updated = true;
					events[ev.bookmaker][i] = ev;
					break;
				}

			if (!ev.updated)
			{
				ev.updated = true;
				events[ev.bookmaker].Add(ev); 
			}

			changes.Enqueue(ev);
		}
		void CloseEvent(EventPool ev)
		{
			//Подстановка даты закрытия при отсутствии
			//if (ev.dtEnd == null)
			//	ev.dtEnd = DateTime.Now;

			//Удаление из массива
			ev.status = EStatusEvent.Over;
			for (int i = 0; i < events[ev.bookmaker].Count; i++)
				if (events[ev.bookmaker][i].id == ev.id)
				{
					events[ev.bookmaker].RemoveAt(i);
					break;
				}

			changes.Enqueue(ev);

		}

	}
}
