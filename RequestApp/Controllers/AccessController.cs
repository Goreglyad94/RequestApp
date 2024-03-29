using Microsoft.AspNetCore.Mvc;
using RequestApp.Application.Requests;
using RequestApp.Models;

namespace RequestApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccessController : ControllerBase
    {
        private readonly IRequestService _requestService;

        public AccessController(IRequestService requestService)
        {
            _requestService = requestService;
        }

        [HttpPost]
        public IActionResult ApplyRequestToResource(AccessModel accessModel)
        {
            switch (accessModel.Action)
            {
                case AccessAction.Grant:
                    _requestService.GrantRequest(accessModel.Resource);
                    break;
                case AccessAction.Deny:
                    _requestService.DenyRequest(accessModel.Resource);
                    break;
            }

            return Ok();
        }
    }
}
