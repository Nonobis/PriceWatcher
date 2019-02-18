using SimpleHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PriceWatcher
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine($"##########   starting service '{AssemblyHelper.AssemblyTitle}', V '{AssemblyHelper.AssemblyVersion}'   ##########");
            ConfigureService.Configure();
        }
    }
}
