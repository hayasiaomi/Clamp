using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Clamp.MUI.Helpers
{
    class PasswordHelper
    {
        public const String StrPermutation = "mikemike";
        public const Int32 BytePermutation1 = 0x19;
        public const Int32 BytePermutation2 = 0x59;
        public const Int32 BytePermutation3 = 0x17;
        public const Int32 BytePermutation4 = 0x41;

        /// <summary>
        /// 解密密码
        /// </summary>
        /// <param name="strData"></param>
        /// <returns></returns>
        public static string Encrypt(string strData)
        {
            try
            {
                string[] sInput = strData.Split("-".ToCharArray());
                byte[] data = new byte[sInput.Length];
                for (int i = 0; i < sInput.Length; i++)
                {
                    data[i] = byte.Parse(sInput[i], NumberStyles.HexNumber);
                }
                var des = new DESCryptoServiceProvider { Key = Encoding.ASCII.GetBytes(StrPermutation), IV = Encoding.ASCII.GetBytes(StrPermutation) };
                var desencrypt = des.CreateDecryptor();
                byte[] result = desencrypt.TransformFinalBlock(data, 0, data.Length);
                return Encoding.UTF8.GetString(result);
            }
            catch { return "解密出错！"; }
        }

        /// <summary>
        /// 加密密码
        /// </summary>
        /// <param name="strData"></param>
        /// <returns></returns>
        public static string Decrypt(string strData)
        {
            try
            {
                byte[] data = Encoding.UTF8.GetBytes(strData);
                var des = new DESCryptoServiceProvider { Key = Encoding.ASCII.GetBytes(StrPermutation), IV = Encoding.ASCII.GetBytes(StrPermutation) };
                var desencrypt = des.CreateEncryptor();
                byte[] result = desencrypt.TransformFinalBlock(data, 0, data.Length);
                return BitConverter.ToString(result);
            }
            catch
            {
                return "转换出错！";
            }
        }

        private static byte[] Encrypt(byte[] strData)
        {
            PasswordDeriveBytes passbytes = new PasswordDeriveBytes(StrPermutation, new byte[] { BytePermutation1, BytePermutation2, BytePermutation3, BytePermutation4 });

            MemoryStream memstream = new MemoryStream();
            Aes aes = new AesManaged();
            aes.Key = passbytes.GetBytes(aes.KeySize / 8);
            aes.IV = passbytes.GetBytes(aes.BlockSize / 8);

            CryptoStream cryptostream = new CryptoStream(memstream,
            aes.CreateEncryptor(), CryptoStreamMode.Write);
            cryptostream.Write(strData, 0, strData.Length);
            cryptostream.Close();
            return memstream.ToArray();
        }

        private static byte[] Decrypt(byte[] strData)
        {
            PasswordDeriveBytes passbytes = new PasswordDeriveBytes(StrPermutation, new byte[] { BytePermutation1, BytePermutation2, BytePermutation3, BytePermutation4 });

            MemoryStream memstream = new MemoryStream();
            Aes aes = new AesManaged();
            aes.Key = passbytes.GetBytes(aes.KeySize / 8);
            aes.IV = passbytes.GetBytes(aes.BlockSize / 8);

            CryptoStream cryptostream = new CryptoStream(memstream,
            aes.CreateDecryptor(), CryptoStreamMode.Write);
            cryptostream.Write(strData, 0, strData.Length);
            cryptostream.Close();
            return memstream.ToArray();
        }

    }
}
