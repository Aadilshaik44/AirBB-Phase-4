using System.Linq.Expressions;

namespace AirBB.Models.DataLayer
{
    public class QueryOptions<T>
    {
        public Expression<Func<T, bool>> Where { get; set; } = null!;
        public Expression<Func<T, object>> OrderBy { get; set; } = null!;
        public string OrderByDirection { get; set; } = "asc"; // "asc" or "desc"
        public string[] Includes { get; set; } = Array.Empty<string>();

        public bool HasWhere => Where != null;
        public bool HasOrderBy => OrderBy != null;
    }
}