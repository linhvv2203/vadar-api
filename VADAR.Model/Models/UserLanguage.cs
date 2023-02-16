// <copyright file="UserLanguage.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace VADAR.Model.Models
{
    /// <summary>
    /// UserLanguage Class.
    /// </summary>
    public class UserLanguage : BaseEntity
    {
        /// <summary>
        /// Gets or sets.
        /// </summary>
        [ForeignKey("UserId")]
        [JsonIgnore]
        public virtual User User { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        [Key]
        [Column(Order = 0)]
        public string UserId { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        [ForeignKey("LanguageId")]
        [JsonIgnore]
        public virtual Language Language { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        [Key]
        [Column(Order = 1)]
        public int LanguageId { get; set; }
    }
}
