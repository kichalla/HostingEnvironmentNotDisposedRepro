using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
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
        public IHostingEnvironment HostingEnvironment { get; private set; }

        public TestServer TestServer { get; }
        public HttpClient Client { get; }

        public LeakReproTest()
        {
            var builder = new WebHostBuilder()
                .UseContentRoot(@"C:\Users\kichalla\source\repos\FileSystemWatcherRepro\WebApp")
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    HostingEnvironment = hostingContext.HostingEnvironment;
                })
                .UseStartup<WebApp.Startup>();
            TestServer = new TestServer(builder);
            Client = TestServer.CreateClient();
        }

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
            var response = await Client.GetAsync("/Foo");

            var content = await response.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
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

            TestServer?.Dispose();
        }
    }
}
