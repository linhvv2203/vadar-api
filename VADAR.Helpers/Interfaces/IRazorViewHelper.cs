// <copyright file="IRazorViewHelper.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System.Threading.Tasks;

namespace VADAR.Helpers.Interfaces
{
    /// <summary>
    /// IRazorViewHelper.
    /// </summary>
    public interface IRazorViewHelper
    {
        /// <summary>
        /// Render cshtml to string.
        /// </summary>
        /// <param name="viewPath">the path view.</param>
        /// <param name="model">Model for binding to view.</param>
        /// <returns>render string.</returns>
        Task<string> RenderViewAsString(string viewPath, object model);
    }
}
