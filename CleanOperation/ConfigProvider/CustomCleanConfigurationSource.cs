using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace CleanOperation.ConfigProvider
{
    public class CustomCleanConfigurationSource : IConfigurationSource
    {
        public Action<DbContextOptionsBuilder> _optionsAction { get; set; }

        public CustomCleanConfigurationSource(Action<DbContextOptionsBuilder> optionsAction)
        {
            _optionsAction = optionsAction;
        }

        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return new CleanCustomConfigurationProvider(_optionsAction, this);
        }
    }
}
