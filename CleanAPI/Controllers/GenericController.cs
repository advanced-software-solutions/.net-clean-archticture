using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace CleanAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GenericController(IConfiguration configuration) : ControllerBase
    {
        /// <summary>
        /// PLEASE BE CAREFUL WHEN USING THIS
        /// it's an initial work, use it at your own risk
        /// </summary>
        [HttpPost]
        public IActionResult Get(QueryGeneric query)
        {
            using (SqlConnection sql = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                string queryString = $"SELECT * from {query.Table}";
                if (query.Columns?.Length > 0)
                {
                    var newArray = new List<string>();
                    foreach (var column in query.Columns) {
                        newArray.Add($"[{column}]");
                    }
                    var columns = string.Join(",", newArray.ToArray());
                    queryString = queryString.Replace("*", columns);
                }
                if (!string.IsNullOrEmpty(query.Where)) {
                    queryString += $"\r\nWHERE {query.Where}";
                }
                if (query.Page > 0)
                {
                    queryString += $"\r\nORDER BY {query.OrderBy}\r\nOFFSET {(query.Page - 1) * query.PageSize} ROWS\r\nFETCH NEXT {query.PageSize} ROWS ONLY";
                }
                var result = sql.Query<object>(queryString).ToList();
                return new JsonResult(result);
            }
        }
    }

    public class QueryGeneric
    {
        public string Table { get; set; }
        public string[]? Columns { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public string OrderBy { get; set; } = "Id";
        public string? Where { get; set; }
    }
}
