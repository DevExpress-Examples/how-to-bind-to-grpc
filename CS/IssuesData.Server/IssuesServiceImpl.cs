using Grpc.Core;
using IssuesData;
using System.Threading.Tasks;

namespace IssuesServer {
    class IssuesServiceImpl : IssuesService.IssuesServiceBase {
        public override async Task<Issues> FetchIssues(Fetch request, ServerCallContext context) {
            var response = new Issues();
            var items = await IssuesDataProvider.GetIssuesAsync(request.Skip, request.Take, request.Sort, request.Filter);
            response.Items.AddRange(items);
            return response;
        }
        public override async Task<Users> GetUsers(Empty request, ServerCallContext context) {
            var response = new Users();
            var items = await IssuesDataProvider.GetUsersAsync();
            response.Items.AddRange(items);
            return response;
        }
        public override async Task<IssueSummaries> GetSummaries(IssueFilter request, ServerCallContext context) {
            return await IssuesDataProvider.GetSummariesAsync(request);
        }
        public override async Task<Empty> UpdateIssue(Issue request, ServerCallContext context) {
            await IssuesDataProvider.UpdateRowAsync(request);
            return new Empty();
        }
    }
}
