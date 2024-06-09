using Lib.AspNetCore.ServerSentEvents.Internals;
using Lib.AspNetCore.ServerSentEvents;
using Microsoft.AspNetCore.Authorization.Policy;

namespace SeatMapService.MiddleWares
{
    public class ServerSentEventsMiddleware<TServerSentEventsService> 
{
   

    public async Task Invoke(HttpContext context, IPolicyEvaluator policyEvaluator)
    {
      
    }

    
}
}
