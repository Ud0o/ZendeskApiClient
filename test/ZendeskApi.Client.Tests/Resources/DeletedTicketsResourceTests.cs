using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using Newtonsoft.Json;
using Xunit;
using ZendeskApi.Client.Resources;
using ZendeskApi.Client.Models;
using ZendeskApi.Client.Tests.ResourcesSampleSites;

namespace ZendeskApi.Client.Tests.Resources
{
    public class DeletedTicketsResourceTests : IDisposable
    {
        private readonly IZendeskApiClient _client;
        private readonly DeletedTicketsResource _resource;
        private readonly Dictionary<long, Ticket> _ticketState = new Dictionary<long, Ticket>();

        public DeletedTicketsResourceTests()
        {
            _client = new DisposableZendeskApiClient(resource => new DeletedTicketResourceSampleSite(resource, _ticketState));
            _resource = new DeletedTicketsResource(_client, NullLogger.Instance);
        }

        [Fact]
        public async Task ShouldListDeletedTickets()
        {
            var tickets = CreateTickets(2);

            var retrievedTickets = (await _resource.ListAsync()).ToArray();

            Assert.Equal(2, retrievedTickets.Length);
            Assert.Equal(JsonConvert.SerializeObject(tickets[0]), JsonConvert.SerializeObject(retrievedTickets[0]));
            Assert.Equal(JsonConvert.SerializeObject(tickets[1]), JsonConvert.SerializeObject(retrievedTickets[1]));
        }
        
        [Fact]
        public async Task ShouldRestoreTicket()
        {
            var ticket = CreateTickets(1).First();

            await _resource.RestoreAsync(ticket.Id);

            Assert.False(_ticketState.ContainsKey(ticket.Id));
        }

        [Fact]
        public async Task ShouldRestoreTicketsForIds()
        {
            var tickets = CreateTickets(2);
            
            await _resource.RestoreAsync(tickets.Select(t => t.Id));
            
            Assert.Equal(0, _ticketState.Count);
        }

        [Fact]
        public async Task ShouldPurgeTicket()
        {
            var ticket = CreateTickets(1).First();
            Assert.Equal(1, _ticketState.Count);

            var jobResponse = await _resource.PurgeAsync(ticket.Id);

            Assert.NotNull(jobResponse);
            Assert.Equal(0, _ticketState.Count);
        }

        [Fact]
        public async Task ShouldPurgeMultipleTickets()
        {
            var tickets = CreateTickets(3);
            var ticketIds = tickets.Select(t => t.Id).ToArray();
            Assert.Equal(3, _ticketState.Count);

            await _resource.PurgeAsync(ticketIds);

            Assert.Equal(0, _ticketState.Count);
        }

        private List<Ticket> CreateTickets(int numberOfTicketsToCreate)
        {
            var tickets = new List<Ticket>();
            for (var i = 0; i < numberOfTicketsToCreate; i++)
            {
                var ticket = new Ticket
                {
                    Id = i,
                    Subject = "My printer is on fire! " + i
                };

                _ticketState.Add(ticket.Id, ticket);
                tickets.Add(ticket);
            }

            return tickets;
        }

        public void Dispose()
        {
            ((IDisposable)_client).Dispose();
        }
    }
}
