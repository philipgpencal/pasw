using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Newtonsoft.Json;
using PASW.Models;
using Xunit;

namespace PASW.Test
{
    public class DiffControllerTest
    {
        public DiffControllerTest()
        {
            _server = new TestServer(new WebHostBuilder().UseStartup<Startup>());
            _client = _server.CreateClient();
        }

        private readonly TestServer _server;
        private readonly HttpClient _client;

        [Fact]
        public async Task Diff_SHOULD_Return_Bad_Request_WHEN_Request_is_Incomplete_missing_left()
        {
            var rightBase64 = "IHsgIm5hbWUiOiJKb2huIiwgImFnZSI6MzAsICJjYXIiOm51bGwgfQ==";

            var response = await _client.PostAsync($"/v1/diff/2/right?data={rightBase64}", null);
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            response = await _client.GetAsync("/v1/diff/2");
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Diff_SHOULD_Return_Bad_Request_WHEN_Request_is_Incomplete_missing_right()
        {
            var leftBase64 = "eyAibmFtZSI6IkpvaG4iLCAiYWdlIjozMCwgImNhciI6bnVsbCB9";

            var response = await _client.PostAsync($"/v1/diff/1/left?data={leftBase64}", null);
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            response = await _client.GetAsync("/v1/diff/1");
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Diff_SHOULD_Return_Bad_Request_WHEN_Request_not_exists()
        {
            var response = await _client.GetAsync("/v1/diff/10");

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Diff_SHOULD_Return_Ok_and_equal()
        {
            var jsonBase64 = "IHsgIm5hbWUiOiJKb2huIiwgImFnZSI6MzAsICJjYXIiOm51bGwgfQ==";

            var response = await _client.PostAsync($"/v1/diff/3/left?data={jsonBase64}", null);
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            response = await _client.PostAsync($"/v1/diff/3/right?data={jsonBase64}", null);
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            response = await _client.GetAsync("/v1/diff/3");
            var result = JsonConvert.DeserializeObject<DiffResultModel>(await response.Content.ReadAsStringAsync());

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            result.Equal.Should().BeTrue();
        }

        [Fact]
        public async Task Diff_SHOULD_Return_Ok_different_size()
        {
            var johnBase64 = "\"IHsgIm5hbWUiOiJKb2huIiwgImFnZSI6MzAsICJjYXIiOm51bGwgfQ==\"";
            var peterJacksonBase64 = "\"IHsgIm5hbWUiOiJQZXRlciBKYWNrc29uIiwgImFnZSI6MzAsICJjYXIiOm51bGwgfQ==\"";

            var response = await _client.PostAsync($"/v1/diff/5/left?data={johnBase64}", null);
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            response = await _client.PostAsync($"/v1/diff/5/right?data={peterJacksonBase64}", null);
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            response = await _client.GetAsync("/v1/diff/5");
            var result = JsonConvert.DeserializeObject<DiffResultModel>(await response.Content.ReadAsStringAsync());

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            result.Equal.Should().BeFalse();
            result.SameSize.Should().BeFalse();
            result.DiffInsights.Count.Should().Be(0);
        }

        [Fact]
        public async Task Diff_SHOULD_Return_Ok_same_size_and_1_insight()
        {
            var johnBase64 = "IHsgIm5hbWUiOiJKb2huIiwgImFnZSI6MzAsICJjYXIiOm51bGwgfQ==";
            var peteBase64 = "IHsgIm5hbWUiOiJQZXRlIiwgImFnZSI6MzAsICJjYXIiOm51bGwgfQ==";

            var response = await _client.PostAsync($"/v1/diff/4/left?data={johnBase64}", null);
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            response = await _client.PostAsync($"/v1/diff/4/right?data={peteBase64}", null);
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            response = await _client.GetAsync("/v1/diff/4");
            var result = JsonConvert.DeserializeObject<DiffResultModel>(await response.Content.ReadAsStringAsync());

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            result.Equal.Should().BeFalse();
            result.SameSize.Should().BeTrue();
            result.DiffInsights.Count.Should().Be(1);
        }

        [Fact]
        public async Task PostDiffEntry_SHOULD_Return_Ok_left()
        {
            var leftBase64 = "eyAibmFtZSI6IkpvaG4iLCAiYWdlIjozMCwgImNhciI6bnVsbCB9";

            var response = await _client.PostAsync($"/v1/diff/6/left?data={leftBase64}", null);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task PostDiffEntry_SHOULD_Return_Ok_right()
        {
            var rightBase64 = "eyAibmFtZSI6IkpvaG4iLCAiYWdlIjozMCwgImNhciI6bnVsbCB9";

            var response = await _client.PostAsync($"/v1/diff/6/right?data={rightBase64}", null);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
    }
}
