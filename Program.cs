using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CopyAsync
{
    class Program
    {
        static void Main(string[] args)
        {
            new Copy().Run(args[0], args[1]);
        }
    }
}
