using System.Collections.Generic;
using Newtonsoft.Json;
using ZendeskApi.Client.Models;

namespace ZendeskApi.Client.Responses
{
    [JsonObject]
    public class OrganizationMembershipsResponse : PaginationResponse<OrganizationMembership>
    {
        [JsonProperty("organization_memberships")]
        public IEnumerable<OrganizationMembership> OrganizationMemberships { get; internal set; }
        
        protected override IEnumerable<OrganizationMembership> Enumerable => OrganizationMemberships;
    }
}
