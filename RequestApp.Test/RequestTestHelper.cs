using RequestApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace RequestApp.Test
{
    public static class RequestTestHelper
    {
        public const int REQUEST_TIMEOUT = 2000;

        public static HttpContent CreateRequestModelHttpContent(string resouseName) =>
            new StringContent(JsonSerializer.Serialize(RequestTestHelper.CreateRequestModel(resouseName)),
                System.Text.Encoding.UTF8,
                "application/json");

        public static HttpContent CreateAccessModelHttpContent(string resouseName, AccessAction accessAction) =>
            new StringContent(JsonSerializer.Serialize(RequestTestHelper.CreateAccessModel(resouseName, accessAction)),
                System.Text.Encoding.UTF8,
                "application/json");

        private static RequestModel CreateRequestModel(string resouseName) =>
            new RequestModel { Resource = resouseName };

        private static AccessModel CreateAccessModel(string resouseName, AccessAction action) =>
            new AccessModel { Resource = resouseName, Action = action };
    }
}
