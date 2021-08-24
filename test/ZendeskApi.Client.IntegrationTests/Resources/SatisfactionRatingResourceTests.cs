using System.Threading.Tasks;
using Xunit;
using ZendeskApi.Client.IntegrationTests.Factories;
using ZendeskApi.Client.Models;

namespace ZendeskApi.Client.IntegrationTests.Resources
{
    public class UsersResourceTests : IClassFixture<ZendeskClientFactory>
    {
        private readonly ZendeskClientFactory _clientFactory;

        public UsersResourceTests(
            ZendeskClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }
        
        [Fact]
        public async Task GetAllAsync_WhenCalledWithCursorPagination_ShouldReturnUsers()
        {
            var client = _clientFactory.GetClient();

            var results = await client
                .Users.GetAllAsync(new CursorPager());

            Assert.NotNull(results);
        }

        [Fact]
        public async Task GetAllByGroupIdAsync_WhenCalledWithCursorPagination_ShouldReturnUsers()
        {
            var client = _clientFactory.GetClient();

            var results = await client
                .Users.GetAllByGroupIdAsync(28963465, new CursorPager());

            Assert.NotNull(results);
        }

        [Fact]
        public async Task GetAllByOrganizationIdAsync_WhenCalledWithCursorPagination_ShouldReturnUsers()
        {
            var client = _clientFactory.GetClient();

            var results = await client
                .Users.GetAllByOrganizationIdAsync(360106870118, new CursorPager());

            Assert.NotNull(results);
        }
    }
}