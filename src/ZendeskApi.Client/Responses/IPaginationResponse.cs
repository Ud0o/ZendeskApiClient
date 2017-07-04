using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using ZendeskApi.Client.Models;

namespace ZendeskApi.Client.Responses
{
    public interface IPagination<T> : IEnumerable<T>
    {
        int Count { get; set; }
        Uri NextPage { get; set; }
        Uri PreviousPage { get; set; }
    }

    public abstract class PaginationResponse<T> : IPagination<T>
    {
        protected abstract IEnumerable<T> Enumerable { get; }

        [JsonProperty("count")]
        public int Count { get; set; } = 0;

        [JsonProperty("next_page")]
        public Uri NextPage { get; set; } = null;

        [JsonProperty("previous_page")]
        public Uri PreviousPage { get; set; } = null;

        [JsonIgnore]
        public Pager Pager
        {
            get {
                if (NextPage == null)
                {
                    return new Pager(null, Count, 100);
                }

                var next = Microsoft.AspNetCore.WebUtilities.QueryHelpers.ParseQuery(NextPage.Query);
                var page = int.Parse(next["page"]);

                return new Pager(new PagerParameters { Page = page, PageSize = Count }, 100);
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return Enumerable.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Enumerable.GetEnumerator();
        }
    }
}
