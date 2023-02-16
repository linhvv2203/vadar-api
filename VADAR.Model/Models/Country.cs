// <copyright file="Country.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System.ComponentModel.DataAnnotations;

namespace VADAR.Model.Models
{
    /// <summary>
    /// Country Class.
    /// </summary>
    public class Country : BaseEntity
    {
        /// <summary>
        /// Gets or sets.
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public string DisplayName { get; set; }
    }
}
