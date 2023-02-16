// <copyright file="Language.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VADAR.Model.Models
{
    /// <summary>
    /// Country Class.
    /// </summary>
    public class Language : BaseEntity
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
        public string Flag { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        [NotMapped]
        public virtual ICollection<UserLanguage> UserLanguages { get; set; }
    }
}
