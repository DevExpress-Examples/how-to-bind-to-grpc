using IssuesData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IssuesServer {
    public static class IssuesDataProvider {
        #region helpers
        static object SyncObject = new object();
        static Lazy<Issue[]> AllIssues = new Lazy<Issue[]>(() => {
            var date = DateTime.Today;
            var rnd = new Random(0);
            return Enumerable.Range(0, 100000)
                .Select(i => {
                    date = date.AddSeconds(-rnd.Next(20 * 60));
                    return new Issue { 
                        Id = i,
                        Subject = OutlookDataGenerator.GetSubject(),
                        UserId = OutlookDataGenerator.GetFromId(),
                        Created = date,
                        Votes = rnd.Next(100),
                        Priority = OutlookDataGenerator.GetPriority()
                    };
                }).ToArray();
        });
        class DefaultSortComparer : IComparer<Issue> {
            int IComparer<Issue>.Compare(Issue x, Issue y) {
                if(x.Created.Date != y.Created.Date)
                    return Comparer<DateTime>.Default.Compare(x.Created.Date, y.Created.Date);
                return Comparer<int>.Default.Compare(x.Votes, y.Votes);
            }
        } 
        #endregion

        public static Task<Issue[]> GetIssuesAsync(int skip, int take, IssueSortOrder sortOrder, IssueFilter filter) {
            return Task.Run(() => {
                var issues = SortIssues(sortOrder, AllIssues.Value);
                if(filter != null)
                  issues = FilterIssues(filter, issues);
                return issues.Skip(skip).Take(take).Select(x => x.Clone()).ToArray();
            });
        }

        public static Task<User[]> GetUsersAsync() {
            return Task.Run(() => {
                return OutlookDataGenerator.Users
                    .Select((x, i) => {
                      var split = x.Split(' ');
                      return new User { Id = i, FirstName = split[0], LastName = split[1] };
                    })
                    .ToArray();
            });
        }

        public static Task<IssueSummaries> GetSummariesAsync(IssueFilter filter) {
            return Task.Run(() => {
                var issues = (IEnumerable<Issue>)AllIssues.Value;
                if(filter != null)
                    issues = FilterIssues(filter, issues);
                var lastCreated = issues.Any() ? issues.Max(x => x.Created) : default(DateTime?);
                return new IssueSummaries { Count = issues.Count(), LastCreated = lastCreated };
            });
        }

        public static Task UpdateRowAsync(Issue row) {
            return Task.Run(() => {
                if(row == null)
                    return;
                Issue data = AllIssues.Value.FirstOrDefault(x => x.Id == row.Id);
                if(data == null)
                    return;
                data.Priority = row.Priority;
                data.Subject = row.Subject;
                data.UserId = row.UserId;
                data.Votes = row.Votes;
                data.Created = row.Created;
            });
        }

        #region filter
        static IEnumerable<Issue> FilterIssues(IssueFilter filter, IEnumerable<Issue> issues) {
            if(filter.CreatedFrom != null || filter.CreatedTo != null) {
                if(filter.CreatedFrom == null || filter.CreatedTo == null) {
                    throw new InvalidOperationException();
                }
                issues = issues.Where(x => x.Created >= filter.CreatedFrom.Value && x.Created < filter.CreatedTo);
            }
            if(filter.MinVotes != null) {
                issues = issues.Where(x => x.Votes >= filter.MinVotes.Value);
            }
            if(filter.Priority != null) {
                issues = issues.Where(x => x.Priority == filter.Priority);
            }
            return issues;
        }
        #endregion

        #region sort
        static IEnumerable<Issue> SortIssues(IssueSortOrder sortOrder, IEnumerable<Issue> issues) {
            switch(sortOrder) {
            case IssueSortOrder.Default:
                return issues;//.OrderByDescending(x => x, new DefaultSortComparer()).ThenByDescending(x => x.Created);
            case IssueSortOrder.CreatedDescending:
                return issues.OrderByDescending(x => x.Created);
            case IssueSortOrder.VotesAscending:
                return issues.OrderBy(x => x.Votes).ThenByDescending(x => x.Created);
            case IssueSortOrder.VotesDescending:
                return issues.OrderByDescending(x => x.Votes).ThenByDescending(x => x.Created);
            default:
                throw new InvalidOperationException();
            }
        } 
        #endregion
    }
}
