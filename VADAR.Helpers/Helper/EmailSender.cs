// <copyright file="EmailSender.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.UI.Services;
using NETCore.MailKit.Core;

namespace VADAR.Helpers.Helper
{
    /// <summary>
    /// Email Sender.
    /// </summary>
    public class EmailSender : IEmailSender
    {
        private readonly IEmailService emailService;

        /// <summary>
        /// Initialises a new instance of the <see cref="EmailSender"/> class.
        /// Email sender constructor.
        /// </summary>
        /// <param name="emailService">Email Service.</param>
        public EmailSender(IEmailService emailService)
        {
            this.emailService = emailService;
        }

        /// <summary>
        /// Send email async.
        /// </summary>
        /// <param name="email">email address.</param>
        /// <param name="subject">subject.</param>
        /// <param name="message">message.</param>
        /// <returns>Task Done.</returns>
        public Task SendEmailAsync(string email, string subject, string message)
        {
            return this.Execute(subject, message, email);
        }

        /// <summary>
        /// Execute email sending.
        /// </summary>
        /// <param name="subject">subject.</param>
        /// <param name="message">message.</param>
        /// <param name="email">email address.</param>
        /// <returns>task done.</returns>
        public Task Execute(string subject, string message, string email)
        {
            this.emailService.Send(email, subject, message, true);

            return Task.CompletedTask;
        }
    }
}
