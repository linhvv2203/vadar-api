// <copyright file="TicketDto.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System.ComponentModel.DataAnnotations;

namespace VADAR.DTO
{
    /// <summary>
    /// User Dto.
    /// </summary>
    public class TicketDto
    {
        /// <summary>
        /// Gets or sets user Id.
        /// </summary>
        [Required]
        public string Method { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        [Required]
        public string Index { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        [Required]
        public dynamic Query { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public dynamic Result { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether gets or sets UseCookie.
        /// </summary>
        public bool UseCookie { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether gets or sets IsSuccessStatusCode.
        /// </summary>
        public bool IsSuccessStatusCode { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public int WorkspaceId { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public string Organisation { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public string Owner { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public string Services { get; set; }
    }
}
