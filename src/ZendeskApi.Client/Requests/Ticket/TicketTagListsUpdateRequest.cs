using System.Collections.Generic;
using Newtonsoft.Json;

namespace ZendeskApi.Client.Requests
{
    /// <summary>
    /// <see href="https://developer.zendesk.com/rest_api/docs/support/tickets#updating-tag-lists">Updating tag lists</see> <see href="https://developer.zendesk.com/rest_api/docs/support/tickets#update-many-tickets">Update Many Tickets</see>.
    /// </summary>
    public class TicketTagListsUpdateRequest
    {
        /// <summary>
        /// An array of tags to be added to a list of tickets
        /// </summary>
        [JsonProperty("additional_tags")]
        public IList<string> AdditionalTags { get; set; }

        /// <summary>
        /// An array of tags to be removed from a list of tickets
        /// </summary>
        [JsonProperty("remove_tags")]
        public IList<string> RemoveTags { get; set; }
    }
}