// <copyright file="WorkspaceViewModelDto.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System;

namespace VADAR.DTO
{
    /// <summary>
    /// Workspace View Model Dto.
    /// </summary>
    public class WorkspaceViewModelDto
    {
        /// <summary>
        /// Gets or sets user Id.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets EndDate.
        /// </summary>
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// Gets or sets Status.
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public int HostLimit { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public int HostActual { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public string CreatedByFullName { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public string UpdateByFullName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether gets or sets IsCreatedOrganisation.
        /// </summary>
        public bool IsCreatedOrganisation { get; set; }
    }
}
