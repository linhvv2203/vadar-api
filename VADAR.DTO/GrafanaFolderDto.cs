// <copyright file="GrafanaFolderDto.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

namespace VADAR.DTO
{
    /// <summary>
    /// Grafana Folder Dto.
    /// </summary>
    public class GrafanaFolderDto
    {
        /// <summary>
        /// Gets or sets id.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Gets or sets uid.
        /// </summary>
        public string Uid { get; set; }

        /// <summary>
        /// Gets or sets title.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets url.
        /// </summary>
        public string Url { get; set; }
    }
}
