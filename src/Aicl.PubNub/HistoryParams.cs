using System;

namespace Aicl.PubNub
{
	public class HistoryParams
	{
		public string ChannelName {get;set;}

		public  int Limit  {get;set;}
		// Start End  Reverse ???
		public HistoryParams ()
		{
			Limit=100;
		}
	}
}

