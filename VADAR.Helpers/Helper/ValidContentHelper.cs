// <copyright file="ValidContentHelper.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System;
using Microsoft.Extensions.Configuration;
using VADAR.Helpers.Interfaces;

namespace VADAR.Helpers.Helper
{
    /// <summary>
    /// ValidContentHelper.
    /// </summary>
    public class ValidContentHelper : IValidContentHelper
    {
        private readonly int limitSizeContent;

        /// <summary>
        /// Initialises a new instance of the <see cref="ValidContentHelper"/> class.
        /// Initializes a new instance of the <see cref="ValidContentHelper"/> class.
        /// </summary>
        /// <param name="configuration">configuration.</param>
        public ValidContentHelper(IConfiguration configuration)
        {
            this.limitSizeContent = Convert.ToInt32(configuration["LimitSizeContent"]);
        }

        /// <inheritdoc/>
        public bool IsValidContent(string content)
        {
            if (string.IsNullOrEmpty(content))
            {
                return true;
            }

            return content.Length < this.limitSizeContent;
        }
    }
}
