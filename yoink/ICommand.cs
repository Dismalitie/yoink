using AbysmalCore.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace yoink
{
    internal interface ICommand
    {
        public static abstract void Invoke(ArgumentParser p, bool quiet);
        public static abstract void Help();
    }
}
