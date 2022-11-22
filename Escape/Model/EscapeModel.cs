using Escape.EventArguments;
using Escape.Persistence;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Escape.Model
{
    public class EscapeModel
    {
        private IDataAccess _dataAccess;
        private int _size;
        private int[,] _board;


        private bool _bomb;
        private bool _enemy1;
        private bool _enemy2;
        private bool _catch;

        private string _winner;

        private int _h = 0, _m = 0, _s = 0;


        public int[,] Board { get => _board; set => _board = value; }
        
        

        public int Hour { get => _h; set => _h = value; }
        public int Minute { get => _m; set => _m = value; }
        public int Second { get => _s; set => _s = value; }
        public string Winner { get => _winner; set => _winner = value; }
        public bool Bomb { get => _bomb; set => _bomb = value; }
        public bool Enemy1 { get => _enemy1; set => _enemy1 = value; }
        public bool Enemy2 { get => _enemy2; set => _enemy2 = value; }
        public bool Catch { get => _catch; set => _catch = value; }
        public int Size { get => _size; set => _size = value; }

        public event EventHandler<GameStartedEventArgs> GameStarted;
        public event EventHandler<GameRefreshEventArgs> GameRefresh;
        public event EventHandler<GameEndedEventArgs> GameEnded;


        public EscapeModel(IDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
            _size = 0;
            _board = new int[_size, _size];

        }

        public void StartNewGame(int size)
        {

            Random mines = new Random();

            _h = 0;
            _m = 0;
            _s = 0;

            _size = size;
            _winner = "";
            _board = new int[size, size];

            _bomb = true;
            _enemy1 = true;
            _enemy2 = true;
            _catch = true;

            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    /*if (i == 0 && j == (size - 1 / 2))
                    {
                        _board[i, j] = 1;
                    }
                    else if (i == (size - 1) && j == 0 || i == (size - 1) && j == (size - 1))
                    {
                        _board[i, j] = 2;
                    }
                    else
                    {
                        _board[i, j] = 0;
                    }*/
                    _board[i, j] = 0;
                }
            }
            _board[0, (size - 1) / 2] = 1;
            _board[(size - 1), 0] = 2;
            _board[(size - 1), (size - 1)] = 3;

            switch (size)
            {
                case 11:
                    Mines(size, mines.Next(3, 6));
                    break;
                case 15:
                    Mines(size, mines.Next(6, 11));
                    break;
                case 21:
                    Mines(size, mines.Next(11, 22));
                    break;
            }

            if (GameStarted is not null)
            {
                GameStarted(this, new GameStartedEventArgs(size, _h, _m, _s,  _board));
            }
        }

        public void Mines(int size, int m)
        {
            Random mines = new Random();

            while (m > 0)
            {
                int r = mines.Next(size);
                int c = mines.Next(size);
                if (_board[r, c] == 0)
                {
                    _board[r, c] = 4;
                    m--;
                }
            }
        }

        public void movePlayer(int row, int col, Keys key, int _size)
        {
            if (key == Keys.Up && _board[row, col] == 1 && row > 0)
            {
                if (_board[row - 1, col] == 4)
                {
                    _board[row, col] = 0;
                    _bomb = false;
                }
                else if (_board[row - 1, col] == 2 || _board[row - 1, col] == 3)
                {
                    _board[row, col] = 0;
                    _catch = false;
                }
                else
                {
                    _board[row - 1, col] = 1;
                    _board[row, col] = 0;
                }
            }
            else if (key == Keys.Down && row < _size - 1 && _board[row, col] == 1)
            {
                if (_board[row + 1, col] == 4)
                {
                    _board[row, col] = 0;
                    _bomb = false;
                }
                else if (_board[row + 1, col] == 2 || _board[row + 1, col] == 3)
                {
                    _board[row, col] = 0;
                    _catch = false;
                }
                else
                {
                    _board[row + 1, col] = 1;
                    _board[row, col] = 0;
                }
            }
            else if (key == Keys.Left && col > 0 && _board[row, col] == 1)
            {
                if (_board[row, col - 1] == 4)
                {
                    _board[row, col] = 0;
                    _bomb = false;
                }
                else if (_board[row, col - 1] == 2 || _board[row, col - 1] == 3)
                {
                    _board[row, col] = 0;
                    _catch = false;
                }
                else
                {
                    _board[row, col - 1] = 1;
                    _board[row, col] = 0;
                }
            }
            else if (key == Keys.Right && col < _size - 1 && _board[row, col] == 1)
            {
                if (_board[row, col + 1] == 4)
                {
                    _board[row, col] = 0;
                    _bomb = false;
                }
                else if (_board[row, col + 1] == 2 || _board[row, col + 1] == 3)
                {
                    _board[row, col] = 0;
                    _catch = false;
                }
                else
                {
                    _board[row, col + 1] = 1;
                    _board[row, col] = 0;
                }
            }

            if(GameRefresh is not null)
            {
                GameRefresh(this, new GameRefreshEventArgs(_board));
            }
            checkGameOver();
        }

        public void moveEnemy1(int playerRow, int playerCol, int enemyRow, int enemyCol)
        {
            if (_enemy1)
            {

                if (Math.Abs(enemyRow - playerRow) >= Math.Abs(enemyCol - playerCol))
                {
                    if (enemyRow >= playerRow)
                    {
                        if (_board[enemyRow - 1, enemyCol] == 4)
                        {
                            _board[enemyRow, enemyCol] = 0;
                            _enemy1 = false;
                        }
                        else if (_board[enemyRow - 1, enemyCol] == 1)
                        {
                            _board[enemyRow - 1, enemyCol] = 2;
                            _board[enemyRow, enemyCol] = 0;
                            _catch = false;
                        }
                        else if (_board[enemyRow, enemyCol] == 2 && _board[enemyRow - 1, enemyCol] != 3)
                        {
                            _board[enemyRow - 1, enemyCol] = 2;
                            _board[enemyRow, enemyCol] = 0;
                        }
                    }
                    if (enemyRow < playerRow)
                    {
                        if (_board[enemyRow + 1, enemyCol] == 4)
                        {
                            _board[enemyRow, enemyCol] = 0;
                            _enemy1 = false;
                        }
                        else if (_board[enemyRow + 1, enemyCol] == 1)
                        {
                            _board[enemyRow + 1, enemyCol] = 2;
                            _board[enemyRow, enemyCol] = 0;
                            _catch = false;
                        }
                        else if (_board[enemyRow, enemyCol] == 2 && _board[enemyRow + 1, enemyCol] != 3)
                        {
                            _board[enemyRow + 1, enemyCol] = 2;
                            _board[enemyRow, enemyCol] = 0;
                        }
                    }
                    
                }


                if (Math.Abs(enemyRow - playerRow) < Math.Abs(enemyCol - playerCol))
                {
                    if (enemyCol >= playerCol)
                    {
                        if (_board[enemyRow, enemyCol - 1] == 4)
                        {
                            _board[enemyRow, enemyCol] = 0;
                            _enemy1 = false;
                        }
                        else if (_board[enemyRow, enemyCol - 1] == 1)
                        {
                            _board[enemyRow, enemyCol - 1] = 2;
                            _board[enemyRow, enemyCol] = 0;
                            _catch = false;
                        }
                        else if (_board[enemyRow, enemyCol] == 2 && _board[enemyRow, enemyCol - 1] != 3)
                        {
                            _board[enemyRow, enemyCol - 1] = 2;
                            _board[enemyRow, enemyCol] = 0;
                        }
                    }
                    if (enemyCol < playerCol)
                    {
                        if (_board[enemyRow, enemyCol + 1] == 4)
                        {
                            _board[enemyRow, enemyCol] = 0;
                            _enemy1 = false;
                        }
                        else if (_board[enemyRow, enemyCol + 1] == 1)
                        {
                            _board[enemyRow, enemyCol + 1] = 2;
                            _board[enemyRow, enemyCol] = 0;
                            _catch = false;
                        }
                        else if (_board[enemyRow, enemyCol] == 2 && _board[enemyRow, enemyCol + 1] != 3)
                        {
                            _board[enemyRow, enemyCol + 1] = 2;
                            _board[enemyRow, enemyCol] = 0;
                        }
                    }
                }
            }
            
            if (GameRefresh is not null)
            {
                GameRefresh(this, new GameRefreshEventArgs(_board));
            }
            checkGameOver();
        }

        public void moveEnemy2(int playerRow, int playerCol, int enemyRow, int enemyCol)
        {
            if (_enemy2)
            {

                if (Math.Abs(enemyRow - playerRow) >= Math.Abs(enemyCol - playerCol))
                {
                    if (enemyRow >= playerRow)
                    {
                        if (_board[enemyRow - 1, enemyCol] == 4)
                        {
                            _board[enemyRow, enemyCol] = 0;
                            _enemy2 = false;
                        }
                        else if (_board[enemyRow - 1, enemyCol] == 1)
                        {
                            _board[enemyRow - 1, enemyCol] = 3;
                            _board[enemyRow, enemyCol] = 0;
                            _catch = false;
                        }
                        else if (_board[enemyRow, enemyCol] == 3 && _board[enemyRow - 1, enemyCol] != 2)
                        {
                            _board[enemyRow - 1, enemyCol] = 3;
                            _board[enemyRow, enemyCol] = 0;
                        }
                    }
                    if (enemyRow < playerRow)
                    {
                        if (_board[enemyRow + 1, enemyCol] == 4)
                        {
                            _board[enemyRow, enemyCol] = 0;
                            _enemy2 = false;
                        }
                        else if (_board[enemyRow + 1, enemyCol] == 1)
                        {
                            _board[enemyRow + 1, enemyCol] = 3;
                            _board[enemyRow, enemyCol] = 0;
                            _catch = false;
                        }
                        else if (_board[enemyRow, enemyCol] == 3 && _board[enemyRow + 1, enemyCol] != 2)
                        {
                            _board[enemyRow + 1, enemyCol] = 3;
                            _board[enemyRow, enemyCol] = 0;
                        }
                    }

                }


                if (Math.Abs(enemyRow - playerRow) < Math.Abs(enemyCol - playerCol))
                {
                    if (enemyCol >= playerCol)
                    {
                        if (_board[enemyRow, enemyCol - 1] == 4)
                        {
                            _board[enemyRow, enemyCol] = 0;
                            _enemy2 = false;
                        }
                        else if (_board[enemyRow, enemyCol - 1] == 1)
                        {
                            _board[enemyRow, enemyCol - 1] = 3;
                            _board[enemyRow, enemyCol] = 0;
                            _catch = false;
                        }
                        else if (_board[enemyRow, enemyCol] == 3 && _board[enemyRow, enemyCol - 1] != 2 )
                        {
                            _board[enemyRow, enemyCol - 1] = 3;
                            _board[enemyRow, enemyCol] = 0;
                        }
                    }
                    if (enemyCol < playerCol)
                    {
                        if (_board[enemyRow, enemyCol + 1] == 4)
                        {
                            _board[enemyRow, enemyCol] = 0;
                            _enemy2 = false;
                        }
                        else if (_board[enemyRow, enemyCol + 1] == 1)
                        {
                            _board[enemyRow, enemyCol + 1] = 3;
                            _board[enemyRow, enemyCol] = 0;
                            _catch = false;
                        }
                        else if (_board[enemyRow, enemyCol] == 3 && _board[enemyRow, enemyCol + 1] != 2)
                        {
                            _board[enemyRow, enemyCol + 1] = 3;
                            _board[enemyRow, enemyCol] = 0;
                        }
                    }
                }
            }


            if (GameRefresh is not null)
            {
                GameRefresh(this, new GameRefreshEventArgs(_board));
            }

            checkGameOver();
        }

        public void checkGameOver()
        {
            if (!_enemy1 && !_enemy2)
            {
                _winner = "player";
            }
            else if (!_bomb)
            {
                _winner = "bomb";
            }

            else if (!_catch)
            {
                _winner = "catched";
            }

            if (!_winner.Equals("") && GameEnded is not null)
            {
                GameEnded(this, new GameEndedEventArgs(_winner, _size));
            }
        }

        public async Task SaveGame(string path)
        {
            await _dataAccess.SaveAsync(path, _size, _h, _m, _s, _board);
        }

        public async Task LoadGame(string path)
        {
            (_size, _h, _m, _s, _board) = await _dataAccess.LoadAsync(path);

            if (GameStarted is not null)
            {
                GameStarted(this, new GameStartedEventArgs(_size, _h, _m, _s, _board));
            }
        }

    }
}

