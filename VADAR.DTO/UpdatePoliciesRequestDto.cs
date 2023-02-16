// <copyright file="UpdatePoliciesRequestDto.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

namespace VADAR.DTO
{
    /// <summary>
    /// UpdatePoliciesRequest Dto.
    /// </summary>
    public class UpdatePoliciesRequestDto
    {
        /// <summary>
        /// Gets or sets.
        /// </summary>
        public int[] PolicyIds { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public int WorkspaceId { get; set; }
    }
}
