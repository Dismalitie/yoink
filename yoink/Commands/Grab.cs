using AbysmalCore.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace yoink.Commands
{
    internal class Grab : ICommand
    {
        static AbysmalConsole _c = Program._c;

        public static void Help()
        {
            _c.WriteColors([
                ("grab", ConsoleColor.Yellow, null),
                (" <package>", ConsoleColor.Blue, null)
            ]); _c.WriteLn();
            _c.WriteColorLn("   gets a package manifest from the provided manifest destination (url/yoink repo) at begins install process", ConsoleColor.White);

            _c.WriteLn();

            _c.WriteColorLn("appliccable flags: ", ConsoleColor.Yellow);
            _c.WriteColorLns([
                ("  -q | -quiet                      installs the package with minimal prompting and display (use flags to minimize)", ConsoleColor.White, null),
                ("  -l:<dir> | -location:<dir>       sets the install location", ConsoleColor.White, null),
                ("  -v:<*.*.*> | -version:<*.*.*>    installs a specific version of a package", ConsoleColor.White, null),
            ]);
        }

        public static async void Invoke(ArgumentParser p, bool quiet)
        {
            string package = p.Arguments[1];
            Manifest manifest = new("https://dummyjson.com/recipes");
        }
    }
}
