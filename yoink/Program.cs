using AbysmalCore.Console;
using AbysmalCore.Debugging;
using yoink;

internal class Program
{
    public static AbysmalConsole _c = AbysmalDebug.Console;

    private static void Main(string[] args)
    {
        AbysmalDebug.Enabled = false;

#if DEBUG
        args = ["grab", "yoink"];
#endif

        if (args.Length < 2)
        {
            _c.WriteColorLn("usage: ", ConsoleColor.Yellow);
            _c.WriteColors([
                ("  yoink", ConsoleColor.Blue, null),
                (" <grab | drop | up | help> <package | command>", ConsoleColor.White, null),
                (" [...]", ConsoleColor.DarkGray, null)
            ]); _c.WriteLn();

            _c.WriteLn();

            _c.WriteColorLn("args: ", ConsoleColor.Yellow);
            _c.WriteColorLns([
                ("  grab    gets a package", ConsoleColor.Green, null),
                ("  drop    uninstalls a package", ConsoleColor.Red, null),
                ("  up      updates a package", ConsoleColor.Blue, null),
                ("  help    tells you what yoink does", ConsoleColor.Magenta, null),
            ]); _c.WriteLn();

            _c.WriteColorLn("flags: ", ConsoleColor.Yellow);
            _c.WriteColorLns([
                ("  -q | -quiet                      installs the package with minimal prompting and display (use flags to minimize)", ConsoleColor.White, null),
                ("  -l:<dir> | -location:<dir>       sets the install location", ConsoleColor.White, null),
                ("  -v:<*.*.*> | -version:<*.*.*>    installs a specific version of a package", ConsoleColor.White, null),
            ]);

            return;
        }

        ArgumentParser p = new(args, 2);
        string command = p.Arguments[0];
        string packageOrCmd = p.Arguments[1];

        bool quiet = p.HasFlag("q")
        | p.HasFlag("quiet");

        switch (command)
        {
            case "help":
                yoink.Commands.Help.Invoke(p, quiet);
                break;
            case "grab":
                yoink.Commands.Grab.Invoke(p, quiet);
                break;
        }
    }
}