﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SixtyNames.Commands
{
    internal class GetEmailsByContract
    {
        #region Constructor

        public GetEmailsByContract(string text)
        {
            Text = text;
        }

        #endregion

        #region Properties

        public string Text { get; }
        public Exception Exception { get; set; }
        public string Response { get; set; }

        #endregion

    }
}
