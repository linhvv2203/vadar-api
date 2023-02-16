// <copyright file="InviteUserToWorkspaceViewModel.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

namespace VADAR.DTO.EmailTemplateViewModel
{
    /// <summary>
    /// InviteUserToWorkspaceViewModel.
    /// </summary>
    public class InviteUserToWorkspaceViewModel
    {
        /// <summary>
        /// Gets or sets.
        /// </summary>
        public string WorkspaceRoleName { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public string InviteCode { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public string WorkspaceName { get; set; }
    }
}
