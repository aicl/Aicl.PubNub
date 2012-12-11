namespace Aicl.PubNub
{
	public class ChannelParams
	{


		public string PublishKey {
			get;set;
		}
		        
		public string SubscribeKey {
			get;set;
		}

		public string SecretKey {
			get;set;
		}

		public string CipherKey {
			get;set;
		}

        
		public bool Ssl {
			get;set;
		}

		public ChannelParams ()	{}
	}
}

