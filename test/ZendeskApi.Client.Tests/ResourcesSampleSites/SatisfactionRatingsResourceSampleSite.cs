using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using ZendeskApi.Client.Extensions;
using ZendeskApi.Client.Models;
using ZendeskApi.Client.Responses;
using ZendeskApi.Client.Tests.Extensions;

namespace ZendeskApi.Client.Tests.ResourcesSampleSites
{
    public class SatisfactionRatingsResourceSampleSite : SampleSite
    {
        private class State
        {
            public IDictionary<long, SatisfactionRating> SatisfactionRatings = new Dictionary<long, SatisfactionRating>();
            public IDictionary<long, List<SatisfactionRating>> SatisfactionRatingsByTicket = new Dictionary<long, List<SatisfactionRating>>();
        }

        public static Action<IRouteBuilder> MatchesRequest
        {
            get
            {
                return rb => rb
                    .MapGet("api/v2/satisfaction_ratings/{id}", (req, resp, routeData) =>
                    {
                        var id = long.Parse(routeData.Values["id"].ToString());

                        var state = req.HttpContext.RequestServices.GetRequiredService<State>();

                        if (!state.SatisfactionRatings.ContainsKey(id))
                        {
                            resp.StatusCode = (int)HttpStatusCode.NotFound;
                            return Task.CompletedTask;
                        }

                        var sr = state.SatisfactionRatings.Single(x => x.Key == id).Value;

                        resp.StatusCode = (int)HttpStatusCode.OK;
                        return resp.WriteAsJson(new SatisfactionRatingResponse { SatisfactionRating = sr });
                    })
                    .MapGet("api/v2/satisfaction_ratings", (req, resp, routeData) =>
                    {
                        var state = req.HttpContext.RequestServices.GetRequiredService<State>();

                        resp.StatusCode = (int)HttpStatusCode.OK;
                        return resp.WriteAsJson(new SatisfactionRatingsResponse { SatisfactionRatings = state.SatisfactionRatings.Values });
                    })
                    .MapPost("api/v2/tickets/{ticketId}/satisfaction_rating", (req, resp, routeData) =>
                    {
                        var sr = req.Body.ReadAs<SatisfactionRating>();
                        var ticketId = long.Parse(routeData.Values["ticketId"].ToString());
                        var state = req.HttpContext.RequestServices.GetRequiredService<State>();

                        sr.Id = long.Parse(Rand.Next().ToString());
                        state.SatisfactionRatings.Add(sr.Id.Value, sr);

                        if (state.SatisfactionRatingsByTicket.ContainsKey(ticketId))
                        {
                            state.SatisfactionRatingsByTicket[ticketId].Add(sr);
                        }
                        else {
                            state.SatisfactionRatingsByTicket.Add(ticketId, new List<SatisfactionRating> { sr });
                        }

                        resp.StatusCode = (int)HttpStatusCode.Created;
                        return resp.WriteAsJson(sr);
                    })
                    ;
            }
        }

        private readonly TestServer _server;

        private HttpClient _client;
        public override HttpClient Client => _client;
        
        public SatisfactionRatingsResourceSampleSite(string resource)
        {
            var webhostbuilder = new WebHostBuilder();
            webhostbuilder
                .ConfigureServices(services => {
                    services.AddSingleton<State>((_) => new State());
                    services.AddRouting();
                    services.AddMemoryCache();
                })
                .Configure(app =>
                {

                    app.UseRouter(MatchesRequest);
                });

            _server = new TestServer(webhostbuilder);
            
            RefreshClient(resource);
        }

        public override void RefreshClient(string resource)
        {
            _client = _server.CreateClient();
            _client.BaseAddress = new Uri($"http://localhost/{CreateResource(resource)}");
        }

        private string CreateResource(string resource)
        {
            resource = resource?.Trim('/');

            return resource != null ? resource + "/" : resource;
        }

        public Uri BaseUri => Client.BaseAddress;

        public override void Dispose()
        {
            Client.Dispose();
            _server.Dispose();
        }
    }
}
