// <copyright file="TokenReceiveHelper.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using Microsoft.AspNetCore.Http;

namespace VADAR.Helpers.Helper
{
    /// <summary>
    /// Token Receive Helper.
    /// </summary>
    public class TokenReceiveHelper
    {
        /// <summary>
        /// From Header And Query String.
        /// </summary>
        /// <param name="request">Http Request.</param>
        /// <returns>token.</returns>
        public static string FromHeaderAndQueryString(HttpRequest request)
        {
            if ((request.Path.Value.StartsWith("/message") || request.Path.Value.StartsWith("/chat") || request.Path.Value.StartsWith("/rtc"))
                && request.Query.TryGetValue("token", out var tokenOutput))
            {
                return tokenOutput;
            }

            if (request.Headers["Authorization"].Count > 0 && request.Headers.TryGetValue("Authorization", out var authorizationToken))
            {
                string token = authorizationToken;
                return token.Length < 9 ? string.Empty : token.Remove(0, 7).Trim();
            }

            return string.Empty;
        }
    }
}
