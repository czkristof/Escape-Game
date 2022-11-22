using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Escape.EventArguments
{
    public class GameRefreshEventArgs
    {
        public int[,] Board { get; set; }

        public GameRefreshEventArgs(int[,] board)
        {
            Board = board;
        }   
    }
}
