namespace CleanBase.Entities
{
    public class ConfigurationItem : EntityRoot
    {
        public string Key { get; set; }
        public string Value { get; set; }
        public int? ConfigurationId { get; set; }
        public Configuration? Configuration { get; set; }
    }
}