// <copyright file="PagedResultDto.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System.Collections.Generic;

namespace VADAR.DTO.AbtractClasses
{
    /// <summary>
    /// Paging Result.
    /// </summary>
    /// <typeparam name="T">T: Class.</typeparam>
    public abstract class PagedResultDto<T>
        where T : class
    {
        /// <summary>
        /// Gets or sets Count.
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// Gets or sets Items.
        /// </summary>
        public IEnumerable<T> Items { get; set; }
    }
}
