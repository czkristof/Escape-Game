using Escape.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Escape.Persistence
{
    public interface IDataAccess
    {
        Task SaveAsync(string path, int size, int h, int m, int s, int[,] board);

        Task<(int Size, int H, int M, int S, int[,] Board)> LoadAsync(string path);
    }
}
