using Microsoft.Extensions.Options;
using Microsoft.VisualBasic.FileIO;
using RequestApp.Domain;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RequestApp.Application.Requests
{
    public interface IRequestService
    {
        Task<Request> AddRequestAsync(string resource);
        void GrantRequest(string resource);
        void DenyRequest(string resource);
    }

    public class RequestService : IRequestService
    {
        private readonly RequestSettings _requestSettings;

        public RequestService(IOptions<RequestSettings> requestSettingsOptions)
        {
            _requestSettings = requestSettingsOptions.Value;
        }

        private readonly ConcurrentDictionary<string, RequestPromise> _requests = new ConcurrentDictionary<string, RequestPromise>();

        public Task<Request> AddRequestAsync(string resource)
        {
            var cts = new CancellationTokenSource();
            var tcsRequest = new TaskCompletionSource<Request>();
            _requests.TryAdd(resource, new RequestPromise(tcsRequest, cts));

            var task = Task.Delay(_requestSettings.RequestTimeout, cts.Token);
            task.ContinueWith(task =>
            {
                if (!_requests.TryRemove(resource, out var requestPromise))
                    return;

                var request = new Request(resource);
                request.DeniedByTimeout();

                requestPromise.TaskCompletionSourceRequest.TrySetResult(request);
            });

            return tcsRequest.Task;
        }

        public void GrantRequest(string resource)
        {
            var request = new Request(resource);
            request.Granted();

            ApplyRequest(resource, request);
        }

        public void DenyRequest(string resource)
        {
            var request = new Request(resource);
            request.DeniedByUser();

            ApplyRequest(resource, request);
        }

        private void ApplyRequest(string resource, Request request)
        {
            if (!_requests.TryRemove(resource, out var tcsRequest))
                return;

            if (tcsRequest.TaskCompletionSourceRequest.TrySetResult(request))
                tcsRequest.CancellationTokenSource.Cancel();
        }
    }

    internal class RequestPromise
    {
        public RequestPromise(TaskCompletionSource<Request> taskCompletionSourceRequest, CancellationTokenSource cancellationTokenSource)
        {
            TaskCompletionSourceRequest = taskCompletionSourceRequest;
            CancellationTokenSource = cancellationTokenSource;
        }

        public TaskCompletionSource<Request> TaskCompletionSourceRequest { get; set; }
        public CancellationTokenSource CancellationTokenSource { get; set; }
    } 
}