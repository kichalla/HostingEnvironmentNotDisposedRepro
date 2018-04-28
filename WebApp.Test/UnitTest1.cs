using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Internal;
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

        public static TheoryData<int> Test1Data
        {
            get
            {
                var d = new TheoryData<int>();
                for (var i = 0; i < 100; i++)
                {
                    d.Add(i);
                }
                return d;
            }
        }
        [Theory]
        [MemberData(nameof(Test1Data))]
        public async Task Test1(int i)
        {
            var response = await Client.GetAsync("/Foo");

            var content = await response.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Contains("Foo", content);
            //Assert.Contains("Sample pages using ASP.NET Core MVC", content);
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
