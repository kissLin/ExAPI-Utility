﻿using System;
using System.Security.Cryptography;
using System.Text;

namespace Lion.ExAPI.Utility.Commons {

    /// <summary>
    /// 加解密
    /// </summary>
    public class FunMD5 {
        /// <summary>
        /// 取得md5值
        /// </summary>
        /// <param name="strKey">md5key</param>
        /// <returns></returns>
        public static string GetMd5HashValue(string strKey) {
            string hash = string.Empty;
            using (MD5 md5Hash = MD5.Create()) {
                hash = GetMd5Hash(md5Hash, strKey);
            }
            return hash;
        }

        private static string GetMd5Hash(MD5 md5Hash, string input) {
            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();

            // Convert the input string to a byte array and compute the hash.
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

            // Loop through each byte of the hashed data
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++) {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }


        // Verify a hash against a string.
        private static bool VerifyMd5Hash(MD5 md5Hash, string input, string hash) {
            // Hash the input.
            string hashOfInput = GetMd5Hash(md5Hash, input);

            // Create a StringComparer an compare the hashes.
            StringComparer comparer = StringComparer.OrdinalIgnoreCase;

            if (0 == comparer.Compare(hashOfInput, hash)) {
                return true;
            } else {
                return false;
            }
        }
    }
}
