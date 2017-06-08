﻿using System.Collections.Generic;
using Newtonsoft.Json;
using ZendeskApi.Client.Models;

namespace ZendeskApi.Client.Requests
{
    public class TicketRequest
    {
        [JsonProperty("ticket")]
        public Ticket Item { get; set; }
    }

    public class TicketsRequest
    {
        [JsonProperty("tickets")]
        public IEnumerable<Ticket> Item { get; set; }
    }
}
