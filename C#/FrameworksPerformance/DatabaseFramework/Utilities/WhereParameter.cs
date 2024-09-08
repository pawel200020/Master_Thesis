using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using DatabaseFramework.Models;
using DatabaseFramework.SqlParams;
using MatchType = DatabaseFramework.SqlParams.MatchType;

namespace DatabaseFramework.Utilities
{
    public class WhereParameter
    {
        private List<Tuple<List<string>, JoinType>> _filters = new();
        private List<JoinType> _joins = new();

        public WhereParameter()
        {
            PrepareParameter(new ReadOnlyDictionary<(string className, string fieldName), (MatchType matchType, string? value)>(new Dictionary<(string className, string fieldName), (MatchType matchType, string? value)>()), JoinType.And);
        }

        public WhereParameter(IReadOnlyDictionary<(string className, string fieldName), (MatchType matchType, string? value)> parameters, JoinType join)
        {
            var param = parameters ?? throw new ArgumentNullException(nameof(parameters));
            PrepareParameter(param, join);
        }

        private void PrepareParameter(IReadOnlyDictionary<(string className, string fieldName), (MatchType matchType, string? value)> parameters, JoinType join)
        {
            if(!parameters.Any())
                return;

            var filters = new List<string>();
            foreach (var ((className,fieldName), (matchType, value)) in parameters)
            {
                var prefix = className switch
                {
                    nameof(Order) => GlobalConstants.OrdersPrefix,
                    nameof(Client) => GlobalConstants.ClientPrefix,
                    nameof(Product) => GlobalConstants.ProductsPrefix,
                    nameof(Employee) => GlobalConstants.EmployeePrefix,
                    nameof(Position) => GlobalConstants.PositionPrefix,
                    nameof(Store) => GlobalConstants.StoresPrefix,
                    _ => throw new InvalidEnumArgumentException("unknown class")
                };

                filters.Add(matchType switch
                {
                    MatchType.StartWith => $"{prefix}.{fieldName} LIKE '%{value?.Replace("\'","\'\'")}'",
                    MatchType.Contains => $"{prefix}.{fieldName} LIKE '%{value?.Replace("\'", "\'\'")}%'",
                    MatchType.EndsWith => $"{prefix}.{fieldName} LIKE '{value?.Replace("\'", "\'\'")}%'",
                    MatchType.Exact => $"{prefix}.{fieldName} = '{value?.Replace("\'", "\'\'")}'",
                    MatchType.Number => $"{prefix}.{fieldName} = {value}",
                    MatchType.IsNull => $"{prefix}.{fieldName} is null",
                    _ => throw new ArgumentOutOfRangeException()
                });
            }
            _filters.Add(new Tuple<List<string>, JoinType>(filters, join));
        }

        public override string ToString()
        {
            if (!_filters.Any())
                return "";

            List<string> mergedFilters = new();
            foreach (var (filters, join) in _filters)
                mergedFilters.Add($"({string.Join($" {join} ", filters)})");

            if (mergedFilters.Count < 2)
                return mergedFilters.First();

            StringBuilder sb = new();
            var joinFilter = mergedFilters.First();
            for (int i = 1; i < mergedFilters.Count; i++)
            {
                sb.Append($"{joinFilter} {_joins[i - 1]} {mergedFilters[i]}");
            }

            return sb.ToString();
        }

        public void AddParameters(IReadOnlyDictionary<(string className, string fieldName), (MatchType matchType, string? value)> parameters, JoinType joinInternal, JoinType joinExternal)
        {
            PrepareParameter(parameters, joinInternal);
            _joins.Add(joinExternal);
        }

    }
}
