using AbysmalCore;
using Newtonsoft.Json;
using Spectre.Console;

namespace yoink
{
    internal struct Manifest
    {
        public Manifest(string url)
        {
            Manifest m = new();

            AnsiConsole.Progress()
            .Start(ctx =>
            {
                var download = ctx.AddTask("[green]Downloading manifest[/]");
                var parse = ctx.AddTask("[green]Parsing[/]");

                HttpClient hc = new();
                string json = hc.GetStringAsync(url).Result;
                download.Increment(100);

                m = JsonConvert.DeserializeObject<Manifest>(json);
                parse.Increment(100);

                if (m.ImageUrl != null)
                {
                    var img = ctx.AddTask("[green]Downloading thumbnail[/]");
                    Stream s = hc.GetStreamAsync(m.ImageUrl).Result;
                    img.Increment(33);

                    m.LocalImgPath = Path.GetRandomFileName();
                    FileStream fs = new(m.LocalImgPath, FileMode.Create, FileAccess.Write);
                    s.CopyTo(fs);
                    img.Increment(33);

                    fs.Flush();
                    s.Dispose();
                    fs.Dispose();
                    img.Increment(34);
                }
            });

            this = m;
        }

        public string Name = "Unknown";
        public string Description = "Unknown";
        public string? ImageUrl = null;
        [JsonIgnore] public string LocalImgPath;

        public Package[] Versions;
    }

    internal struct Package
    {
        public string Version;
        public string DefaultInstallPath;
        public string EntryFile;

        public string[] FileUrls;
    }
}
