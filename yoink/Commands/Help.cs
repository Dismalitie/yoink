using AbysmalCore.Console;

namespace yoink.Commands
{
    internal class Help : ICommand
    {
        public static void Invoke(ArgumentParser p, bool quiet)
        {
            string cmd = p.Arguments[1];

            switch (cmd)
            {
                case "grab":
                    Grab.Help();
                    break;
                case "help":
                    Program._c.WriteLn("Really? It's pretty self-explanatory.");
                    break;
            }
        }

        static void ICommand.Help() => Invoke(new(["help help"], 1), false);
    }
}
