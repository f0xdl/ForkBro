using System;
using System.Linq;

namespace ForkBro.Model.EventModel
{
	public class EventPool
	{
		public long id;
		public EBookmakers bookmaker;
		public ESport sport;
		public EBetType betType;
		public Command[] commands;
		public bool updated;
		public EStatusEvent status;
		public BookmakerEvent[] events;

		public bool BookmakerHasUpdate => events.Count(x => x.status == EStatusEvent.Updated) > 0;
	}
}