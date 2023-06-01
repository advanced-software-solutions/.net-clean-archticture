namespace CleanBase.Entities
{
    public class CleanConfigurationItem : EntityRoot
    {
        public string Key { get; set; }
        public string Value { get; set; }
        public int? CleanConfigurationId { get; set; }
        public CleanConfiguration? Configuration { get; set; }
    }
}