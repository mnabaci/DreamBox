using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageProcessing
{
    public class HttpExeption : PhpServerException
    {
        public HttpExeption(string mesj, string _kod)
            : base(mesj, _kod)
        {

        }
    }
}
