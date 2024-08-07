using Entities.Models;
using Lib.AspNetCore.ServerSentEvents;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Entities.DataProvider;
using Services.Interface;
using Services.Services;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Entities;

namespace SeatMapService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SeatMapController : ControllerBase
    {

        private ISeatMapServerSentEventsService _seatMapServerSentEventsService { get; set; }
        private RedisCacheService _redisCacheService { get; set; }
        public SeatMapController(ISeatMapServerSentEventsService seatMapServerSentEventsService, RedisCacheService redis) : base()
        {
            _seatMapServerSentEventsService = seatMapServerSentEventsService;
            _redisCacheService = redis;
        }

        [HttpGet("[action]/{cid}/{EventId}")]
        public async Task<IActionResult> ChooseEvent(Guid cid, Guid EventId)
        {

            var client = _seatMapServerSentEventsService.GetClient(cid);

            if (client == null)
                return BadRequest();

            _seatMapServerSentEventsService.AddToGroup(EventId.ToString(), client);
            List<Guid> TicketIds = _redisCacheService.LockedTickets(EventId.ToString() + ".*");

            await client.SendEventAsync(new ServerSentEvent()
            {Id = Guid.NewGuid().ToString(),
                Type = EventTypes.AllLockedTcikets,
                Data = new List<string>() { System.Text.Json.JsonSerializer.Serialize(TicketIds) }
            });

            return Ok(new ServerSentEvent()
            {
                Id = Guid.NewGuid().ToString(),
                Type = EventTypes.AllLockedTcikets,
                Data = new List<string>() { System.Text.Json.JsonSerializer.Serialize(TicketIds) }
            });


        }

        [HttpGet("[action]/{cid}/{EventId}")]
        public async Task<IActionResult> LeaveEvent(Guid cid, Guid EventId)
        {

            var client = _seatMapServerSentEventsService.GetClient(cid);

            if (client == null)
                return BadRequest();

            _seatMapServerSentEventsService.RemoveFromGroup(EventId.ToString(), client);


            return Ok();


        }



        [HttpPost("[action]")]
        public async Task<IActionResult> ReservedTickets(Guid EventId, List<Guid> TicketIds)
        {
            string res = System.Text.Json.JsonSerializer.Serialize<MessageDto<List<Guid>>>(value: new() { Message = TicketIds });
            await _seatMapServerSentEventsService.SendEventAsync(EventId.ToString(), new ServerSentEvent()
            {
                Id = Guid.NewGuid().ToString(),
                Data = new List<string> { res },
                Type = EventTypes.ReservedTcikets,
            });
            return Ok(new { });
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> LockTickets(Guid EventId, List<Guid> TicketIds)
        {


            string key = "";
            bool isOk = true;
            foreach (var ticket in TicketIds)
            {
                key = EventId.ToString() + "." + ticket.ToString();

                if (_redisCacheService.IsExist(key))
                {
                    isOk = false;
                }


            }

            if (isOk)
            {

                foreach (var ticket in TicketIds)
                {
                    key = EventId.ToString() + "." + ticket.ToString();
                    _redisCacheService.AddKey<string>(key, "", TimeSpan.FromMinutes(15));
                }
                string res = System.Text.Json.JsonSerializer.Serialize<MessageDto<List<Guid>>>(value: new() { Message = TicketIds });
                await _seatMapServerSentEventsService.SendEventAsync(EventId.ToString(), new ServerSentEvent()
                {
                    Id = Guid.NewGuid().ToString(),
                    Data = new List<string> { res },
                    Type = EventTypes.LockedTciket,
                });
                return Ok(new { });
            }
            else
            {
                return BadRequest(new() { });
            }
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> UnLockTickets(Guid EventId, List<Guid> TicketIds)
        {
            string key = "";
            bool isOk = true;
            foreach (var ticket in TicketIds)
            {
                key = EventId.ToString() + "." + ticket.ToString();

                if (_redisCacheService.IsExist(key))
                {
                    _redisCacheService.Remove(EventId.ToString() + "." + ticket.ToString());
                }


            }

            string res = System.Text.Json.JsonSerializer.Serialize<MessageDto<List<Guid>>>(value: new() { Message = TicketIds });
            await _seatMapServerSentEventsService.SendEventAsync(EventId.ToString(), new ServerSentEvent()
            {
                Id = Guid.NewGuid().ToString(),
                Data = new List<string> { res },
                Type = EventTypes.UnLockedTciket,
            });
            return Ok();

        }

        [HttpGet("[action]/{Cid}")]
        public async Task<IActionResult> GetClient(Guid Cid)
        {
            var client = _seatMapServerSentEventsService.GetClient(Cid);
            var clients = _seatMapServerSentEventsService.GetClients();
            return Ok();
        }


        [HttpGet("[action]/{Cid}")]
        public async Task<IActionResult> DisconnectClient(Guid Cid)
        {
            var client = _seatMapServerSentEventsService.GetClient(Cid);
            await client.DisconnectAsync();
            return Ok();
        }


        [HttpGet("[action]/{Cid}/{GroupName}")]
        public async Task<IActionResult> toGroup(Guid Cid, string GroupName)
        {
            var client = _seatMapServerSentEventsService.GetClient(Cid);
            _seatMapServerSentEventsService.AddToGroup(GroupName, client);

            return Ok();
        }


        [HttpGet("[action]/{GroupName}/{msg}")]
        public async Task<IActionResult> msgGroup(string GroupName, string msg)
        {

            _seatMapServerSentEventsService.SendEventAsync(GroupName, msg);

            return Ok();
        }



        [HttpGet("[action]/{Cid}/{eventName}/{message}")]
        public IActionResult sendEvent(Guid Cid, string eventName, string message)
        {
            var client = _seatMapServerSentEventsService.GetClient(Cid);
            client.SendEventAsync(new ServerSentEvent() { Type = eventName, Data = message.Split(',').ToList() });

            return Ok();
        }
    }
}
