// <copyright file="GroupPagingRequestDto.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using VADAR.DTO.AbtractClasses;

namespace VADAR.DTO
{
    /// <summary>
    /// Group Request Dto.
    /// </summary>
    public class GroupPagingRequestDto : PagingRequestDto
    {
        /// <summary>
        /// Gets or sets.
        /// </summary>
        public string GroupName { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public string CreatedById { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public int WorkspaceId { get; set; }
    }
}
