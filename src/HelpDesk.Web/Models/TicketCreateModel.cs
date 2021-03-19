using System.ComponentModel.DataAnnotations;

namespace HelpDesk.Web.Models
{
    public class TicketCreateModel
    {
        [Required(ErrorMessage = "Title is required")]
        public string Title { get; set; }
        
        [Required(ErrorMessage = "SubmitterName is required")]
        public string SubmitterName { get; set; }
        
        [Required(ErrorMessage = "SubmitterEmail is required")]
        public string SubmitterEmail { get; set; }
    }
}