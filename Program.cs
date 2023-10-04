using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SixtyNames
{
    /// <summary>
    /// Начальный класс Program
    /// </summary>
    internal class Program
    {
        /// <summary>
        /// Точка входа
        /// </summary>
        /// <param name="args">Аргументы</param>
        static void Main(string[] args)
        {
            var programViewControll = ProgramViewControl.GetInstance();
            var programModel = ProgramModel.GetInstance();

            programViewControll.Run();

         }
    }
}
