namespace CleanBase.Entities
{
    public class Configuration : EntityRoot
    {
        public string Name { get; set; }
        public List<ConfigurationItem>? ConfigurationItems { get; set; }
    }
}
