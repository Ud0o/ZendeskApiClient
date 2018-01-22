using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ZendeskApi.Client.Models
{
    [JsonObject("satisfaction_rating")]
    public class SatisfactionRating
    {
        [JsonProperty]
        public long? Id { get; set; }

        [JsonProperty("group_id")]
        public long GroupId { get; set; }

        [JsonProperty("assignee_id")]
        public long AssigneeId { get; set; }

        [JsonProperty("requester_id")]
        public long RequesterId { get; set; }

        [JsonProperty("ticket_id")]
        public long TicketId { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        [JsonProperty("score")]
        public SatisfactionRatingScore Score { get; set; }

        [JsonProperty("created_at")]
        public DateTime? CreatedAt { get; set; }

        [JsonProperty("updated_at")]
        public DateTime? UpdatedAt { get; set; }

        [JsonProperty("comment")]
        public string Comment { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }
    }
}
