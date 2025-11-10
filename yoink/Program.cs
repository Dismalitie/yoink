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
        args = ["grab", "yoink.tests.dummy1 -l:C"];
#endif

        if (args.Length < 2)
        {
            _c.WriteColorLn("usage: ", ConsoleColor.Yellow);
            _c.WriteColors([
                ("  yoink", ConsoleColor.Blue, null),
                (" <grab | drop | up | help> <package | command>", ConsoleColor.White, null)
            ]); _c.WriteLn();

            _c.WriteLn();

            _c.WriteColorLn("commands: ", ConsoleColor.Yellow);
            _c.WriteColorLns([
                ("  grab    gets a package", ConsoleColor.Green, null),
                ("  drop    uninstalls a package", ConsoleColor.Red, null),
                ("  up      updates a package", ConsoleColor.Blue, null),
                ("  help    tells you what yoink does", ConsoleColor.Magenta, null),
            ]); _c.WriteLn();

            return;
        }

        args = args.Select(a => a.Contains("yoink.tests.dummy1") ? "https://dummyjson.com/c/0d09-2e89-4469-89fa" : a).ToArray();

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