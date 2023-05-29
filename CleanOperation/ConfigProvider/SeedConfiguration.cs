using CleanBase.Entities;
using CleanOperation.DataAccess;

namespace CleanOperation.ConfigProvider
{
    public class SeedConfiguration
    {
        private readonly AppDataContext dataContext;

        public SeedConfiguration(AppDataContext dataContext)
        {
            this.dataContext = dataContext;
        }
        public void Seed()
        {
            dataContext.Add<CleanConfiguration>(new CleanConfiguration
            {
                Name = "App",
                ConfigurationItems =
                new() {
                    new CleanConfigurationItem
                    {
                        Key = "Title",
                        Value = "Clean Arch .NET 6"
                    }
            }
            });
            dataContext.SaveChanges();
        }
    }
}
