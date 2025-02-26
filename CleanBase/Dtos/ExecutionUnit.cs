namespace CleanBase.Dtos
{
    public record ExecutionUnit
    {
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public Target Target { get; set; }
        public Status Status { get; set; }
        public string Location { get; set; }
        public object DataIn { get; set; }
        public object DataOut { get; set; }
        public string Error { get; set; }
        public string ErrorDetails { get; set; }
        public string Version { get; set; }
        public string Name { get; set; }
    }

    public enum Target
    {
        API,
        Database,
        Queue
    }

    public enum Status
    {
        Success,
        Failure
    }
}
