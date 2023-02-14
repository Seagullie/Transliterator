using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transliterator.Core.Helpers.Exceptions
{
    public class TableNotSetException : Exception
    {
        public TableNotSetException(string? message) : base(message)
        {
        }
    }
}