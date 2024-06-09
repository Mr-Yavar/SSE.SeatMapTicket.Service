using Lib.AspNetCore.ServerSentEvents;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Entities.DataProvider;
using SeatMapService.Models;
using Services.Interface;

namespace SeatMapService.Controllers;

    [Route("api/[controller]")]
    [ApiController]
    public class ManageEventsController : ControllerBase
    {

        private ISeatMapServerSentEventsService _seatMapServerSentEventsService { get; set; }
        private RedisCacheService _redisCacheService { get; set; }
        public ManageEventsController(ISeatMapServerSentEventsService seatMapServerSentEventsService, RedisCacheService redis) : base()
        {
            _seatMapServerSentEventsService = seatMapServerSentEventsService;
            _redisCacheService = redis;
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> SetEventTickets([FromBody]EventDto dto)
        {
            
          
            return Ok();


        }

    }


