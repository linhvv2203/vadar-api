// <copyright file="Guard.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System;

namespace VADAR.Helpers
{
    /// <summary>
    /// Guard class for argument validation.
    /// </summary>
    public static class Guard
    {
        /// <summary>
        /// Is Not Null.
        /// </summary>
        /// <param name="value">Value.</param>
        /// <param name="name">Name.</param>
        public static void IsNotNull(object value, string name)
        {
            if (value == null)
            {
                throw new ArgumentNullException(name);
            }
        }
    }
}
