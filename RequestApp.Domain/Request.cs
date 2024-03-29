namespace RequestApp.Domain
{
    public class Request
    {
        public Request(string resource)
        {
            Resource = resource;
        }

        public string? Resource { get; private set; }
        public RequestStatus Decision { get; private set; }
        public string Reason { get; private set; }

        public void Granted()
        {
            this.SetStatus(RequestStatus.Granted);
            this.Reason = string.Empty;
        }

        public void DeniedByUser()
        {
            this.Reason = "Denied by user";
            this.SetStatus(RequestStatus.Denied);
        }

        public void DeniedByTimeout()
        {
            this.Reason = "Timeout expired";
            this.SetStatus(RequestStatus.Denied);
        }

        private void SetStatus(RequestStatus requestStatus)
        {
            this.Decision = requestStatus;
        }
    }

    public enum RequestStatus
    {
        Granted,
        Denied
    }
}
