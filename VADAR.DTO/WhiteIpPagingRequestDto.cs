// <copyright file="WhiteIpPagingRequestDto.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using VADAR.DTO.AbtractClasses;

namespace VADAR.DTO
{
    /// <summary>
    /// WhiteIpPagingRequestDto.
    /// </summary>
    public class WhiteIpPagingRequestDto : PagingRequestDto
    {
        /// <summary>
        /// Gets or sets.
        /// </summary>
        public int WorkspaceId { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public string Ip { get; set; }
    }
}
