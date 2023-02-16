// <copyright file="IGenericUnitOfWork.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using VADAR.Repository.Common;

namespace VADAR.Repository.UnitOfWork.Interfaces
{
    /// <summary>
    /// IGenericUnitOfWork.
    /// </summary>
    /// <typeparam name="TInterface">Interface of repository.</typeparam>
    public interface IGenericUnitOfWork<TInterface> : IUnitOfWork
    {
        /// <summary>
        /// Gets repository.
        /// </summary>
        TInterface Repository { get; }
    }
}
