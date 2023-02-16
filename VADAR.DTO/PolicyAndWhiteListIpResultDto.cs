// <copyright file="PolicyAndWhiteListIpResultDto.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

namespace VADAR.DTO
{
    /// <summary>
    /// Policy And White List Ip Result Dto.
    /// </summary>
    public class PolicyAndWhiteListIpResultDto
    {
        /// <summary>
        /// Gets or sets policies.
        /// </summary>
        public PolicyDto[] Policies { get; set; }

        /// <summary>
        /// Gets or sets ips.
        /// </summary>
        public IpDto[] Ips { get; set; }
    }
}
