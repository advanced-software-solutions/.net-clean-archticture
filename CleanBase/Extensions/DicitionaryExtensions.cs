
namespace CleanBase.Extensions;

public static class DicitionaryExtensions
{
    public static string ToSqlParamters(this Dictionary<string, string> dic)
    {
        return string.Join(',', dic.Select(r => $"[{r.Key}]=@{r.Key.Replace(" ", "").Replace("-", "")}\n\r").ToArray());
    }
}
