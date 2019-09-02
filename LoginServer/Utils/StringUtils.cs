using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Assets.Util
{
    public static class StringUtils
    {
        public static int MIN_PLAYER_NAME_LENTH = 4;
        public static int MAX_PLAYER_NAME_LENTH = 14;
        private static string[] LIMIT_CHAR = new string[] { "%", ",", "*", "^", "#", "$", "&", ":", "_", "[", "]", "|" };

        public static bool ValidPlayerName(string playerName)
        {
            if (ValidStrLength(playerName, MIN_PLAYER_NAME_LENTH, MAX_PLAYER_NAME_LENTH))
            {
                return ValidNamePattern(playerName);
            }
            return false;
        }

        private static bool ValidNamePattern(string name)
        {
            if (IsBlank(name))
            {
                return false;
            }

            foreach (string element in LIMIT_CHAR)
            {
                if (name.Contains(element))
                {
                    return false;
                }
            }
            return true;
        }

        private static bool IsBlank(string name)
        {
            return string.IsNullOrEmpty(name);
        }

        public static bool IsAlphaNumericNoSpace(string data)
        {
            return Regex.IsMatch(data, @"^[a-zA-Z0-9]+$");
        }

        public static bool IsAlphaNumericWithSpaceSpace(string data)
        {
            return Regex.IsMatch(data, @"^[a-zA-Z0-9 ]+$");
        }

        public static bool IsAllowedUsername(string data)
        {
            return Regex.IsMatch(data, @"^[a-zA-Z0-9]+$") && ValidLenth(data.Length, 4, 14);
        }

        public static bool IsAllowedPassWord(string data)
        {
            return Regex.IsMatch(data, @"^[a-zA-Z0-9]+$") && !ValidLenth(data.Length, 4, 14);
        }

        private static bool ValidLenth(int length, int min, int max)
        {
            return length >= min && length <= max;
        }

        private static int ToEncode(string element, string encoding)
        {
            var enc1251 = Encoding.GetEncoding(encoding);
            return enc1251.GetBytes(element).Count();
        }
        private static int ToDefaultEncoding(string element)
        {
            return Encoding.UTF8.GetBytes(element).Count();
        }

        private static bool ValidStrLength(string element, int minLength, int maxLength)
        {
            try
            {
                return ValidLenth(ToEncode(element, "GB18030"), minLength, maxLength);
            }
            catch
            {
            }
            try
            {
                return ValidLenth(ToEncode(element, "GB2312"), minLength, maxLength);
            }
            catch
            {
            }
            try
            {
                return ValidLenth(ToEncode(element, "GBK"), minLength, maxLength);
            }
            catch
            {
            }
            return ValidLenth(ToDefaultEncoding(element), minLength, maxLength);
        }

        public static string GetHash(this string password)
        {
            return string.Join("", password.Select(c => ((int)c).ToString("X2")));
        }

        public static string CreateMD5(this string input)
        {
            using (MD5 md5 = MD5.Create())
            {
               byte[] hashBytes = md5.ComputeHash(Encoding.UTF8.GetBytes(input));
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                return sb.ToString();
            }
        }

        public static string GetPassword(this string password)
        {
            using (MD5 md5 = MD5.Create())
            {
                md5.Initialize();
                md5.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(md5.Hash);
            }
          
        }

        public static bool VerifyPassword(this string username, string dbuserPassword, string userPassword)
        {
            byte[] m_hash = dbuserPassword.Base64Decode();
            string u_hash = username + userPassword;
            using (MD5 md5 = MD5.Create())
            {
                md5.Initialize();
                md5.ComputeHash(Encoding.UTF8.GetBytes(u_hash));
                return m_hash.SequenceEqual(md5.Hash);
            }
        }

        public static string Base64Encode(this string plainText)
        {
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }

        public static byte[] Base64Decode(this string base64EncodedData)
        {
            var base64EncodedBytes = Convert.FromBase64String(base64EncodedData);
            return base64EncodedBytes;
        }
    }
}
