using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageProcessing
{
    public class PhpServerException : Exception
    {
        public string Kod { get; set; }

        public PhpServerException(string message = null, string kod = null)
            : base(message)
        {
            this.Kod = kod;
        }

    }
}
