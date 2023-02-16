// <copyright file="HostPagingRequestDto.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System;
using VADAR.DTO.AbtractClasses;

namespace VADAR.DTO
{
    /// <summary>
    /// Host Paging Request Dto.
    /// </summary>
    public class HostPagingRequestDto : PagingRequestDto
    {
        /// <summary>
        /// Gets or sets.
        /// </summary>
        public string HostName { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public string CreatedById { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public Guid GroupId { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public int WorkspaceId { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public int CheckExist { get; set; }
    }
}
