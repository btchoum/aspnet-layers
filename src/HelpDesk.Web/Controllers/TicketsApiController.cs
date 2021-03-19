using System;
using System.Threading;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using HelpDesk.Web.Data;
using HelpDesk.Web.Entities;
using Microsoft.EntityFrameworkCore;

namespace HelpDesk.Web.Controllers
{
    [Route("api/tickets")]
    [ApiController]
    public class TicketsApiController : ControllerBase
    {
        private readonly HelpDeskDbContext _context;

        public TicketsApiController(HelpDeskDbContext context)
        {
            _context = context;
        }

        [HttpGet("")]
        public async Task<IActionResult> Get()
        {
            var tickets = await _context.Tickets.ToListAsync();
            return Ok(tickets);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> Get(int id)
        {
            var ticket = await _context.Tickets
                .FirstOrDefaultAsync(t => t.Id == id);

            if (ticket == null) return NotFound();
            
            return Ok(ticket);
        }

        [HttpPost("")]
        public async Task<IActionResult> Post(TicketCreateModel model, CancellationToken cancellationToken)
        {
            var ticket = new Ticket
            {
                Title = model.Title,
                CreatedAt = DateTime.Now
            };

            _context.Add(ticket);

            await _context.SaveChangesAsync(cancellationToken);

            return Ok();
        }
    }

    public class TicketCreateModel
    {
        public string Title { get; set; }
    }
}
