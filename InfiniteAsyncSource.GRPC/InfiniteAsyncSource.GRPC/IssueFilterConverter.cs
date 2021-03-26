using DevExpress.Data.Filtering;
using DevExpress.Xpf.Data;
using IssuesData;
using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using System.Windows.Markup;

namespace InfiniteAsyncSource.GRPC {
    public class IssueFilterConverter : MarkupExtension, IValueConverter {
        object IValueConverter.Convert(object filter, Type targetType, object parameter, CultureInfo culture) {
            return ((CriteriaOperator)filter).Match(
                binary: (propertyName, value, type) => {
                    if(propertyName == "Votes" && type == BinaryOperatorType.GreaterOrEqual)
                        return new IssueFilter { MinVotes = (int)value };

                    if(propertyName == "Priority" && type == BinaryOperatorType.Equal)
                        return new IssueFilter { Priority = (Priority)value };

                    if(propertyName == "Created") {
                        if(type == BinaryOperatorType.GreaterOrEqual)
                            return new IssueFilter { CreatedFrom = (DateTime)value };
                        if(type == BinaryOperatorType.Less)
                            return new IssueFilter { CreatedTo = (DateTime)value };
                    }

                    throw new InvalidOperationException();
                },
                and: filters => {
                    return new IssueFilter {
                        CreatedFrom = filters.Select(x => x.CreatedFrom).SingleOrDefault(x => x != null),
                        CreatedTo = filters.Select(x => x.CreatedTo).SingleOrDefault(x => x != null),
                        MinVotes = filters.Select(x => x.MinVotes).SingleOrDefault(x => x != null),
                        Priority = filters.Select(x => x.Priority).SingleOrDefault(x => x != null)
                    };
                },
                @null: default(IssueFilter)
            );
        }
        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
        public override object ProvideValue(IServiceProvider serviceProvider) => this;
    }
}
