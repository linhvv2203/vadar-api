// <copyright file="PoliciesPagingRequestDto.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using VADAR.DTO.AbtractClasses;

namespace VADAR.DTO
{
    /// <summary>
    /// PoliciesPagingRequestDto.
    /// </summary>
    public class PoliciesPagingRequestDto : PagingRequestDto
    {
        /// <summary>
        /// Gets or sets.
        /// </summary>
        public int WorkspaceId { get; set; }
    }
}
