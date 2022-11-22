using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Escape.EventArguments
{
    public class GameEndedEventArgs
    {
        public string Winner { get; set; }
        public int Size { get; set; }

        public GameEndedEventArgs(string winner, int size)
        {
            Winner = winner;
            Size = size;
        }   
    }
}
