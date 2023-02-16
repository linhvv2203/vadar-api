// <copyright file="RolePagingRequestDto.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using VADAR.DTO.AbtractClasses;

namespace VADAR.DTO
{
    /// <summary>
    /// RolePagingRequestDto.
    /// </summary>
    public class RolePagingRequestDto : PagingRequestDto
    {
        /// <summary>
        /// Gets or sets WorkspaceId.
        /// </summary>
        public int WorkspaceId { get; set; }

        /// <summary>
        /// Gets or sets WorkspaceRoleName.
        /// </summary>
        public string WorkspaceRoleName { get; set; }
    }
}
