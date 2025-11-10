using AbysmalCore;
using AbysmalCore.Console;
using Newtonsoft.Json;
using Spectre.Console;

namespace yoink
{
    internal struct Manifest
    {
        private static AbysmalConsole _c = Program._c;
        public Manifest(string url)
        {
            _c.Write($"grabbing manifest from {url}");
            HttpClient hc = new();
            string json = hc.GetStringAsync(url).Result;
            this = JsonConvert.DeserializeObject<Manifest>(json);
            _c.WriteColorLn(" [done]", ConsoleColor.Green);

            if (ImageUrl != null)
            {
                _c.Write("grabbing thumbnail");
                Stream s = hc.GetStreamAsync(ImageUrl).Result;
                _c.WriteColorLn(" [done]", ConsoleColor.Green);

                LocalImgPath = Path.GetTempFileName();
                FileStream fs = new(LocalImgPath, FileMode.Create, FileAccess.Write);
                s.CopyTo(fs);

                fs.Flush();
                s.Dispose();
                fs.Dispose();
            }
        }

        public string Name = "unknown";
        public string Description = "no description";
        public string? ImageUrl = null;
        [JsonIgnore] public string? LocalImgPath = null;

        public Package[] Versions;

        public void Display()
        {
            if (LocalImgPath != null)
            {
                var image = new CanvasImage(LocalImgPath);
                image.MaxWidth(12);
                AnsiConsole.Write(image);
            }

            _c.WriteColorLns([
                ($"name:", ConsoleColor.Yellow, null),
                ($"  {Name}", ConsoleColor.White, null),
                ($"description:", ConsoleColor.Yellow, null),
                ($"  {Description}", ConsoleColor.White, null),
            ]);

            _c.WriteColor($"version(s): ", ConsoleColor.Yellow);
            _c.WriteLn(Versions.Length.ToString());
            foreach (Package p in Versions)
            {
                p.Display("  ", latest: Versions.Last().Version == p.Version);
                _c.WriteLn();
            }
        }

        public Package? GetVersion(string ver)
        {
            try { return Versions.First(p => p.Version == ver); }
            catch { return null; }
        }
    }

    internal struct File
    {
        public string Name;
        public string RelativeLocation;
        public string DownloadUrl;
    }

    internal struct Package
    {
        private static AbysmalConsole _c = Program._c;
        public Package()
        {
            
        }

        public string Version = "unknown";
        public bool Reccomended = false;
        public string Description = "no description";
        public string DefaultInstallPath;
        public string EntryFile;

        public File[] Files = [];

        public void Display(string indent = "", bool expanded = false, ConsoleColor titleCol = ConsoleColor.Blue, bool latest = false)
        {
            _c.WriteColor($"{indent}version: ", titleCol);
            if (Reccomended)
            {
                _c.WriteColors([
                    (Version, ConsoleColor.White, null),
                    (" (recommended)\n", ConsoleColor.Green, null),
                ]);
            }
            else if (latest)
            {
                _c.WriteColors([
                    (Version, ConsoleColor.White, null),
                    (" (latest)\n", ConsoleColor.Yellow, null),
                ]);
            }
            else _c.WriteLn(Version);

            _c.WriteColor($"{indent}description: ", titleCol);
            _c.WriteLn(Description);
            _c.WriteColor($"{indent}file(s): ", titleCol);
            _c.WriteLn(Files.Length.ToString());

            if (expanded)
            {
                foreach (File f in Files)
                {
                    _c.WriteColor($"{indent}  file ", ConsoleColor.Blue);
                    _c.WriteLn(f.Name);
                }

                _c.WriteColor($"{indent}installs to: ", titleCol);
                _c.WriteLn(DefaultInstallPath);
                _c.WriteColor($"{indent}installation executable: ", titleCol);
                _c.WriteLn(EntryFile);
            }
        }
    }
}
