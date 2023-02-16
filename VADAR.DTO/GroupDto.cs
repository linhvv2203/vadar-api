// <copyright file="GroupDto.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System;
using System.ComponentModel.DataAnnotations;

namespace VADAR.DTO
{
    /// <summary>
    /// Group Dto.
    /// </summary>
    public class GroupDto
    {
        /// <summary>
        /// Gets or sets group Id.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        [Required]
        [RegularExpression("^[a-zA-Z0-9_]*$", ErrorMessage = "Your Group Name can only contain letters, numbers and character '_'.")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public string Description { get; set; }

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
        /// Gets or sets WorksapceId.
        /// </summary>
        public int WorkspaceId { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public string ZabbixRef { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public string WazuhRef { get; set; }
    }
}
