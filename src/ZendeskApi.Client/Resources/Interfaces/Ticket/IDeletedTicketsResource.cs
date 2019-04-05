using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ZendeskApi.Client.Models;
using ZendeskApi.Client.Queries;
using ZendeskApi.Client.Responses;

namespace ZendeskApi.Client.Resources
{
    public interface IDeletedTicketsResource
    {
        Task<DeletedTicketsListResponse> ListAsync(PagerParameters pager = null);
        Task RestoreAsync(long ticketId);
        Task RestoreAsync(IEnumerable<long> ticketIds);
        Task<JobStatusResponse> PurgeAsync(long ticketId);
        Task<JobStatusResponse> PurgeAsync(IEnumerable<long> ticketIds);
        Task<DeletedTicketsListResponse> ListAsync(Action<IZendeskQuery> builder, PagerParameters pager = null);
    }
}