using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ZendeskApi.Client.Extensions;
using ZendeskApi.Client.Models;
using ZendeskApi.Client.Requests;
using ZendeskApi.Client.Responses;

namespace ZendeskApi.Client.Resources
{
    /// <summary>
    /// <see cref="https://developer.zendesk.com/rest_api/docs/core/groups"/>
    /// </summary>
    public class GroupsResource : IGroupsResource
    {
        private const string GroupsResourceUri = "api/v2/groups";
        private const string GroupsByUserResourceUriFormat = "api/v2/users/{0}/groups";
        private const string AssignableGroupUri = @"api/v2/groups/assignable";

        private readonly IZendeskApiClient _apiClient;
        private readonly ILogger _logger;

        private readonly Func<ILogger, string, IDisposable> _loggerScope =
            LoggerMessage.DefineScope<string>(typeof(GroupsResource).Name + ": {0}");

        public GroupsResource(IZendeskApiClient apiClient,
            ILogger logger)
        {
            _apiClient = apiClient;
            _logger = logger;
        }

        public async Task<GroupListResponse> ListAsync(PagerParameters pager = null)
        {
            using (_loggerScope(_logger, "ListAsync"))
            using (var client = _apiClient.CreateClient())
            {
                var response = await client.GetAsync(GroupsResourceUri, pager).ConfigureAwait(false);

                response.EnsureSuccessStatusCode();

                return await response.Content.ReadAsAsync<GroupListResponse>();
            }
        }

        public async Task<GroupListResponse> ListAsync(long userId, PagerParameters pager = null)
        {
            using (_loggerScope(_logger, $"ListAsync({userId})"))
            using (var client = _apiClient.CreateClient())
            {
                var response = await client.GetAsync(string.Format(GroupsByUserResourceUriFormat, userId), pager).ConfigureAwait(false);

                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    _logger.LogInformation("UserResponse {0} not found", userId);
                    return null;
                }

                response.EnsureSuccessStatusCode();

                return await response.Content.ReadAsAsync<GroupListResponse>();
            }
        }

        public async Task<GroupListResponse> ListAssignableAsync(PagerParameters pager = null)
        {
            using (_loggerScope(_logger, "ListAssignableAsync"))
            using (var client = _apiClient.CreateClient())
            {
                var response = await client.GetAsync(AssignableGroupUri, pager).ConfigureAwait(false);

                response.EnsureSuccessStatusCode();

                return await response.Content.ReadAsAsync<GroupListResponse>();
            }
        }

        public async Task<Group> GetAsync(long groupId)
        {
            using (_loggerScope(_logger, $"GetAsync({groupId})"))
            using (var client = _apiClient.CreateClient(GroupsResourceUri))
            {
                var response = await client.GetAsync(groupId.ToString()).ConfigureAwait(false);

                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    _logger.LogInformation("GroupResponse {0} not found", groupId);
                    return null;
                }

                response.EnsureSuccessStatusCode();

                var groupResponse = await response.Content.ReadAsAsync<GroupResponse>();
                return groupResponse.Group;
            }
        }

        public async Task<Group> CreateAsync(GroupCreateRequest group)
        {
            using (_loggerScope(_logger, "PostAsync")) // Maybe incluse the request in the log?
            using (var client = _apiClient.CreateClient())
            {
                var response = await client.PostAsJsonAsync(GroupsResourceUri, group).ConfigureAwait(false);

                if (response.StatusCode != System.Net.HttpStatusCode.Created)
                {
                    throw new HttpRequestException(
                        $"Status code retrieved was {response.StatusCode} and not a 201 as expected" +
                        Environment.NewLine +
                        "See: https://developer.zendesk.com/rest_api/docs/core/groups#create-groups");
                }
                
                return await response.Content.ReadAsAsync<Group>();
            }
        }

        public async Task<Group> UpdateAsync(GroupUpdateRequest group)
        {
            using (_loggerScope(_logger, "PutAsync"))
            using (var client = _apiClient.CreateClient(GroupsResourceUri))
            {
                var response = await client.PutAsJsonAsync(group.Id.ToString(), group).ConfigureAwait(false);

                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    _logger.LogInformation("Cannot update group as group {0} cannot be found", group.Id);
                    return null;
                }

                response.EnsureSuccessStatusCode();

                return await response.Content.ReadAsAsync<Group>();
            }
        }

        public async Task DeleteAsync(long groupId)
        {
            using (_loggerScope(_logger, $"DeleteAsync({groupId})"))
            using (var client = _apiClient.CreateClient(GroupsResourceUri))
            {
                var response = await client.DeleteAsync(groupId.ToString()).ConfigureAwait(false);

                if (response.StatusCode != System.Net.HttpStatusCode.NoContent)
                {
                    throw new HttpRequestException(
                        $"Status code retrieved was {response.StatusCode} and not a 204 as expected" +
                        Environment.NewLine +
                        "See: https://developer.zendesk.com/rest_api/docs/core/attachments#delete-upload");
                }
            }
        }
    }
}
