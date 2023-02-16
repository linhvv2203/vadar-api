// <copyright file="IEntity.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

namespace VADAR.Model.Common.Interfaces
{
    /// <summary>
    /// Entity Interface.
    /// </summary>
    /// <typeparam name="T">Type.</typeparam>
    public interface IEntity<T>
    {
        /// <summary>
        /// Gets or sets <see cref="Id"/>.
        /// </summary>
        T Id { get; set; }
    }
}
