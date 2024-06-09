using Lib.AspNetCore.ServerSentEvents;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interface
{
    public class ServerSentEventsClientIdProvider : IServerSentEventsClientIdProvider
    {
        
        public Guid AcquireClientId(HttpContext context)
        {
            context.Request.Query.TryGetValue("cid", out var clientId);

            if (Guid.TryParse(clientId, out Guid GClientId))
                return GClientId;

            return Guid.Empty;
        }

        public void ReleaseClientId(Guid clientId, HttpContext context)
        {
           
        }
    }
}
