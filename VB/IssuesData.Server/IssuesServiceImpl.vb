Imports Grpc.Core
Imports IssuesData
Imports System.Threading.Tasks

Namespace IssuesServer
	Friend Class IssuesServiceImpl
		Inherits IssuesService.IssuesServiceBase

		Public Overrides Async Function FetchIssues(ByVal request As Fetch, ByVal context As ServerCallContext) As Task(Of Issues)
			Dim response = New Issues()
			Dim items = Await IssuesDataProvider.GetIssuesAsync(request.Skip, request.Take, request.Sort, request.Filter)
			response.Items.AddRange(items)
			Return response
		End Function
		Public Overrides Async Function GetUsers(ByVal request As Empty, ByVal context As ServerCallContext) As Task(Of Users)
			Dim response = New Users()
			Dim items = Await IssuesDataProvider.GetUsersAsync()
			response.Items.AddRange(items)
			Return response
		End Function
		Public Overrides Async Function GetSummaries(ByVal request As IssueFilter, ByVal context As ServerCallContext) As Task(Of IssueSummaries)
			Return Await IssuesDataProvider.GetSummariesAsync(request)
		End Function
		Public Overrides Async Function UpdateIssue(ByVal request As Issue, ByVal context As ServerCallContext) As Task(Of Empty)
			Await IssuesDataProvider.UpdateRowAsync(request)
			Return New Empty()
		End Function
	End Class
End Namespace
