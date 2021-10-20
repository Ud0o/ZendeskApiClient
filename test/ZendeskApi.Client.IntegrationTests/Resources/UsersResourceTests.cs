using System.Threading.Tasks;
using Xunit;
using ZendeskApi.Client.IntegrationTests.Factories;
using ZendeskApi.Client.Models;

namespace ZendeskApi.Client.IntegrationTests.Resources
{
    public class SatisfactionRatingResourceTests : IClassFixture<ZendeskClientFactory>
    {
        private readonly ZendeskClientFactory _clientFactory;

        public SatisfactionRatingResourceTests(
            ZendeskClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }
        
        [Fact]
        public async Task GetAllAsync_WhenCalledWithCursorPagination_ShouldReturnSatisfactionRatings()
        {
            var client = _clientFactory.GetClient();

            var results = await client
                .SatisfactionRatings.GetAllAsync(new CursorPager());

            Assert.NotNull(results);
        }
    }
}