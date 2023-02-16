// <copyright file="WorkspacePagingRequestDto.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using VADAR.DTO.AbtractClasses;

namespace VADAR.DTO
{
    /// <summary>
    /// Workspace Request Dto.
    /// </summary>
    public class WorkspacePagingRequestDto : PagingRequestDto
    {
        /// <summary>
        /// Gets or sets.
        /// </summary>
        public string WorkspaceName { get; set; }
    }
}
