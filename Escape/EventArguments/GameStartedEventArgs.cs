using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Escape.EventArguments
{
    public class GameStartedEventArgs
    {
        public int BoardSize { get; set; }
        public int[,] Board { get; set; }
        public int H { get; set; }
        public int M { get; set; }
        public int S { get; set; }

        public GameStartedEventArgs(int boardSize, int h, int m, int s, int[,] board)
        {
            BoardSize = boardSize;
            H = h;
            M = m;
            S = s;
            Board = board;
        }
    }
}
