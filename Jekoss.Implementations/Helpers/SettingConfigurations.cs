using Microsoft.Extensions.Configuration;

namespace Jekoss.Implementations.Helpers
{
    public class SettingConfigurations
    {
        private readonly IConfigurationRoot _root;

        public SettingConfigurations(IConfigurationRoot root)
        {
            _root = root;
        }
    }
}