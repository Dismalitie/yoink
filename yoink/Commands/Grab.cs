using AbysmalCore.Console;
using Spectre.Console;
using System.Diagnostics.Tracing;

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
        }

        public static void Invoke(string[] args)
        {
            string pkgName = args[1];

            Manifest manifest = new(pkgName);
            manifest.Display();

            string reccomendedVer;
            try { reccomendedVer = manifest.Versions.First(v => v.Reccomended).Version; }
            catch { reccomendedVer = "none"; }

            string latestPkgVer = manifest.Versions.Last().Version;
            string selectedVer = AnsiConsole.Prompt(new TextPrompt<string>("package version to grab").DefaultValue($"reccomended: {reccomendedVer}, latest: {latestPkgVer}"));

            Package pkg;
            if (manifest.GetVersion(selectedVer.Trim()) == null)
            {
                _c.WriteColorLn("invalid package version!", ConsoleColor.Red);
                return;
            }
            else pkg = (Package)manifest.GetVersion(selectedVer.Trim())!;

            void loop()
            {
                _c.WriteLn();
                pkg.Display(titleCol: ConsoleColor.Yellow, expanded: true);
                _c.WriteLn();

                string[] options = ["change install dest", "grab", "restore", "cancel"];
                string option = AnsiConsole.Prompt(new SelectionPrompt<string>().AddChoices(options));
                if (option == options[0])
                {
                    string dest = AnsiConsole.Prompt(new TextPrompt<string>("new install dest:"));
                    pkg.DefaultInstallPath = dest;

                    _c.Clear();
                    loop();
                }
                else if (option == options[2])
                {
                    pkg = (Package)manifest.GetVersion(selectedVer.Trim())!;
                    _c.Clear();
                    loop();
                }
                else if (option == options[3]) return;
            }
            loop();

            bool confirmed = AnsiConsole.Prompt(new ConfirmationPrompt("grab this package? make sure you trust it"));
            if (!confirmed) return;

            AnsiConsole.Progress()
            .Start(ctx =>
            {
                List<ProgressTask> tasks = new();
                foreach (File f in pkg.Files) tasks.Add(ctx.AddTask($"{f.Name} [blue]downloading from[/] {f.DownloadUrl} [blue]to[/] {Path.GetFullPath(Path.Combine(pkg.DefaultInstallPath, f.RelativeLocation))}"));

                while (!ctx.IsFinished)
                {
                    foreach (ProgressTask task in tasks)
                    {
                        task.Increment(task.MaxValue);
                    }
                }
            });

        }
    }
}
