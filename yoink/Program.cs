using Spectre.Console;

internal class Program
{
    private static void Main(string[] args)
    {
#if DEBUG
        args = ["grab", "yoink.tests.dummy1 -l:C"];
#endif

        if (args.Length < 2)
        {
            AnsiConsole.MarkupLine("[yellow]usage:[/]");
            AnsiConsole.MarkupLine("[blue]yoink[/] <grab|up|drop|help> <package|command> [gray][[...]][/]");

            AnsiConsole.MarkupLine("[yellow]commands:[/]");
            AnsiConsole.MarkupLine("  [green]grab[/] [gray]<package>[/]    gets a package");
            AnsiConsole.MarkupLine("  [red]drop[/] [gray]<package>[/]    uninstalls a package");
            AnsiConsole.MarkupLine("  [blue]up[/] [gray][[package]][/]    updates a package");
            AnsiConsole.MarkupLine("  [magenta]help[/] [gray][[command]][/]    tells you what different commands do");

            AnsiConsole.MarkupLine("[yellow]commands:[/]");
            AnsiConsole.MarkupLine("  -q | -quiet    installs the package with minimal prompting and display (use flags to minimize)");
            AnsiConsole.MarkupLine("  -l:<dir> | -location:<dir>    sets the install location");
            AnsiConsole.MarkupLine("  -v:<*.*.*> | -version:<*.*.*>    installs a specific version of a package");

            return;
        }

        args = args.Select(a => a.Contains("yoink.tests.dummy1") ? "https://dummyjson.com/c/cec4-6996-4bb4-878d" : a).ToArray();

        switch (args[0])
        {
            case "help":
                yoink.Commands.Help.Invoke(args);
                break;
            case "grab":
                yoink.Commands.Grab.Invoke(args);
                break;
        }
    }
}