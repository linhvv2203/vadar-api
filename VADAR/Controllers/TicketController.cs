// <copyright file="TicketController.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using VADAR.DTO;
using VADAR.Helpers.Enums;
using VADAR.Helpers.Helper;
using VADAR.Service.Interfaces;
using VADAR.WebAPI.Controllers.BaseControllers;
using VADAR.WebAPI.Model;

namespace VADAR.WebAPI.Controllers
{
    /// <summary>
    /// User Controller.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class TicketController : BaseController
    {
        private readonly ILoggerHelper<TicketController> logger;
        private readonly ITicketService ticketService;

        /// <summary>
        /// Initialises a new instance of the <see cref="TicketController"/> class.
        /// </summary>
        /// <param name="logger">The _logger.</param>
        /// <param name="ticketService">User service.</param>
        public TicketController(
            ILoggerHelper<TicketController> logger,
            ITicketService ticketService)
        {
            this.logger = logger;
            this.ticketService = ticketService;
        }

        /// <summary>
        /// Get User base information APi.
        /// </summary>
        /// <param name="ticket">workspaceId.</param>
        /// <returns>User Base Information.</returns>
        /// <remarks>
        /// Sample request:
        ///     POST api/Employee
        ///     {
        ///       "firstName": "Mike",
        ///       "lastName": "Andrew",
        ///       "emailId": "Mike.Andrew@gmail.com"
        ///     },
        /// </remarks>
        [HttpPost]
        public async Task<ApiResponse<TicketDto>> Index([FromBody] TicketDto ticket)
        {
            try
            {
                return new ApiResponse<TicketDto>(EnApiStatusCode.Success, await this.ticketService.Index(ticket));
            }
            catch (Exception ex)
            {
                this.logger.LogError(new EventId(0), ex, ex.GetBaseException().Message + '\n' + ex.GetBaseException().StackTrace);
                return new ApiResponse<TicketDto>(ex.HResult);
            }
        }
    }
}
