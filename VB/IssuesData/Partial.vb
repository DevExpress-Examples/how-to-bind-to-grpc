Imports Google.Protobuf.WellKnownTypes
Imports System

Namespace IssuesData
	Partial Friend Class Issue
		Public Property Created() As DateTime
			Get
				Return CreatedProto.ToDateTime()
			End Get
			Set(ByVal value As DateTime)
				CreatedProto = Timestamp.FromDateTime(DateTime.SpecifyKind(value, DateTimeKind.Utc))
			End Set
		End Property
	End Class

	Partial Friend Class User
		Public ReadOnly Property FullName() As String
			Get
				Return FirstName & " " & LastName
			End Get
		End Property
	End Class

	Partial Friend Class IssueFilter
		Public Property CreatedFrom() As DateTime?
			Get
				Return CreatedFromProto?.ToDateTime()
			End Get
			Set(ByVal value? As DateTime)
				CreatedFromProto = If(value IsNot Nothing, Timestamp.FromDateTime(DateTime.SpecifyKind(value.Value, DateTimeKind.Utc)), Nothing)
			End Set
		End Property
		Public Property CreatedTo() As DateTime?
			Get
				Return CreatedToProto?.ToDateTime()
			End Get
			Set(ByVal value? As DateTime)
				CreatedToProto = If(value IsNot Nothing, Timestamp.FromDateTime(DateTime.SpecifyKind(value.Value, DateTimeKind.Utc)), Nothing)
			End Set
		End Property
		Public Property Priority() As Priority?
			Get
				Return If(PriorityProto IsNot Nothing, CType(PriorityProto.Value, Priority), CType(Nothing, Priority?))
			End Get
			Set(ByVal value? As Priority)
				PriorityProto = If(value IsNot Nothing, CInt(value.Value), CType(Nothing, Integer?))
			End Set
		End Property
	End Class

	Partial Friend Class IssueSummaries
		Public Property LastCreated() As DateTime?
			Get
				Return LastCreatedProto?.ToDateTime()
			End Get
			Set(ByVal value? As DateTime)
				LastCreatedProto = If(value IsNot Nothing, Timestamp.FromDateTime(DateTime.SpecifyKind(value.Value, DateTimeKind.Utc)), Nothing)
			End Set
		End Property
	End Class
End Namespace
