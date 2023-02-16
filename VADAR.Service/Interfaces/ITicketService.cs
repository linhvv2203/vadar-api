// <copyright file="ITicketService.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System.Threading.Tasks;
using VADAR.DTO;

namespace VADAR.Service.Interfaces
{
    /// <summary>
    /// User Service Interface.
    /// </summary>
    public interface ITicketService
    {
        /// <summary>
        /// Add new user if it is not exist.
        /// </summary>
        /// <param name="ticket">User information.</param>
        /// <returns>User Created.</returns>
        Task<TicketDto> Index(TicketDto ticket);

        /// <summary>
        /// Add new user if it is not exist.
        /// </summary>
        /// <param name="ticket">User information.</param>
        /// <returns>User Created.</returns>
        Task<TicketDto> Ticket(TicketDto ticket);

        /// <summary>
        /// Add new user if it is not exist.
        /// </summary>
        /// <param name="ticket">User information.</param>
        /// <returns>User Created.</returns>
        Task<TicketDto> GetTicketElasticsearch(TicketDto ticket);
    }
}
