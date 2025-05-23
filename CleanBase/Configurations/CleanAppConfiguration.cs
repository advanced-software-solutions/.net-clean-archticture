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
    public Datastore Datastore { get; set; }
    public ResponseCache ResponseCache { get; set; }
}

public class ResponseCache
{
    public bool Enabled { get; set; }
    public int Duration { get; set; }
    public string[] VaryByHeaders { get; set; }
}

public class Datastore
{
    public string ConnectionString { get; set; }
    public string Provider { get; set; }
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



