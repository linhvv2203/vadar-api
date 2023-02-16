using AutoFixture;
using Microsoft.AspNetCore.Http;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using VADAR.DTO;
using VADAR.Helpers.Helper;
using VADAR.Helpers.Interfaces;
using VADAR.Service.Interfaces;
using VADAR.WebAPI.Controllers.BaseControllers;
using Xunit;

namespace VADAR.Tests.WebAPI.Unit
{
    public class DashboardControllerTest
    {
        private Fixture fixture;
        private Mock<ILoggerHelper<DashboardController>> mockLogger;
        private Mock<IDashboardService> mockDashboard;
        private Mock<IElasticSearchCallApiHelper> mocElastic;
        private DashboardController dashboardController;
        public DashboardControllerTest()
        {
            fixture = new Fixture();
            fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
                .ForEach(behaviour => fixture.Behaviors.Remove(behaviour));
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        }

        [Fact]
        public async void TestDashboardSummary()
        {
            // Arange
            Setup();
            var dataRequest = new HostStatisticRequestDto();
            dataRequest.FromDate = DateTime.Now.AddDays(-15);
            dataRequest.ToDate = DateTime.Now;
            dataRequest.Group = "VSEC";
            dataRequest.Level = 7;
            var expect = new SummaryDto
            {
                Active = 192,
                Disconnect = 0,
                UnHealthy = 192,
                Healthy = 0
            };

            mockDashboard.Setup(p => p.GetDashboardSummarys(It.IsAny<HostStatisticRequestDto>())).ReturnsAsync(expect);

            // Act
            var data = await dashboardController.GetDashboardSummary();

            // Assert
            mockDashboard.Verify(p => p.GetDashboardSummarys(It.IsAny<HostStatisticRequestDto>()), Times.Once);
            Assert.Equal(1, data.Status);
            Assert.Equal(data.Data.Active, expect.Active);
            Assert.Equal(data.Data.Disconnect, expect.Disconnect);
            Assert.Equal(data.Data.Healthy, expect.Healthy);
            Assert.Equal(data.Data.UnHealthy, expect.UnHealthy);
        }

        [Fact]
        public async void TestHostStatistics()
        {
            // Arange
            Setup();
            var dataRequest = new HostStatisticRequestDto();
            dataRequest.FromDate = DateTime.Now.AddDays(-15);
            dataRequest.ToDate = DateTime.Now;
            dataRequest.Group = "VSEC";
            dataRequest.Level = 7;
            var expect = new List<HostStatisticDto>
            {
                new HostStatisticDto
                {
                    Host = "Test",
                    Issues = "test"
                }
            };

            mockDashboard.Setup(p => p.GetHostStatistics(It.IsAny<HostStatisticRequestDto>())).ReturnsAsync(expect);

            // Act
            var data = await dashboardController.GetHostStatistics();

            // Assert
            mockDashboard.Verify(p => p.GetHostStatistics(It.IsAny<HostStatisticRequestDto>()), Times.Once);
            Assert.Equal(1, data.Status);
            Assert.Equal(expect[0], data.Data[0]);
        }


        [Fact]
        public async void TestGetSecurityEventByTime()
        {
            // Arange
            Setup();
            var dataRequest = new HostStatisticRequestDto();
            dataRequest.FromDate = DateTime.Now.AddDays(-15);
            dataRequest.ToDate = DateTime.Now;
            dataRequest.Group = "VSEC";
            dataRequest.Level = 7;
            var expect = new EventSecurityReturnDto
            {
                Total = 1,
                X = new List<EventSecurityDto> { new EventSecurityDto { Data = new List<int> { 1, 2, 3 }, Label = "label1" } },
                Y = new List<string> { "label1" }
            };

            mockDashboard.Setup(p => p.GetSecurityEventByTime(It.IsAny<HostStatisticRequestDto>())).ReturnsAsync(expect);

            // Act
            var data = await dashboardController.GetSecurityEventByTime();

            // Assert
            mockDashboard.Verify(p => p.GetSecurityEventByTime(It.IsAny<HostStatisticRequestDto>()), Times.Once);
            Assert.Equal(1, data.Status);
            Assert.Single(data.Data.X);
            Assert.Equal(expect, data.Data);
        }

        [Fact]
        public async void TestGetPerformanceEvent()
        {
            // Arange
            Setup();
            var dataRequest = new HostStatisticRequestDto();
            dataRequest.FromDate = DateTime.Now.AddDays(-15);
            dataRequest.ToDate = DateTime.Now;
            dataRequest.Group = "VSEC";
            dataRequest.Level = 7;
            var expect = new EventSecurityReturnDto
            {
                Total = 1,
                X = new List<EventSecurityDto> { new EventSecurityDto { Data = new List<int> { 1, 2, 3 }, Label = "label1" } },
                Y = new List<string> { "label1" }
            };

            mockDashboard.Setup(p => p.GetPerformanceEvent(It.IsAny<HostStatisticRequestDto>())).ReturnsAsync(expect);

            // Act
            var data = await dashboardController.GetPerformanceEvent();

            // Assert
            mockDashboard.Verify(p => p.GetPerformanceEvent(It.IsAny<HostStatisticRequestDto>()), Times.Once);
            Assert.Equal(1, data.Status);
            Assert.Single(data.Data.X);
            Assert.Equal(expect, data.Data);
        }

        [Fact]
        public async void TestGetTop10SecurityEvent()
        {
            // Arange
            Setup();
            var dataRequest = new HostStatisticRequestDto();
            dataRequest.FromDate = DateTime.Now.AddDays(-15);
            dataRequest.ToDate = DateTime.Now;
            dataRequest.Group = "VSEC";
            dataRequest.Level = 7;
            var expect = new List<EventDto>
            {
                new EventDto
                {
                    HostName = "Host1",
                    Value = 1
                }
            };

            mockDashboard.Setup(p => p.GetTop10SecurityEvent(It.IsAny<HostStatisticRequestDto>())).ReturnsAsync(expect);

            // Act
            var data = await dashboardController.GetTop10SecurityEvent();

            // Assert
            mockDashboard.Verify(p => p.GetTop10SecurityEvent(It.IsAny<HostStatisticRequestDto>()), Times.Once);
            Assert.Equal(1, data.Status);
            Assert.Equal(expect, data.Data);
        }

        [Fact]
        public async void TestGetTop10AttackIP()
        {
            // Arange
            Setup();
            var dataRequest = new HostStatisticRequestDto();
            dataRequest.FromDate = DateTime.Now.AddDays(-15);
            dataRequest.ToDate = DateTime.Now;
            dataRequest.Group = "VSEC";
            dataRequest.Level = 7;
            var expect = new List<EventDto>
            {
                new EventDto
                {
                    HostName = "Host1",
                    Value = 1
                }
            };

            mockDashboard.Setup(p => p.GetTop10AttackIP(It.IsAny<HostStatisticRequestDto>(), 601)).ReturnsAsync(expect);

            // Act
            var data = await dashboardController.GetTop10AttackIP();

            // Assert
            mockDashboard.Verify(p => p.GetTop10AttackIP(It.IsAny<HostStatisticRequestDto>(), 601), Times.Once);
            Assert.Equal(1, data.Status);
            Assert.Equal(expect, data.Data);
        }

        [Fact]
        public async void TestGetLast10SecurityEvent()
        {
            // Arange
            Setup();
            var dataRequest = new HostStatisticRequestDto();
            dataRequest.FromDate = DateTime.Now.AddDays(-15);
            dataRequest.ToDate = DateTime.Now;
            dataRequest.Group = "VSEC";
            dataRequest.Level = 7;
            var expect = new List<SecurityEventReturnDto>
            {
                new SecurityEventReturnDto
                {
                   Description  = "description",
                   Host  = "Host1",
                   Timestamp = DateTime.Now.ToString()
                }
            };

            mockDashboard.Setup(p => p.GetLast10SecurityEvent(It.IsAny<HostStatisticRequestDto>())).ReturnsAsync(expect);

            // Act
            var data = await dashboardController.GetLast10SecurityEvent();

            // Assert
            mockDashboard.Verify(p => p.GetLast10SecurityEvent(It.IsAny<HostStatisticRequestDto>()), Times.Once);
            Assert.Equal(1, data.Status);
            Assert.Equal(expect, data.Data);
        }

        [Fact]
        public async void TestGetLast10PerformanceEvent()
        {
            // Arange
            Setup();
            var dataRequest = new HostStatisticRequestDto();
            dataRequest.FromDate = DateTime.Now.AddDays(-15);
            dataRequest.ToDate = DateTime.Now;
            dataRequest.Group = "VSEC";
            dataRequest.Level = 7;
            var expect = new List<PerformanceEventReturnDto>
            {
                new PerformanceEventReturnDto
                {
                   EventName = "",
                   HostName = "Host",
                   Time = DateTime.Now.ToString()
                }
            };

            mockDashboard.Setup(p => p.GetLast10PerformanceEvent(It.IsAny<HostStatisticRequestDto>())).ReturnsAsync(expect);

            // Act
            var data = await dashboardController.GetLast10PerformanceEvent();

            // Assert
            mockDashboard.Verify(p => p.GetLast10PerformanceEvent(It.IsAny<HostStatisticRequestDto>()), Times.Once);
            Assert.Equal(1, data.Status);
            Assert.Equal(expect, data.Data);
        }

        /// <summary>
        /// Provides a common set of functions that are performed just before each test method is called.
        /// </summary>
        internal void Setup()
        {
            mockDashboard = new Mock<IDashboardService>(MockBehavior.Strict);
            mocElastic = new Mock<IElasticSearchCallApiHelper>(MockBehavior.Strict);
            mockLogger = new Mock<ILoggerHelper<DashboardController>>(MockBehavior.Strict);
            dashboardController = new DashboardController(mockLogger.Object, mockDashboard.Object);
            dashboardController.ControllerContext.HttpContext = new DefaultHttpContext();
            var userInfo = "{\"name\":\"Son Minh\",\"given_name\":\"Son\",\"family_name\":\"Minh\",\"email\":\"minhson1@vsec.com.vn\",\"sub\":\"08d656bc-3dcb-8976-a55e-dd57588d4ec1\"}";
            dashboardController.ControllerContext.HttpContext.Request.Headers["UserInfo"] = userInfo;
        }
    }
}
