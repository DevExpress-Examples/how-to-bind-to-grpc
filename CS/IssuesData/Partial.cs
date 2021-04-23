using Google.Protobuf.WellKnownTypes;
using System;

namespace IssuesData {
    partial class Issue {
        public DateTime Created {
            get => CreatedProto.ToDateTime();
            set => CreatedProto = Timestamp.FromDateTime(DateTime.SpecifyKind(value, DateTimeKind.Utc));
        }
    }

    partial class User {
        public string FullName => FirstName + " " + LastName;
    }

    partial class IssueFilter {
        public DateTime? CreatedFrom {
            get => CreatedFromProto?.ToDateTime();
            set => CreatedFromProto = value != null ? Timestamp.FromDateTime(DateTime.SpecifyKind(value.Value, DateTimeKind.Utc)) : null;
        }
        public DateTime? CreatedTo {
            get => CreatedToProto?.ToDateTime();
            set => CreatedToProto = value != null ? Timestamp.FromDateTime(DateTime.SpecifyKind(value.Value, DateTimeKind.Utc)) : null;
        }
        public Priority? Priority {
            get => PriorityProto != null ? (Priority)PriorityProto.Value : default(Priority?);
            set => PriorityProto = value != null ? (int)value.Value : default(int?);
        }
    }

    partial class IssueSummaries {
        public DateTime? LastCreated {
            get => LastCreatedProto?.ToDateTime();
            set => LastCreatedProto = value != null ? Timestamp.FromDateTime(DateTime.SpecifyKind(value.Value, DateTimeKind.Utc)) : null;
        }
    }
}
