using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RequestApp.Application.Requests;
using RequestApp.Models;

namespace RequestApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RequestController : ControllerBase
    {
        private readonly IRequestService _requestService;
        private readonly IMapper _mapper;

        public RequestController(IRequestService requestService, IMapper mapper)
        {
            _requestService = requestService;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<IActionResult> SetRequestToResource(RequestModel requestModel)
        {
            var request = await _requestService.AddRequestAsync(requestModel.Resource);
            var response = _mapper.Map<RequestResponse>(request);

            return Ok(response);
        }
    }
}
