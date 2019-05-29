using System.Threading.Tasks;
using Carvana;

namespace Twittimation
{
    public interface ICommand
    {
        string Name { get; }
        Task<Result> Execute(string[] args);
    }

    public static class CommandExtensions
    {
        public static void Execute(this ICommand cmd) => cmd.Execute(new string[0]);
    }
}
