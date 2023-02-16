// <copyright file="UserClaim.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace VADAR.Model.Models
{
    /// <summary>
    /// User Claim.
    /// </summary>
    public class UserClaim : BaseEntity
    {
        /// <summary>
        /// Gets or sets.
        /// </summary>
        [Key]
        [Column(Order = 0)]
        public string UserId { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        [ForeignKey("UserId")]
        [JsonIgnore]
        public User User { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        [Key]
        [Column(Order = 1)]
        public int ClaimId { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        [ForeignKey("ClaimId")]
        [JsonIgnore]
        public Claim Claim { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsPublic.
        /// </summary>
        public bool IsPublic { get; set; }
    }
}
