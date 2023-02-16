// <copyright file="CreateWhiteIpDto.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System;

namespace VADAR.DTO
{
    /// <summary>
    /// WhiteIp Dto.
    /// </summary>
    public class CreateWhiteIpDto
    {
        /// <summary>
        /// Gets or sets Ip.
        /// </summary>
        public string[] Ip { get; set; }

        /// <summary>
        /// Gets or sets Description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets Workspace Id.
        /// </summary>
        public int WorkspaceId { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public DateTime? CreatedDate { get; set; }
    }
}
