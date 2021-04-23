Imports DevExpress.Mvvm
Imports DevExpress.Mvvm.DataAnnotations
Imports DevExpress.Xpf.Data
Imports DevExpress.Mvvm.Xpf
Imports System
Imports System.ComponentModel
Imports System.Linq
Imports System.Threading.Tasks
Imports IssuesData
Imports Grpc.Core

Namespace InfiniteAsyncSource.GRPC
	Public Class IssueServiceViewModel
		Inherits ViewModelBase

		Public Property Users() As User()
			Get
				Return GetValue(Of User())()
			End Get
			Private Set(ByVal value As User())
				SetValue(value)
			End Set
		End Property
		Private channel As Channel
		Private client As IssuesService.IssuesServiceClient
		Public Sub New()
			channel = New Channel("127.0.0.1:30051", ChannelCredentials.Insecure)
			client = New IssuesService.IssuesServiceClient(channel)
			AssignUsers()
		End Sub

		<Command>
		Public Sub Closed()
			channel.ShutdownAsync().Wait()
		End Sub

		Private Async Sub AssignUsers()
			If Not IsInDesignMode Then
				Try
					Users = (Await client.GetUsersAsync(New Empty())).Items.ToArray()
				Catch
				End Try
			End If
		End Sub

		<Command>
		Public Sub FetchIssues(ByVal args As FetchRowsAsyncArgs)
			args.Result = GetIssuesAsync(args)
		End Sub
		Private Async Function GetIssuesAsync(ByVal args As FetchRowsAsyncArgs) As Task(Of FetchRowsResult)
			Dim take = If(args.Take, 30)
			Dim issues = Await client.FetchIssuesAsync(New Fetch With {
				.Skip = args.Skip,
				.Take = take,
				.Sort = GetIssueSortOrder(args.SortOrder),
				.Filter = CType(args.Filter, IssueFilter)
			})
			Dim issuesArray = issues.Items.ToArray()
			Return New FetchRowsResult(issuesArray, hasMoreRows:= issuesArray.Length = take)
		End Function
		Private Shared Function GetIssueSortOrder(ByVal sortOrder() As SortDefinition) As IssueSortOrder
			If sortOrder.Length > 0 Then
				Dim sort = sortOrder.Single()
				If sort.PropertyName = "Created" Then
					If sort.Direction <> ListSortDirection.Descending Then
						Throw New InvalidOperationException()
					End If
					Return IssueSortOrder.CreatedDescending
				End If
				If sort.PropertyName = "Votes" Then
					Return If(sort.Direction = ListSortDirection.Ascending, IssueSortOrder.VotesAscending, IssueSortOrder.VotesDescending)
				End If
			End If
			Return IssueSortOrder.Default
		End Function

		<Command>
		Public Sub GetTotalSummaries(ByVal args As GetSummariesAsyncArgs)
			args.Result = GetTotalSummariesAsync(args)
		End Sub
		Private Async Function GetTotalSummariesAsync(ByVal e As GetSummariesAsyncArgs) As Task(Of Object())
			Dim summaryValues = Await client.GetSummariesAsync(If(CType(e.Filter, IssueFilter), New IssueFilter()))
			Return e.Summaries.Select(Function(x)
				If x.SummaryType = SummaryType.Count Then
					Return DirectCast(summaryValues.Count, Object)
				End If
				If x.SummaryType = SummaryType.Max AndAlso x.PropertyName = "Created" Then
					Return summaryValues.LastCreated
				End If
				Throw New InvalidOperationException()
			End Function).ToArray()
		End Function

		<Command>
		Public Sub GetUniqueValues(ByVal args As GetUniqueValuesAsyncArgs)
			If args.PropertyName = "Priority" Then
				Dim values = System.Enum.GetValues(GetType(Priority)).Cast(Of Object)().ToArray()
				args.Result = Task.FromResult(values)
			Else
				Throw New InvalidOperationException()
			End If
		End Sub

		<Command>
		Public Sub UpdateIssue(ByVal args As RowValidationArgs)
			args.ResultAsync = UpdateIssueAsync(CType(args.Item, Issue))
		End Sub
		Private Async Function UpdateIssueAsync(ByVal issue As Issue) As Task(Of ValidationErrorInfo)
			Await client.UpdateIssueAsync(issue)
			Return Nothing
		End Function
	End Class
End Namespace
