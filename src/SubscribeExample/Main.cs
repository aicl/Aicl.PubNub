using System;
using System.Linq;
using Aicl.PubNub;

namespace Csharp
{
    class SubscribeExample
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

			SubscribeParams sp = new SubscribeParams{
				ChannelName = channel,
				Receiver= o=>{
					Console.WriteLine("Incoming message:");
					Console.WriteLine(o);
					Console.WriteLine(o.GetType());
					return true;
				},
				ConnectCallback = o=>{
					Console.WriteLine("ConnectCallback:");
					Console.WriteLine(o);
                	return true;
				},
				DisConnectCallback = o=>{
					Console.WriteLine("DisConnectCallback:");
					Console.WriteLine(o);
                	return true;
				},
				ReConnectCallback = o=>{
					Console.WriteLine("ReConnectCallback:");
					Console.WriteLine(o);
                	return true;
				},
				ErrorCallback = o=>{
					Console.WriteLine("ErrorCallback:");
					Console.WriteLine(o);
                	return true;
				}

			};

			pubChannel.Subscribe(sp);

        }

	}
	
}