using CleanBase.Dtos;
using CleanBase.Entities;
using System.Text.Json.Serialization;

namespace CleanBase
{
    [JsonSerializable(typeof(Guid))]
    [JsonSerializable(typeof(EntityRoot))]
    [JsonSerializable(typeof(EntityResult<>))]
    [JsonSerializable(typeof(EntityResult<TodoList>))]
    [JsonSerializable(typeof(EntityResult<TodoItem>))]
    [JsonSerializable(typeof(EntityResult<List<TodoList>>))]
    [JsonSerializable(typeof(EntityResult<List<TodoItem>>))]
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
