using System.Threading;
using System.Collections.Generic;
using ServiceStack.Text;
using System.Text;
using System.Net;
using System;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Linq;

namespace Aicl.PubNub
{
	public class PubChannel
	{
		ChannelParams Params {get;set;}

		string origin = "pubsub.pubnub.com";
        ManualResetEvent webRequestDone;
		volatile  bool abort;
		List<ChannelStatus> subscriptions;

		public PubChannel (ChannelParams channelParams)
		{
			Params= channelParams;

			if (Params.Ssl)
            {
                origin = "https://" + origin;
            }
            else
            {
                origin = "http://" + origin;
            }
            webRequestDone = new ManualResetEvent(true);
		}

		public List<object> Publish(string channel, Dictionary<string, object> args)
		{
			var message = JsonSerializer.SerializeToString(args, typeof(object));
			return PublishImpl(channel, message);
		}

		public List<object> Publish(string channel, object[] args)
		{
			var message = JsonSerializer.SerializeToString(args, typeof(object[]));
			return PublishImpl(channel, message);
		}

		public List<object> Publish<T>(string channel, T request)
		{
			var message = JsonSerializer.SerializeToString<T>(request);
			return PublishImpl(channel, message);
		}

		public List<object> Publish<T>(string channel, IList<T> request)
		{
			var message = JsonSerializer.SerializeToString<IList<T>>(request);
			return PublishImpl(channel, message);
		}

		public List<object> Publish<T>(string channel, string message)
		{
			return PublishImpl(channel, JsonSerializer.SerializeToString<string>(message));
		}

		List<object> PublishImpl(string channel, string message){
			string signature = "0";
            if (!string.IsNullOrEmpty( Params.SecretKey) )
            {
                StringBuilder string_to_sign = new StringBuilder();
                string_to_sign
                    .Append(Params.PublishKey)
                    .Append('/')
                    .Append(Params.SubscribeKey)
                    .Append('/')
                    .Append(Params.SecretKey)
                    .Append('/')
                    .Append(channel)
                    .Append('/')
                    .Append( message);

                // Sign Message
                signature = GetHmacsha256(string_to_sign.ToString());
            }

            // Build URL
            List<string> url = new List<string>();
            url.Add("publish");
            url.Add(Params.PublishKey);
            url.Add(Params.SubscribeKey);
            url.Add(signature);
            url.Add(channel);
            url.Add("0");
            url.Add(message);

            // Return JSONArray
            return Request(url);
		}


		List<object> Request(List<string> urlComponents)
        {
            try
            {

                string temp = null;
                int count = 0;
                byte[] buf = new byte[8192];
                StringBuilder url = new StringBuilder();
                StringBuilder sb = new StringBuilder();               

                // Add Origin To The Request
                url.Append(origin);

                // Generate URL with UTF-8 Encoding
                foreach (string url_bit in urlComponents)
                {
                    url.Append("/");
                    url.Append(EncodeUrIcomponent(url_bit));
                }
                // Create Request
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url.ToString());

                // Set Timeout
                request.Timeout = 310000;
                request.ReadWriteTimeout = 310000;

                request.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip");
                request.Headers.Add("V", "3.1");
                request.UserAgent = "C#-Mono";

                webRequestDone.Reset();
                IAsyncResult asyncResult = request.BeginGetResponse(new AsyncCallback(RequestCallBack), null);
                webRequestDone.WaitOne();
                if (abort)
                {
                    return new List<object>();
                }

                // Receive Response
                HttpWebResponse response = (HttpWebResponse)request.EndGetResponse(asyncResult);

                // Read
                using (Stream stream = response.GetResponseStream())
                {
                    Stream resStream = stream;
                    if (response.ContentEncoding.ToLower().Contains("gzip"))
                    {
                        resStream = new GZipStream(stream, CompressionMode.Decompress);
                    }
                    else
                    {
                        resStream = stream;
                    }
                    do
                    {
                        count = resStream.Read(buf, 0, buf.Length);
                        if (count != 0)
                        {
                            temp = Encoding.UTF8.GetString(buf, 0, count);
                            sb.Append(temp);
                        }
                    } while (count > 0);
                }

                // Parse Response
                string message = sb.ToString();
				return JsonSerializer.DeserializeFromString<List<object>>(message);

                
            }
            catch (Exception)
            {
                List<object> error = new List<object>();
                if (urlComponents[0] == "time")
                {
                    error.Add("0");
                }
                else if (urlComponents[0] == "history")
                {
                    error.Add("Error: Failed JSONP HTTP Request.");
                }
                else if (urlComponents[0] == "publish")
                {
                    error.Add("0");
                    error.Add("Error: Failed JSONP HTTP Request.");
                }
                else if (urlComponents[0] == "subscribe")
                {
                    error.Add("0");
                    error.Add("0");
                }
                return error;
            }
        }


		string EncodeUrIcomponent(string s)
        {
            StringBuilder o = new StringBuilder();
            foreach (char ch in s.ToCharArray())
            {
                if (IsUnsafe(ch))
                {
                    o.Append('%');
                    o.Append(ToHex(ch / 16));
                    o.Append(ToHex(ch % 16));
                }
                else o.Append(ch);
            }
            return o.ToString();
        }

		void RequestCallBack(IAsyncResult result)
        {
            // release thread block
            webRequestDone.Set();
        }

		char ToHex(int ch)
        {
            return (char)(ch < 10 ? '0' + ch : 'A' + ch - 10);
        }

        bool IsUnsafe(char ch)
        {
            return " ~`!@#$%^&*()+=[]\\{}|;':\",./<>?".IndexOf(ch) >= 0;
        }

        static string GetHmacsha256(string text)
        {
            HMACSHA256 sha256 = new HMACSHA256();
            byte[] data = Encoding.Default.GetBytes(text);
            byte[] hash = sha256.ComputeHash(data);
            string hexaHash = "";
            foreach (byte b in hash) hexaHash += String.Format("{0:x2}", b);
            return hexaHash;
        }

		public bool VerifySignature(string signature, int timestamp, string token){
			return GetHmacsha256(string.Format("{0}{1}",timestamp, token))==signature;
		}

		public void Abort()
        {
            abort = true;
            webRequestDone.Set();
        }


		public void Subscribe(SubscribeParams subscribeParams)
        {

            bool is_disconnect = false;
            bool is_alreadyConnect = false;
                        
			string channel = subscribeParams.ChannelName;
                        
            // Ensure Single Connection
            if (subscriptions != null && subscriptions.Count > 0)
            {
                bool channel_exist = false;
                foreach (ChannelStatus cs in subscriptions)
                {
                    if (cs.Channel == channel)
                    {
                        channel_exist = true;
                        if (!cs.Connected)
                        {
                            cs.Connected = true;
                        }
                        else
                            is_alreadyConnect = true;
                        break;
                    }
                }
                if (!channel_exist)
                {
                    ChannelStatus cs = new ChannelStatus();
                    cs.Channel = channel;
                    cs.Connected = true;
                    subscriptions.Add(cs);
                }
                else if (is_alreadyConnect)
                {
					subscribeParams.ErrorCallback("Already Connected");
                    return;
                }
                
            }
            else
            {
                // New Channel
                ChannelStatus cs = new ChannelStatus();
                cs.Channel = channel;
                cs.Connected = true;
                subscriptions = new List<ChannelStatus>();
                subscriptions.Add(cs);
            }

            bool is_reconnected = false;
            //  Begin Recusive Subscribe
            while (true)
            {
                try
                {
                    // Build URL
                    List<string> url = new List<string>();
                    url.Add("subscribe");
                    url.Add(Params.SubscribeKey);
                    url.Add(channel);
                    url.Add("0");
                    url.Add(subscribeParams.Timetoken.ToString());

                    // Stop Connection?     
                    is_disconnect = false;
                    foreach (ChannelStatus cs in subscriptions)
                    {
                        if (cs.Channel == channel)
                        {
                            if (!cs.Connected)
                            {
                                subscribeParams.DisConnectCallback("Disconnected to channel : " + channel);
                                is_disconnect = true;
                                break;
                            }
                        }
                    }
                    if (is_disconnect)
                        return;

                    // Wait for Message
                    List<object> response = Request(url);

                    // Stop Connection?
                    foreach (ChannelStatus cs in subscriptions)
                    {
                        if (cs.Channel == channel)
                        {
                            if (!cs.Connected)
                            {
                                subscribeParams.DisConnectCallback("Disconnected to channel : " + channel);
                                is_disconnect = true;
                                break;
                            }
                        }
                    }
                    if (is_disconnect)
                        return;
                    // Problem?
                    if (response == null || response[1].ToString() == "0")
                    {
                        
                        for (int i = 0; i < subscriptions.Count(); i++)
                        {
                            ChannelStatus cs = subscriptions[i];
                            if (cs.Channel == channel)
                            {
                                subscriptions.RemoveAt(i);
                                subscribeParams.DisConnectCallback("Disconnected to channel : " + channel);
                            }
                        }

                        // Ensure Connected (Call Time Function)
                        while (true)
                        {
                            string time_token = Time().ToString();
                            if (time_token == "0")
                            {
                                // Reconnect Callback
                                subscribeParams.ReConnectCallback("Reconnecting to channel : " + channel);
                                Thread.Sleep(5000);
                            }
                            else
                            {
                                is_reconnected = true;
                                break;
                            }
                        }
                        if (is_reconnected)
                        {
                            break;
                        }
                    }
                    else
                    {
                        foreach (ChannelStatus cs in subscriptions)
                        {
                            if (cs.Channel == channel)
                            {
                                // Connect Callback
                                if (!cs.First)
                                {
                                    cs.First = true;
                                    subscribeParams.ConnectCallback("Connected to channel : " + channel);
                                    break;
                                }
                            }
                        }
                    }
                    // Update TimeToken
                    if (response[1].ToString().Length > 0)
                        subscribeParams.Timetoken = (object)response[1];

                    // Run user Callback and Reconnect if user permits.
                    object message = "";
					object[] o =	JsonSerializer.DeserializeFromString<object[]>(response[0].ToString());
                    foreach (object msg in o)
                    {                      
                        message = msg;   
                        if (!subscribeParams.Receiver(message)) return;
                    }
                }
                catch (Exception e )
                {
					subscribeParams.ErrorCallback(e);
                    Thread.Sleep(1000);
                }
            }
            if (is_reconnected)
            {
                // Reconnect Callback
                Subscribe(subscribeParams);
            }
        }

		public object Time()
        {
            List<string> url = new List<string>();

            url.Add("time");
            url.Add("0");

            List<object> response = Request(url);
            return response[0];
        }

		public List<object> History( HistoryParams historyParams)
        {
			string channel = historyParams.ChannelName;
            List<string> url = new List<string>();

            url.Add("history");
            url.Add(Params.SubscribeKey );
            url.Add(channel);
            url.Add("0");
            url.Add(historyParams.Limit.ToString());
            if (!string.IsNullOrEmpty(Params.CipherKey))
            {
                ClsCrypto pc = new ClsCrypto(Params.CipherKey);
                return pc.Decrypt(Request(url));
            }
            else
            {
				List<object> objTop = Request(url);
				return objTop;
            }
        }

	}
}

