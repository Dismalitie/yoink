using Spectre.Console;

namespace yoink.Commands
{
    internal class Grab : ICommand
    {
        public static void Help()
        {
            AnsiConsole.MarkupLine("[yellow]grab[/] [blue]<package>[/]");
            AnsiConsole.MarkupLine("  gets a package manifest from the provided manifest destination (url/yoink repo) at begins install process");
        }

        public static void Invoke(string[] args)
        {
            string pkgName = args[1];

            Manifest manifest = new(pkgName);
            manifest.Display();

            string recommendedVer;
            try { recommendedVer = manifest.Versions.First(v => v.Recommended).Version; }
            catch { recommendedVer = "none"; }

            string latestPkgVer = manifest.Versions.Last().Version;
            string selectedVer = AnsiConsole.Prompt(new TextPrompt<string>($"package version to grab [gray](recommended: {recommendedVer}, latest: {latestPkgVer}, <enter> for recommended)[/]")
                .DefaultValue(recommendedVer));

            Package pkg;
            if (selectedVer == string.Empty) pkg = (Package)manifest.GetVersion(recommendedVer)!;
            else if (manifest.GetVersion(selectedVer.Trim()) == null)
            {
                AnsiConsole.MarkupLine("[red]/!\\ specified version not found in manifest[/]");
                return;
            }
            else pkg = (Package)manifest.GetVersion(selectedVer.Trim())!;

            void loop()
            {
                AnsiConsole.WriteLine();
                pkg.Display(titleCol: ConsoleColor.Yellow, expanded: true);
                AnsiConsole.WriteLine();

                //                          0                  1                      2                            3
                string[] options = ["[green]grab[/]", "[magenta]restore[/]", "[blue]change install dest[/]", "[red]cancel[/]"];
                string option = AnsiConsole.Prompt(new SelectionPrompt<string>().AddChoices(options));
                if (option == options[2])
                {
                    string dest = AnsiConsole.Prompt(new TextPrompt<string>("new install dest:"));
                    pkg.RootInstallPath = dest;

                    AnsiConsole.Clear();
                    loop();
                }
                else if (option == options[1])
                {
                    pkg = (Package)manifest.GetVersion(selectedVer.Trim())!;
                    AnsiConsole.Clear();
                    loop();
                }
                else if (option == options[3]) return;
            }
            loop();

            bool confirmed = AnsiConsole.Prompt(new ConfirmationPrompt("grab this package? make sure you trust it"));
            if (!confirmed) return;

            HttpClient hc = new();
            AnsiConsole.Progress()
            .Start(ctx =>
            {
                List<(ProgressTask task, string url, string path)> files = new();
                foreach (File f in pkg.Files) files.Add((
                    ctx.AddTask($"{f.Name} [blue]downloading from[/] {f.DownloadUrl} [blue]to[/] {Path.GetFullPath(Path.Combine(pkg.RootInstallPath, f.RelativeLocation))}"),
                    f.DownloadUrl,
                    Path.GetFullPath(Path.Combine(pkg.RootInstallPath, f.RelativeLocation))
                ));

                while (!ctx.IsFinished)
                {
                    foreach (var file in files)
                    {
                        Stream data = hc.GetStreamAsync(file.url).Result;
                        FileStream destination = new(file.path, FileMode.Create, FileAccess.Write);
                        data.CopyTo(destination);

                        data.Dispose();
                        destination.Dispose();

                        file.task.Increment(100);
                    }
                }
            });
        }
    }
}
