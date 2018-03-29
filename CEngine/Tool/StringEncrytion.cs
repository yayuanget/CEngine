using System;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using UnityEngine;
namespace CEngine
{
    public class StringEncryption
    {
        #region 方法一 C#中对字符串加密解密（对称算法）
        private static byte[] Keys = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF };
        private static string secretKey = "TashanNet";
        public static string SecretKey { set { secretKey = value; } }
        ///<summary>
        /// DES加密字符串
        /// </summary>
        ///<param name = "encryptString" > 待加密的字符串 </param>
        ///<param name="encryptKey">加密密钥,要求为8位</param>
        ///<returns>加密成功返回加密后的字符串，失败返回源串</returns>
        public static string EncryptDES(string encryptString, string encryptKey)
        {
            try
            {
                byte[] rgbKey = Encoding.UTF8.GetBytes(encryptKey.Substring(0, 8));
                byte[] rgbIV = Keys;
                byte[] inputByteArray = Encoding.UTF8.GetBytes(encryptString);
                string str = "";

                DESCryptoServiceProvider dCSP = new DESCryptoServiceProvider();
                MemoryStream mStream = new MemoryStream();
                CryptoStream cStream = new CryptoStream(mStream, dCSP.CreateEncryptor(rgbKey, rgbIV), CryptoStreamMode.Write);
                cStream.Write(inputByteArray, 0, inputByteArray.Length);
                cStream.FlushFinalBlock();
                cStream.Close();
                str = Convert.ToBase64String(mStream.ToArray());
                //CDebug.LogError("EncryptDES " + str);
                return str;
            }
            catch
            {
                return encryptString;
            }
        }

        /// <summary>
        /// DES加密重构
        /// </summary>
        /// <param name="encryptString"></param>
        /// <returns></returns>
        public static string EncryptDES(string encryptString)
        {
            try
            {
                byte[] rgbKey = Encoding.UTF8.GetBytes(secretKey.Substring(0, 8));
                byte[] rgbIV = Keys;
                byte[] inputByteArray = Encoding.UTF8.GetBytes(encryptString);
                string str = "";

                DESCryptoServiceProvider dCSP = new DESCryptoServiceProvider();
                MemoryStream mStream = new MemoryStream();
                CryptoStream cStream = new CryptoStream(mStream, dCSP.CreateEncryptor(rgbKey, rgbIV), CryptoStreamMode.Write);
                cStream.Write(inputByteArray, 0, inputByteArray.Length);
                cStream.FlushFinalBlock();
                cStream.Close();
                str = Convert.ToBase64String(mStream.ToArray());
                //CDebug.LogError("EncryptDES " + str);
                return str;
            }
            catch
            {
                return encryptString;
            }
        }

        ///<summary>
        ///DES解密字符串
        ///</summary>
        /// <param name = "decryptString" > 待解密的字符串 </param >
        ///<param name="decryptKey">解密密钥,要求为8位,和加密密钥相同</param>
        ///<returns>解密成功返回解密后的字符串，失败返源串</returns>
        public static string DecryptDES(string decryptString, string decryptKey)
        {
            try
            {
                byte[] rgbKey = Encoding.UTF8.GetBytes(decryptKey);
                byte[] rgbIV = Keys;
                byte[] inputByteArray = Convert.FromBase64String(decryptString);
                string str = "";

                DESCryptoServiceProvider DCSP = new DESCryptoServiceProvider();
                MemoryStream mStream = new MemoryStream();
                CryptoStream cStream = new CryptoStream(mStream, DCSP.CreateDecryptor(rgbKey, rgbIV), CryptoStreamMode.Write);
                cStream.Write(inputByteArray, 0, inputByteArray.Length);
                cStream.FlushFinalBlock();
                cStream.Close();

                str = Encoding.UTF8.GetString(mStream.ToArray());
                //CDebug.LogError("DecryptDES " + str);
                return str;
            }
            catch
            {
                CDebug.Log("catch");
                return decryptString;
            }

        }

        /// <summary>
        /// DES解密重构
        /// </summary>
        /// <param name="decryptString"></param>
        /// <returns></returns>
        public static string DecryptDES(string decryptString)
        {
            try
            {
                byte[] rgbKey = Encoding.UTF8.GetBytes(secretKey);
                byte[] rgbIV = Keys;
                byte[] inputByteArray = Convert.FromBase64String(decryptString);
                string str = "";

                DESCryptoServiceProvider DCSP = new DESCryptoServiceProvider();
                MemoryStream mStream = new MemoryStream();
                CryptoStream cStream = new CryptoStream(mStream, DCSP.CreateDecryptor(rgbKey, rgbIV), CryptoStreamMode.Write);
                cStream.Write(inputByteArray, 0, inputByteArray.Length);
                cStream.FlushFinalBlock();
                cStream.Close();

                str = Encoding.UTF8.GetString(mStream.ToArray());
                //CDebug.LogError("DecryptDES " + str);
                return str;
            }
            catch
            {
                CDebug.Log("catch");
                return decryptString;
            }

        }

        #endregion

        #region
        // MD5不可逆加密
        //32位加密
        public string GetMD5_32(string s, string _input_charset)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] t = md5.ComputeHash(Encoding.GetEncoding(_input_charset).GetBytes(s));
            StringBuilder sb = new StringBuilder(32);

            for (int i = 0; i < t.Length; i++)
            {
                sb.Append(t[i].ToString("x").PadLeft(2, '0'));
            }

            return sb.ToString();
        }



        //16位加密
        public static string GetMd5_16(string ConvertString)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            string t2 = BitConverter.ToString(md5.ComputeHash(UTF8Encoding.Default.GetBytes(ConvertString)), 4, 8);
            t2 = t2.Replace("-", "");
            return t2;
        }

        //157即二进制的10011101，原AssetBundle中每一位都与之进行半加。  
        const Byte key = 157;


        //加密函数，这里是一个很简单的例子。更简单的方法还有在最后一位增加一个字符使Disunity解析文件失败等。  
        public static void Encypt(ref Byte[] targetData)
        {
            int dataLength = targetData.Length;
            for (int i = 0; i < dataLength; ++i)
            {
                //对targetData中的每一byte都和10011101做异或运算。  
                targetData[i] = (byte)(targetData[i] ^ key);
            }
        }

        #endregion
    }
}