using FrokBro.Model;
using System;
using System.Linq;

namespace ForkBro.Model
{
	public struct Pool
	{
		public long id;
		public Bookmaker bookmaker;
		public Sport sport;
		public BetType betType;
		public Command[] commands;
		public bool updated;
		public StatusEvent status;
		public BookmakerEvent[] events;

		public bool BookmakerHasUpdate => events.Count(x => x.status == StatusEvent.Updated) > 0;
	}
}