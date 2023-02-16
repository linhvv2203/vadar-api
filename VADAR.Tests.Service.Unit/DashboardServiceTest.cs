using AutoFixture;
using Moq;
using System;
using System.Linq;
using System.Threading.Tasks;
using VADAR.DTO;
using VADAR.Helpers.Interfaces;
using VADAR.Repository.UnitOfWork.Interfaces;
using VADAR.Service.Interfaces;
using VADAR.Service.Services;
using Xunit;

namespace VADAR.Tests.Service.Unit
{
    public class DashboardServiceTest
    {
        private Fixture fixture;
        private Mock<IElasticSearchCallApiHelper> mocElastic;
        private Mock<ICallApiZabbixHelper> mockZabbix;
        private IDashboardService dashboardService;
        private Mock<IDashboardUnitOfWork> mockDashboardUnitOfWork;
        public DashboardServiceTest()
        {
            fixture = new Fixture();
            fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
                .ForEach(behaviour => fixture.Behaviors.Remove(behaviour));
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        }

        [Fact]
        public void TestDashboardSummary()
        {
            //Arrange
            HostStatisticRequestDto dataRequest = new HostStatisticRequestDto();
            dataRequest.FromDate = DateTime.Now.AddDays(-15);
            dataRequest.ToDate = DateTime.Now;
            dataRequest.Group = "VSEC";
            dataRequest.Level = 7;
            SummaryDto expect = new SummaryDto()
            {
                Active = 192,
                Disconnect = 0,
                UnHealthy = 192,
                Healthy = 0
            };
            var ex = Task.FromResult(expect);
            var reut = fixture.Create<Task<SummaryDto>>();
            var reut1 = fixture.Create<Task<string>>();

            Setup();
            string responseString = @"{
  ""took"": 414,
  ""timed_out"": false,
  ""_shards"": {
    ""total"": 52,
    ""successful"": 52,
    ""skipped"": 0,
    ""failed"": 0
  },
  ""hits"": {
    ""total"": {
      ""value"": 192,
      ""relation"": ""eq""
    },
    ""max_score"": null,
    ""hits"": [
      {
        ""_index"": ""wazuh-monitoring-3.x-2020.05.24"",
        ""_type"": ""_doc"",
        ""_id"": ""uf6RRHIBENNuuStZwL7t"",
        ""_version"": 1,
        ""_score"": null,
        ""_source"": {
          ""os"": {
            ""arch"": ""x86_64"",
            ""major"": ""6"",
            ""minor"": ""10"",
            ""name"": ""CentOS Linux"",
            ""platform"": ""centos"",
            ""uname"": ""Linux |werserver |2.6.32-754.11.1.el6.x86_64 |#1 SMP Tue Feb 26 15:38:56 UTC 2019 |x86_64"",
            ""version"": ""6.10""
          },
          ""status"": ""Active"",
          ""registerIP"": ""any"",
          ""mergedSum"": ""f69c8a56d2cd8b401643a52cf423ad1c"",
          ""dateAdd"": ""2020-03-27 07:47:46"",
          ""lastKeepAlive"": ""2020-05-24 02:44:47"",
          ""id"": ""060"",
          ""configSum"": ""4b54b212c2dd96f8b25aebfea2a104cd"",
          ""ip"": ""192.168.2.14"",
          ""group"": [
            ""Daklak"",
            ""Test"",
            ""VSEC"",
            ""Daklak_Webs"",
            ""Daklak_Apps""
          ],
          ""node_name"": ""node02"",
          ""manager"": ""vsec-logstash"",
          ""version"": ""Wazuh v3.11.4"",
          ""name"": ""werserver"",
          ""timestamp"": ""2020-05-24T02:45:02.308Z"",
          ""host"": ""vsec-logstash"",
          ""cluster"": {
            ""name"": ""wazuh""
          }
        },
        ""fields"": {
          ""timestamp"": [
            ""2020-05-24T02:45:02.308Z""
          ]
        },
        ""highlight"": {
          ""group"": [
            ""@kibana-highlighted-field@Daklak@/kibana-highlighted-field@"",
            ""@kibana-highlighted-field@Daklak_Webs@/kibana-highlighted-field@""
          ]
        },
        ""sort"": [
          -9223372036854776000
        ]
      },
      {
        ""_index"": ""wazuh-monitoring-3.x-2020.05.24"",
        ""_type"": ""_doc"",
        ""_id"": ""CwLWRHIBENNuuStZaUE6"",
        ""_version"": 1,
        ""_score"": null,
        ""_source"": {
          ""os"": {
            ""arch"": ""x86_64"",
            ""major"": ""6"",
            ""minor"": ""10"",
            ""name"": ""CentOS Linux"",
            ""platform"": ""centos"",
            ""uname"": ""Linux |werserver |2.6.32-754.11.1.el6.x86_64 |#1 SMP Tue Feb 26 15:38:56 UTC 2019 |x86_64"",
            ""version"": ""6.10""
          },
          ""version"": ""Wazuh v3.11.4"",
          ""status"": ""Active"",
          ""manager"": ""vsec-logstash"",
          ""configSum"": ""4b54b212c2dd96f8b25aebfea2a104cd"",
          ""node_name"": ""node02"",
          ""name"": ""werserver"",
          ""group"": [
            ""Daklak"",
            ""Test"",
            ""VSEC"",
            ""Daklak_Webs"",
            ""Daklak_Apps""
          ],
          ""mergedSum"": ""f69c8a56d2cd8b401643a52cf423ad1c"",
          ""dateAdd"": ""2020-03-27 07:47:46"",
          ""ip"": ""192.168.2.14"",
          ""registerIP"": ""any"",
          ""id"": ""060"",
          ""lastKeepAlive"": ""2020-05-24 03:59:47"",
          ""timestamp"": ""2020-05-24T04:00:01.841Z"",
          ""host"": ""vsec-logstash"",
          ""cluster"": {
            ""name"": ""wazuh""
          }
        },
        ""fields"": {
          ""timestamp"": [
            ""2020-05-24T04:00:01.841Z""
          ]
        },
        ""highlight"": {
          ""group"": [
            ""@kibana-highlighted-field@Daklak@/kibana-highlighted-field@"",
            ""@kibana-highlighted-field@Daklak_Webs@/kibana-highlighted-field@""
          ]
        },
        ""sort"": [
          -9223372036854776000
        ]
      },
      {
        ""_index"": ""wazuh-monitoring-3.x-2020.05.24"",
        ""_type"": ""_doc"",
        ""_id"": ""kwPxRHIBENNuuStZ363W"",
        ""_version"": 1,
        ""_score"": null,
        ""_source"": {
          ""os"": {
            ""arch"": ""x86_64"",
            ""major"": ""6"",
            ""minor"": ""10"",
            ""name"": ""CentOS Linux"",
            ""platform"": ""centos"",
            ""uname"": ""Linux |werserver |2.6.32-754.11.1.el6.x86_64 |#1 SMP Tue Feb 26 15:38:56 UTC 2019 |x86_64"",
            ""version"": ""6.10""
          },
          ""manager"": ""vsec-logstash"",
          ""version"": ""Wazuh v3.11.4"",
          ""lastKeepAlive"": ""2020-05-24 04:29:57"",
          ""name"": ""werserver"",
          ""mergedSum"": ""f69c8a56d2cd8b401643a52cf423ad1c"",
          ""configSum"": ""4b54b212c2dd96f8b25aebfea2a104cd"",
          ""dateAdd"": ""2020-03-27 07:47:46"",
          ""ip"": ""192.168.2.14"",
          ""id"": ""060"",
          ""status"": ""Active"",
          ""group"": [
            ""Daklak"",
            ""Test"",
            ""VSEC"",
            ""Daklak_Webs"",
            ""Daklak_Apps""
          ],
          ""registerIP"": ""any"",
          ""node_name"": ""node02"",
          ""timestamp"": ""2020-05-24T04:30:01.684Z"",
          ""host"": ""vsec-logstash"",
          ""cluster"": {
            ""name"": ""wazuh""
          }
        },
        ""fields"": {
          ""timestamp"": [
            ""2020-05-24T04:30:01.684Z""
          ]
        },
        ""highlight"": {
          ""group"": [
            ""@kibana-highlighted-field@Daklak@/kibana-highlighted-field@"",
            ""@kibana-highlighted-field@Daklak_Webs@/kibana-highlighted-field@""
          ]
        },
        ""sort"": [
          -9223372036854776000
        ]
      },
      {
        ""_index"": ""wazuh-monitoring-3.x-2020.05.24"",
        ""_type"": ""_doc"",
        ""_id"": ""lAPxRHIBENNuuStZ363W"",
        ""_version"": 1,
        ""_score"": null,
        ""_source"": {
          ""os"": {
            ""arch"": ""x86_64"",
            ""major"": ""6"",
            ""minor"": ""8"",
            ""name"": ""CentOS Linux"",
            ""platform"": ""centos"",
            ""uname"": ""Linux |webserver18 |2.6.32-642.el6.x86_64 |#1 SMP Tue May 10 17:27:01 UTC 2016 |x86_64"",
            ""version"": ""6.8""
          },
          ""manager"": ""vsec-logstash"",
          ""version"": ""Wazuh v3.11.4"",
          ""lastKeepAlive"": ""2020-05-24 04:29:54"",
          ""name"": ""webserver18"",
          ""mergedSum"": ""535237cb82b543ec180a59bde6425db5"",
          ""configSum"": ""d149ab60df71e6fb53b2d38db75bc733"",
          ""dateAdd"": ""2020-03-31 02:15:08"",
          ""ip"": ""192.168.2.18"",
          ""id"": ""061"",
          ""status"": ""Active"",
          ""group"": [
            ""Daklak"",
            ""Test"",
            ""VSEC"",
            ""Daklak_Webs""
          ],
          ""registerIP"": ""any"",
          ""node_name"": ""node02"",
          ""timestamp"": ""2020-05-24T04:30:01.684Z"",
          ""host"": ""vsec-logstash"",
          ""cluster"": {
            ""name"": ""wazuh""
          }
        },
        ""fields"": {
          ""timestamp"": [
            ""2020-05-24T04:30:01.684Z""
          ]
        },
        ""highlight"": {
          ""group"": [
            ""@kibana-highlighted-field@Daklak@/kibana-highlighted-field@"",
            ""@kibana-highlighted-field@Daklak_Webs@/kibana-highlighted-field@""
          ]
        },
        ""sort"": [
          -9223372036854776000
        ]
      },
      {
        ""_index"": ""wazuh-monitoring-3.x-2020.05.24"",
        ""_type"": ""_doc"",
        ""_id"": ""SAC6RHIBENNuuStZ8t4T"",
        ""_version"": 1,
        ""_score"": null,
        ""_source"": {
          ""os"": {
            ""arch"": ""x86_64"",
            ""major"": ""6"",
            ""minor"": ""8"",
            ""name"": ""CentOS Linux"",
            ""platform"": ""centos"",
            ""uname"": ""Linux |webserver18 |2.6.32-642.el6.x86_64 |#1 SMP Tue May 10 17:27:01 UTC 2016 |x86_64"",
            ""version"": ""6.8""
          },
          ""name"": ""webserver18"",
          ""configSum"": ""d149ab60df71e6fb53b2d38db75bc733"",
          ""manager"": ""vsec-logstash"",
          ""dateAdd"": ""2020-03-31 02:15:08"",
          ""node_name"": ""node02"",
          ""mergedSum"": ""535237cb82b543ec180a59bde6425db5"",
          ""status"": ""Active"",
          ""group"": [
            ""Daklak"",
            ""Test"",
            ""VSEC"",
            ""Daklak_Webs""
          ],
          ""id"": ""061"",
          ""lastKeepAlive"": ""2020-05-24 03:29:50"",
          ""registerIP"": ""any"",
          ""version"": ""Wazuh v3.11.4"",
          ""ip"": ""192.168.2.18"",
          ""timestamp"": ""2020-05-24T03:30:01.868Z"",
          ""host"": ""vsec-logstash"",
          ""cluster"": {
            ""name"": ""wazuh""
          }
        },
        ""fields"": {
          ""timestamp"": [
            ""2020-05-24T03:30:01.868Z""
          ]
        },
        ""highlight"": {
          ""group"": [
            ""@kibana-highlighted-field@Daklak@/kibana-highlighted-field@"",
            ""@kibana-highlighted-field@Daklak_Webs@/kibana-highlighted-field@""
          ]
        },
        ""sort"": [
          -9223372036854776000
        ]
      },
      {
        ""_index"": ""wazuh-monitoring-3.x-2020.05.24"",
        ""_type"": ""_doc"",
        ""_id"": ""XAHIRHIBENNuuStZrpDz"",
        ""_version"": 1,
        ""_score"": null,
        ""_source"": {
          ""os"": {
            ""arch"": ""x86_64"",
            ""major"": ""6"",
            ""minor"": ""10"",
            ""name"": ""CentOS Linux"",
            ""platform"": ""centos"",
            ""uname"": ""Linux |werserver |2.6.32-754.11.1.el6.x86_64 |#1 SMP Tue Feb 26 15:38:56 UTC 2019 |x86_64"",
            ""version"": ""6.10""
          },
          ""id"": ""060"",
          ""lastKeepAlive"": ""2020-05-24 03:44:48"",
          ""dateAdd"": ""2020-03-27 07:47:46"",
          ""version"": ""Wazuh v3.11.4"",
          ""mergedSum"": ""f69c8a56d2cd8b401643a52cf423ad1c"",
          ""node_name"": ""node02"",
          ""group"": [
            ""Daklak"",
            ""Test"",
            ""VSEC"",
            ""Daklak_Webs"",
            ""Daklak_Apps""
          ],
          ""name"": ""werserver"",
          ""status"": ""Active"",
          ""registerIP"": ""any"",
          ""manager"": ""vsec-logstash"",
          ""configSum"": ""4b54b212c2dd96f8b25aebfea2a104cd"",
          ""ip"": ""192.168.2.14"",
          ""timestamp"": ""2020-05-24T03:45:02.183Z"",
          ""host"": ""vsec-logstash"",
          ""cluster"": {
            ""name"": ""wazuh""
          }
        },
        ""fields"": {
          ""timestamp"": [
            ""2020-05-24T03:45:02.183Z""
          ]
        },
        ""highlight"": {
          ""group"": [
            ""@kibana-highlighted-field@Daklak@/kibana-highlighted-field@"",
            ""@kibana-highlighted-field@Daklak_Webs@/kibana-highlighted-field@""
          ]
        },
        ""sort"": [
          -9223372036854776000
        ]
      },
      {
        ""_index"": ""wazuh-monitoring-3.x-2020.05.24"",
        ""_type"": ""_doc"",
        ""_id"": ""HQT_RHIBENNuuStZnmEn"",
        ""_version"": 1,
        ""_score"": null,
        ""_source"": {
          ""os"": {
            ""arch"": ""x86_64"",
            ""major"": ""6"",
            ""minor"": ""10"",
            ""name"": ""CentOS Linux"",
            ""platform"": ""centos"",
            ""uname"": ""Linux |werserver |2.6.32-754.11.1.el6.x86_64 |#1 SMP Tue Feb 26 15:38:56 UTC 2019 |x86_64"",
            ""version"": ""6.10""
          },
          ""id"": ""060"",
          ""dateAdd"": ""2020-03-27 07:47:46"",
          ""manager"": ""vsec-logstash"",
          ""status"": ""Active"",
          ""node_name"": ""node02"",
          ""lastKeepAlive"": ""2020-05-24 04:44:57"",
          ""ip"": ""192.168.2.14"",
          ""mergedSum"": ""f69c8a56d2cd8b401643a52cf423ad1c"",
          ""group"": [
            ""Daklak"",
            ""Test"",
            ""VSEC"",
            ""Daklak_Webs"",
            ""Daklak_Apps""
          ],
          ""version"": ""Wazuh v3.11.4"",
          ""configSum"": ""4b54b212c2dd96f8b25aebfea2a104cd"",
          ""registerIP"": ""any"",
          ""name"": ""werserver"",
          ""timestamp"": ""2020-05-24T04:45:02.374Z"",
          ""host"": ""vsec-logstash"",
          ""cluster"": {
            ""name"": ""wazuh""
          }
        },
        ""fields"": {
          ""timestamp"": [
            ""2020-05-24T04:45:02.374Z""
          ]
        },
        ""highlight"": {
          ""group"": [
            ""@kibana-highlighted-field@Daklak@/kibana-highlighted-field@"",
            ""@kibana-highlighted-field@Daklak_Webs@/kibana-highlighted-field@""
          ]
        },
        ""sort"": [
          -9223372036854776000
        ]
      },
      {
        ""_index"": ""wazuh-monitoring-3.x-2020.05.24"",
        ""_type"": ""_doc"",
        ""_id"": ""HgT_RHIBENNuuStZnmEn"",
        ""_version"": 1,
        ""_score"": null,
        ""_source"": {
          ""os"": {
            ""arch"": ""x86_64"",
            ""major"": ""6"",
            ""minor"": ""8"",
            ""name"": ""CentOS Linux"",
            ""platform"": ""centos"",
            ""uname"": ""Linux |webserver18 |2.6.32-642.el6.x86_64 |#1 SMP Tue May 10 17:27:01 UTC 2016 |x86_64"",
            ""version"": ""6.8""
          },
          ""id"": ""061"",
          ""dateAdd"": ""2020-03-31 02:15:08"",
          ""manager"": ""vsec-logstash"",
          ""status"": ""Active"",
          ""node_name"": ""node02"",
          ""lastKeepAlive"": ""2020-05-24 04:44:51"",
          ""ip"": ""192.168.2.18"",
          ""mergedSum"": ""535237cb82b543ec180a59bde6425db5"",
          ""group"": [
            ""Daklak"",
            ""Test"",
            ""VSEC"",
            ""Daklak_Webs""
          ],
          ""version"": ""Wazuh v3.11.4"",
          ""configSum"": ""d149ab60df71e6fb53b2d38db75bc733"",
          ""registerIP"": ""any"",
          ""name"": ""webserver18"",
          ""timestamp"": ""2020-05-24T04:45:02.374Z"",
          ""host"": ""vsec-logstash"",
          ""cluster"": {
            ""name"": ""wazuh""
          }
        },
        ""fields"": {
          ""timestamp"": [
            ""2020-05-24T04:45:02.374Z""
          ]
        },
        ""highlight"": {
          ""group"": [
            ""@kibana-highlighted-field@Daklak@/kibana-highlighted-field@"",
            ""@kibana-highlighted-field@Daklak_Webs@/kibana-highlighted-field@""
          ]
        },
        ""sort"": [
          -9223372036854776000
        ]
      },
      {
        ""_index"": ""wazuh-monitoring-3.x-2020.05.24"",
        ""_type"": ""_doc"",
        ""_id"": ""gf-fRHIBENNuuStZem-G"",
        ""_version"": 1,
        ""_score"": null,
        ""_source"": {
          ""os"": {
            ""arch"": ""x86_64"",
            ""major"": ""6"",
            ""minor"": ""10"",
            ""name"": ""CentOS Linux"",
            ""platform"": ""centos"",
            ""uname"": ""Linux |werserver |2.6.32-754.11.1.el6.x86_64 |#1 SMP Tue Feb 26 15:38:56 UTC 2019 |x86_64"",
            ""version"": ""6.10""
          },
          ""name"": ""werserver"",
          ""version"": ""Wazuh v3.11.4"",
          ""manager"": ""vsec-logstash"",
          ""status"": ""Active"",
          ""node_name"": ""node02"",
          ""id"": ""060"",
          ""dateAdd"": ""2020-03-27 07:47:46"",
          ""configSum"": ""4b54b212c2dd96f8b25aebfea2a104cd"",
          ""ip"": ""192.168.2.14"",
          ""lastKeepAlive"": ""2020-05-24 02:59:57"",
          ""mergedSum"": ""f69c8a56d2cd8b401643a52cf423ad1c"",
          ""registerIP"": ""any"",
          ""group"": [
            ""Daklak"",
            ""Test"",
            ""VSEC"",
            ""Daklak_Webs"",
            ""Daklak_Apps""
          ],
          ""timestamp"": ""2020-05-24T03:00:01.789Z"",
          ""host"": ""vsec-logstash"",
          ""cluster"": {
            ""name"": ""wazuh""
          }
        },
        ""fields"": {
          ""timestamp"": [
            ""2020-05-24T03:00:01.789Z""
          ]
        },
        ""highlight"": {
          ""group"": [
            ""@kibana-highlighted-field@Daklak@/kibana-highlighted-field@"",
            ""@kibana-highlighted-field@Daklak_Webs@/kibana-highlighted-field@""
          ]
        },
        ""sort"": [
          -9223372036854776000
        ]
      },
      {
        ""_index"": ""wazuh-monitoring-3.x-2020.05.24"",
        ""_type"": ""_doc"",
        ""_id"": ""sA7NRXIBENNuuStZnPJq"",
        ""_version"": 1,
        ""_score"": null,
        ""_source"": {
          ""os"": {
            ""arch"": ""x86_64"",
            ""major"": ""6"",
            ""minor"": ""10"",
            ""name"": ""CentOS Linux"",
            ""platform"": ""centos"",
            ""uname"": ""Linux |werserver |2.6.32-754.11.1.el6.x86_64 |#1 SMP Tue Feb 26 15:38:56 UTC 2019 |x86_64"",
            ""version"": ""6.10""
          },
          ""configSum"": ""4b54b212c2dd96f8b25aebfea2a104cd"",
          ""manager"": ""vsec-logstash"",
          ""registerIP"": ""any"",
          ""lastKeepAlive"": ""2020-05-24 08:29:47"",
          ""group"": [
            ""Daklak"",
            ""Test"",
            ""VSEC"",
            ""Daklak_Webs"",
            ""Daklak_Apps""
          ],
          ""ip"": ""192.168.2.14"",
          ""id"": ""060"",
          ""status"": ""Active"",
          ""mergedSum"": ""f69c8a56d2cd8b401643a52cf423ad1c"",
          ""version"": ""Wazuh v3.11.4"",
          ""node_name"": ""node02"",
          ""dateAdd"": ""2020-03-27 07:47:46"",
          ""name"": ""werserver"",
          ""timestamp"": ""2020-05-24T08:30:02.344Z"",
          ""host"": ""vsec-logstash"",
          ""cluster"": {
            ""name"": ""wazuh""
          }
        },
        ""fields"": {
          ""timestamp"": [
            ""2020-05-24T08:30:02.344Z""
          ]
        },
        ""highlight"": {
          ""group"": [
            ""@kibana-highlighted-field@Daklak@/kibana-highlighted-field@"",
            ""@kibana-highlighted-field@Daklak_Webs@/kibana-highlighted-field@""
          ]
        },
        ""sort"": [
          -9223372036854776000
        ]
      },
      {
        ""_index"": ""wazuh-monitoring-3.x-2020.05.24"",
        ""_type"": ""_doc"",
        ""_id"": ""sQ_bRXIBENNuuStZVqMO"",
        ""_version"": 1,
        ""_score"": null,
        ""_source"": {
          ""os"": {
            ""arch"": ""x86_64"",
            ""major"": ""6"",
            ""minor"": ""8"",
            ""name"": ""CentOS Linux"",
            ""platform"": ""centos"",
            ""uname"": ""Linux |webserver18 |2.6.32-642.el6.x86_64 |#1 SMP Tue May 10 17:27:01 UTC 2016 |x86_64"",
            ""version"": ""6.8""
          },
          ""dateAdd"": ""2020-03-31 02:15:08"",
          ""configSum"": ""d149ab60df71e6fb53b2d38db75bc733"",
          ""ip"": ""192.168.2.18"",
          ""id"": ""061"",
          ""lastKeepAlive"": ""2020-05-24 08:44:50"",
          ""version"": ""Wazuh v3.11.4"",
          ""node_name"": ""node02"",
          ""manager"": ""vsec-logstash"",
          ""name"": ""webserver18"",
          ""mergedSum"": ""535237cb82b543ec180a59bde6425db5"",
          ""group"": [
            ""Daklak"",
            ""Test"",
            ""VSEC"",
            ""Daklak_Webs""
          ],
          ""status"": ""Active"",
          ""registerIP"": ""any"",
          ""timestamp"": ""2020-05-24T08:45:01.835Z"",
          ""host"": ""vsec-logstash"",
          ""cluster"": {
            ""name"": ""wazuh""
          }
        },
        ""fields"": {
          ""timestamp"": [
            ""2020-05-24T08:45:01.835Z""
          ]
        },
        ""highlight"": {
          ""group"": [
            ""@kibana-highlighted-field@Daklak@/kibana-highlighted-field@"",
            ""@kibana-highlighted-field@Daklak_Webs@/kibana-highlighted-field@""
          ]
        },
        ""sort"": [
          -9223372036854776000
        ]
      },
      {
        ""_index"": ""wazuh-monitoring-3.x-2020.05.24"",
        ""_type"": ""_doc"",
        ""_id"": ""Mw2yRXIBENNuuStZJIPY"",
        ""_version"": 1,
        ""_score"": null,
        ""_source"": {
          ""os"": {
            ""arch"": ""x86_64"",
            ""major"": ""6"",
            ""minor"": ""8"",
            ""name"": ""CentOS Linux"",
            ""platform"": ""centos"",
            ""uname"": ""Linux |webserver18 |2.6.32-642.el6.x86_64 |#1 SMP Tue May 10 17:27:01 UTC 2016 |x86_64"",
            ""version"": ""6.8""
          },
          ""registerIP"": ""any"",
          ""lastKeepAlive"": ""2020-05-24 07:59:50"",
          ""manager"": ""vsec-logstash"",
          ""dateAdd"": ""2020-03-31 02:15:08"",
          ""ip"": ""192.168.2.18"",
          ""configSum"": ""d149ab60df71e6fb53b2d38db75bc733"",
          ""id"": ""061"",
          ""status"": ""Active"",
          ""group"": [
            ""Daklak"",
            ""Test"",
            ""VSEC"",
            ""Daklak_Webs""
          ],
          ""version"": ""Wazuh v3.11.4"",
          ""name"": ""webserver18"",
          ""node_name"": ""node02"",
          ""mergedSum"": ""535237cb82b543ec180a59bde6425db5"",
          ""timestamp"": ""2020-05-24T08:00:02.260Z"",
          ""host"": ""vsec-logstash"",
          ""cluster"": {
            ""name"": ""wazuh""
          }
        },
        ""fields"": {
          ""timestamp"": [
            ""2020-05-24T08:00:02.260Z""
          ]
        },
        ""highlight"": {
          ""group"": [
            ""@kibana-highlighted-field@Daklak@/kibana-highlighted-field@"",
            ""@kibana-highlighted-field@Daklak_Webs@/kibana-highlighted-field@""
          ]
        },
        ""sort"": [
          -9223372036854776000
        ]
      },
      {
        ""_index"": ""wazuh-monitoring-3.x-2020.05.24"",
        ""_type"": ""_doc"",
        ""_id"": ""-xDpRXIBENNuuStZEFQS"",
        ""_version"": 1,
        ""_score"": null,
        ""_source"": {
          ""os"": {
            ""arch"": ""x86_64"",
            ""major"": ""6"",
            ""minor"": ""10"",
            ""name"": ""CentOS Linux"",
            ""platform"": ""centos"",
            ""uname"": ""Linux |werserver |2.6.32-754.11.1.el6.x86_64 |#1 SMP Tue Feb 26 15:38:56 UTC 2019 |x86_64"",
            ""version"": ""6.10""
          },
          ""name"": ""werserver"",
          ""manager"": ""vsec-logstash"",
          ""dateAdd"": ""2020-03-27 07:47:46"",
          ""configSum"": ""4b54b212c2dd96f8b25aebfea2a104cd"",
          ""version"": ""Wazuh v3.11.4"",
          ""registerIP"": ""any"",
          ""status"": ""Active"",
          ""id"": ""060"",
          ""ip"": ""192.168.2.14"",
          ""node_name"": ""node02"",
          ""group"": [
            ""Daklak"",
            ""Test"",
            ""VSEC"",
            ""Daklak_Webs"",
            ""Daklak_Apps""
          ],
          ""mergedSum"": ""f69c8a56d2cd8b401643a52cf423ad1c"",
          ""lastKeepAlive"": ""2020-05-24 08:59:57"",
          ""timestamp"": ""2020-05-24T09:00:01.420Z"",
          ""host"": ""vsec-logstash"",
          ""cluster"": {
            ""name"": ""wazuh""
          }
        },
        ""fields"": {
          ""timestamp"": [
            ""2020-05-24T09:00:01.420Z""
          ]
        },
        ""highlight"": {
          ""group"": [
            ""@kibana-highlighted-field@Daklak@/kibana-highlighted-field@"",
            ""@kibana-highlighted-field@Daklak_Webs@/kibana-highlighted-field@""
          ]
        },
        ""sort"": [
          -9223372036854776000
        ]
      },
      {
        ""_index"": ""wazuh-monitoring-3.x-2020.05.24"",
        ""_type"": ""_doc"",
        ""_id"": ""8BMfRnIBENNuuStZ_iej"",
        ""_version"": 1,
        ""_score"": null,
        ""_source"": {
          ""os"": {
            ""arch"": ""x86_64"",
            ""major"": ""6"",
            ""minor"": ""10"",
            ""name"": ""CentOS Linux"",
            ""platform"": ""centos"",
            ""uname"": ""Linux |werserver |2.6.32-754.11.1.el6.x86_64 |#1 SMP Tue Feb 26 15:38:56 UTC 2019 |x86_64"",
            ""version"": ""6.10""
          },
          ""manager"": ""vsec-logstash"",
          ""status"": ""Active"",
          ""group"": [
            ""Daklak"",
            ""Test"",
            ""VSEC"",
            ""Daklak_Webs"",
            ""Daklak_Apps""
          ],
          ""mergedSum"": ""f69c8a56d2cd8b401643a52cf423ad1c"",
          ""node_name"": ""node02"",
          ""version"": ""Wazuh v3.11.4"",
          ""name"": ""werserver"",
          ""dateAdd"": ""2020-03-27 07:47:46"",
          ""id"": ""060"",
          ""registerIP"": ""any"",
          ""lastKeepAlive"": ""2020-05-24 09:59:47"",
          ""ip"": ""192.168.2.14"",
          ""configSum"": ""4b54b212c2dd96f8b25aebfea2a104cd"",
          ""timestamp"": ""2020-05-24T10:00:01.442Z"",
          ""host"": ""vsec-logstash"",
          ""cluster"": {
            ""name"": ""wazuh""
          }
        },
        ""fields"": {
          ""timestamp"": [
            ""2020-05-24T10:00:01.442Z""
          ]
        },
        ""highlight"": {
          ""group"": [
            ""@kibana-highlighted-field@Daklak@/kibana-highlighted-field@"",
            ""@kibana-highlighted-field@Daklak_Webs@/kibana-highlighted-field@""
          ]
        },
        ""sort"": [
          -9223372036854776000
        ]
      },
      {
        ""_index"": ""wazuh-monitoring-3.x-2020.05.24"",
        ""_type"": ""_doc"",
        ""_id"": ""YRZkRnIBENNuuStZqrVb"",
        ""_version"": 1,
        ""_score"": null,
        ""_source"": {
          ""os"": {
            ""arch"": ""x86_64"",
            ""major"": ""6"",
            ""minor"": ""10"",
            ""name"": ""CentOS Linux"",
            ""platform"": ""centos"",
            ""uname"": ""Linux |werserver |2.6.32-754.11.1.el6.x86_64 |#1 SMP Tue Feb 26 15:38:56 UTC 2019 |x86_64"",
            ""version"": ""6.10""
          },
          ""mergedSum"": ""f69c8a56d2cd8b401643a52cf423ad1c"",
          ""registerIP"": ""any"",
          ""version"": ""Wazuh v3.11.4"",
          ""ip"": ""192.168.2.14"",
          ""dateAdd"": ""2020-03-27 07:47:46"",
          ""manager"": ""vsec-logstash"",
          ""lastKeepAlive"": ""2020-05-24 11:14:48"",
          ""node_name"": ""node02"",
          ""group"": [
            ""Daklak"",
            ""Test"",
            ""VSEC"",
            ""Daklak_Webs"",
            ""Daklak_Apps""
          ],
          ""id"": ""060"",
          ""configSum"": ""4b54b212c2dd96f8b25aebfea2a104cd"",
          ""name"": ""werserver"",
          ""status"": ""Active"",
          ""timestamp"": ""2020-05-24T11:15:01.849Z"",
          ""host"": ""vsec-logstash"",
          ""cluster"": {
            ""name"": ""wazuh""
          }
        },
        ""fields"": {
          ""timestamp"": [
            ""2020-05-24T11:15:01.849Z""
          ]
        },
        ""highlight"": {
          ""group"": [
            ""@kibana-highlighted-field@Daklak@/kibana-highlighted-field@"",
            ""@kibana-highlighted-field@Daklak_Webs@/kibana-highlighted-field@""
          ]
        },
        ""sort"": [
          -9223372036854776000
        ]
      },
      {
        ""_index"": ""wazuh-monitoring-3.x-2020.05.24"",
        ""_type"": ""_doc"",
        ""_id"": ""YhZkRnIBENNuuStZqrVb"",
        ""_version"": 1,
        ""_score"": null,
        ""_source"": {
          ""os"": {
            ""arch"": ""x86_64"",
            ""major"": ""6"",
            ""minor"": ""8"",
            ""name"": ""CentOS Linux"",
            ""platform"": ""centos"",
            ""uname"": ""Linux |webserver18 |2.6.32-642.el6.x86_64 |#1 SMP Tue May 10 17:27:01 UTC 2016 |x86_64"",
            ""version"": ""6.8""
          },
          ""mergedSum"": ""535237cb82b543ec180a59bde6425db5"",
          ""registerIP"": ""any"",
          ""version"": ""Wazuh v3.11.4"",
          ""ip"": ""192.168.2.18"",
          ""dateAdd"": ""2020-03-31 02:15:08"",
          ""manager"": ""vsec-logstash"",
          ""lastKeepAlive"": ""2020-05-24 11:14:47"",
          ""node_name"": ""node02"",
          ""group"": [
            ""Daklak"",
            ""Test"",
            ""VSEC"",
            ""Daklak_Webs""
          ],
          ""id"": ""061"",
          ""configSum"": ""d149ab60df71e6fb53b2d38db75bc733"",
          ""name"": ""webserver18"",
          ""status"": ""Active"",
          ""timestamp"": ""2020-05-24T11:15:01.849Z"",
          ""host"": ""vsec-logstash"",
          ""cluster"": {
            ""name"": ""wazuh""
          }
        },
        ""fields"": {
          ""timestamp"": [
            ""2020-05-24T11:15:01.849Z""
          ]
        },
        ""highlight"": {
          ""group"": [
            ""@kibana-highlighted-field@Daklak@/kibana-highlighted-field@"",
            ""@kibana-highlighted-field@Daklak_Webs@/kibana-highlighted-field@""
          ]
        },
        ""sort"": [
          -9223372036854776000
        ]
      },
      {
        ""_index"": ""wazuh-monitoring-3.x-2020.05.24"",
        ""_type"": ""_doc"",
        ""_id"": ""IBVJRnIBENNuuStZM0d9"",
        ""_version"": 1,
        ""_score"": null,
        ""_source"": {
          ""os"": {
            ""arch"": ""x86_64"",
            ""major"": ""6"",
            ""minor"": ""10"",
            ""name"": ""CentOS Linux"",
            ""platform"": ""centos"",
            ""uname"": ""Linux |werserver |2.6.32-754.11.1.el6.x86_64 |#1 SMP Tue Feb 26 15:38:56 UTC 2019 |x86_64"",
            ""version"": ""6.10""
          },
          ""name"": ""werserver"",
          ""ip"": ""192.168.2.14"",
          ""mergedSum"": ""f69c8a56d2cd8b401643a52cf423ad1c"",
          ""lastKeepAlive"": ""2020-05-24 10:44:58"",
          ""manager"": ""vsec-logstash"",
          ""configSum"": ""4b54b212c2dd96f8b25aebfea2a104cd"",
          ""id"": ""060"",
          ""dateAdd"": ""2020-03-27 07:47:46"",
          ""group"": [
            ""Daklak"",
            ""Test"",
            ""VSEC"",
            ""Daklak_Webs"",
            ""Daklak_Apps""
          ],
          ""registerIP"": ""any"",
          ""version"": ""Wazuh v3.11.4"",
          ""status"": ""Active"",
          ""node_name"": ""node02"",
          ""timestamp"": ""2020-05-24T10:45:01.950Z"",
          ""host"": ""vsec-logstash"",
          ""cluster"": {
            ""name"": ""wazuh""
          }
        },
        ""fields"": {
          ""timestamp"": [
            ""2020-05-24T10:45:01.950Z""
          ]
        },
        ""highlight"": {
          ""group"": [
            ""@kibana-highlighted-field@Daklak@/kibana-highlighted-field@"",
            ""@kibana-highlighted-field@Daklak_Webs@/kibana-highlighted-field@""
          ]
        },
        ""sort"": [
          -9223372036854776000
        ]
      },
      {
        ""_index"": ""wazuh-monitoring-3.x-2020.05.24"",
        ""_type"": ""_doc"",
        ""_id"": ""IRVJRnIBENNuuStZM0d9"",
        ""_version"": 1,
        ""_score"": null,
        ""_source"": {
          ""os"": {
            ""arch"": ""x86_64"",
            ""major"": ""6"",
            ""minor"": ""8"",
            ""name"": ""CentOS Linux"",
            ""platform"": ""centos"",
            ""uname"": ""Linux |webserver18 |2.6.32-642.el6.x86_64 |#1 SMP Tue May 10 17:27:01 UTC 2016 |x86_64"",
            ""version"": ""6.8""
          },
          ""name"": ""webserver18"",
          ""ip"": ""192.168.2.18"",
          ""mergedSum"": ""535237cb82b543ec180a59bde6425db5"",
          ""lastKeepAlive"": ""2020-05-24 10:44:51"",
          ""manager"": ""vsec-logstash"",
          ""configSum"": ""d149ab60df71e6fb53b2d38db75bc733"",
          ""id"": ""061"",
          ""dateAdd"": ""2020-03-31 02:15:08"",
          ""group"": [
            ""Daklak"",
            ""Test"",
            ""VSEC"",
            ""Daklak_Webs""
          ],
          ""registerIP"": ""any"",
          ""version"": ""Wazuh v3.11.4"",
          ""status"": ""Active"",
          ""node_name"": ""node02"",
          ""timestamp"": ""2020-05-24T10:45:01.950Z"",
          ""host"": ""vsec-logstash"",
          ""cluster"": {
            ""name"": ""wazuh""
          }
        },
        ""fields"": {
          ""timestamp"": [
            ""2020-05-24T10:45:01.950Z""
          ]
        },
        ""highlight"": {
          ""group"": [
            ""@kibana-highlighted-field@Daklak@/kibana-highlighted-field@"",
            ""@kibana-highlighted-field@Daklak_Webs@/kibana-highlighted-field@""
          ]
        },
        ""sort"": [
          -9223372036854776000
        ]
      },
      {
        ""_index"": ""wazuh-monitoring-3.x-2020.05.24"",
        ""_type"": ""_doc"",
        ""_id"": ""oBMtRnIBENNuuStZvON3"",
        ""_version"": 1,
        ""_score"": null,
        ""_source"": {
          ""os"": {
            ""arch"": ""x86_64"",
            ""major"": ""6"",
            ""minor"": ""10"",
            ""name"": ""CentOS Linux"",
            ""platform"": ""centos"",
            ""uname"": ""Linux |werserver |2.6.32-754.11.1.el6.x86_64 |#1 SMP Tue Feb 26 15:38:56 UTC 2019 |x86_64"",
            ""version"": ""6.10""
          },
          ""registerIP"": ""any"",
          ""name"": ""werserver"",
          ""manager"": ""vsec-logstash"",
          ""version"": ""Wazuh v3.11.4"",
          ""lastKeepAlive"": ""2020-05-24 10:14:48"",
          ""status"": ""Active"",
          ""id"": ""060"",
          ""group"": [
            ""Daklak"",
            ""Test"",
            ""VSEC"",
            ""Daklak_Webs"",
            ""Daklak_Apps""
          ],
          ""mergedSum"": ""f69c8a56d2cd8b401643a52cf423ad1c"",
          ""dateAdd"": ""2020-03-27 07:47:46"",
          ""ip"": ""192.168.2.14"",
          ""node_name"": ""node02"",
          ""configSum"": ""4b54b212c2dd96f8b25aebfea2a104cd"",
          ""timestamp"": ""2020-05-24T10:15:02.009Z"",
          ""host"": ""vsec-logstash"",
          ""cluster"": {
            ""name"": ""wazuh""
          }
        },
        ""fields"": {
          ""timestamp"": [
            ""2020-05-24T10:15:02.009Z""
          ]
        },
        ""highlight"": {
          ""group"": [
            ""@kibana-highlighted-field@Daklak@/kibana-highlighted-field@"",
            ""@kibana-highlighted-field@Daklak_Webs@/kibana-highlighted-field@""
          ]
        },
        ""sort"": [
          -9223372036854776000
        ]
      },
      {
        ""_index"": ""wazuh-monitoring-3.x-2020.05.24"",
        ""_type"": ""_doc"",
        ""_id"": ""axH2RXIBENNuuStZzRGy"",
        ""_version"": 1,
        ""_score"": null,
        ""_source"": {
          ""os"": {
            ""arch"": ""x86_64"",
            ""major"": ""6"",
            ""minor"": ""8"",
            ""name"": ""CentOS Linux"",
            ""platform"": ""centos"",
            ""uname"": ""Linux |webserver18 |2.6.32-642.el6.x86_64 |#1 SMP Tue May 10 17:27:01 UTC 2016 |x86_64"",
            ""version"": ""6.8""
          },
          ""name"": ""webserver18"",
          ""id"": ""061"",
          ""group"": [
            ""Daklak"",
            ""Test"",
            ""VSEC"",
            ""Daklak_Webs""
          ],
          ""dateAdd"": ""2020-03-31 02:15:08"",
          ""version"": ""Wazuh v3.11.4"",
          ""node_name"": ""node02"",
          ""registerIP"": ""any"",
          ""ip"": ""192.168.2.18"",
          ""configSum"": ""d149ab60df71e6fb53b2d38db75bc733"",
          ""lastKeepAlive"": ""2020-05-24 09:14:57"",
          ""status"": ""Active"",
          ""manager"": ""vsec-logstash"",
          ""mergedSum"": ""535237cb82b543ec180a59bde6425db5"",
          ""timestamp"": ""2020-05-24T09:15:01.934Z"",
          ""host"": ""vsec-logstash"",
          ""cluster"": {
            ""name"": ""wazuh""
          }
        },
        ""fields"": {
          ""timestamp"": [
            ""2020-05-24T09:15:01.934Z""
          ]
        },
        ""highlight"": {
          ""group"": [
            ""@kibana-highlighted-field@Daklak@/kibana-highlighted-field@"",
            ""@kibana-highlighted-field@Daklak_Webs@/kibana-highlighted-field@""
          ]
        },
        ""sort"": [
          -9223372036854776000
        ]
      },
      {
        ""_index"": ""wazuh-monitoring-3.x-2020.05.24"",
        ""_type"": ""_doc"",
        ""_id"": ""QRVWRnIBENNuuStZ7fkX"",
        ""_version"": 1,
        ""_score"": null,
        ""_source"": {
          ""os"": {
            ""arch"": ""x86_64"",
            ""major"": ""6"",
            ""minor"": ""10"",
            ""name"": ""CentOS Linux"",
            ""platform"": ""centos"",
            ""uname"": ""Linux |werserver |2.6.32-754.11.1.el6.x86_64 |#1 SMP Tue Feb 26 15:38:56 UTC 2019 |x86_64"",
            ""version"": ""6.10""
          },
          ""name"": ""werserver"",
          ""status"": ""Active"",
          ""configSum"": ""4b54b212c2dd96f8b25aebfea2a104cd"",
          ""mergedSum"": ""f69c8a56d2cd8b401643a52cf423ad1c"",
          ""dateAdd"": ""2020-03-27 07:47:46"",
          ""node_name"": ""node02"",
          ""registerIP"": ""any"",
          ""id"": ""060"",
          ""lastKeepAlive"": ""2020-05-24 10:59:49"",
          ""ip"": ""192.168.2.14"",
          ""manager"": ""vsec-logstash"",
          ""group"": [
            ""Daklak"",
            ""Test"",
            ""VSEC"",
            ""Daklak_Webs"",
            ""Daklak_Apps""
          ],
          ""version"": ""Wazuh v3.11.4"",
          ""timestamp"": ""2020-05-24T11:00:01.435Z"",
          ""host"": ""vsec-logstash"",
          ""cluster"": {
            ""name"": ""wazuh""
          }
        },
        ""fields"": {
          ""timestamp"": [
            ""2020-05-24T11:00:01.435Z""
          ]
        },
        ""highlight"": {
          ""group"": [
            ""@kibana-highlighted-field@Daklak@/kibana-highlighted-field@"",
            ""@kibana-highlighted-field@Daklak_Webs@/kibana-highlighted-field@""
          ]
        },
        ""sort"": [
          -9223372036854776000
        ]
      },
      {
        ""_index"": ""wazuh-monitoring-3.x-2020.05.24"",
        ""_type"": ""_doc"",
        ""_id"": ""BCjJR3IBENNuuStZt_4e"",
        ""_version"": 1,
        ""_score"": null,
        ""_source"": {
          ""os"": {
            ""arch"": ""x86_64"",
            ""major"": ""6"",
            ""minor"": ""10"",
            ""name"": ""CentOS Linux"",
            ""platform"": ""centos"",
            ""uname"": ""Linux |werserver |2.6.32-754.11.1.el6.x86_64 |#1 SMP Tue Feb 26 15:38:56 UTC 2019 |x86_64"",
            ""version"": ""6.10""
          },
          ""name"": ""werserver"",
          ""group"": [
            ""Daklak"",
            ""Test"",
            ""VSEC"",
            ""Daklak_Webs"",
            ""Daklak_Apps""
          ],
          ""configSum"": ""4b54b212c2dd96f8b25aebfea2a104cd"",
          ""status"": ""Active"",
          ""ip"": ""192.168.2.14"",
          ""id"": ""060"",
          ""lastKeepAlive"": ""2020-05-24 17:44:52"",
          ""dateAdd"": ""2020-03-27 07:47:46"",
          ""manager"": ""vsec-logstash"",
          ""mergedSum"": ""f69c8a56d2cd8b401643a52cf423ad1c"",
          ""node_name"": ""node02"",
          ""registerIP"": ""any"",
          ""version"": ""Wazuh v3.11.4"",
          ""timestamp"": ""2020-05-24T17:45:01.463Z"",
          ""host"": ""vsec-logstash"",
          ""cluster"": {
            ""name"": ""wazuh""
          }
        },
        ""fields"": {
          ""timestamp"": [
            ""2020-05-24T17:45:01.463Z""
          ]
        },
        ""highlight"": {
          ""group"": [
            ""@kibana-highlighted-field@Daklak@/kibana-highlighted-field@"",
            ""@kibana-highlighted-field@Daklak_Webs@/kibana-highlighted-field@""
          ]
        },
        ""sort"": [
          -9223372036854776000
        ]
      },
      {
        ""_index"": ""wazuh-monitoring-3.x-2020.05.24"",
        ""_type"": ""_doc"",
        ""_id"": ""BSjJR3IBENNuuStZt_4e"",
        ""_version"": 1,
        ""_score"": null,
        ""_source"": {
          ""os"": {
            ""arch"": ""x86_64"",
            ""major"": ""6"",
            ""minor"": ""8"",
            ""name"": ""CentOS Linux"",
            ""platform"": ""centos"",
            ""uname"": ""Linux |webserver18 |2.6.32-642.el6.x86_64 |#1 SMP Tue May 10 17:27:01 UTC 2016 |x86_64"",
            ""version"": ""6.8""
          },
          ""name"": ""webserver18"",
          ""group"": [
            ""Daklak"",
            ""Test"",
            ""VSEC"",
            ""Daklak_Webs""
          ],
          ""configSum"": ""d149ab60df71e6fb53b2d38db75bc733"",
          ""status"": ""Active"",
          ""ip"": ""192.168.2.18"",
          ""id"": ""061"",
          ""lastKeepAlive"": ""2020-05-24 17:44:54"",
          ""dateAdd"": ""2020-03-31 02:15:08"",
          ""manager"": ""vsec-logstash"",
          ""mergedSum"": ""535237cb82b543ec180a59bde6425db5"",
          ""node_name"": ""node02"",
          ""registerIP"": ""any"",
          ""version"": ""Wazuh v3.11.4"",
          ""timestamp"": ""2020-05-24T17:45:01.463Z"",
          ""host"": ""vsec-logstash"",
          ""cluster"": {
            ""name"": ""wazuh""
          }
        },
        ""fields"": {
          ""timestamp"": [
            ""2020-05-24T17:45:01.463Z""
          ]
        },
        ""highlight"": {
          ""group"": [
            ""@kibana-highlighted-field@Daklak@/kibana-highlighted-field@"",
            ""@kibana-highlighted-field@Daklak_Webs@/kibana-highlighted-field@""
          ]
        },
        ""sort"": [
          -9223372036854776000
        ]
      },
      {
        ""_index"": ""wazuh-monitoring-3.x-2020.05.24"",
        ""_type"": ""_doc"",
        ""_id"": ""zSeuR3IBENNuuStZQ5jp"",
        ""_version"": 1,
        ""_score"": null,
        ""_source"": {
          ""os"": {
            ""arch"": ""x86_64"",
            ""major"": ""6"",
            ""minor"": ""10"",
            ""name"": ""CentOS Linux"",
            ""platform"": ""centos"",
            ""uname"": ""Linux |werserver |2.6.32-754.11.1.el6.x86_64 |#1 SMP Tue Feb 26 15:38:56 UTC 2019 |x86_64"",
            ""version"": ""6.10""
          },
          ""registerIP"": ""any"",
          ""mergedSum"": ""f69c8a56d2cd8b401643a52cf423ad1c"",
          ""ip"": ""192.168.2.14"",
          ""lastKeepAlive"": ""2020-05-24 17:14:51"",
          ""version"": ""Wazuh v3.11.4"",
          ""manager"": ""vsec-logstash"",
          ""dateAdd"": ""2020-03-27 07:47:46"",
          ""id"": ""060"",
          ""configSum"": ""4b54b212c2dd96f8b25aebfea2a104cd"",
          ""node_name"": ""node02"",
          ""status"": ""Active"",
          ""name"": ""werserver"",
          ""group"": [
            ""Daklak"",
            ""Test"",
            ""VSEC"",
            ""Daklak_Webs"",
            ""Daklak_Apps""
          ],
          ""timestamp"": ""2020-05-24T17:15:02.501Z"",
          ""host"": ""vsec-logstash"",
          ""cluster"": {
            ""name"": ""wazuh""
          }
        },
        ""fields"": {
          ""timestamp"": [
            ""2020-05-24T17:15:02.501Z""
          ]
        },
        ""highlight"": {
          ""group"": [
            ""@kibana-highlighted-field@Daklak@/kibana-highlighted-field@"",
            ""@kibana-highlighted-field@Daklak_Webs@/kibana-highlighted-field@""
          ]
        },
        ""sort"": [
          -9223372036854776000
        ]
      },
      {
        ""_index"": ""wazuh-monitoring-3.x-2020.05.24"",
        ""_type"": ""_doc"",
        ""_id"": ""zieuR3IBENNuuStZQ5jp"",
        ""_version"": 1,
        ""_score"": null,
        ""_source"": {
          ""os"": {
            ""arch"": ""x86_64"",
            ""major"": ""6"",
            ""minor"": ""8"",
            ""name"": ""CentOS Linux"",
            ""platform"": ""centos"",
            ""uname"": ""Linux |webserver18 |2.6.32-642.el6.x86_64 |#1 SMP Tue May 10 17:27:01 UTC 2016 |x86_64"",
            ""version"": ""6.8""
          },
          ""registerIP"": ""any"",
          ""mergedSum"": ""535237cb82b543ec180a59bde6425db5"",
          ""ip"": ""192.168.2.18"",
          ""lastKeepAlive"": ""2020-05-24 17:14:55"",
          ""version"": ""Wazuh v3.11.4"",
          ""manager"": ""vsec-logstash"",
          ""dateAdd"": ""2020-03-31 02:15:08"",
          ""id"": ""061"",
          ""configSum"": ""d149ab60df71e6fb53b2d38db75bc733"",
          ""node_name"": ""node02"",
          ""status"": ""Active"",
          ""name"": ""webserver18"",
          ""group"": [
            ""Daklak"",
            ""Test"",
            ""VSEC"",
            ""Daklak_Webs""
          ],
          ""timestamp"": ""2020-05-24T17:15:02.501Z"",
          ""host"": ""vsec-logstash"",
          ""cluster"": {
            ""name"": ""wazuh""
          }
        },
        ""fields"": {
          ""timestamp"": [
            ""2020-05-24T17:15:02.501Z""
          ]
        },
        ""highlight"": {
          ""group"": [
            ""@kibana-highlighted-field@Daklak@/kibana-highlighted-field@"",
            ""@kibana-highlighted-field@Daklak_Webs@/kibana-highlighted-field@""
          ]
        },
        ""sort"": [
          -9223372036854776000
        ]
      },
      {
        ""_index"": ""wazuh-monitoring-3.x-2020.05.24"",
        ""_type"": ""_doc"",
        ""_id"": ""5iWFR3IBENNuuStZDnmi"",
        ""_version"": 1,
        ""_score"": null,
        ""_source"": {
          ""os"": {
            ""arch"": ""x86_64"",
            ""major"": ""6"",
            ""minor"": ""8"",
            ""name"": ""CentOS Linux"",
            ""platform"": ""centos"",
            ""uname"": ""Linux |webserver18 |2.6.32-642.el6.x86_64 |#1 SMP Tue May 10 17:27:01 UTC 2016 |x86_64"",
            ""version"": ""6.8""
          },
          ""version"": ""Wazuh v3.11.4"",
          ""lastKeepAlive"": ""2020-05-24 16:29:51"",
          ""ip"": ""192.168.2.18"",
          ""id"": ""061"",
          ""dateAdd"": ""2020-03-31 02:15:08"",
          ""configSum"": ""d149ab60df71e6fb53b2d38db75bc733"",
          ""group"": [
            ""Daklak"",
            ""Test"",
            ""VSEC"",
            ""Daklak_Webs""
          ],
          ""name"": ""webserver18"",
          ""mergedSum"": ""535237cb82b543ec180a59bde6425db5"",
          ""status"": ""Active"",
          ""node_name"": ""node02"",
          ""manager"": ""vsec-logstash"",
          ""registerIP"": ""any"",
          ""timestamp"": ""2020-05-24T16:30:01.889Z"",
          ""host"": ""vsec-logstash"",
          ""cluster"": {
            ""name"": ""wazuh""
          }
        },
        ""fields"": {
          ""timestamp"": [
            ""2020-05-24T16:30:01.889Z""
          ]
        },
        ""highlight"": {
          ""group"": [
            ""@kibana-highlighted-field@Daklak@/kibana-highlighted-field@"",
            ""@kibana-highlighted-field@Daklak_Webs@/kibana-highlighted-field@""
          ]
        },
        ""sort"": [
          -9223372036854776000
        ]
      },
      {
        ""_index"": ""wazuh-monitoring-3.x-2020.05.24"",
        ""_type"": ""_doc"",
        ""_id"": ""5yR3R3IBENNuuStZUcfC"",
        ""_version"": 1,
        ""_score"": null,
        ""_source"": {
          ""os"": {
            ""arch"": ""x86_64"",
            ""major"": ""6"",
            ""minor"": ""10"",
            ""name"": ""CentOS Linux"",
            ""platform"": ""centos"",
            ""uname"": ""Linux |werserver |2.6.32-754.11.1.el6.x86_64 |#1 SMP Tue Feb 26 15:38:56 UTC 2019 |x86_64"",
            ""version"": ""6.10""
          },
          ""dateAdd"": ""2020-03-27 07:47:46"",
          ""lastKeepAlive"": ""2020-05-24 16:14:59"",
          ""configSum"": ""4b54b212c2dd96f8b25aebfea2a104cd"",
          ""mergedSum"": ""f69c8a56d2cd8b401643a52cf423ad1c"",
          ""manager"": ""vsec-logstash"",
          ""status"": ""Active"",
          ""ip"": ""192.168.2.14"",
          ""registerIP"": ""any"",
          ""group"": [
            ""Daklak"",
            ""Test"",
            ""VSEC"",
            ""Daklak_Webs"",
            ""Daklak_Apps""
          ],
          ""version"": ""Wazuh v3.11.4"",
          ""name"": ""werserver"",
          ""node_name"": ""node02"",
          ""id"": ""060"",
          ""timestamp"": ""2020-05-24T16:15:01.569Z"",
          ""host"": ""vsec-logstash"",
          ""cluster"": {
            ""name"": ""wazuh""
          }
        },
        ""fields"": {
          ""timestamp"": [
            ""2020-05-24T16:15:01.569Z""
          ]
        },
        ""highlight"": {
          ""group"": [
            ""@kibana-highlighted-field@Daklak@/kibana-highlighted-field@"",
            ""@kibana-highlighted-field@Daklak_Webs@/kibana-highlighted-field@""
          ]
        },
        ""sort"": [
          -9223372036854776000
        ]
      },
      {
        ""_index"": ""wazuh-monitoring-3.x-2020.05.24"",
        ""_type"": ""_doc"",
        ""_id"": ""wCnXR3IBENNuuStZdK_A"",
        ""_version"": 1,
        ""_score"": null,
        ""_source"": {
          ""os"": {
            ""arch"": ""x86_64"",
            ""major"": ""6"",
            ""minor"": ""8"",
            ""name"": ""CentOS Linux"",
            ""platform"": ""centos"",
            ""uname"": ""Linux |webserver18 |2.6.32-642.el6.x86_64 |#1 SMP Tue May 10 17:27:01 UTC 2016 |x86_64"",
            ""version"": ""6.8""
          },
          ""node_name"": ""node02"",
          ""dateAdd"": ""2020-03-31 02:15:08"",
          ""status"": ""Active"",
          ""lastKeepAlive"": ""2020-05-24 17:59:44"",
          ""registerIP"": ""any"",
          ""configSum"": ""d149ab60df71e6fb53b2d38db75bc733"",
          ""group"": [
            ""Daklak"",
            ""Test"",
            ""VSEC"",
            ""Daklak_Webs""
          ],
          ""manager"": ""vsec-logstash"",
          ""id"": ""061"",
          ""name"": ""webserver18"",
          ""version"": ""Wazuh v3.11.4"",
          ""mergedSum"": ""535237cb82b543ec180a59bde6425db5"",
          ""ip"": ""192.168.2.18"",
          ""timestamp"": ""2020-05-24T18:00:01.979Z"",
          ""host"": ""vsec-logstash"",
          ""cluster"": {
            ""name"": ""wazuh""
          }
        },
        ""fields"": {
          ""timestamp"": [
            ""2020-05-24T18:00:01.979Z""
          ]
        },
        ""highlight"": {
          ""group"": [
            ""@kibana-highlighted-field@Daklak@/kibana-highlighted-field@"",
            ""@kibana-highlighted-field@Daklak_Webs@/kibana-highlighted-field@""
          ]
        },
        ""sort"": [
          -9223372036854776000
        ]
      },
      {
        ""_index"": ""wazuh-monitoring-3.x-2020.05.24"",
        ""_type"": ""_doc"",
        ""_id"": ""rCi7R3IBENNuuStZ_UuA"",
        ""_version"": 1,
        ""_score"": null,
        ""_source"": {
          ""os"": {
            ""arch"": ""x86_64"",
            ""major"": ""6"",
            ""minor"": ""10"",
            ""name"": ""CentOS Linux"",
            ""platform"": ""centos"",
            ""uname"": ""Linux |werserver |2.6.32-754.11.1.el6.x86_64 |#1 SMP Tue Feb 26 15:38:56 UTC 2019 |x86_64"",
            ""version"": ""6.10""
          },
          ""registerIP"": ""any"",
          ""version"": ""Wazuh v3.11.4"",
          ""node_name"": ""node02"",
          ""name"": ""werserver"",
          ""manager"": ""vsec-logstash"",
          ""mergedSum"": ""f69c8a56d2cd8b401643a52cf423ad1c"",
          ""lastKeepAlive"": ""2020-05-24 17:29:51"",
          ""ip"": ""192.168.2.14"",
          ""dateAdd"": ""2020-03-27 07:47:46"",
          ""configSum"": ""4b54b212c2dd96f8b25aebfea2a104cd"",
          ""group"": [
            ""Daklak"",
            ""Test"",
            ""VSEC"",
            ""Daklak_Webs"",
            ""Daklak_Apps""
          ],
          ""status"": ""Active"",
          ""id"": ""060"",
          ""timestamp"": ""2020-05-24T17:30:01.979Z"",
          ""host"": ""vsec-logstash"",
          ""cluster"": {
            ""name"": ""wazuh""
          }
        },
        ""fields"": {
          ""timestamp"": [
            ""2020-05-24T17:30:01.979Z""
          ]
        },
        ""highlight"": {
          ""group"": [
            ""@kibana-highlighted-field@Daklak@/kibana-highlighted-field@"",
            ""@kibana-highlighted-field@Daklak_Webs@/kibana-highlighted-field@""
          ]
        },
        ""sort"": [
          -9223372036854776000
        ]
      },
      {
        ""_index"": ""wazuh-monitoring-3.x-2020.05.24"",
        ""_type"": ""_doc"",
        ""_id"": ""oCsASHIBENNuuStZpc6L"",
        ""_version"": 1,
        ""_score"": null,
        ""_source"": {
          ""os"": {
            ""arch"": ""x86_64"",
            ""major"": ""6"",
            ""minor"": ""10"",
            ""name"": ""CentOS Linux"",
            ""platform"": ""centos"",
            ""uname"": ""Linux |werserver |2.6.32-754.11.1.el6.x86_64 |#1 SMP Tue Feb 26 15:38:56 UTC 2019 |x86_64"",
            ""version"": ""6.10""
          },
          ""mergedSum"": ""f69c8a56d2cd8b401643a52cf423ad1c"",
          ""ip"": ""192.168.2.14"",
          ""name"": ""werserver"",
          ""lastKeepAlive"": ""2020-05-24 18:44:51"",
          ""group"": [
            ""Daklak"",
            ""Test"",
            ""VSEC"",
            ""Daklak_Webs"",
            ""Daklak_Apps""
          ],
          ""node_name"": ""node02"",
          ""dateAdd"": ""2020-03-27 07:47:46"",
          ""status"": ""Active"",
          ""registerIP"": ""any"",
          ""id"": ""060"",
          ""version"": ""Wazuh v3.11.4"",
          ""configSum"": ""4b54b212c2dd96f8b25aebfea2a104cd"",
          ""manager"": ""vsec-logstash"",
          ""timestamp"": ""2020-05-24T18:45:01.452Z"",
          ""host"": ""vsec-logstash"",
          ""cluster"": {
            ""name"": ""wazuh""
          }
        },
        ""fields"": {
          ""timestamp"": [
            ""2020-05-24T18:45:01.452Z""
          ]
        },
        ""highlight"": {
          ""group"": [
            ""@kibana-highlighted-field@Daklak@/kibana-highlighted-field@"",
            ""@kibana-highlighted-field@Daklak_Webs@/kibana-highlighted-field@""
          ]
        },
        ""sort"": [
          -9223372036854776000
        ]
      },
      {
        ""_index"": ""wazuh-monitoring-3.x-2020.05.24"",
        ""_type"": ""_doc"",
        ""_id"": ""3C0pSHIBENNuuStZ2-gN"",
        ""_version"": 1,
        ""_score"": null,
        ""_source"": {
          ""os"": {
            ""arch"": ""x86_64"",
            ""major"": ""6"",
            ""minor"": ""10"",
            ""name"": ""CentOS Linux"",
            ""platform"": ""centos"",
            ""uname"": ""Linux |werserver |2.6.32-754.11.1.el6.x86_64 |#1 SMP Tue Feb 26 15:38:56 UTC 2019 |x86_64"",
            ""version"": ""6.10""
          },
          ""manager"": ""vsec-logstash"",
          ""group"": [
            ""Daklak"",
            ""Test"",
            ""VSEC"",
            ""Daklak_Webs"",
            ""Daklak_Apps""
          ],
          ""mergedSum"": ""f69c8a56d2cd8b401643a52cf423ad1c"",
          ""lastKeepAlive"": ""2020-05-24 19:29:52"",
          ""registerIP"": ""any"",
          ""configSum"": ""4b54b212c2dd96f8b25aebfea2a104cd"",
          ""id"": ""060"",
          ""ip"": ""192.168.2.14"",
          ""version"": ""Wazuh v3.11.4"",
          ""status"": ""Active"",
          ""name"": ""werserver"",
          ""node_name"": ""node02"",
          ""dateAdd"": ""2020-03-27 07:47:46"",
          ""timestamp"": ""2020-05-24T19:30:02.122Z"",
          ""host"": ""vsec-logstash"",
          ""cluster"": {
            ""name"": ""wazuh""
          }
        },
        ""fields"": {
          ""timestamp"": [
            ""2020-05-24T19:30:02.122Z""
          ]
        },
        ""highlight"": {
          ""group"": [
            ""@kibana-highlighted-field@Daklak@/kibana-highlighted-field@"",
            ""@kibana-highlighted-field@Daklak_Webs@/kibana-highlighted-field@""
          ]
        },
        ""sort"": [
          -9223372036854776000
        ]
      },
      {
        ""_index"": ""wazuh-monitoring-3.x-2020.05.24"",
        ""_type"": ""_doc"",
        ""_id"": ""3S0pSHIBENNuuStZ2-gN"",
        ""_version"": 1,
        ""_score"": null,
        ""_source"": {
          ""os"": {
            ""arch"": ""x86_64"",
            ""major"": ""6"",
            ""minor"": ""8"",
            ""name"": ""CentOS Linux"",
            ""platform"": ""centos"",
            ""uname"": ""Linux |webserver18 |2.6.32-642.el6.x86_64 |#1 SMP Tue May 10 17:27:01 UTC 2016 |x86_64"",
            ""version"": ""6.8""
          },
          ""manager"": ""vsec-logstash"",
          ""group"": [
            ""Daklak"",
            ""Test"",
            ""VSEC"",
            ""Daklak_Webs""
          ],
          ""mergedSum"": ""535237cb82b543ec180a59bde6425db5"",
          ""lastKeepAlive"": ""2020-05-24 19:29:44"",
          ""registerIP"": ""any"",
          ""configSum"": ""d149ab60df71e6fb53b2d38db75bc733"",
          ""id"": ""061"",
          ""ip"": ""192.168.2.18"",
          ""version"": ""Wazuh v3.11.4"",
          ""status"": ""Active"",
          ""name"": ""webserver18"",
          ""node_name"": ""node02"",
          ""dateAdd"": ""2020-03-31 02:15:08"",
          ""timestamp"": ""2020-05-24T19:30:02.122Z"",
          ""host"": ""vsec-logstash"",
          ""cluster"": {
            ""name"": ""wazuh""
          }
        },
        ""fields"": {
          ""timestamp"": [
            ""2020-05-24T19:30:02.122Z""
          ]
        },
        ""highlight"": {
          ""group"": [
            ""@kibana-highlighted-field@Daklak@/kibana-highlighted-field@"",
            ""@kibana-highlighted-field@Daklak_Webs@/kibana-highlighted-field@""
          ]
        },
        ""sort"": [
          -9223372036854776000
        ]
      },
      {
        ""_index"": ""wazuh-monitoring-3.x-2020.05.24"",
        ""_type"": ""_doc"",
        ""_id"": ""fivyR3IBENNuuStZ7BwD"",
        ""_version"": 1,
        ""_score"": null,
        ""_source"": {
          ""os"": {
            ""arch"": ""x86_64"",
            ""major"": ""6"",
            ""minor"": ""10"",
            ""name"": ""CentOS Linux"",
            ""platform"": ""centos"",
            ""uname"": ""Linux |werserver |2.6.32-754.11.1.el6.x86_64 |#1 SMP Tue Feb 26 15:38:56 UTC 2019 |x86_64"",
            ""version"": ""6.10""
          },
          ""mergedSum"": ""f69c8a56d2cd8b401643a52cf423ad1c"",
          ""version"": ""Wazuh v3.11.4"",
          ""node_name"": ""node02"",
          ""registerIP"": ""any"",
          ""status"": ""Active"",
          ""ip"": ""192.168.2.14"",
          ""configSum"": ""4b54b212c2dd96f8b25aebfea2a104cd"",
          ""lastKeepAlive"": ""2020-05-24 18:29:51"",
          ""name"": ""werserver"",
          ""id"": ""060"",
          ""dateAdd"": ""2020-03-27 07:47:46"",
          ""group"": [
            ""Daklak"",
            ""Test"",
            ""VSEC"",
            ""Daklak_Webs"",
            ""Daklak_Apps""
          ],
          ""manager"": ""vsec-logstash"",
          ""timestamp"": ""2020-05-24T18:30:01.989Z"",
          ""host"": ""vsec-logstash"",
          ""cluster"": {
            ""name"": ""wazuh""
          }
        },
        ""fields"": {
          ""timestamp"": [
            ""2020-05-24T18:30:01.989Z""
          ]
        },
        ""highlight"": {
          ""group"": [
            ""@kibana-highlighted-field@Daklak@/kibana-highlighted-field@"",
            ""@kibana-highlighted-field@Daklak_Webs@/kibana-highlighted-field@""
          ]
        },
        ""sort"": [
          -9223372036854776000
        ]
      },
      {
        ""_index"": ""wazuh-monitoring-3.x-2020.05.24"",
        ""_type"": ""_doc"",
        ""_id"": ""fyvyR3IBENNuuStZ7BwD"",
        ""_version"": 1,
        ""_score"": null,
        ""_source"": {
          ""os"": {
            ""arch"": ""x86_64"",
            ""major"": ""6"",
            ""minor"": ""8"",
            ""name"": ""CentOS Linux"",
            ""platform"": ""centos"",
            ""uname"": ""Linux |webserver18 |2.6.32-642.el6.x86_64 |#1 SMP Tue May 10 17:27:01 UTC 2016 |x86_64"",
            ""version"": ""6.8""
          },
          ""mergedSum"": ""535237cb82b543ec180a59bde6425db5"",
          ""version"": ""Wazuh v3.11.4"",
          ""node_name"": ""node02"",
          ""registerIP"": ""any"",
          ""status"": ""Active"",
          ""ip"": ""192.168.2.18"",
          ""configSum"": ""d149ab60df71e6fb53b2d38db75bc733"",
          ""lastKeepAlive"": ""2020-05-24 18:29:54"",
          ""name"": ""webserver18"",
          ""id"": ""061"",
          ""dateAdd"": ""2020-03-31 02:15:08"",
          ""group"": [
            ""Daklak"",
            ""Test"",
            ""VSEC"",
            ""Daklak_Webs""
          ],
          ""manager"": ""vsec-logstash"",
          ""timestamp"": ""2020-05-24T18:30:01.989Z"",
          ""host"": ""vsec-logstash"",
          ""cluster"": {
            ""name"": ""wazuh""
          }
        },
        ""fields"": {
          ""timestamp"": [
            ""2020-05-24T18:30:01.989Z""
          ]
        },
        ""highlight"": {
          ""group"": [
            ""@kibana-highlighted-field@Daklak@/kibana-highlighted-field@"",
            ""@kibana-highlighted-field@Daklak_Webs@/kibana-highlighted-field@""
          ]
        },
        ""sort"": [
          -9223372036854776000
        ]
      },
      {
        ""_index"": ""wazuh-monitoring-3.x-2020.05.24"",
        ""_type"": ""_doc"",
        ""_id"": ""ajBTSHIBENNuuStZDwc-"",
        ""_version"": 1,
        ""_score"": null,
        ""_source"": {
          ""os"": {
            ""arch"": ""x86_64"",
            ""major"": ""6"",
            ""minor"": ""10"",
            ""name"": ""CentOS Linux"",
            ""platform"": ""centos"",
            ""uname"": ""Linux |werserver |2.6.32-754.11.1.el6.x86_64 |#1 SMP Tue Feb 26 15:38:56 UTC 2019 |x86_64"",
            ""version"": ""6.10""
          },
          ""manager"": ""vsec-logstash"",
          ""name"": ""werserver"",
          ""id"": ""060"",
          ""lastKeepAlive"": ""2020-05-24 20:14:50"",
          ""registerIP"": ""any"",
          ""configSum"": ""4b54b212c2dd96f8b25aebfea2a104cd"",
          ""mergedSum"": ""f69c8a56d2cd8b401643a52cf423ad1c"",
          ""version"": ""Wazuh v3.11.4"",
          ""dateAdd"": ""2020-03-27 07:47:46"",
          ""group"": [
            ""Daklak"",
            ""Test"",
            ""VSEC"",
            ""Daklak_Webs"",
            ""Daklak_Apps""
          ],
          ""status"": ""Active"",
          ""node_name"": ""node02"",
          ""ip"": ""192.168.2.14"",
          ""timestamp"": ""2020-05-24T20:15:02.463Z"",
          ""host"": ""vsec-logstash"",
          ""cluster"": {
            ""name"": ""wazuh""
          }
        },
        ""fields"": {
          ""timestamp"": [
            ""2020-05-24T20:15:02.463Z""
          ]
        },
        ""highlight"": {
          ""group"": [
            ""@kibana-highlighted-field@Daklak@/kibana-highlighted-field@"",
            ""@kibana-highlighted-field@Daklak_Webs@/kibana-highlighted-field@""
          ]
        },
        ""sort"": [
          -9223372036854776000
        ]
      },
      {
        ""_index"": ""wazuh-monitoring-3.x-2020.05.24"",
        ""_type"": ""_doc"",
        ""_id"": ""LiwOSHIBENNuuStZYn_7"",
        ""_version"": 1,
        ""_score"": null,
        ""_source"": {
          ""os"": {
            ""arch"": ""x86_64"",
            ""major"": ""6"",
            ""minor"": ""10"",
            ""name"": ""CentOS Linux"",
            ""platform"": ""centos"",
            ""uname"": ""Linux |werserver |2.6.32-754.11.1.el6.x86_64 |#1 SMP Tue Feb 26 15:38:56 UTC 2019 |x86_64"",
            ""version"": ""6.10""
          },
          ""configSum"": ""4b54b212c2dd96f8b25aebfea2a104cd"",
          ""registerIP"": ""any"",
          ""id"": ""060"",
          ""name"": ""werserver"",
          ""lastKeepAlive"": ""2020-05-24 18:59:51"",
          ""status"": ""Active"",
          ""ip"": ""192.168.2.14"",
          ""dateAdd"": ""2020-03-27 07:47:46"",
          ""manager"": ""vsec-logstash"",
          ""node_name"": ""node02"",
          ""version"": ""Wazuh v3.11.4"",
          ""mergedSum"": ""f69c8a56d2cd8b401643a52cf423ad1c"",
          ""group"": [
            ""Daklak"",
            ""Test"",
            ""VSEC"",
            ""Daklak_Webs"",
            ""Daklak_Apps""
          ],
          ""timestamp"": ""2020-05-24T19:00:01.913Z"",
          ""host"": ""vsec-logstash"",
          ""cluster"": {
            ""name"": ""wazuh""
          }
        },
        ""fields"": {
          ""timestamp"": [
            ""2020-05-24T19:00:01.913Z""
          ]
        },
        ""highlight"": {
          ""group"": [
            ""@kibana-highlighted-field@Daklak@/kibana-highlighted-field@"",
            ""@kibana-highlighted-field@Daklak_Webs@/kibana-highlighted-field@""
          ]
        },
        ""sort"": [
          -9223372036854776000
        ]
      },
      {
        ""_index"": ""wazuh-monitoring-3.x-2020.05.24"",
        ""_type"": ""_doc"",
        ""_id"": ""hi9FSHIBENNuuStZUUx2"",
        ""_version"": 1,
        ""_score"": null,
        ""_source"": {
          ""os"": {
            ""arch"": ""x86_64"",
            ""major"": ""6"",
            ""minor"": ""10"",
            ""name"": ""CentOS Linux"",
            ""platform"": ""centos"",
            ""uname"": ""Linux |werserver |2.6.32-754.11.1.el6.x86_64 |#1 SMP Tue Feb 26 15:38:56 UTC 2019 |x86_64"",
            ""version"": ""6.10""
          },
          ""dateAdd"": ""2020-03-27 07:47:46"",
          ""id"": ""060"",
          ""group"": [
            ""Daklak"",
            ""Test"",
            ""VSEC"",
            ""Daklak_Webs"",
            ""Daklak_Apps""
          ],
          ""node_name"": ""node02"",
          ""registerIP"": ""any"",
          ""lastKeepAlive"": ""2020-05-24 19:59:50"",
          ""manager"": ""vsec-logstash"",
          ""mergedSum"": ""f69c8a56d2cd8b401643a52cf423ad1c"",
          ""name"": ""werserver"",
          ""version"": ""Wazuh v3.11.4"",
          ""configSum"": ""4b54b212c2dd96f8b25aebfea2a104cd"",
          ""ip"": ""192.168.2.14"",
          ""status"": ""Active"",
          ""timestamp"": ""2020-05-24T20:00:01.911Z"",
          ""host"": ""vsec-logstash"",
          ""cluster"": {
            ""name"": ""wazuh""
          }
        },
        ""fields"": {
          ""timestamp"": [
            ""2020-05-24T20:00:01.911Z""
          ]
        },
        ""highlight"": {
          ""group"": [
            ""@kibana-highlighted-field@Daklak@/kibana-highlighted-field@"",
            ""@kibana-highlighted-field@Daklak_Webs@/kibana-highlighted-field@""
          ]
        },
        ""sort"": [
          -9223372036854776000
        ]
      },
      {
        ""_index"": ""wazuh-monitoring-3.x-2020.05.24"",
        ""_type"": ""_doc"",
        ""_id"": ""4CrlR3IBENNuuStZLmop"",
        ""_version"": 1,
        ""_score"": null,
        ""_source"": {
          ""os"": {
            ""arch"": ""x86_64"",
            ""major"": ""6"",
            ""minor"": ""8"",
            ""name"": ""CentOS Linux"",
            ""platform"": ""centos"",
            ""uname"": ""Linux |webserver18 |2.6.32-642.el6.x86_64 |#1 SMP Tue May 10 17:27:01 UTC 2016 |x86_64"",
            ""version"": ""6.8""
          },
          ""name"": ""webserver18"",
          ""configSum"": ""d149ab60df71e6fb53b2d38db75bc733"",
          ""version"": ""Wazuh v3.11.4"",
          ""status"": ""Active"",
          ""node_name"": ""node02"",
          ""mergedSum"": ""535237cb82b543ec180a59bde6425db5"",
          ""id"": ""061"",
          ""registerIP"": ""any"",
          ""manager"": ""vsec-logstash"",
          ""group"": [
            ""Daklak"",
            ""Test"",
            ""VSEC"",
            ""Daklak_Webs""
          ],
          ""dateAdd"": ""2020-03-31 02:15:08"",
          ""ip"": ""192.168.2.18"",
          ""lastKeepAlive"": ""2020-05-24 18:14:44"",
          ""timestamp"": ""2020-05-24T18:15:01.416Z"",
          ""host"": ""vsec-logstash"",
          ""cluster"": {
            ""name"": ""wazuh""
          }
        },
        ""fields"": {
          ""timestamp"": [
            ""2020-05-24T18:15:01.416Z""
          ]
        },
        ""highlight"": {
          ""group"": [
            ""@kibana-highlighted-field@Daklak@/kibana-highlighted-field@"",
            ""@kibana-highlighted-field@Daklak_Webs@/kibana-highlighted-field@""
          ]
        },
        ""sort"": [
          -9223372036854776000
        ]
      },
      {
        ""_index"": ""wazuh-monitoring-3.x-2020.05.24"",
        ""_type"": ""_doc"",
        ""_id"": ""1v6ERHIBENNuuStZAgv5"",
        ""_version"": 1,
        ""_score"": null,
        ""_source"": {
          ""os"": {
            ""arch"": ""x86_64"",
            ""major"": ""6"",
            ""minor"": ""8"",
            ""name"": ""CentOS Linux"",
            ""platform"": ""centos"",
            ""uname"": ""Linux |webserver18 |2.6.32-642.el6.x86_64 |#1 SMP Tue May 10 17:27:01 UTC 2016 |x86_64"",
            ""version"": ""6.8""
          },
          ""lastKeepAlive"": ""2020-05-24 02:29:52"",
          ""ip"": ""192.168.2.18"",
          ""id"": ""061"",
          ""mergedSum"": ""535237cb82b543ec180a59bde6425db5"",
          ""manager"": ""vsec-logstash"",
          ""dateAdd"": ""2020-03-31 02:15:08"",
          ""node_name"": ""node02"",
          ""registerIP"": ""any"",
          ""group"": [
            ""Daklak"",
            ""Test"",
            ""VSEC"",
            ""Daklak_Webs""
          ],
          ""configSum"": ""d149ab60df71e6fb53b2d38db75bc733"",
          ""status"": ""Active"",
          ""version"": ""Wazuh v3.11.4"",
          ""name"": ""webserver18"",
          ""timestamp"": ""2020-05-24T02:30:01.712Z"",
          ""host"": ""vsec-logstash"",
          ""cluster"": {
            ""name"": ""wazuh""
          }
        },
        ""fields"": {
          ""timestamp"": [
            ""2020-05-24T02:30:01.712Z""
          ]
        },
        ""highlight"": {
          ""group"": [
            ""@kibana-highlighted-field@Daklak@/kibana-highlighted-field@"",
            ""@kibana-highlighted-field@Daklak_Webs@/kibana-highlighted-field@""
          ]
        },
        ""sort"": [
          -9223372036854776000
        ]
      },
      {
        ""_index"": ""wazuh-monitoring-3.x-2020.05.24"",
        ""_type"": ""_doc"",
        ""_id"": ""rAdERXIBENNuuStZRuN1"",
        ""_version"": 1,
        ""_score"": null,
        ""_source"": {
          ""os"": {
            ""arch"": ""x86_64"",
            ""major"": ""6"",
            ""minor"": ""10"",
            ""name"": ""CentOS Linux"",
            ""platform"": ""centos"",
            ""uname"": ""Linux |werserver |2.6.32-754.11.1.el6.x86_64 |#1 SMP Tue Feb 26 15:38:56 UTC 2019 |x86_64"",
            ""version"": ""6.10""
          },
          ""manager"": ""vsec-logstash"",
          ""version"": ""Wazuh v3.11.4"",
          ""ip"": ""192.168.2.14"",
          ""id"": ""060"",
          ""group"": [
            ""Daklak"",
            ""Test"",
            ""VSEC"",
            ""Daklak_Webs"",
            ""Daklak_Apps""
          ],
          ""name"": ""werserver"",
          ""lastKeepAlive"": ""2020-05-24 05:59:58"",
          ""registerIP"": ""any"",
          ""status"": ""Active"",
          ""node_name"": ""node02"",
          ""dateAdd"": ""2020-03-27 07:47:46"",
          ""mergedSum"": ""f69c8a56d2cd8b401643a52cf423ad1c"",
          ""configSum"": ""4b54b212c2dd96f8b25aebfea2a104cd"",
          ""timestamp"": ""2020-05-24T06:00:01.906Z"",
          ""host"": ""vsec-logstash"",
          ""cluster"": {
            ""name"": ""wazuh""
          }
        },
        ""fields"": {
          ""timestamp"": [
            ""2020-05-24T06:00:01.906Z""
          ]
        },
        ""highlight"": {
          ""group"": [
            ""@kibana-highlighted-field@Daklak@/kibana-highlighted-field@"",
            ""@kibana-highlighted-field@Daklak_Webs@/kibana-highlighted-field@""
          ]
        },
        ""sort"": [
          -9223372036854776000
        ]
      },
      {
        ""_index"": ""wazuh-monitoring-3.x-2020.05.24"",
        ""_type"": ""_doc"",
        ""_id"": ""rQdERXIBENNuuStZRuN1"",
        ""_version"": 1,
        ""_score"": null,
        ""_source"": {
          ""os"": {
            ""arch"": ""x86_64"",
            ""major"": ""6"",
            ""minor"": ""8"",
            ""name"": ""CentOS Linux"",
            ""platform"": ""centos"",
            ""uname"": ""Linux |webserver18 |2.6.32-642.el6.x86_64 |#1 SMP Tue May 10 17:27:01 UTC 2016 |x86_64"",
            ""version"": ""6.8""
          },
          ""manager"": ""vsec-logstash"",
          ""version"": ""Wazuh v3.11.4"",
          ""ip"": ""192.168.2.18"",
          ""id"": ""061"",
          ""group"": [
            ""Daklak"",
            ""Test"",
            ""VSEC"",
            ""Daklak_Webs""
          ],
          ""name"": ""webserver18"",
          ""lastKeepAlive"": ""2020-05-24 05:59:52"",
          ""registerIP"": ""any"",
          ""status"": ""Active"",
          ""node_name"": ""node02"",
          ""dateAdd"": ""2020-03-31 02:15:08"",
          ""mergedSum"": ""535237cb82b543ec180a59bde6425db5"",
          ""configSum"": ""d149ab60df71e6fb53b2d38db75bc733"",
          ""timestamp"": ""2020-05-24T06:00:01.906Z"",
          ""host"": ""vsec-logstash"",
          ""cluster"": {
            ""name"": ""wazuh""
          }
        },
        ""fields"": {
          ""timestamp"": [
            ""2020-05-24T06:00:01.906Z""
          ]
        },
        ""highlight"": {
          ""group"": [
            ""@kibana-highlighted-field@Daklak@/kibana-highlighted-field@"",
            ""@kibana-highlighted-field@Daklak_Webs@/kibana-highlighted-field@""
          ]
        },
        ""sort"": [
          -9223372036854776000
        ]
      },
      {
        ""_index"": ""wazuh-monitoring-3.x-2020.05.24"",
        ""_type"": ""_doc"",
        ""_id"": ""mgUbRXIBENNuuStZFs5r"",
        ""_version"": 1,
        ""_score"": null,
        ""_source"": {
          ""os"": {
            ""arch"": ""x86_64"",
            ""major"": ""6"",
            ""minor"": ""8"",
            ""name"": ""CentOS Linux"",
            ""platform"": ""centos"",
            ""uname"": ""Linux |webserver18 |2.6.32-642.el6.x86_64 |#1 SMP Tue May 10 17:27:01 UTC 2016 |x86_64"",
            ""version"": ""6.8""
          },
          ""node_name"": ""node02"",
          ""registerIP"": ""any"",
          ""manager"": ""vsec-logstash"",
          ""group"": [
            ""Daklak"",
            ""Test"",
            ""VSEC"",
            ""Daklak_Webs""
          ],
          ""configSum"": ""d149ab60df71e6fb53b2d38db75bc733"",
          ""name"": ""webserver18"",
          ""dateAdd"": ""2020-03-31 02:15:08"",
          ""status"": ""Active"",
          ""ip"": ""192.168.2.18"",
          ""version"": ""Wazuh v3.11.4"",
          ""id"": ""061"",
          ""lastKeepAlive"": ""2020-05-24 05:14:52"",
          ""mergedSum"": ""535237cb82b543ec180a59bde6425db5"",
          ""timestamp"": ""2020-05-24T05:15:02.633Z"",
          ""host"": ""vsec-logstash"",
          ""cluster"": {
            ""name"": ""wazuh""
          }
        },
        ""fields"": {
          ""timestamp"": [
            ""2020-05-24T05:15:02.633Z""
          ]
        },
        ""highlight"": {
          ""group"": [
            ""@kibana-highlighted-field@Daklak@/kibana-highlighted-field@"",
            ""@kibana-highlighted-field@Daklak_Webs@/kibana-highlighted-field@""
          ]
        },
        ""sort"": [
          -9223372036854776000
        ]
      },
      {
        ""_index"": ""wazuh-monitoring-3.x-2020.05.24"",
        ""_type"": ""_doc"",
        ""_id"": ""fghSRXIBENNuuStZBJ8I"",
        ""_version"": 1,
        ""_score"": null,
        ""_source"": {
          ""os"": {
            ""arch"": ""x86_64"",
            ""major"": ""6"",
            ""minor"": ""8"",
            ""name"": ""CentOS Linux"",
            ""platform"": ""centos"",
            ""uname"": ""Linux |webserver18 |2.6.32-642.el6.x86_64 |#1 SMP Tue May 10 17:27:01 UTC 2016 |x86_64"",
            ""version"": ""6.8""
          },
          ""status"": ""Active"",
          ""registerIP"": ""any"",
          ""name"": ""webserver18"",
          ""id"": ""061"",
          ""manager"": ""vsec-logstash"",
          ""lastKeepAlive"": ""2020-05-24 06:14:52"",
          ""dateAdd"": ""2020-03-31 02:15:08"",
          ""configSum"": ""d149ab60df71e6fb53b2d38db75bc733"",
          ""group"": [
            ""Daklak"",
            ""Test"",
            ""VSEC"",
            ""Daklak_Webs""
          ],
          ""node_name"": ""node02"",
          ""version"": ""Wazuh v3.11.4"",
          ""ip"": ""192.168.2.18"",
          ""mergedSum"": ""535237cb82b543ec180a59bde6425db5"",
          ""timestamp"": ""2020-05-24T06:15:02.412Z"",
          ""host"": ""vsec-logstash"",
          ""cluster"": {
            ""name"": ""wazuh""
          }
        },
        ""fields"": {
          ""timestamp"": [
            ""2020-05-24T06:15:02.412Z""
          ]
        },
        ""highlight"": {
          ""group"": [
            ""@kibana-highlighted-field@Daklak@/kibana-highlighted-field@"",
            ""@kibana-highlighted-field@Daklak_Webs@/kibana-highlighted-field@""
          ]
        },
        ""sort"": [
          -9223372036854776000
        ]
      },
      {
        ""_index"": ""wazuh-monitoring-3.x-2020.05.24"",
        ""_type"": ""_doc"",
        ""_id"": ""Twp7RXIBENNuuStZNbPv"",
        ""_version"": 1,
        ""_score"": null,
        ""_source"": {
          ""os"": {
            ""arch"": ""x86_64"",
            ""major"": ""6"",
            ""minor"": ""8"",
            ""name"": ""CentOS Linux"",
            ""platform"": ""centos"",
            ""uname"": ""Linux |webserver18 |2.6.32-642.el6.x86_64 |#1 SMP Tue May 10 17:27:01 UTC 2016 |x86_64"",
            ""version"": ""6.8""
          },
          ""name"": ""webserver18"",
          ""group"": [
            ""Daklak"",
            ""Test"",
            ""VSEC"",
            ""Daklak_Webs""
          ],
          ""manager"": ""vsec-logstash"",
          ""ip"": ""192.168.2.18"",
          ""dateAdd"": ""2020-03-31 02:15:08"",
          ""mergedSum"": ""535237cb82b543ec180a59bde6425db5"",
          ""registerIP"": ""any"",
          ""status"": ""Active"",
          ""lastKeepAlive"": ""2020-05-24 06:59:49"",
          ""configSum"": ""d149ab60df71e6fb53b2d38db75bc733"",
          ""version"": ""Wazuh v3.11.4"",
          ""id"": ""061"",
          ""node_name"": ""node02"",
          ""timestamp"": ""2020-05-24T07:00:02.161Z"",
          ""host"": ""vsec-logstash"",
          ""cluster"": {
            ""name"": ""wazuh""
          }
        },
        ""fields"": {
          ""timestamp"": [
            ""2020-05-24T07:00:02.161Z""
          ]
        },
        ""highlight"": {
          ""group"": [
            ""@kibana-highlighted-field@Daklak@/kibana-highlighted-field@"",
            ""@kibana-highlighted-field@Daklak_Webs@/kibana-highlighted-field@""
          ]
        },
        ""sort"": [
          -9223372036854776000
        ]
      },
      {
        ""_index"": ""wazuh-monitoring-3.x-2020.05.24"",
        ""_type"": ""_doc"",
        ""_id"": ""EAptRXIBENNuuStZewEf"",
        ""_version"": 1,
        ""_score"": null,
        ""_source"": {
          ""os"": {
            ""arch"": ""x86_64"",
            ""major"": ""6"",
            ""minor"": ""8"",
            ""name"": ""CentOS Linux"",
            ""platform"": ""centos"",
            ""uname"": ""Linux |webserver18 |2.6.32-642.el6.x86_64 |#1 SMP Tue May 10 17:27:01 UTC 2016 |x86_64"",
            ""version"": ""6.8""
          },
          ""version"": ""Wazuh v3.11.4"",
          ""registerIP"": ""any"",
          ""mergedSum"": ""535237cb82b543ec180a59bde6425db5"",
          ""ip"": ""192.168.2.18"",
          ""node_name"": ""node02"",
          ""id"": ""061"",
          ""dateAdd"": ""2020-03-31 02:15:08"",
          ""name"": ""webserver18"",
          ""configSum"": ""d149ab60df71e6fb53b2d38db75bc733"",
          ""manager"": ""vsec-logstash"",
          ""group"": [
            ""Daklak"",
            ""Test"",
            ""VSEC"",
            ""Daklak_Webs""
          ],
          ""lastKeepAlive"": ""2020-05-24 06:44:49"",
          ""status"": ""Active"",
          ""timestamp"": ""2020-05-24T06:45:02.349Z"",
          ""host"": ""vsec-logstash"",
          ""cluster"": {
            ""name"": ""wazuh""
          }
        },
        ""fields"": {
          ""timestamp"": [
            ""2020-05-24T06:45:02.349Z""
          ]
        },
        ""highlight"": {
          ""group"": [
            ""@kibana-highlighted-field@Daklak@/kibana-highlighted-field@"",
            ""@kibana-highlighted-field@Daklak_Webs@/kibana-highlighted-field@""
          ]
        },
        ""sort"": [
          -9223372036854776000
        ]
      },
      {
        ""_index"": ""wazuh-monitoring-3.x-2020.05.24"",
        ""_type"": ""_doc"",
        ""_id"": ""mgc2RXIBENNuuStZjDLl"",
        ""_version"": 1,
        ""_score"": null,
        ""_source"": {
          ""os"": {
            ""arch"": ""x86_64"",
            ""major"": ""6"",
            ""minor"": ""10"",
            ""name"": ""CentOS Linux"",
            ""platform"": ""centos"",
            ""uname"": ""Linux |werserver |2.6.32-754.11.1.el6.x86_64 |#1 SMP Tue Feb 26 15:38:56 UTC 2019 |x86_64"",
            ""version"": ""6.10""
          },
          ""ip"": ""192.168.2.14"",
          ""group"": [
            ""Daklak"",
            ""Test"",
            ""VSEC"",
            ""Daklak_Webs"",
            ""Daklak_Apps""
          ],
          ""manager"": ""vsec-logstash"",
          ""version"": ""Wazuh v3.11.4"",
          ""node_name"": ""node02"",
          ""registerIP"": ""any"",
          ""lastKeepAlive"": ""2020-05-24 05:44:48"",
          ""dateAdd"": ""2020-03-27 07:47:46"",
          ""name"": ""werserver"",
          ""status"": ""Active"",
          ""id"": ""060"",
          ""mergedSum"": ""f69c8a56d2cd8b401643a52cf423ad1c"",
          ""configSum"": ""4b54b212c2dd96f8b25aebfea2a104cd"",
          ""timestamp"": ""2020-05-24T05:45:02.431Z"",
          ""host"": ""vsec-logstash"",
          ""cluster"": {
            ""name"": ""wazuh""
          }
        },
        ""fields"": {
          ""timestamp"": [
            ""2020-05-24T05:45:02.431Z""
          ]
        },
        ""highlight"": {
          ""group"": [
            ""@kibana-highlighted-field@Daklak@/kibana-highlighted-field@"",
            ""@kibana-highlighted-field@Daklak_Webs@/kibana-highlighted-field@""
          ]
        },
        ""sort"": [
          -9223372036854776000
        ]
      },
      {
        ""_index"": ""wazuh-monitoring-3.x-2020.05.24"",
        ""_type"": ""_doc"",
        ""_id"": ""mwc2RXIBENNuuStZjDLl"",
        ""_version"": 1,
        ""_score"": null,
        ""_source"": {
          ""os"": {
            ""arch"": ""x86_64"",
            ""major"": ""6"",
            ""minor"": ""8"",
            ""name"": ""CentOS Linux"",
            ""platform"": ""centos"",
            ""uname"": ""Linux |webserver18 |2.6.32-642.el6.x86_64 |#1 SMP Tue May 10 17:27:01 UTC 2016 |x86_64"",
            ""version"": ""6.8""
          },
          ""ip"": ""192.168.2.18"",
          ""group"": [
            ""Daklak"",
            ""Test"",
            ""VSEC"",
            ""Daklak_Webs""
          ],
          ""manager"": ""vsec-logstash"",
          ""version"": ""Wazuh v3.11.4"",
          ""node_name"": ""node02"",
          ""registerIP"": ""any"",
          ""lastKeepAlive"": ""2020-05-24 05:44:52"",
          ""dateAdd"": ""2020-03-31 02:15:08"",
          ""name"": ""webserver18"",
          ""status"": ""Active"",
          ""id"": ""061"",
          ""mergedSum"": ""535237cb82b543ec180a59bde6425db5"",
          ""configSum"": ""d149ab60df71e6fb53b2d38db75bc733"",
          ""timestamp"": ""2020-05-24T05:45:02.431Z"",
          ""host"": ""vsec-logstash"",
          ""cluster"": {
            ""name"": ""wazuh""
          }
        },
        ""fields"": {
          ""timestamp"": [
            ""2020-05-24T05:45:02.431Z""
          ]
        },
        ""highlight"": {
          ""group"": [
            ""@kibana-highlighted-field@Daklak@/kibana-highlighted-field@"",
            ""@kibana-highlighted-field@Daklak_Webs@/kibana-highlighted-field@""
          ]
        },
        ""sort"": [
          -9223372036854776000
        ]
      },
      {
        ""_index"": ""wazuh-monitoring-3.x-2020.05.24"",
        ""_type"": ""_doc"",
        ""_id"": ""ZRiARnIBENNuuStZIRml"",
        ""_version"": 1,
        ""_score"": null,
        ""_source"": {
          ""os"": {
            ""arch"": ""x86_64"",
            ""major"": ""6"",
            ""minor"": ""8"",
            ""name"": ""CentOS Linux"",
            ""platform"": ""centos"",
            ""uname"": ""Linux |webserver18 |2.6.32-642.el6.x86_64 |#1 SMP Tue May 10 17:27:01 UTC 2016 |x86_64"",
            ""version"": ""6.8""
          },
          ""version"": ""Wazuh v3.11.4"",
          ""dateAdd"": ""2020-03-31 02:15:08"",
          ""manager"": ""vsec-logstash"",
          ""group"": [
            ""Daklak"",
            ""Test"",
            ""VSEC"",
            ""Daklak_Webs""
          ],
          ""mergedSum"": ""535237cb82b543ec180a59bde6425db5"",
          ""registerIP"": ""any"",
          ""ip"": ""192.168.2.18"",
          ""name"": ""webserver18"",
          ""configSum"": ""d149ab60df71e6fb53b2d38db75bc733"",
          ""lastKeepAlive"": ""2020-05-24 11:44:57"",
          ""id"": ""061"",
          ""status"": ""Active"",
          ""node_name"": ""node02"",
          ""timestamp"": ""2020-05-24T11:45:01.853Z"",
          ""host"": ""vsec-logstash"",
          ""cluster"": {
            ""name"": ""wazuh""
          }
        },
        ""fields"": {
          ""timestamp"": [
            ""2020-05-24T11:45:01.853Z""
          ]
        },
        ""highlight"": {
          ""group"": [
            ""@kibana-highlighted-field@Daklak@/kibana-highlighted-field@"",
            ""@kibana-highlighted-field@Daklak_Webs@/kibana-highlighted-field@""
          ]
        },
        ""sort"": [
          -9223372036854776000
        ]
      },
      {
        ""_index"": ""wazuh-monitoring-3.x-2020.05.24"",
        ""_type"": ""_doc"",
        ""_id"": ""wxq3RnIBENNuuStZEOab"",
        ""_version"": 1,
        ""_score"": null,
        ""_source"": {
          ""os"": {
            ""arch"": ""x86_64"",
            ""major"": ""6"",
            ""minor"": ""8"",
            ""name"": ""CentOS Linux"",
            ""platform"": ""centos"",
            ""uname"": ""Linux |webserver18 |2.6.32-642.el6.x86_64 |#1 SMP Tue May 10 17:27:01 UTC 2016 |x86_64"",
            ""version"": ""6.8""
          },
          ""dateAdd"": ""2020-03-31 02:15:08"",
          ""mergedSum"": ""535237cb82b543ec180a59bde6425db5"",
          ""id"": ""061"",
          ""version"": ""Wazuh v3.11.4"",
          ""status"": ""Active"",
          ""name"": ""webserver18"",
          ""configSum"": ""d149ab60df71e6fb53b2d38db75bc733"",
          ""node_name"": ""node02"",
          ""ip"": ""192.168.2.18"",
          ""group"": [
            ""Daklak"",
            ""Test"",
            ""VSEC"",
            ""Daklak_Webs""
          ],
          ""lastKeepAlive"": ""2020-05-24 12:44:52"",
          ""registerIP"": ""any"",
          ""manager"": ""vsec-logstash"",
          ""timestamp"": ""2020-05-24T12:45:01.971Z"",
          ""host"": ""vsec-logstash"",
          ""cluster"": {
            ""name"": ""wazuh""
          }
        },
        ""fields"": {
          ""timestamp"": [
            ""2020-05-24T12:45:01.971Z""
          ]
        },
        ""highlight"": {
          ""group"": [
            ""@kibana-highlighted-field@Daklak@/kibana-highlighted-field@"",
            ""@kibana-highlighted-field@Daklak_Webs@/kibana-highlighted-field@""
          ]
        },
        ""sort"": [
          -9223372036854776000
        ]
      },
      {
        ""_index"": ""wazuh-monitoring-3.x-2020.05.24"",
        ""_type"": ""_doc"",
        ""_id"": ""zRiNRnIBENNuuStZ38pt"",
        ""_version"": 1,
        ""_score"": null,
        ""_source"": {
          ""os"": {
            ""arch"": ""x86_64"",
            ""major"": ""6"",
            ""minor"": ""8"",
            ""name"": ""CentOS Linux"",
            ""platform"": ""centos"",
            ""uname"": ""Linux |webserver18 |2.6.32-642.el6.x86_64 |#1 SMP Tue May 10 17:27:01 UTC 2016 |x86_64"",
            ""version"": ""6.8""
          },
          ""mergedSum"": ""535237cb82b543ec180a59bde6425db5"",
          ""status"": ""Active"",
          ""manager"": ""vsec-logstash"",
          ""node_name"": ""node02"",
          ""configSum"": ""d149ab60df71e6fb53b2d38db75bc733"",
          ""group"": [
            ""Daklak"",
            ""Test"",
            ""VSEC"",
            ""Daklak_Webs""
          ],
          ""version"": ""Wazuh v3.11.4"",
          ""id"": ""061"",
          ""dateAdd"": ""2020-03-31 02:15:08"",
          ""lastKeepAlive"": ""2020-05-24 11:59:57"",
          ""registerIP"": ""any"",
          ""name"": ""webserver18"",
          ""ip"": ""192.168.2.18"",
          ""timestamp"": ""2020-05-24T12:00:02.410Z"",
          ""host"": ""vsec-logstash"",
          ""cluster"": {
            ""name"": ""wazuh""
          }
        },
        ""fields"": {
          ""timestamp"": [
            ""2020-05-24T12:00:02.410Z""
          ]
        },
        ""highlight"": {
          ""group"": [
            ""@kibana-highlighted-field@Daklak@/kibana-highlighted-field@"",
            ""@kibana-highlighted-field@Daklak_Webs@/kibana-highlighted-field@""
          ]
        },
        ""sort"": [
          -9223372036854776000
        ]
      },
      {
        ""_index"": ""wazuh-monitoring-3.x-2020.05.24"",
        ""_type"": ""_doc"",
        ""_id"": ""MxdyRnIBENNuuStZZ2j0"",
        ""_version"": 1,
        ""_score"": null,
        ""_source"": {
          ""os"": {
            ""arch"": ""x86_64"",
            ""major"": ""6"",
            ""minor"": ""10"",
            ""name"": ""CentOS Linux"",
            ""platform"": ""centos"",
            ""uname"": ""Linux |werserver |2.6.32-754.11.1.el6.x86_64 |#1 SMP Tue Feb 26 15:38:56 UTC 2019 |x86_64"",
            ""version"": ""6.10""
          },
          ""configSum"": ""4b54b212c2dd96f8b25aebfea2a104cd"",
          ""dateAdd"": ""2020-03-27 07:47:46"",
          ""node_name"": ""node02"",
          ""registerIP"": ""any"",
          ""status"": ""Active"",
          ""group"": [
            ""Daklak"",
            ""Test"",
            ""VSEC"",
            ""Daklak_Webs"",
            ""Daklak_Apps""
          ],
          ""manager"": ""vsec-logstash"",
          ""lastKeepAlive"": ""2020-05-24 11:29:48"",
          ""name"": ""werserver"",
          ""mergedSum"": ""f69c8a56d2cd8b401643a52cf423ad1c"",
          ""id"": ""060"",
          ""ip"": ""192.168.2.14"",
          ""version"": ""Wazuh v3.11.4"",
          ""timestamp"": ""2020-05-24T11:30:02.352Z"",
          ""host"": ""vsec-logstash"",
          ""cluster"": {
            ""name"": ""wazuh""
          }
        },
        ""fields"": {
          ""timestamp"": [
            ""2020-05-24T11:30:02.352Z""
          ]
        },
        ""highlight"": {
          ""group"": [
            ""@kibana-highlighted-field@Daklak@/kibana-highlighted-field@"",
            ""@kibana-highlighted-field@Daklak_Webs@/kibana-highlighted-field@""
          ]
        },
        ""sort"": [
          -9223372036854776000
        ]
      },
      {
        ""_index"": ""wazuh-monitoring-3.x-2020.05.24"",
        ""_type"": ""_doc"",
        ""_id"": ""NBdyRnIBENNuuStZZ2j0"",
        ""_version"": 1,
        ""_score"": null,
        ""_source"": {
          ""os"": {
            ""arch"": ""x86_64"",
            ""major"": ""6"",
            ""minor"": ""8"",
            ""name"": ""CentOS Linux"",
            ""platform"": ""centos"",
            ""uname"": ""Linux |webserver18 |2.6.32-642.el6.x86_64 |#1 SMP Tue May 10 17:27:01 UTC 2016 |x86_64"",
            ""version"": ""6.8""
          },
          ""configSum"": ""d149ab60df71e6fb53b2d38db75bc733"",
          ""dateAdd"": ""2020-03-31 02:15:08"",
          ""node_name"": ""node02"",
          ""registerIP"": ""any"",
          ""status"": ""Active"",
          ""group"": [
            ""Daklak"",
            ""Test"",
            ""VSEC"",
            ""Daklak_Webs""
          ],
          ""manager"": ""vsec-logstash"",
          ""lastKeepAlive"": ""2020-05-24 11:29:48"",
          ""name"": ""webserver18"",
          ""mergedSum"": ""535237cb82b543ec180a59bde6425db5"",
          ""id"": ""061"",
          ""ip"": ""192.168.2.18"",
          ""version"": ""Wazuh v3.11.4"",
          ""timestamp"": ""2020-05-24T11:30:02.352Z"",
          ""host"": ""vsec-logstash"",
          ""cluster"": {
            ""name"": ""wazuh""
          }
        },
        ""fields"": {
          ""timestamp"": [
            ""2020-05-24T11:30:02.352Z""
          ]
        },
        ""highlight"": {
          ""group"": [
            ""@kibana-highlighted-field@Daklak@/kibana-highlighted-field@"",
            ""@kibana-highlighted-field@Daklak_Webs@/kibana-highlighted-field@""
          ]
        },
        ""sort"": [
          -9223372036854776000
        ]
      },
      {
        ""_index"": ""wazuh-monitoring-3.x-2020.05.24"",
        ""_type"": ""_doc"",
        ""_id"": ""6h8JR3IBENNuuStZdyTq"",
        ""_version"": 1,
        ""_score"": null,
        ""_source"": {
          ""os"": {
            ""arch"": ""x86_64"",
            ""major"": ""6"",
            ""minor"": ""10"",
            ""name"": ""CentOS Linux"",
            ""platform"": ""centos"",
            ""uname"": ""Linux |werserver |2.6.32-754.11.1.el6.x86_64 |#1 SMP Tue Feb 26 15:38:56 UTC 2019 |x86_64"",
            ""version"": ""6.10""
          },
          ""ip"": ""192.168.2.14"",
          ""version"": ""Wazuh v3.11.4"",
          ""mergedSum"": ""f69c8a56d2cd8b401643a52cf423ad1c"",
          ""group"": [
            ""Daklak"",
            ""Test"",
            ""VSEC"",
            ""Daklak_Webs"",
            ""Daklak_Apps""
          ],
          ""configSum"": ""4b54b212c2dd96f8b25aebfea2a104cd"",
          ""lastKeepAlive"": ""2020-05-24 14:14:57"",
          ""node_name"": ""node02"",
          ""manager"": ""vsec-logstash"",
          ""id"": ""060"",
          ""name"": ""werserver"",
          ""dateAdd"": ""2020-03-27 07:47:46"",
          ""status"": ""Active"",
          ""registerIP"": ""any"",
          ""timestamp"": ""2020-05-24T14:15:02.372Z"",
          ""host"": ""vsec-logstash"",
          ""cluster"": {
            ""name"": ""wazuh""
          }
        },
        ""fields"": {
          ""timestamp"": [
            ""2020-05-24T14:15:02.372Z""
          ]
        },
        ""highlight"": {
          ""group"": [
            ""@kibana-highlighted-field@Daklak@/kibana-highlighted-field@"",
            ""@kibana-highlighted-field@Daklak_Webs@/kibana-highlighted-field@""
          ]
        },
        ""sort"": [
          -9223372036854776000
        ]
      },
      {
        ""_index"": ""wazuh-monitoring-3.x-2020.05.24"",
        ""_type"": ""_doc"",
        ""_id"": ""6x8JR3IBENNuuStZdyTq"",
        ""_version"": 1,
        ""_score"": null,
        ""_source"": {
          ""os"": {
            ""arch"": ""x86_64"",
            ""major"": ""6"",
            ""minor"": ""8"",
            ""name"": ""CentOS Linux"",
            ""platform"": ""centos"",
            ""uname"": ""Linux |webserver18 |2.6.32-642.el6.x86_64 |#1 SMP Tue May 10 17:27:01 UTC 2016 |x86_64"",
            ""version"": ""6.8""
          },
          ""ip"": ""192.168.2.18"",
          ""version"": ""Wazuh v3.11.4"",
          ""mergedSum"": ""535237cb82b543ec180a59bde6425db5"",
          ""group"": [
            ""Daklak"",
            ""Test"",
            ""VSEC"",
            ""Daklak_Webs""
          ],
          ""configSum"": ""d149ab60df71e6fb53b2d38db75bc733"",
          ""lastKeepAlive"": ""2020-05-24 14:14:52"",
          ""node_name"": ""node02"",
          ""manager"": ""vsec-logstash"",
          ""id"": ""061"",
          ""name"": ""webserver18"",
          ""dateAdd"": ""2020-03-31 02:15:08"",
          ""status"": ""Active"",
          ""registerIP"": ""any"",
          ""timestamp"": ""2020-05-24T14:15:02.372Z"",
          ""host"": ""vsec-logstash"",
          ""cluster"": {
            ""name"": ""wazuh""
          }
        },
        ""fields"": {
          ""timestamp"": [
            ""2020-05-24T14:15:02.372Z""
          ]
        },
        ""highlight"": {
          ""group"": [
            ""@kibana-highlighted-field@Daklak@/kibana-highlighted-field@"",
            ""@kibana-highlighted-field@Daklak_Webs@/kibana-highlighted-field@""
          ]
        },
        ""sort"": [
          -9223372036854776000
        ]
      },
      {
        ""_index"": ""wazuh-monitoring-3.x-2020.05.24"",
        ""_type"": ""_doc"",
        ""_id"": ""Rx3uRnIBENNuuStZAbqA"",
        ""_version"": 1,
        ""_score"": null,
        ""_source"": {
          ""os"": {
            ""arch"": ""x86_64"",
            ""major"": ""6"",
            ""minor"": ""10"",
            ""name"": ""CentOS Linux"",
            ""platform"": ""centos"",
            ""uname"": ""Linux |werserver |2.6.32-754.11.1.el6.x86_64 |#1 SMP Tue Feb 26 15:38:56 UTC 2019 |x86_64"",
            ""version"": ""6.10""
          },
          ""node_name"": ""node02"",
          ""mergedSum"": ""f69c8a56d2cd8b401643a52cf423ad1c"",
          ""lastKeepAlive"": ""2020-05-24 13:44:46"",
          ""id"": ""060"",
          ""name"": ""werserver"",
          ""configSum"": ""4b54b212c2dd96f8b25aebfea2a104cd"",
          ""registerIP"": ""any"",
          ""group"": [
            ""Daklak"",
            ""Test"",
            ""VSEC"",
            ""Daklak_Webs"",
            ""Daklak_Apps""
          ],
          ""status"": ""Active"",
          ""version"": ""Wazuh v3.11.4"",
          ""dateAdd"": ""2020-03-27 07:47:46"",
          ""ip"": ""192.168.2.14"",
          ""manager"": ""vsec-logstash"",
          ""timestamp"": ""2020-05-24T13:45:02.584Z"",
          ""host"": ""vsec-logstash"",
          ""cluster"": {
            ""name"": ""wazuh""
          }
        },
        ""fields"": {
          ""timestamp"": [
            ""2020-05-24T13:45:02.584Z""
          ]
        },
        ""highlight"": {
          ""group"": [
            ""@kibana-highlighted-field@Daklak@/kibana-highlighted-field@"",
            ""@kibana-highlighted-field@Daklak_Webs@/kibana-highlighted-field@""
          ]
        },
        ""sort"": [
          -9223372036854776000
        ]
      },
      {
        ""_index"": ""wazuh-monitoring-3.x-2020.05.24"",
        ""_type"": ""_doc"",
        ""_id"": ""kCEyR3IBENNuuStZqTxw"",
        ""_version"": 1,
        ""_score"": null,
        ""_source"": {
          ""os"": {
            ""arch"": ""x86_64"",
            ""major"": ""6"",
            ""minor"": ""8"",
            ""name"": ""CentOS Linux"",
            ""platform"": ""centos"",
            ""uname"": ""Linux |webserver18 |2.6.32-642.el6.x86_64 |#1 SMP Tue May 10 17:27:01 UTC 2016 |x86_64"",
            ""version"": ""6.8""
          },
          ""registerIP"": ""any"",
          ""status"": ""Active"",
          ""configSum"": ""d149ab60df71e6fb53b2d38db75bc733"",
          ""node_name"": ""node02"",
          ""mergedSum"": ""535237cb82b543ec180a59bde6425db5"",
          ""name"": ""webserver18"",
          ""id"": ""061"",
          ""dateAdd"": ""2020-03-31 02:15:08"",
          ""version"": ""Wazuh v3.11.4"",
          ""lastKeepAlive"": ""2020-05-24 14:59:51"",
          ""ip"": ""192.168.2.18"",
          ""group"": [
            ""Daklak"",
            ""Test"",
            ""VSEC"",
            ""Daklak_Webs""
          ],
          ""manager"": ""vsec-logstash"",
          ""timestamp"": ""2020-05-24T15:00:02.030Z"",
          ""host"": ""vsec-logstash"",
          ""cluster"": {
            ""name"": ""wazuh""
          }
        },
        ""fields"": {
          ""timestamp"": [
            ""2020-05-24T15:00:02.030Z""
          ]
        },
        ""highlight"": {
          ""group"": [
            ""@kibana-highlighted-field@Daklak@/kibana-highlighted-field@"",
            ""@kibana-highlighted-field@Daklak_Webs@/kibana-highlighted-field@""
          ]
        },
        ""sort"": [
          -9223372036854776000
        ]
      },
      {
        ""_index"": ""wazuh-monitoring-3.x-2020.05.24"",
        ""_type"": ""_doc"",
        ""_id"": ""vSAkR3IBENNuuStZ64p3"",
        ""_version"": 1,
        ""_score"": null,
        ""_source"": {
          ""os"": {
            ""arch"": ""x86_64"",
            ""major"": ""6"",
            ""minor"": ""10"",
            ""name"": ""CentOS Linux"",
            ""platform"": ""centos"",
            ""uname"": ""Linux |werserver |2.6.32-754.11.1.el6.x86_64 |#1 SMP Tue Feb 26 15:38:56 UTC 2019 |x86_64"",
            ""version"": ""6.10""
          },
          ""version"": ""Wazuh v3.11.4"",
          ""name"": ""werserver"",
          ""group"": [
            ""Daklak"",
            ""Test"",
            ""VSEC"",
            ""Daklak_Webs"",
            ""Daklak_Apps""
          ],
          ""manager"": ""vsec-logstash"",
          ""configSum"": ""4b54b212c2dd96f8b25aebfea2a104cd"",
          ""mergedSum"": ""f69c8a56d2cd8b401643a52cf423ad1c"",
          ""registerIP"": ""any"",
          ""lastKeepAlive"": ""2020-05-24 14:44:59"",
          ""dateAdd"": ""2020-03-27 07:47:46"",
          ""id"": ""060"",
          ""ip"": ""192.168.2.14"",
          ""node_name"": ""node02"",
          ""status"": ""Active"",
          ""timestamp"": ""2020-05-24T14:45:01.426Z"",
          ""host"": ""vsec-logstash"",
          ""cluster"": {
            ""name"": ""wazuh""
          }
        },
        ""fields"": {
          ""timestamp"": [
            ""2020-05-24T14:45:01.426Z""
          ]
        },
        ""highlight"": {
          ""group"": [
            ""@kibana-highlighted-field@Daklak@/kibana-highlighted-field@"",
            ""@kibana-highlighted-field@Daklak_Webs@/kibana-highlighted-field@""
          ]
        },
        ""sort"": [
          -9223372036854776000
        ]
      },
      {
        ""_index"": ""wazuh-monitoring-3.x-2020.05.24"",
        ""_type"": ""_doc"",
        ""_id"": ""viAkR3IBENNuuStZ64p3"",
        ""_version"": 1,
        ""_score"": null,
        ""_source"": {
          ""os"": {
            ""arch"": ""x86_64"",
            ""major"": ""6"",
            ""minor"": ""8"",
            ""name"": ""CentOS Linux"",
            ""platform"": ""centos"",
            ""uname"": ""Linux |webserver18 |2.6.32-642.el6.x86_64 |#1 SMP Tue May 10 17:27:01 UTC 2016 |x86_64"",
            ""version"": ""6.8""
          },
          ""version"": ""Wazuh v3.11.4"",
          ""name"": ""webserver18"",
          ""group"": [
            ""Daklak"",
            ""Test"",
            ""VSEC"",
            ""Daklak_Webs""
          ],
          ""manager"": ""vsec-logstash"",
          ""configSum"": ""d149ab60df71e6fb53b2d38db75bc733"",
          ""mergedSum"": ""535237cb82b543ec180a59bde6425db5"",
          ""registerIP"": ""any"",
          ""lastKeepAlive"": ""2020-05-24 14:45:00"",
          ""dateAdd"": ""2020-03-31 02:15:08"",
          ""id"": ""061"",
          ""ip"": ""192.168.2.18"",
          ""node_name"": ""node02"",
          ""status"": ""Active"",
          ""timestamp"": ""2020-05-24T14:45:01.426Z"",
          ""host"": ""vsec-logstash"",
          ""cluster"": {
            ""name"": ""wazuh""
          }
        },
        ""fields"": {
          ""timestamp"": [
            ""2020-05-24T14:45:01.426Z""
          ]
        },
        ""highlight"": {
          ""group"": [
            ""@kibana-highlighted-field@Daklak@/kibana-highlighted-field@"",
            ""@kibana-highlighted-field@Daklak_Webs@/kibana-highlighted-field@""
          ]
        },
        ""sort"": [
          -9223372036854776000
        ]
      },
      {
        ""_index"": ""wazuh-monitoring-3.x-2020.05.24"",
        ""_type"": ""_doc"",
        ""_id"": ""Mh77RnIBENNuuStZumrZ"",
        ""_version"": 1,
        ""_score"": null,
        ""_source"": {
          ""os"": {
            ""arch"": ""x86_64"",
            ""major"": ""6"",
            ""minor"": ""10"",
            ""name"": ""CentOS Linux"",
            ""platform"": ""centos"",
            ""uname"": ""Linux |werserver |2.6.32-754.11.1.el6.x86_64 |#1 SMP Tue Feb 26 15:38:56 UTC 2019 |x86_64"",
            ""version"": ""6.10""
          },
          ""manager"": ""vsec-logstash"",
          ""name"": ""werserver"",
          ""group"": [
            ""Daklak"",
            ""Test"",
            ""VSEC"",
            ""Daklak_Webs"",
            ""Daklak_Apps""
          ],
          ""mergedSum"": ""f69c8a56d2cd8b401643a52cf423ad1c"",
          ""id"": ""060"",
          ""node_name"": ""node02"",
          ""configSum"": ""4b54b212c2dd96f8b25aebfea2a104cd"",
          ""status"": ""Active"",
          ""dateAdd"": ""2020-03-27 07:47:46"",
          ""ip"": ""192.168.2.14"",
          ""version"": ""Wazuh v3.11.4"",
          ""registerIP"": ""any"",
          ""lastKeepAlive"": ""2020-05-24 13:59:46"",
          ""timestamp"": ""2020-05-24T14:00:02.003Z"",
          ""host"": ""vsec-logstash"",
          ""cluster"": {
            ""name"": ""wazuh""
          }
        },
        ""fields"": {
          ""timestamp"": [
            ""2020-05-24T14:00:02.003Z""
          ]
        },
        ""highlight"": {
          ""group"": [
            ""@kibana-highlighted-field@Daklak@/kibana-highlighted-field@"",
            ""@kibana-highlighted-field@Daklak_Webs@/kibana-highlighted-field@""
          ]
        },
        ""sort"": [
          -9223372036854776000
        ]
      },
      {
        ""_index"": ""wazuh-monitoring-3.x-2020.05.24"",
        ""_type"": ""_doc"",
        ""_id"": ""Mx77RnIBENNuuStZumrZ"",
        ""_version"": 1,
        ""_score"": null,
        ""_source"": {
          ""os"": {
            ""arch"": ""x86_64"",
            ""major"": ""6"",
            ""minor"": ""8"",
            ""name"": ""CentOS Linux"",
            ""platform"": ""centos"",
            ""uname"": ""Linux |webserver18 |2.6.32-642.el6.x86_64 |#1 SMP Tue May 10 17:27:01 UTC 2016 |x86_64"",
            ""version"": ""6.8""
          },
          ""manager"": ""vsec-logstash"",
          ""name"": ""webserver18"",
          ""group"": [
            ""Daklak"",
            ""Test"",
            ""VSEC"",
            ""Daklak_Webs""
          ],
          ""mergedSum"": ""535237cb82b543ec180a59bde6425db5"",
          ""id"": ""061"",
          ""node_name"": ""node02"",
          ""configSum"": ""d149ab60df71e6fb53b2d38db75bc733"",
          ""status"": ""Active"",
          ""dateAdd"": ""2020-03-31 02:15:08"",
          ""ip"": ""192.168.2.18"",
          ""version"": ""Wazuh v3.11.4"",
          ""registerIP"": ""any"",
          ""lastKeepAlive"": ""2020-05-24 13:59:51"",
          ""timestamp"": ""2020-05-24T14:00:02.003Z"",
          ""host"": ""vsec-logstash"",
          ""cluster"": {
            ""name"": ""wazuh""
          }
        },
        ""fields"": {
          ""timestamp"": [
            ""2020-05-24T14:00:02.003Z""
          ]
        },
        ""highlight"": {
          ""group"": [
            ""@kibana-highlighted-field@Daklak@/kibana-highlighted-field@"",
            ""@kibana-highlighted-field@Daklak_Webs@/kibana-highlighted-field@""
          ]
        },
        ""sort"": [
          -9223372036854776000
        ]
      },
      {
        ""_index"": ""wazuh-monitoring-3.x-2020.05.24"",
        ""_type"": ""_doc"",
        ""_id"": ""WCJOR3IBENNuuStZIKiy"",
        ""_version"": 1,
        ""_score"": null,
        ""_source"": {
          ""os"": {
            ""arch"": ""x86_64"",
            ""major"": ""6"",
            ""minor"": ""8"",
            ""name"": ""CentOS Linux"",
            ""platform"": ""centos"",
            ""uname"": ""Linux |webserver18 |2.6.32-642.el6.x86_64 |#1 SMP Tue May 10 17:27:01 UTC 2016 |x86_64"",
            ""version"": ""6.8""
          },
          ""dateAdd"": ""2020-03-31 02:15:08"",
          ""id"": ""061"",
          ""status"": ""Active"",
          ""manager"": ""vsec-logstash"",
          ""version"": ""Wazuh v3.11.4"",
          ""mergedSum"": ""535237cb82b543ec180a59bde6425db5"",
          ""lastKeepAlive"": ""2020-05-24 15:29:51"",
          ""group"": [
            ""Daklak"",
            ""Test"",
            ""VSEC"",
            ""Daklak_Webs""
          ],
          ""name"": ""webserver18"",
          ""ip"": ""192.168.2.18"",
          ""registerIP"": ""any"",
          ""node_name"": ""node02"",
          ""configSum"": ""d149ab60df71e6fb53b2d38db75bc733"",
          ""timestamp"": ""2020-05-24T15:30:02.036Z"",
          ""host"": ""vsec-logstash"",
          ""cluster"": {
            ""name"": ""wazuh""
          }
        },
        ""fields"": {
          ""timestamp"": [
            ""2020-05-24T15:30:02.036Z""
          ]
        },
        ""highlight"": {
          ""group"": [
            ""@kibana-highlighted-field@Daklak@/kibana-highlighted-field@"",
            ""@kibana-highlighted-field@Daklak_Webs@/kibana-highlighted-field@""
          ]
        },
        ""sort"": [
          -9223372036854776000
        ]
      },
      {
        ""_index"": ""wazuh-monitoring-3.x-2020.05.24"",
        ""_type"": ""_doc"",
        ""_id"": ""ZSNbR3IBENNuuStZ3Vuh"",
        ""_version"": 1,
        ""_score"": null,
        ""_source"": {
          ""os"": {
            ""arch"": ""x86_64"",
            ""major"": ""6"",
            ""minor"": ""8"",
            ""name"": ""CentOS Linux"",
            ""platform"": ""centos"",
            ""uname"": ""Linux |webserver18 |2.6.32-642.el6.x86_64 |#1 SMP Tue May 10 17:27:01 UTC 2016 |x86_64"",
            ""version"": ""6.8""
          },
          ""dateAdd"": ""2020-03-31 02:15:08"",
          ""status"": ""Active"",
          ""ip"": ""192.168.2.18"",
          ""lastKeepAlive"": ""2020-05-24 15:44:50"",
          ""version"": ""Wazuh v3.11.4"",
          ""node_name"": ""node02"",
          ""registerIP"": ""any"",
          ""configSum"": ""d149ab60df71e6fb53b2d38db75bc733"",
          ""manager"": ""vsec-logstash"",
          ""mergedSum"": ""535237cb82b543ec180a59bde6425db5"",
          ""id"": ""061"",
          ""name"": ""webserver18"",
          ""group"": [
            ""Daklak"",
            ""Test"",
            ""VSEC"",
            ""Daklak_Webs""
          ],
          ""timestamp"": ""2020-05-24T15:45:02.367Z"",
          ""host"": ""vsec-logstash"",
          ""cluster"": {
            ""name"": ""wazuh""
          }
        },
        ""fields"": {
          ""timestamp"": [
            ""2020-05-24T15:45:02.367Z""
          ]
        },
        ""highlight"": {
          ""group"": [
            ""@kibana-highlighted-field@Daklak@/kibana-highlighted-field@"",
            ""@kibana-highlighted-field@Daklak_Webs@/kibana-highlighted-field@""
          ]
        },
        ""sort"": [
          -9223372036854776000
        ]
      },
      {
        ""_index"": ""wazuh-monitoring-3.x-2020.05.24"",
        ""_type"": ""_doc"",
        ""_id"": ""pzFuSHIBENNuuStZhmpn"",
        ""_version"": 1,
        ""_score"": null,
        ""_source"": {
          ""os"": {
            ""arch"": ""x86_64"",
            ""major"": ""6"",
            ""minor"": ""8"",
            ""name"": ""CentOS Linux"",
            ""platform"": ""centos"",
            ""uname"": ""Linux |webserver18 |2.6.32-642.el6.x86_64 |#1 SMP Tue May 10 17:27:01 UTC 2016 |x86_64"",
            ""version"": ""6.8""
          },
          ""id"": ""061"",
          ""manager"": ""vsec-logstash"",
          ""mergedSum"": ""535237cb82b543ec180a59bde6425db5"",
          ""registerIP"": ""any"",
          ""configSum"": ""d149ab60df71e6fb53b2d38db75bc733"",
          ""name"": ""webserver18"",
          ""lastKeepAlive"": ""2020-05-24 20:44:53"",
          ""node_name"": ""node02"",
          ""version"": ""Wazuh v3.11.4"",
          ""ip"": ""192.168.2.18"",
          ""group"": [
            ""Daklak"",
            ""Test"",
            ""VSEC"",
            ""Daklak_Webs""
          ],
          ""status"": ""Active"",
          ""dateAdd"": ""2020-03-31 02:15:08"",
          ""timestamp"": ""2020-05-24T20:45:02.437Z"",
          ""host"": ""vsec-logstash"",
          ""cluster"": {
            ""name"": ""wazuh""
          }
        },
        ""fields"": {
          ""timestamp"": [
            ""2020-05-24T20:45:02.437Z""
          ]
        },
        ""highlight"": {
          ""group"": [
            ""@kibana-highlighted-field@Daklak@/kibana-highlighted-field@"",
            ""@kibana-highlighted-field@Daklak_Webs@/kibana-highlighted-field@""
          ]
        },
        ""sort"": [
          -9223372036854776000
        ]
      },
      {
        ""_index"": ""wazuh-monitoring-3.x-2020.05.24"",
        ""_type"": ""_doc"",
        ""_id"": ""HzSzSHIBENNuuStZLuoT"",
        ""_version"": 1,
        ""_score"": null,
        ""_source"": {
          ""os"": {
            ""arch"": ""x86_64"",
            ""major"": ""6"",
            ""minor"": ""10"",
            ""name"": ""CentOS Linux"",
            ""platform"": ""centos"",
            ""uname"": ""Linux |werserver |2.6.32-754.11.1.el6.x86_64 |#1 SMP Tue Feb 26 15:38:56 UTC 2019 |x86_64"",
            ""version"": ""6.10""
          },
          ""lastKeepAlive"": ""2020-05-24 21:59:50"",
          ""manager"": ""vsec-logstash"",
          ""name"": ""werserver"",
          ""dateAdd"": ""2020-03-27 07:47:46"",
          ""mergedSum"": ""f69c8a56d2cd8b401643a52cf423ad1c"",
          ""group"": [
            ""Daklak"",
            ""Test"",
            ""VSEC"",
            ""Daklak_Webs"",
            ""Daklak_Apps""
          ],
          ""configSum"": ""4b54b212c2dd96f8b25aebfea2a104cd"",
          ""ip"": ""192.168.2.14"",
          ""node_name"": ""node02"",
          ""status"": ""Active"",
          ""registerIP"": ""any"",
          ""id"": ""060"",
          ""version"": ""Wazuh v3.11.4"",
          ""timestamp"": ""2020-05-24T22:00:01.812Z"",
          ""host"": ""vsec-logstash"",
          ""cluster"": {
            ""name"": ""wazuh""
          }
        },
        ""fields"": {
          ""timestamp"": [
            ""2020-05-24T22:00:01.812Z""
          ]
        },
        ""highlight"": {
          ""group"": [
            ""@kibana-highlighted-field@Daklak@/kibana-highlighted-field@"",
            ""@kibana-highlighted-field@Daklak_Webs@/kibana-highlighted-field@""
          ]
        },
        ""sort"": [
          -9223372036854776000
        ]
      },
      {
        ""_index"": ""wazuh-monitoring-3.x-2020.05.24"",
        ""_type"": ""_doc"",
        ""_id"": ""IDSzSHIBENNuuStZLuoT"",
        ""_version"": 1,
        ""_score"": null,
        ""_source"": {
          ""os"": {
            ""arch"": ""x86_64"",
            ""major"": ""6"",
            ""minor"": ""8"",
            ""name"": ""CentOS Linux"",
            ""platform"": ""centos"",
            ""uname"": ""Linux |webserver18 |2.6.32-642.el6.x86_64 |#1 SMP Tue May 10 17:27:01 UTC 2016 |x86_64"",
            ""version"": ""6.8""
          },
          ""lastKeepAlive"": ""2020-05-24 21:59:53"",
          ""manager"": ""vsec-logstash"",
          ""name"": ""webserver18"",
          ""dateAdd"": ""2020-03-31 02:15:08"",
          ""mergedSum"": ""535237cb82b543ec180a59bde6425db5"",
          ""group"": [
            ""Daklak"",
            ""Test"",
            ""VSEC"",
            ""Daklak_Webs""
          ],
          ""configSum"": ""d149ab60df71e6fb53b2d38db75bc733"",
          ""ip"": ""192.168.2.18"",
          ""node_name"": ""node02"",
          ""status"": ""Active"",
          ""registerIP"": ""any"",
          ""id"": ""061"",
          ""version"": ""Wazuh v3.11.4"",
          ""timestamp"": ""2020-05-24T22:00:01.812Z"",
          ""host"": ""vsec-logstash"",
          ""cluster"": {
            ""name"": ""wazuh""
          }
        },
        ""fields"": {
          ""timestamp"": [
            ""2020-05-24T22:00:01.812Z""
          ]
        },
        ""highlight"": {
          ""group"": [
            ""@kibana-highlighted-field@Daklak@/kibana-highlighted-field@"",
            ""@kibana-highlighted-field@Daklak_Webs@/kibana-highlighted-field@""
          ]
        },
        ""sort"": [
          -9223372036854776000
        ]
      },
      {
        ""_index"": ""wazuh-monitoring-3.x-2020.05.24"",
        ""_type"": ""_doc"",
        ""_id"": ""UzBgSHIBENNuuStZyLe_"",
        ""_version"": 1,
        ""_score"": null,
        ""_source"": {
          ""os"": {
            ""arch"": ""x86_64"",
            ""major"": ""6"",
            ""minor"": ""8"",
            ""name"": ""CentOS Linux"",
            ""platform"": ""centos"",
            ""uname"": ""Linux |webserver18 |2.6.32-642.el6.x86_64 |#1 SMP Tue May 10 17:27:01 UTC 2016 |x86_64"",
            ""version"": ""6.8""
          },
          ""configSum"": ""d149ab60df71e6fb53b2d38db75bc733"",
          ""registerIP"": ""any"",
          ""node_name"": ""node02"",
          ""name"": ""webserver18"",
          ""status"": ""Active"",
          ""id"": ""061"",
          ""dateAdd"": ""2020-03-31 02:15:08"",
          ""version"": ""Wazuh v3.11.4"",
          ""group"": [
            ""Daklak"",
            ""Test"",
            ""VSEC"",
            ""Daklak_Webs""
          ],
          ""ip"": ""192.168.2.18"",
          ""mergedSum"": ""535237cb82b543ec180a59bde6425db5"",
          ""lastKeepAlive"": ""2020-05-24 20:29:53"",
          ""manager"": ""vsec-logstash"",
          ""timestamp"": ""2020-05-24T20:30:01.920Z"",
          ""host"": ""vsec-logstash"",
          ""cluster"": {
            ""name"": ""wazuh""
          }
        },
        ""fields"": {
          ""timestamp"": [
            ""2020-05-24T20:30:01.920Z""
          ]
        },
        ""highlight"": {
          ""group"": [
            ""@kibana-highlighted-field@Daklak@/kibana-highlighted-field@"",
            ""@kibana-highlighted-field@Daklak_Webs@/kibana-highlighted-field@""
          ]
        },
        ""sort"": [
          -9223372036854776000
        ]
      },
      {
        ""_index"": ""wazuh-monitoring-3.x-2020.05.24"",
        ""_type"": ""_doc"",
        ""_id"": ""BzSlSHIBENNuuStZdDkt"",
        ""_version"": 1,
        ""_score"": null,
        ""_source"": {
          ""os"": {
            ""arch"": ""x86_64"",
            ""major"": ""6"",
            ""minor"": ""10"",
            ""name"": ""CentOS Linux"",
            ""platform"": ""centos"",
            ""uname"": ""Linux |werserver |2.6.32-754.11.1.el6.x86_64 |#1 SMP Tue Feb 26 15:38:56 UTC 2019 |x86_64"",
            ""version"": ""6.10""
          },
          ""ip"": ""192.168.2.14"",
          ""registerIP"": ""any"",
          ""configSum"": ""4b54b212c2dd96f8b25aebfea2a104cd"",
          ""lastKeepAlive"": ""2020-05-24 21:44:51"",
          ""id"": ""060"",
          ""status"": ""Active"",
          ""mergedSum"": ""f69c8a56d2cd8b401643a52cf423ad1c"",
          ""manager"": ""vsec-logstash"",
          ""version"": ""Wazuh v3.11.4"",
          ""group"": [
            ""Daklak"",
            ""Test"",
            ""VSEC"",
            ""Daklak_Webs"",
            ""Daklak_Apps""
          ],
          ""name"": ""werserver"",
          ""dateAdd"": ""2020-03-27 07:47:46"",
          ""node_name"": ""node02"",
          ""timestamp"": ""2020-05-24T21:45:02.249Z"",
          ""host"": ""vsec-logstash"",
          ""cluster"": {
            ""name"": ""wazuh""
          }
        },
        ""fields"": {
          ""timestamp"": [
            ""2020-05-24T21:45:02.249Z""
          ]
        },
        ""highlight"": {
          ""group"": [
            ""@kibana-highlighted-field@Daklak@/kibana-highlighted-field@"",
            ""@kibana-highlighted-field@Daklak_Webs@/kibana-highlighted-field@""
          ]
        },
        ""sort"": [
          -9223372036854776000
        ]
      },
      {
        ""_index"": ""wazuh-monitoring-3.x-2020.05.24"",
        ""_type"": ""_doc"",
        ""_id"": ""gTbOSHIBENNuuStZpFWl"",
        ""_version"": 1,
        ""_score"": null,
        ""_source"": {
          ""os"": {
            ""arch"": ""x86_64"",
            ""major"": ""6"",
            ""minor"": ""10"",
            ""name"": ""CentOS Linux"",
            ""platform"": ""centos"",
            ""uname"": ""Linux |werserver |2.6.32-754.11.1.el6.x86_64 |#1 SMP Tue Feb 26 15:38:56 UTC 2019 |x86_64"",
            ""version"": ""6.10""
          },
          ""lastKeepAlive"": ""2020-05-24 22:29:50"",
          ""group"": [
            ""Daklak"",
            ""Test"",
            ""VSEC"",
            ""Daklak_Webs"",
            ""Daklak_Apps""
          ],
          ""status"": ""Active"",
          ""manager"": ""vsec-logstash"",
          ""name"": ""werserver"",
          ""registerIP"": ""any"",
          ""configSum"": ""4b54b212c2dd96f8b25aebfea2a104cd"",
          ""id"": ""060"",
          ""mergedSum"": ""f69c8a56d2cd8b401643a52cf423ad1c"",
          ""version"": ""Wazuh v3.11.4"",
          ""dateAdd"": ""2020-03-27 07:47:46"",
          ""node_name"": ""node02"",
          ""ip"": ""192.168.2.14"",
          ""timestamp"": ""2020-05-24T22:30:01.637Z"",
          ""host"": ""vsec-logstash"",
          ""cluster"": {
            ""name"": ""wazuh""
          }
        },
        ""fields"": {
          ""timestamp"": [
            ""2020-05-24T22:30:01.637Z""
          ]
        },
        ""highlight"": {
          ""group"": [
            ""@kibana-highlighted-field@Daklak@/kibana-highlighted-field@"",
            ""@kibana-highlighted-field@Daklak_Webs@/kibana-highlighted-field@""
          ]
        },
        ""sort"": [
          -9223372036854776000
        ]
      },
      {
        ""_index"": ""wazuh-monitoring-3.x-2020.05.24"",
        ""_type"": ""_doc"",
        ""_id"": ""gjbOSHIBENNuuStZpFWl"",
        ""_version"": 1,
        ""_score"": null,
        ""_source"": {
          ""os"": {
            ""arch"": ""x86_64"",
            ""major"": ""6"",
            ""minor"": ""8"",
            ""name"": ""CentOS Linux"",
            ""platform"": ""centos"",
            ""uname"": ""Linux |webserver18 |2.6.32-642.el6.x86_64 |#1 SMP Tue May 10 17:27:01 UTC 2016 |x86_64"",
            ""version"": ""6.8""
          },
          ""lastKeepAlive"": ""2020-05-24 22:29:45"",
          ""group"": [
            ""Daklak"",
            ""Test"",
            ""VSEC"",
            ""Daklak_Webs""
          ],
          ""status"": ""Active"",
          ""manager"": ""vsec-logstash"",
          ""name"": ""webserver18"",
          ""registerIP"": ""any"",
          ""configSum"": ""d149ab60df71e6fb53b2d38db75bc733"",
          ""id"": ""061"",
          ""mergedSum"": ""535237cb82b543ec180a59bde6425db5"",
          ""version"": ""Wazuh v3.11.4"",
          ""dateAdd"": ""2020-03-31 02:15:08"",
          ""node_name"": ""node02"",
          ""ip"": ""192.168.2.18"",
          ""timestamp"": ""2020-05-24T22:30:01.637Z"",
          ""host"": ""vsec-logstash"",
          ""cluster"": {
            ""name"": ""wazuh""
          }
        },
        ""fields"": {
          ""timestamp"": [
            ""2020-05-24T22:30:01.637Z""
          ]
        },
        ""highlight"": {
          ""group"": [
            ""@kibana-highlighted-field@Daklak@/kibana-highlighted-field@"",
            ""@kibana-highlighted-field@Daklak_Webs@/kibana-highlighted-field@""
          ]
        },
        ""sort"": [
          -9223372036854776000
        ]
      },
      {
        ""_index"": ""wazuh-monitoring-3.x-2020.05.24"",
        ""_type"": ""_doc"",
        ""_id"": ""KzOXSHIBENNuuStZt4gD"",
        ""_version"": 1,
        ""_score"": null,
        ""_source"": {
          ""os"": {
            ""arch"": ""x86_64"",
            ""major"": ""6"",
            ""minor"": ""10"",
            ""name"": ""CentOS Linux"",
            ""platform"": ""centos"",
            ""uname"": ""Linux |werserver |2.6.32-754.11.1.el6.x86_64 |#1 SMP Tue Feb 26 15:38:56 UTC 2019 |x86_64"",
            ""version"": ""6.10""
          },
          ""manager"": ""vsec-logstash"",
          ""ip"": ""192.168.2.14"",
          ""dateAdd"": ""2020-03-27 07:47:46"",
          ""status"": ""Active"",
          ""version"": ""Wazuh v3.11.4"",
          ""registerIP"": ""any"",
          ""name"": ""werserver"",
          ""configSum"": ""4b54b212c2dd96f8b25aebfea2a104cd"",
          ""lastKeepAlive"": ""2020-05-24 21:29:52"",
          ""node_name"": ""node02"",
          ""group"": [
            ""Daklak"",
            ""Test"",
            ""VSEC"",
            ""Daklak_Webs"",
            ""Daklak_Apps""
          ],
          ""mergedSum"": ""f69c8a56d2cd8b401643a52cf423ad1c"",
          ""id"": ""060"",
          ""timestamp"": ""2020-05-24T21:30:01.854Z"",
          ""host"": ""vsec-logstash"",
          ""cluster"": {
            ""name"": ""wazuh""
          }
        },
        ""fields"": {
          ""timestamp"": [
            ""2020-05-24T21:30:01.854Z""
          ]
        },
        ""highlight"": {
          ""group"": [
            ""@kibana-highlighted-field@Daklak@/kibana-highlighted-field@"",
            ""@kibana-highlighted-field@Daklak_Webs@/kibana-highlighted-field@""
          ]
        },
        ""sort"": [
          -9223372036854776000
        ]
      },
      {
        ""_index"": ""wazuh-monitoring-3.x-2020.05.24"",
        ""_type"": ""_doc"",
        ""_id"": ""LDOXSHIBENNuuStZt4gD"",
        ""_version"": 1,
        ""_score"": null,
        ""_source"": {
          ""os"": {
            ""arch"": ""x86_64"",
            ""major"": ""6"",
            ""minor"": ""8"",
            ""name"": ""CentOS Linux"",
            ""platform"": ""centos"",
            ""uname"": ""Linux |webserver18 |2.6.32-642.el6.x86_64 |#1 SMP Tue May 10 17:27:01 UTC 2016 |x86_64"",
            ""version"": ""6.8""
          },
          ""manager"": ""vsec-logstash"",
          ""ip"": ""192.168.2.18"",
          ""dateAdd"": ""2020-03-31 02:15:08"",
          ""status"": ""Active"",
          ""version"": ""Wazuh v3.11.4"",
          ""registerIP"": ""any"",
          ""name"": ""webserver18"",
          ""configSum"": ""d149ab60df71e6fb53b2d38db75bc733"",
          ""lastKeepAlive"": ""2020-05-24 21:29:53"",
          ""node_name"": ""node02"",
          ""group"": [
            ""Daklak"",
            ""Test"",
            ""VSEC"",
            ""Daklak_Webs""
          ],
          ""mergedSum"": ""535237cb82b543ec180a59bde6425db5"",
          ""id"": ""061"",
          ""timestamp"": ""2020-05-24T21:30:01.854Z"",
          ""host"": ""vsec-logstash"",
          ""cluster"": {
            ""name"": ""wazuh""
          }
        },
        ""fields"": {
          ""timestamp"": [
            ""2020-05-24T21:30:01.854Z""
          ]
        },
        ""highlight"": {
          ""group"": [
            ""@kibana-highlighted-field@Daklak@/kibana-highlighted-field@"",
            ""@kibana-highlighted-field@Daklak_Webs@/kibana-highlighted-field@""
          ]
        },
        ""sort"": [
          -9223372036854776000
        ]
      },
      {
        ""_index"": ""wazuh-monitoring-3.x-2020.05.24"",
        ""_type"": ""_doc"",
        ""_id"": ""2DXASHIBENNuuStZ66VQ"",
        ""_version"": 1,
        ""_score"": null,
        ""_source"": {
          ""os"": {
            ""arch"": ""x86_64"",
            ""major"": ""6"",
            ""minor"": ""10"",
            ""name"": ""CentOS Linux"",
            ""platform"": ""centos"",
            ""uname"": ""Linux |werserver |2.6.32-754.11.1.el6.x86_64 |#1 SMP Tue Feb 26 15:38:56 UTC 2019 |x86_64"",
            ""version"": ""6.10""
          },
          ""version"": ""Wazuh v3.11.4"",
          ""group"": [
            ""Daklak"",
            ""Test"",
            ""VSEC"",
            ""Daklak_Webs"",
            ""Daklak_Apps""
          ],
          ""id"": ""060"",
          ""ip"": ""192.168.2.14"",
          ""mergedSum"": ""f69c8a56d2cd8b401643a52cf423ad1c"",
          ""name"": ""werserver"",
          ""manager"": ""vsec-logstash"",
          ""dateAdd"": ""2020-03-27 07:47:46"",
          ""lastKeepAlive"": ""2020-05-24 22:15:00"",
          ""status"": ""Active"",
          ""registerIP"": ""any"",
          ""node_name"": ""node02"",
          ""configSum"": ""4b54b212c2dd96f8b25aebfea2a104cd"",
          ""timestamp"": ""2020-05-24T22:15:02.228Z"",
          ""host"": ""vsec-logstash"",
          ""cluster"": {
            ""name"": ""wazuh""
          }
        },
        ""fields"": {
          ""timestamp"": [
            ""2020-05-24T22:15:02.228Z""
          ]
        },
        ""highlight"": {
          ""group"": [
            ""@kibana-highlighted-field@Daklak@/kibana-highlighted-field@"",
            ""@kibana-highlighted-field@Daklak_Webs@/kibana-highlighted-field@""
          ]
        },
        ""sort"": [
          -9223372036854776000
        ]
      },
      {
        ""_index"": ""wazuh-monitoring-3.x-2020.05.24"",
        ""_type"": ""_doc"",
        ""_id"": ""2TXASHIBENNuuStZ66VQ"",
        ""_version"": 1,
        ""_score"": null,
        ""_source"": {
          ""os"": {
            ""arch"": ""x86_64"",
            ""major"": ""6"",
            ""minor"": ""8"",
            ""name"": ""CentOS Linux"",
            ""platform"": ""centos"",
            ""uname"": ""Linux |webserver18 |2.6.32-642.el6.x86_64 |#1 SMP Tue May 10 17:27:01 UTC 2016 |x86_64"",
            ""version"": ""6.8""
          },
          ""version"": ""Wazuh v3.11.4"",
          ""group"": [
            ""Daklak"",
            ""Test"",
            ""VSEC"",
            ""Daklak_Webs""
          ],
          ""id"": ""061"",
          ""ip"": ""192.168.2.18"",
          ""mergedSum"": ""535237cb82b543ec180a59bde6425db5"",
          ""name"": ""webserver18"",
          ""manager"": ""vsec-logstash"",
          ""dateAdd"": ""2020-03-31 02:15:08"",
          ""lastKeepAlive"": ""2020-05-24 22:14:54"",
          ""status"": ""Active"",
          ""registerIP"": ""any"",
          ""node_name"": ""node02"",
          ""configSum"": ""d149ab60df71e6fb53b2d38db75bc733"",
          ""timestamp"": ""2020-05-24T22:15:02.228Z"",
          ""host"": ""vsec-logstash"",
          ""cluster"": {
            ""name"": ""wazuh""
          }
        },
        ""fields"": {
          ""timestamp"": [
            ""2020-05-24T22:15:02.228Z""
          ]
        },
        ""highlight"": {
          ""group"": [
            ""@kibana-highlighted-field@Daklak@/kibana-highlighted-field@"",
            ""@kibana-highlighted-field@Daklak_Webs@/kibana-highlighted-field@""
          ]
        },
        ""sort"": [
          -9223372036854776000
        ]
      },
      {
        ""_index"": ""wazuh-monitoring-3.x-2020.05.24"",
        ""_type"": ""_doc"",
        ""_id"": ""JDfcSHIBENNuuStZYgdh"",
        ""_version"": 1,
        ""_score"": null,
        ""_source"": {
          ""os"": {
            ""arch"": ""x86_64"",
            ""major"": ""6"",
            ""minor"": ""10"",
            ""name"": ""CentOS Linux"",
            ""platform"": ""centos"",
            ""uname"": ""Linux |werserver |2.6.32-754.11.1.el6.x86_64 |#1 SMP Tue Feb 26 15:38:56 UTC 2019 |x86_64"",
            ""version"": ""6.10""
          },
          ""node_name"": ""node02"",
          ""mergedSum"": ""f69c8a56d2cd8b401643a52cf423ad1c"",
          ""manager"": ""vsec-logstash"",
          ""group"": [
            ""Daklak"",
            ""Test"",
            ""VSEC"",
            ""Daklak_Webs"",
            ""Daklak_Apps""
          ],
          ""ip"": ""192.168.2.14"",
          ""registerIP"": ""any"",
          ""configSum"": ""4b54b212c2dd96f8b25aebfea2a104cd"",
          ""lastKeepAlive"": ""2020-05-24 22:44:50"",
          ""id"": ""060"",
          ""name"": ""werserver"",
          ""dateAdd"": ""2020-03-27 07:47:46"",
          ""status"": ""Active"",
          ""version"": ""Wazuh v3.11.4"",
          ""timestamp"": ""2020-05-24T22:45:02.175Z"",
          ""host"": ""vsec-logstash"",
          ""cluster"": {
            ""name"": ""wazuh""
          }
        },
        ""fields"": {
          ""timestamp"": [
            ""2020-05-24T22:45:02.175Z""
          ]
        },
        ""highlight"": {
          ""group"": [
            ""@kibana-highlighted-field@Daklak@/kibana-highlighted-field@"",
            ""@kibana-highlighted-field@Daklak_Webs@/kibana-highlighted-field@""
          ]
        },
        ""sort"": [
          -9223372036854776000
        ]
      },
      {
        ""_index"": ""wazuh-monitoring-3.x-2020.05.24"",
        ""_type"": ""_doc"",
        ""_id"": ""8TfqSHIBENNuuStZG7fS"",
        ""_version"": 1,
        ""_score"": null,
        ""_source"": {
          ""os"": {
            ""arch"": ""x86_64"",
            ""major"": ""6"",
            ""minor"": ""8"",
            ""name"": ""CentOS Linux"",
            ""platform"": ""centos"",
            ""uname"": ""Linux |webserver18 |2.6.32-642.el6.x86_64 |#1 SMP Tue May 10 17:27:01 UTC 2016 |x86_64"",
            ""version"": ""6.8""
          },
          ""node_name"": ""node02"",
          ""configSum"": ""d149ab60df71e6fb53b2d38db75bc733"",
          ""group"": [
            ""Daklak"",
            ""Test"",
            ""VSEC"",
            ""Daklak_Webs""
          ],
          ""mergedSum"": ""535237cb82b543ec180a59bde6425db5"",
          ""lastKeepAlive"": ""2020-05-24 22:59:46"",
          ""status"": ""Active"",
          ""ip"": ""192.168.2.18"",
          ""version"": ""Wazuh v3.11.4"",
          ""name"": ""webserver18"",
          ""manager"": ""vsec-logstash"",
          ""id"": ""061"",
          ""dateAdd"": ""2020-03-31 02:15:08"",
          ""registerIP"": ""any"",
          ""timestamp"": ""2020-05-24T23:00:01.616Z"",
          ""host"": ""vsec-logstash"",
          ""cluster"": {
            ""name"": ""wazuh""
          }
        },
        ""fields"": {
          ""timestamp"": [
            ""2020-05-24T23:00:01.616Z""
          ]
        },
        ""highlight"": {
          ""group"": [
            ""@kibana-highlighted-field@Daklak@/kibana-highlighted-field@"",
            ""@kibana-highlighted-field@Daklak_Webs@/kibana-highlighted-field@""
          ]
        },
        ""sort"": [
          -9223372036854776000
        ]
      },
      {
        ""_index"": ""wazuh-monitoring-3.x-2020.05.24"",
        ""_type"": ""_doc"",
        ""_id"": ""hzj3SHIBENNuuStZ2XGo"",
        ""_version"": 1,
        ""_score"": null,
        ""_source"": {
          ""os"": {
            ""arch"": ""x86_64"",
            ""major"": ""6"",
            ""minor"": ""10"",
            ""name"": ""CentOS Linux"",
            ""platform"": ""centos"",
            ""uname"": ""Linux |werserver |2.6.32-754.11.1.el6.x86_64 |#1 SMP Tue Feb 26 15:38:56 UTC 2019 |x86_64"",
            ""version"": ""6.10""
          },
          ""name"": ""werserver"",
          ""status"": ""Active"",
          ""configSum"": ""4b54b212c2dd96f8b25aebfea2a104cd"",
          ""mergedSum"": ""f69c8a56d2cd8b401643a52cf423ad1c"",
          ""id"": ""060"",
          ""version"": ""Wazuh v3.11.4"",
          ""lastKeepAlive"": ""2020-05-24 23:14:51"",
          ""registerIP"": ""any"",
          ""ip"": ""192.168.2.14"",
          ""group"": [
            ""Daklak"",
            ""Test"",
            ""VSEC"",
            ""Daklak_Webs"",
            ""Daklak_Apps""
          ],
          ""node_name"": ""node02"",
          ""manager"": ""vsec-logstash"",
          ""dateAdd"": ""2020-03-27 07:47:46"",
          ""timestamp"": ""2020-05-24T23:15:02.183Z"",
          ""host"": ""vsec-logstash"",
          ""cluster"": {
            ""name"": ""wazuh""
          }
        },
        ""fields"": {
          ""timestamp"": [
            ""2020-05-24T23:15:02.183Z""
          ]
        },
        ""highlight"": {
          ""group"": [
            ""@kibana-highlighted-field@Daklak@/kibana-highlighted-field@"",
            ""@kibana-highlighted-field@Daklak_Webs@/kibana-highlighted-field@""
          ]
        },
        ""sort"": [
          -9223372036854776000
        ]
      },
      {
        ""_index"": ""wazuh-monitoring-3.x-2020.05.24"",
        ""_type"": ""_doc"",
        ""_id"": ""iDj3SHIBENNuuStZ2XGo"",
        ""_version"": 1,
        ""_score"": null,
        ""_source"": {
          ""os"": {
            ""arch"": ""x86_64"",
            ""major"": ""6"",
            ""minor"": ""8"",
            ""name"": ""CentOS Linux"",
            ""platform"": ""centos"",
            ""uname"": ""Linux |webserver18 |2.6.32-642.el6.x86_64 |#1 SMP Tue May 10 17:27:01 UTC 2016 |x86_64"",
            ""version"": ""6.8""
          },
          ""name"": ""webserver18"",
          ""status"": ""Active"",
          ""configSum"": ""d149ab60df71e6fb53b2d38db75bc733"",
          ""mergedSum"": ""535237cb82b543ec180a59bde6425db5"",
          ""id"": ""061"",
          ""version"": ""Wazuh v3.11.4"",
          ""lastKeepAlive"": ""2020-05-24 23:14:56"",
          ""registerIP"": ""any"",
          ""ip"": ""192.168.2.18"",
          ""group"": [
            ""Daklak"",
            ""Test"",
            ""VSEC"",
            ""Daklak_Webs""
          ],
          ""node_name"": ""node02"",
          ""manager"": ""vsec-logstash"",
          ""dateAdd"": ""2020-03-31 02:15:08"",
          ""timestamp"": ""2020-05-24T23:15:02.183Z"",
          ""host"": ""vsec-logstash"",
          ""cluster"": {
            ""name"": ""wazuh""
          }
        },
        ""fields"": {
          ""timestamp"": [
            ""2020-05-24T23:15:02.183Z""
          ]
        },
        ""highlight"": {
          ""group"": [
            ""@kibana-highlighted-field@Daklak@/kibana-highlighted-field@"",
            ""@kibana-highlighted-field@Daklak_Webs@/kibana-highlighted-field@""
          ]
        },
        ""sort"": [
          -9223372036854776000
        ]
      },
      {
        ""_index"": ""wazuh-monitoring-3.x-2020.05.24"",
        ""_type"": ""_doc"",
        ""_id"": ""AzkTSXIBENNuuStZUdNJ"",
        ""_version"": 1,
        ""_score"": null,
        ""_source"": {
          ""os"": {
            ""arch"": ""x86_64"",
            ""major"": ""6"",
            ""minor"": ""10"",
            ""name"": ""CentOS Linux"",
            ""platform"": ""centos"",
            ""uname"": ""Linux |werserver |2.6.32-754.11.1.el6.x86_64 |#1 SMP Tue Feb 26 15:38:56 UTC 2019 |x86_64"",
            ""version"": ""6.10""
          },
          ""id"": ""060"",
          ""ip"": ""192.168.2.14"",
          ""dateAdd"": ""2020-03-27 07:47:46"",
          ""name"": ""werserver"",
          ""lastKeepAlive"": ""2020-05-24 23:44:59"",
          ""status"": ""Active"",
          ""manager"": ""vsec-logstash"",
          ""version"": ""Wazuh v3.11.4"",
          ""node_name"": ""node02"",
          ""mergedSum"": ""f69c8a56d2cd8b401643a52cf423ad1c"",
          ""group"": [
            ""Daklak"",
            ""Test"",
            ""VSEC"",
            ""Daklak_Webs"",
            ""Daklak_Apps""
          ],
          ""registerIP"": ""any"",
          ""configSum"": ""4b54b212c2dd96f8b25aebfea2a104cd"",
          ""timestamp"": ""2020-05-24T23:45:02.278Z"",
          ""host"": ""vsec-logstash"",
          ""cluster"": {
            ""name"": ""wazuh""
          }
        },
        ""fields"": {
          ""timestamp"": [
            ""2020-05-24T23:45:02.278Z""
          ]
        },
        ""highlight"": {
          ""group"": [
            ""@kibana-highlighted-field@Daklak@/kibana-highlighted-field@"",
            ""@kibana-highlighted-field@Daklak_Webs@/kibana-highlighted-field@""
          ]
        },
        ""sort"": [
          -9223372036854776000
        ]
      },
      {
        ""_index"": ""wazuh-monitoring-3.x-2020.05.24"",
        ""_type"": ""_doc"",
        ""_id"": ""BDkTSXIBENNuuStZUdNJ"",
        ""_version"": 1,
        ""_score"": null,
        ""_source"": {
          ""os"": {
            ""arch"": ""x86_64"",
            ""major"": ""6"",
            ""minor"": ""8"",
            ""name"": ""CentOS Linux"",
            ""platform"": ""centos"",
            ""uname"": ""Linux |webserver18 |2.6.32-642.el6.x86_64 |#1 SMP Tue May 10 17:27:01 UTC 2016 |x86_64"",
            ""version"": ""6.8""
          },
          ""id"": ""061"",
          ""ip"": ""192.168.2.18"",
          ""dateAdd"": ""2020-03-31 02:15:08"",
          ""name"": ""webserver18"",
          ""lastKeepAlive"": ""2020-05-24 23:44:51"",
          ""status"": ""Active"",
          ""manager"": ""vsec-logstash"",
          ""version"": ""Wazuh v3.11.4"",
          ""node_name"": ""node02"",
          ""mergedSum"": ""535237cb82b543ec180a59bde6425db5"",
          ""group"": [
            ""Daklak"",
            ""Test"",
            ""VSEC"",
            ""Daklak_Webs""
          ],
          ""registerIP"": ""any"",
          ""configSum"": ""d149ab60df71e6fb53b2d38db75bc733"",
          ""timestamp"": ""2020-05-24T23:45:02.278Z"",
          ""host"": ""vsec-logstash"",
          ""cluster"": {
            ""name"": ""wazuh""
          }
        },
        ""fields"": {
          ""timestamp"": [
            ""2020-05-24T23:45:02.278Z""
          ]
        },
        ""highlight"": {
          ""group"": [
            ""@kibana-highlighted-field@Daklak@/kibana-highlighted-field@"",
            ""@kibana-highlighted-field@Daklak_Webs@/kibana-highlighted-field@""
          ]
        },
        ""sort"": [
          -9223372036854776000
        ]
      },
      {
        ""_index"": ""wazuh-monitoring-3.x-2020.05.25"",
        ""_type"": ""_doc"",
        ""_id"": ""xTohSXIBENNuuStZDINp"",
        ""_version"": 1,
        ""_score"": null,
        ""_source"": {
          ""os"": {
            ""arch"": ""x86_64"",
            ""major"": ""6"",
            ""minor"": ""8"",
            ""name"": ""CentOS Linux"",
            ""platform"": ""centos"",
            ""uname"": ""Linux |webserver18 |2.6.32-642.el6.x86_64 |#1 SMP Tue May 10 17:27:01 UTC 2016 |x86_64"",
            ""version"": ""6.8""
          },
          ""node_name"": ""node02"",
          ""dateAdd"": ""2020-03-31 02:15:08"",
          ""configSum"": ""d149ab60df71e6fb53b2d38db75bc733"",
          ""ip"": ""192.168.2.18"",
          ""version"": ""Wazuh v3.11.4"",
          ""status"": ""Active"",
          ""registerIP"": ""any"",
          ""lastKeepAlive"": ""2020-05-24 23:59:51"",
          ""manager"": ""vsec-logstash"",
          ""name"": ""webserver18"",
          ""mergedSum"": ""535237cb82b543ec180a59bde6425db5"",
          ""id"": ""061"",
          ""group"": [
            ""Daklak"",
            ""Test"",
            ""VSEC"",
            ""Daklak_Webs""
          ],
          ""timestamp"": ""2020-05-25T00:00:02.147Z"",
          ""host"": ""vsec-logstash"",
          ""cluster"": {
            ""name"": ""wazuh""
          }
        },
        ""fields"": {
          ""timestamp"": [
            ""2020-05-25T00:00:02.147Z""
          ]
        },
        ""highlight"": {
          ""group"": [
            ""@kibana-highlighted-field@Daklak@/kibana-highlighted-field@"",
            ""@kibana-highlighted-field@Daklak_Webs@/kibana-highlighted-field@""
          ]
        },
        ""sort"": [
          -9223372036854776000
        ]
      },
      {
        ""_index"": ""wazuh-monitoring-3.x-2020.05.25"",
        ""_type"": ""_doc"",
        ""_id"": ""kjs8SXIBENNuuStZgvL-"",
        ""_version"": 1,
        ""_score"": null,
        ""_source"": {
          ""os"": {
            ""arch"": ""x86_64"",
            ""major"": ""6"",
            ""minor"": ""10"",
            ""name"": ""CentOS Linux"",
            ""platform"": ""centos"",
            ""uname"": ""Linux |werserver |2.6.32-754.11.1.el6.x86_64 |#1 SMP Tue Feb 26 15:38:56 UTC 2019 |x86_64"",
            ""version"": ""6.10""
          },
          ""id"": ""060"",
          ""configSum"": ""4b54b212c2dd96f8b25aebfea2a104cd"",
          ""mergedSum"": ""f69c8a56d2cd8b401643a52cf423ad1c"",
          ""status"": ""Active"",
          ""ip"": ""192.168.2.14"",
          ""registerIP"": ""any"",
          ""version"": ""Wazuh v3.11.4"",
          ""group"": [
            ""Daklak"",
            ""Test"",
            ""VSEC"",
            ""Daklak_Webs"",
            ""Daklak_Apps""
          ],
          ""dateAdd"": ""2020-03-27 07:47:46"",
          ""lastKeepAlive"": ""2020-05-25 00:29:50"",
          ""node_name"": ""node02"",
          ""manager"": ""vsec-logstash"",
          ""name"": ""werserver"",
          ""timestamp"": ""2020-05-25T00:30:01.983Z"",
          ""host"": ""vsec-logstash"",
          ""cluster"": {
            ""name"": ""wazuh""
          }
        },
        ""fields"": {
          ""timestamp"": [
            ""2020-05-25T00:30:01.983Z""
          ]
        },
        ""highlight"": {
          ""group"": [
            ""@kibana-highlighted-field@Daklak@/kibana-highlighted-field@"",
            ""@kibana-highlighted-field@Daklak_Webs@/kibana-highlighted-field@""
          ]
        },
        ""sort"": [
          -9223372036854776000
        ]
      },
      {
        ""_index"": ""wazuh-monitoring-3.x-2020.05.25"",
        ""_type"": ""_doc"",
        ""_id"": ""Wz5lSXIBENNuuStZuBqW"",
        ""_version"": 1,
        ""_score"": null,
        ""_source"": {
          ""os"": {
            ""arch"": ""x86_64"",
            ""major"": ""6"",
            ""minor"": ""10"",
            ""name"": ""CentOS Linux"",
            ""platform"": ""centos"",
            ""uname"": ""Linux |werserver |2.6.32-754.11.1.el6.x86_64 |#1 SMP Tue Feb 26 15:38:56 UTC 2019 |x86_64"",
            ""version"": ""6.10""
          },
          ""id"": ""060"",
          ""lastKeepAlive"": ""2020-05-25 01:15:00"",
          ""dateAdd"": ""2020-03-27 07:47:46"",
          ""status"": ""Active"",
          ""manager"": ""vsec-logstash"",
          ""name"": ""werserver"",
          ""configSum"": ""4b54b212c2dd96f8b25aebfea2a104cd"",
          ""mergedSum"": ""f69c8a56d2cd8b401643a52cf423ad1c"",
          ""group"": [
            ""Daklak"",
            ""Test"",
            ""VSEC"",
            ""Daklak_Webs"",
            ""Daklak_Apps""
          ],
          ""registerIP"": ""any"",
          ""ip"": ""192.168.2.14"",
          ""version"": ""Wazuh v3.11.4"",
          ""node_name"": ""node02"",
          ""timestamp"": ""2020-05-25T01:15:02.671Z"",
          ""host"": ""vsec-logstash"",
          ""cluster"": {
            ""name"": ""wazuh""
          }
        },
        ""fields"": {
          ""timestamp"": [
            ""2020-05-25T01:15:02.671Z""
          ]
        },
        ""highlight"": {
          ""group"": [
            ""@kibana-highlighted-field@Daklak@/kibana-highlighted-field@"",
            ""@kibana-highlighted-field@Daklak_Webs@/kibana-highlighted-field@""
          ]
        },
        ""sort"": [
          -9223372036854776000
        ]
      },
      {
        ""_index"": ""wazuh-monitoring-3.x-2020.05.25"",
        ""_type"": ""_doc"",
        ""_id"": ""MD5zSXIBENNuuStZcdjT"",
        ""_version"": 1,
        ""_score"": null,
        ""_source"": {
          ""os"": {
            ""arch"": ""x86_64"",
            ""major"": ""6"",
            ""minor"": ""10"",
            ""name"": ""CentOS Linux"",
            ""platform"": ""centos"",
            ""uname"": ""Linux |werserver |2.6.32-754.11.1.el6.x86_64 |#1 SMP Tue Feb 26 15:38:56 UTC 2019 |x86_64"",
            ""version"": ""6.10""
          },
          ""manager"": ""vsec-logstash"",
          ""version"": ""Wazuh v3.11.4"",
          ""configSum"": ""4b54b212c2dd96f8b25aebfea2a104cd"",
          ""group"": [
            ""Daklak"",
            ""Test"",
            ""VSEC"",
            ""Daklak_Webs"",
            ""Daklak_Apps""
          ],
          ""registerIP"": ""any"",
          ""status"": ""Active"",
          ""id"": ""060"",
          ""dateAdd"": ""2020-03-27 07:47:46"",
          ""node_name"": ""node02"",
          ""ip"": ""192.168.2.14"",
          ""name"": ""werserver"",
          ""mergedSum"": ""f69c8a56d2cd8b401643a52cf423ad1c"",
          ""lastKeepAlive"": ""2020-05-25 01:29:51"",
          ""timestamp"": ""2020-05-25T01:30:02.063Z"",
          ""host"": ""vsec-logstash"",
          ""cluster"": {
            ""name"": ""wazuh""
          }
        },
        ""fields"": {
          ""timestamp"": [
            ""2020-05-25T01:30:02.063Z""
          ]
        },
        ""highlight"": {
          ""group"": [
            ""@kibana-highlighted-field@Daklak@/kibana-highlighted-field@"",
            ""@kibana-highlighted-field@Daklak_Webs@/kibana-highlighted-field@""
          ]
        },
        ""sort"": [
          -9223372036854776000
        ]
      },
      {
        ""_index"": ""wazuh-monitoring-3.x-2020.05.25"",
        ""_type"": ""_doc"",
        ""_id"": ""MT5zSXIBENNuuStZcdjT"",
        ""_version"": 1,
        ""_score"": null,
        ""_source"": {
          ""os"": {
            ""arch"": ""x86_64"",
            ""major"": ""6"",
            ""minor"": ""8"",
            ""name"": ""CentOS Linux"",
            ""platform"": ""centos"",
            ""uname"": ""Linux |webserver18 |2.6.32-642.el6.x86_64 |#1 SMP Tue May 10 17:27:01 UTC 2016 |x86_64"",
            ""version"": ""6.8""
          },
          ""manager"": ""vsec-logstash"",
          ""version"": ""Wazuh v3.11.4"",
          ""configSum"": ""d149ab60df71e6fb53b2d38db75bc733"",
          ""group"": [
            ""Daklak"",
            ""Test"",
            ""VSEC"",
            ""Daklak_Webs""
          ],
          ""registerIP"": ""any"",
          ""status"": ""Active"",
          ""id"": ""061"",
          ""dateAdd"": ""2020-03-31 02:15:08"",
          ""node_name"": ""node02"",
          ""ip"": ""192.168.2.18"",
          ""name"": ""webserver18"",
          ""mergedSum"": ""535237cb82b543ec180a59bde6425db5"",
          ""lastKeepAlive"": ""2020-05-25 01:29:46"",
          ""timestamp"": ""2020-05-25T01:30:02.063Z"",
          ""host"": ""vsec-logstash"",
          ""cluster"": {
            ""name"": ""wazuh""
          }
        },
        ""fields"": {
          ""timestamp"": [
            ""2020-05-25T01:30:02.063Z""
          ]
        },
        ""highlight"": {
          ""group"": [
            ""@kibana-highlighted-field@Daklak@/kibana-highlighted-field@"",
            ""@kibana-highlighted-field@Daklak_Webs@/kibana-highlighted-field@""
          ]
        },
        ""sort"": [
          -9223372036854776000
        ]
      },
      {
        ""_index"": ""wazuh-monitoring-3.x-2020.05.25"",
        ""_type"": ""_doc"",
        ""_id"": ""Az-BSXIBENNuuStZLJZX"",
        ""_version"": 1,
        ""_score"": null,
        ""_source"": {
          ""os"": {
            ""arch"": ""x86_64"",
            ""major"": ""6"",
            ""minor"": ""10"",
            ""name"": ""CentOS Linux"",
            ""platform"": ""centos"",
            ""uname"": ""Linux |werserver |2.6.32-754.11.1.el6.x86_64 |#1 SMP Tue Feb 26 15:38:56 UTC 2019 |x86_64"",
            ""version"": ""6.10""
          },
          ""version"": ""Wazuh v3.11.4"",
          ""lastKeepAlive"": ""2020-05-25 01:44:51"",
          ""name"": ""werserver"",
          ""dateAdd"": ""2020-03-27 07:47:46"",
          ""status"": ""Active"",
          ""id"": ""060"",
          ""node_name"": ""node02"",
          ""manager"": ""vsec-logstash"",
          ""configSum"": ""4b54b212c2dd96f8b25aebfea2a104cd"",
          ""mergedSum"": ""f69c8a56d2cd8b401643a52cf423ad1c"",
          ""registerIP"": ""any"",
          ""group"": [
            ""Daklak"",
            ""Test"",
            ""VSEC"",
            ""Daklak_Webs"",
            ""Daklak_Apps""
          ],
          ""ip"": ""192.168.2.14"",
          ""timestamp"": ""2020-05-25T01:45:01.785Z"",
          ""host"": ""vsec-logstash"",
          ""cluster"": {
            ""name"": ""wazuh""
          }
        },
        ""fields"": {
          ""timestamp"": [
            ""2020-05-25T01:45:01.785Z""
          ]
        },
        ""highlight"": {
          ""group"": [
            ""@kibana-highlighted-field@Daklak@/kibana-highlighted-field@"",
            ""@kibana-highlighted-field@Daklak_Webs@/kibana-highlighted-field@""
          ]
        },
        ""sort"": [
          -9223372036854776000
        ]
      },
      {
        ""_index"": ""wazuh-monitoring-3.x-2020.05.25"",
        ""_type"": ""_doc"",
        ""_id"": ""BD-BSXIBENNuuStZLJZX"",
        ""_version"": 1,
        ""_score"": null,
        ""_source"": {
          ""os"": {
            ""arch"": ""x86_64"",
            ""major"": ""6"",
            ""minor"": ""8"",
            ""name"": ""CentOS Linux"",
            ""platform"": ""centos"",
            ""uname"": ""Linux |webserver18 |2.6.32-642.el6.x86_64 |#1 SMP Tue May 10 17:27:01 UTC 2016 |x86_64"",
            ""version"": ""6.8""
          },
          ""version"": ""Wazuh v3.11.4"",
          ""lastKeepAlive"": ""2020-05-25 01:44:52"",
          ""name"": ""webserver18"",
          ""dateAdd"": ""2020-03-31 02:15:08"",
          ""status"": ""Active"",
          ""id"": ""061"",
          ""node_name"": ""node02"",
          ""manager"": ""vsec-logstash"",
          ""configSum"": ""d149ab60df71e6fb53b2d38db75bc733"",
          ""mergedSum"": ""535237cb82b543ec180a59bde6425db5"",
          ""registerIP"": ""any"",
          ""group"": [
            ""Daklak"",
            ""Test"",
            ""VSEC"",
            ""Daklak_Webs""
          ],
          ""ip"": ""192.168.2.18"",
          ""timestamp"": ""2020-05-25T01:45:01.785Z"",
          ""host"": ""vsec-logstash"",
          ""cluster"": {
            ""name"": ""wazuh""
          }
        },
        ""fields"": {
          ""timestamp"": [
            ""2020-05-25T01:45:01.785Z""
          ]
        },
        ""highlight"": {
          ""group"": [
            ""@kibana-highlighted-field@Daklak@/kibana-highlighted-field@"",
            ""@kibana-highlighted-field@Daklak_Webs@/kibana-highlighted-field@""
          ]
        },
        ""sort"": [
          -9223372036854776000
        ]
      },
      {
        ""_index"": ""wazuh-monitoring-3.x-2020.05.25"",
        ""_type"": ""_doc"",
        ""_id"": ""I0GcSXIBENNuuStZpR0-"",
        ""_version"": 1,
        ""_score"": null,
        ""_source"": {
          ""os"": {
            ""arch"": ""x86_64"",
            ""major"": ""6"",
            ""minor"": ""10"",
            ""name"": ""CentOS Linux"",
            ""platform"": ""centos"",
            ""uname"": ""Linux |werserver |2.6.32-754.11.1.el6.x86_64 |#1 SMP Tue Feb 26 15:38:56 UTC 2019 |x86_64"",
            ""version"": ""6.10""
          },
          ""registerIP"": ""any"",
          ""status"": ""Active"",
          ""name"": ""werserver"",
          ""version"": ""Wazuh v3.11.4"",
          ""id"": ""060"",
          ""manager"": ""vsec-logstash"",
          ""dateAdd"": ""2020-03-27 07:47:46"",
          ""mergedSum"": ""f69c8a56d2cd8b401643a52cf423ad1c"",
          ""node_name"": ""node02"",
          ""lastKeepAlive"": ""2020-05-25 02:14:52"",
          ""group"": [
            ""Daklak"",
            ""Test"",
            ""VSEC"",
            ""Daklak_Webs"",
            ""Daklak_Apps""
          ],
          ""ip"": ""192.168.2.14"",
          ""configSum"": ""4b54b212c2dd96f8b25aebfea2a104cd"",
          ""timestamp"": ""2020-05-25T02:15:02.205Z"",
          ""host"": ""vsec-logstash"",
          ""cluster"": {
            ""name"": ""wazuh""
          }
        },
        ""fields"": {
          ""timestamp"": [
            ""2020-05-25T02:15:02.205Z""
          ]
        },
        ""highlight"": {
          ""group"": [
            ""@kibana-highlighted-field@Daklak@/kibana-highlighted-field@"",
            ""@kibana-highlighted-field@Daklak_Webs@/kibana-highlighted-field@""
          ]
        },
        ""sort"": [
          -9223372036854776000
        ]
      },
      {
        ""_index"": ""wazuh-monitoring-3.x-2020.05.24"",
        ""_type"": ""_doc"",
        ""_id"": ""1f6ERHIBENNuuStZAgv5"",
        ""_version"": 1,
        ""_score"": null,
        ""_source"": {
          ""os"": {
            ""arch"": ""x86_64"",
            ""major"": ""6"",
            ""minor"": ""10"",
            ""name"": ""CentOS Linux"",
            ""platform"": ""centos"",
            ""uname"": ""Linux |werserver |2.6.32-754.11.1.el6.x86_64 |#1 SMP Tue Feb 26 15:38:56 UTC 2019 |x86_64"",
            ""version"": ""6.10""
          },
          ""lastKeepAlive"": ""2020-05-24 02:29:47"",
          ""ip"": ""192.168.2.14"",
          ""id"": ""060"",
          ""mergedSum"": ""f69c8a56d2cd8b401643a52cf423ad1c"",
          ""manager"": ""vsec-logstash"",
          ""dateAdd"": ""2020-03-27 07:47:46"",
          ""node_name"": ""node02"",
          ""registerIP"": ""any"",
          ""group"": [
            ""Daklak"",
            ""Test"",
            ""VSEC"",
            ""Daklak_Webs"",
            ""Daklak_Apps""
          ],
          ""configSum"": ""4b54b212c2dd96f8b25aebfea2a104cd"",
          ""status"": ""Active"",
          ""version"": ""Wazuh v3.11.4"",
          ""name"": ""werserver"",
          ""timestamp"": ""2020-05-24T02:30:01.712Z"",
          ""host"": ""vsec-logstash"",
          ""cluster"": {
            ""name"": ""wazuh""
          }
        },
        ""fields"": {
          ""timestamp"": [
            ""2020-05-24T02:30:01.712Z""
          ]
        },
        ""highlight"": {
          ""group"": [
            ""@kibana-highlighted-field@Daklak@/kibana-highlighted-field@"",
            ""@kibana-highlighted-field@Daklak_Webs@/kibana-highlighted-field@""
          ]
        },
        ""sort"": [
          -9223372036854776000
        ]
      },
      {
        ""_index"": ""wazuh-monitoring-3.x-2020.05.24"",
        ""_type"": ""_doc"",
        ""_id"": ""RQYoRXIBENNuuStZ0IF3"",
        ""_version"": 1,
        ""_score"": null,
        ""_source"": {
          ""os"": {
            ""arch"": ""x86_64"",
            ""major"": ""6"",
            ""minor"": ""10"",
            ""name"": ""CentOS Linux"",
            ""platform"": ""centos"",
            ""uname"": ""Linux |werserver |2.6.32-754.11.1.el6.x86_64 |#1 SMP Tue Feb 26 15:38:56 UTC 2019 |x86_64"",
            ""version"": ""6.10""
          },
          ""node_name"": ""node02"",
          ""lastKeepAlive"": ""2020-05-24 05:29:48"",
          ""group"": [
            ""Daklak"",
            ""Test"",
            ""VSEC"",
            ""Daklak_Webs"",
            ""Daklak_Apps""
          ],
          ""manager"": ""vsec-logstash"",
          ""dateAdd"": ""2020-03-27 07:47:46"",
          ""status"": ""Active"",
          ""ip"": ""192.168.2.14"",
          ""version"": ""Wazuh v3.11.4"",
          ""configSum"": ""4b54b212c2dd96f8b25aebfea2a104cd"",
          ""registerIP"": ""any"",
          ""name"": ""werserver"",
          ""id"": ""060"",
          ""mergedSum"": ""f69c8a56d2cd8b401643a52cf423ad1c"",
          ""timestamp"": ""2020-05-24T05:30:02.227Z"",
          ""host"": ""vsec-logstash"",
          ""cluster"": {
            ""name"": ""wazuh""
          }
        },
        ""fields"": {
          ""timestamp"": [
            ""2020-05-24T05:30:02.227Z""
          ]
        },
        ""highlight"": {
          ""group"": [
            ""@kibana-highlighted-field@Daklak@/kibana-highlighted-field@"",
            ""@kibana-highlighted-field@Daklak_Webs@/kibana-highlighted-field@""
          ]
        },
        ""sort"": [
          -9223372036854776000
        ]
      },
      {
        ""_index"": ""wazuh-monitoring-3.x-2020.05.24"",
        ""_type"": ""_doc"",
        ""_id"": ""RgYoRXIBENNuuStZ0IF3"",
        ""_version"": 1,
        ""_score"": null,
        ""_source"": {
          ""os"": {
            ""arch"": ""x86_64"",
            ""major"": ""6"",
            ""minor"": ""8"",
            ""name"": ""CentOS Linux"",
            ""platform"": ""centos"",
            ""uname"": ""Linux |webserver18 |2.6.32-642.el6.x86_64 |#1 SMP Tue May 10 17:27:01 UTC 2016 |x86_64"",
            ""version"": ""6.8""
          },
          ""node_name"": ""node02"",
          ""lastKeepAlive"": ""2020-05-24 05:29:52"",
          ""group"": [
            ""Daklak"",
            ""Test"",
            ""VSEC"",
            ""Daklak_Webs""
          ],
          ""manager"": ""vsec-logstash"",
          ""dateAdd"": ""2020-03-31 02:15:08"",
          ""status"": ""Active"",
          ""ip"": ""192.168.2.18"",
          ""version"": ""Wazuh v3.11.4"",
          ""configSum"": ""d149ab60df71e6fb53b2d38db75bc733"",
          ""registerIP"": ""any"",
          ""name"": ""webserver18"",
          ""id"": ""061"",
          ""mergedSum"": ""535237cb82b543ec180a59bde6425db5"",
          ""timestamp"": ""2020-05-24T05:30:02.227Z"",
          ""host"": ""vsec-logstash"",
          ""cluster"": {
            ""name"": ""wazuh""
          }
        },
        ""fields"": {
          ""timestamp"": [
            ""2020-05-24T05:30:02.227Z""
          ]
        },
        ""highlight"": {
          ""group"": [
            ""@kibana-highlighted-field@Daklak@/kibana-highlighted-field@"",
            ""@kibana-highlighted-field@Daklak_Webs@/kibana-highlighted-field@""
          ]
        },
        ""sort"": [
          -9223372036854776000
        ]
      },
      {
        ""_index"": ""wazuh-monitoring-3.x-2020.05.24"",
        ""_type"": ""_doc"",
        ""_id"": ""DwptRXIBENNuuStZewEf"",
        ""_version"": 1,
        ""_score"": null,
        ""_source"": {
          ""os"": {
            ""arch"": ""x86_64"",
            ""major"": ""6"",
            ""minor"": ""10"",
            ""name"": ""CentOS Linux"",
            ""platform"": ""centos"",
            ""uname"": ""Linux |werserver |2.6.32-754.11.1.el6.x86_64 |#1 SMP Tue Feb 26 15:38:56 UTC 2019 |x86_64"",
            ""version"": ""6.10""
          },
          ""version"": ""Wazuh v3.11.4"",
          ""registerIP"": ""any"",
          ""mergedSum"": ""f69c8a56d2cd8b401643a52cf423ad1c"",
          ""ip"": ""192.168.2.14"",
          ""node_name"": ""node02"",
          ""id"": ""060"",
          ""dateAdd"": ""2020-03-27 07:47:46"",
          ""name"": ""werserver"",
          ""configSum"": ""4b54b212c2dd96f8b25aebfea2a104cd"",
          ""manager"": ""vsec-logstash"",
          ""group"": [
            ""Daklak"",
            ""Test"",
            ""VSEC"",
            ""Daklak_Webs"",
            ""Daklak_Apps""
          ],
          ""lastKeepAlive"": ""2020-05-24 06:44:48"",
          ""status"": ""Active"",
          ""timestamp"": ""2020-05-24T06:45:02.349Z"",
          ""host"": ""vsec-logstash"",
          ""cluster"": {
            ""name"": ""wazuh""
          }
        },
        ""fields"": {
          ""timestamp"": [
            ""2020-05-24T06:45:02.349Z""
          ]
        },
        ""highlight"": {
          ""group"": [
            ""@kibana-highlighted-field@Daklak@/kibana-highlighted-field@"",
            ""@kibana-highlighted-field@Daklak_Webs@/kibana-highlighted-field@""
          ]
        },
        ""sort"": [
          -9223372036854776000
        ]
      },
      {
        ""_index"": ""wazuh-monitoring-3.x-2020.05.24"",
        ""_type"": ""_doc"",
        ""_id"": ""SwlfRXIBENNuuStZvVBL"",
        ""_version"": 1,
        ""_score"": null,
        ""_source"": {
          ""os"": {
            ""arch"": ""x86_64"",
            ""major"": ""6"",
            ""minor"": ""10"",
            ""name"": ""CentOS Linux"",
            ""platform"": ""centos"",
            ""uname"": ""Linux |werserver |2.6.32-754.11.1.el6.x86_64 |#1 SMP Tue Feb 26 15:38:56 UTC 2019 |x86_64"",
            ""version"": ""6.10""
          },
          ""manager"": ""vsec-logstash"",
          ""configSum"": ""4b54b212c2dd96f8b25aebfea2a104cd"",
          ""registerIP"": ""any"",
          ""ip"": ""192.168.2.14"",
          ""status"": ""Active"",
          ""id"": ""060"",
          ""lastKeepAlive"": ""2020-05-24 06:29:48"",
          ""group"": [
            ""Daklak"",
            ""Test"",
            ""VSEC"",
            ""Daklak_Webs"",
            ""Daklak_Apps""
          ],
          ""dateAdd"": ""2020-03-27 07:47:46"",
          ""mergedSum"": ""f69c8a56d2cd8b401643a52cf423ad1c"",
          ""node_name"": ""node02"",
          ""name"": ""werserver"",
          ""version"": ""Wazuh v3.11.4"",
          ""timestamp"": ""2020-05-24T06:30:01.807Z"",
          ""host"": ""vsec-logstash"",
          ""cluster"": {
            ""name"": ""wazuh""
          }
        },
        ""fields"": {
          ""timestamp"": [
            ""2020-05-24T06:30:01.807Z""
          ]
        },
        ""highlight"": {
          ""group"": [
            ""@kibana-highlighted-field@Daklak@/kibana-highlighted-field@"",
            ""@kibana-highlighted-field@Daklak_Webs@/kibana-highlighted-field@""
          ]
        },
        ""sort"": [
          -9223372036854776000
        ]
      },
      {
        ""_index"": ""wazuh-monitoring-3.x-2020.05.24"",
        ""_type"": ""_doc"",
        ""_id"": ""TAlfRXIBENNuuStZvVBL"",
        ""_version"": 1,
        ""_score"": null,
        ""_source"": {
          ""os"": {
            ""arch"": ""x86_64"",
            ""major"": ""6"",
            ""minor"": ""8"",
            ""name"": ""CentOS Linux"",
            ""platform"": ""centos"",
            ""uname"": ""Linux |webserver18 |2.6.32-642.el6.x86_64 |#1 SMP Tue May 10 17:27:01 UTC 2016 |x86_64"",
            ""version"": ""6.8""
          },
          ""manager"": ""vsec-logstash"",
          ""configSum"": ""d149ab60df71e6fb53b2d38db75bc733"",
          ""registerIP"": ""any"",
          ""ip"": ""192.168.2.18"",
          ""status"": ""Active"",
          ""id"": ""061"",
          ""lastKeepAlive"": ""2020-05-24 06:29:42"",
          ""group"": [
            ""Daklak"",
            ""Test"",
            ""VSEC"",
            ""Daklak_Webs""
          ],
          ""dateAdd"": ""2020-03-31 02:15:08"",
          ""mergedSum"": ""535237cb82b543ec180a59bde6425db5"",
          ""node_name"": ""node02"",
          ""name"": ""webserver18"",
          ""version"": ""Wazuh v3.11.4"",
          ""timestamp"": ""2020-05-24T06:30:01.807Z"",
          ""host"": ""vsec-logstash"",
          ""cluster"": {
            ""name"": ""wazuh""
          }
        },
        ""fields"": {
          ""timestamp"": [
            ""2020-05-24T06:30:01.807Z""
          ]
        },
        ""highlight"": {
          ""group"": [
            ""@kibana-highlighted-field@Daklak@/kibana-highlighted-field@"",
            ""@kibana-highlighted-field@Daklak_Webs@/kibana-highlighted-field@""
          ]
        },
        ""sort"": [
          -9223372036854776000
        ]
      },
      {
        ""_index"": ""wazuh-monitoring-3.x-2020.05.24"",
        ""_type"": ""_doc"",
        ""_id"": ""Tgp7RXIBENNuuStZNbPv"",
        ""_version"": 1,
        ""_score"": null,
        ""_source"": {
          ""os"": {
            ""arch"": ""x86_64"",
            ""major"": ""6"",
            ""minor"": ""10"",
            ""name"": ""CentOS Linux"",
            ""platform"": ""centos"",
            ""uname"": ""Linux |werserver |2.6.32-754.11.1.el6.x86_64 |#1 SMP Tue Feb 26 15:38:56 UTC 2019 |x86_64"",
            ""version"": ""6.10""
          },
          ""name"": ""werserver"",
          ""group"": [
            ""Daklak"",
            ""Test"",
            ""VSEC"",
            ""Daklak_Webs"",
            ""Daklak_Apps""
          ],
          ""manager"": ""vsec-logstash"",
          ""ip"": ""192.168.2.14"",
          ""dateAdd"": ""2020-03-27 07:47:46"",
          ""mergedSum"": ""f69c8a56d2cd8b401643a52cf423ad1c"",
          ""registerIP"": ""any"",
          ""status"": ""Active"",
          ""lastKeepAlive"": ""2020-05-24 06:59:48"",
          ""configSum"": ""4b54b212c2dd96f8b25aebfea2a104cd"",
          ""version"": ""Wazuh v3.11.4"",
          ""id"": ""060"",
          ""node_name"": ""node02"",
          ""timestamp"": ""2020-05-24T07:00:02.161Z"",
          ""host"": ""vsec-logstash"",
          ""cluster"": {
            ""name"": ""wazuh""
          }
        },
        ""fields"": {
          ""timestamp"": [
            ""2020-05-24T07:00:02.161Z""
          ]
        },
        ""highlight"": {
          ""group"": [
            ""@kibana-highlighted-field@Daklak@/kibana-highlighted-field@"",
            ""@kibana-highlighted-field@Daklak_Webs@/kibana-highlighted-field@""
          ]
        },
        ""sort"": [
          -9223372036854776000
        ]
      },
      {
        ""_index"": ""wazuh-monitoring-3.x-2020.05.24"",
        ""_type"": ""_doc"",
        ""_id"": ""fQhSRXIBENNuuStZBJ8I"",
        ""_version"": 1,
        ""_score"": null,
        ""_source"": {
          ""os"": {
            ""arch"": ""x86_64"",
            ""major"": ""6"",
            ""minor"": ""10"",
            ""name"": ""CentOS Linux"",
            ""platform"": ""centos"",
            ""uname"": ""Linux |werserver |2.6.32-754.11.1.el6.x86_64 |#1 SMP Tue Feb 26 15:38:56 UTC 2019 |x86_64"",
            ""version"": ""6.10""
          },
          ""status"": ""Active"",
          ""registerIP"": ""any"",
          ""name"": ""werserver"",
          ""id"": ""060"",
          ""manager"": ""vsec-logstash"",
          ""lastKeepAlive"": ""2020-05-24 06:14:58"",
          ""dateAdd"": ""2020-03-27 07:47:46"",
          ""configSum"": ""4b54b212c2dd96f8b25aebfea2a104cd"",
          ""group"": [
            ""Daklak"",
            ""Test"",
            ""VSEC"",
            ""Daklak_Webs"",
            ""Daklak_Apps""
          ],
          ""node_name"": ""node02"",
          ""version"": ""Wazuh v3.11.4"",
          ""ip"": ""192.168.2.14"",
          ""mergedSum"": ""f69c8a56d2cd8b401643a52cf423ad1c"",
          ""timestamp"": ""2020-05-24T06:15:02.412Z"",
          ""host"": ""vsec-logstash"",
          ""cluster"": {
            ""name"": ""wazuh""
          }
        },
        ""fields"": {
          ""timestamp"": [
            ""2020-05-24T06:15:02.412Z""
          ]
        },
        ""highlight"": {
          ""group"": [
            ""@kibana-highlighted-field@Daklak@/kibana-highlighted-field@"",
            ""@kibana-highlighted-field@Daklak_Webs@/kibana-highlighted-field@""
          ]
        },
        ""sort"": [
          -9223372036854776000
        ]
      },
      {
        ""_index"": ""wazuh-monitoring-3.x-2020.05.24"",
        ""_type"": ""_doc"",
        ""_id"": ""mQUbRXIBENNuuStZFs5r"",
        ""_version"": 1,
        ""_score"": null,
        ""_source"": {
          ""os"": {
            ""arch"": ""x86_64"",
            ""major"": ""6"",
            ""minor"": ""10"",
            ""name"": ""CentOS Linux"",
            ""platform"": ""centos"",
            ""uname"": ""Linux |werserver |2.6.32-754.11.1.el6.x86_64 |#1 SMP Tue Feb 26 15:38:56 UTC 2019 |x86_64"",
            ""version"": ""6.10""
          },
          ""node_name"": ""node02"",
          ""registerIP"": ""any"",
          ""manager"": ""vsec-logstash"",
          ""group"": [
            ""Daklak"",
            ""Test"",
            ""VSEC"",
            ""Daklak_Webs"",
            ""Daklak_Apps""
          ],
          ""configSum"": ""4b54b212c2dd96f8b25aebfea2a104cd"",
          ""name"": ""werserver"",
          ""dateAdd"": ""2020-03-27 07:47:46"",
          ""status"": ""Active"",
          ""ip"": ""192.168.2.14"",
          ""version"": ""Wazuh v3.11.4"",
          ""id"": ""060"",
          ""lastKeepAlive"": ""2020-05-24 05:14:47"",
          ""mergedSum"": ""f69c8a56d2cd8b401643a52cf423ad1c"",
          ""timestamp"": ""2020-05-24T05:15:02.633Z"",
          ""host"": ""vsec-logstash"",
          ""cluster"": {
            ""name"": ""wazuh""
          }
        },
        ""fields"": {
          ""timestamp"": [
            ""2020-05-24T05:15:02.633Z""
          ]
        },
        ""highlight"": {
          ""group"": [
            ""@kibana-highlighted-field@Daklak@/kibana-highlighted-field@"",
            ""@kibana-highlighted-field@Daklak_Webs@/kibana-highlighted-field@""
          ]
        },
        ""sort"": [
          -9223372036854776000
        ]
      },
      {
        ""_index"": ""wazuh-monitoring-3.x-2020.05.24"",
        ""_type"": ""_doc"",
        ""_id"": ""HQUNRXIBENNuuStZWBNE"",
        ""_version"": 1,
        ""_score"": null,
        ""_source"": {
          ""os"": {
            ""arch"": ""x86_64"",
            ""major"": ""6"",
            ""minor"": ""10"",
            ""name"": ""CentOS Linux"",
            ""platform"": ""centos"",
            ""uname"": ""Linux |werserver |2.6.32-754.11.1.el6.x86_64 |#1 SMP Tue Feb 26 15:38:56 UTC 2019 |x86_64"",
            ""version"": ""6.10""
          },
          ""registerIP"": ""any"",
          ""status"": ""Active"",
          ""node_name"": ""node02"",
          ""lastKeepAlive"": ""2020-05-24 04:59:47"",
          ""configSum"": ""4b54b212c2dd96f8b25aebfea2a104cd"",
          ""group"": [
            ""Daklak"",
            ""Test"",
            ""VSEC"",
            ""Daklak_Webs"",
            ""Daklak_Apps""
          ],
          ""name"": ""werserver"",
          ""ip"": ""192.168.2.14"",
          ""mergedSum"": ""f69c8a56d2cd8b401643a52cf423ad1c"",
          ""version"": ""Wazuh v3.11.4"",
          ""dateAdd"": ""2020-03-27 07:47:46"",
          ""id"": ""060"",
          ""manager"": ""vsec-logstash"",
          ""timestamp"": ""2020-05-24T05:00:01.988Z"",
          ""host"": ""vsec-logstash"",
          ""cluster"": {
            ""name"": ""wazuh""
          }
        },
        ""fields"": {
          ""timestamp"": [
            ""2020-05-24T05:00:01.988Z""
          ]
        },
        ""highlight"": {
          ""group"": [
            ""@kibana-highlighted-field@Daklak@/kibana-highlighted-field@"",
            ""@kibana-highlighted-field@Daklak_Webs@/kibana-highlighted-field@""
          ]
        },
        ""sort"": [
          -9223372036854776000
        ]
      },
      {
        ""_index"": ""wazuh-monitoring-3.x-2020.05.24"",
        ""_type"": ""_doc"",
        ""_id"": ""HgUNRXIBENNuuStZWBNE"",
        ""_version"": 1,
        ""_score"": null,
        ""_source"": {
          ""os"": {
            ""arch"": ""x86_64"",
            ""major"": ""6"",
            ""minor"": ""8"",
            ""name"": ""CentOS Linux"",
            ""platform"": ""centos"",
            ""uname"": ""Linux |webserver18 |2.6.32-642.el6.x86_64 |#1 SMP Tue May 10 17:27:01 UTC 2016 |x86_64"",
            ""version"": ""6.8""
          },
          ""registerIP"": ""any"",
          ""status"": ""Active"",
          ""node_name"": ""node02"",
          ""lastKeepAlive"": ""2020-05-24 04:59:42"",
          ""configSum"": ""d149ab60df71e6fb53b2d38db75bc733"",
          ""group"": [
            ""Daklak"",
            ""Test"",
            ""VSEC"",
            ""Daklak_Webs""
          ],
          ""name"": ""webserver18"",
          ""ip"": ""192.168.2.18"",
          ""mergedSum"": ""535237cb82b543ec180a59bde6425db5"",
          ""version"": ""Wazuh v3.11.4"",
          ""dateAdd"": ""2020-03-31 02:15:08"",
          ""id"": ""061"",
          ""manager"": ""vsec-logstash"",
          ""timestamp"": ""2020-05-24T05:00:01.988Z"",
          ""host"": ""vsec-logstash"",
          ""cluster"": {
            ""name"": ""wazuh""
          }
        },
        ""fields"": {
          ""timestamp"": [
            ""2020-05-24T05:00:01.988Z""
          ]
        },
        ""highlight"": {
          ""group"": [
            ""@kibana-highlighted-field@Daklak@/kibana-highlighted-field@"",
            ""@kibana-highlighted-field@Daklak_Webs@/kibana-highlighted-field@""
          ]
        },
        ""sort"": [
          -9223372036854776000
        ]
      },
      {
        ""_index"": ""wazuh-monitoring-3.x-2020.05.24"",
        ""_type"": ""_doc"",
        ""_id"": ""AxQ7RnIBENNuuStZdpd1"",
        ""_version"": 1,
        ""_score"": null,
        ""_source"": {
          ""os"": {
            ""arch"": ""x86_64"",
            ""major"": ""6"",
            ""minor"": ""10"",
            ""name"": ""CentOS Linux"",
            ""platform"": ""centos"",
            ""uname"": ""Linux |werserver |2.6.32-754.11.1.el6.x86_64 |#1 SMP Tue Feb 26 15:38:56 UTC 2019 |x86_64"",
            ""version"": ""6.10""
          },
          ""mergedSum"": ""f69c8a56d2cd8b401643a52cf423ad1c"",
          ""status"": ""Active"",
          ""lastKeepAlive"": ""2020-05-24 10:29:58"",
          ""version"": ""Wazuh v3.11.4"",
          ""manager"": ""vsec-logstash"",
          ""ip"": ""192.168.2.14"",
          ""dateAdd"": ""2020-03-27 07:47:46"",
          ""registerIP"": ""any"",
          ""group"": [
            ""Daklak"",
            ""Test"",
            ""VSEC"",
            ""Daklak_Webs"",
            ""Daklak_Apps""
          ],
          ""name"": ""werserver"",
          ""node_name"": ""node02"",
          ""configSum"": ""4b54b212c2dd96f8b25aebfea2a104cd"",
          ""id"": ""060"",
          ""timestamp"": ""2020-05-24T10:30:01.592Z"",
          ""host"": ""vsec-logstash"",
          ""cluster"": {
            ""name"": ""wazuh""
          }
        },
        ""fields"": {
          ""timestamp"": [
            ""2020-05-24T10:30:01.592Z""
          ]
        },
        ""highlight"": {
          ""group"": [
            ""@kibana-highlighted-field@Daklak@/kibana-highlighted-field@"",
            ""@kibana-highlighted-field@Daklak_Webs@/kibana-highlighted-field@""
          ]
        },
        ""sort"": [
          -9223372036854776000
        ]
      },
      {
        ""_index"": ""wazuh-monitoring-3.x-2020.05.24"",
        ""_type"": ""_doc"",
        ""_id"": ""BBQ7RnIBENNuuStZdpd1"",
        ""_version"": 1,
        ""_score"": null,
        ""_source"": {
          ""os"": {
            ""arch"": ""x86_64"",
            ""major"": ""6"",
            ""minor"": ""8"",
            ""name"": ""CentOS Linux"",
            ""platform"": ""centos"",
            ""uname"": ""Linux |webserver18 |2.6.32-642.el6.x86_64 |#1 SMP Tue May 10 17:27:01 UTC 2016 |x86_64"",
            ""version"": ""6.8""
          },
          ""mergedSum"": ""535237cb82b543ec180a59bde6425db5"",
          ""status"": ""Active"",
          ""lastKeepAlive"": ""2020-05-24 10:29:51"",
          ""version"": ""Wazuh v3.11.4"",
          ""manager"": ""vsec-logstash"",
          ""ip"": ""192.168.2.18"",
          ""dateAdd"": ""2020-03-31 02:15:08"",
          ""registerIP"": ""any"",
          ""group"": [
            ""Daklak"",
            ""Test"",
            ""VSEC"",
            ""Daklak_Webs""
          ],
          ""name"": ""webserver18"",
          ""node_name"": ""node02"",
          ""configSum"": ""d149ab60df71e6fb53b2d38db75bc733"",
          ""id"": ""061"",
          ""timestamp"": ""2020-05-24T10:30:01.592Z"",
          ""host"": ""vsec-logstash"",
          ""cluster"": {
            ""name"": ""wazuh""
          }
        },
        ""fields"": {
          ""timestamp"": [
            ""2020-05-24T10:30:01.592Z""
          ]
        },
        ""highlight"": {
          ""group"": [
            ""@kibana-highlighted-field@Daklak@/kibana-highlighted-field@"",
            ""@kibana-highlighted-field@Daklak_Webs@/kibana-highlighted-field@""
          ]
        },
        ""sort"": [
          -9223372036854776000
        ]
      },
      {
        ""_index"": ""wazuh-monitoring-3.x-2020.05.24"",
        ""_type"": ""_doc"",
        ""_id"": ""QhVWRnIBENNuuStZ7fkX"",
        ""_version"": 1,
        ""_score"": null,
        ""_source"": {
          ""os"": {
            ""arch"": ""x86_64"",
            ""major"": ""6"",
            ""minor"": ""8"",
            ""name"": ""CentOS Linux"",
            ""platform"": ""centos"",
            ""uname"": ""Linux |webserver18 |2.6.32-642.el6.x86_64 |#1 SMP Tue May 10 17:27:01 UTC 2016 |x86_64"",
            ""version"": ""6.8""
          },
          ""name"": ""webserver18"",
          ""status"": ""Active"",
          ""configSum"": ""d149ab60df71e6fb53b2d38db75bc733"",
          ""mergedSum"": ""535237cb82b543ec180a59bde6425db5"",
          ""dateAdd"": ""2020-03-31 02:15:08"",
          ""node_name"": ""node02"",
          ""registerIP"": ""any"",
          ""id"": ""061"",
          ""lastKeepAlive"": ""2020-05-24 10:59:51"",
          ""ip"": ""192.168.2.18"",
          ""manager"": ""vsec-logstash"",
          ""group"": [
            ""Daklak"",
            ""Test"",
            ""VSEC"",
            ""Daklak_Webs""
          ],
          ""version"": ""Wazuh v3.11.4"",
          ""timestamp"": ""2020-05-24T11:00:01.435Z"",
          ""host"": ""vsec-logstash"",
          ""cluster"": {
            ""name"": ""wazuh""
          }
        },
        ""fields"": {
          ""timestamp"": [
            ""2020-05-24T11:00:01.435Z""
          ]
        },
        ""highlight"": {
          ""group"": [
            ""@kibana-highlighted-field@Daklak@/kibana-highlighted-field@"",
            ""@kibana-highlighted-field@Daklak_Webs@/kibana-highlighted-field@""
          ]
        },
        ""sort"": [
          -9223372036854776000
        ]
      },
      {
        ""_index"": ""wazuh-monitoring-3.x-2020.05.24"",
        ""_type"": ""_doc"",
        ""_id"": ""IhISRnIBENNuuStZRXY1"",
        ""_version"": 1,
        ""_score"": null,
        ""_source"": {
          ""os"": {
            ""arch"": ""x86_64"",
            ""major"": ""6"",
            ""minor"": ""10"",
            ""name"": ""CentOS Linux"",
            ""platform"": ""centos"",
            ""uname"": ""Linux |werserver |2.6.32-754.11.1.el6.x86_64 |#1 SMP Tue Feb 26 15:38:56 UTC 2019 |x86_64"",
            ""version"": ""6.10""
          },
          ""ip"": ""192.168.2.14"",
          ""mergedSum"": ""f69c8a56d2cd8b401643a52cf423ad1c"",
          ""node_name"": ""node02"",
          ""id"": ""060"",
          ""name"": ""werserver"",
          ""version"": ""Wazuh v3.11.4"",
          ""dateAdd"": ""2020-03-27 07:47:46"",
          ""registerIP"": ""any"",
          ""configSum"": ""4b54b212c2dd96f8b25aebfea2a104cd"",
          ""manager"": ""vsec-logstash"",
          ""status"": ""Active"",
          ""group"": [
            ""Daklak"",
            ""Test"",
            ""VSEC"",
            ""Daklak_Webs"",
            ""Daklak_Apps""
          ],
          ""lastKeepAlive"": ""2020-05-24 09:44:47"",
          ""timestamp"": ""2020-05-24T09:45:02.001Z"",
          ""host"": ""vsec-logstash"",
          ""cluster"": {
            ""name"": ""wazuh""
          }
        },
        ""fields"": {
          ""timestamp"": [
            ""2020-05-24T09:45:02.001Z""
          ]
        },
        ""highlight"": {
          ""group"": [
            ""@kibana-highlighted-field@Daklak@/kibana-highlighted-field@"",
            ""@kibana-highlighted-field@Daklak_Webs@/kibana-highlighted-field@""
          ]
        },
        ""sort"": [
          -9223372036854776000
        ]
      },
      {
        ""_index"": ""wazuh-monitoring-3.x-2020.05.24"",
        ""_type"": ""_doc"",
        ""_id"": ""IxISRnIBENNuuStZRXY1"",
        ""_version"": 1,
        ""_score"": null,
        ""_source"": {
          ""os"": {
            ""arch"": ""x86_64"",
            ""major"": ""6"",
            ""minor"": ""8"",
            ""name"": ""CentOS Linux"",
            ""platform"": ""centos"",
            ""uname"": ""Linux |webserver18 |2.6.32-642.el6.x86_64 |#1 SMP Tue May 10 17:27:01 UTC 2016 |x86_64"",
            ""version"": ""6.8""
          },
          ""ip"": ""192.168.2.18"",
          ""mergedSum"": ""535237cb82b543ec180a59bde6425db5"",
          ""node_name"": ""node02"",
          ""id"": ""061"",
          ""name"": ""webserver18"",
          ""version"": ""Wazuh v3.11.4"",
          ""dateAdd"": ""2020-03-31 02:15:08"",
          ""registerIP"": ""any"",
          ""configSum"": ""d149ab60df71e6fb53b2d38db75bc733"",
          ""manager"": ""vsec-logstash"",
          ""status"": ""Active"",
          ""group"": [
            ""Daklak"",
            ""Test"",
            ""VSEC"",
            ""Daklak_Webs""
          ],
          ""lastKeepAlive"": ""2020-05-24 09:44:51"",
          ""timestamp"": ""2020-05-24T09:45:02.001Z"",
          ""host"": ""vsec-logstash"",
          ""cluster"": {
            ""name"": ""wazuh""
          }
        },
        ""fields"": {
          ""timestamp"": [
            ""2020-05-24T09:45:02.001Z""
          ]
        },
        ""highlight"": {
          ""group"": [
            ""@kibana-highlighted-field@Daklak@/kibana-highlighted-field@"",
            ""@kibana-highlighted-field@Daklak_Webs@/kibana-highlighted-field@""
          ]
        },
        ""sort"": [
          -9223372036854776000
        ]
      },
      {
        ""_index"": ""wazuh-monitoring-3.x-2020.05.24"",
        ""_type"": ""_doc"",
        ""_id"": ""7REERnIBENNuuStZh8PT"",
        ""_version"": 1,
        ""_score"": null,
        ""_source"": {
          ""os"": {
            ""arch"": ""x86_64"",
            ""major"": ""6"",
            ""minor"": ""10"",
            ""name"": ""CentOS Linux"",
            ""platform"": ""centos"",
            ""uname"": ""Linux |werserver |2.6.32-754.11.1.el6.x86_64 |#1 SMP Tue Feb 26 15:38:56 UTC 2019 |x86_64"",
            ""version"": ""6.10""
          },
          ""mergedSum"": ""f69c8a56d2cd8b401643a52cf423ad1c"",
          ""manager"": ""vsec-logstash"",
          ""id"": ""060"",
          ""status"": ""Active"",
          ""registerIP"": ""any"",
          ""configSum"": ""4b54b212c2dd96f8b25aebfea2a104cd"",
          ""dateAdd"": ""2020-03-27 07:47:46"",
          ""ip"": ""192.168.2.14"",
          ""lastKeepAlive"": ""2020-05-24 09:29:48"",
          ""version"": ""Wazuh v3.11.4"",
          ""name"": ""werserver"",
          ""node_name"": ""node02"",
          ""group"": [
            ""Daklak"",
            ""Test"",
            ""VSEC"",
            ""Daklak_Webs"",
            ""Daklak_Apps""
          ],
          ""timestamp"": ""2020-05-24T09:30:01.553Z"",
          ""host"": ""vsec-logstash"",
          ""cluster"": {
            ""name"": ""wazuh""
          }
        },
        ""fields"": {
          ""timestamp"": [
            ""2020-05-24T09:30:01.553Z""
          ]
        },
        ""highlight"": {
          ""group"": [
            ""@kibana-highlighted-field@Daklak@/kibana-highlighted-field@"",
            ""@kibana-highlighted-field@Daklak_Webs@/kibana-highlighted-field@""
          ]
        },
        ""sort"": [
          -9223372036854776000
        ]
      },
      {
        ""_index"": ""wazuh-monitoring-3.x-2020.05.24"",
        ""_type"": ""_doc"",
        ""_id"": ""7hEERnIBENNuuStZh8PT"",
        ""_version"": 1,
        ""_score"": null,
        ""_source"": {
          ""os"": {
            ""arch"": ""x86_64"",
            ""major"": ""6"",
            ""minor"": ""8"",
            ""name"": ""CentOS Linux"",
            ""platform"": ""centos"",
            ""uname"": ""Linux |webserver18 |2.6.32-642.el6.x86_64 |#1 SMP Tue May 10 17:27:01 UTC 2016 |x86_64"",
            ""version"": ""6.8""
          },
          ""mergedSum"": ""535237cb82b543ec180a59bde6425db5"",
          ""manager"": ""vsec-logstash"",
          ""id"": ""061"",
          ""status"": ""Active"",
          ""registerIP"": ""any"",
          ""configSum"": ""d149ab60df71e6fb53b2d38db75bc733"",
          ""dateAdd"": ""2020-03-31 02:15:08"",
          ""ip"": ""192.168.2.18"",
          ""lastKeepAlive"": ""2020-05-24 09:29:47"",
          ""version"": ""Wazuh v3.11.4"",
          ""name"": ""webserver18"",
          ""node_name"": ""node02"",
          ""group"": [
            ""Daklak"",
            ""Test"",
            ""VSEC"",
            ""Daklak_Webs""
          ],
          ""timestamp"": ""2020-05-24T09:30:01.553Z"",
          ""host"": ""vsec-logstash"",
          ""cluster"": {
            ""name"": ""wazuh""
          }
        },
        ""fields"": {
          ""timestamp"": [
            ""2020-05-24T09:30:01.553Z""
          ]
        },
        ""highlight"": {
          ""group"": [
            ""@kibana-highlighted-field@Daklak@/kibana-highlighted-field@"",
            ""@kibana-highlighted-field@Daklak_Webs@/kibana-highlighted-field@""
          ]
        },
        ""sort"": [
          -9223372036854776000
        ]
      },
      {
        ""_index"": ""wazuh-monitoring-3.x-2020.05.24"",
        ""_type"": ""_doc"",
        ""_id"": ""oRMtRnIBENNuuStZvON3"",
        ""_version"": 1,
        ""_score"": null,
        ""_source"": {
          ""os"": {
            ""arch"": ""x86_64"",
            ""major"": ""6"",
            ""minor"": ""8"",
            ""name"": ""CentOS Linux"",
            ""platform"": ""centos"",
            ""uname"": ""Linux |webserver18 |2.6.32-642.el6.x86_64 |#1 SMP Tue May 10 17:27:01 UTC 2016 |x86_64"",
            ""version"": ""6.8""
          },
          ""registerIP"": ""any"",
          ""name"": ""webserver18"",
          ""manager"": ""vsec-logstash"",
          ""version"": ""Wazuh v3.11.4"",
          ""lastKeepAlive"": ""2020-05-24 10:14:51"",
          ""status"": ""Active"",
          ""id"": ""061"",
          ""group"": [
            ""Daklak"",
            ""Test"",
            ""VSEC"",
            ""Daklak_Webs""
          ],
          ""mergedSum"": ""535237cb82b543ec180a59bde6425db5"",
          ""dateAdd"": ""2020-03-31 02:15:08"",
          ""ip"": ""192.168.2.18"",
          ""node_name"": ""node02"",
          ""configSum"": ""d149ab60df71e6fb53b2d38db75bc733"",
          ""timestamp"": ""2020-05-24T10:15:02.009Z"",
          ""host"": ""vsec-logstash"",
          ""cluster"": {
            ""name"": ""wazuh""
          }
        },
        ""fields"": {
          ""timestamp"": [
            ""2020-05-24T10:15:02.009Z""
          ]
        },
        ""highlight"": {
          ""group"": [
            ""@kibana-highlighted-field@Daklak@/kibana-highlighted-field@"",
            ""@kibana-highlighted-field@Daklak_Webs@/kibana-highlighted-field@""
          ]
        },
        ""sort"": [
          -9223372036854776000
        ]
      },
      {
        ""_index"": ""wazuh-monitoring-3.x-2020.05.24"",
        ""_type"": ""_doc"",
        ""_id"": ""8RMfRnIBENNuuStZ_iej"",
        ""_version"": 1,
        ""_score"": null,
        ""_source"": {
          ""os"": {
            ""arch"": ""x86_64"",
            ""major"": ""6"",
            ""minor"": ""8"",
            ""name"": ""CentOS Linux"",
            ""platform"": ""centos"",
            ""uname"": ""Linux |webserver18 |2.6.32-642.el6.x86_64 |#1 SMP Tue May 10 17:27:01 UTC 2016 |x86_64"",
            ""version"": ""6.8""
          },
          ""manager"": ""vsec-logstash"",
          ""status"": ""Active"",
          ""group"": [
            ""Daklak"",
            ""Test"",
            ""VSEC"",
            ""Daklak_Webs""
          ],
          ""mergedSum"": ""535237cb82b543ec180a59bde6425db5"",
          ""node_name"": ""node02"",
          ""version"": ""Wazuh v3.11.4"",
          ""name"": ""webserver18"",
          ""dateAdd"": ""2020-03-31 02:15:08"",
          ""id"": ""061"",
          ""registerIP"": ""any"",
          ""lastKeepAlive"": ""2020-05-24 09:59:51"",
          ""ip"": ""192.168.2.18"",
          ""configSum"": ""d149ab60df71e6fb53b2d38db75bc733"",
          ""timestamp"": ""2020-05-24T10:00:01.442Z"",
          ""host"": ""vsec-logstash"",
          ""cluster"": {
            ""name"": ""wazuh""
          }
        },
        ""fields"": {
          ""timestamp"": [
            ""2020-05-24T10:00:01.442Z""
          ]
        },
        ""highlight"": {
          ""group"": [
            ""@kibana-highlighted-field@Daklak@/kibana-highlighted-field@"",
            ""@kibana-highlighted-field@Daklak_Webs@/kibana-highlighted-field@""
          ]
        },
        ""sort"": [
          -9223372036854776000
        ]
      },
      {
        ""_index"": ""wazuh-monitoring-3.x-2020.05.24"",
        ""_type"": ""_doc"",
        ""_id"": ""LiRpR3IBENNuuStZlw1q"",
        ""_version"": 1,
        ""_score"": null,
        ""_source"": {
          ""os"": {
            ""arch"": ""x86_64"",
            ""major"": ""6"",
            ""minor"": ""10"",
            ""name"": ""CentOS Linux"",
            ""platform"": ""centos"",
            ""uname"": ""Linux |werserver |2.6.32-754.11.1.el6.x86_64 |#1 SMP Tue Feb 26 15:38:56 UTC 2019 |x86_64"",
            ""version"": ""6.10""
          },
          ""group"": [
            ""Daklak"",
            ""Test"",
            ""VSEC"",
            ""Daklak_Webs"",
            ""Daklak_Apps""
          ],
          ""node_name"": ""node02"",
          ""ip"": ""192.168.2.14"",
          ""status"": ""Active"",
          ""version"": ""Wazuh v3.11.4"",
          ""dateAdd"": ""2020-03-27 07:47:46"",
          ""id"": ""060"",
          ""manager"": ""vsec-logstash"",
          ""registerIP"": ""any"",
          ""mergedSum"": ""f69c8a56d2cd8b401643a52cf423ad1c"",
          ""lastKeepAlive"": ""2020-05-24 15:59:50"",
          ""configSum"": ""4b54b212c2dd96f8b25aebfea2a104cd"",
          ""name"": ""werserver"",
          ""timestamp"": ""2020-05-24T16:00:01.895Z"",
          ""host"": ""vsec-logstash"",
          ""cluster"": {
            ""name"": ""wazuh""
          }
        },
        ""fields"": {
          ""timestamp"": [
            ""2020-05-24T16:00:01.895Z""
          ]
        },
        ""highlight"": {
          ""group"": [
            ""@kibana-highlighted-field@Daklak@/kibana-highlighted-field@"",
            ""@kibana-highlighted-field@Daklak_Webs@/kibana-highlighted-field@""
          ]
        },
        ""sort"": [
          -9223372036854776000
        ]
      },
      {
        ""_index"": ""wazuh-monitoring-3.x-2020.05.24"",
        ""_type"": ""_doc"",
        ""_id"": ""LyRpR3IBENNuuStZlw1q"",
        ""_version"": 1,
        ""_score"": null,
        ""_source"": {
          ""os"": {
            ""arch"": ""x86_64"",
            ""major"": ""6"",
            ""minor"": ""8"",
            ""name"": ""CentOS Linux"",
            ""platform"": ""centos"",
            ""uname"": ""Linux |webserver18 |2.6.32-642.el6.x86_64 |#1 SMP Tue May 10 17:27:01 UTC 2016 |x86_64"",
            ""version"": ""6.8""
          },
          ""group"": [
            ""Daklak"",
            ""Test"",
            ""VSEC"",
            ""Daklak_Webs""
          ],
          ""node_name"": ""node02"",
          ""ip"": ""192.168.2.18"",
          ""status"": ""Active"",
          ""version"": ""Wazuh v3.11.4"",
          ""dateAdd"": ""2020-03-31 02:15:08"",
          ""id"": ""061"",
          ""manager"": ""vsec-logstash"",
          ""registerIP"": ""any"",
          ""mergedSum"": ""535237cb82b543ec180a59bde6425db5"",
          ""lastKeepAlive"": ""2020-05-24 15:59:51"",
          ""configSum"": ""d149ab60df71e6fb53b2d38db75bc733"",
          ""name"": ""webserver18"",
          ""timestamp"": ""2020-05-24T16:00:01.895Z"",
          ""host"": ""vsec-logstash"",
          ""cluster"": {
            ""name"": ""wazuh""
          }
        },
        ""fields"": {
          ""timestamp"": [
            ""2020-05-24T16:00:01.895Z""
          ]
        },
        ""highlight"": {
          ""group"": [
            ""@kibana-highlighted-field@Daklak@/kibana-highlighted-field@"",
            ""@kibana-highlighted-field@Daklak_Webs@/kibana-highlighted-field@""
          ]
        },
        ""sort"": [
          -9223372036854776000
        ]
      },
      {
        ""_index"": ""wazuh-monitoring-3.x-2020.05.24"",
        ""_type"": ""_doc"",
        ""_id"": ""ZCNbR3IBENNuuStZ3Vuh"",
        ""_version"": 1,
        ""_score"": null,
        ""_source"": {
          ""os"": {
            ""arch"": ""x86_64"",
            ""major"": ""6"",
            ""minor"": ""10"",
            ""name"": ""CentOS Linux"",
            ""platform"": ""centos"",
            ""uname"": ""Linux |werserver |2.6.32-754.11.1.el6.x86_64 |#1 SMP Tue Feb 26 15:38:56 UTC 2019 |x86_64"",
            ""version"": ""6.10""
          },
          ""dateAdd"": ""2020-03-27 07:47:46"",
          ""status"": ""Active"",
          ""ip"": ""192.168.2.14"",
          ""lastKeepAlive"": ""2020-05-24 15:44:49"",
          ""version"": ""Wazuh v3.11.4"",
          ""node_name"": ""node02"",
          ""registerIP"": ""any"",
          ""configSum"": ""4b54b212c2dd96f8b25aebfea2a104cd"",
          ""manager"": ""vsec-logstash"",
          ""mergedSum"": ""f69c8a56d2cd8b401643a52cf423ad1c"",
          ""id"": ""060"",
          ""name"": ""werserver"",
          ""group"": [
            ""Daklak"",
            ""Test"",
            ""VSEC"",
            ""Daklak_Webs"",
            ""Daklak_Apps""
          ],
          ""timestamp"": ""2020-05-24T15:45:02.367Z"",
          ""host"": ""vsec-logstash"",
          ""cluster"": {
            ""name"": ""wazuh""
          }
        },
        ""fields"": {
          ""timestamp"": [
            ""2020-05-24T15:45:02.367Z""
          ]
        },
        ""highlight"": {
          ""group"": [
            ""@kibana-highlighted-field@Daklak@/kibana-highlighted-field@"",
            ""@kibana-highlighted-field@Daklak_Webs@/kibana-highlighted-field@""
          ]
        },
        ""sort"": [
          -9223372036854776000
        ]
      },
      {
        ""_index"": ""wazuh-monitoring-3.x-2020.05.24"",
        ""_type"": ""_doc"",
        ""_id"": ""xyFAR3IBENNuuStZYviz"",
        ""_version"": 1,
        ""_score"": null,
        ""_source"": {
          ""os"": {
            ""arch"": ""x86_64"",
            ""major"": ""6"",
            ""minor"": ""10"",
            ""name"": ""CentOS Linux"",
            ""platform"": ""centos"",
            ""uname"": ""Linux |werserver |2.6.32-754.11.1.el6.x86_64 |#1 SMP Tue Feb 26 15:38:56 UTC 2019 |x86_64"",
            ""version"": ""6.10""
          },
          ""ip"": ""192.168.2.14"",
          ""status"": ""Active"",
          ""version"": ""Wazuh v3.11.4"",
          ""registerIP"": ""any"",
          ""node_name"": ""node02"",
          ""configSum"": ""4b54b212c2dd96f8b25aebfea2a104cd"",
          ""name"": ""werserver"",
          ""group"": [
            ""Daklak"",
            ""Test"",
            ""VSEC"",
            ""Daklak_Webs"",
            ""Daklak_Apps""
          ],
          ""mergedSum"": ""f69c8a56d2cd8b401643a52cf423ad1c"",
          ""id"": ""060"",
          ""manager"": ""vsec-logstash"",
          ""dateAdd"": ""2020-03-27 07:47:46"",
          ""lastKeepAlive"": ""2020-05-24 15:14:49"",
          ""timestamp"": ""2020-05-24T15:15:01.429Z"",
          ""host"": ""vsec-logstash"",
          ""cluster"": {
            ""name"": ""wazuh""
          }
        },
        ""fields"": {
          ""timestamp"": [
            ""2020-05-24T15:15:01.429Z""
          ]
        },
        ""highlight"": {
          ""group"": [
            ""@kibana-highlighted-field@Daklak@/kibana-highlighted-field@"",
            ""@kibana-highlighted-field@Daklak_Webs@/kibana-highlighted-field@""
          ]
        },
        ""sort"": [
          -9223372036854776000
        ]
      },
      {
        ""_index"": ""wazuh-monitoring-3.x-2020.05.24"",
        ""_type"": ""_doc"",
        ""_id"": ""yCFAR3IBENNuuStZYviz"",
        ""_version"": 1,
        ""_score"": null,
        ""_source"": {
          ""os"": {
            ""arch"": ""x86_64"",
            ""major"": ""6"",
            ""minor"": ""8"",
            ""name"": ""CentOS Linux"",
            ""platform"": ""centos"",
            ""uname"": ""Linux |webserver18 |2.6.32-642.el6.x86_64 |#1 SMP Tue May 10 17:27:01 UTC 2016 |x86_64"",
            ""version"": ""6.8""
          },
          ""ip"": ""192.168.2.18"",
          ""status"": ""Active"",
          ""version"": ""Wazuh v3.11.4"",
          ""registerIP"": ""any"",
          ""node_name"": ""node02"",
          ""configSum"": ""d149ab60df71e6fb53b2d38db75bc733"",
          ""name"": ""webserver18"",
          ""group"": [
            ""Daklak"",
            ""Test"",
            ""VSEC"",
            ""Daklak_Webs""
          ],
          ""mergedSum"": ""535237cb82b543ec180a59bde6425db5"",
          ""id"": ""061"",
          ""manager"": ""vsec-logstash"",
          ""dateAdd"": ""2020-03-31 02:15:08"",
          ""lastKeepAlive"": ""2020-05-24 15:14:51"",
          ""timestamp"": ""2020-05-24T15:15:01.429Z"",
          ""host"": ""vsec-logstash"",
          ""cluster"": {
            ""name"": ""wazuh""
          }
        },
        ""fields"": {
          ""timestamp"": [
            ""2020-05-24T15:15:01.429Z""
          ]
        },
        ""highlight"": {
          ""group"": [
            ""@kibana-highlighted-field@Daklak@/kibana-highlighted-field@"",
            ""@kibana-highlighted-field@Daklak_Webs@/kibana-highlighted-field@""
          ]
        },
        ""sort"": [
          -9223372036854776000
        ]
      },
      {
        ""_index"": ""wazuh-monitoring-3.x-2020.05.24"",
        ""_type"": ""_doc"",
        ""_id"": ""VyJOR3IBENNuuStZIKiy"",
        ""_version"": 1,
        ""_score"": null,
        ""_source"": {
          ""os"": {
            ""arch"": ""x86_64"",
            ""major"": ""6"",
            ""minor"": ""10"",
            ""name"": ""CentOS Linux"",
            ""platform"": ""centos"",
            ""uname"": ""Linux |werserver |2.6.32-754.11.1.el6.x86_64 |#1 SMP Tue Feb 26 15:38:56 UTC 2019 |x86_64"",
            ""version"": ""6.10""
          },
          ""dateAdd"": ""2020-03-27 07:47:46"",
          ""id"": ""060"",
          ""status"": ""Active"",
          ""manager"": ""vsec-logstash"",
          ""version"": ""Wazuh v3.11.4"",
          ""mergedSum"": ""f69c8a56d2cd8b401643a52cf423ad1c"",
          ""lastKeepAlive"": ""2020-05-24 15:29:49"",
          ""group"": [
            ""Daklak"",
            ""Test"",
            ""VSEC"",
            ""Daklak_Webs"",
            ""Daklak_Apps""
          ],
          ""name"": ""werserver"",
          ""ip"": ""192.168.2.14"",
          ""registerIP"": ""any"",
          ""node_name"": ""node02"",
          ""configSum"": ""4b54b212c2dd96f8b25aebfea2a104cd"",
          ""timestamp"": ""2020-05-24T15:30:02.036Z"",
          ""host"": ""vsec-logstash"",
          ""cluster"": {
            ""name"": ""wazuh""
          }
        },
        ""fields"": {
          ""timestamp"": [
            ""2020-05-24T15:30:02.036Z""
          ]
        },
        ""highlight"": {
          ""group"": [
            ""@kibana-highlighted-field@Daklak@/kibana-highlighted-field@"",
            ""@kibana-highlighted-field@Daklak_Webs@/kibana-highlighted-field@""
          ]
        },
        ""sort"": [
          -9223372036854776000
        ]
      },
      {
        ""_index"": ""wazuh-monitoring-3.x-2020.05.24"",
        ""_type"": ""_doc"",
        ""_id"": ""jyEyR3IBENNuuStZqTxw"",
        ""_version"": 1,
        ""_score"": null,
        ""_source"": {
          ""os"": {
            ""arch"": ""x86_64"",
            ""major"": ""6"",
            ""minor"": ""10"",
            ""name"": ""CentOS Linux"",
            ""platform"": ""centos"",
            ""uname"": ""Linux |werserver |2.6.32-754.11.1.el6.x86_64 |#1 SMP Tue Feb 26 15:38:56 UTC 2019 |x86_64"",
            ""version"": ""6.10""
          },
          ""registerIP"": ""any"",
          ""status"": ""Active"",
          ""configSum"": ""4b54b212c2dd96f8b25aebfea2a104cd"",
          ""node_name"": ""node02"",
          ""mergedSum"": ""f69c8a56d2cd8b401643a52cf423ad1c"",
          ""name"": ""werserver"",
          ""id"": ""060"",
          ""dateAdd"": ""2020-03-27 07:47:46"",
          ""version"": ""Wazuh v3.11.4"",
          ""lastKeepAlive"": ""2020-05-24 14:59:49"",
          ""ip"": ""192.168.2.14"",
          ""group"": [
            ""Daklak"",
            ""Test"",
            ""VSEC"",
            ""Daklak_Webs"",
            ""Daklak_Apps""
          ],
          ""manager"": ""vsec-logstash"",
          ""timestamp"": ""2020-05-24T15:00:02.030Z"",
          ""host"": ""vsec-logstash"",
          ""cluster"": {
            ""name"": ""wazuh""
          }
        },
        ""fields"": {
          ""timestamp"": [
            ""2020-05-24T15:00:02.030Z""
          ]
        },
        ""highlight"": {
          ""group"": [
            ""@kibana-highlighted-field@Daklak@/kibana-highlighted-field@"",
            ""@kibana-highlighted-field@Daklak_Webs@/kibana-highlighted-field@""
          ]
        },
        ""sort"": [
          -9223372036854776000
        ]
      },
      {
        ""_index"": ""wazuh-monitoring-3.x-2020.05.24"",
        ""_type"": ""_doc"",
        ""_id"": ""WR8XR3IBENNuuStZMdnN"",
        ""_version"": 1,
        ""_score"": null,
        ""_source"": {
          ""os"": {
            ""arch"": ""x86_64"",
            ""major"": ""6"",
            ""minor"": ""10"",
            ""name"": ""CentOS Linux"",
            ""platform"": ""centos"",
            ""uname"": ""Linux |werserver |2.6.32-754.11.1.el6.x86_64 |#1 SMP Tue Feb 26 15:38:56 UTC 2019 |x86_64"",
            ""version"": ""6.10""
          },
          ""lastKeepAlive"": ""2020-05-24 14:29:58"",
          ""dateAdd"": ""2020-03-27 07:47:46"",
          ""ip"": ""192.168.2.14"",
          ""group"": [
            ""Daklak"",
            ""Test"",
            ""VSEC"",
            ""Daklak_Webs"",
            ""Daklak_Apps""
          ],
          ""id"": ""060"",
          ""version"": ""Wazuh v3.11.4"",
          ""status"": ""Active"",
          ""mergedSum"": ""f69c8a56d2cd8b401643a52cf423ad1c"",
          ""registerIP"": ""any"",
          ""configSum"": ""4b54b212c2dd96f8b25aebfea2a104cd"",
          ""node_name"": ""node02"",
          ""manager"": ""vsec-logstash"",
          ""name"": ""werserver"",
          ""timestamp"": ""2020-05-24T14:30:01.928Z"",
          ""host"": ""vsec-logstash"",
          ""cluster"": {
            ""name"": ""wazuh""
          }
        },
        ""fields"": {
          ""timestamp"": [
            ""2020-05-24T14:30:01.928Z""
          ]
        },
        ""highlight"": {
          ""group"": [
            ""@kibana-highlighted-field@Daklak@/kibana-highlighted-field@"",
            ""@kibana-highlighted-field@Daklak_Webs@/kibana-highlighted-field@""
          ]
        },
        ""sort"": [
          -9223372036854776000
        ]
      },
      {
        ""_index"": ""wazuh-monitoring-3.x-2020.05.24"",
        ""_type"": ""_doc"",
        ""_id"": ""Wh8XR3IBENNuuStZMdnN"",
        ""_version"": 1,
        ""_score"": null,
        ""_source"": {
          ""os"": {
            ""arch"": ""x86_64"",
            ""major"": ""6"",
            ""minor"": ""8"",
            ""name"": ""CentOS Linux"",
            ""platform"": ""centos"",
            ""uname"": ""Linux |webserver18 |2.6.32-642.el6.x86_64 |#1 SMP Tue May 10 17:27:01 UTC 2016 |x86_64"",
            ""version"": ""6.8""
          },
          ""lastKeepAlive"": ""2020-05-24 14:29:51"",
          ""dateAdd"": ""2020-03-31 02:15:08"",
          ""ip"": ""192.168.2.18"",
          ""group"": [
            ""Daklak"",
            ""Test"",
            ""VSEC"",
            ""Daklak_Webs""
          ],
          ""id"": ""061"",
          ""version"": ""Wazuh v3.11.4"",
          ""status"": ""Active"",
          ""mergedSum"": ""535237cb82b543ec180a59bde6425db5"",
          ""registerIP"": ""any"",
          ""configSum"": ""d149ab60df71e6fb53b2d38db75bc733"",
          ""node_name"": ""node02"",
          ""manager"": ""vsec-logstash"",
          ""name"": ""webserver18"",
          ""timestamp"": ""2020-05-24T14:30:01.928Z"",
          ""host"": ""vsec-logstash"",
          ""cluster"": {
            ""name"": ""wazuh""
          }
        },
        ""fields"": {
          ""timestamp"": [
            ""2020-05-24T14:30:01.928Z""
          ]
        },
        ""highlight"": {
          ""group"": [
            ""@kibana-highlighted-field@Daklak@/kibana-highlighted-field@"",
            ""@kibana-highlighted-field@Daklak_Webs@/kibana-highlighted-field@""
          ]
        },
        ""sort"": [
          -9223372036854776000
        ]
      },
      {
        ""_index"": ""wazuh-monitoring-3.x-2020.05.24"",
        ""_type"": ""_doc"",
        ""_id"": ""hy9FSHIBENNuuStZUUx2"",
        ""_version"": 1,
        ""_score"": null,
        ""_source"": {
          ""os"": {
            ""arch"": ""x86_64"",
            ""major"": ""6"",
            ""minor"": ""8"",
            ""name"": ""CentOS Linux"",
            ""platform"": ""centos"",
            ""uname"": ""Linux |webserver18 |2.6.32-642.el6.x86_64 |#1 SMP Tue May 10 17:27:01 UTC 2016 |x86_64"",
            ""version"": ""6.8""
          },
          ""dateAdd"": ""2020-03-31 02:15:08"",
          ""id"": ""061"",
          ""group"": [
            ""Daklak"",
            ""Test"",
            ""VSEC"",
            ""Daklak_Webs""
          ],
          ""node_name"": ""node02"",
          ""registerIP"": ""any"",
          ""lastKeepAlive"": ""2020-05-24 19:59:53"",
          ""manager"": ""vsec-logstash"",
          ""mergedSum"": ""535237cb82b543ec180a59bde6425db5"",
          ""name"": ""webserver18"",
          ""version"": ""Wazuh v3.11.4"",
          ""configSum"": ""d149ab60df71e6fb53b2d38db75bc733"",
          ""ip"": ""192.168.2.18"",
          ""status"": ""Active"",
          ""timestamp"": ""2020-05-24T20:00:01.911Z"",
          ""host"": ""vsec-logstash"",
          ""cluster"": {
            ""name"": ""wazuh""
          }
        },
        ""fields"": {
          ""timestamp"": [
            ""2020-05-24T20:00:01.911Z""
          ]
        },
        ""highlight"": {
          ""group"": [
            ""@kibana-highlighted-field@Daklak@/kibana-highlighted-field@"",
            ""@kibana-highlighted-field@Daklak_Webs@/kibana-highlighted-field@""
          ]
        },
        ""sort"": [
          -9223372036854776000
        ]
      },
      {
        ""_index"": ""wazuh-monitoring-3.x-2020.05.24"",
        ""_type"": ""_doc"",
        ""_id"": ""LywOSHIBENNuuStZYn_7"",
        ""_version"": 1,
        ""_score"": null,
        ""_source"": {
          ""os"": {
            ""arch"": ""x86_64"",
            ""major"": ""6"",
            ""minor"": ""8"",
            ""name"": ""CentOS Linux"",
            ""platform"": ""centos"",
            ""uname"": ""Linux |webserver18 |2.6.32-642.el6.x86_64 |#1 SMP Tue May 10 17:27:01 UTC 2016 |x86_64"",
            ""version"": ""6.8""
          },
          ""configSum"": ""d149ab60df71e6fb53b2d38db75bc733"",
          ""registerIP"": ""any"",
          ""id"": ""061"",
          ""name"": ""webserver18"",
          ""lastKeepAlive"": ""2020-05-24 18:59:54"",
          ""status"": ""Active"",
          ""ip"": ""192.168.2.18"",
          ""dateAdd"": ""2020-03-31 02:15:08"",
          ""manager"": ""vsec-logstash"",
          ""node_name"": ""node02"",
          ""version"": ""Wazuh v3.11.4"",
          ""mergedSum"": ""535237cb82b543ec180a59bde6425db5"",
          ""group"": [
            ""Daklak"",
            ""Test"",
            ""VSEC"",
            ""Daklak_Webs""
          ],
          ""timestamp"": ""2020-05-24T19:00:01.913Z"",
          ""host"": ""vsec-logstash"",
          ""cluster"": {
            ""name"": ""wazuh""
          }
        },
        ""fields"": {
          ""timestamp"": [
            ""2020-05-24T19:00:01.913Z""
          ]
        },
        ""highlight"": {
          ""group"": [
            ""@kibana-highlighted-field@Daklak@/kibana-highlighted-field@"",
            ""@kibana-highlighted-field@Daklak_Webs@/kibana-highlighted-field@""
          ]
        },
        ""sort"": [
          -9223372036854776000
        ]
      },
      {
        ""_index"": ""wazuh-monitoring-3.x-2020.05.24"",
        ""_type"": ""_doc"",
        ""_id"": ""0S43SHIBENNuuStZk5nZ"",
        ""_version"": 1,
        ""_score"": null,
        ""_source"": {
          ""os"": {
            ""arch"": ""x86_64"",
            ""major"": ""6"",
            ""minor"": ""10"",
            ""name"": ""CentOS Linux"",
            ""platform"": ""centos"",
            ""uname"": ""Linux |werserver |2.6.32-754.11.1.el6.x86_64 |#1 SMP Tue Feb 26 15:38:56 UTC 2019 |x86_64"",
            ""version"": ""6.10""
          },
          ""mergedSum"": ""f69c8a56d2cd8b401643a52cf423ad1c"",
          ""lastKeepAlive"": ""2020-05-24 19:44:51"",
          ""manager"": ""vsec-logstash"",
          ""node_name"": ""node02"",
          ""registerIP"": ""any"",
          ""group"": [
            ""Daklak"",
            ""Test"",
            ""VSEC"",
            ""Daklak_Webs"",
            ""Daklak_Apps""
          ],
          ""configSum"": ""4b54b212c2dd96f8b25aebfea2a104cd"",
          ""version"": ""Wazuh v3.11.4"",
          ""dateAdd"": ""2020-03-27 07:47:46"",
          ""name"": ""werserver"",
          ""id"": ""060"",
          ""ip"": ""192.168.2.14"",
          ""status"": ""Active"",
          ""timestamp"": ""2020-05-24T19:45:01.397Z"",
          ""host"": ""vsec-logstash"",
          ""cluster"": {
            ""name"": ""wazuh""
          }
        },
        ""fields"": {
          ""timestamp"": [
            ""2020-05-24T19:45:01.397Z""
          ]
        },
        ""highlight"": {
          ""group"": [
            ""@kibana-highlighted-field@Daklak@/kibana-highlighted-field@"",
            ""@kibana-highlighted-field@Daklak_Webs@/kibana-highlighted-field@""
          ]
        },
        ""sort"": [
          -9223372036854776000
        ]
      },
      {
        ""_index"": ""wazuh-monitoring-3.x-2020.05.24"",
        ""_type"": ""_doc"",
        ""_id"": ""0i43SHIBENNuuStZk5nZ"",
        ""_version"": 1,
        ""_score"": null,
        ""_source"": {
          ""os"": {
            ""arch"": ""x86_64"",
            ""major"": ""6"",
            ""minor"": ""8"",
            ""name"": ""CentOS Linux"",
            ""platform"": ""centos"",
            ""uname"": ""Linux |webserver18 |2.6.32-642.el6.x86_64 |#1 SMP Tue May 10 17:27:01 UTC 2016 |x86_64"",
            ""version"": ""6.8""
          },
          ""mergedSum"": ""535237cb82b543ec180a59bde6425db5"",
          ""lastKeepAlive"": ""2020-05-24 19:44:53"",
          ""manager"": ""vsec-logstash"",
          ""node_name"": ""node02"",
          ""registerIP"": ""any"",
          ""group"": [
            ""Daklak"",
            ""Test"",
            ""VSEC"",
            ""Daklak_Webs""
          ],
          ""configSum"": ""d149ab60df71e6fb53b2d38db75bc733"",
          ""version"": ""Wazuh v3.11.4"",
          ""dateAdd"": ""2020-03-31 02:15:08"",
          ""name"": ""webserver18"",
          ""id"": ""061"",
          ""ip"": ""192.168.2.18"",
          ""status"": ""Active"",
          ""timestamp"": ""2020-05-24T19:45:01.397Z"",
          ""host"": ""vsec-logstash"",
          ""cluster"": {
            ""name"": ""wazuh""
          }
        },
        ""fields"": {
          ""timestamp"": [
            ""2020-05-24T19:45:01.397Z""
          ]
        },
        ""highlight"": {
          ""group"": [
            ""@kibana-highlighted-field@Daklak@/kibana-highlighted-field@"",
            ""@kibana-highlighted-field@Daklak_Webs@/kibana-highlighted-field@""
          ]
        },
        ""sort"": [
          -9223372036854776000
        ]
      },
      {
        ""_index"": ""wazuh-monitoring-3.x-2020.05.24"",
        ""_type"": ""_doc"",
        ""_id"": ""9C0cSHIBENNuuStZHDnx"",
        ""_version"": 1,
        ""_score"": null,
        ""_source"": {
          ""os"": {
            ""arch"": ""x86_64"",
            ""major"": ""6"",
            ""minor"": ""10"",
            ""name"": ""CentOS Linux"",
            ""platform"": ""centos"",
            ""uname"": ""Linux |werserver |2.6.32-754.11.1.el6.x86_64 |#1 SMP Tue Feb 26 15:38:56 UTC 2019 |x86_64"",
            ""version"": ""6.10""
          },
          ""lastKeepAlive"": ""2020-05-24 19:14:51"",
          ""registerIP"": ""any"",
          ""node_name"": ""node02"",
          ""configSum"": ""4b54b212c2dd96f8b25aebfea2a104cd"",
          ""manager"": ""vsec-logstash"",
          ""group"": [
            ""Daklak"",
            ""Test"",
            ""VSEC"",
            ""Daklak_Webs"",
            ""Daklak_Apps""
          ],
          ""dateAdd"": ""2020-03-27 07:47:46"",
          ""id"": ""060"",
          ""ip"": ""192.168.2.14"",
          ""version"": ""Wazuh v3.11.4"",
          ""name"": ""werserver"",
          ""mergedSum"": ""f69c8a56d2cd8b401643a52cf423ad1c"",
          ""status"": ""Active"",
          ""timestamp"": ""2020-05-24T19:15:01.483Z"",
          ""host"": ""vsec-logstash"",
          ""cluster"": {
            ""name"": ""wazuh""
          }
        },
        ""fields"": {
          ""timestamp"": [
            ""2020-05-24T19:15:01.483Z""
          ]
        },
        ""highlight"": {
          ""group"": [
            ""@kibana-highlighted-field@Daklak@/kibana-highlighted-field@"",
            ""@kibana-highlighted-field@Daklak_Webs@/kibana-highlighted-field@""
          ]
        },
        ""sort"": [
          -9223372036854776000
        ]
      },
      {
        ""_index"": ""wazuh-monitoring-3.x-2020.05.24"",
        ""_type"": ""_doc"",
        ""_id"": ""9S0cSHIBENNuuStZHDnx"",
        ""_version"": 1,
        ""_score"": null,
        ""_source"": {
          ""os"": {
            ""arch"": ""x86_64"",
            ""major"": ""6"",
            ""minor"": ""8"",
            ""name"": ""CentOS Linux"",
            ""platform"": ""centos"",
            ""uname"": ""Linux |webserver18 |2.6.32-642.el6.x86_64 |#1 SMP Tue May 10 17:27:01 UTC 2016 |x86_64"",
            ""version"": ""6.8""
          },
          ""lastKeepAlive"": ""2020-05-24 19:14:54"",
          ""registerIP"": ""any"",
          ""node_name"": ""node02"",
          ""configSum"": ""d149ab60df71e6fb53b2d38db75bc733"",
          ""manager"": ""vsec-logstash"",
          ""group"": [
            ""Daklak"",
            ""Test"",
            ""VSEC"",
            ""Daklak_Webs""
          ],
          ""dateAdd"": ""2020-03-31 02:15:08"",
          ""id"": ""061"",
          ""ip"": ""192.168.2.18"",
          ""version"": ""Wazuh v3.11.4"",
          ""name"": ""webserver18"",
          ""mergedSum"": ""535237cb82b543ec180a59bde6425db5"",
          ""status"": ""Active"",
          ""timestamp"": ""2020-05-24T19:15:01.483Z"",
          ""host"": ""vsec-logstash"",
          ""cluster"": {
            ""name"": ""wazuh""
          }
        },
        ""fields"": {
          ""timestamp"": [
            ""2020-05-24T19:15:01.483Z""
          ]
        },
        ""highlight"": {
          ""group"": [
            ""@kibana-highlighted-field@Daklak@/kibana-highlighted-field@"",
            ""@kibana-highlighted-field@Daklak_Webs@/kibana-highlighted-field@""
          ]
        },
        ""sort"": [
          -9223372036854776000
        ]
      },
      {
        ""_index"": ""wazuh-monitoring-3.x-2020.05.24"",
        ""_type"": ""_doc"",
        ""_id"": ""UjBgSHIBENNuuStZyLe_"",
        ""_version"": 1,
        ""_score"": null,
        ""_source"": {
          ""os"": {
            ""arch"": ""x86_64"",
            ""major"": ""6"",
            ""minor"": ""10"",
            ""name"": ""CentOS Linux"",
            ""platform"": ""centos"",
            ""uname"": ""Linux |werserver |2.6.32-754.11.1.el6.x86_64 |#1 SMP Tue Feb 26 15:38:56 UTC 2019 |x86_64"",
            ""version"": ""6.10""
          },
          ""configSum"": ""4b54b212c2dd96f8b25aebfea2a104cd"",
          ""registerIP"": ""any"",
          ""node_name"": ""node02"",
          ""name"": ""werserver"",
          ""status"": ""Active"",
          ""id"": ""060"",
          ""dateAdd"": ""2020-03-27 07:47:46"",
          ""version"": ""Wazuh v3.11.4"",
          ""group"": [
            ""Daklak"",
            ""Test"",
            ""VSEC"",
            ""Daklak_Webs"",
            ""Daklak_Apps""
          ],
          ""ip"": ""192.168.2.14"",
          ""mergedSum"": ""f69c8a56d2cd8b401643a52cf423ad1c"",
          ""lastKeepAlive"": ""2020-05-24 20:29:50"",
          ""manager"": ""vsec-logstash"",
          ""timestamp"": ""2020-05-24T20:30:01.920Z"",
          ""host"": ""vsec-logstash"",
          ""cluster"": {
            ""name"": ""wazuh""
          }
        },
        ""fields"": {
          ""timestamp"": [
            ""2020-05-24T20:30:01.920Z""
          ]
        },
        ""highlight"": {
          ""group"": [
            ""@kibana-highlighted-field@Daklak@/kibana-highlighted-field@"",
            ""@kibana-highlighted-field@Daklak_Webs@/kibana-highlighted-field@""
          ]
        },
        ""sort"": [
          -9223372036854776000
        ]
      },
      {
        ""_index"": ""wazuh-monitoring-3.x-2020.05.24"",
        ""_type"": ""_doc"",
        ""_id"": ""azBTSHIBENNuuStZDwc-"",
        ""_version"": 1,
        ""_score"": null,
        ""_source"": {
          ""os"": {
            ""arch"": ""x86_64"",
            ""major"": ""6"",
            ""minor"": ""8"",
            ""name"": ""CentOS Linux"",
            ""platform"": ""centos"",
            ""uname"": ""Linux |webserver18 |2.6.32-642.el6.x86_64 |#1 SMP Tue May 10 17:27:01 UTC 2016 |x86_64"",
            ""version"": ""6.8""
          },
          ""manager"": ""vsec-logstash"",
          ""name"": ""webserver18"",
          ""id"": ""061"",
          ""lastKeepAlive"": ""2020-05-24 20:14:53"",
          ""registerIP"": ""any"",
          ""configSum"": ""d149ab60df71e6fb53b2d38db75bc733"",
          ""mergedSum"": ""535237cb82b543ec180a59bde6425db5"",
          ""version"": ""Wazuh v3.11.4"",
          ""dateAdd"": ""2020-03-31 02:15:08"",
          ""group"": [
            ""Daklak"",
            ""Test"",
            ""VSEC"",
            ""Daklak_Webs""
          ],
          ""status"": ""Active"",
          ""node_name"": ""node02"",
          ""ip"": ""192.168.2.18"",
          ""timestamp"": ""2020-05-24T20:15:02.463Z"",
          ""host"": ""vsec-logstash"",
          ""cluster"": {
            ""name"": ""wazuh""
          }
        },
        ""fields"": {
          ""timestamp"": [
            ""2020-05-24T20:15:02.463Z""
          ]
        },
        ""highlight"": {
          ""group"": [
            ""@kibana-highlighted-field@Daklak@/kibana-highlighted-field@"",
            ""@kibana-highlighted-field@Daklak_Webs@/kibana-highlighted-field@""
          ]
        },
        ""sort"": [
          -9223372036854776000
        ]
      },
      {
        ""_index"": ""wazuh-monitoring-3.x-2020.05.24"",
        ""_type"": ""_doc"",
        ""_id"": ""oSsASHIBENNuuStZpc6L"",
        ""_version"": 1,
        ""_score"": null,
        ""_source"": {
          ""os"": {
            ""arch"": ""x86_64"",
            ""major"": ""6"",
            ""minor"": ""8"",
            ""name"": ""CentOS Linux"",
            ""platform"": ""centos"",
            ""uname"": ""Linux |webserver18 |2.6.32-642.el6.x86_64 |#1 SMP Tue May 10 17:27:01 UTC 2016 |x86_64"",
            ""version"": ""6.8""
          },
          ""mergedSum"": ""535237cb82b543ec180a59bde6425db5"",
          ""ip"": ""192.168.2.18"",
          ""name"": ""webserver18"",
          ""lastKeepAlive"": ""2020-05-24 18:44:54"",
          ""group"": [
            ""Daklak"",
            ""Test"",
            ""VSEC"",
            ""Daklak_Webs""
          ],
          ""node_name"": ""node02"",
          ""dateAdd"": ""2020-03-31 02:15:08"",
          ""status"": ""Active"",
          ""registerIP"": ""any"",
          ""id"": ""061"",
          ""version"": ""Wazuh v3.11.4"",
          ""configSum"": ""d149ab60df71e6fb53b2d38db75bc733"",
          ""manager"": ""vsec-logstash"",
          ""timestamp"": ""2020-05-24T18:45:01.452Z"",
          ""host"": ""vsec-logstash"",
          ""cluster"": {
            ""name"": ""wazuh""
          }
        },
        ""fields"": {
          ""timestamp"": [
            ""2020-05-24T18:45:01.452Z""
          ]
        },
        ""highlight"": {
          ""group"": [
            ""@kibana-highlighted-field@Daklak@/kibana-highlighted-field@"",
            ""@kibana-highlighted-field@Daklak_Webs@/kibana-highlighted-field@""
          ]
        },
        ""sort"": [
          -9223372036854776000
        ]
      },
      {
        ""_index"": ""wazuh-monitoring-3.x-2020.05.24"",
        ""_type"": ""_doc"",
        ""_id"": ""-wCtRHIBENNuuStZOCpd"",
        ""_version"": 1,
        ""_score"": null,
        ""_source"": {
          ""os"": {
            ""arch"": ""x86_64"",
            ""major"": ""6"",
            ""minor"": ""10"",
            ""name"": ""CentOS Linux"",
            ""platform"": ""centos"",
            ""uname"": ""Linux |werserver |2.6.32-754.11.1.el6.x86_64 |#1 SMP Tue Feb 26 15:38:56 UTC 2019 |x86_64"",
            ""version"": ""6.10""
          },
          ""manager"": ""vsec-logstash"",
          ""group"": [
            ""Daklak"",
            ""Test"",
            ""VSEC"",
            ""Daklak_Webs"",
            ""Daklak_Apps""
          ],
          ""name"": ""werserver"",
          ""status"": ""Active"",
          ""configSum"": ""4b54b212c2dd96f8b25aebfea2a104cd"",
          ""registerIP"": ""any"",
          ""node_name"": ""node02"",
          ""dateAdd"": ""2020-03-27 07:47:46"",
          ""ip"": ""192.168.2.14"",
          ""mergedSum"": ""f69c8a56d2cd8b401643a52cf423ad1c"",
          ""version"": ""Wazuh v3.11.4"",
          ""id"": ""060"",
          ""lastKeepAlive"": ""2020-05-24 03:14:57"",
          ""timestamp"": ""2020-05-24T03:15:02.359Z"",
          ""host"": ""vsec-logstash"",
          ""cluster"": {
            ""name"": ""wazuh""
          }
        },
        ""fields"": {
          ""timestamp"": [
            ""2020-05-24T03:15:02.359Z""
          ]
        },
        ""highlight"": {
          ""group"": [
            ""@kibana-highlighted-field@Daklak@/kibana-highlighted-field@"",
            ""@kibana-highlighted-field@Daklak_Webs@/kibana-highlighted-field@""
          ]
        },
        ""sort"": [
          -9223372036854776000
        ]
      },
      {
        ""_index"": ""wazuh-monitoring-3.x-2020.05.24"",
        ""_type"": ""_doc"",
        ""_id"": ""_ACtRHIBENNuuStZOCpd"",
        ""_version"": 1,
        ""_score"": null,
        ""_source"": {
          ""os"": {
            ""arch"": ""x86_64"",
            ""major"": ""6"",
            ""minor"": ""8"",
            ""name"": ""CentOS Linux"",
            ""platform"": ""centos"",
            ""uname"": ""Linux |webserver18 |2.6.32-642.el6.x86_64 |#1 SMP Tue May 10 17:27:01 UTC 2016 |x86_64"",
            ""version"": ""6.8""
          },
          ""manager"": ""vsec-logstash"",
          ""group"": [
            ""Daklak"",
            ""Test"",
            ""VSEC"",
            ""Daklak_Webs""
          ],
          ""name"": ""webserver18"",
          ""status"": ""Active"",
          ""configSum"": ""d149ab60df71e6fb53b2d38db75bc733"",
          ""registerIP"": ""any"",
          ""node_name"": ""node02"",
          ""dateAdd"": ""2020-03-31 02:15:08"",
          ""ip"": ""192.168.2.18"",
          ""mergedSum"": ""535237cb82b543ec180a59bde6425db5"",
          ""version"": ""Wazuh v3.11.4"",
          ""id"": ""061"",
          ""lastKeepAlive"": ""2020-05-24 03:14:51"",
          ""timestamp"": ""2020-05-24T03:15:02.359Z"",
          ""host"": ""vsec-logstash"",
          ""cluster"": {
            ""name"": ""wazuh""
          }
        },
        ""fields"": {
          ""timestamp"": [
            ""2020-05-24T03:15:02.359Z""
          ]
        },
        ""highlight"": {
          ""group"": [
            ""@kibana-highlighted-field@Daklak@/kibana-highlighted-field@"",
            ""@kibana-highlighted-field@Daklak_Webs@/kibana-highlighted-field@""
          ]
        },
        ""sort"": [
          -9223372036854776000
        ]
      },
      {
        ""_index"": ""wazuh-monitoring-3.x-2020.05.24"",
        ""_type"": ""_doc"",
        ""_id"": ""gv-fRHIBENNuuStZem-G"",
        ""_version"": 1,
        ""_score"": null,
        ""_source"": {
          ""os"": {
            ""arch"": ""x86_64"",
            ""major"": ""6"",
            ""minor"": ""8"",
            ""name"": ""CentOS Linux"",
            ""platform"": ""centos"",
            ""uname"": ""Linux |webserver18 |2.6.32-642.el6.x86_64 |#1 SMP Tue May 10 17:27:01 UTC 2016 |x86_64"",
            ""version"": ""6.8""
          },
          ""name"": ""webserver18"",
          ""version"": ""Wazuh v3.11.4"",
          ""manager"": ""vsec-logstash"",
          ""status"": ""Active"",
          ""node_name"": ""node02"",
          ""id"": ""061"",
          ""dateAdd"": ""2020-03-31 02:15:08"",
          ""configSum"": ""d149ab60df71e6fb53b2d38db75bc733"",
          ""ip"": ""192.168.2.18"",
          ""lastKeepAlive"": ""2020-05-24 02:59:51"",
          ""mergedSum"": ""535237cb82b543ec180a59bde6425db5"",
          ""registerIP"": ""any"",
          ""group"": [
            ""Daklak"",
            ""Test"",
            ""VSEC"",
            ""Daklak_Webs""
          ],
          ""timestamp"": ""2020-05-24T03:00:01.789Z"",
          ""host"": ""vsec-logstash"",
          ""cluster"": {
            ""name"": ""wazuh""
          }
        },
        ""fields"": {
          ""timestamp"": [
            ""2020-05-24T03:00:01.789Z""
          ]
        },
        ""highlight"": {
          ""group"": [
            ""@kibana-highlighted-field@Daklak@/kibana-highlighted-field@"",
            ""@kibana-highlighted-field@Daklak_Webs@/kibana-highlighted-field@""
          ]
        },
        ""sort"": [
          -9223372036854776000
        ]
      },
      {
        ""_index"": ""wazuh-monitoring-3.x-2020.05.24"",
        ""_type"": ""_doc"",
        ""_id"": ""XQHIRHIBENNuuStZrpDz"",
        ""_version"": 1,
        ""_score"": null,
        ""_source"": {
          ""os"": {
            ""arch"": ""x86_64"",
            ""major"": ""6"",
            ""minor"": ""8"",
            ""name"": ""CentOS Linux"",
            ""platform"": ""centos"",
            ""uname"": ""Linux |webserver18 |2.6.32-642.el6.x86_64 |#1 SMP Tue May 10 17:27:01 UTC 2016 |x86_64"",
            ""version"": ""6.8""
          },
          ""id"": ""061"",
          ""lastKeepAlive"": ""2020-05-24 03:44:44"",
          ""dateAdd"": ""2020-03-31 02:15:08"",
          ""version"": ""Wazuh v3.11.4"",
          ""mergedSum"": ""535237cb82b543ec180a59bde6425db5"",
          ""node_name"": ""node02"",
          ""group"": [
            ""Daklak"",
            ""Test"",
            ""VSEC"",
            ""Daklak_Webs""
          ],
          ""name"": ""webserver18"",
          ""status"": ""Active"",
          ""registerIP"": ""any"",
          ""manager"": ""vsec-logstash"",
          ""configSum"": ""d149ab60df71e6fb53b2d38db75bc733"",
          ""ip"": ""192.168.2.18"",
          ""timestamp"": ""2020-05-24T03:45:02.183Z"",
          ""host"": ""vsec-logstash"",
          ""cluster"": {
            ""name"": ""wazuh""
          }
        },
        ""fields"": {
          ""timestamp"": [
            ""2020-05-24T03:45:02.183Z""
          ]
        },
        ""highlight"": {
          ""group"": [
            ""@kibana-highlighted-field@Daklak@/kibana-highlighted-field@"",
            ""@kibana-highlighted-field@Daklak_Webs@/kibana-highlighted-field@""
          ]
        },
        ""sort"": [
          -9223372036854776000
        ]
      },
      {
        ""_index"": ""wazuh-monitoring-3.x-2020.05.24"",
        ""_type"": ""_doc"",
        ""_id"": ""RwC6RHIBENNuuStZ8t4T"",
        ""_version"": 1,
        ""_score"": null,
        ""_source"": {
          ""os"": {
            ""arch"": ""x86_64"",
            ""major"": ""6"",
            ""minor"": ""10"",
            ""name"": ""CentOS Linux"",
            ""platform"": ""centos"",
            ""uname"": ""Linux |werserver |2.6.32-754.11.1.el6.x86_64 |#1 SMP Tue Feb 26 15:38:56 UTC 2019 |x86_64"",
            ""version"": ""6.10""
          },
          ""name"": ""werserver"",
          ""configSum"": ""4b54b212c2dd96f8b25aebfea2a104cd"",
          ""manager"": ""vsec-logstash"",
          ""dateAdd"": ""2020-03-27 07:47:46"",
          ""node_name"": ""node02"",
          ""mergedSum"": ""f69c8a56d2cd8b401643a52cf423ad1c"",
          ""status"": ""Active"",
          ""group"": [
            ""Daklak"",
            ""Test"",
            ""VSEC"",
            ""Daklak_Webs"",
            ""Daklak_Apps""
          ],
          ""id"": ""060"",
          ""lastKeepAlive"": ""2020-05-24 03:29:48"",
          ""registerIP"": ""any"",
          ""version"": ""Wazuh v3.11.4"",
          ""ip"": ""192.168.2.14"",
          ""timestamp"": ""2020-05-24T03:30:01.868Z"",
          ""host"": ""vsec-logstash"",
          ""cluster"": {
            ""name"": ""wazuh""
          }
        },
        ""fields"": {
          ""timestamp"": [
            ""2020-05-24T03:30:01.868Z""
          ]
        },
        ""highlight"": {
          ""group"": [
            ""@kibana-highlighted-field@Daklak@/kibana-highlighted-field@"",
            ""@kibana-highlighted-field@Daklak_Webs@/kibana-highlighted-field@""
          ]
        },
        ""sort"": [
          -9223372036854776000
        ]
      },
      {
        ""_index"": ""wazuh-monitoring-3.x-2020.05.24"",
        ""_type"": ""_doc"",
        ""_id"": ""YALkRHIBENNuuStZJvvQ"",
        ""_version"": 1,
        ""_score"": null,
        ""_source"": {
          ""os"": {
            ""arch"": ""x86_64"",
            ""major"": ""6"",
            ""minor"": ""10"",
            ""name"": ""CentOS Linux"",
            ""platform"": ""centos"",
            ""uname"": ""Linux |werserver |2.6.32-754.11.1.el6.x86_64 |#1 SMP Tue Feb 26 15:38:56 UTC 2019 |x86_64"",
            ""version"": ""6.10""
          },
          ""node_name"": ""node02"",
          ""status"": ""Active"",
          ""name"": ""werserver"",
          ""ip"": ""192.168.2.14"",
          ""id"": ""060"",
          ""registerIP"": ""any"",
          ""dateAdd"": ""2020-03-27 07:47:46"",
          ""mergedSum"": ""f69c8a56d2cd8b401643a52cf423ad1c"",
          ""configSum"": ""4b54b212c2dd96f8b25aebfea2a104cd"",
          ""manager"": ""vsec-logstash"",
          ""lastKeepAlive"": ""2020-05-24 04:14:47"",
          ""group"": [
            ""Daklak"",
            ""Test"",
            ""VSEC"",
            ""Daklak_Webs"",
            ""Daklak_Apps""
          ],
          ""version"": ""Wazuh v3.11.4"",
          ""timestamp"": ""2020-05-24T04:15:02.347Z"",
          ""host"": ""vsec-logstash"",
          ""cluster"": {
            ""name"": ""wazuh""
          }
        },
        ""fields"": {
          ""timestamp"": [
            ""2020-05-24T04:15:02.347Z""
          ]
        },
        ""highlight"": {
          ""group"": [
            ""@kibana-highlighted-field@Daklak@/kibana-highlighted-field@"",
            ""@kibana-highlighted-field@Daklak_Webs@/kibana-highlighted-field@""
          ]
        },
        ""sort"": [
          -9223372036854776000
        ]
      },
      {
        ""_index"": ""wazuh-monitoring-3.x-2020.05.24"",
        ""_type"": ""_doc"",
        ""_id"": ""YQLkRHIBENNuuStZJvvQ"",
        ""_version"": 1,
        ""_score"": null,
        ""_source"": {
          ""os"": {
            ""arch"": ""x86_64"",
            ""major"": ""6"",
            ""minor"": ""8"",
            ""name"": ""CentOS Linux"",
            ""platform"": ""centos"",
            ""uname"": ""Linux |webserver18 |2.6.32-642.el6.x86_64 |#1 SMP Tue May 10 17:27:01 UTC 2016 |x86_64"",
            ""version"": ""6.8""
          },
          ""node_name"": ""node02"",
          ""status"": ""Active"",
          ""name"": ""webserver18"",
          ""ip"": ""192.168.2.18"",
          ""id"": ""061"",
          ""registerIP"": ""any"",
          ""dateAdd"": ""2020-03-31 02:15:08"",
          ""mergedSum"": ""535237cb82b543ec180a59bde6425db5"",
          ""configSum"": ""d149ab60df71e6fb53b2d38db75bc733"",
          ""manager"": ""vsec-logstash"",
          ""lastKeepAlive"": ""2020-05-24 04:14:54"",
          ""group"": [
            ""Daklak"",
            ""Test"",
            ""VSEC"",
            ""Daklak_Webs""
          ],
          ""version"": ""Wazuh v3.11.4"",
          ""timestamp"": ""2020-05-24T04:15:02.347Z"",
          ""host"": ""vsec-logstash"",
          ""cluster"": {
            ""name"": ""wazuh""
          }
        },
        ""fields"": {
          ""timestamp"": [
            ""2020-05-24T04:15:02.347Z""
          ]
        },
        ""highlight"": {
          ""group"": [
            ""@kibana-highlighted-field@Daklak@/kibana-highlighted-field@"",
            ""@kibana-highlighted-field@Daklak_Webs@/kibana-highlighted-field@""
          ]
        },
        ""sort"": [
          -9223372036854776000
        ]
      },
      {
        ""_index"": ""wazuh-monitoring-3.x-2020.05.24"",
        ""_type"": ""_doc"",
        ""_id"": ""DALWRHIBENNuuStZaUE6"",
        ""_version"": 1,
        ""_score"": null,
        ""_source"": {
          ""os"": {
            ""arch"": ""x86_64"",
            ""major"": ""6"",
            ""minor"": ""8"",
            ""name"": ""CentOS Linux"",
            ""platform"": ""centos"",
            ""uname"": ""Linux |webserver18 |2.6.32-642.el6.x86_64 |#1 SMP Tue May 10 17:27:01 UTC 2016 |x86_64"",
            ""version"": ""6.8""
          },
          ""version"": ""Wazuh v3.11.4"",
          ""status"": ""Active"",
          ""manager"": ""vsec-logstash"",
          ""configSum"": ""d149ab60df71e6fb53b2d38db75bc733"",
          ""node_name"": ""node02"",
          ""name"": ""webserver18"",
          ""group"": [
            ""Daklak"",
            ""Test"",
            ""VSEC"",
            ""Daklak_Webs""
          ],
          ""mergedSum"": ""535237cb82b543ec180a59bde6425db5"",
          ""dateAdd"": ""2020-03-31 02:15:08"",
          ""ip"": ""192.168.2.18"",
          ""registerIP"": ""any"",
          ""id"": ""061"",
          ""lastKeepAlive"": ""2020-05-24 03:59:54"",
          ""timestamp"": ""2020-05-24T04:00:01.841Z"",
          ""host"": ""vsec-logstash"",
          ""cluster"": {
            ""name"": ""wazuh""
          }
        },
        ""fields"": {
          ""timestamp"": [
            ""2020-05-24T04:00:01.841Z""
          ]
        },
        ""highlight"": {
          ""group"": [
            ""@kibana-highlighted-field@Daklak@/kibana-highlighted-field@"",
            ""@kibana-highlighted-field@Daklak_Webs@/kibana-highlighted-field@""
          ]
        },
        ""sort"": [
          -9223372036854776000
        ]
      },
      {
        ""_index"": ""wazuh-monitoring-3.x-2020.05.24"",
        ""_type"": ""_doc"",
        ""_id"": ""uv6RRHIBENNuuStZwL7t"",
        ""_version"": 1,
        ""_score"": null,
        ""_source"": {
          ""os"": {
            ""arch"": ""x86_64"",
            ""major"": ""6"",
            ""minor"": ""8"",
            ""name"": ""CentOS Linux"",
            ""platform"": ""centos"",
            ""uname"": ""Linux |webserver18 |2.6.32-642.el6.x86_64 |#1 SMP Tue May 10 17:27:01 UTC 2016 |x86_64"",
            ""version"": ""6.8""
          },
          ""status"": ""Active"",
          ""registerIP"": ""any"",
          ""mergedSum"": ""535237cb82b543ec180a59bde6425db5"",
          ""dateAdd"": ""2020-03-31 02:15:08"",
          ""lastKeepAlive"": ""2020-05-24 02:44:51"",
          ""id"": ""061"",
          ""configSum"": ""d149ab60df71e6fb53b2d38db75bc733"",
          ""ip"": ""192.168.2.18"",
          ""group"": [
            ""Daklak"",
            ""Test"",
            ""VSEC"",
            ""Daklak_Webs""
          ],
          ""node_name"": ""node02"",
          ""manager"": ""vsec-logstash"",
          ""version"": ""Wazuh v3.11.4"",
          ""name"": ""webserver18"",
          ""timestamp"": ""2020-05-24T02:45:02.308Z"",
          ""host"": ""vsec-logstash"",
          ""cluster"": {
            ""name"": ""wazuh""
          }
        },
        ""fields"": {
          ""timestamp"": [
            ""2020-05-24T02:45:02.308Z""
          ]
        },
        ""highlight"": {
          ""group"": [
            ""@kibana-highlighted-field@Daklak@/kibana-highlighted-field@"",
            ""@kibana-highlighted-field@Daklak_Webs@/kibana-highlighted-field@""
          ]
        },
        ""sort"": [
          -9223372036854776000
        ]
      },
      {
        ""_index"": ""wazuh-monitoring-3.x-2020.05.24"",
        ""_type"": ""_doc"",
        ""_id"": ""wwykRXIBENNuuStZZtF0"",
        ""_version"": 1,
        ""_score"": null,
        ""_source"": {
          ""os"": {
            ""arch"": ""x86_64"",
            ""major"": ""6"",
            ""minor"": ""10"",
            ""name"": ""CentOS Linux"",
            ""platform"": ""centos"",
            ""uname"": ""Linux |werserver |2.6.32-754.11.1.el6.x86_64 |#1 SMP Tue Feb 26 15:38:56 UTC 2019 |x86_64"",
            ""version"": ""6.10""
          },
          ""manager"": ""vsec-logstash"",
          ""lastKeepAlive"": ""2020-05-24 07:44:57"",
          ""version"": ""Wazuh v3.11.4"",
          ""status"": ""Active"",
          ""id"": ""060"",
          ""registerIP"": ""any"",
          ""name"": ""werserver"",
          ""dateAdd"": ""2020-03-27 07:47:46"",
          ""ip"": ""192.168.2.14"",
          ""group"": [
            ""Daklak"",
            ""Test"",
            ""VSEC"",
            ""Daklak_Webs"",
            ""Daklak_Apps""
          ],
          ""configSum"": ""4b54b212c2dd96f8b25aebfea2a104cd"",
          ""mergedSum"": ""f69c8a56d2cd8b401643a52cf423ad1c"",
          ""node_name"": ""node02"",
          ""timestamp"": ""2020-05-24T07:45:01.553Z"",
          ""host"": ""vsec-logstash"",
          ""cluster"": {
            ""name"": ""wazuh""
          }
        },
        ""fields"": {
          ""timestamp"": [
            ""2020-05-24T07:45:01.553Z""
          ]
        },
        ""highlight"": {
          ""group"": [
            ""@kibana-highlighted-field@Daklak@/kibana-highlighted-field@"",
            ""@kibana-highlighted-field@Daklak_Webs@/kibana-highlighted-field@""
          ]
        },
        ""sort"": [
          -9223372036854776000
        ]
      },
      {
        ""_index"": ""wazuh-monitoring-3.x-2020.05.24"",
        ""_type"": ""_doc"",
        ""_id"": ""xAykRXIBENNuuStZZtF0"",
        ""_version"": 1,
        ""_score"": null,
        ""_source"": {
          ""os"": {
            ""arch"": ""x86_64"",
            ""major"": ""6"",
            ""minor"": ""8"",
            ""name"": ""CentOS Linux"",
            ""platform"": ""centos"",
            ""uname"": ""Linux |webserver18 |2.6.32-642.el6.x86_64 |#1 SMP Tue May 10 17:27:01 UTC 2016 |x86_64"",
            ""version"": ""6.8""
          },
          ""manager"": ""vsec-logstash"",
          ""lastKeepAlive"": ""2020-05-24 07:45:00"",
          ""version"": ""Wazuh v3.11.4"",
          ""status"": ""Active"",
          ""id"": ""061"",
          ""registerIP"": ""any"",
          ""name"": ""webserver18"",
          ""dateAdd"": ""2020-03-31 02:15:08"",
          ""ip"": ""192.168.2.18"",
          ""group"": [
            ""Daklak"",
            ""Test"",
            ""VSEC"",
            ""Daklak_Webs""
          ],
          ""configSum"": ""d149ab60df71e6fb53b2d38db75bc733"",
          ""mergedSum"": ""535237cb82b543ec180a59bde6425db5"",
          ""node_name"": ""node02"",
          ""timestamp"": ""2020-05-24T07:45:01.553Z"",
          ""host"": ""vsec-logstash"",
          ""cluster"": {
            ""name"": ""wazuh""
          }
        },
        ""fields"": {
          ""timestamp"": [
            ""2020-05-24T07:45:01.553Z""
          ]
        },
        ""highlight"": {
          ""group"": [
            ""@kibana-highlighted-field@Daklak@/kibana-highlighted-field@"",
            ""@kibana-highlighted-field@Daklak_Webs@/kibana-highlighted-field@""
          ]
        },
        ""sort"": [
          -9223372036854776000
        ]
      },
      {
        ""_index"": ""wazuh-monitoring-3.x-2020.05.24"",
        ""_type"": ""_doc"",
        ""_id"": ""MAuIRXIBENNuuStZ724t"",
        ""_version"": 1,
        ""_score"": null,
        ""_source"": {
          ""os"": {
            ""arch"": ""x86_64"",
            ""major"": ""6"",
            ""minor"": ""10"",
            ""name"": ""CentOS Linux"",
            ""platform"": ""centos"",
            ""uname"": ""Linux |werserver |2.6.32-754.11.1.el6.x86_64 |#1 SMP Tue Feb 26 15:38:56 UTC 2019 |x86_64"",
            ""version"": ""6.10""
          },
          ""group"": [
            ""Daklak"",
            ""Test"",
            ""VSEC"",
            ""Daklak_Webs"",
            ""Daklak_Apps""
          ],
          ""mergedSum"": ""f69c8a56d2cd8b401643a52cf423ad1c"",
          ""configSum"": ""4b54b212c2dd96f8b25aebfea2a104cd"",
          ""manager"": ""vsec-logstash"",
          ""ip"": ""192.168.2.14"",
          ""name"": ""werserver"",
          ""node_name"": ""node02"",
          ""version"": ""Wazuh v3.11.4"",
          ""registerIP"": ""any"",
          ""status"": ""Active"",
          ""dateAdd"": ""2020-03-27 07:47:46"",
          ""lastKeepAlive"": ""2020-05-24 07:14:49"",
          ""id"": ""060"",
          ""timestamp"": ""2020-05-24T07:15:01.545Z"",
          ""host"": ""vsec-logstash"",
          ""cluster"": {
            ""name"": ""wazuh""
          }
        },
        ""fields"": {
          ""timestamp"": [
            ""2020-05-24T07:15:01.545Z""
          ]
        },
        ""highlight"": {
          ""group"": [
            ""@kibana-highlighted-field@Daklak@/kibana-highlighted-field@"",
            ""@kibana-highlighted-field@Daklak_Webs@/kibana-highlighted-field@""
          ]
        },
        ""sort"": [
          -9223372036854776000
        ]
      },
      {
        ""_index"": ""wazuh-monitoring-3.x-2020.05.24"",
        ""_type"": ""_doc"",
        ""_id"": ""MQuIRXIBENNuuStZ724t"",
        ""_version"": 1,
        ""_score"": null,
        ""_source"": {
          ""os"": {
            ""arch"": ""x86_64"",
            ""major"": ""6"",
            ""minor"": ""8"",
            ""name"": ""CentOS Linux"",
            ""platform"": ""centos"",
            ""uname"": ""Linux |webserver18 |2.6.32-642.el6.x86_64 |#1 SMP Tue May 10 17:27:01 UTC 2016 |x86_64"",
            ""version"": ""6.8""
          },
          ""group"": [
            ""Daklak"",
            ""Test"",
            ""VSEC"",
            ""Daklak_Webs""
          ],
          ""mergedSum"": ""535237cb82b543ec180a59bde6425db5"",
          ""configSum"": ""d149ab60df71e6fb53b2d38db75bc733"",
          ""manager"": ""vsec-logstash"",
          ""ip"": ""192.168.2.18"",
          ""name"": ""webserver18"",
          ""node_name"": ""node02"",
          ""version"": ""Wazuh v3.11.4"",
          ""registerIP"": ""any"",
          ""status"": ""Active"",
          ""dateAdd"": ""2020-03-31 02:15:08"",
          ""lastKeepAlive"": ""2020-05-24 07:14:49"",
          ""id"": ""061"",
          ""timestamp"": ""2020-05-24T07:15:01.545Z"",
          ""host"": ""vsec-logstash"",
          ""cluster"": {
            ""name"": ""wazuh""
          }
        },
        ""fields"": {
          ""timestamp"": [
            ""2020-05-24T07:15:01.545Z""
          ]
        },
        ""highlight"": {
          ""group"": [
            ""@kibana-highlighted-field@Daklak@/kibana-highlighted-field@"",
            ""@kibana-highlighted-field@Daklak_Webs@/kibana-highlighted-field@""
          ]
        },
        ""sort"": [
          -9223372036854776000
        ]
      },
      {
        ""_index"": ""wazuh-monitoring-3.x-2020.05.24"",
        ""_type"": ""_doc"",
        ""_id"": ""Mg2yRXIBENNuuStZJIPY"",
        ""_version"": 1,
        ""_score"": null,
        ""_source"": {
          ""os"": {
            ""arch"": ""x86_64"",
            ""major"": ""6"",
            ""minor"": ""10"",
            ""name"": ""CentOS Linux"",
            ""platform"": ""centos"",
            ""uname"": ""Linux |werserver |2.6.32-754.11.1.el6.x86_64 |#1 SMP Tue Feb 26 15:38:56 UTC 2019 |x86_64"",
            ""version"": ""6.10""
          },
          ""registerIP"": ""any"",
          ""lastKeepAlive"": ""2020-05-24 07:59:47"",
          ""manager"": ""vsec-logstash"",
          ""dateAdd"": ""2020-03-27 07:47:46"",
          ""ip"": ""192.168.2.14"",
          ""configSum"": ""4b54b212c2dd96f8b25aebfea2a104cd"",
          ""id"": ""060"",
          ""status"": ""Active"",
          ""group"": [
            ""Daklak"",
            ""Test"",
            ""VSEC"",
            ""Daklak_Webs"",
            ""Daklak_Apps""
          ],
          ""version"": ""Wazuh v3.11.4"",
          ""name"": ""werserver"",
          ""node_name"": ""node02"",
          ""mergedSum"": ""f69c8a56d2cd8b401643a52cf423ad1c"",
          ""timestamp"": ""2020-05-24T08:00:02.260Z"",
          ""host"": ""vsec-logstash"",
          ""cluster"": {
            ""name"": ""wazuh""
          }
        },
        ""fields"": {
          ""timestamp"": [
            ""2020-05-24T08:00:02.260Z""
          ]
        },
        ""highlight"": {
          ""group"": [
            ""@kibana-highlighted-field@Daklak@/kibana-highlighted-field@"",
            ""@kibana-highlighted-field@Daklak_Webs@/kibana-highlighted-field@""
          ]
        },
        ""sort"": [
          -9223372036854776000
        ]
      },
      {
        ""_index"": ""wazuh-monitoring-3.x-2020.05.24"",
        ""_type"": ""_doc"",
        ""_id"": ""_BDpRXIBENNuuStZEFQS"",
        ""_version"": 1,
        ""_score"": null,
        ""_source"": {
          ""os"": {
            ""arch"": ""x86_64"",
            ""major"": ""6"",
            ""minor"": ""8"",
            ""name"": ""CentOS Linux"",
            ""platform"": ""centos"",
            ""uname"": ""Linux |webserver18 |2.6.32-642.el6.x86_64 |#1 SMP Tue May 10 17:27:01 UTC 2016 |x86_64"",
            ""version"": ""6.8""
          },
          ""name"": ""webserver18"",
          ""manager"": ""vsec-logstash"",
          ""dateAdd"": ""2020-03-31 02:15:08"",
          ""configSum"": ""d149ab60df71e6fb53b2d38db75bc733"",
          ""version"": ""Wazuh v3.11.4"",
          ""registerIP"": ""any"",
          ""status"": ""Active"",
          ""id"": ""061"",
          ""ip"": ""192.168.2.18"",
          ""node_name"": ""node02"",
          ""group"": [
            ""Daklak"",
            ""Test"",
            ""VSEC"",
            ""Daklak_Webs""
          ],
          ""mergedSum"": ""535237cb82b543ec180a59bde6425db5"",
          ""lastKeepAlive"": ""2020-05-24 08:59:57"",
          ""timestamp"": ""2020-05-24T09:00:01.420Z"",
          ""host"": ""vsec-logstash"",
          ""cluster"": {
            ""name"": ""wazuh""
          }
        },
        ""fields"": {
          ""timestamp"": [
            ""2020-05-24T09:00:01.420Z""
          ]
        },
        ""highlight"": {
          ""group"": [
            ""@kibana-highlighted-field@Daklak@/kibana-highlighted-field@"",
            ""@kibana-highlighted-field@Daklak_Webs@/kibana-highlighted-field@""
          ]
        },
        ""sort"": [
          -9223372036854776000
        ]
      },
      {
        ""_index"": ""wazuh-monitoring-3.x-2020.05.24"",
        ""_type"": ""_doc"",
        ""_id"": ""qwyWRXIBENNuuStZrB98"",
        ""_version"": 1,
        ""_score"": null,
        ""_source"": {
          ""os"": {
            ""arch"": ""x86_64"",
            ""major"": ""6"",
            ""minor"": ""10"",
            ""name"": ""CentOS Linux"",
            ""platform"": ""centos"",
            ""uname"": ""Linux |werserver |2.6.32-754.11.1.el6.x86_64 |#1 SMP Tue Feb 26 15:38:56 UTC 2019 |x86_64"",
            ""version"": ""6.10""
          },
          ""node_name"": ""node02"",
          ""ip"": ""192.168.2.14"",
          ""dateAdd"": ""2020-03-27 07:47:46"",
          ""configSum"": ""4b54b212c2dd96f8b25aebfea2a104cd"",
          ""registerIP"": ""any"",
          ""status"": ""Active"",
          ""mergedSum"": ""f69c8a56d2cd8b401643a52cf423ad1c"",
          ""id"": ""060"",
          ""group"": [
            ""Daklak"",
            ""Test"",
            ""VSEC"",
            ""Daklak_Webs"",
            ""Daklak_Apps""
          ],
          ""version"": ""Wazuh v3.11.4"",
          ""lastKeepAlive"": ""2020-05-24 07:29:59"",
          ""name"": ""werserver"",
          ""manager"": ""vsec-logstash"",
          ""timestamp"": ""2020-05-24T07:30:01.975Z"",
          ""host"": ""vsec-logstash"",
          ""cluster"": {
            ""name"": ""wazuh""
          }
        },
        ""fields"": {
          ""timestamp"": [
            ""2020-05-24T07:30:01.975Z""
          ]
        },
        ""highlight"": {
          ""group"": [
            ""@kibana-highlighted-field@Daklak@/kibana-highlighted-field@"",
            ""@kibana-highlighted-field@Daklak_Webs@/kibana-highlighted-field@""
          ]
        },
        ""sort"": [
          -9223372036854776000
        ]
      },
      {
        ""_index"": ""wazuh-monitoring-3.x-2020.05.24"",
        ""_type"": ""_doc"",
        ""_id"": ""rAyWRXIBENNuuStZrB98"",
        ""_version"": 1,
        ""_score"": null,
        ""_source"": {
          ""os"": {
            ""arch"": ""x86_64"",
            ""major"": ""6"",
            ""minor"": ""8"",
            ""name"": ""CentOS Linux"",
            ""platform"": ""centos"",
            ""uname"": ""Linux |webserver18 |2.6.32-642.el6.x86_64 |#1 SMP Tue May 10 17:27:01 UTC 2016 |x86_64"",
            ""version"": ""6.8""
          },
          ""node_name"": ""node02"",
          ""ip"": ""192.168.2.18"",
          ""dateAdd"": ""2020-03-31 02:15:08"",
          ""configSum"": ""d149ab60df71e6fb53b2d38db75bc733"",
          ""registerIP"": ""any"",
          ""status"": ""Active"",
          ""mergedSum"": ""535237cb82b543ec180a59bde6425db5"",
          ""id"": ""061"",
          ""group"": [
            ""Daklak"",
            ""Test"",
            ""VSEC"",
            ""Daklak_Webs""
          ],
          ""version"": ""Wazuh v3.11.4"",
          ""lastKeepAlive"": ""2020-05-24 07:29:49"",
          ""name"": ""webserver18"",
          ""manager"": ""vsec-logstash"",
          ""timestamp"": ""2020-05-24T07:30:01.975Z"",
          ""host"": ""vsec-logstash"",
          ""cluster"": {
            ""name"": ""wazuh""
          }
        },
        ""fields"": {
          ""timestamp"": [
            ""2020-05-24T07:30:01.975Z""
          ]
        },
        ""highlight"": {
          ""group"": [
            ""@kibana-highlighted-field@Daklak@/kibana-highlighted-field@"",
            ""@kibana-highlighted-field@Daklak_Webs@/kibana-highlighted-field@""
          ]
        },
        ""sort"": [
          -9223372036854776000
        ]
      },
      {
        ""_index"": ""wazuh-monitoring-3.x-2020.05.24"",
        ""_type"": ""_doc"",
        ""_id"": ""ahH2RXIBENNuuStZzRGy"",
        ""_version"": 1,
        ""_score"": null,
        ""_source"": {
          ""os"": {
            ""arch"": ""x86_64"",
            ""major"": ""6"",
            ""minor"": ""10"",
            ""name"": ""CentOS Linux"",
            ""platform"": ""centos"",
            ""uname"": ""Linux |werserver |2.6.32-754.11.1.el6.x86_64 |#1 SMP Tue Feb 26 15:38:56 UTC 2019 |x86_64"",
            ""version"": ""6.10""
          },
          ""name"": ""werserver"",
          ""id"": ""060"",
          ""group"": [
            ""Daklak"",
            ""Test"",
            ""VSEC"",
            ""Daklak_Webs"",
            ""Daklak_Apps""
          ],
          ""dateAdd"": ""2020-03-27 07:47:46"",
          ""version"": ""Wazuh v3.11.4"",
          ""node_name"": ""node02"",
          ""registerIP"": ""any"",
          ""ip"": ""192.168.2.14"",
          ""configSum"": ""4b54b212c2dd96f8b25aebfea2a104cd"",
          ""lastKeepAlive"": ""2020-05-24 09:14:58"",
          ""status"": ""Active"",
          ""manager"": ""vsec-logstash"",
          ""mergedSum"": ""f69c8a56d2cd8b401643a52cf423ad1c"",
          ""timestamp"": ""2020-05-24T09:15:01.934Z"",
          ""host"": ""vsec-logstash"",
          ""cluster"": {
            ""name"": ""wazuh""
          }
        },
        ""fields"": {
          ""timestamp"": [
            ""2020-05-24T09:15:01.934Z""
          ]
        },
        ""highlight"": {
          ""group"": [
            ""@kibana-highlighted-field@Daklak@/kibana-highlighted-field@"",
            ""@kibana-highlighted-field@Daklak_Webs@/kibana-highlighted-field@""
          ]
        },
        ""sort"": [
          -9223372036854776000
        ]
      },
      {
        ""_index"": ""wazuh-monitoring-3.x-2020.05.24"",
        ""_type"": ""_doc"",
        ""_id"": ""sQ7NRXIBENNuuStZnPJq"",
        ""_version"": 1,
        ""_score"": null,
        ""_source"": {
          ""os"": {
            ""arch"": ""x86_64"",
            ""major"": ""6"",
            ""minor"": ""8"",
            ""name"": ""CentOS Linux"",
            ""platform"": ""centos"",
            ""uname"": ""Linux |webserver18 |2.6.32-642.el6.x86_64 |#1 SMP Tue May 10 17:27:01 UTC 2016 |x86_64"",
            ""version"": ""6.8""
          },
          ""configSum"": ""d149ab60df71e6fb53b2d38db75bc733"",
          ""manager"": ""vsec-logstash"",
          ""registerIP"": ""any"",
          ""lastKeepAlive"": ""2020-05-24 08:29:50"",
          ""group"": [
            ""Daklak"",
            ""Test"",
            ""VSEC"",
            ""Daklak_Webs""
          ],
          ""ip"": ""192.168.2.18"",
          ""id"": ""061"",
          ""status"": ""Active"",
          ""mergedSum"": ""535237cb82b543ec180a59bde6425db5"",
          ""version"": ""Wazuh v3.11.4"",
          ""node_name"": ""node02"",
          ""dateAdd"": ""2020-03-31 02:15:08"",
          ""name"": ""webserver18"",
          ""timestamp"": ""2020-05-24T08:30:02.344Z"",
          ""host"": ""vsec-logstash"",
          ""cluster"": {
            ""name"": ""wazuh""
          }
        },
        ""fields"": {
          ""timestamp"": [
            ""2020-05-24T08:30:02.344Z""
          ]
        },
        ""highlight"": {
          ""group"": [
            ""@kibana-highlighted-field@Daklak@/kibana-highlighted-field@"",
            ""@kibana-highlighted-field@Daklak_Webs@/kibana-highlighted-field@""
          ]
        },
        ""sort"": [
          -9223372036854776000
        ]
      },
      {
        ""_index"": ""wazuh-monitoring-3.x-2020.05.24"",
        ""_type"": ""_doc"",
        ""_id"": ""sA_bRXIBENNuuStZVqMO"",
        ""_version"": 1,
        ""_score"": null,
        ""_source"": {
          ""os"": {
            ""arch"": ""x86_64"",
            ""major"": ""6"",
            ""minor"": ""10"",
            ""name"": ""CentOS Linux"",
            ""platform"": ""centos"",
            ""uname"": ""Linux |werserver |2.6.32-754.11.1.el6.x86_64 |#1 SMP Tue Feb 26 15:38:56 UTC 2019 |x86_64"",
            ""version"": ""6.10""
          },
          ""dateAdd"": ""2020-03-27 07:47:46"",
          ""configSum"": ""4b54b212c2dd96f8b25aebfea2a104cd"",
          ""ip"": ""192.168.2.14"",
          ""id"": ""060"",
          ""lastKeepAlive"": ""2020-05-24 08:44:57"",
          ""version"": ""Wazuh v3.11.4"",
          ""node_name"": ""node02"",
          ""manager"": ""vsec-logstash"",
          ""name"": ""werserver"",
          ""mergedSum"": ""f69c8a56d2cd8b401643a52cf423ad1c"",
          ""group"": [
            ""Daklak"",
            ""Test"",
            ""VSEC"",
            ""Daklak_Webs"",
            ""Daklak_Apps""
          ],
          ""status"": ""Active"",
          ""registerIP"": ""any"",
          ""timestamp"": ""2020-05-24T08:45:01.835Z"",
          ""host"": ""vsec-logstash"",
          ""cluster"": {
            ""name"": ""wazuh""
          }
        },
        ""fields"": {
          ""timestamp"": [
            ""2020-05-24T08:45:01.835Z""
          ]
        },
        ""highlight"": {
          ""group"": [
            ""@kibana-highlighted-field@Daklak@/kibana-highlighted-field@"",
            ""@kibana-highlighted-field@Daklak_Webs@/kibana-highlighted-field@""
          ]
        },
        ""sort"": [
          -9223372036854776000
        ]
      },
      {
        ""_index"": ""wazuh-monitoring-3.x-2020.05.24"",
        ""_type"": ""_doc"",
        ""_id"": ""Eg6_RXIBENNuuStZ3kHL"",
        ""_version"": 1,
        ""_score"": null,
        ""_source"": {
          ""os"": {
            ""arch"": ""x86_64"",
            ""major"": ""6"",
            ""minor"": ""10"",
            ""name"": ""CentOS Linux"",
            ""platform"": ""centos"",
            ""uname"": ""Linux |werserver |2.6.32-754.11.1.el6.x86_64 |#1 SMP Tue Feb 26 15:38:56 UTC 2019 |x86_64"",
            ""version"": ""6.10""
          },
          ""ip"": ""192.168.2.14"",
          ""status"": ""Active"",
          ""mergedSum"": ""f69c8a56d2cd8b401643a52cf423ad1c"",
          ""manager"": ""vsec-logstash"",
          ""name"": ""werserver"",
          ""node_name"": ""node02"",
          ""id"": ""060"",
          ""dateAdd"": ""2020-03-27 07:47:46"",
          ""version"": ""Wazuh v3.11.4"",
          ""lastKeepAlive"": ""2020-05-24 08:14:47"",
          ""configSum"": ""4b54b212c2dd96f8b25aebfea2a104cd"",
          ""group"": [
            ""Daklak"",
            ""Test"",
            ""VSEC"",
            ""Daklak_Webs"",
            ""Daklak_Apps""
          ],
          ""registerIP"": ""any"",
          ""timestamp"": ""2020-05-24T08:15:01.828Z"",
          ""host"": ""vsec-logstash"",
          ""cluster"": {
            ""name"": ""wazuh""
          }
        },
        ""fields"": {
          ""timestamp"": [
            ""2020-05-24T08:15:01.828Z""
          ]
        },
        ""highlight"": {
          ""group"": [
            ""@kibana-highlighted-field@Daklak@/kibana-highlighted-field@"",
            ""@kibana-highlighted-field@Daklak_Webs@/kibana-highlighted-field@""
          ]
        },
        ""sort"": [
          -9223372036854776000
        ]
      },
      {
        ""_index"": ""wazuh-monitoring-3.x-2020.05.24"",
        ""_type"": ""_doc"",
        ""_id"": ""Ew6_RXIBENNuuStZ3kHL"",
        ""_version"": 1,
        ""_score"": null,
        ""_source"": {
          ""os"": {
            ""arch"": ""x86_64"",
            ""major"": ""6"",
            ""minor"": ""8"",
            ""name"": ""CentOS Linux"",
            ""platform"": ""centos"",
            ""uname"": ""Linux |webserver18 |2.6.32-642.el6.x86_64 |#1 SMP Tue May 10 17:27:01 UTC 2016 |x86_64"",
            ""version"": ""6.8""
          },
          ""ip"": ""192.168.2.18"",
          ""status"": ""Active"",
          ""mergedSum"": ""535237cb82b543ec180a59bde6425db5"",
          ""manager"": ""vsec-logstash"",
          ""name"": ""webserver18"",
          ""node_name"": ""node02"",
          ""id"": ""061"",
          ""dateAdd"": ""2020-03-31 02:15:08"",
          ""version"": ""Wazuh v3.11.4"",
          ""lastKeepAlive"": ""2020-05-24 08:14:50"",
          ""configSum"": ""d149ab60df71e6fb53b2d38db75bc733"",
          ""group"": [
            ""Daklak"",
            ""Test"",
            ""VSEC"",
            ""Daklak_Webs""
          ],
          ""registerIP"": ""any"",
          ""timestamp"": ""2020-05-24T08:15:01.828Z"",
          ""host"": ""vsec-logstash"",
          ""cluster"": {
            ""name"": ""wazuh""
          }
        },
        ""fields"": {
          ""timestamp"": [
            ""2020-05-24T08:15:01.828Z""
          ]
        },
        ""highlight"": {
          ""group"": [
            ""@kibana-highlighted-field@Daklak@/kibana-highlighted-field@"",
            ""@kibana-highlighted-field@Daklak_Webs@/kibana-highlighted-field@""
          ]
        },
        ""sort"": [
          -9223372036854776000
        ]
      },
      {
        ""_index"": ""wazuh-monitoring-3.x-2020.05.24"",
        ""_type"": ""_doc"",
        ""_id"": ""KBvERnIBENNuuStZypq_"",
        ""_version"": 1,
        ""_score"": null,
        ""_source"": {
          ""os"": {
            ""arch"": ""x86_64"",
            ""major"": ""6"",
            ""minor"": ""10"",
            ""name"": ""CentOS Linux"",
            ""platform"": ""centos"",
            ""uname"": ""Linux |werserver |2.6.32-754.11.1.el6.x86_64 |#1 SMP Tue Feb 26 15:38:56 UTC 2019 |x86_64"",
            ""version"": ""6.10""
          },
          ""registerIP"": ""any"",
          ""manager"": ""vsec-logstash"",
          ""node_name"": ""node02"",
          ""status"": ""Active"",
          ""dateAdd"": ""2020-03-27 07:47:46"",
          ""mergedSum"": ""f69c8a56d2cd8b401643a52cf423ad1c"",
          ""ip"": ""192.168.2.14"",
          ""name"": ""werserver"",
          ""configSum"": ""4b54b212c2dd96f8b25aebfea2a104cd"",
          ""version"": ""Wazuh v3.11.4"",
          ""id"": ""060"",
          ""group"": [
            ""Daklak"",
            ""Test"",
            ""VSEC"",
            ""Daklak_Webs"",
            ""Daklak_Apps""
          ],
          ""lastKeepAlive"": ""2020-05-24 12:59:56"",
          ""timestamp"": ""2020-05-24T13:00:01.593Z"",
          ""host"": ""vsec-logstash"",
          ""cluster"": {
            ""name"": ""wazuh""
          }
        },
        ""fields"": {
          ""timestamp"": [
            ""2020-05-24T13:00:01.593Z""
          ]
        },
        ""highlight"": {
          ""group"": [
            ""@kibana-highlighted-field@Daklak@/kibana-highlighted-field@"",
            ""@kibana-highlighted-field@Daklak_Webs@/kibana-highlighted-field@""
          ]
        },
        ""sort"": [
          -9223372036854776000
        ]
      },
      {
        ""_index"": ""wazuh-monitoring-3.x-2020.05.24"",
        ""_type"": ""_doc"",
        ""_id"": ""KRvERnIBENNuuStZypq_"",
        ""_version"": 1,
        ""_score"": null,
        ""_source"": {
          ""os"": {
            ""arch"": ""x86_64"",
            ""major"": ""6"",
            ""minor"": ""8"",
            ""name"": ""CentOS Linux"",
            ""platform"": ""centos"",
            ""uname"": ""Linux |webserver18 |2.6.32-642.el6.x86_64 |#1 SMP Tue May 10 17:27:01 UTC 2016 |x86_64"",
            ""version"": ""6.8""
          },
          ""registerIP"": ""any"",
          ""manager"": ""vsec-logstash"",
          ""node_name"": ""node02"",
          ""status"": ""Active"",
          ""dateAdd"": ""2020-03-31 02:15:08"",
          ""mergedSum"": ""535237cb82b543ec180a59bde6425db5"",
          ""ip"": ""192.168.2.18"",
          ""name"": ""webserver18"",
          ""configSum"": ""d149ab60df71e6fb53b2d38db75bc733"",
          ""version"": ""Wazuh v3.11.4"",
          ""id"": ""061"",
          ""group"": [
            ""Daklak"",
            ""Test"",
            ""VSEC"",
            ""Daklak_Webs""
          ],
          ""lastKeepAlive"": ""2020-05-24 12:59:51"",
          ""timestamp"": ""2020-05-24T13:00:01.593Z"",
          ""host"": ""vsec-logstash"",
          ""cluster"": {
            ""name"": ""wazuh""
          }
        },
        ""fields"": {
          ""timestamp"": [
            ""2020-05-24T13:00:01.593Z""
          ]
        },
        ""highlight"": {
          ""group"": [
            ""@kibana-highlighted-field@Daklak@/kibana-highlighted-field@"",
            ""@kibana-highlighted-field@Daklak_Webs@/kibana-highlighted-field@""
          ]
        },
        ""sort"": [
          -9223372036854776000
        ]
      },
      {
        ""_index"": ""wazuh-monitoring-3.x-2020.05.24"",
        ""_type"": ""_doc"",
        ""_id"": ""wR3gRnIBENNuuStZQwe1"",
        ""_version"": 1,
        ""_score"": null,
        ""_source"": {
          ""os"": {
            ""arch"": ""x86_64"",
            ""major"": ""6"",
            ""minor"": ""10"",
            ""name"": ""CentOS Linux"",
            ""platform"": ""centos"",
            ""uname"": ""Linux |werserver |2.6.32-754.11.1.el6.x86_64 |#1 SMP Tue Feb 26 15:38:56 UTC 2019 |x86_64"",
            ""version"": ""6.10""
          },
          ""dateAdd"": ""2020-03-27 07:47:46"",
          ""id"": ""060"",
          ""lastKeepAlive"": ""2020-05-24 13:29:46"",
          ""manager"": ""vsec-logstash"",
          ""version"": ""Wazuh v3.11.4"",
          ""status"": ""Active"",
          ""mergedSum"": ""f69c8a56d2cd8b401643a52cf423ad1c"",
          ""registerIP"": ""any"",
          ""ip"": ""192.168.2.14"",
          ""configSum"": ""4b54b212c2dd96f8b25aebfea2a104cd"",
          ""group"": [
            ""Daklak"",
            ""Test"",
            ""VSEC"",
            ""Daklak_Webs"",
            ""Daklak_Apps""
          ],
          ""node_name"": ""node02"",
          ""name"": ""werserver"",
          ""timestamp"": ""2020-05-24T13:30:02.032Z"",
          ""host"": ""vsec-logstash"",
          ""cluster"": {
            ""name"": ""wazuh""
          }
        },
        ""fields"": {
          ""timestamp"": [
            ""2020-05-24T13:30:02.032Z""
          ]
        },
        ""highlight"": {
          ""group"": [
            ""@kibana-highlighted-field@Daklak@/kibana-highlighted-field@"",
            ""@kibana-highlighted-field@Daklak_Webs@/kibana-highlighted-field@""
          ]
        },
        ""sort"": [
          -9223372036854776000
        ]
      },
      {
        ""_index"": ""wazuh-monitoring-3.x-2020.05.24"",
        ""_type"": ""_doc"",
        ""_id"": ""wh3gRnIBENNuuStZQwe1"",
        ""_version"": 1,
        ""_score"": null,
        ""_source"": {
          ""os"": {
            ""arch"": ""x86_64"",
            ""major"": ""6"",
            ""minor"": ""8"",
            ""name"": ""CentOS Linux"",
            ""platform"": ""centos"",
            ""uname"": ""Linux |webserver18 |2.6.32-642.el6.x86_64 |#1 SMP Tue May 10 17:27:01 UTC 2016 |x86_64"",
            ""version"": ""6.8""
          },
          ""dateAdd"": ""2020-03-31 02:15:08"",
          ""id"": ""061"",
          ""lastKeepAlive"": ""2020-05-24 13:29:51"",
          ""manager"": ""vsec-logstash"",
          ""version"": ""Wazuh v3.11.4"",
          ""status"": ""Active"",
          ""mergedSum"": ""535237cb82b543ec180a59bde6425db5"",
          ""registerIP"": ""any"",
          ""ip"": ""192.168.2.18"",
          ""configSum"": ""d149ab60df71e6fb53b2d38db75bc733"",
          ""group"": [
            ""Daklak"",
            ""Test"",
            ""VSEC"",
            ""Daklak_Webs""
          ],
          ""node_name"": ""node02"",
          ""name"": ""webserver18"",
          ""timestamp"": ""2020-05-24T13:30:02.032Z"",
          ""host"": ""vsec-logstash"",
          ""cluster"": {
            ""name"": ""wazuh""
          }
        },
        ""fields"": {
          ""timestamp"": [
            ""2020-05-24T13:30:02.032Z""
          ]
        },
        ""highlight"": {
          ""group"": [
            ""@kibana-highlighted-field@Daklak@/kibana-highlighted-field@"",
            ""@kibana-highlighted-field@Daklak_Webs@/kibana-highlighted-field@""
          ]
        },
        ""sort"": [
          -9223372036854776000
        ]
      },
      {
        ""_index"": ""wazuh-monitoring-3.x-2020.05.24"",
        ""_type"": ""_doc"",
        ""_id"": ""whq3RnIBENNuuStZEOab"",
        ""_version"": 1,
        ""_score"": null,
        ""_source"": {
          ""os"": {
            ""arch"": ""x86_64"",
            ""major"": ""6"",
            ""minor"": ""10"",
            ""name"": ""CentOS Linux"",
            ""platform"": ""centos"",
            ""uname"": ""Linux |werserver |2.6.32-754.11.1.el6.x86_64 |#1 SMP Tue Feb 26 15:38:56 UTC 2019 |x86_64"",
            ""version"": ""6.10""
          },
          ""dateAdd"": ""2020-03-27 07:47:46"",
          ""mergedSum"": ""f69c8a56d2cd8b401643a52cf423ad1c"",
          ""id"": ""060"",
          ""version"": ""Wazuh v3.11.4"",
          ""status"": ""Active"",
          ""name"": ""werserver"",
          ""configSum"": ""4b54b212c2dd96f8b25aebfea2a104cd"",
          ""node_name"": ""node02"",
          ""ip"": ""192.168.2.14"",
          ""group"": [
            ""Daklak"",
            ""Test"",
            ""VSEC"",
            ""Daklak_Webs"",
            ""Daklak_Apps""
          ],
          ""lastKeepAlive"": ""2020-05-24 12:44:47"",
          ""registerIP"": ""any"",
          ""manager"": ""vsec-logstash"",
          ""timestamp"": ""2020-05-24T12:45:01.971Z"",
          ""host"": ""vsec-logstash"",
          ""cluster"": {
            ""name"": ""wazuh""
          }
        },
        ""fields"": {
          ""timestamp"": [
            ""2020-05-24T12:45:01.971Z""
          ]
        },
        ""highlight"": {
          ""group"": [
            ""@kibana-highlighted-field@Daklak@/kibana-highlighted-field@"",
            ""@kibana-highlighted-field@Daklak_Webs@/kibana-highlighted-field@""
          ]
        },
        ""sort"": [
          -9223372036854776000
        ]
      },
      {
        ""_index"": ""wazuh-monitoring-3.x-2020.05.24"",
        ""_type"": ""_doc"",
        ""_id"": ""ZRzSRnIBENNuuStZiFRc"",
        ""_version"": 1,
        ""_score"": null,
        ""_source"": {
          ""os"": {
            ""arch"": ""x86_64"",
            ""major"": ""6"",
            ""minor"": ""10"",
            ""name"": ""CentOS Linux"",
            ""platform"": ""centos"",
            ""uname"": ""Linux |werserver |2.6.32-754.11.1.el6.x86_64 |#1 SMP Tue Feb 26 15:38:56 UTC 2019 |x86_64"",
            ""version"": ""6.10""
          },
          ""status"": ""Active"",
          ""registerIP"": ""any"",
          ""configSum"": ""4b54b212c2dd96f8b25aebfea2a104cd"",
          ""group"": [
            ""Daklak"",
            ""Test"",
            ""VSEC"",
            ""Daklak_Webs"",
            ""Daklak_Apps""
          ],
          ""mergedSum"": ""f69c8a56d2cd8b401643a52cf423ad1c"",
          ""ip"": ""192.168.2.14"",
          ""dateAdd"": ""2020-03-27 07:47:46"",
          ""node_name"": ""node02"",
          ""version"": ""Wazuh v3.11.4"",
          ""name"": ""werserver"",
          ""manager"": ""vsec-logstash"",
          ""lastKeepAlive"": ""2020-05-24 13:14:56"",
          ""id"": ""060"",
          ""timestamp"": ""2020-05-24T13:15:02.105Z"",
          ""host"": ""vsec-logstash"",
          ""cluster"": {
            ""name"": ""wazuh""
          }
        },
        ""fields"": {
          ""timestamp"": [
            ""2020-05-24T13:15:02.105Z""
          ]
        },
        ""highlight"": {
          ""group"": [
            ""@kibana-highlighted-field@Daklak@/kibana-highlighted-field@"",
            ""@kibana-highlighted-field@Daklak_Webs@/kibana-highlighted-field@""
          ]
        },
        ""sort"": [
          -9223372036854776000
        ]
      },
      {
        ""_index"": ""wazuh-monitoring-3.x-2020.05.24"",
        ""_type"": ""_doc"",
        ""_id"": ""ZhzSRnIBENNuuStZiFRc"",
        ""_version"": 1,
        ""_score"": null,
        ""_source"": {
          ""os"": {
            ""arch"": ""x86_64"",
            ""major"": ""6"",
            ""minor"": ""8"",
            ""name"": ""CentOS Linux"",
            ""platform"": ""centos"",
            ""uname"": ""Linux |webserver18 |2.6.32-642.el6.x86_64 |#1 SMP Tue May 10 17:27:01 UTC 2016 |x86_64"",
            ""version"": ""6.8""
          },
          ""status"": ""Active"",
          ""registerIP"": ""any"",
          ""configSum"": ""d149ab60df71e6fb53b2d38db75bc733"",
          ""group"": [
            ""Daklak"",
            ""Test"",
            ""VSEC"",
            ""Daklak_Webs""
          ],
          ""mergedSum"": ""535237cb82b543ec180a59bde6425db5"",
          ""ip"": ""192.168.2.18"",
          ""dateAdd"": ""2020-03-31 02:15:08"",
          ""node_name"": ""node02"",
          ""version"": ""Wazuh v3.11.4"",
          ""name"": ""webserver18"",
          ""manager"": ""vsec-logstash"",
          ""lastKeepAlive"": ""2020-05-24 13:14:52"",
          ""id"": ""061"",
          ""timestamp"": ""2020-05-24T13:15:02.105Z"",
          ""host"": ""vsec-logstash"",
          ""cluster"": {
            ""name"": ""wazuh""
          }
        },
        ""fields"": {
          ""timestamp"": [
            ""2020-05-24T13:15:02.105Z""
          ]
        },
        ""highlight"": {
          ""group"": [
            ""@kibana-highlighted-field@Daklak@/kibana-highlighted-field@"",
            ""@kibana-highlighted-field@Daklak_Webs@/kibana-highlighted-field@""
          ]
        },
        ""sort"": [
          -9223372036854776000
        ]
      },
      {
        ""_index"": ""wazuh-monitoring-3.x-2020.05.24"",
        ""_type"": ""_doc"",
        ""_id"": ""zBiNRnIBENNuuStZ38pt"",
        ""_version"": 1,
        ""_score"": null,
        ""_source"": {
          ""os"": {
            ""arch"": ""x86_64"",
            ""major"": ""6"",
            ""minor"": ""10"",
            ""name"": ""CentOS Linux"",
            ""platform"": ""centos"",
            ""uname"": ""Linux |werserver |2.6.32-754.11.1.el6.x86_64 |#1 SMP Tue Feb 26 15:38:56 UTC 2019 |x86_64"",
            ""version"": ""6.10""
          },
          ""mergedSum"": ""f69c8a56d2cd8b401643a52cf423ad1c"",
          ""status"": ""Active"",
          ""manager"": ""vsec-logstash"",
          ""node_name"": ""node02"",
          ""configSum"": ""4b54b212c2dd96f8b25aebfea2a104cd"",
          ""group"": [
            ""Daklak"",
            ""Test"",
            ""VSEC"",
            ""Daklak_Webs"",
            ""Daklak_Apps""
          ],
          ""version"": ""Wazuh v3.11.4"",
          ""id"": ""060"",
          ""dateAdd"": ""2020-03-27 07:47:46"",
          ""lastKeepAlive"": ""2020-05-24 11:59:59"",
          ""registerIP"": ""any"",
          ""name"": ""werserver"",
          ""ip"": ""192.168.2.14"",
          ""timestamp"": ""2020-05-24T12:00:02.410Z"",
          ""host"": ""vsec-logstash"",
          ""cluster"": {
            ""name"": ""wazuh""
          }
        },
        ""fields"": {
          ""timestamp"": [
            ""2020-05-24T12:00:02.410Z""
          ]
        },
        ""highlight"": {
          ""group"": [
            ""@kibana-highlighted-field@Daklak@/kibana-highlighted-field@"",
            ""@kibana-highlighted-field@Daklak_Webs@/kibana-highlighted-field@""
          ]
        },
        ""sort"": [
          -9223372036854776000
        ]
      },
      {
        ""_index"": ""wazuh-monitoring-3.x-2020.05.24"",
        ""_type"": ""_doc"",
        ""_id"": ""ZBiARnIBENNuuStZIRml"",
        ""_version"": 1,
        ""_score"": null,
        ""_source"": {
          ""os"": {
            ""arch"": ""x86_64"",
            ""major"": ""6"",
            ""minor"": ""10"",
            ""name"": ""CentOS Linux"",
            ""platform"": ""centos"",
            ""uname"": ""Linux |werserver |2.6.32-754.11.1.el6.x86_64 |#1 SMP Tue Feb 26 15:38:56 UTC 2019 |x86_64"",
            ""version"": ""6.10""
          },
          ""version"": ""Wazuh v3.11.4"",
          ""dateAdd"": ""2020-03-27 07:47:46"",
          ""manager"": ""vsec-logstash"",
          ""group"": [
            ""Daklak"",
            ""Test"",
            ""VSEC"",
            ""Daklak_Webs"",
            ""Daklak_Apps""
          ],
          ""mergedSum"": ""f69c8a56d2cd8b401643a52cf423ad1c"",
          ""registerIP"": ""any"",
          ""ip"": ""192.168.2.14"",
          ""name"": ""werserver"",
          ""configSum"": ""4b54b212c2dd96f8b25aebfea2a104cd"",
          ""lastKeepAlive"": ""2020-05-24 11:44:49"",
          ""id"": ""060"",
          ""status"": ""Active"",
          ""node_name"": ""node02"",
          ""timestamp"": ""2020-05-24T11:45:01.853Z"",
          ""host"": ""vsec-logstash"",
          ""cluster"": {
            ""name"": ""wazuh""
          }
        },
        ""fields"": {
          ""timestamp"": [
            ""2020-05-24T11:45:01.853Z""
          ]
        },
        ""highlight"": {
          ""group"": [
            ""@kibana-highlighted-field@Daklak@/kibana-highlighted-field@"",
            ""@kibana-highlighted-field@Daklak_Webs@/kibana-highlighted-field@""
          ]
        },
        ""sort"": [
          -9223372036854776000
        ]
      },
      {
        ""_index"": ""wazuh-monitoring-3.x-2020.05.24"",
        ""_type"": ""_doc"",
        ""_id"": ""SB3uRnIBENNuuStZAbqA"",
        ""_version"": 1,
        ""_score"": null,
        ""_source"": {
          ""os"": {
            ""arch"": ""x86_64"",
            ""major"": ""6"",
            ""minor"": ""8"",
            ""name"": ""CentOS Linux"",
            ""platform"": ""centos"",
            ""uname"": ""Linux |webserver18 |2.6.32-642.el6.x86_64 |#1 SMP Tue May 10 17:27:01 UTC 2016 |x86_64"",
            ""version"": ""6.8""
          },
          ""node_name"": ""node02"",
          ""mergedSum"": ""535237cb82b543ec180a59bde6425db5"",
          ""lastKeepAlive"": ""2020-05-24 13:44:52"",
          ""id"": ""061"",
          ""name"": ""webserver18"",
          ""configSum"": ""d149ab60df71e6fb53b2d38db75bc733"",
          ""registerIP"": ""any"",
          ""group"": [
            ""Daklak"",
            ""Test"",
            ""VSEC"",
            ""Daklak_Webs""
          ],
          ""status"": ""Active"",
          ""version"": ""Wazuh v3.11.4"",
          ""dateAdd"": ""2020-03-31 02:15:08"",
          ""ip"": ""192.168.2.18"",
          ""manager"": ""vsec-logstash"",
          ""timestamp"": ""2020-05-24T13:45:02.584Z"",
          ""host"": ""vsec-logstash"",
          ""cluster"": {
            ""name"": ""wazuh""
          }
        },
        ""fields"": {
          ""timestamp"": [
            ""2020-05-24T13:45:02.584Z""
          ]
        },
        ""highlight"": {
          ""group"": [
            ""@kibana-highlighted-field@Daklak@/kibana-highlighted-field@"",
            ""@kibana-highlighted-field@Daklak_Webs@/kibana-highlighted-field@""
          ]
        },
        ""sort"": [
          -9223372036854776000
        ]
      },
      {
        ""_index"": ""wazuh-monitoring-3.x-2020.05.24"",
        ""_type"": ""_doc"",
        ""_id"": ""yhqpRnIBENNuuStZUzWw"",
        ""_version"": 1,
        ""_score"": null,
        ""_source"": {
          ""os"": {
            ""arch"": ""x86_64"",
            ""major"": ""6"",
            ""minor"": ""10"",
            ""name"": ""CentOS Linux"",
            ""platform"": ""centos"",
            ""uname"": ""Linux |werserver |2.6.32-754.11.1.el6.x86_64 |#1 SMP Tue Feb 26 15:38:56 UTC 2019 |x86_64"",
            ""version"": ""6.10""
          },
          ""group"": [
            ""Daklak"",
            ""Test"",
            ""VSEC"",
            ""Daklak_Webs"",
            ""Daklak_Apps""
          ],
          ""manager"": ""vsec-logstash"",
          ""mergedSum"": ""f69c8a56d2cd8b401643a52cf423ad1c"",
          ""status"": ""Active"",
          ""name"": ""werserver"",
          ""node_name"": ""node02"",
          ""configSum"": ""4b54b212c2dd96f8b25aebfea2a104cd"",
          ""lastKeepAlive"": ""2020-05-24 12:29:49"",
          ""registerIP"": ""any"",
          ""dateAdd"": ""2020-03-27 07:47:46"",
          ""version"": ""Wazuh v3.11.4"",
          ""id"": ""060"",
          ""ip"": ""192.168.2.14"",
          ""timestamp"": ""2020-05-24T12:30:01.648Z"",
          ""host"": ""vsec-logstash"",
          ""cluster"": {
            ""name"": ""wazuh""
          }
        },
        ""fields"": {
          ""timestamp"": [
            ""2020-05-24T12:30:01.648Z""
          ]
        },
        ""highlight"": {
          ""group"": [
            ""@kibana-highlighted-field@Daklak@/kibana-highlighted-field@"",
            ""@kibana-highlighted-field@Daklak_Webs@/kibana-highlighted-field@""
          ]
        },
        ""sort"": [
          -9223372036854776000
        ]
      },
      {
        ""_index"": ""wazuh-monitoring-3.x-2020.05.24"",
        ""_type"": ""_doc"",
        ""_id"": ""yxqpRnIBENNuuStZUzWw"",
        ""_version"": 1,
        ""_score"": null,
        ""_source"": {
          ""os"": {
            ""arch"": ""x86_64"",
            ""major"": ""6"",
            ""minor"": ""8"",
            ""name"": ""CentOS Linux"",
            ""platform"": ""centos"",
            ""uname"": ""Linux |webserver18 |2.6.32-642.el6.x86_64 |#1 SMP Tue May 10 17:27:01 UTC 2016 |x86_64"",
            ""version"": ""6.8""
          },
          ""group"": [
            ""Daklak"",
            ""Test"",
            ""VSEC"",
            ""Daklak_Webs""
          ],
          ""manager"": ""vsec-logstash"",
          ""mergedSum"": ""535237cb82b543ec180a59bde6425db5"",
          ""status"": ""Active"",
          ""name"": ""webserver18"",
          ""node_name"": ""node02"",
          ""configSum"": ""d149ab60df71e6fb53b2d38db75bc733"",
          ""lastKeepAlive"": ""2020-05-24 12:29:43"",
          ""registerIP"": ""any"",
          ""dateAdd"": ""2020-03-31 02:15:08"",
          ""version"": ""Wazuh v3.11.4"",
          ""id"": ""061"",
          ""ip"": ""192.168.2.18"",
          ""timestamp"": ""2020-05-24T12:30:01.648Z"",
          ""host"": ""vsec-logstash"",
          ""cluster"": {
            ""name"": ""wazuh""
          }
        },
        ""fields"": {
          ""timestamp"": [
            ""2020-05-24T12:30:01.648Z""
          ]
        },
        ""highlight"": {
          ""group"": [
            ""@kibana-highlighted-field@Daklak@/kibana-highlighted-field@"",
            ""@kibana-highlighted-field@Daklak_Webs@/kibana-highlighted-field@""
          ]
        },
        ""sort"": [
          -9223372036854776000
        ]
      },
      {
        ""_index"": ""wazuh-monitoring-3.x-2020.05.24"",
        ""_type"": ""_doc"",
        ""_id"": ""5hmbRnIBENNuuStZmoVW"",
        ""_version"": 1,
        ""_score"": null,
        ""_source"": {
          ""os"": {
            ""arch"": ""x86_64"",
            ""major"": ""6"",
            ""minor"": ""10"",
            ""name"": ""CentOS Linux"",
            ""platform"": ""centos"",
            ""uname"": ""Linux |werserver |2.6.32-754.11.1.el6.x86_64 |#1 SMP Tue Feb 26 15:38:56 UTC 2019 |x86_64"",
            ""version"": ""6.10""
          },
          ""status"": ""Active"",
          ""dateAdd"": ""2020-03-27 07:47:46"",
          ""group"": [
            ""Daklak"",
            ""Test"",
            ""VSEC"",
            ""Daklak_Webs"",
            ""Daklak_Apps""
          ],
          ""name"": ""werserver"",
          ""id"": ""060"",
          ""mergedSum"": ""f69c8a56d2cd8b401643a52cf423ad1c"",
          ""lastKeepAlive"": ""2020-05-24 12:14:59"",
          ""configSum"": ""4b54b212c2dd96f8b25aebfea2a104cd"",
          ""manager"": ""vsec-logstash"",
          ""ip"": ""192.168.2.14"",
          ""node_name"": ""node02"",
          ""version"": ""Wazuh v3.11.4"",
          ""registerIP"": ""any"",
          ""timestamp"": ""2020-05-24T12:15:02.231Z"",
          ""host"": ""vsec-logstash"",
          ""cluster"": {
            ""name"": ""wazuh""
          }
        },
        ""fields"": {
          ""timestamp"": [
            ""2020-05-24T12:15:02.231Z""
          ]
        },
        ""highlight"": {
          ""group"": [
            ""@kibana-highlighted-field@Daklak@/kibana-highlighted-field@"",
            ""@kibana-highlighted-field@Daklak_Webs@/kibana-highlighted-field@""
          ]
        },
        ""sort"": [
          -9223372036854776000
        ]
      },
      {
        ""_index"": ""wazuh-monitoring-3.x-2020.05.24"",
        ""_type"": ""_doc"",
        ""_id"": ""5xmbRnIBENNuuStZmoVW"",
        ""_version"": 1,
        ""_score"": null,
        ""_source"": {
          ""os"": {
            ""arch"": ""x86_64"",
            ""major"": ""6"",
            ""minor"": ""8"",
            ""name"": ""CentOS Linux"",
            ""platform"": ""centos"",
            ""uname"": ""Linux |webserver18 |2.6.32-642.el6.x86_64 |#1 SMP Tue May 10 17:27:01 UTC 2016 |x86_64"",
            ""version"": ""6.8""
          },
          ""status"": ""Active"",
          ""dateAdd"": ""2020-03-31 02:15:08"",
          ""group"": [
            ""Daklak"",
            ""Test"",
            ""VSEC"",
            ""Daklak_Webs""
          ],
          ""name"": ""webserver18"",
          ""id"": ""061"",
          ""mergedSum"": ""535237cb82b543ec180a59bde6425db5"",
          ""lastKeepAlive"": ""2020-05-24 12:14:58"",
          ""configSum"": ""d149ab60df71e6fb53b2d38db75bc733"",
          ""manager"": ""vsec-logstash"",
          ""ip"": ""192.168.2.18"",
          ""node_name"": ""node02"",
          ""version"": ""Wazuh v3.11.4"",
          ""registerIP"": ""any"",
          ""timestamp"": ""2020-05-24T12:15:02.231Z"",
          ""host"": ""vsec-logstash"",
          ""cluster"": {
            ""name"": ""wazuh""
          }
        },
        ""fields"": {
          ""timestamp"": [
            ""2020-05-24T12:15:02.231Z""
          ]
        },
        ""highlight"": {
          ""group"": [
            ""@kibana-highlighted-field@Daklak@/kibana-highlighted-field@"",
            ""@kibana-highlighted-field@Daklak_Webs@/kibana-highlighted-field@""
          ]
        },
        ""sort"": [
          -9223372036854776000
        ]
      },
      {
        ""_index"": ""wazuh-monitoring-3.x-2020.05.24"",
        ""_type"": ""_doc"",
        ""_id"": ""3yrlR3IBENNuuStZLmop"",
        ""_version"": 1,
        ""_score"": null,
        ""_source"": {
          ""os"": {
            ""arch"": ""x86_64"",
            ""major"": ""6"",
            ""minor"": ""10"",
            ""name"": ""CentOS Linux"",
            ""platform"": ""centos"",
            ""uname"": ""Linux |werserver |2.6.32-754.11.1.el6.x86_64 |#1 SMP Tue Feb 26 15:38:56 UTC 2019 |x86_64"",
            ""version"": ""6.10""
          },
          ""name"": ""werserver"",
          ""configSum"": ""4b54b212c2dd96f8b25aebfea2a104cd"",
          ""version"": ""Wazuh v3.11.4"",
          ""status"": ""Active"",
          ""node_name"": ""node02"",
          ""mergedSum"": ""f69c8a56d2cd8b401643a52cf423ad1c"",
          ""id"": ""060"",
          ""registerIP"": ""any"",
          ""manager"": ""vsec-logstash"",
          ""group"": [
            ""Daklak"",
            ""Test"",
            ""VSEC"",
            ""Daklak_Webs"",
            ""Daklak_Apps""
          ],
          ""dateAdd"": ""2020-03-27 07:47:46"",
          ""ip"": ""192.168.2.14"",
          ""lastKeepAlive"": ""2020-05-24 18:14:51"",
          ""timestamp"": ""2020-05-24T18:15:01.416Z"",
          ""host"": ""vsec-logstash"",
          ""cluster"": {
            ""name"": ""wazuh""
          }
        },
        ""fields"": {
          ""timestamp"": [
            ""2020-05-24T18:15:01.416Z""
          ]
        },
        ""highlight"": {
          ""group"": [
            ""@kibana-highlighted-field@Daklak@/kibana-highlighted-field@"",
            ""@kibana-highlighted-field@Daklak_Webs@/kibana-highlighted-field@""
          ]
        },
        ""sort"": [
          -9223372036854776000
        ]
      },
      {
        ""_index"": ""wazuh-monitoring-3.x-2020.05.24"",
        ""_type"": ""_doc"",
        ""_id"": ""SSaSR3IBENNuuStZzCwK"",
        ""_version"": 1,
        ""_score"": null,
        ""_source"": {
          ""os"": {
            ""arch"": ""x86_64"",
            ""major"": ""6"",
            ""minor"": ""10"",
            ""name"": ""CentOS Linux"",
            ""platform"": ""centos"",
            ""uname"": ""Linux |werserver |2.6.32-754.11.1.el6.x86_64 |#1 SMP Tue Feb 26 15:38:56 UTC 2019 |x86_64"",
            ""version"": ""6.10""
          },
          ""mergedSum"": ""f69c8a56d2cd8b401643a52cf423ad1c"",
          ""name"": ""werserver"",
          ""manager"": ""vsec-logstash"",
          ""dateAdd"": ""2020-03-27 07:47:46"",
          ""node_name"": ""node02"",
          ""version"": ""Wazuh v3.11.4"",
          ""registerIP"": ""any"",
          ""group"": [
            ""Daklak"",
            ""Test"",
            ""VSEC"",
            ""Daklak_Webs"",
            ""Daklak_Apps""
          ],
          ""lastKeepAlive"": ""2020-05-24 16:44:51"",
          ""status"": ""Active"",
          ""configSum"": ""4b54b212c2dd96f8b25aebfea2a104cd"",
          ""id"": ""060"",
          ""ip"": ""192.168.2.14"",
          ""timestamp"": ""2020-05-24T16:45:02.340Z"",
          ""host"": ""vsec-logstash"",
          ""cluster"": {
            ""name"": ""wazuh""
          }
        },
        ""fields"": {
          ""timestamp"": [
            ""2020-05-24T16:45:02.340Z""
          ]
        },
        ""highlight"": {
          ""group"": [
            ""@kibana-highlighted-field@Daklak@/kibana-highlighted-field@"",
            ""@kibana-highlighted-field@Daklak_Webs@/kibana-highlighted-field@""
          ]
        },
        ""sort"": [
          -9223372036854776000
        ]
      },
      {
        ""_index"": ""wazuh-monitoring-3.x-2020.05.24"",
        ""_type"": ""_doc"",
        ""_id"": ""SiaSR3IBENNuuStZzCwK"",
        ""_version"": 1,
        ""_score"": null,
        ""_source"": {
          ""os"": {
            ""arch"": ""x86_64"",
            ""major"": ""6"",
            ""minor"": ""8"",
            ""name"": ""CentOS Linux"",
            ""platform"": ""centos"",
            ""uname"": ""Linux |webserver18 |2.6.32-642.el6.x86_64 |#1 SMP Tue May 10 17:27:01 UTC 2016 |x86_64"",
            ""version"": ""6.8""
          },
          ""mergedSum"": ""535237cb82b543ec180a59bde6425db5"",
          ""name"": ""webserver18"",
          ""manager"": ""vsec-logstash"",
          ""dateAdd"": ""2020-03-31 02:15:08"",
          ""node_name"": ""node02"",
          ""version"": ""Wazuh v3.11.4"",
          ""registerIP"": ""any"",
          ""group"": [
            ""Daklak"",
            ""Test"",
            ""VSEC"",
            ""Daklak_Webs""
          ],
          ""lastKeepAlive"": ""2020-05-24 16:44:52"",
          ""status"": ""Active"",
          ""configSum"": ""d149ab60df71e6fb53b2d38db75bc733"",
          ""id"": ""061"",
          ""ip"": ""192.168.2.18"",
          ""timestamp"": ""2020-05-24T16:45:02.340Z"",
          ""host"": ""vsec-logstash"",
          ""cluster"": {
            ""name"": ""wazuh""
          }
        },
        ""fields"": {
          ""timestamp"": [
            ""2020-05-24T16:45:02.340Z""
          ]
        },
        ""highlight"": {
          ""group"": [
            ""@kibana-highlighted-field@Daklak@/kibana-highlighted-field@"",
            ""@kibana-highlighted-field@Daklak_Webs@/kibana-highlighted-field@""
          ]
        },
        ""sort"": [
          -9223372036854776000
        ]
      },
      {
        ""_index"": ""wazuh-monitoring-3.x-2020.05.24"",
        ""_type"": ""_doc"",
        ""_id"": ""rSi7R3IBENNuuStZ_UuA"",
        ""_version"": 1,
        ""_score"": null,
        ""_source"": {
          ""os"": {
            ""arch"": ""x86_64"",
            ""major"": ""6"",
            ""minor"": ""8"",
            ""name"": ""CentOS Linux"",
            ""platform"": ""centos"",
            ""uname"": ""Linux |webserver18 |2.6.32-642.el6.x86_64 |#1 SMP Tue May 10 17:27:01 UTC 2016 |x86_64"",
            ""version"": ""6.8""
          },
          ""registerIP"": ""any"",
          ""version"": ""Wazuh v3.11.4"",
          ""node_name"": ""node02"",
          ""name"": ""webserver18"",
          ""manager"": ""vsec-logstash"",
          ""mergedSum"": ""535237cb82b543ec180a59bde6425db5"",
          ""lastKeepAlive"": ""2020-05-24 17:29:54"",
          ""ip"": ""192.168.2.18"",
          ""dateAdd"": ""2020-03-31 02:15:08"",
          ""configSum"": ""d149ab60df71e6fb53b2d38db75bc733"",
          ""group"": [
            ""Daklak"",
            ""Test"",
            ""VSEC"",
            ""Daklak_Webs""
          ],
          ""status"": ""Active"",
          ""id"": ""061"",
          ""timestamp"": ""2020-05-24T17:30:01.979Z"",
          ""host"": ""vsec-logstash"",
          ""cluster"": {
            ""name"": ""wazuh""
          }
        },
        ""fields"": {
          ""timestamp"": [
            ""2020-05-24T17:30:01.979Z""
          ]
        },
        ""highlight"": {
          ""group"": [
            ""@kibana-highlighted-field@Daklak@/kibana-highlighted-field@"",
            ""@kibana-highlighted-field@Daklak_Webs@/kibana-highlighted-field@""
          ]
        },
        ""sort"": [
          -9223372036854776000
        ]
      },
      {
        ""_index"": ""wazuh-monitoring-3.x-2020.05.24"",
        ""_type"": ""_doc"",
        ""_id"": ""6CR3R3IBENNuuStZUcfC"",
        ""_version"": 1,
        ""_score"": null,
        ""_source"": {
          ""os"": {
            ""arch"": ""x86_64"",
            ""major"": ""6"",
            ""minor"": ""8"",
            ""name"": ""CentOS Linux"",
            ""platform"": ""centos"",
            ""uname"": ""Linux |webserver18 |2.6.32-642.el6.x86_64 |#1 SMP Tue May 10 17:27:01 UTC 2016 |x86_64"",
            ""version"": ""6.8""
          },
          ""dateAdd"": ""2020-03-31 02:15:08"",
          ""lastKeepAlive"": ""2020-05-24 16:15:01"",
          ""configSum"": ""d149ab60df71e6fb53b2d38db75bc733"",
          ""mergedSum"": ""535237cb82b543ec180a59bde6425db5"",
          ""manager"": ""vsec-logstash"",
          ""status"": ""Active"",
          ""ip"": ""192.168.2.18"",
          ""registerIP"": ""any"",
          ""group"": [
            ""Daklak"",
            ""Test"",
            ""VSEC"",
            ""Daklak_Webs""
          ],
          ""version"": ""Wazuh v3.11.4"",
          ""name"": ""webserver18"",
          ""node_name"": ""node02"",
          ""id"": ""061"",
          ""timestamp"": ""2020-05-24T16:15:01.569Z"",
          ""host"": ""vsec-logstash"",
          ""cluster"": {
            ""name"": ""wazuh""
          }
        },
        ""fields"": {
          ""timestamp"": [
            ""2020-05-24T16:15:01.569Z""
          ]
        },
        ""highlight"": {
          ""group"": [
            ""@kibana-highlighted-field@Daklak@/kibana-highlighted-field@"",
            ""@kibana-highlighted-field@Daklak_Webs@/kibana-highlighted-field@""
          ]
        },
        ""sort"": [
          -9223372036854776000
        ]
      },
      {
        ""_index"": ""wazuh-monitoring-3.x-2020.05.24"",
        ""_type"": ""_doc"",
        ""_id"": ""vynXR3IBENNuuStZdK_A"",
        ""_version"": 1,
        ""_score"": null,
        ""_source"": {
          ""os"": {
            ""arch"": ""x86_64"",
            ""major"": ""6"",
            ""minor"": ""10"",
            ""name"": ""CentOS Linux"",
            ""platform"": ""centos"",
            ""uname"": ""Linux |werserver |2.6.32-754.11.1.el6.x86_64 |#1 SMP Tue Feb 26 15:38:56 UTC 2019 |x86_64"",
            ""version"": ""6.10""
          },
          ""node_name"": ""node02"",
          ""dateAdd"": ""2020-03-27 07:47:46"",
          ""status"": ""Active"",
          ""lastKeepAlive"": ""2020-05-24 17:59:52"",
          ""registerIP"": ""any"",
          ""configSum"": ""4b54b212c2dd96f8b25aebfea2a104cd"",
          ""group"": [
            ""Daklak"",
            ""Test"",
            ""VSEC"",
            ""Daklak_Webs"",
            ""Daklak_Apps""
          ],
          ""manager"": ""vsec-logstash"",
          ""id"": ""060"",
          ""name"": ""werserver"",
          ""version"": ""Wazuh v3.11.4"",
          ""mergedSum"": ""f69c8a56d2cd8b401643a52cf423ad1c"",
          ""ip"": ""192.168.2.14"",
          ""timestamp"": ""2020-05-24T18:00:01.979Z"",
          ""host"": ""vsec-logstash"",
          ""cluster"": {
            ""name"": ""wazuh""
          }
        },
        ""fields"": {
          ""timestamp"": [
            ""2020-05-24T18:00:01.979Z""
          ]
        },
        ""highlight"": {
          ""group"": [
            ""@kibana-highlighted-field@Daklak@/kibana-highlighted-field@"",
            ""@kibana-highlighted-field@Daklak_Webs@/kibana-highlighted-field@""
          ]
        },
        ""sort"": [
          -9223372036854776000
        ]
      },
      {
        ""_index"": ""wazuh-monitoring-3.x-2020.05.24"",
        ""_type"": ""_doc"",
        ""_id"": ""ECagR3IBENNuuStZhtyq"",
        ""_version"": 1,
        ""_score"": null,
        ""_source"": {
          ""os"": {
            ""arch"": ""x86_64"",
            ""major"": ""6"",
            ""minor"": ""10"",
            ""name"": ""CentOS Linux"",
            ""platform"": ""centos"",
            ""uname"": ""Linux |werserver |2.6.32-754.11.1.el6.x86_64 |#1 SMP Tue Feb 26 15:38:56 UTC 2019 |x86_64"",
            ""version"": ""6.10""
          },
          ""node_name"": ""node02"",
          ""registerIP"": ""any"",
          ""id"": ""060"",
          ""dateAdd"": ""2020-03-27 07:47:46"",
          ""ip"": ""192.168.2.14"",
          ""lastKeepAlive"": ""2020-05-24 16:59:51"",
          ""manager"": ""vsec-logstash"",
          ""status"": ""Active"",
          ""version"": ""Wazuh v3.11.4"",
          ""mergedSum"": ""f69c8a56d2cd8b401643a52cf423ad1c"",
          ""configSum"": ""4b54b212c2dd96f8b25aebfea2a104cd"",
          ""name"": ""werserver"",
          ""group"": [
            ""Daklak"",
            ""Test"",
            ""VSEC"",
            ""Daklak_Webs"",
            ""Daklak_Apps""
          ],
          ""timestamp"": ""2020-05-24T17:00:02.085Z"",
          ""host"": ""vsec-logstash"",
          ""cluster"": {
            ""name"": ""wazuh""
          }
        },
        ""fields"": {
          ""timestamp"": [
            ""2020-05-24T17:00:02.085Z""
          ]
        },
        ""highlight"": {
          ""group"": [
            ""@kibana-highlighted-field@Daklak@/kibana-highlighted-field@"",
            ""@kibana-highlighted-field@Daklak_Webs@/kibana-highlighted-field@""
          ]
        },
        ""sort"": [
          -9223372036854776000
        ]
      },
      {
        ""_index"": ""wazuh-monitoring-3.x-2020.05.24"",
        ""_type"": ""_doc"",
        ""_id"": ""ESagR3IBENNuuStZhtyq"",
        ""_version"": 1,
        ""_score"": null,
        ""_source"": {
          ""os"": {
            ""arch"": ""x86_64"",
            ""major"": ""6"",
            ""minor"": ""8"",
            ""name"": ""CentOS Linux"",
            ""platform"": ""centos"",
            ""uname"": ""Linux |webserver18 |2.6.32-642.el6.x86_64 |#1 SMP Tue May 10 17:27:01 UTC 2016 |x86_64"",
            ""version"": ""6.8""
          },
          ""node_name"": ""node02"",
          ""registerIP"": ""any"",
          ""id"": ""061"",
          ""dateAdd"": ""2020-03-31 02:15:08"",
          ""ip"": ""192.168.2.18"",
          ""lastKeepAlive"": ""2020-05-24 16:59:55"",
          ""manager"": ""vsec-logstash"",
          ""status"": ""Active"",
          ""version"": ""Wazuh v3.11.4"",
          ""mergedSum"": ""535237cb82b543ec180a59bde6425db5"",
          ""configSum"": ""d149ab60df71e6fb53b2d38db75bc733"",
          ""name"": ""webserver18"",
          ""group"": [
            ""Daklak"",
            ""Test"",
            ""VSEC"",
            ""Daklak_Webs""
          ],
          ""timestamp"": ""2020-05-24T17:00:02.085Z"",
          ""host"": ""vsec-logstash"",
          ""cluster"": {
            ""name"": ""wazuh""
          }
        },
        ""fields"": {
          ""timestamp"": [
            ""2020-05-24T17:00:02.085Z""
          ]
        },
        ""highlight"": {
          ""group"": [
            ""@kibana-highlighted-field@Daklak@/kibana-highlighted-field@"",
            ""@kibana-highlighted-field@Daklak_Webs@/kibana-highlighted-field@""
          ]
        },
        ""sort"": [
          -9223372036854776000
        ]
      },
      {
        ""_index"": ""wazuh-monitoring-3.x-2020.05.24"",
        ""_type"": ""_doc"",
        ""_id"": ""5SWFR3IBENNuuStZDnmi"",
        ""_version"": 1,
        ""_score"": null,
        ""_source"": {
          ""os"": {
            ""arch"": ""x86_64"",
            ""major"": ""6"",
            ""minor"": ""10"",
            ""name"": ""CentOS Linux"",
            ""platform"": ""centos"",
            ""uname"": ""Linux |werserver |2.6.32-754.11.1.el6.x86_64 |#1 SMP Tue Feb 26 15:38:56 UTC 2019 |x86_64"",
            ""version"": ""6.10""
          },
          ""version"": ""Wazuh v3.11.4"",
          ""lastKeepAlive"": ""2020-05-24 16:29:49"",
          ""ip"": ""192.168.2.14"",
          ""id"": ""060"",
          ""dateAdd"": ""2020-03-27 07:47:46"",
          ""configSum"": ""4b54b212c2dd96f8b25aebfea2a104cd"",
          ""group"": [
            ""Daklak"",
            ""Test"",
            ""VSEC"",
            ""Daklak_Webs"",
            ""Daklak_Apps""
          ],
          ""name"": ""werserver"",
          ""mergedSum"": ""f69c8a56d2cd8b401643a52cf423ad1c"",
          ""status"": ""Active"",
          ""node_name"": ""node02"",
          ""manager"": ""vsec-logstash"",
          ""registerIP"": ""any"",
          ""timestamp"": ""2020-05-24T16:30:01.889Z"",
          ""host"": ""vsec-logstash"",
          ""cluster"": {
            ""name"": ""wazuh""
          }
        },
        ""fields"": {
          ""timestamp"": [
            ""2020-05-24T16:30:01.889Z""
          ]
        },
        ""highlight"": {
          ""group"": [
            ""@kibana-highlighted-field@Daklak@/kibana-highlighted-field@"",
            ""@kibana-highlighted-field@Daklak_Webs@/kibana-highlighted-field@""
          ]
        },
        ""sort"": [
          -9223372036854776000
        ]
      },
      {
        ""_index"": ""wazuh-monitoring-3.x-2020.05.24"",
        ""_type"": ""_doc"",
        ""_id"": ""LTJ8SHIBENNuuStZPxu0"",
        ""_version"": 1,
        ""_score"": null,
        ""_source"": {
          ""os"": {
            ""arch"": ""x86_64"",
            ""major"": ""6"",
            ""minor"": ""10"",
            ""name"": ""CentOS Linux"",
            ""platform"": ""centos"",
            ""uname"": ""Linux |werserver |2.6.32-754.11.1.el6.x86_64 |#1 SMP Tue Feb 26 15:38:56 UTC 2019 |x86_64"",
            ""version"": ""6.10""
          },
          ""lastKeepAlive"": ""2020-05-24 20:59:50"",
          ""mergedSum"": ""f69c8a56d2cd8b401643a52cf423ad1c"",
          ""node_name"": ""node02"",
          ""id"": ""060"",
          ""group"": [
            ""Daklak"",
            ""Test"",
            ""VSEC"",
            ""Daklak_Webs"",
            ""Daklak_Apps""
          ],
          ""dateAdd"": ""2020-03-27 07:47:46"",
          ""registerIP"": ""any"",
          ""ip"": ""192.168.2.14"",
          ""manager"": ""vsec-logstash"",
          ""status"": ""Active"",
          ""name"": ""werserver"",
          ""configSum"": ""4b54b212c2dd96f8b25aebfea2a104cd"",
          ""version"": ""Wazuh v3.11.4"",
          ""timestamp"": ""2020-05-24T21:00:01.846Z"",
          ""host"": ""vsec-logstash"",
          ""cluster"": {
            ""name"": ""wazuh""
          }
        },
        ""fields"": {
          ""timestamp"": [
            ""2020-05-24T21:00:01.846Z""
          ]
        },
        ""highlight"": {
          ""group"": [
            ""@kibana-highlighted-field@Daklak@/kibana-highlighted-field@"",
            ""@kibana-highlighted-field@Daklak_Webs@/kibana-highlighted-field@""
          ]
        },
        ""sort"": [
          -9223372036854776000
        ]
      },
      {
        ""_index"": ""wazuh-monitoring-3.x-2020.05.24"",
        ""_type"": ""_doc"",
        ""_id"": ""LjJ8SHIBENNuuStZPxu0"",
        ""_version"": 1,
        ""_score"": null,
        ""_source"": {
          ""os"": {
            ""arch"": ""x86_64"",
            ""major"": ""6"",
            ""minor"": ""8"",
            ""name"": ""CentOS Linux"",
            ""platform"": ""centos"",
            ""uname"": ""Linux |webserver18 |2.6.32-642.el6.x86_64 |#1 SMP Tue May 10 17:27:01 UTC 2016 |x86_64"",
            ""version"": ""6.8""
          },
          ""lastKeepAlive"": ""2020-05-24 20:59:43"",
          ""mergedSum"": ""535237cb82b543ec180a59bde6425db5"",
          ""node_name"": ""node02"",
          ""id"": ""061"",
          ""group"": [
            ""Daklak"",
            ""Test"",
            ""VSEC"",
            ""Daklak_Webs""
          ],
          ""dateAdd"": ""2020-03-31 02:15:08"",
          ""registerIP"": ""any"",
          ""ip"": ""192.168.2.18"",
          ""manager"": ""vsec-logstash"",
          ""status"": ""Active"",
          ""name"": ""webserver18"",
          ""configSum"": ""d149ab60df71e6fb53b2d38db75bc733"",
          ""version"": ""Wazuh v3.11.4"",
          ""timestamp"": ""2020-05-24T21:00:01.846Z"",
          ""host"": ""vsec-logstash"",
          ""cluster"": {
            ""name"": ""wazuh""
          }
        },
        ""fields"": {
          ""timestamp"": [
            ""2020-05-24T21:00:01.846Z""
          ]
        },
        ""highlight"": {
          ""group"": [
            ""@kibana-highlighted-field@Daklak@/kibana-highlighted-field@"",
            ""@kibana-highlighted-field@Daklak_Webs@/kibana-highlighted-field@""
          ]
        },
        ""sort"": [
          -9223372036854776000
        ]
      },
      {
        ""_index"": ""wazuh-monitoring-3.x-2020.05.24"",
        ""_type"": ""_doc"",
        ""_id"": ""JTfcSHIBENNuuStZYgdh"",
        ""_version"": 1,
        ""_score"": null,
        ""_source"": {
          ""os"": {
            ""arch"": ""x86_64"",
            ""major"": ""6"",
            ""minor"": ""8"",
            ""name"": ""CentOS Linux"",
            ""platform"": ""centos"",
            ""uname"": ""Linux |webserver18 |2.6.32-642.el6.x86_64 |#1 SMP Tue May 10 17:27:01 UTC 2016 |x86_64"",
            ""version"": ""6.8""
          },
          ""node_name"": ""node02"",
          ""mergedSum"": ""535237cb82b543ec180a59bde6425db5"",
          ""manager"": ""vsec-logstash"",
          ""group"": [
            ""Daklak"",
            ""Test"",
            ""VSEC"",
            ""Daklak_Webs""
          ],
          ""ip"": ""192.168.2.18"",
          ""registerIP"": ""any"",
          ""configSum"": ""d149ab60df71e6fb53b2d38db75bc733"",
          ""lastKeepAlive"": ""2020-05-24 22:44:46"",
          ""id"": ""061"",
          ""name"": ""webserver18"",
          ""dateAdd"": ""2020-03-31 02:15:08"",
          ""status"": ""Active"",
          ""version"": ""Wazuh v3.11.4"",
          ""timestamp"": ""2020-05-24T22:45:02.175Z"",
          ""host"": ""vsec-logstash"",
          ""cluster"": {
            ""name"": ""wazuh""
          }
        },
        ""fields"": {
          ""timestamp"": [
            ""2020-05-24T22:45:02.175Z""
          ]
        },
        ""highlight"": {
          ""group"": [
            ""@kibana-highlighted-field@Daklak@/kibana-highlighted-field@"",
            ""@kibana-highlighted-field@Daklak_Webs@/kibana-highlighted-field@""
          ]
        },
        ""sort"": [
          -9223372036854776000
        ]
      },
      {
        ""_index"": ""wazuh-monitoring-3.x-2020.05.24"",
        ""_type"": ""_doc"",
        ""_id"": ""qDKJSHIBENNuuStZ_dYr"",
        ""_version"": 1,
        ""_score"": null,
        ""_source"": {
          ""os"": {
            ""arch"": ""x86_64"",
            ""major"": ""6"",
            ""minor"": ""10"",
            ""name"": ""CentOS Linux"",
            ""platform"": ""centos"",
            ""uname"": ""Linux |werserver |2.6.32-754.11.1.el6.x86_64 |#1 SMP Tue Feb 26 15:38:56 UTC 2019 |x86_64"",
            ""version"": ""6.10""
          },
          ""version"": ""Wazuh v3.11.4"",
          ""registerIP"": ""any"",
          ""group"": [
            ""Daklak"",
            ""Test"",
            ""VSEC"",
            ""Daklak_Webs"",
            ""Daklak_Apps""
          ],
          ""name"": ""werserver"",
          ""manager"": ""vsec-logstash"",
          ""dateAdd"": ""2020-03-27 07:47:46"",
          ""mergedSum"": ""f69c8a56d2cd8b401643a52cf423ad1c"",
          ""configSum"": ""4b54b212c2dd96f8b25aebfea2a104cd"",
          ""lastKeepAlive"": ""2020-05-24 21:14:51"",
          ""id"": ""060"",
          ""node_name"": ""node02"",
          ""status"": ""Active"",
          ""ip"": ""192.168.2.14"",
          ""timestamp"": ""2020-05-24T21:15:02.315Z"",
          ""host"": ""vsec-logstash"",
          ""cluster"": {
            ""name"": ""wazuh""
          }
        },
        ""fields"": {
          ""timestamp"": [
            ""2020-05-24T21:15:02.315Z""
          ]
        },
        ""highlight"": {
          ""group"": [
            ""@kibana-highlighted-field@Daklak@/kibana-highlighted-field@"",
            ""@kibana-highlighted-field@Daklak_Webs@/kibana-highlighted-field@""
          ]
        },
        ""sort"": [
          -9223372036854776000
        ]
      },
      {
        ""_index"": ""wazuh-monitoring-3.x-2020.05.24"",
        ""_type"": ""_doc"",
        ""_id"": ""qTKJSHIBENNuuStZ_dYr"",
        ""_version"": 1,
        ""_score"": null,
        ""_source"": {
          ""os"": {
            ""arch"": ""x86_64"",
            ""major"": ""6"",
            ""minor"": ""8"",
            ""name"": ""CentOS Linux"",
            ""platform"": ""centos"",
            ""uname"": ""Linux |webserver18 |2.6.32-642.el6.x86_64 |#1 SMP Tue May 10 17:27:01 UTC 2016 |x86_64"",
            ""version"": ""6.8""
          },
          ""version"": ""Wazuh v3.11.4"",
          ""registerIP"": ""any"",
          ""group"": [
            ""Daklak"",
            ""Test"",
            ""VSEC"",
            ""Daklak_Webs""
          ],
          ""name"": ""webserver18"",
          ""manager"": ""vsec-logstash"",
          ""dateAdd"": ""2020-03-31 02:15:08"",
          ""mergedSum"": ""535237cb82b543ec180a59bde6425db5"",
          ""configSum"": ""d149ab60df71e6fb53b2d38db75bc733"",
          ""lastKeepAlive"": ""2020-05-24 21:14:53"",
          ""id"": ""061"",
          ""node_name"": ""node02"",
          ""status"": ""Active"",
          ""ip"": ""192.168.2.18"",
          ""timestamp"": ""2020-05-24T21:15:02.315Z"",
          ""host"": ""vsec-logstash"",
          ""cluster"": {
            ""name"": ""wazuh""
          }
        },
        ""fields"": {
          ""timestamp"": [
            ""2020-05-24T21:15:02.315Z""
          ]
        },
        ""highlight"": {
          ""group"": [
            ""@kibana-highlighted-field@Daklak@/kibana-highlighted-field@"",
            ""@kibana-highlighted-field@Daklak_Webs@/kibana-highlighted-field@""
          ]
        },
        ""sort"": [
          -9223372036854776000
        ]
      },
      {
        ""_index"": ""wazuh-monitoring-3.x-2020.05.24"",
        ""_type"": ""_doc"",
        ""_id"": ""CDSlSHIBENNuuStZdDkt"",
        ""_version"": 1,
        ""_score"": null,
        ""_source"": {
          ""os"": {
            ""arch"": ""x86_64"",
            ""major"": ""6"",
            ""minor"": ""8"",
            ""name"": ""CentOS Linux"",
            ""platform"": ""centos"",
            ""uname"": ""Linux |webserver18 |2.6.32-642.el6.x86_64 |#1 SMP Tue May 10 17:27:01 UTC 2016 |x86_64"",
            ""version"": ""6.8""
          },
          ""ip"": ""192.168.2.18"",
          ""registerIP"": ""any"",
          ""configSum"": ""d149ab60df71e6fb53b2d38db75bc733"",
          ""lastKeepAlive"": ""2020-05-24 21:44:53"",
          ""id"": ""061"",
          ""status"": ""Active"",
          ""mergedSum"": ""535237cb82b543ec180a59bde6425db5"",
          ""manager"": ""vsec-logstash"",
          ""version"": ""Wazuh v3.11.4"",
          ""group"": [
            ""Daklak"",
            ""Test"",
            ""VSEC"",
            ""Daklak_Webs""
          ],
          ""name"": ""webserver18"",
          ""dateAdd"": ""2020-03-31 02:15:08"",
          ""node_name"": ""node02"",
          ""timestamp"": ""2020-05-24T21:45:02.249Z"",
          ""host"": ""vsec-logstash"",
          ""cluster"": {
            ""name"": ""wazuh""
          }
        },
        ""fields"": {
          ""timestamp"": [
            ""2020-05-24T21:45:02.249Z""
          ]
        },
        ""highlight"": {
          ""group"": [
            ""@kibana-highlighted-field@Daklak@/kibana-highlighted-field@"",
            ""@kibana-highlighted-field@Daklak_Webs@/kibana-highlighted-field@""
          ]
        },
        ""sort"": [
          -9223372036854776000
        ]
      },
      {
        ""_index"": ""wazuh-monitoring-3.x-2020.05.24"",
        ""_type"": ""_doc"",
        ""_id"": ""pjFuSHIBENNuuStZhmpn"",
        ""_version"": 1,
        ""_score"": null,
        ""_source"": {
          ""os"": {
            ""arch"": ""x86_64"",
            ""major"": ""6"",
            ""minor"": ""10"",
            ""name"": ""CentOS Linux"",
            ""platform"": ""centos"",
            ""uname"": ""Linux |werserver |2.6.32-754.11.1.el6.x86_64 |#1 SMP Tue Feb 26 15:38:56 UTC 2019 |x86_64"",
            ""version"": ""6.10""
          },
          ""id"": ""060"",
          ""manager"": ""vsec-logstash"",
          ""mergedSum"": ""f69c8a56d2cd8b401643a52cf423ad1c"",
          ""registerIP"": ""any"",
          ""configSum"": ""4b54b212c2dd96f8b25aebfea2a104cd"",
          ""name"": ""werserver"",
          ""lastKeepAlive"": ""2020-05-24 20:45:00"",
          ""node_name"": ""node02"",
          ""version"": ""Wazuh v3.11.4"",
          ""ip"": ""192.168.2.14"",
          ""group"": [
            ""Daklak"",
            ""Test"",
            ""VSEC"",
            ""Daklak_Webs"",
            ""Daklak_Apps""
          ],
          ""status"": ""Active"",
          ""dateAdd"": ""2020-03-27 07:47:46"",
          ""timestamp"": ""2020-05-24T20:45:02.437Z"",
          ""host"": ""vsec-logstash"",
          ""cluster"": {
            ""name"": ""wazuh""
          }
        },
        ""fields"": {
          ""timestamp"": [
            ""2020-05-24T20:45:02.437Z""
          ]
        },
        ""highlight"": {
          ""group"": [
            ""@kibana-highlighted-field@Daklak@/kibana-highlighted-field@"",
            ""@kibana-highlighted-field@Daklak_Webs@/kibana-highlighted-field@""
          ]
        },
        ""sort"": [
          -9223372036854776000
        ]
      },
      {
        ""_index"": ""wazuh-monitoring-3.x-2020.05.24"",
        ""_type"": ""_doc"",
        ""_id"": ""8DfqSHIBENNuuStZG7fS"",
        ""_version"": 1,
        ""_score"": null,
        ""_source"": {
          ""os"": {
            ""arch"": ""x86_64"",
            ""major"": ""6"",
            ""minor"": ""10"",
            ""name"": ""CentOS Linux"",
            ""platform"": ""centos"",
            ""uname"": ""Linux |werserver |2.6.32-754.11.1.el6.x86_64 |#1 SMP Tue Feb 26 15:38:56 UTC 2019 |x86_64"",
            ""version"": ""6.10""
          },
          ""node_name"": ""node02"",
          ""configSum"": ""4b54b212c2dd96f8b25aebfea2a104cd"",
          ""group"": [
            ""Daklak"",
            ""Test"",
            ""VSEC"",
            ""Daklak_Webs"",
            ""Daklak_Apps""
          ],
          ""mergedSum"": ""f69c8a56d2cd8b401643a52cf423ad1c"",
          ""lastKeepAlive"": ""2020-05-24 22:59:50"",
          ""status"": ""Active"",
          ""ip"": ""192.168.2.14"",
          ""version"": ""Wazuh v3.11.4"",
          ""name"": ""werserver"",
          ""manager"": ""vsec-logstash"",
          ""id"": ""060"",
          ""dateAdd"": ""2020-03-27 07:47:46"",
          ""registerIP"": ""any"",
          ""timestamp"": ""2020-05-24T23:00:01.616Z"",
          ""host"": ""vsec-logstash"",
          ""cluster"": {
            ""name"": ""wazuh""
          }
        },
        ""fields"": {
          ""timestamp"": [
            ""2020-05-24T23:00:01.616Z""
          ]
        },
        ""highlight"": {
          ""group"": [
            ""@kibana-highlighted-field@Daklak@/kibana-highlighted-field@"",
            ""@kibana-highlighted-field@Daklak_Webs@/kibana-highlighted-field@""
          ]
        },
        ""sort"": [
          -9223372036854776000
        ]
      },
      {
        ""_index"": ""wazuh-monitoring-3.x-2020.05.24"",
        ""_type"": ""_doc"",
        ""_id"": ""AjkFSXIBENNuuStZkyL-"",
        ""_version"": 1,
        ""_score"": null,
        ""_source"": {
          ""os"": {
            ""arch"": ""x86_64"",
            ""major"": ""6"",
            ""minor"": ""10"",
            ""name"": ""CentOS Linux"",
            ""platform"": ""centos"",
            ""uname"": ""Linux |werserver |2.6.32-754.11.1.el6.x86_64 |#1 SMP Tue Feb 26 15:38:56 UTC 2019 |x86_64"",
            ""version"": ""6.10""
          },
          ""dateAdd"": ""2020-03-27 07:47:46"",
          ""node_name"": ""node02"",
          ""group"": [
            ""Daklak"",
            ""Test"",
            ""VSEC"",
            ""Daklak_Webs"",
            ""Daklak_Apps""
          ],
          ""id"": ""060"",
          ""name"": ""werserver"",
          ""version"": ""Wazuh v3.11.4"",
          ""registerIP"": ""any"",
          ""status"": ""Active"",
          ""ip"": ""192.168.2.14"",
          ""mergedSum"": ""f69c8a56d2cd8b401643a52cf423ad1c"",
          ""manager"": ""vsec-logstash"",
          ""lastKeepAlive"": ""2020-05-24 23:29:51"",
          ""configSum"": ""4b54b212c2dd96f8b25aebfea2a104cd"",
          ""timestamp"": ""2020-05-24T23:30:01.854Z"",
          ""host"": ""vsec-logstash"",
          ""cluster"": {
            ""name"": ""wazuh""
          }
        },
        ""fields"": {
          ""timestamp"": [
            ""2020-05-24T23:30:01.854Z""
          ]
        },
        ""highlight"": {
          ""group"": [
            ""@kibana-highlighted-field@Daklak@/kibana-highlighted-field@"",
            ""@kibana-highlighted-field@Daklak_Webs@/kibana-highlighted-field@""
          ]
        },
        ""sort"": [
          -9223372036854776000
        ]
      },
      {
        ""_index"": ""wazuh-monitoring-3.x-2020.05.24"",
        ""_type"": ""_doc"",
        ""_id"": ""AzkFSXIBENNuuStZkyL-"",
        ""_version"": 1,
        ""_score"": null,
        ""_source"": {
          ""os"": {
            ""arch"": ""x86_64"",
            ""major"": ""6"",
            ""minor"": ""8"",
            ""name"": ""CentOS Linux"",
            ""platform"": ""centos"",
            ""uname"": ""Linux |webserver18 |2.6.32-642.el6.x86_64 |#1 SMP Tue May 10 17:27:01 UTC 2016 |x86_64"",
            ""version"": ""6.8""
          },
          ""dateAdd"": ""2020-03-31 02:15:08"",
          ""node_name"": ""node02"",
          ""group"": [
            ""Daklak"",
            ""Test"",
            ""VSEC"",
            ""Daklak_Webs""
          ],
          ""id"": ""061"",
          ""name"": ""webserver18"",
          ""version"": ""Wazuh v3.11.4"",
          ""registerIP"": ""any"",
          ""status"": ""Active"",
          ""ip"": ""192.168.2.18"",
          ""mergedSum"": ""535237cb82b543ec180a59bde6425db5"",
          ""manager"": ""vsec-logstash"",
          ""lastKeepAlive"": ""2020-05-24 23:29:56"",
          ""configSum"": ""d149ab60df71e6fb53b2d38db75bc733"",
          ""timestamp"": ""2020-05-24T23:30:01.854Z"",
          ""host"": ""vsec-logstash"",
          ""cluster"": {
            ""name"": ""wazuh""
          }
        },
        ""fields"": {
          ""timestamp"": [
            ""2020-05-24T23:30:01.854Z""
          ]
        },
        ""highlight"": {
          ""group"": [
            ""@kibana-highlighted-field@Daklak@/kibana-highlighted-field@"",
            ""@kibana-highlighted-field@Daklak_Webs@/kibana-highlighted-field@""
          ]
        },
        ""sort"": [
          -9223372036854776000
        ]
      },
      {
        ""_index"": ""wazuh-monitoring-3.x-2020.05.25"",
        ""_type"": ""_doc"",
        ""_id"": ""xDohSXIBENNuuStZDINp"",
        ""_version"": 1,
        ""_score"": null,
        ""_source"": {
          ""os"": {
            ""arch"": ""x86_64"",
            ""major"": ""6"",
            ""minor"": ""10"",
            ""name"": ""CentOS Linux"",
            ""platform"": ""centos"",
            ""uname"": ""Linux |werserver |2.6.32-754.11.1.el6.x86_64 |#1 SMP Tue Feb 26 15:38:56 UTC 2019 |x86_64"",
            ""version"": ""6.10""
          },
          ""node_name"": ""node02"",
          ""dateAdd"": ""2020-03-27 07:47:46"",
          ""configSum"": ""4b54b212c2dd96f8b25aebfea2a104cd"",
          ""ip"": ""192.168.2.14"",
          ""version"": ""Wazuh v3.11.4"",
          ""status"": ""Active"",
          ""registerIP"": ""any"",
          ""lastKeepAlive"": ""2020-05-24 23:59:49"",
          ""manager"": ""vsec-logstash"",
          ""name"": ""werserver"",
          ""mergedSum"": ""f69c8a56d2cd8b401643a52cf423ad1c"",
          ""id"": ""060"",
          ""group"": [
            ""Daklak"",
            ""Test"",
            ""VSEC"",
            ""Daklak_Webs"",
            ""Daklak_Apps""
          ],
          ""timestamp"": ""2020-05-25T00:00:02.147Z"",
          ""host"": ""vsec-logstash"",
          ""cluster"": {
            ""name"": ""wazuh""
          }
        },
        ""fields"": {
          ""timestamp"": [
            ""2020-05-25T00:00:02.147Z""
          ]
        },
        ""highlight"": {
          ""group"": [
            ""@kibana-highlighted-field@Daklak@/kibana-highlighted-field@"",
            ""@kibana-highlighted-field@Daklak_Webs@/kibana-highlighted-field@""
          ]
        },
        ""sort"": [
          -9223372036854776000
        ]
      },
      {
        ""_index"": ""wazuh-monitoring-3.x-2020.05.25"",
        ""_type"": ""_doc"",
        ""_id"": ""UzsuSXIBENNuuStZyT8y"",
        ""_version"": 1,
        ""_score"": null,
        ""_source"": {
          ""os"": {
            ""arch"": ""x86_64"",
            ""major"": ""6"",
            ""minor"": ""10"",
            ""name"": ""CentOS Linux"",
            ""platform"": ""centos"",
            ""uname"": ""Linux |werserver |2.6.32-754.11.1.el6.x86_64 |#1 SMP Tue Feb 26 15:38:56 UTC 2019 |x86_64"",
            ""version"": ""6.10""
          },
          ""node_name"": ""node02"",
          ""name"": ""werserver"",
          ""lastKeepAlive"": ""2020-05-25 00:14:51"",
          ""configSum"": ""4b54b212c2dd96f8b25aebfea2a104cd"",
          ""manager"": ""vsec-logstash"",
          ""registerIP"": ""any"",
          ""group"": [
            ""Daklak"",
            ""Test"",
            ""VSEC"",
            ""Daklak_Webs"",
            ""Daklak_Apps""
          ],
          ""ip"": ""192.168.2.14"",
          ""mergedSum"": ""f69c8a56d2cd8b401643a52cf423ad1c"",
          ""dateAdd"": ""2020-03-27 07:47:46"",
          ""id"": ""060"",
          ""version"": ""Wazuh v3.11.4"",
          ""status"": ""Active"",
          ""timestamp"": ""2020-05-25T00:15:02.445Z"",
          ""host"": ""vsec-logstash"",
          ""cluster"": {
            ""name"": ""wazuh""
          }
        },
        ""fields"": {
          ""timestamp"": [
            ""2020-05-25T00:15:02.445Z""
          ]
        },
        ""highlight"": {
          ""group"": [
            ""@kibana-highlighted-field@Daklak@/kibana-highlighted-field@"",
            ""@kibana-highlighted-field@Daklak_Webs@/kibana-highlighted-field@""
          ]
        },
        ""sort"": [
          -9223372036854776000
        ]
      },
      {
        ""_index"": ""wazuh-monitoring-3.x-2020.05.25"",
        ""_type"": ""_doc"",
        ""_id"": ""VDsuSXIBENNuuStZyT8y"",
        ""_version"": 1,
        ""_score"": null,
        ""_source"": {
          ""os"": {
            ""arch"": ""x86_64"",
            ""major"": ""6"",
            ""minor"": ""8"",
            ""name"": ""CentOS Linux"",
            ""platform"": ""centos"",
            ""uname"": ""Linux |webserver18 |2.6.32-642.el6.x86_64 |#1 SMP Tue May 10 17:27:01 UTC 2016 |x86_64"",
            ""version"": ""6.8""
          },
          ""node_name"": ""node02"",
          ""name"": ""webserver18"",
          ""lastKeepAlive"": ""2020-05-25 00:14:54"",
          ""configSum"": ""d149ab60df71e6fb53b2d38db75bc733"",
          ""manager"": ""vsec-logstash"",
          ""registerIP"": ""any"",
          ""group"": [
            ""Daklak"",
            ""Test"",
            ""VSEC"",
            ""Daklak_Webs""
          ],
          ""ip"": ""192.168.2.18"",
          ""mergedSum"": ""535237cb82b543ec180a59bde6425db5"",
          ""dateAdd"": ""2020-03-31 02:15:08"",
          ""id"": ""061"",
          ""version"": ""Wazuh v3.11.4"",
          ""status"": ""Active"",
          ""timestamp"": ""2020-05-25T00:15:02.445Z"",
          ""host"": ""vsec-logstash"",
          ""cluster"": {
            ""name"": ""wazuh""
          }
        },
        ""fields"": {
          ""timestamp"": [
            ""2020-05-25T00:15:02.445Z""
          ]
        },
        ""highlight"": {
          ""group"": [
            ""@kibana-highlighted-field@Daklak@/kibana-highlighted-field@"",
            ""@kibana-highlighted-field@Daklak_Webs@/kibana-highlighted-field@""
          ]
        },
        ""sort"": [
          -9223372036854776000
        ]
      },
      {
        ""_index"": ""wazuh-monitoring-3.x-2020.05.25"",
        ""_type"": ""_doc"",
        ""_id"": ""kzs8SXIBENNuuStZgvL-"",
        ""_version"": 1,
        ""_score"": null,
        ""_source"": {
          ""os"": {
            ""arch"": ""x86_64"",
            ""major"": ""6"",
            ""minor"": ""8"",
            ""name"": ""CentOS Linux"",
            ""platform"": ""centos"",
            ""uname"": ""Linux |webserver18 |2.6.32-642.el6.x86_64 |#1 SMP Tue May 10 17:27:01 UTC 2016 |x86_64"",
            ""version"": ""6.8""
          },
          ""id"": ""061"",
          ""configSum"": ""d149ab60df71e6fb53b2d38db75bc733"",
          ""mergedSum"": ""535237cb82b543ec180a59bde6425db5"",
          ""status"": ""Active"",
          ""ip"": ""192.168.2.18"",
          ""registerIP"": ""any"",
          ""version"": ""Wazuh v3.11.4"",
          ""group"": [
            ""Daklak"",
            ""Test"",
            ""VSEC"",
            ""Daklak_Webs""
          ],
          ""dateAdd"": ""2020-03-31 02:15:08"",
          ""lastKeepAlive"": ""2020-05-25 00:29:54"",
          ""node_name"": ""node02"",
          ""manager"": ""vsec-logstash"",
          ""name"": ""webserver18"",
          ""timestamp"": ""2020-05-25T00:30:01.983Z"",
          ""host"": ""vsec-logstash"",
          ""cluster"": {
            ""name"": ""wazuh""
          }
        },
        ""fields"": {
          ""timestamp"": [
            ""2020-05-25T00:30:01.983Z""
          ]
        },
        ""highlight"": {
          ""group"": [
            ""@kibana-highlighted-field@Daklak@/kibana-highlighted-field@"",
            ""@kibana-highlighted-field@Daklak_Webs@/kibana-highlighted-field@""
          ]
        },
        ""sort"": [
          -9223372036854776000
        ]
      },
      {
        ""_index"": ""wazuh-monitoring-3.x-2020.05.25"",
        ""_type"": ""_doc"",
        ""_id"": ""QzxKSXIBENNuuStZPaYs"",
        ""_version"": 1,
        ""_score"": null,
        ""_source"": {
          ""os"": {
            ""arch"": ""x86_64"",
            ""major"": ""6"",
            ""minor"": ""10"",
            ""name"": ""CentOS Linux"",
            ""platform"": ""centos"",
            ""uname"": ""Linux |werserver |2.6.32-754.11.1.el6.x86_64 |#1 SMP Tue Feb 26 15:38:56 UTC 2019 |x86_64"",
            ""version"": ""6.10""
          },
          ""node_name"": ""node02"",
          ""id"": ""060"",
          ""status"": ""Active"",
          ""ip"": ""192.168.2.14"",
          ""dateAdd"": ""2020-03-27 07:47:46"",
          ""lastKeepAlive"": ""2020-05-25 00:44:50"",
          ""mergedSum"": ""f69c8a56d2cd8b401643a52cf423ad1c"",
          ""version"": ""Wazuh v3.11.4"",
          ""group"": [
            ""Daklak"",
            ""Test"",
            ""VSEC"",
            ""Daklak_Webs"",
            ""Daklak_Apps""
          ],
          ""registerIP"": ""any"",
          ""configSum"": ""4b54b212c2dd96f8b25aebfea2a104cd"",
          ""manager"": ""vsec-logstash"",
          ""name"": ""werserver"",
          ""timestamp"": ""2020-05-25T00:45:01.616Z"",
          ""host"": ""vsec-logstash"",
          ""cluster"": {
            ""name"": ""wazuh""
          }
        },
        ""fields"": {
          ""timestamp"": [
            ""2020-05-25T00:45:01.616Z""
          ]
        },
        ""highlight"": {
          ""group"": [
            ""@kibana-highlighted-field@Daklak@/kibana-highlighted-field@"",
            ""@kibana-highlighted-field@Daklak_Webs@/kibana-highlighted-field@""
          ]
        },
        ""sort"": [
          -9223372036854776000
        ]
      },
      {
        ""_index"": ""wazuh-monitoring-3.x-2020.05.25"",
        ""_type"": ""_doc"",
        ""_id"": ""RDxKSXIBENNuuStZPaYs"",
        ""_version"": 1,
        ""_score"": null,
        ""_source"": {
          ""os"": {
            ""arch"": ""x86_64"",
            ""major"": ""6"",
            ""minor"": ""8"",
            ""name"": ""CentOS Linux"",
            ""platform"": ""centos"",
            ""uname"": ""Linux |webserver18 |2.6.32-642.el6.x86_64 |#1 SMP Tue May 10 17:27:01 UTC 2016 |x86_64"",
            ""version"": ""6.8""
          },
          ""node_name"": ""node02"",
          ""id"": ""061"",
          ""status"": ""Active"",
          ""ip"": ""192.168.2.18"",
          ""dateAdd"": ""2020-03-31 02:15:08"",
          ""lastKeepAlive"": ""2020-05-25 00:44:47"",
          ""mergedSum"": ""535237cb82b543ec180a59bde6425db5"",
          ""version"": ""Wazuh v3.11.4"",
          ""group"": [
            ""Daklak"",
            ""Test"",
            ""VSEC"",
            ""Daklak_Webs""
          ],
          ""registerIP"": ""any"",
          ""configSum"": ""d149ab60df71e6fb53b2d38db75bc733"",
          ""manager"": ""vsec-logstash"",
          ""name"": ""webserver18"",
          ""timestamp"": ""2020-05-25T00:45:01.616Z"",
          ""host"": ""vsec-logstash"",
          ""cluster"": {
            ""name"": ""wazuh""
          }
        },
        ""fields"": {
          ""timestamp"": [
            ""2020-05-25T00:45:01.616Z""
          ]
        },
        ""highlight"": {
          ""group"": [
            ""@kibana-highlighted-field@Daklak@/kibana-highlighted-field@"",
            ""@kibana-highlighted-field@Daklak_Webs@/kibana-highlighted-field@""
          ]
        },
        ""sort"": [
          -9223372036854776000
        ]
      },
      {
        ""_index"": ""wazuh-monitoring-3.x-2020.05.25"",
        ""_type"": ""_doc"",
        ""_id"": ""Cj1XSXIBENNuuStZ-lmW"",
        ""_version"": 1,
        ""_score"": null,
        ""_source"": {
          ""os"": {
            ""arch"": ""x86_64"",
            ""major"": ""6"",
            ""minor"": ""10"",
            ""name"": ""CentOS Linux"",
            ""platform"": ""centos"",
            ""uname"": ""Linux |werserver |2.6.32-754.11.1.el6.x86_64 |#1 SMP Tue Feb 26 15:38:56 UTC 2019 |x86_64"",
            ""version"": ""6.10""
          },
          ""version"": ""Wazuh v3.11.4"",
          ""registerIP"": ""any"",
          ""status"": ""Active"",
          ""id"": ""060"",
          ""dateAdd"": ""2020-03-27 07:47:46"",
          ""manager"": ""vsec-logstash"",
          ""group"": [
            ""Daklak"",
            ""Test"",
            ""VSEC"",
            ""Daklak_Webs"",
            ""Daklak_Apps""
          ],
          ""ip"": ""192.168.2.14"",
          ""configSum"": ""4b54b212c2dd96f8b25aebfea2a104cd"",
          ""node_name"": ""node02"",
          ""mergedSum"": ""f69c8a56d2cd8b401643a52cf423ad1c"",
          ""lastKeepAlive"": ""2020-05-25 00:59:50"",
          ""name"": ""werserver"",
          ""timestamp"": ""2020-05-25T01:00:02.070Z"",
          ""host"": ""vsec-logstash"",
          ""cluster"": {
            ""name"": ""wazuh""
          }
        },
        ""fields"": {
          ""timestamp"": [
            ""2020-05-25T01:00:02.070Z""
          ]
        },
        ""highlight"": {
          ""group"": [
            ""@kibana-highlighted-field@Daklak@/kibana-highlighted-field@"",
            ""@kibana-highlighted-field@Daklak_Webs@/kibana-highlighted-field@""
          ]
        },
        ""sort"": [
          -9223372036854776000
        ]
      },
      {
        ""_index"": ""wazuh-monitoring-3.x-2020.05.25"",
        ""_type"": ""_doc"",
        ""_id"": ""Cz1XSXIBENNuuStZ-lmW"",
        ""_version"": 1,
        ""_score"": null,
        ""_source"": {
          ""os"": {
            ""arch"": ""x86_64"",
            ""major"": ""6"",
            ""minor"": ""8"",
            ""name"": ""CentOS Linux"",
            ""platform"": ""centos"",
            ""uname"": ""Linux |webserver18 |2.6.32-642.el6.x86_64 |#1 SMP Tue May 10 17:27:01 UTC 2016 |x86_64"",
            ""version"": ""6.8""
          },
          ""version"": ""Wazuh v3.11.4"",
          ""registerIP"": ""any"",
          ""status"": ""Active"",
          ""id"": ""061"",
          ""dateAdd"": ""2020-03-31 02:15:08"",
          ""manager"": ""vsec-logstash"",
          ""group"": [
            ""Daklak"",
            ""Test"",
            ""VSEC"",
            ""Daklak_Webs""
          ],
          ""ip"": ""192.168.2.18"",
          ""configSum"": ""d149ab60df71e6fb53b2d38db75bc733"",
          ""node_name"": ""node02"",
          ""mergedSum"": ""535237cb82b543ec180a59bde6425db5"",
          ""lastKeepAlive"": ""2020-05-25 00:59:56"",
          ""name"": ""webserver18"",
          ""timestamp"": ""2020-05-25T01:00:02.070Z"",
          ""host"": ""vsec-logstash"",
          ""cluster"": {
            ""name"": ""wazuh""
          }
        },
        ""fields"": {
          ""timestamp"": [
            ""2020-05-25T01:00:02.070Z""
          ]
        },
        ""highlight"": {
          ""group"": [
            ""@kibana-highlighted-field@Daklak@/kibana-highlighted-field@"",
            ""@kibana-highlighted-field@Daklak_Webs@/kibana-highlighted-field@""
          ]
        },
        ""sort"": [
          -9223372036854776000
        ]
      },
      {
        ""_index"": ""wazuh-monitoring-3.x-2020.05.25"",
        ""_type"": ""_doc"",
        ""_id"": ""XD5lSXIBENNuuStZuBqW"",
        ""_version"": 1,
        ""_score"": null,
        ""_source"": {
          ""os"": {
            ""arch"": ""x86_64"",
            ""major"": ""6"",
            ""minor"": ""8"",
            ""name"": ""CentOS Linux"",
            ""platform"": ""centos"",
            ""uname"": ""Linux |webserver18 |2.6.32-642.el6.x86_64 |#1 SMP Tue May 10 17:27:01 UTC 2016 |x86_64"",
            ""version"": ""6.8""
          },
          ""id"": ""061"",
          ""lastKeepAlive"": ""2020-05-25 01:14:56"",
          ""dateAdd"": ""2020-03-31 02:15:08"",
          ""status"": ""Active"",
          ""manager"": ""vsec-logstash"",
          ""name"": ""webserver18"",
          ""configSum"": ""d149ab60df71e6fb53b2d38db75bc733"",
          ""mergedSum"": ""535237cb82b543ec180a59bde6425db5"",
          ""group"": [
            ""Daklak"",
            ""Test"",
            ""VSEC"",
            ""Daklak_Webs""
          ],
          ""registerIP"": ""any"",
          ""ip"": ""192.168.2.18"",
          ""version"": ""Wazuh v3.11.4"",
          ""node_name"": ""node02"",
          ""timestamp"": ""2020-05-25T01:15:02.671Z"",
          ""host"": ""vsec-logstash"",
          ""cluster"": {
            ""name"": ""wazuh""
          }
        },
        ""fields"": {
          ""timestamp"": [
            ""2020-05-25T01:15:02.671Z""
          ]
        },
        ""highlight"": {
          ""group"": [
            ""@kibana-highlighted-field@Daklak@/kibana-highlighted-field@"",
            ""@kibana-highlighted-field@Daklak_Webs@/kibana-highlighted-field@""
          ]
        },
        ""sort"": [
          -9223372036854776000
        ]
      },
      {
        ""_index"": ""wazuh-monitoring-3.x-2020.05.25"",
        ""_type"": ""_doc"",
        ""_id"": ""fUCOSXIBENNuuStZ6lXm"",
        ""_version"": 1,
        ""_score"": null,
        ""_source"": {
          ""os"": {
            ""arch"": ""x86_64"",
            ""major"": ""6"",
            ""minor"": ""10"",
            ""name"": ""CentOS Linux"",
            ""platform"": ""centos"",
            ""uname"": ""Linux |werserver |2.6.32-754.11.1.el6.x86_64 |#1 SMP Tue Feb 26 15:38:56 UTC 2019 |x86_64"",
            ""version"": ""6.10""
          },
          ""name"": ""werserver"",
          ""group"": [
            ""Daklak"",
            ""Test"",
            ""VSEC"",
            ""Daklak_Webs"",
            ""Daklak_Apps""
          ],
          ""dateAdd"": ""2020-03-27 07:47:46"",
          ""id"": ""060"",
          ""lastKeepAlive"": ""2020-05-25 01:59:52"",
          ""status"": ""Active"",
          ""mergedSum"": ""f69c8a56d2cd8b401643a52cf423ad1c"",
          ""configSum"": ""4b54b212c2dd96f8b25aebfea2a104cd"",
          ""registerIP"": ""any"",
          ""ip"": ""192.168.2.14"",
          ""version"": ""Wazuh v3.11.4"",
          ""manager"": ""vsec-logstash"",
          ""node_name"": ""node02"",
          ""timestamp"": ""2020-05-25T02:00:02.536Z"",
          ""host"": ""vsec-logstash"",
          ""cluster"": {
            ""name"": ""wazuh""
          }
        },
        ""fields"": {
          ""timestamp"": [
            ""2020-05-25T02:00:02.536Z""
          ]
        },
        ""highlight"": {
          ""group"": [
            ""@kibana-highlighted-field@Daklak@/kibana-highlighted-field@"",
            ""@kibana-highlighted-field@Daklak_Webs@/kibana-highlighted-field@""
          ]
        },
        ""sort"": [
          -9223372036854776000
        ]
      },
      {
        ""_index"": ""wazuh-monitoring-3.x-2020.05.25"",
        ""_type"": ""_doc"",
        ""_id"": ""fkCOSXIBENNuuStZ6lXm"",
        ""_version"": 1,
        ""_score"": null,
        ""_source"": {
          ""os"": {
            ""arch"": ""x86_64"",
            ""major"": ""6"",
            ""minor"": ""8"",
            ""name"": ""CentOS Linux"",
            ""platform"": ""centos"",
            ""uname"": ""Linux |webserver18 |2.6.32-642.el6.x86_64 |#1 SMP Tue May 10 17:27:01 UTC 2016 |x86_64"",
            ""version"": ""6.8""
          },
          ""name"": ""webserver18"",
          ""group"": [
            ""Daklak"",
            ""Test"",
            ""VSEC"",
            ""Daklak_Webs""
          ],
          ""dateAdd"": ""2020-03-31 02:15:08"",
          ""id"": ""061"",
          ""lastKeepAlive"": ""2020-05-25 01:59:54"",
          ""status"": ""Active"",
          ""mergedSum"": ""535237cb82b543ec180a59bde6425db5"",
          ""configSum"": ""d149ab60df71e6fb53b2d38db75bc733"",
          ""registerIP"": ""any"",
          ""ip"": ""192.168.2.18"",
          ""version"": ""Wazuh v3.11.4"",
          ""manager"": ""vsec-logstash"",
          ""node_name"": ""node02"",
          ""timestamp"": ""2020-05-25T02:00:02.536Z"",
          ""host"": ""vsec-logstash"",
          ""cluster"": {
            ""name"": ""wazuh""
          }
        },
        ""fields"": {
          ""timestamp"": [
            ""2020-05-25T02:00:02.536Z""
          ]
        },
        ""highlight"": {
          ""group"": [
            ""@kibana-highlighted-field@Daklak@/kibana-highlighted-field@"",
            ""@kibana-highlighted-field@Daklak_Webs@/kibana-highlighted-field@""
          ]
        },
        ""sort"": [
          -9223372036854776000
        ]
      },
      {
        ""_index"": ""wazuh-monitoring-3.x-2020.05.25"",
        ""_type"": ""_doc"",
        ""_id"": ""JEGcSXIBENNuuStZpR0-"",
        ""_version"": 1,
        ""_score"": null,
        ""_source"": {
          ""os"": {
            ""arch"": ""x86_64"",
            ""major"": ""6"",
            ""minor"": ""8"",
            ""name"": ""CentOS Linux"",
            ""platform"": ""centos"",
            ""uname"": ""Linux |webserver18 |2.6.32-642.el6.x86_64 |#1 SMP Tue May 10 17:27:01 UTC 2016 |x86_64"",
            ""version"": ""6.8""
          },
          ""registerIP"": ""any"",
          ""status"": ""Active"",
          ""name"": ""webserver18"",
          ""version"": ""Wazuh v3.11.4"",
          ""id"": ""061"",
          ""manager"": ""vsec-logstash"",
          ""dateAdd"": ""2020-03-31 02:15:08"",
          ""mergedSum"": ""535237cb82b543ec180a59bde6425db5"",
          ""node_name"": ""node02"",
          ""lastKeepAlive"": ""2020-05-25 02:14:53"",
          ""group"": [
            ""Daklak"",
            ""Test"",
            ""VSEC"",
            ""Daklak_Webs""
          ],
          ""ip"": ""192.168.2.18"",
          ""configSum"": ""d149ab60df71e6fb53b2d38db75bc733"",
          ""timestamp"": ""2020-05-25T02:15:02.205Z"",
          ""host"": ""vsec-logstash"",
          ""cluster"": {
            ""name"": ""wazuh""
          }
        },
        ""fields"": {
          ""timestamp"": [
            ""2020-05-25T02:15:02.205Z""
          ]
        },
        ""highlight"": {
          ""group"": [
            ""@kibana-highlighted-field@Daklak@/kibana-highlighted-field@"",
            ""@kibana-highlighted-field@Daklak_Webs@/kibana-highlighted-field@""
          ]
        },
        ""sort"": [
          -9223372036854776000
        ]
      }
    ]
  },
  ""aggregations"": {
    ""2"": {
      ""buckets"": [
        {
          ""key_as_string"": ""2020-05-24T09:30:00.000+07:00"",
          ""key"": 1590287400000,
          ""doc_count"": 2
        },
        {
          ""key_as_string"": ""2020-05-24T09:45:00.000+07:00"",
          ""key"": 1590288300000,
          ""doc_count"": 2
        },
        {
          ""key_as_string"": ""2020-05-24T10:00:00.000+07:00"",
          ""key"": 1590289200000,
          ""doc_count"": 2
        },
        {
          ""key_as_string"": ""2020-05-24T10:15:00.000+07:00"",
          ""key"": 1590290100000,
          ""doc_count"": 2
        },
        {
          ""key_as_string"": ""2020-05-24T10:30:00.000+07:00"",
          ""key"": 1590291000000,
          ""doc_count"": 2
        },
        {
          ""key_as_string"": ""2020-05-24T10:45:00.000+07:00"",
          ""key"": 1590291900000,
          ""doc_count"": 2
        },
        {
          ""key_as_string"": ""2020-05-24T11:00:00.000+07:00"",
          ""key"": 1590292800000,
          ""doc_count"": 2
        },
        {
          ""key_as_string"": ""2020-05-24T11:15:00.000+07:00"",
          ""key"": 1590293700000,
          ""doc_count"": 2
        },
        {
          ""key_as_string"": ""2020-05-24T11:30:00.000+07:00"",
          ""key"": 1590294600000,
          ""doc_count"": 2
        },
        {
          ""key_as_string"": ""2020-05-24T11:45:00.000+07:00"",
          ""key"": 1590295500000,
          ""doc_count"": 2
        },
        {
          ""key_as_string"": ""2020-05-24T12:00:00.000+07:00"",
          ""key"": 1590296400000,
          ""doc_count"": 2
        },
        {
          ""key_as_string"": ""2020-05-24T12:15:00.000+07:00"",
          ""key"": 1590297300000,
          ""doc_count"": 2
        },
        {
          ""key_as_string"": ""2020-05-24T12:30:00.000+07:00"",
          ""key"": 1590298200000,
          ""doc_count"": 2
        },
        {
          ""key_as_string"": ""2020-05-24T12:45:00.000+07:00"",
          ""key"": 1590299100000,
          ""doc_count"": 2
        },
        {
          ""key_as_string"": ""2020-05-24T13:00:00.000+07:00"",
          ""key"": 1590300000000,
          ""doc_count"": 2
        },
        {
          ""key_as_string"": ""2020-05-24T13:15:00.000+07:00"",
          ""key"": 1590300900000,
          ""doc_count"": 2
        },
        {
          ""key_as_string"": ""2020-05-24T13:30:00.000+07:00"",
          ""key"": 1590301800000,
          ""doc_count"": 2
        },
        {
          ""key_as_string"": ""2020-05-24T13:45:00.000+07:00"",
          ""key"": 1590302700000,
          ""doc_count"": 2
        },
        {
          ""key_as_string"": ""2020-05-24T14:00:00.000+07:00"",
          ""key"": 1590303600000,
          ""doc_count"": 2
        },
        {
          ""key_as_string"": ""2020-05-24T14:15:00.000+07:00"",
          ""key"": 1590304500000,
          ""doc_count"": 2
        },
        {
          ""key_as_string"": ""2020-05-24T14:30:00.000+07:00"",
          ""key"": 1590305400000,
          ""doc_count"": 2
        },
        {
          ""key_as_string"": ""2020-05-24T14:45:00.000+07:00"",
          ""key"": 1590306300000,
          ""doc_count"": 2
        },
        {
          ""key_as_string"": ""2020-05-24T15:00:00.000+07:00"",
          ""key"": 1590307200000,
          ""doc_count"": 2
        },
        {
          ""key_as_string"": ""2020-05-24T15:15:00.000+07:00"",
          ""key"": 1590308100000,
          ""doc_count"": 2
        },
        {
          ""key_as_string"": ""2020-05-24T15:30:00.000+07:00"",
          ""key"": 1590309000000,
          ""doc_count"": 2
        },
        {
          ""key_as_string"": ""2020-05-24T15:45:00.000+07:00"",
          ""key"": 1590309900000,
          ""doc_count"": 2
        },
        {
          ""key_as_string"": ""2020-05-24T16:00:00.000+07:00"",
          ""key"": 1590310800000,
          ""doc_count"": 2
        },
        {
          ""key_as_string"": ""2020-05-24T16:15:00.000+07:00"",
          ""key"": 1590311700000,
          ""doc_count"": 2
        },
        {
          ""key_as_string"": ""2020-05-24T16:30:00.000+07:00"",
          ""key"": 1590312600000,
          ""doc_count"": 2
        },
        {
          ""key_as_string"": ""2020-05-24T16:45:00.000+07:00"",
          ""key"": 1590313500000,
          ""doc_count"": 2
        },
        {
          ""key_as_string"": ""2020-05-24T17:00:00.000+07:00"",
          ""key"": 1590314400000,
          ""doc_count"": 2
        },
        {
          ""key_as_string"": ""2020-05-24T17:15:00.000+07:00"",
          ""key"": 1590315300000,
          ""doc_count"": 2
        },
        {
          ""key_as_string"": ""2020-05-24T17:30:00.000+07:00"",
          ""key"": 1590316200000,
          ""doc_count"": 2
        },
        {
          ""key_as_string"": ""2020-05-24T17:45:00.000+07:00"",
          ""key"": 1590317100000,
          ""doc_count"": 2
        },
        {
          ""key_as_string"": ""2020-05-24T18:00:00.000+07:00"",
          ""key"": 1590318000000,
          ""doc_count"": 2
        },
        {
          ""key_as_string"": ""2020-05-24T18:15:00.000+07:00"",
          ""key"": 1590318900000,
          ""doc_count"": 2
        },
        {
          ""key_as_string"": ""2020-05-24T18:30:00.000+07:00"",
          ""key"": 1590319800000,
          ""doc_count"": 2
        },
        {
          ""key_as_string"": ""2020-05-24T18:45:00.000+07:00"",
          ""key"": 1590320700000,
          ""doc_count"": 2
        },
        {
          ""key_as_string"": ""2020-05-24T19:00:00.000+07:00"",
          ""key"": 1590321600000,
          ""doc_count"": 2
        },
        {
          ""key_as_string"": ""2020-05-24T19:15:00.000+07:00"",
          ""key"": 1590322500000,
          ""doc_count"": 2
        },
        {
          ""key_as_string"": ""2020-05-24T19:30:00.000+07:00"",
          ""key"": 1590323400000,
          ""doc_count"": 2
        },
        {
          ""key_as_string"": ""2020-05-24T19:45:00.000+07:00"",
          ""key"": 1590324300000,
          ""doc_count"": 2
        },
        {
          ""key_as_string"": ""2020-05-24T20:00:00.000+07:00"",
          ""key"": 1590325200000,
          ""doc_count"": 2
        },
        {
          ""key_as_string"": ""2020-05-24T20:15:00.000+07:00"",
          ""key"": 1590326100000,
          ""doc_count"": 2
        },
        {
          ""key_as_string"": ""2020-05-24T20:30:00.000+07:00"",
          ""key"": 1590327000000,
          ""doc_count"": 2
        },
        {
          ""key_as_string"": ""2020-05-24T20:45:00.000+07:00"",
          ""key"": 1590327900000,
          ""doc_count"": 2
        },
        {
          ""key_as_string"": ""2020-05-24T21:00:00.000+07:00"",
          ""key"": 1590328800000,
          ""doc_count"": 2
        },
        {
          ""key_as_string"": ""2020-05-24T21:15:00.000+07:00"",
          ""key"": 1590329700000,
          ""doc_count"": 2
        },
        {
          ""key_as_string"": ""2020-05-24T21:30:00.000+07:00"",
          ""key"": 1590330600000,
          ""doc_count"": 2
        },
        {
          ""key_as_string"": ""2020-05-24T21:45:00.000+07:00"",
          ""key"": 1590331500000,
          ""doc_count"": 2
        },
        {
          ""key_as_string"": ""2020-05-24T22:00:00.000+07:00"",
          ""key"": 1590332400000,
          ""doc_count"": 2
        },
        {
          ""key_as_string"": ""2020-05-24T22:15:00.000+07:00"",
          ""key"": 1590333300000,
          ""doc_count"": 2
        },
        {
          ""key_as_string"": ""2020-05-24T22:30:00.000+07:00"",
          ""key"": 1590334200000,
          ""doc_count"": 2
        },
        {
          ""key_as_string"": ""2020-05-24T22:45:00.000+07:00"",
          ""key"": 1590335100000,
          ""doc_count"": 2
        },
        {
          ""key_as_string"": ""2020-05-24T23:00:00.000+07:00"",
          ""key"": 1590336000000,
          ""doc_count"": 2
        },
        {
          ""key_as_string"": ""2020-05-24T23:15:00.000+07:00"",
          ""key"": 1590336900000,
          ""doc_count"": 2
        },
        {
          ""key_as_string"": ""2020-05-24T23:30:00.000+07:00"",
          ""key"": 1590337800000,
          ""doc_count"": 2
        },
        {
          ""key_as_string"": ""2020-05-24T23:45:00.000+07:00"",
          ""key"": 1590338700000,
          ""doc_count"": 2
        },
        {
          ""key_as_string"": ""2020-05-25T00:00:00.000+07:00"",
          ""key"": 1590339600000,
          ""doc_count"": 2
        },
        {
          ""key_as_string"": ""2020-05-25T00:15:00.000+07:00"",
          ""key"": 1590340500000,
          ""doc_count"": 2
        },
        {
          ""key_as_string"": ""2020-05-25T00:30:00.000+07:00"",
          ""key"": 1590341400000,
          ""doc_count"": 2
        },
        {
          ""key_as_string"": ""2020-05-25T00:45:00.000+07:00"",
          ""key"": 1590342300000,
          ""doc_count"": 2
        },
        {
          ""key_as_string"": ""2020-05-25T01:00:00.000+07:00"",
          ""key"": 1590343200000,
          ""doc_count"": 2
        },
        {
          ""key_as_string"": ""2020-05-25T01:15:00.000+07:00"",
          ""key"": 1590344100000,
          ""doc_count"": 2
        },
        {
          ""key_as_string"": ""2020-05-25T01:30:00.000+07:00"",
          ""key"": 1590345000000,
          ""doc_count"": 2
        },
        {
          ""key_as_string"": ""2020-05-25T01:45:00.000+07:00"",
          ""key"": 1590345900000,
          ""doc_count"": 2
        },
        {
          ""key_as_string"": ""2020-05-25T02:00:00.000+07:00"",
          ""key"": 1590346800000,
          ""doc_count"": 2
        },
        {
          ""key_as_string"": ""2020-05-25T02:15:00.000+07:00"",
          ""key"": 1590347700000,
          ""doc_count"": 2
        },
        {
          ""key_as_string"": ""2020-05-25T02:30:00.000+07:00"",
          ""key"": 1590348600000,
          ""doc_count"": 2
        },
        {
          ""key_as_string"": ""2020-05-25T02:45:00.000+07:00"",
          ""key"": 1590349500000,
          ""doc_count"": 2
        },
        {
          ""key_as_string"": ""2020-05-25T03:00:00.000+07:00"",
          ""key"": 1590350400000,
          ""doc_count"": 2
        },
        {
          ""key_as_string"": ""2020-05-25T03:15:00.000+07:00"",
          ""key"": 1590351300000,
          ""doc_count"": 2
        },
        {
          ""key_as_string"": ""2020-05-25T03:30:00.000+07:00"",
          ""key"": 1590352200000,
          ""doc_count"": 2
        },
        {
          ""key_as_string"": ""2020-05-25T03:45:00.000+07:00"",
          ""key"": 1590353100000,
          ""doc_count"": 2
        },
        {
          ""key_as_string"": ""2020-05-25T04:00:00.000+07:00"",
          ""key"": 1590354000000,
          ""doc_count"": 2
        },
        {
          ""key_as_string"": ""2020-05-25T04:15:00.000+07:00"",
          ""key"": 1590354900000,
          ""doc_count"": 2
        },
        {
          ""key_as_string"": ""2020-05-25T04:30:00.000+07:00"",
          ""key"": 1590355800000,
          ""doc_count"": 2
        },
        {
          ""key_as_string"": ""2020-05-25T04:45:00.000+07:00"",
          ""key"": 1590356700000,
          ""doc_count"": 2
        },
        {
          ""key_as_string"": ""2020-05-25T05:00:00.000+07:00"",
          ""key"": 1590357600000,
          ""doc_count"": 2
        },
        {
          ""key_as_string"": ""2020-05-25T05:15:00.000+07:00"",
          ""key"": 1590358500000,
          ""doc_count"": 2
        },
        {
          ""key_as_string"": ""2020-05-25T05:30:00.000+07:00"",
          ""key"": 1590359400000,
          ""doc_count"": 2
        },
        {
          ""key_as_string"": ""2020-05-25T05:45:00.000+07:00"",
          ""key"": 1590360300000,
          ""doc_count"": 2
        },
        {
          ""key_as_string"": ""2020-05-25T06:00:00.000+07:00"",
          ""key"": 1590361200000,
          ""doc_count"": 2
        },
        {
          ""key_as_string"": ""2020-05-25T06:15:00.000+07:00"",
          ""key"": 1590362100000,
          ""doc_count"": 2
        },
        {
          ""key_as_string"": ""2020-05-25T06:30:00.000+07:00"",
          ""key"": 1590363000000,
          ""doc_count"": 2
        },
        {
          ""key_as_string"": ""2020-05-25T06:45:00.000+07:00"",
          ""key"": 1590363900000,
          ""doc_count"": 2
        },
        {
          ""key_as_string"": ""2020-05-25T07:00:00.000+07:00"",
          ""key"": 1590364800000,
          ""doc_count"": 2
        },
        {
          ""key_as_string"": ""2020-05-25T07:15:00.000+07:00"",
          ""key"": 1590365700000,
          ""doc_count"": 2
        },
        {
          ""key_as_string"": ""2020-05-25T07:30:00.000+07:00"",
          ""key"": 1590366600000,
          ""doc_count"": 2
        },
        {
          ""key_as_string"": ""2020-05-25T07:45:00.000+07:00"",
          ""key"": 1590367500000,
          ""doc_count"": 2
        },
        {
          ""key_as_string"": ""2020-05-25T08:00:00.000+07:00"",
          ""key"": 1590368400000,
          ""doc_count"": 2
        },
        {
          ""key_as_string"": ""2020-05-25T08:15:00.000+07:00"",
          ""key"": 1590369300000,
          ""doc_count"": 2
        },
        {
          ""key_as_string"": ""2020-05-25T08:30:00.000+07:00"",
          ""key"": 1590370200000,
          ""doc_count"": 2
        },
        {
          ""key_as_string"": ""2020-05-25T08:45:00.000+07:00"",
          ""key"": 1590371100000,
          ""doc_count"": 2
        },
        {
          ""key_as_string"": ""2020-05-25T09:00:00.000+07:00"",
          ""key"": 1590372000000,
          ""doc_count"": 2
        },
        {
          ""key_as_string"": ""2020-05-25T09:15:00.000+07:00"",
          ""key"": 1590372900000,
          ""doc_count"": 2
        }
      ]
    }
  }
}";
            string zabbixJson = @"{
  ""jsonrpc"": ""2.0"",
  ""result"": [
    {
      ""hostid"": ""10427"",
      ""host"": ""WIN-N5PCAN1SF0L"",
      ""interfaces"": [
        {
          ""interfaceid"": ""101"",
          ""ip"": ""13.229.151.233""
        },
        {
          ""interfaceid"": ""102"",
          ""ip"": ""13.250.112.168""
        }
      ]
    },
    {
      ""hostid"": ""10415"",
      ""host"": ""misc-jira-172.31.2.75-18.140.31.106"",
      ""interfaces"": [
        {
          ""interfaceid"": ""92"",
          ""ip"": ""172.31.2.75""
        }
      ]
    },
    {
      ""hostid"": ""10422"",
      ""host"": ""vd-waz-master-52.76.198.210-10.60.1.60"",
      ""interfaces"": [
        {
          ""interfaceid"": ""96"",
          ""ip"": ""10.60.1.60""
        }
      ]
    },
    {
      ""hostid"": ""10428"",
      ""host"": ""WIN-N5PCAN1SF0L-01"",
      ""interfaces"": [
        {
          ""interfaceid"": ""103"",
          ""ip"": ""13.250.112.168""
        }
      ]
    },
    {
      ""hostid"": ""10323"",
      ""host"": ""gcafe-db-2856"",
      ""interfaces"": [
        {
          ""interfaceid"": ""7"",
          ""ip"": ""103.239.122.21""
        }
      ]
    },
    {
      ""hostid"": ""10393"",
      ""host"": ""ss-dev-mon-172.31.11.21-18.141.10.171"",
      ""interfaces"": [
        {
          ""interfaceid"": ""77"",
          ""ip"": ""172.31.11.21""
        }
      ]
    },
    {
      ""hostid"": ""10324"",
      ""host"": ""werserver"",
      ""interfaces"": [
        {
          ""interfaceid"": ""8"",
          ""ip"": ""171.251.51.130""
        }
      ]
    },
    {
      ""hostid"": ""10319"",
      ""host"": ""es215-192.168.20.215"",
      ""interfaces"": [
        {
          ""interfaceid"": ""3"",
          ""ip"": ""125.212.198.212""
        }
      ]
    },
    {
      ""hostid"": ""10320"",
      ""host"": ""g-es-zabbix216"",
      ""interfaces"": [
        {
          ""interfaceid"": ""4"",
          ""ip"": ""125.212.198.212""
        }
      ]
    },
    {
      ""hostid"": ""10437"",
      ""host"": ""ss-app-3.1.81.23-172.31.15.115"",
      ""interfaces"": [
        {
          ""interfaceid"": ""112"",
          ""ip"": ""172.31.15.115""
        }
      ]
    },
    {
      ""hostid"": ""10429"",
      ""host"": ""vipn-portal-171.244.36.88"",
      ""interfaces"": [
        {
          ""interfaceid"": ""104"",
          ""ip"": ""171.244.36.88""
        }
      ]
    },
    {
      ""hostid"": ""10423"",
      ""host"": ""vd-wazuh-worker1-13.250.160.203-10.60.1.188"",
      ""interfaces"": [
        {
          ""interfaceid"": ""97"",
          ""ip"": ""10.60.1.188""
        }
      ]
    },
    {
      ""hostid"": ""10084"",
      ""host"": ""Zabbix server"",
      ""interfaces"": [
        {
          ""interfaceid"": ""1"",
          ""ip"": ""127.0.0.1""
        }
      ]
    },
    {
      ""hostid"": ""10322"",
      ""host"": ""WIN-59DMA2U0F87"",
      ""interfaces"": [
        {
          ""interfaceid"": ""6"",
          ""ip"": ""103.239.122.154""
        }
      ]
    },
    {
      ""hostid"": ""10318"",
      ""host"": ""ss-vpn-20.0.30.213-3.1.133.41"",
      ""interfaces"": [
        {
          ""interfaceid"": ""2"",
          ""ip"": ""3.1.133.41""
        }
      ]
    },
    {
      ""hostid"": ""10430"",
      ""host"": ""vipn-worker1-171.244.36.105"",
      ""interfaces"": [
        {
          ""interfaceid"": ""105"",
          ""ip"": ""171.244.36.105""
        }
      ]
    },
    {
      ""hostid"": ""10418"",
      ""host"": ""bb-blockchain1-172.31.10.191-18.139.219.55"",
      ""interfaces"": [
        {
          ""interfaceid"": ""93"",
          ""ip"": ""172.31.10.191""
        }
      ]
    },
    {
      ""hostid"": ""10444"",
      ""host"": ""24h-ids-man1"",
      ""interfaces"": [
        {
          ""interfaceid"": ""116"",
          ""ip"": ""125.212.247.17""
        }
      ]
    },
    {
      ""hostid"": ""10432"",
      ""host"": ""Crtx-2-3.9.97.161"",
      ""interfaces"": [
        {
          ""interfaceid"": ""107"",
          ""ip"": ""3.9.97.161""
        }
      ]
    },
    {
      ""hostid"": ""10433"",
      ""host"": ""Crtx-6-52.56.174.76"",
      ""interfaces"": [
        {
          ""interfaceid"": ""108"",
          ""ip"": ""52.56.174.76""
        }
      ]
    },
    {
      ""hostid"": ""10455"",
      ""host"": ""bugbounty.vn-db-192.168.1.102"",
      ""interfaces"": [
        {
          ""interfaceid"": ""125"",
          ""ip"": ""192.168.1.102""
        }
      ]
    },
    {
      ""hostid"": ""10424"",
      ""host"": ""vd-wazuh-worker2-52.77.171.5-10.60.1.31"",
      ""interfaces"": [
        {
          ""interfaceid"": ""98"",
          ""ip"": ""10.60.1.31""
        }
      ]
    },
    {
      ""hostid"": ""10454"",
      ""host"": ""bugbounty.vn-103.192.236.186"",
      ""interfaces"": [
        {
          ""interfaceid"": ""124"",
          ""ip"": ""192.168.1.100""
        }
      ]
    },
    {
      ""hostid"": ""10419"",
      ""host"": ""bb-blockchain2-172.31.35.253-18.140.244.48"",
      ""interfaces"": [
        {
          ""interfaceid"": ""94"",
          ""ip"": ""172.31.35.253""
        }
      ]
    },
    {
      ""hostid"": ""10425"",
      ""host"": ""vd-zabx-proxy-18.139.8.62-10.60.1.193"",
      ""interfaces"": [
        {
          ""interfaceid"": ""99"",
          ""ip"": ""10.60.1.193""
        }
      ]
    },
    {
      ""hostid"": ""10431"",
      ""host"": ""vipn-worker2-171.244.36.112"",
      ""interfaces"": [
        {
          ""interfaceid"": ""106"",
          ""ip"": ""171.244.36.112""
        }
      ]
    },
    {
      ""hostid"": ""10434"",
      ""host"": ""Crtx-3-3.9.73.54"",
      ""interfaces"": [
        {
          ""interfaceid"": ""109"",
          ""ip"": ""3.9.73.54""
        }
      ]
    },
    {
      ""hostid"": ""10435"",
      ""host"": ""Crtx-5-3.9.34.24"",
      ""interfaces"": [
        {
          ""interfaceid"": ""110"",
          ""ip"": ""3.9.34.24""
        }
      ]
    },
    {
      ""hostid"": ""10442"",
      ""host"": ""ss-mon-4website-20.0.40.138"",
      ""interfaces"": [
        {
          ""interfaceid"": ""114"",
          ""ip"": ""20.0.40.138""
        }
      ]
    },
    {
      ""hostid"": ""10399"",
      ""host"": ""bb-dev-172.31.35.6-3.0.151.40"",
      ""interfaces"": [
        {
          ""interfaceid"": ""81"",
          ""ip"": ""172.31.35.6""
        }
      ]
    },
    {
      ""hostid"": ""10456"",
      ""host"": ""bugbounty.vn-db-192.168.1.103"",
      ""interfaces"": [
        {
          ""interfaceid"": ""126"",
          ""ip"": ""192.168.1.103""
        }
      ]
    },
    {
      ""hostid"": ""10445"",
      ""host"": ""24h-ids-man2"",
      ""interfaces"": [
        {
          ""interfaceid"": ""117"",
          ""ip"": ""125.212.247.110""
        }
      ]
    },
    {
      ""hostid"": ""10384"",
      ""host"": ""ss-db-20.0.40.166"",
      ""interfaces"": [
        {
          ""interfaceid"": ""68"",
          ""ip"": ""20.0.40.166""
        }
      ]
    },
    {
      ""hostid"": ""10381"",
      ""host"": ""ss-apigw-20.0.20.5-18.139.42.30"",
      ""interfaces"": [
        {
          ""interfaceid"": ""65"",
          ""ip"": ""20.0.20.5""
        }
      ]
    },
    {
      ""hostid"": ""10387"",
      ""host"": ""webserver18"",
      ""interfaces"": [
        {
          ""interfaceid"": ""71"",
          ""ip"": ""222.255.158.34""
        }
      ]
    },
    {
      ""hostid"": ""10355"",
      ""host"": ""ss-be-20.0.30.207-18.139.137.16"",
      ""interfaces"": [
        {
          ""interfaceid"": ""38"",
          ""ip"": ""20.0.30.207""
        },
        {
          ""interfaceid"": ""39"",
          ""ip"": ""20.0.30.123""
        }
      ]
    },
    {
      ""hostid"": ""10389"",
      ""host"": ""ss-dev-scan-core-172.31.4.65-54.255.165.53"",
      ""interfaces"": [
        {
          ""interfaceid"": ""73"",
          ""ip"": ""172.31.4.65""
        }
      ]
    },
    {
      ""hostid"": ""10395"",
      ""host"": ""misc-mail-172.31.10.70-52.76.61.146"",
      ""interfaces"": [
        {
          ""interfaceid"": ""78"",
          ""ip"": ""52.76.61.146""
        },
        {
          ""interfaceid"": ""115"",
          ""ip"": ""172.31.10.70""
        }
      ]
    },
    {
      ""hostid"": ""10391"",
      ""host"": ""ss-dev-172.31.15.63-52.221.105.77"",
      ""interfaces"": [
        {
          ""interfaceid"": ""75"",
          ""ip"": ""172.31.15.63""
        }
      ]
    },
    {
      ""hostid"": ""10380"",
      ""host"": ""ss-blog-172.31.23.79-18.139.231.175"",
      ""interfaces"": [
        {
          ""interfaceid"": ""64"",
          ""ip"": ""172.31.23.79""
        }
      ]
    },
    {
      ""hostid"": ""10400"",
      ""host"": ""misc-gitlab-build-172.31.19.69-13.228.52.236"",
      ""interfaces"": [
        {
          ""interfaceid"": ""82"",
          ""ip"": ""172.31.19.69""
        }
      ]
    },
    {
      ""hostid"": ""10382"",
      ""host"": ""ss-master1-20.0.20.110-54.254.209.201"",
      ""interfaces"": [
        {
          ""interfaceid"": ""66"",
          ""ip"": ""20.0.20.110""
        }
      ]
    },
    {
      ""hostid"": ""10383"",
      ""host"": ""ss-master2-20.0.20.121-13.229.236.175"",
      ""interfaces"": [
        {
          ""interfaceid"": ""67"",
          ""ip"": ""20.0.20.121""
        }
      ]
    },
    {
      ""hostid"": ""10385"",
      ""host"": ""ss-worker-20.0.20.224-54.254.203.217"",
      ""interfaces"": [
        {
          ""interfaceid"": ""69"",
          ""ip"": ""20.0.20.224""
        }
      ]
    },
    {
      ""hostid"": ""10390"",
      ""host"": ""ss-mon-grapite-20.0.40.191"",
      ""interfaces"": [
        {
          ""interfaceid"": ""74"",
          ""ip"": ""20.0.40.191""
        }
      ]
    },
    {
      ""hostid"": ""10392"",
      ""host"": ""ss-mon-icinga-20.0.10.192-54.169.70.201"",
      ""interfaces"": [
        {
          ""interfaceid"": ""76"",
          ""ip"": ""20.0.10.192""
        }
      ]
    },
    {
      ""hostid"": ""10396"",
      ""host"": ""misc-gitlab-172.26.6.92-13.229.69.146"",
      ""interfaces"": [
        {
          ""interfaceid"": ""79"",
          ""ip"": ""13.229.69.146""
        }
      ]
    },
    {
      ""hostid"": ""10398"",
      ""host"": ""vweb-chat1-30.0.0.220-13.250.39.194"",
      ""interfaces"": [
        {
          ""interfaceid"": ""80"",
          ""ip"": ""30.0.0.220""
        }
      ]
    },
    {
      ""hostid"": ""10401"",
      ""host"": ""vweb-db1-330.0.0.171-18.141.11.104"",
      ""interfaces"": [
        {
          ""interfaceid"": ""83"",
          ""ip"": ""30.0.0.171""
        }
      ]
    },
    {
      ""hostid"": ""10402"",
      ""host"": ""ss-fe-20.0.20.22-18.139.243.110"",
      ""interfaces"": [
        {
          ""interfaceid"": ""84"",
          ""ip"": ""20.0.20.22""
        }
      ]
    },
    {
      ""hostid"": ""10403"",
      ""host"": ""vweb-web-proxy-30.0.0.140-52.77.36.99"",
      ""interfaces"": [
        {
          ""interfaceid"": ""85"",
          ""ip"": ""30.0.0.140""
        }
      ]
    },
    {
      ""hostid"": ""10405"",
      ""host"": ""vweb-dev-10.0.20.145-52.220.126.210"",
      ""interfaces"": [
        {
          ""interfaceid"": ""87"",
          ""ip"": ""10.0.20.145""
        }
      ]
    },
    {
      ""hostid"": ""10410"",
      ""host"": ""misc-waz-man1-20.0.10.22-13.229.17.177"",
      ""interfaces"": [
        {
          ""interfaceid"": ""90"",
          ""ip"": ""20.0.10.22""
        }
      ]
    },
    {
      ""hostid"": ""10406"",
      ""host"": ""vweb-chat2-30.0.20.176-3.0.110.216"",
      ""interfaces"": [
        {
          ""interfaceid"": ""88"",
          ""ip"": ""30.0.20.176""
        }
      ]
    },
    {
      ""hostid"": ""10441"",
      ""host"": ""ss-identity-20.0.30.41-54.255.157.226"",
      ""interfaces"": [
        {
          ""interfaceid"": ""113"",
          ""ip"": ""20.0.30.41""
        }
      ]
    },
    {
      ""hostid"": ""10421"",
      ""host"": ""misc-vpn-test-18.139.162.20-172.31.1.126"",
      ""interfaces"": [
        {
          ""interfaceid"": ""95"",
          ""ip"": ""172.31.1.126""
        }
      ]
    },
    {
      ""hostid"": ""10388"",
      ""host"": ""DESKTOP-U581JKO"",
      ""interfaces"": [
        {
          ""interfaceid"": ""72"",
          ""ip"": ""118.70.177.33""
        }
      ]
    },
    {
      ""hostid"": ""10426"",
      ""host"": ""vd-nginx-3.1.75.116-10.60.1.243"",
      ""interfaces"": [
        {
          ""interfaceid"": ""100"",
          ""ip"": ""10.60.1.243""
        }
      ]
    },
    {
      ""hostid"": ""10411"",
      ""host"": ""misc-waz-man2-20.0.30.111-52.74.232.192"",
      ""interfaces"": [
        {
          ""interfaceid"": ""91"",
          ""ip"": ""20.0.30.111""
        }
      ]
    },
    {
      ""hostid"": ""10404"",
      ""host"": ""bb-com-dev-172.31.35.66-18.141.41.154"",
      ""interfaces"": [
        {
          ""interfaceid"": ""86"",
          ""ip"": ""172.31.35.66""
        }
      ]
    },
    {
      ""hostid"": ""10407"",
      ""host"": ""vkyc-api-172.31.11.242-113.229.158.103"",
      ""interfaces"": [
        {
          ""interfaceid"": ""89"",
          ""ip"": ""172.31.11.242""
        }
      ]
    },
    {
      ""hostid"": ""10436"",
      ""host"": ""Crtx-1-3.9.206.22"",
      ""interfaces"": [
        {
          ""interfaceid"": ""111"",
          ""ip"": ""3.9.206.22""
        }
      ]
    },
    {
      ""hostid"": ""10453"",
      ""host"": ""bugbounty.vn-103.192.236.194"",
      ""interfaces"": [
        {
          ""interfaceid"": ""123"",
          ""ip"": ""192.168.1.101""
        }
      ]
    }
  ],
  ""id"": 2
}";

            var mockResponse = Task.FromResult(responseString);
            mockZabbix.Setup(p => p.GetHostProblem()).ReturnsAsync(zabbixJson);

            mocElastic.Setup(o => o.GetDashboardSummary(dataRequest, "Active")).Returns(mockResponse);

            //Act
            var result = dashboardService.GetDashboardSummarys(dataRequest);

            //Assert
            Assert.Equal(result.Result.Active, ex.Result.Active);
            Assert.Equal(result.Result.Disconnect, ex.Result.Disconnect);
            Assert.Equal(result.Result.Healthy, ex.Result.Healthy);
            Assert.Equal(result.Result.UnHealthy, ex.Result.UnHealthy);
        }

        /// <summary>
        /// Provides a common set of functions that are performed just before each test method is called.
        /// </summary>
        internal void Setup()
        {
            mockZabbix = new Mock<ICallApiZabbixHelper>(MockBehavior.Strict);
            mocElastic = new Mock<IElasticSearchCallApiHelper>(MockBehavior.Strict);
            mockDashboardUnitOfWork = new Mock<IDashboardUnitOfWork>(MockBehavior.Strict);
            dashboardService = new DashboardService(mocElastic.Object, mockZabbix.Object, mockDashboardUnitOfWork.Object);
        }
    }
}
