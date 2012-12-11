﻿using System;
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
				Message ="hello aicl.pubnub",
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
			/*

            //Initialize pubnub state
            Channel  objPubnub = new Channel(
               "demo",  // PUBLISH_KEY
                "demo",  // SUBSCRIBE_KEY
                "demo",  // SECRET_KEY
                "",      // CIPHER_KEY (Cipher key is Optional)
                false    // SSL_ON?
            );

            //channel name
            string channel = "my_channel";

            // Publish String Message
            Dictionary<string, object> args = new Dictionary<string, object>();
            args.Add("channel", channel);
            args.Add("message", "Hello Csharp - mono");
            List<object> info = null;

            // publish Response
            info = objPubnub.Publish(args);
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

            // Publish Message in array format
            args = new Dictionary<string, object>();
            object[] objArr = new object[7];

            objArr[0] = "Sunday";
            objArr[1] = "Monday";
            objArr[2] = "Tuesday";
            objArr[3] = "Wednesday";
            objArr[4] = "Thursday";
            objArr[5] = "Friday";
            objArr[6] = "Saturday";

            args.Add("channel", channel);
            args.Add("message", objArr);

            // publish Response
            info = objPubnub.Publish(args);
            if (info != null)
            {
                if (info.Count == 3) //success
                {
                    Console.WriteLine("[" + info[0].ToString() + ", " + info[1] + ", " + info[2] + "]");
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

            // Publish message in Dictionary format
            args = new Dictionary<string, object>();
            Dictionary<string, object> objDict = new Dictionary<string, object>();
            Dictionary<string, object> val1 = new Dictionary<string, object>();
            objDict.Add("Student", "Male");
            val1.Add("Name", "Jhon");
            val1.Add("Age", 2);
            objDict.Add("Info", val1);

            args.Add("channel", channel);
            args.Add("message", objDict);

            // publish Response
            info = objPubnub.Publish(args);
            if (info != null)
            {
                if (info.Count == 3) //success
                {
                    Console.WriteLine("[" + info[0].ToString() + ", " + info[1] + ", " + info[2] + "]");
                }
                else if (info.Count == 2) //error
                {
                    Console.WriteLine("[" + info[0].ToString() + ", " + info[1] +  "]");
                }
            }
            else
            {
                Console.WriteLine("Error in network connection");
            }

            Console.ReadKey();
            */
        }
    }
}
