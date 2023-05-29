namespace CleanBase.Entities
{
    public class CleanConfiguration : EntityRoot
    {
        public string Name { get; set; }
        public List<CleanConfigurationItem>? ConfigurationItems { get; set; }
    }
}
