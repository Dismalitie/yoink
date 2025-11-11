using Newtonsoft.Json;
using Spectre.Console;

namespace yoink
{
    internal struct Manifest
    {
        public Manifest(string url)
        {
            AnsiConsole.Markup($"grabbing manifest from {url}");
            HttpClient hc = new();
            string json = hc.GetStringAsync(url).Result;
            this = JsonConvert.DeserializeObject<Manifest>(json);
            AnsiConsole.MarkupLine(" [green][[done]][/]");

            if (ImageUrl != null)
            {
                AnsiConsole.Markup($"grabbing thumbnail");
                Stream s = hc.GetStreamAsync(ImageUrl).Result;
                AnsiConsole.MarkupLine(" [green][[done]][/]");

                LocalImgPath = Path.GetTempFileName();
                FileStream fs = new(LocalImgPath, FileMode.Create, FileAccess.Write);
                s.CopyTo(fs);

                s.Dispose();
                fs.Dispose();
            }

            hc.Dispose();
        }

        [JsonProperty("name")]      public string Name = "unknown";
        [JsonProperty("desc")]      public string Description = "no description";
        [JsonProperty("thumbnail")] public string? ImageUrl = null;
        [JsonIgnore]                public string? LocalImgPath = null;

        [JsonProperty("vers")] public Package[] Versions;

        public void Display()
        {
            if (LocalImgPath != null)
            {
                var image = new CanvasImage(LocalImgPath);
                image.MaxWidth(12);
                AnsiConsole.Write(image);
            }

            AnsiConsole.MarkupLine($"[yellow]name:[/]");
            AnsiConsole.MarkupLine($"  {Name}");
            AnsiConsole.MarkupLine($"[yellow]description:[/]");
            AnsiConsole.MarkupLine($"  {Description}");

            AnsiConsole.MarkupLine($"[yellow]version(s):[/] {Versions.Length}");
            foreach (Package p in Versions)
            {
                p.Display("  ", latest: Versions.Last().Version == p.Version);
                AnsiConsole.WriteLine();
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
        public File() { }
        [JsonProperty("name")]     public string Name = "file";
        [JsonProperty("path")]     public string RelativeLocation;
        [JsonProperty("url")]      public string DownloadUrl;
        [JsonProperty("critical")] public bool Critical = false;

        public void Download(string path)
        {
            HttpClient hc = new();
            byte[] data = hc.GetByteArrayAsync(DownloadUrl).Result;
            FileStream fs = new(path, FileMode.Create, FileAccess.Write);
            fs.Write(data, 0, data.Length);
            fs.Flush();
            fs.Dispose();
            hc.Dispose();
        }
    }

    internal struct Package
    {
        public Package() { }

        [JsonProperty("ver")]            public string Version = "unknown";
        [JsonProperty("recommended")]    public bool Recommended = false;
        [JsonProperty("desc")]           public string Description = "no description";
        [JsonProperty("root")]           public string RootInstallPath;
        [JsonProperty("postInstallCmd")] public string PostInstallCmd;

        [JsonProperty("files")] public File[] Files = [];

        public void Display(string indent = "", bool expanded = false, ConsoleColor titleCol = ConsoleColor.Blue, bool latest = false)
        {
            string tCol = Color.FromConsoleColor(titleCol).ToMarkup();

            AnsiConsole.MarkupLine($"[{tCol}]{indent}version:[/]");
            if (Recommended) AnsiConsole.MarkupLine($"{indent}  {Version} [green](recommended)[/]");
            else if (latest) AnsiConsole.MarkupLine($"{indent}  {Version} [yellow](latest)[/]");
            else AnsiConsole.MarkupLine($"{indent}  {Version}");

            AnsiConsole.MarkupLine($"[{tCol}]{indent}description:[/]");
            AnsiConsole.MarkupLine($"{indent}  {Description}");
            AnsiConsole.MarkupLine($"{indent}[{tCol}]files:[/] {Files.Length}");

            if (expanded)
            {
                foreach (File f in Files) AnsiConsole.MarkupLine($"{indent}  [blue]file[/] {f.Name}");

                AnsiConsole.MarkupLine($"{indent}[{tCol}]installs to:[/]");
                AnsiConsole.MarkupLine($"  {RootInstallPath}");
                AnsiConsole.MarkupLine($"{indent}[{tCol}]post install command:[/]");
                AnsiConsole.MarkupLine($"  {PostInstallCmd}");
            }
        }
    }
}
