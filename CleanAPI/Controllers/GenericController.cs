using CleanBase.Extensions;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace CleanAPI.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class GenericController(IConfiguration configuration) : ControllerBase
{
    [HttpPost]
    public IActionResult Get(QueryGeneric query)
    {
        CheckForSqlInjection check = new();
        check.IsSafe(query);
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
        CheckForSqlInjection check = new();
        check.IsSafe(insert);
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
        CheckForSqlInjection check = new();
        check.IsSafe(update);
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

    [HttpPost]
    public IActionResult Delete(DeleteGeneric query)
    {
        CheckForSqlInjection check = new();
        check.IsSafe(query);
        using (SqlConnection sql = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            string queryString = $"DELETE from {query.Table}";
            queryString += $"\r\nWHERE {string.Join(" and ", query.Data.Select(r => $"[{r.Key.Replace(" ", "").Replace("-", "")}]={r.Value}").ToArray())}";
            var result = sql.Execute(queryString);
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
public class DeleteGeneric
{
    public string Table { get; set; }
    public Dictionary<string, string> Data { get; set; } = new();
}
public class CheckForSqlInjection
{
    public List<string> NoPass = new List<string>()
    {
        "delete",
        "drop",
        "insert",
        "create"
    };


    public void IsSafe(QueryGeneric parameter)
    {
        foreach (var item in NoPass)
        {
            if (MatchExact(parameter.Table, item))
                throw new Exception("Not safe sql");
            if (parameter.Where != null && MatchExact(parameter.Where, item))
                throw new Exception("Not safe sql");
            if (!string.IsNullOrEmpty(parameter.OrderBy) && MatchExact(parameter.OrderBy, item))
                throw new Exception("Not safe sql");
            if (parameter.Columns?.Length > 0)
            {
                for (int i = 0; i < parameter.Columns.Length; i++)
                {
                    if (MatchExact(parameter.Columns[i], item))
                        throw new Exception("Not safe sql");
                }
            }
        }
    }

    public void IsSafe(InsertGeneric parameter)
    {
        foreach (var item in NoPass)
        {
            if (MatchExact(parameter.Table, item))
                throw new Exception("Not safe sql");
        }
    }
    public void IsSafe(UpdateGeneric parameter)
    {
        foreach (var item in NoPass)
        {
            if (MatchExact(parameter.Table, item))
                throw new Exception("Not safe sql");
        }
    }
    public void IsSafe(DeleteGeneric parameter)
    {
        foreach (var item in NoPass)
        {
            if (MatchExact(parameter.Table, item))
                throw new Exception("Not safe sql");
        }
    }
    public bool MatchExact(string word, string match)
    {
        return Regex.IsMatch(word.Trim().ToLower(), @$"\b{match}\b");
    }

}
