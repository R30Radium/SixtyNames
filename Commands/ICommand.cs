using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SixtyNames.Commands
{
    interface ICommand
    {
        string Text { get; }

        Exception Exception { get; set; }

        string Response { get; set; }    
    }
}
