using Escape.Model;
using Escape.Persistence;
using Moq;
using System.Drawing;
using System.Reflection;
using System.Security.Policy;
using System.Windows.Forms;

namespace EscapeTest
{
    [TestClass]
    public class TestEscape
    {
        private EscapeModel _model;
        private Mock<IDataAccess> _mock;
        private int _mockedSize;
        private int[,] _mockedBoard;

        private bool _bomb;
        private bool _enemy1;
        private bool _enemy2;
        private bool _catch;

        private int _h = 0, _m = 0, _s = 0;

        [TestInitialize]
        public void Initialize()
        {
            Random mines = new Random();

            _mockedSize = 11;
            _mockedBoard = new int[_mockedSize, _mockedSize];
            for (int i = 0; i < _mockedSize; i++)
            {
                for (int j = 0; j < _mockedSize; j++)
                {
                    _mockedBoard[i, j] = 0;
                }
            }
            _mockedBoard[0, (_mockedSize - 1) / 2] = 1;
            _mockedBoard[(_mockedSize - 1), 0] = 2;
            _mockedBoard[(_mockedSize - 1), (_mockedSize - 1)] = 3;

           

            _mock = new Mock<IDataAccess>();
            _mock.Setup(mock => mock.LoadAsync(It.IsAny<string>()))
                .Returns(() => Task.FromResult((_mockedSize, _h, _m, _s, _mockedBoard)));
            _model = new EscapeModel(_mock.Object);

            _model.StartNewGame(11);
        }



        [TestMethod]
        public void StartNewGameBoardSize()
        {
            _model.StartNewGame(_mockedSize);

            Assert.AreEqual(_mockedBoard.Length, _model.Board.Length);
        }

        [TestMethod]
        public void StartNewGameBoardFields()
        {
            _model.StartNewGame(_mockedSize);
            for (int i = 0; i < _mockedSize; i++)
            {
                for (int j = 0; j < _mockedSize; j++)
                {
                    if (_model.Board[i,j] == 4)
                    {
                        _mockedBoard[i, j] = 4;
                    }
                    Assert.IsTrue(_mockedBoard[i, j] == _model.Board[i, j]);
                }
            }
        }

        [TestMethod]
        public void CheckGameOverIfPlayerWin()
        {
            _model.Winner = "";
            _model.Enemy1 = false;
            _model.Enemy2 = false;

            _model.checkGameOver();
            Assert.AreEqual("player", _model.Winner);
        }

        [TestMethod]
        public void CheckGameOverIfCatched()
        {
            _model.Winner = "";
            _model.Enemy1 = true;
            _model.Enemy2 = true;
            _model.Bomb = true;
            _model.Catch = false;

            _model.checkGameOver();
            Assert.AreEqual("catched", _model.Winner);
        }

        [TestMethod]
        public void CheckGameOverIfExplosed()
        {
            _model.Winner = "";
            _model.Enemy1 = true;
            _model.Enemy2 = true;
            _model.Catch = true;
            _model.Bomb = false;

            _model.checkGameOver();
            Assert.AreEqual("bomb", _model.Winner);
        }

        [TestMethod]
        public void PlayerMoveDown()
        {
            _model.movePlayer(0, (_model.Size - 1) / 2, Keys.Down, _model.Size);
            if (_model.Board[1, (_model.Size - 1) / 2] != 4)
            {
                Assert.AreEqual(_model.Board[1, (_model.Size - 1) / 2], 1);
                Assert.AreEqual(_model.Board[0, (_model.Size - 1) / 2], 0);
            }
            else
            {
                Assert.AreEqual(_model.Board[1, (_model.Size - 1) / 2], 4);
            }
        }

        [TestMethod]
        public void PlayerMoveUp()
        {
            _model.movePlayer(1, (_model.Size - 1) / 2, Keys.Up, _model.Size);
            if (_model.Board[0, (_model.Size - 1) / 2] != 4)
            {
                Assert.AreEqual(_model.Board[0, (_model.Size - 1) / 2], 1);
            }
        }

        [TestMethod]
        public void MovePLayerLeft()
        {
            _model.movePlayer(0, (_model.Size - 1) / 2, Keys.Left, _model.Size);
            if (_model.Board[0, ((_model.Size - 1) / 2 - 1)] != 4)
            {
                Assert.AreEqual(_model.Board[0, ((_model.Size - 1) / 2 - 1)], 1);
                Assert.AreEqual(_model.Board[0, ((_model.Size - 1) / 2)], 0);
            }
            else
            {
                Assert.AreEqual(_model.Board[0, ((_model.Size - 1) / 2 - 1)], 4);
            }
        }

        [TestMethod]
        public void MovePLayerRight()
        {
            _model.movePlayer(0, (_model.Size - 1) / 2, Keys.Right, _model.Size);
            if (_model.Board[0, ((_model.Size - 1) / 2 + 1)] != 4)
            {
                Assert.AreEqual(_model.Board[0, ((_model.Size - 1) / 2 + 1)], 1);
                Assert.AreEqual(_model.Board[0, ((_model.Size - 1) / 2)], 0);
            }
            else
            {
                Assert.AreEqual(_model.Board[0, ((_model.Size - 1) / 2 + 1)], 4);
            }
        }

        [TestMethod]
        public void Enemy1Move()
        {
            _model.moveEnemy1(0, (_model.Size - 1) / 2, (_model.Size - 1), 0);
            if (_model.Board[(_model.Size - 2), 0] != 4)
            {
                Assert.AreEqual(_model.Board[(_model.Size - 2), 0], 2);
            }
            else
            {
                Assert.AreEqual(_model.Board[(_model.Size - 2), 0], 4);
            }
        }

        [TestMethod]
        public void Enemy2Move()
        {
            _model.moveEnemy2(0, (_model.Size - 1) / 2, (_model.Size - 1), (_model.Size - 1));
            if (_model.Board[(_model.Size - 2), (_model.Size - 1)] != 4)
            {
                Assert.AreEqual(_model.Board[(_model.Size - 2), (_model.Size - 1)], 3);
            }
            else
            {
                Assert.AreEqual(_model.Board[(_model.Size - 2), (_model.Size - 1)], 4);
            }
        }
    }
}