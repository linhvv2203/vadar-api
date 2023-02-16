// <copyright file="VADARException.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System;

namespace VADAR.Exceptions
{
    /// <summary>
    /// Customized exception.
    /// </summary>
    public class VadarException : Exception
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="VadarException"/> class.
        /// </summary>
        /// <param name="code">code.</param>
        public VadarException(ErrorCode code)
            : base(code.ToString())
        {
            this.HResult = (int)code;
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="VadarException"/> class.
        /// </summary>
        /// <param name="code">code.</param>
        /// <param name="message">message.</param>
        public VadarException(ErrorCode code, string message)
            : base(message)
        {
            this.HResult = (int)code;
        }
    }
}
