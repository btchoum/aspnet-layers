using System;

namespace HelpDesk.Web.Entities
{
    public class Ticket
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}