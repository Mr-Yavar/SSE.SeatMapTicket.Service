using Entities;
using Entities.DataProvider;
using Entities.Models;
using Lib.AspNetCore.ServerSentEvents;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Services.Interface;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services
{
    public class SeatMapServerSentEventsService : ServerSentEventsService, ISeatMapServerSentEventsService
    {
        public SeatMapServerSentEventsService(IOptions<ServerSentEventsServiceOptions<SeatMapServerSentEventsService>> options,IDatabase redis ) : base(options.ToBaseServerSentEventsServiceOptions())
        {
            redis.Multiplexer.GetSubscriber().Subscribe("__keyspace@0__:*",
                    (channel, message) => {

                        //Lock Ticket
                        //{eventId}.{ticketId}

                       


                        Console.WriteLine($"received {message} on {channel}");

                        string[] parts = channel.ToString().Split(':');

                        string subKey = parts[0];
                        string key = parts[1];
                        

                        switch(message)
                        {
                            case "set":
                                if (key.Contains("."))
                                {

                                    string[] info = key.Split(".");


                                    PublishLockedTicket(groupName: info[0], Guid.Parse(info[1]));

                                }


                               //set commend
                                break;
                            case "expire":
                               //set command and update call it
                                break;
                            case "expired":
                                //when it exipred
                                if (key.Contains("."))
                                {

                                    string[] info = key.Split(".");


                                    PublishUnLockedTicket(groupName: info[0], Guid.Parse(info[1]));

                                }
                                break;
                        }

                    }
                    );
        }

        /// <summary>
        /// اطلاع رسانی صندلی ازاد شده به همه کاربران حاضر در رویداد
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="Id"></param>
        private void PublishUnLockedTicket(string groupName,Guid Id)
        {
            string res = System.Text.Json.JsonSerializer.Serialize<MessageDto<Guid>>(value: new() { Message = Id });
            this.SendEventAsync(groupName, new ServerSentEvent()
            {
                Id = Guid.NewGuid().ToString(),
                Data = new List<string> { res },
                Type = EventTypes.UnLockedTciket,
            });

            
        }

        /// <summary>
        /// اطلاع رسانی صندلی قفل شده به همه کاربران حاضر در رویداد
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="Id"></param>
        private void PublishLockedTicket(string groupName, Guid Id)
        {
            string res = System.Text.Json.JsonSerializer.Serialize<MessageDto<Guid>>(value: new() { Message = Id });
            this.SendEventAsync(groupName, new ServerSentEvent()
            {
                Id = Guid.NewGuid().ToString(),
                Data = new List<string> { res },
                Type = EventTypes.LockedTciket,
            });

            
        }

        
    }
}
