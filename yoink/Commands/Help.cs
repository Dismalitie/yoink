using Spectre.Console;

namespace yoink.Commands
{
    internal class Help : ICommand
    {
        public static void Invoke(string[] args)
        {
            string cmd = args[1];

            switch (cmd)
            {
                case "grab":
                    Grab.Help();
                    break;
                case "help":
                    AnsiConsole.MarkupLine("[red]really? its pretty self-explanatory[/]");
                    break;
            }
        }

        static void ICommand.Help() => Invoke(["help help"]);
    }
}
