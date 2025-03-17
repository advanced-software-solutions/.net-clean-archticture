using CleanBase.Entities;
using System.Text.Json.Serialization;

namespace CleanBase
{
    [JsonSerializable(typeof(EntityRoot))]
    [JsonSerializable(typeof(TodoItem))]
    [JsonSerializable(typeof(TodoList))]
    [JsonSerializable(typeof(CleanConfiguration))]
    [JsonSerializable(typeof(CleanConfigurationItem))]
    [JsonSerializable(typeof(List<EntityRoot>))]
    [JsonSerializable(typeof(List<TodoItem>))]
    [JsonSerializable(typeof(List<TodoList>))]
    [JsonSerializable(typeof(List<CleanConfiguration>))]
    [JsonSerializable(typeof(List<CleanConfigurationItem>))]
    public partial class AppJsonSerializerContext : JsonSerializerContext
    {
    }
}
