using System;
using System.Collections.Generic;
using System.Linq;
using Aicl.PubNub;

namespace Csharp
{
    class HistoryExample
    {
        static public void Main()
        {
            //Initialize pubnub state
            PubChannel pubChannel = new PubChannel( new ChannelParams{
				PublishKey="demo",
				SubscribeKey="demo",
				SecretKey="demo"
			});

            //channel name
            string channel = "my_channel";

            // History
            
            List<object> history = pubChannel.History(new HistoryParams{
				ChannelName=channel,
				Limit=5
			});

			int i=0;
            foreach (object history_message in history)
            {
                Console.Write("History Message: ");
                Console.WriteLine(history_message);
				Console.WriteLine("---------------------  {0}  ----------", i++);
	
            }

            // Get PubNub Server Time
            object timestamp = pubChannel.Time();
            Console.WriteLine("Server Time: " + timestamp.ToString());
            
        }


    }
}