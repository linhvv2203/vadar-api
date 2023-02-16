// <copyright file="IReCAPTCHAHelper.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

namespace VADAR.Helpers.Interfaces
{
    /// <summary>
    /// Interface for ReCapchaHelper.
    /// </summary>
    public interface IRecaptchaHelper
    {
        /// <summary>
        /// Verify reCaptcha.
        /// </summary>
        /// <param name="recaptcha">Recaptcha.</param>
        /// <returns>True means recaptcha valid.</returns>
        bool IsValidRecaptcha(string recaptcha);
    }
}
