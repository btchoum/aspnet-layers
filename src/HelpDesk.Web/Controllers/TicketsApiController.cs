using System;
using System.Threading;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using HelpDesk.Web.Data;
using HelpDesk.Web.Entities;
using HelpDesk.Web.Infrastructure;
using HelpDesk.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace HelpDesk.Web.Controllers
{
    [Route("api/tickets")]
    [ApiController]
    public class TicketsApiController : ControllerBase
    {
        private readonly HelpDeskDbContext _context;
        private readonly IEmailGateway _emailGateway;

        public TicketsApiController(HelpDeskDbContext context, IEmailGateway emailGateway)
        {
            _context = context;
            _emailGateway = emailGateway;
        }

        [HttpGet("")]
        public async Task<IActionResult> Get()
        {
            var tickets = await _context.Tickets.ToListAsync();
            return Ok(tickets);
        }

        [HttpGet("{id:int}", Name = "GetTicketById")]
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
                CreatedAt = DateTime.Now,
                SubmitterName = model.SubmitterName,
                SubmitterEmail = model.SubmitterEmail,
                Status = TicketStatus.New
            };

            _context.Add(ticket);

            await _context.SaveChangesAsync(cancellationToken);

            return CreatedAtRoute("GetTicketById", new {id = ticket.Id}, ticket);
        }

        [HttpPost("{id:int}/close")]
        public async Task<IActionResult> Close(
            [FromRoute] int id,
            [FromBody] CloseTicketModel model, 
            CancellationToken cancellationToken)
        {
            // Get ticket from database
            var ticket = await _context.Tickets
                .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);

            if (ticket == null) return NotFound();

            if (ticket.Status == TicketStatus.Closed || ticket.Status == TicketStatus.Cancelled)
            {
                return BadRequest($"Ticket is currently {ticket.Status}");
            }
            
            //Update the ticket as closed
            ticket.Status = TicketStatus.Closed;
            ticket.ClosedAt = DateTime.UtcNow;
            if (!string.IsNullOrEmpty(model.Comment))
            {
                ticket.ClosureMessage = model.Comment;
            }
            await _context.SaveChangesAsync(cancellationToken);

            // send email to requestor re: closure
            var messageBody = "Your ticket was closed";
            if (!string.IsNullOrEmpty(ticket.ClosureMessage))
            {
                messageBody += $"<p><b>Closure Comment:</b></p> <p>{ticket.ClosureMessage}</p>";
            }

            var email = new Email
            {
                Subject = "Ticket Closed",
                To = ticket.SubmitterEmail,
                Body = messageBody
            };

            await _emailGateway.SendAsync(email);
            
            return Ok();
        }
    }
}
