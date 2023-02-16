// <copyright file="RegistrationDto.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

namespace VADAR.DTO
{
    /// <summary>
    /// RegistrationDto.
    /// </summary>
    public class RegistrationDto
    {
        /// <summary>
        /// Gets or sets UserName.
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets CompanyName.
        /// </summary>
        public string CompanyName { get; set; }

        /// <summary>
        /// Gets or sets Email.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets UserId.
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// Gets or sets Language.
        /// </summary>
        public string Language { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether gets or sets ExistUser.
        /// </summary>
        public bool ExistUser { get; set; }

        /// <summary>
        /// Gets or sets AccessKey.
        /// </summary>
        public string AccessKey { get; set; }
    }
}
