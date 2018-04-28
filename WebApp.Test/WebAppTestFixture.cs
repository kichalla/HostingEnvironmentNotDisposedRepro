using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.FileProviders;
using System;
using System.IO;
using System.Net.Http;
using System.Reflection;

namespace WebApp.Test
{
    public class WebAppTestFixture : IDisposable
    {
        private readonly TestServer _testServer;

        public WebAppTestFixture()
        {
            var builder = new WebHostBuilder()
                .UseContentRoot(GetWebApplicationRootFolder())
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    HostingEnvironment = hostingContext.HostingEnvironment;
                })
                .UseStartup<WebApp.Startup>();
            _testServer = new TestServer(builder);
            Client = _testServer.CreateClient();
        }

        private IHostingEnvironment HostingEnvironment { get; set; }

        public HttpClient Client { get; }

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

        private string GetWebApplicationRootFolder()
        {
            // FileSystemWatcherRepro\
            //  - FileSystemWatcherRepro.sln
            //  - WebApp\
            //  - WebApp.Test\bin\release\netcoreapp2.1\WebApp.Test.dll
            var currentLocation = Assembly.GetExecutingAssembly().Location;
            var currentDir = new FileInfo(currentLocation).Directory;
            while (currentDir.GetFiles("FileSystemWatcherRepro.sln").Length == 0)
            {
                Console.WriteLine(currentDir.FullName);
                currentDir = currentDir.Parent;
            }
            return Path.Combine(currentDir.FullName, "WebApp");
        }
    }
}
