using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.Interface;

namespace SeatMapService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public  class NotificationsController : ControllerBase
    {
        private readonly INotificationsServerSentEventsService _serverSentEventsService;

        public NotificationsController(INotificationsServerSentEventsService serverSentEventsService)
        {
            _serverSentEventsService = serverSentEventsService;
            _serverSentEventsService.GetClients();
        }

        [HttpGet("[action]/{text}")]
        public IActionResult AddtoGroup(string GroupName)
        {
            var t =_serverSentEventsService.GetClients();
            _serverSentEventsService.AddToGroup(GroupName, t.ElementAt(0));
            return Ok();
        }

        [HttpGet("[action]/{text}/{GroupName}")]
        public IActionResult Send(string GroupName,string text)
        {
           _serverSentEventsService.SendEventAsync(GroupName,text);
            return Ok();
        }
    }
}
