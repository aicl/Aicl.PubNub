using System;

namespace Aicl.PubNub
{
	public class SubscribeParams
	{
		public string ChannelName {get;set;}

		public Func<object,bool> Receiver {get;set;}

		public Func<object,bool> ConnectCallback {get;set;}

		public Func<object,bool> DisConnectCallback {get;set;}

		public Func<object,bool> ReConnectCallback {get;set;}

		public Func<object,bool> ErrorCallback {get;set;}

		internal object Timetoken {get;set;}

		public SubscribeParams ()
		{
			Receiver= f=> true;
			ConnectCallback= f=> false;
			DisConnectCallback= f=> false;
			ReConnectCallback= f=> false;
			ErrorCallback= f=> true;
			Timetoken=0;
		}
	}
}

