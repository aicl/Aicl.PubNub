using System;
using System.Collections.Generic;
using System.Linq;
using Aicl.PubNub;

namespace Csharp
{
	public class TestMessage{

		public int Id {get;set;}
		public string Message{get;set;}
		public DateTime Date {get;set;}
		public bool SomeBool {get;set;}

	}


    class PublishExample
    {
        static public void Main()
        {


			Console.WriteLine("HOla mundo");

			PubChannel pubnub = new PubChannel( new ChannelParams{
				PublishKey="demo",
				SubscribeKey="demo",
				SecretKey="demo"
			});

			string channel= "my_channel";

			List<object> info = null;
			info= pubnub.Publish(channel, "Hola Mundo soy Aicl.PubNub");
			if (info != null)
            {
                if (info.Count == 3) //success
                {
                    Console.WriteLine("[ " + info[0].ToString() + ", " + info[1] + ", " + info[2] + "]");
                }
                else if (info.Count == 2) //error
                {
                    Console.WriteLine("[" + info[0].ToString() + ", " + info[1] + "]");
                }
            }
            else 
            {
                Console.WriteLine("Error in network connection");
            }


			TestMessage tm = new TestMessage{
				Id= 10,
				Message ="hello aicl.pubnub New Publish",
				SomeBool =true,
				Date = DateTime.Today
			};


			info= pubnub.Publish<TestMessage>(channel, tm);
			if (info != null)
            {
                if (info.Count == 3) //success
                {
                    Console.WriteLine("[ " + info[0].ToString() + ", " + info[1] + ", " + info[2] + "]");
                }
                else if (info.Count == 2) //error
                {
                    Console.WriteLine("[" + info[0].ToString() + ", " + info[1] + "]");
                }
            }
            else 
            {
                Console.WriteLine("Error in network connection");
            }


			List<TestMessage> ltm = new List<TestMessage>();
			ltm.Add(tm);

			tm = new TestMessage{
				Id= 20,
				Message ="hello aicl.pubnub id = 20",
				Date = DateTime.Today.AddDays(10)
			};

			ltm.Add(tm);

			info= pubnub.Publish<TestMessage>(channel, ltm);
			if (info != null)
            {
                if (info.Count == 3) //success
                {
                    Console.WriteLine("[ " + info[0].ToString() + ", " + info[1] + ", " + info[2] + "]");
                }
                else if (info.Count == 2) //error
                {
                    Console.WriteLine("[" + info[0].ToString() + ", " + info[1] + "]");
                }
            }
            else 
            {
                Console.WriteLine("Error in network connection");
            }

			Console.WriteLine("This is The End");

        }


    }
}
