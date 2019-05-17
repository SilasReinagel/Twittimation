using Twittimation.Commands;
using Twittimation.IO;

namespace Twittimation
{
    public class Program
    {
        private static AppDataJsonStorage _io { get; } = new AppDataJsonStorage("Twittimation");

        public static void Main(string[] args)
        {
            var cli = new Cli("Invalid Command, type 'help' to display all commands with their help sections");
            AddCommands(cli);
            if (args.Length == 0)
                cli.Execute(nameof(Run));
            else
                cli.Execute(args);
            
        }

        private static void AddCommands(Cli cli)
        {
            var credentials = new KeyStored<Credentials>(_io, "Credentials", () => new Credentials("N/A", "N/A", "N/A", "N/A"));
            var tasks = new KeyStored<Tasks>(_io, "Tasks", () => new Tasks());
            var tweet = new Tweet(credentials);
            cli.AddCommands(
                new SaveCredentials(credentials),
                new ScheduleTweet(tasks, tweet),
                new ScheduleTweetCollection(tasks, tweet),
                new Run(tasks, cli),
                new ListTasks(tasks),
                new Cancel(tasks),
                tweet,
                new Help(cli.Commands));
        }
    }
}
