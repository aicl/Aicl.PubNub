using System;
using System.IO;
using System.Net;
using System.Text;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Web.Script.Serialization;
using System.Linq;
using Newtonsoft.Json.Linq;
using System.Threading;
using System.IO.Compression;

/**
 * PubNub 3.1 Real-time Push Cloud API
 *
 * @author Stephen Blum
 * @package pubnub
 */

namespace Aicl.PubNub
{
    public class ChannelStatus 
    {
        
		public string Channel {get ; set;}

		public bool Connected {get;set;}

		public bool First {get;set;}

    }
    
}
