using Escape.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Escape.Persistence
{
    public class DataAccess : IDataAccess
    {
        public async Task SaveAsync(string path, int size, int h, int m, int s, int[,] board)
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(path))
                {
                    sw.WriteLine(size);
                    sw.WriteLine(h);
                    sw.WriteLine(m);
                    sw.WriteLine(s);
                    for (int i = 0; i < size; i++)
                    {
                        for (int j = 0; j < size; j++)
                        {
                            await sw.WriteAsync(board[i, j] + " ");
                        }
                    }
                }
            }
            catch
            {
                throw new DataExceptions();
            }
        }

        public async Task<(int Size, int H, int M, int S, int[,] Board)> LoadAsync(string path)
        {
            try
            {
                using (StreamReader sr = new StreamReader(path))
                {
                    string line = await sr.ReadLineAsync();
                    int size = int.Parse(line);

                    line = await sr.ReadLineAsync();
                    int h = int.Parse(line);

                    line = await sr.ReadLineAsync();
                    int m = int.Parse(line);

                    line = await sr.ReadLineAsync();
                    int s = int.Parse(line);

                    line = await sr.ReadLineAsync();
                    string[] numbers = line.Split(' ');
                    int[,] board = new int[size, size];

                    int counter = 0;
                    for (int i = 0; i < size; i++)
                    {
                        for (int j = 0; j < size; j++)
                        {
                            board[i, j] = int.Parse(numbers[counter]);
                            counter++;
                        }
                    }
                    return (size, h, m, s, board);
                }
            }
            catch
            {
                throw new DataExceptions();
            }
        }
    }
}
