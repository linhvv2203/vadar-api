// <copyright file="HostViewModelDto.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;

namespace VADAR.DTO
{
    /// <summary>
    /// Host View Model Dto.
    /// </summary>
    public class HostViewModelDto
    {
        /// <summary>
        /// Gets or sets Id.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets Name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets NameEngine.
        /// </summary>
        public string NameEngine { get; set; }

        /// <summary>
        /// Gets or sets Status.
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// Gets or sets Type of devices.
        /// </summary>
        public int Type { get; set; }

        /// <summary>
        /// Gets or sets Os.
        /// </summary>
        public string Os { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public string Ip { get; set; }

        /// <summary>
        /// Gets or sets Description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets GroupName.
        /// </summary>
        public string GroupName { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public DateTime? UpdatedDate { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public string UpdatedById { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public DateTime? CreatedDate { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public string CreatedById { get; set; }

        /// <summary>
        /// Gets or sets GroupId.
        /// </summary>
        public string GroupId { get; set; }

        /// <summary>
        /// Gets or sets Wazuhref.
        /// </summary>
        public string WazuhRef { get; set; }

        /// <summary>
        /// Gets or sets ZabbixRef.
        /// </summary>
        public string ZabbixRef { get; set; }

        /// <summary>
        /// Gets or sets MachineId.
        /// </summary>
        public string MachineId { get; set; }

        /// <summary>
        /// Gets or sets LinkDownloadPerformance.
        /// </summary>
        public List<LinkDownloadViewDto> LinkDownloadPerformance { get; set; }

        /// <summary>
        /// Gets or sets LinkDownloadSecurity.
        /// </summary>
        public List<LinkDownloadViewDto> LinkDownloadSecurity { get; set; }
    }
}
