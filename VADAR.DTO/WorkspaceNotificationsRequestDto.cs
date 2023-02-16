// <copyright file="WorkspaceNotificationsRequestDto.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

namespace VADAR.DTO
{
    /// <summary>
    /// Workspace Notifications Request.
    /// </summary>
    public class WorkspaceNotificationsRequestDto
    {
        /// <summary>
        /// Gets or sets Address.
        /// </summary>
        public string[] Address { get; set; }

        /// <summary>
        /// Gets or sets Notification Type.
        /// </summary>
        public int Type { get; set; }
    }
}
