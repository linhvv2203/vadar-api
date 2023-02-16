// <copyright file="LicenseDto.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System;

namespace VADAR.DTO
{
    /// <summary>
    /// License Dto.
    /// </summary>
    public class LicenseDto
    {
        /// <summary>
        /// Gets or sets.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public int HostLimit { get; set; }

        /// <summary>
        /// Gets or sets StartDate.
        /// </summary>
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// Gets or sets EndDate.
        /// </summary>
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// Gets or sets Status.
        /// </summary>
        public int Status { get; set; }
    }
}
