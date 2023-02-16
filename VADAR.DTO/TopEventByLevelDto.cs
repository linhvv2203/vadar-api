// <copyright file="TopEventByLevelDto.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System.Collections.Generic;

namespace VADAR.DTO
{
    /// <summary>
    /// Logs Security request.
    /// </summary>
    public class TopEventByLevelDto
    {
        /// <summary>
        /// Gets or sets Level.
        /// </summary>
        public int? Level { get; set; }

        /// <summary>
        /// Gets or sets EventName.
        /// </summary>
        public string EventName { get; set; }

        /// <summary>
        /// Gets or sets hosts.
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public string Hosts { get; set; }
    }
}
