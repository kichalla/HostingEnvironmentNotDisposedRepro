using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.FileProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace WebApp.Test
{
    public class LeakReproTest : IDisposable
    {
        private readonly TestServer _testServer;
        private readonly HttpClient _client;

        public LeakReproTest()
        {
            var builder = new WebHostBuilder()
                .UseContentRoot(@"..\..\..\..\WebApp") // WebApp.Test\bin\release\netcoreapp2.1\WebApp.Test.dll
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    HostingEnvironment = hostingContext.HostingEnvironment;
                })
                .UseStartup<WebApp.Startup>();
            _testServer = new TestServer(builder);
            _client = _testServer.CreateClient();
        }

        private IHostingEnvironment HostingEnvironment { get; set; }

        // Create fake tests
        public static IEnumerable<object[]> FakeTests
        {
            get
            {
                return Enumerable
                    .Range(0, 100)
                    .Select(i => new object[] { i });
            }
        }

        [Theory]
        [MemberData(nameof(FakeTests))]
        public async Task Test(int testIteration)
        {
            // Arrange & Act
            var response = await _client.GetAsync("/Foo");

            //Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var content = await response.Content.ReadAsStringAsync();
            Assert.Contains("Foo", content);
        }

        public void Dispose()
        {
            //if (HostingEnvironment?.ContentRootFileProvider is PhysicalFileProvider contentPhysicalFileProvider)
            //{
            //    contentPhysicalFileProvider.Dispose();
            //}

            //if (HostingEnvironment?.WebRootFileProvider is PhysicalFileProvider webRootPhysicalFileProvider)
            //{
            //    webRootPhysicalFileProvider.Dispose();
            //}

            _testServer?.Dispose();
        }
    }
}
