namespace HelpDeskTicketing.Api.Models
{
    public class UpdateTicketDto
    {
        public string Title { get; set; }
        public string Description { get; set; } 
        public TicketPriority Priority { get; set; }
        public TicketStatus Status { get; set; }
    }
}
