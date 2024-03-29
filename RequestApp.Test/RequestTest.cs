using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities.Resources;
using RequestApp.Application;
using RequestApp.Application.Requests;
using RequestApp.Controllers;
using RequestApp.Domain;
using RequestApp.Models;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace RequestApp.Test
{
    public class RequestTest : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private const string requestApiUrl = "api/request";
        private const string accessApiUrl = "api/access";

        public RequestTest(WebApplicationFactory<Program> factory)
        {
            _factory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services.Configure<RequestSettings>(opts =>
                    {
                        opts.RequestTimeout = RequestTestHelper.REQUEST_TIMEOUT;
                    });
                });
            });
        }

        [Fact]
        public async Task CanGrantRequest()
        {
            var resourceName = Guid.NewGuid().ToString();
            var httpClient = _factory.CreateClient();

            HttpResponseMessage requestResult = default;
            HttpResponseMessage accessResult = default;

            var taskRequest = httpClient.PostAsync(requestApiUrl, RequestTestHelper.CreateRequestModelHttpContent(resourceName)).ContinueWith(async task => 
            {
                requestResult = await task;
            });

            await Task.Delay(300);

            var taskAccess = httpClient.PostAsync(accessApiUrl, RequestTestHelper.CreateAccessModelHttpContent(resourceName, AccessAction.Grant)).ContinueWith(async task =>
            {
                accessResult = await task;
            });

            await Task.WhenAll(taskRequest, taskAccess);


            var requestResponce = JsonSerializer.Deserialize<RequestResponse>(await requestResult.Content.ReadAsStringAsync(), new JsonSerializerOptions 
            { 
                Converters = { new JsonStringEnumConverter() } 
            });

            Assert.NotNull(requestResponce);
            Assert.Equal(requestResponce.Resource, resourceName);
            Assert.Equal(requestResponce.Decision, RequestStatus.Granted);
            Assert.Empty(requestResponce.Reason);
        }

        [Fact]
        public async Task CanDenyRequest()
        {
            var resourceName = Guid.NewGuid().ToString();
            var httpClient = _factory.CreateClient();

            HttpResponseMessage requestResult = default;
            HttpResponseMessage accessResult = default;

            var taskRequest = httpClient.PostAsync(requestApiUrl, RequestTestHelper.CreateRequestModelHttpContent(resourceName)).ContinueWith(async task =>
            {
                requestResult = await task;
            });

            await Task.Delay(300);

            var taskAccess = httpClient.PostAsync(accessApiUrl, RequestTestHelper.CreateAccessModelHttpContent(resourceName, AccessAction.Deny)).ContinueWith(async task =>
            {
                accessResult = await task;
            });

            await Task.WhenAll(taskRequest, taskAccess);


            var requestResponce = JsonSerializer.Deserialize<RequestResponse>(await requestResult.Content.ReadAsStringAsync(), new JsonSerializerOptions
            {
                Converters = { new JsonStringEnumConverter() }
            });

            Assert.NotNull(requestResponce);
            Assert.Equal(requestResponce.Resource, resourceName);
            Assert.Equal(requestResponce.Decision, RequestStatus.Denied);
            Assert.Equal(requestResponce.Reason, "Denied by user");
        }

        [Fact]
        public async Task DenyRequest_WhenTimeoutIsOver()
        {
            var resourceName = Guid.NewGuid().ToString();
            var httpClient = _factory.CreateClient();

            var taskRequest = await httpClient.PostAsync(requestApiUrl, RequestTestHelper.CreateRequestModelHttpContent(resourceName));

            var requestResponce = JsonSerializer.Deserialize<RequestResponse>(await taskRequest.Content.ReadAsStringAsync(), new JsonSerializerOptions
            {
                Converters = { new JsonStringEnumConverter() }
            });

            Assert.NotNull(requestResponce);
            Assert.Equal(requestResponce.Resource, resourceName);
            Assert.Equal(requestResponce.Decision, RequestStatus.Denied);
            Assert.Equal(requestResponce.Reason, "Timeout expired");
        }

        [Fact]
        public async Task CanAccess_WhenSendUnsupportedAction()
        {
            var resourceName = Guid.NewGuid().ToString();
            var httpClient = _factory.CreateClient();

            var taskAccess = await httpClient.PostAsync(accessApiUrl, RequestTestHelper.CreateAccessModelHttpContent(resourceName, (AccessAction)3));

            Assert.NotNull(taskAccess);
            Assert.Equal(taskAccess.StatusCode, HttpStatusCode.OK);
        }
    }
}