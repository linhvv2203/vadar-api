// <copyright file="StringHelper.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using VADAR.Helpers.Enums;
using VADAR.Helpers.Interfaces;

namespace VADAR.Helpers.Helper
{
    /// <summary>
    /// VerifyBase64StringHelper.
    /// </summary>
    public class StringHelper : IStringHelper
    {
        /// <inheritdoc/>
        public string GetHostName(dynamic hostName)
        {
            if (hostName.GetType().Name == "JArray" && hostName.Count > 0)
            {
                return hostName[hostName.Count - 1].Value.ToString();
            }

            return hostName.ToString();
        }

        /// <inheritdoc/>
        public bool ValidateFileType(string fileName, List<string> fileTypes)
        {
            if (string.IsNullOrEmpty(fileName) || fileName.Split('.').Length <= 1)
            {
                return false;
            }

            var fileType = fileName.Split('.').LastOrDefault();
            return fileTypes.Any(t => fileType != null && t == fileType.ToLower());
        }

        /// <inheritdoc/>
        public bool IsBase64(string base64String)
        {
            if (string.IsNullOrEmpty(base64String))
            {
                return false;
            }

            if (base64String.Contains(","))
            {
                base64String = base64String.Split(",")[1];
            }

            // Credit: oybek https://stackoverflow.com/users/794764/oybek
            if (string.IsNullOrEmpty(base64String) || base64String.Length % 4 != 0 || base64String.Contains(" ") ||
                base64String.Contains("\t") || base64String.Contains("\r") || base64String.Contains("\n"))
            {
                return false;
            }

            try
            {
                Convert.FromBase64String(base64String);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <inheritdoc/>
        public bool IsVerifyFileType(string base64String)
        {
            if (string.IsNullOrEmpty(base64String))
            {
                return false;
            }

            var fileType = new string[] { "image/jpg", "image/png", "image/jpeg", "image/gif", "application/vnd.openxmlformats-officedocument.wordprocessingml.document", "application/x-zip-compressed", "application/octet-stream", "application/msword", "text/plain" };

            if (!fileType.Any(base64String.Contains))
            {
                return false;
            }

            return true;
        }

        /// <inheritdoc/>
        public string UserStatusName(int status, string language)
        {
            switch (status)
            {
                case (int)EnUserStatus.Pending:
                    return language.ToLower() switch
                    {
                        "vi-vn" => "Đang chờ xử lý",
                        _ => "Pending"
                    };

                case (int)EnUserStatus.SubmitForApproving:
                    return language.ToLower() switch
                    {
                        "vi-vn" => "Gửi để xem xét",
                        _ => "Submit for approving"
                    };

                case (int)EnUserStatus.Active:
                    return language.ToLower() switch
                    {
                        "vi-vn" => "Hoạt động",
                        _ => "Active"
                    };

                case (int)EnUserStatus.Rejected:
                    return language.ToLower() switch
                    {
                        "vi-vn" => "Đã từ chối",
                        _ => "Rejected"
                    };

                case (int)EnUserStatus.Cancel:
                    return language.ToLower() switch
                    {
                        "vi-vn" => "Đã bị hủy",
                        _ => "Cancelled"
                    };

                case (int)EnUserStatus.Blocked:
                    return language.ToLower() switch
                    {
                        "vi-vn" => "Đã bị chặn",
                        _ => "Blocked"
                    };

                default: return string.Empty;
            }
        }

        /// <inheritdoc/>
        public string GenerateSlug(string phrase)
        {
            var str = this.RemoveAccent(phrase).ToLower();

            // invalid chars
            str = Regex.Replace(str, @"[^a-z0-9\s-]", string.Empty);

            // convert multiple spaces into one space
            str = Regex.Replace(str, @"\s+", " ").Trim();

            // cut and trim
            str = str.Substring(0, str.Length <= 45 ? str.Length : 45).Trim();
            str = Regex.Replace(str, @"\s", "-"); // hyphens
            return str;
        }

        /// <inheritdoc/>
        public bool IsValidUrl(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                return false;
            }

            try
            {
                return Regex.IsMatch(
                    url,
#pragma warning disable SA1118 // Parameter should not span multiple lines
                    @"^(ht|f)tp(s?)\:\/\/[0-9a-zA-Z]([-.\w]*[0-9a-zA-Z])*(:(0-9)*)*(\/?)([a-zA-Z0-9\-\.\?\,\'\/\\\+&%\$#_]*)?$",
#pragma warning disable SA1117 // Parameters should be on same line or separate lines
                    RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
#pragma warning restore SA1117 // Parameters should be on same line or separate lines
#pragma warning restore SA1117 // Parameters should be on same line or separate lines
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
        }

        /// <inheritdoc/>
        public bool IsValidTelegram(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                return false;
            }

            try
            {
                return Regex.IsMatch(
                    token,
#pragma warning disable SA1118 // Parameter should not span multiple lines
                    @"[0-9]{9}:[a-zA-Z0-9_-]{35}",
#pragma warning disable SA1117 // Parameters should be on same line or separate lines
                    RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
#pragma warning restore SA1117 // Parameters should be on same line or separate lines
#pragma warning restore SA1117 // Parameters should be on same line or separate lines
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
        }

        /// <inheritdoc/>
        public bool IsValidPhoneNumber(string phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
            {
                return false;
            }

            try
            {
                return Regex.IsMatch(
                    phoneNumber,
#pragma warning disable SA1118 // Parameter should not span multiple lines
                    @"^(09|01[2|6|8|9])+([0-9]{8})$",
#pragma warning disable SA1117 // Parameters should be on same line or separate lines
                    RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
#pragma warning restore SA1117 // Parameters should be on same line or separate lines
#pragma warning restore SA1117 // Parameters should be on same line or separate lines
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
        }

        /// <inheritdoc/>
        public bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return false;
            }

            try
            {
                // Normalize the domain
                email = Regex.Replace(email, @"(@)(.+)$", DomainMapper,
#pragma warning disable SA1117 // Parameters should be on same line or separate lines
                                      RegexOptions.None, TimeSpan.FromMilliseconds(200));
#pragma warning restore SA1117 // Parameters should be on same line or separate lines

                // Examines the domain part of the email and normalizes it.
                string DomainMapper(Match match)
                {
                    // Use IdnMapping class to convert Unicode domain names.
                    var idn = new IdnMapping();

                    // Pull out and process domain name (throws ArgumentException on invalid)
                    var domainName = idn.GetAscii(match.Groups[2].Value);

                    return match.Groups[1].Value + domainName;
                }
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
            catch (ArgumentException)
            {
                return false;
            }

            try
            {
                return Regex.IsMatch(
                    email,
#pragma warning disable SA1118 // Parameter should not span multiple lines
                    @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                    @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-0-9a-z]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$",
#pragma warning disable SA1117 // Parameters should be on same line or separate lines
                    RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
#pragma warning restore SA1117 // Parameters should be on same line or separate lines
#pragma warning restore SA1117 // Parameters should be on same line or separate lines
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
        }

        /// <inheritdoc/>
        public string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = Convert.FromBase64String(base64EncodedData);
            return Encoding.UTF8.GetString(base64EncodedBytes);
        }

        /// <inheritdoc/>
        public string EncodeBase64(string value)
        {
            var valueBytes = Encoding.UTF8.GetBytes(value);
            return Convert.ToBase64String(valueBytes);
        }

        /// <inheritdoc/>
        public string RemoveVietnameseTone(string companyName)
        {
            var result = companyName.ToLower();
            result = Regex.Replace(result, "à|á|ạ|ả|ã|â|ầ|ấ|ậ|ẩ|ẫ|ă|ằ|ắ|ặ|ẳ|ẵ|/g", "a");
            result = Regex.Replace(result, "è|é|ẹ|ẻ|ẽ|ê|ề|ế|ệ|ể|ễ|/g", "e");
            result = Regex.Replace(result, "ì|í|ị|ỉ|ĩ|/g", "i");
            result = Regex.Replace(result, "ò|ó|ọ|ỏ|õ|ô|ồ|ố|ộ|ổ|ỗ|ơ|ờ|ớ|ợ|ở|ỡ|/g", "o");
            result = Regex.Replace(result, "ù|ú|ụ|ủ|ũ|ư|ừ|ứ|ự|ử|ữ|/g", "u");
            result = Regex.Replace(result, "ỳ|ý|ỵ|ỷ|ỹ|/g", "y");
            result = Regex.Replace(result, "đ", "d");
            return result;
        }

        /// <inheritdoc/>
        public string GenerateRandomString()
        {
            const int length = 7;

            // creating a StringBuilder object()
            var strBuild = new StringBuilder();
            var random = new Random();

            for (var i = 0; i < length; i++)
            {
                var flt = random.NextDouble();
                var shift = Convert.ToInt32(Math.Floor(25 * flt));
                var letter = Convert.ToChar(shift + 65);
                strBuild.Append(letter);
            }

            return strBuild.ToString().ToUpper();
        }

        private string RemoveAccent(string txt)
        {
            var bytes = Encoding.GetEncoding("Cyrillic").GetBytes(txt);
            return Encoding.ASCII.GetString(bytes);
        }
    }
}
