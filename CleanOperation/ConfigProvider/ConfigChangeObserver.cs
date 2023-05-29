namespace CleanOperation.ConfigProvider
{
    public class ConfigChangeObserver
    {
        public event EventHandler<ConfigChangeEventArgs> Changed;

        public void OnChanged(ConfigChangeEventArgs e)
        {
            ThreadPool.QueueUserWorkItem((_) => Changed?.Invoke(this, e));
        }

        private static readonly Lazy<ConfigChangeObserver> lazy = new Lazy<ConfigChangeObserver>(() => new ConfigChangeObserver());

        private ConfigChangeObserver() { }

        public static ConfigChangeObserver Instance => lazy.Value;
    }
    public class ConfigChangeEventArgs : EventArgs
    {
        public object State { get; set; }

        public ConfigChangeEventArgs(object state)
        {
            State = state;
        }
    }
}
