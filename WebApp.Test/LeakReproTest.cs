using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.FileProviders;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using Xunit;

namespace WebApp.Test
{
    public class LeakReproTest : IClassFixture<WebAppTestFixture>
    {
        private readonly HttpClient _client;

        public LeakReproTest(WebAppTestFixture testFixture)
        {
            _client = testFixture.Client;
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
            // Arrange & Act
            var response = await _client.GetAsync("/Foo");

            //Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var content = await response.Content.ReadAsStringAsync();
            Assert.Contains("Foo", content);
        }
    }
}
