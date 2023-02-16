// <copyright file="ExtensionMethods.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using Newtonsoft.Json;

namespace VADAR.Helpers.Extensions
{
    /// <summary>
    /// Extension Methods.
    /// </summary>
    public static class ExtensionMethods
    {
        /// <summary>
        /// Deep Clone.
        /// </summary>
        /// <typeparam name="T">Generic Type.</typeparam>
        /// <param name="a">Object for clone.</param>
        /// <returns>new instance cloned.</returns>
        public static T DataClone<T>(this T a)
        {
            return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(a));
        }
    }
}
