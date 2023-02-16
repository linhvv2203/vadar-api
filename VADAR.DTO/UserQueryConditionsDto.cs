// <copyright file="UserQueryConditionsDto.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using VADAR.DTO.AbtractClasses;

namespace VADAR.DTO
{
    /// <summary>
    /// User Query Conditions Dto.
    /// </summary>
    public class UserQueryConditionsDto : PagingRequestDto
    {
        /// <summary>
        /// Gets or sets UserName.
        /// </summary>
        public string UserName { get; set; }
    }
}
