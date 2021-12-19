using System.Collections.Generic;
using System.Threading.Tasks;

namespace XcpcArchive
{
    public interface IStartupInitializer
    {
        Task DoWorkAsync();
    }

    public class StartupInitializer : IStartupInitializer
    {
        private readonly IEnumerable<IStartupInitializer> _initializers;

        public StartupInitializer(IEnumerable<IStartupInitializer> initializers)
        {
            _initializers = initializers;
        }

        public async Task DoWorkAsync()
        {
            foreach (var initializer in _initializers)
            {
                await initializer.DoWorkAsync();
            }
        }
    }
}
