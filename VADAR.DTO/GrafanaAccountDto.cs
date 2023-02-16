// <copyright file="GrafanaAccountDto.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>
using Newtonsoft.Json;

namespace VADAR.DTO
{
    /// <summary>
    /// Grafana Account Dto.
    /// </summary>
    public class GrafanaAccountDto
    {
        /// <summary>
        /// Gets or sets id.
        /// </summary>
        [JsonProperty("id")]
        public long Id { get; set; }

        /// <summary>
        /// Gets or sets email.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets login.
        /// </summary>
        public string Login { get; set; }

        /// <summary>
        /// Gets or sets password.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets organization Id.
        /// </summary>
        public long OrgId { get; set; }
    }
}
