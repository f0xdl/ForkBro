using ForkBro.Common;
using ForkBro.OnlineScanner.EventLinks;
using System;
using System.Linq;

namespace ForkBro.Mediator
{
	public struct Pool
	{
		public long id;
		public Bookmaker bookmaker;
		public Sport sport;
		public Command[] commands;
		public bool updated;
		public StatusEvent status;
		public BookmakerEvent[] events;

		public bool BookmakerHasUpdate => events.Count(x => x.status == StatusEvent.Updated) > 0;
	}
}