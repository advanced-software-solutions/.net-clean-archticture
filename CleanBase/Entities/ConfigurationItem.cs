namespace CleanBase.Entities
{
    public class ConfigurationItem : EntityParent
    {
        public string Key { get; set; }
        public string Value { get; set; }
        public int? ConfigurationId { get; set; }
        public Configuration? Configuration { get; set; }
    }
}