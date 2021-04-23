Imports System
Imports Grpc.Core
Imports IssuesData

Namespace IssuesServer
	Friend Class Program
		Private Const Port As Integer = 30051

		Public Shared Sub Main(ByVal args() As String)
			Dim server As New Server With {
				.Services = { IssuesService.BindService(New IssuesServiceImpl()) },
				.Ports = { New ServerPort("localhost", Port, ServerCredentials.Insecure) }
			}
			server.Start()

			Console.WriteLine("Issues server listening on port " & Port)
			Console.WriteLine("Press any key to stop the server...")
			Console.ReadKey()

			server.ShutdownAsync().Wait()
		End Sub
	End Class
End Namespace
