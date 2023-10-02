using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SixtyNames
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var programViewControll = ProgramViewControl.GetInstance();
            var programModel = ProgramModel.GetInstance();

            programViewControll.Run();

            Console.ReadLine();
         }
    }
}
