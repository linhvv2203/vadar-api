// <copyright file="JWTTokenHelper.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace VADAR.Helpers.Helper
{
    /// <summary>
    /// Helper class for JWT Token.
    /// </summary>
    public class JWTTokenHelper
    {
        /// <summary>
        /// Get Claims from token.
        /// </summary>
        /// <param name="jwtToken">jwt token.</param>
        /// <returns>IEnumerable of Claims.</returns>
        public static IEnumerable<Claim> GetTokenClaims(string jwtToken)
        {
            var handler = new JwtSecurityTokenHandler();
            var tokenS = handler.ReadToken(jwtToken) as JwtSecurityToken;

            return tokenS?.Claims;
        }
    }
}
