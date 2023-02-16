// <copyright file="Entity.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using VADAR.Model.Common.Interfaces;

namespace VADAR.Model.Common
{
    /// <summary>
    /// Entity Base Class.
    /// </summary>
    /// <typeparam name="T">T object.</typeparam>
    public abstract class Entity<T> : IEntity<T>
    {
        /// <inheritdoc/>
        public T Id { get; set; }
    }
}
