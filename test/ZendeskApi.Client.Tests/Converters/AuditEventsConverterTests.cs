using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Xunit;
using ZendeskApi.Client.Responses;

namespace ZendeskApi.Client.Tests.Converters
{
    public class AuditEventsConverterTests
    {
        [Fact]
        public void TicketAudit_Should_DeserializeEvents()
        {
            var json = File.ReadAllText(AppContext.BaseDirectory + "/Converters/ticketAuditMultiSearchOneResult.json");
            var audit = JsonConvert.DeserializeObject<TicketAuditResponse>(json);
            
            Assert.Equal(6, audit.First().Events.Count());
        }

        [Fact]
        public void TicketAudit_Should_DeserializeEvents_When_SingleResult()
        {
            var json = File.ReadAllText(AppContext.BaseDirectory + "/Converters/ticketAuditSingleResult.json");
            var audit = JsonConvert.DeserializeObject<SingleTicketAuditResponse>(json);
            
            Assert.Equal(6, audit.Audit.Events.Count());
        }

        [Fact]
        public void TicketAudit_Should_NotFail_When_NoEvents()
        {
            var json = File.ReadAllText(AppContext.BaseDirectory + "/Converters/ticketAuditMultiSearchNoEvents.json");
            var audit = JsonConvert.DeserializeObject<TicketAuditResponse>(json);
            
            Assert.Empty(audit.First().Events);
        }
    }
}