using System;
using System.Collections.Concurrent;
using System.Linq;

namespace WoTget.Core.Repositories
{
    public class RepositoryFactory
    {
        ConcurrentDictionary<string, IRepository> repos;
        public RepositoryFactory()
        {
            repos = new ConcurrentDictionary<string, IRepository>();
        }

        public bool RegisterRepository(string name, IRepository repository)
        {
            return repos.TryAdd(name,repository);
        }

        public IRepository Get(string name)
        {
            if (repos.Count == 0) throw new ArgumentException("No registered Repsoitory!");

            IRepository first = repos.First().Value;
            repos.TryGetValue(name, out first);
            return first;
        }
    }
}
