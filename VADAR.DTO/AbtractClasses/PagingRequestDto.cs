// <copyright file="PagingRequestDto.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

namespace VADAR.DTO.AbtractClasses
{
    /// <summary>
    /// Paging Base Dto.
    /// </summary>
    public abstract class PagingRequestDto
    {
        private int pageIndex = 1;
        private int pageSize = 10;

        /// <summary>
        /// Gets or sets Page Index.
        /// </summary>
        public int PageIndex
        {
            get => this.pageIndex;

            set => this.pageIndex = value <= 0 ? 1 : value;
        }

        /// <summary>
        /// Gets or sets Page Size.
        /// </summary>
        public int PageSize
        {
            get => this.pageSize;

            set => this.pageSize = value > 50 ? 50 : value;
        }
    }
}
