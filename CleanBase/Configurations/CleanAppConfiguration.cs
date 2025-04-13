namespace CleanBase.Configurations;

public class Auth
{
    public string Authority { get; set; }
    public string Audience { get; set; }
    public List<string> ValidAudiences { get; set; }
    public List<string> ValidIssuers { get; set; }
    public string Key { get; set; }
}

public class CleanAppConfiguration
{
    public const string Name = "CleanAppConfiguration";
    public InMemoryCaching InMemoryCaching { get; set; }
    public Auth Auth { get; set; }
}

public class Configs
{
    public string Host { get; set; }
    public string Port { get; set; }
}

public class InMemoryCaching
{
    public bool Enabled { get; set; }
    public string Provider { get; set; }
    public Configs Configs { get; set; }
}



