// <copyright file="Hash.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace VADAR.Helpers
{
    /// <summary>
    /// Has Helper.
    /// </summary>
    public class Hash
    {
        /// <summary>
        /// Convert long to decimal.
        /// </summary>
        /// <returns>decimal value.</returns>
        private static readonly char[] Padding = { '=' };

        /// <summary>
        /// ConvertToDecimal.
        /// </summary>
        /// <param name="value">value.</param>
        /// <returns>decimal.</returns>
        public static decimal ConvertToDecimal(long value)
        {
            return Convert.ToDecimal(value);
        }

        /// <summary>
        /// Hash string.
        /// </summary>
        /// <param name="input">input.</param>
        /// <returns>hashed string.</returns>
        public static string StringHashing(string input)
        {
            var sb = new StringBuilder();

            using (var hash = SHA256.Create())
            {
                var enc = Encoding.UTF8;
                var result = hash.ComputeHash(enc.GetBytes(input));

                foreach (var b in result)
                {
                    sb.Append(b.ToString("x2"));
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// Hash string to base64.
        /// </summary>
        /// <param name="input">input data.</param>
        /// <returns>base64 format data.</returns>
        public static string StringHashingToBase64(string input)
        {
            using var hash = SHA256.Create();
            var enc = Encoding.UTF8;
            var result = hash.ComputeHash(enc.GetBytes(input));
            return Convert.ToBase64String(result);
        }

        /// <summary>
        /// Convert date time to unix timestamp.
        /// </summary>
        /// <param name="date">date time.</param>
        /// <returns>unix timestamp.</returns>
        public static int ConvertDateTimeToUnixTimeStamp(DateTime date)
        {
            return (int)date.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
        }

        /// <summary>
        /// Convert unix timestamp date time.
        /// </summary>
        /// <param name="timeStamp">time stamp.</param>
        /// <returns>Date time.</returns>
        public static DateTime ConvertUnixTimeStampDateTime(int timeStamp)
        {
            return new DateTime(1970, 1, 1).AddSeconds(timeStamp).ToLocalTime();
        }

        /// <summary>
        /// Encrypt string to Bytes by AES.
        /// </summary>
        /// <param name="plainText">plain text.</param>
        /// <param name="key">key.</param>
        /// <returns>Aes string encrypted.</returns>
        public static byte[] EncryptStringToBytes_Aes(string plainText, byte[] key)
        {
            byte[] encrypted;
            byte[] iV;

            using var aesAlg = Aes.Create();
            aesAlg.Key = key;

            aesAlg.GenerateIV();
            iV = aesAlg.IV;

            aesAlg.Mode = CipherMode.CBC;

            var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

            // Create the streams used for encryption.
            using var msEncrypt = new MemoryStream();
            using var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write);
            using (var swEncrypt = new StreamWriter(csEncrypt))
            {
                // Write all data to the stream.
                swEncrypt.Write(plainText);
            }

            encrypted = msEncrypt.ToArray();

            var combinedIvCt = new byte[iV.Length + encrypted.Length];
            Array.Copy(iV, 0, combinedIvCt, 0, iV.Length);
            Array.Copy(encrypted, 0, combinedIvCt, iV.Length, encrypted.Length);

            // Return the encrypted bytes from the memory stream.
            return combinedIvCt;
        }

        /// <summary>
        /// Decrypt string encrypted Aes to plain text.
        /// </summary>
        /// <param name="cipherTextCombined">cipher text combined.</param>
        /// <param name="key">key.</param>
        /// <returns>string decrypted.</returns>
        public static string DecryptStringFromBytes_Aes(byte[] cipherTextCombined, byte[] key)
        {
            // Declare the string used to hold
            // the decrypted text.
            string plaintext;

            // Create an Aes object
            // with the specified key and IV.
            using var aesAlg = Aes.Create();
            aesAlg.Key = key;

            var iV = new byte[aesAlg.BlockSize / 8];
            var cipherText = new byte[cipherTextCombined.Length - iV.Length];

            Array.Copy(cipherTextCombined, iV, iV.Length);
            Array.Copy(cipherTextCombined, iV.Length, cipherText, 0, cipherText.Length);

            aesAlg.IV = iV;

            aesAlg.Mode = CipherMode.CBC;

            // Create a decrytor to perform the stream transform.
            var decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

            // Create the streams used for decryption.
            using var msDecrypt = new MemoryStream(cipherText);
            using var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
            using var srDecrypt = new StreamReader(csDecrypt);

            // Read the decrypted bytes from the decrypting stream
            // and place them in a string.
            plaintext = srDecrypt.ReadToEnd();

            return plaintext;
        }

        /// <summary>
        /// Encode the VADAReUrl64.
        /// </summary>
        /// <param name="bytes">bytes.</param>
        /// <returns>string.</returns>
        public static string EncodeBase64Url(byte[] bytes)
        {
            var base64Url = Convert.ToBase64String(bytes).TrimEnd(Padding).Replace('+', '-').Replace('/', '_');

            return base64Url;
        }

        /// <summary>
        /// DecodeBase64Url.
        /// </summary>
        /// <param name="vAdaRe64Url">VADARe64Url.</param>
        /// <returns>byte.</returns>
        public static byte[] DecodeBase64Url(string vAdaRe64Url)
        {
            var incoming = vAdaRe64Url.Replace('_', '/').Replace('-', '+');
            switch (vAdaRe64Url.Length % 4)
            {
                case 2: incoming += "=="; break;
                case 3: incoming += "="; break;
            }

            return Convert.FromBase64String(incoming);
        }
    }
}
