using System.Threading.Tasks;
using Carvana;
using Twittimation.IO;

namespace Twittimation.Commands
{
    public sealed class WithDisplayedUsername : ICommand
    {
        private readonly ILog _log;
        private readonly IStored<Credentials> _credentials;
        private readonly ICommand _inner;

        public WithDisplayedUsername(ILog log, IStored<Credentials> credentials, ICommand inner)
        {
            _log = log;
            _credentials = credentials;
            _inner = inner;
        }

        public string Name => _inner.Name;

        public async Task<Result> Execute(string[] args)
        {
            var credentials = _credentials.Get();
            var username = credentials.AreValid ? credentials.Username : "None";
            _log.Info($"{nameof(credentials.Username)}: {username}");
            _log.Info("");
            return await _inner.Execute(args);
        }
    }
}
