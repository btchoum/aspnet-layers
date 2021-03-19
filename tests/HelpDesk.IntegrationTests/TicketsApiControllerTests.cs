using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using HelpDesk.Web;
using HelpDesk.Web.Entities;
using HelpDesk.Web.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace HelpDesk.IntegrationTests
{
    public class TicketsApiControllerTests : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly WebApplicationFactory<Startup> _factory;

        public TicketsApiControllerTests(WebApplicationFactory<Startup> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task Get_All_Tickets()
        {
            var client = _factory.CreateClient();

            var response = await client.GetAsync("api/tickets");

            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task Create_Ticket()
        {
            var client = _factory.CreateClient();

            // Create new ticket
            var payload = new TicketCreateModel
            {
                Title = "Test Title",
                SubmitterName = "John Doe",
                SubmitterEmail = "john@example.com"
            };
            
            var response = await client.PostAsJsonAsync("api/tickets", payload);
            response.EnsureSuccessStatusCode();

            // Read the ticket back
            var location = response.Headers.Location?.ToString();
            Assert.NotNull(location);
            var getResponse = await client.GetAsync(location);
            getResponse.EnsureSuccessStatusCode();

            var ticket = await getResponse.Content.ReadFromJsonAsync<Ticket>();
            Assert.NotNull(ticket);
            
            Assert.Equal(payload.Title, ticket.Title);
            Assert.Equal(payload.SubmitterName, ticket.SubmitterName);
            Assert.Equal(payload.SubmitterEmail, ticket.SubmitterEmail);
            Assert.Equal(TicketStatus.New, ticket.Status);
        }

        [Fact]
        public async Task Closing_Ticket()
        {
            var ticketId = await GivenExistingTicketId();
            
            var client = _factory.CreateClient();

            var payload = new CloseTicketModel
            {
                Comment = "Test Close Comment"
            };
            var url = $"/api/tickets/{ticketId}/close";
            var response = await client.PostAsJsonAsync(url, payload);
            response.EnsureSuccessStatusCode();

            var ticket = await GetTicketById(ticketId);
            
            Assert.Equal(TicketStatus.Closed, ticket.Status);
        }


        [Fact]
        public async Task Cannot_Close_A_Closed_Ticket()
        {
            var ticketId = await GivenExistingTicketId();

            var client = _factory.CreateClient();

            var payload = new CloseTicketModel
            {
                Comment = "Test Close Comment"
            };
            var url = $"/api/tickets/{ticketId}/close";
            var response = await client.PostAsJsonAsync(url, payload);
            response.EnsureSuccessStatusCode();

            var secondResponse = await client.PostAsJsonAsync(url, payload);
            Assert.Equal(HttpStatusCode.BadRequest, secondResponse.StatusCode);
        }

        private async Task<int> GivenExistingTicketId()
        {
            var client = _factory.CreateClient();

            // Create new ticket
            var payload = new TicketCreateModel
            {
                Title = "Test Title",
                SubmitterName = "John Doe",
                SubmitterEmail = "john@example.com"
            };

            var response = await client.PostAsJsonAsync("api/tickets", payload);
            response.EnsureSuccessStatusCode();

            var ticket = await response.Content.ReadFromJsonAsync<Ticket>();
            Assert.NotNull(ticket);

            return ticket.Id;
        }

        private async Task<Ticket> GetTicketById(int id)
        {
            var client = _factory.CreateClient();

            var url = $"/api/tickets/{id}";


            var response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var ticket = await response.Content.ReadFromJsonAsync<Ticket>();
            Assert.NotNull(ticket);

            return ticket;
        }
    }
}
