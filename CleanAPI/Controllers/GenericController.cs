using CleanBase.Extensions;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace CleanAPI.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
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
                    foreach (var column in query.Columns)
                    {
                        newArray.Add($"[{column}]");
                    }
                    var columns = string.Join(",", newArray.ToArray());
                    queryString = queryString.Replace("*", columns);
                }
                if (!string.IsNullOrEmpty(query.Where))
                {
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

        [HttpPost]
        public IActionResult Insert(InsertGeneric insert)
        {
            string query = $"INSERT INTO {insert.Table} \n\r(";
            query += $"" + string.Join(',', insert.Data.Select(r => $"[{r.Key.Replace(" ", "").Replace("-", "")}]").ToArray()) + ")\n\r";
            query += $"VALUES \n\r(" + string.Join(',', insert.Data.Select(r => $"@{r.Key}").ToArray()) + ")";
            using SqlConnection sql = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
            Dictionary<string, object> parametersQuery = new Dictionary<string, object>();
            foreach (var parameter in insert.Data)
            {
                parametersQuery.Add(parameter.Key, parameter.Value);
            }
            var effectedRows = sql.Execute(query, new DynamicParameters(parametersQuery));
            return Ok(effectedRows);
        }

        [HttpPut]
        public IActionResult Update(UpdateGeneric update) 
        {
            string query = $"UPDATE {update.Table} \n\r";
            query += $"SET " + update.Data.ToSqlParamters() + "\n\r";
            if (update.Values?.Count > 0)
            {
                query += "WHERE " + update.Values.ToSqlParamters();
            }
            using SqlConnection sql = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
            Dictionary<string, object> parametersQuery = new Dictionary<string, object>();
            foreach (var parameter in update.Data)
            {
                parametersQuery.Add(parameter.Key, parameter.Value);
            }
            foreach (var parameter in update.Values)
            {
                parametersQuery.Add(parameter.Key, parameter.Value);
            }

            var effectedRows = sql.Execute(query, new DynamicParameters(parametersQuery));
            return Ok(effectedRows);
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
    public class InsertGeneric
    {
        public string Table { get; set; }
        public Dictionary<string, string> Data { get; set; } = new();
    } 
    public class UpdateGeneric
    {
        public string Table { get; set; }
        public Dictionary<string, string> Data { get; set; } = new();
        public Dictionary<string, string> Values { get; set; } = new();
    }
}
