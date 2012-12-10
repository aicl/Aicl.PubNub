using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using Newtonsoft.Json.Linq;

namespace Aicl.PubNub
{
    public class ClsCrypto
    {
        string cipherKey = "";
        public ClsCrypto(string cipherKey)
        {
            this.cipherKey = cipherKey;            
        }
        // Basic function for encrypt or decrypt a string 
        // for encrypt type=true
        // for decrypt type=false
        public string EncryptOrDecrypt(bool type,string plainStr)   
        {
            RijndaelManaged aesEncryption = new RijndaelManaged();   
            aesEncryption.KeySize = 256;             
            aesEncryption.BlockSize = 128;             
            aesEncryption.Mode = CipherMode.CBC;          
            aesEncryption.Padding = PaddingMode.PKCS7;
            aesEncryption.IV = ASCIIEncoding.UTF8.GetBytes("0123456789012345");
            aesEncryption.Key = Md5(this.cipherKey);
            if (type)
            {
                ICryptoTransform crypto = aesEncryption.CreateEncryptor();
                byte[] plainText = ASCIIEncoding.UTF8.GetBytes(plainStr);
                byte[] cipherText = crypto.TransformFinalBlock(plainText, 0, plainText.Length);
                return Convert.ToBase64String(cipherText);
            }
            else
            {
                ICryptoTransform decrypto = aesEncryption.CreateDecryptor();
                byte[] encryptedBytes = Convert.FromBase64CharArray(plainStr.ToCharArray(), 0, plainStr.Length);
                return ASCIIEncoding.UTF8.GetString(decrypto.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length));
            }
        }

       
        //decrypt string
        public string Decrypt(string cipherStr)
        {
            return EncryptOrDecrypt(false, cipherStr);
        } 


		// decrypt array of objects
		public JArray Decrypt(object[] cipherArr)
        {
            JArray plainArr = new JArray();

            foreach (var o in cipherArr)
            {
                if (o.GetType() == typeof(object[]))
                {
                    plainArr.Add(Decrypt((List<object>)o));
                }
                else if (o.GetType() == typeof(string))
                {
                    plainArr.Add(Decrypt((string)o));
                }
                else
                {
                    plainArr.Add(Decrypt((Dictionary<string, object>)o));
                }
            }

            return plainArr;
        }

        // decrypt list of objects for history function
        public List<object> Decrypt(List<object> cipherArr)
        {
            List<object> lstObj = new List<object>();
            foreach (object o in cipherArr)
            {
                if (o.GetType() == typeof(object[]))
                {
                    lstObj.Add(Decrypt((object[])o));
                }
                else if (o.GetType() == typeof(string))
                {
                    lstObj.Add(Decrypt(o.ToString()));
                }
                else
                {
                    lstObj.Add(Decrypt((Dictionary<string,object>)o));
                }
            }
            return lstObj;
        }


        // decrypt object with key value pair <string,object> format
        public JObject Decrypt(Dictionary<string, object> cipherObj)
        {
            JObject objPlain = new JObject();

            foreach (KeyValuePair<string, object> pair in cipherObj)
            {
                if (pair.Value.GetType() == typeof(string))
                {
                    objPlain.Add(pair.Key, Decrypt(pair.Value.ToString()));
                }
                else
                {
                    objPlain.Add(pair.Key, Decrypt((Dictionary<string, object>)pair.Value));
                }
            }
            return objPlain;
        }



		 //encrypt string
        public string Encrypt(string plainStr)
        {
            return EncryptOrDecrypt(true, plainStr);
        }

		// encrypt array of objects
        public object Encrypt(object[] plainArr)
        {
            object[] cipherArr = new object[plainArr.Count()];
            for (int i = 0; i < plainArr.Count(); i++)
            {
                cipherArr[i] = Encrypt((string)plainArr[i]);
            }
            return cipherArr;
        }

		 // encrypt object with key value pair <string,object> format
        public Dictionary<string, object> Encrypt(Dictionary<string, object> plainObj)
        {
            Dictionary<string, object> newDict = new Dictionary<string, object>();
            foreach (KeyValuePair<string, object> pair in plainObj)
            {
                if (pair.Value.GetType() == typeof(string))
                {
                    newDict.Add(pair.Key, Encrypt(pair.Value.ToString()));
                }
                else
                {
                    newDict.Add(pair.Key, Encrypt((Dictionary<string, object>)pair.Value));
                }
            }
            return newDict;
        }

       //md5 used for AES encryption key
        private static byte[] Md5(string cipherKey)
        {
            MD5 obj = new MD5CryptoServiceProvider();
            byte[] data = Encoding.Default.GetBytes(cipherKey);
            return obj.ComputeHash(data);
        }
    }
}

/*

public JsonArrayObjects Decrypt (object[] cipherArr)
		{
			JsonArrayObjects j = new JsonArrayObjects();

			foreach( object o in cipherArr){
				if(o.GetType()==typeof(string))
					j.Add(JsonObject.Parse( Decrypt( (string)o) ));
				else if(o.GetType()==typeof(object[]))
				{
					j.AddRange( Decrypt((List<JsonObject>)o));
				}
				else
                {
                    j.Add(Decrypt((Dictionary<string, object>)o));
                }
			}

			return j;
		}

        
		public List<JsonObject> Decrypt(List<JsonObject> cipherArr)
		{
			List<JsonObject> j = new List<JsonObject>();
			foreach( object o in cipherArr){
				if(o.GetType()==typeof(string))
					j.Add(JsonObject.Parse( Decrypt( (string)o) ));
				else if(o.GetType()==typeof(object[]))
				{
					j.AddRange( Decrypt((object[])o));
				}
				else
                {
                    j.Add(Decrypt((Dictionary<string, object>)o));
                }
			}

			return j;
		}

		public JsonObject Decrypt(Dictionary<string, object> cipherObj)
        {
            JsonObject objPlain = new JsonObject();

            foreach (KeyValuePair<string, object> pair in cipherObj)
            {
                if (pair.Value.GetType() == typeof(string))
                {
                    objPlain.Add(pair.Key, Decrypt(pair.Value.ToString()));
                }
                else
                {
                    objPlain.Add(pair.Key, Decrypt((Dictionary<string, object>)pair.Value).SerializeToString());
                }
            }
            return objPlain;
        }

*/


