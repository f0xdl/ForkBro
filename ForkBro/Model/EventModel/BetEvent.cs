using System;
namespace ForkBro.Model.EventModel
{
	public class BetEvent
	{
		public int id;
		public EBookmakers bookmaker;
		public ESport sport;
		public EBetType betType;
		public Command[] commands;
		//public DateTime dtStart;
		//public DateTime dtEnd;
		public bool updated;
		public EStatusEvent status;

	}
}