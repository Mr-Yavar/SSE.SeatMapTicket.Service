namespace SeatMapService.Models
{
    public class EventDto
    {
        public Guid Id { get; set; }

        public List<Guid> TicketIds { get; set; }
    }
}
