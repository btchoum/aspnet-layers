using System;

namespace HelpDesk.Web.Entities
{
    public class Ticket
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string SubmitterEmail { get; set; }
        public string SubmitterName { get; set; }
        public TicketStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ClosedAt { get; set; }
        public string ClosureMessage { get; set; }
    }

    public enum TicketStatus
    {
        New = 1,
        Active,
        Resolved,
        Pending,
        Closed,
        Cancelled
    }
}