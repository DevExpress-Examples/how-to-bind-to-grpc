Imports IssuesData
Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Threading.Tasks

Namespace IssuesServer
	Public Module IssuesDataProvider
		#Region "helpers"
		Private SyncObject As New Object()
		Private AllIssues As New Lazy(Of Issue())(Function()
			Dim [date] = DateTime.Today
			Dim rnd = New Random(0)
			Return Enumerable.Range(0, 100000).Select(Function(i)
				[date] = [date].AddSeconds(-rnd.Next(20 * 60))
				Return New Issue With {
					.Id = i,
					.Subject = OutlookDataGenerator.GetSubject(),
					.UserId = OutlookDataGenerator.GetFromId(),
					.Created = [date],
					.Votes = rnd.Next(100),
					.Priority = OutlookDataGenerator.GetPriority()
				}
			End Function).ToArray()
		End Function)
		Private Class DefaultSortComparer
			Implements IComparer(Of Issue)

			Private Function IComparerGeneric_Compare(ByVal x As Issue, ByVal y As Issue) As Integer Implements IComparer(Of Issue).Compare
				If x.Created.Date <> y.Created.Date Then
					Return Comparer(Of DateTime).Default.Compare(x.Created.Date, y.Created.Date)
				End If
				Return Comparer(Of Integer).Default.Compare(x.Votes, y.Votes)
			End Function
		End Class
		#End Region

		Public Function GetIssuesAsync(ByVal skip As Integer, ByVal take As Integer, ByVal sortOrder As IssueSortOrder, ByVal filter As IssueFilter) As Task(Of Issue())
			Return Task.Run(Function()
				Dim issues = SortIssues(sortOrder, AllIssues.Value)
				If filter IsNot Nothing Then
					issues = FilterIssues(filter, issues)
				End If
				Return issues.Skip(skip).Take(take).Select(Function(x) x.Clone()).ToArray()
			End Function)
		End Function

		Public Function GetUsersAsync() As Task(Of User())
			Return Task.Run(Function()
				Return OutlookDataGenerator.Users.Select(Function(x, i)
					Dim split = x.Split(" "c)
					Return New User With {
						.Id = i,
						.FirstName = split(0),
						.LastName = split(1)
					}
				End Function).ToArray()
			End Function)
		End Function

		Public Function GetSummariesAsync(ByVal filter As IssueFilter) As Task(Of IssueSummaries)
			Return Task.Run(Function()
				Dim issues = DirectCast(AllIssues.Value, IEnumerable(Of Issue))
				If filter IsNot Nothing Then
					issues = FilterIssues(filter, issues)
				End If
				Dim lastCreated = If(issues.Any(), issues.Max(Function(x) x.Created), CType(Nothing, DateTime?))
				Return New IssueSummaries With {
					.Count = issues.Count(),
					.LastCreated = lastCreated
				}
			End Function)
		End Function

		Public Function UpdateRowAsync(ByVal row As Issue) As Task
			Return Task.Run(Sub()
				If row Is Nothing Then
					Return
				End If
				Dim data As Issue = AllIssues.Value.FirstOrDefault(Function(x) x.Id = row.Id)
				If data Is Nothing Then
					Return
				End If
				data.Priority = row.Priority
				data.Subject = row.Subject
				data.UserId = row.UserId
				data.Votes = row.Votes
				data.Created = row.Created
			End Sub)
		End Function

		#Region "filter"
		Private Function FilterIssues(ByVal filter As IssueFilter, ByVal issues As IEnumerable(Of Issue)) As IEnumerable(Of Issue)
			If filter.CreatedFrom IsNot Nothing OrElse filter.CreatedTo IsNot Nothing Then
				If filter.CreatedFrom Is Nothing OrElse filter.CreatedTo Is Nothing Then
					Throw New InvalidOperationException()
				End If
'INSTANT VB WARNING: Comparisons involving nullable type instances require Option Strict Off:
				issues = issues.Where(Function(x) x.Created >= filter.CreatedFrom.Value AndAlso x.Created < filter.CreatedTo)
			End If
			If filter.MinVotes IsNot Nothing Then
				issues = issues.Where(Function(x) x.Votes >= filter.MinVotes.Value)
			End If
			If filter.Priority IsNot Nothing Then
				issues = issues.Where(Function(x) filter.Priority.Equals(x.Priority))
			End If
			Return issues
		End Function
		#End Region

		#Region "sort"
		Private Function SortIssues(ByVal sortOrder As IssueSortOrder, ByVal issues As IEnumerable(Of Issue)) As IEnumerable(Of Issue)
			Select Case sortOrder
			Case IssueSortOrder.Default
				Return issues '.OrderByDescending(x => x, new DefaultSortComparer()).ThenByDescending(x => x.Created);
			Case IssueSortOrder.CreatedDescending
				Return issues.OrderByDescending(Function(x) x.Created)
			Case IssueSortOrder.VotesAscending
				Return issues.OrderBy(Function(x) x.Votes).ThenByDescending(Function(x) x.Created)
			Case IssueSortOrder.VotesDescending
				Return issues.OrderByDescending(Function(x) x.Votes).ThenByDescending(Function(x) x.Created)
			Case Else
				Throw New InvalidOperationException()
			End Select
		End Function
		#End Region
	End Module
End Namespace
