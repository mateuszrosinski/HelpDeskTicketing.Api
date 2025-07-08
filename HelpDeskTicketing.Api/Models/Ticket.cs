//using static HelpDeskTicketing.Api.Models.TicketEnums;

using System.ComponentModel.DataAnnotations.Schema;

namespace HelpDeskTicketing.Api.Models
{
    public class Ticket
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public TicketPriority Priority { get; set; }
        public TicketStatus Status { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // 1. Właściwość przechowująca klucz obcy (ID użytkownika)
        public string OwnerId { get; set; }

        // 2. Właściwość nawigacyjna, która reprezentuje powiązany obiekt użytkownika
        [ForeignKey("OwnerId")]
        public virtual ApplicationUser Owner { get; set; }

    }
}
