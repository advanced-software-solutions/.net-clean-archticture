namespace CleanBase.Entities;

public partial class CleanConfigurationItem : EntityRoot
{
    public string Key { get; set; }
    public string Value { get; set; }
    public Guid CleanConfigurationId { get; set; }
    public CleanConfiguration? Configuration { get; set; }
}
