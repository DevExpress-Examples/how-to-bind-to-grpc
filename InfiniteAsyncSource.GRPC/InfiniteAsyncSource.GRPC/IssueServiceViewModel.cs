using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Xpf.Data;
using DevExpress.Mvvm.Xpf;
using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using IssuesData;
using Grpc.Core;

namespace InfiniteAsyncSource.GRPC {
    public class IssueServiceViewModel : ViewModelBase {
        public User[] Users {
            get { return GetValue<User[]>(); }
            private set { SetValue(value); }
        }
        Channel channel;
        IssuesService.IssuesServiceClient client;
        public IssueServiceViewModel() {
            channel = new Channel("127.0.0.1:30051", ChannelCredentials.Insecure);
            client = new IssuesService.IssuesServiceClient(channel);
            AssignUsers();
        }

        [Command]
        public void Closed() {
            channel.ShutdownAsync().Wait();
        } 

        async void AssignUsers() {
            if(!IsInDesignMode)
                Users = (await client.GetUsersAsync(new Empty())).Items.ToArray();
        }

        [Command]
        public void FetchIssues(FetchRowsAsyncArgs args) {
            args.Result = GetIssuesAsync(args);
        }
        async Task<FetchRowsResult> GetIssuesAsync(FetchRowsAsyncArgs args) {
            var take = args.Take ?? 30;
            var issues = await client.FetchIssuesAsync(new Fetch { 
                Skip = args.Skip,
                Take = take,
                Sort = GetIssueSortOrder(args.SortOrder),
                Filter = (IssueFilter)args.Filter
            });
            var issuesArray = issues.Items.ToArray();
            return new FetchRowsResult(issuesArray, hasMoreRows: issuesArray.Length == take);
        }
        static IssueSortOrder GetIssueSortOrder(SortDefinition[] sortOrder) {
            if(sortOrder.Length > 0) {
                var sort = sortOrder.Single();
                if(sort.PropertyName == "Created") {
                    if(sort.Direction != ListSortDirection.Descending)
                        throw new InvalidOperationException();
                    return IssueSortOrder.CreatedDescending;
                }
                if(sort.PropertyName == "Votes") {
                    return sort.Direction == ListSortDirection.Ascending
                        ? IssueSortOrder.VotesAscending
                        : IssueSortOrder.VotesDescending;
                }
            }
            return IssueSortOrder.Default;
        }

        [Command]
        public void GetTotalSummaries(GetSummariesAsyncArgs args) {
            args.Result = GetTotalSummariesAsync(args);
        }
        async Task<object[]> GetTotalSummariesAsync(GetSummariesAsyncArgs e) {
            var summaryValues = await client.GetSummariesAsync((IssueFilter)e.Filter ?? new IssueFilter());
            return e.Summaries.Select(x => {
                if(x.SummaryType == SummaryType.Count)
                    return (object)summaryValues.Count;
                if(x.SummaryType == SummaryType.Max && x.PropertyName == "Created")
                    return summaryValues.LastCreated;
                throw new InvalidOperationException();
            }).ToArray();
        }

        [Command]
        public void GetUniqueValues(GetUniqueValuesAsyncArgs args) {
            if(args.PropertyName == "Priority") {
                var values = Enum.GetValues(typeof(Priority)).Cast<object>().ToArray();
                args.Result = Task.FromResult(values);
            } else {
                throw new InvalidOperationException();
            }
        }

        [Command]
        public void UpdateIssue(RowValidationArgs args) {
            args.ResultAsync = UpdateIssueAsync((Issue)args.Row);
        }
        async Task<ValidationErrorInfo> UpdateIssueAsync(Issue issue) {
            await client.UpdateIssueAsync(issue);
            return null;
        }
    }
}
