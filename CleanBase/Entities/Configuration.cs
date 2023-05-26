namespace CleanBase.Entities
{
    public class Configuration : EntityParent
    {
        public string Name { get; set; }
        public List<ConfigurationItem>? ConfigurationItems { get; set; }
    }
}
