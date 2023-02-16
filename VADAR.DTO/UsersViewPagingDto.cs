// <copyright file="UsersViewPagingDto.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System.Collections.Generic;

namespace VADAR.DTO
{
    /// <summary>
    /// Users View Paging Dto.
    /// </summary>
    public class UsersViewPagingDto
    {
        /// <summary>
        /// Gets or sets.
        /// </summary>
        public int TotalUser { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public IEnumerable<UserDto> Users { get; set; }
    }
}
