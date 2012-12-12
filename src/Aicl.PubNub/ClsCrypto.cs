using System;
using System.Text;
using System.Security.Cryptography;

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


		 //encrypt string
        public string Encrypt(string plainStr)
        {
            return EncryptOrDecrypt(true, plainStr);
        }


       //md5 used for AES encryption key
        static byte[] Md5(string cipherKey)
        {
            MD5 obj = new MD5CryptoServiceProvider();
            byte[] data = Encoding.Default.GetBytes(cipherKey);
            return obj.ComputeHash(data);
        }
    }
}




