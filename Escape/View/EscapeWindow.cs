using Escape.Model;
using Escape.Persistence;
using System.Diagnostics;
using Timer = System.Windows.Forms.Timer;


namespace Escape.View
{
    public partial class EscapeWindow : Form
    {
        private EscapeModel _model;
        private int _size;
        private int[,] _board;
        private bool _isStarted = false;

        private Timer _timer;
        private int _playerMove = 3;


        public EscapeWindow()
        {
            InitializeComponent();

            

            _size = 0;
            _board = new int[_size, _size];


            _timer = new Timer();
            _timer.Interval = 1000;

            _model = new EscapeModel(new DataAccess());
            _model.GameStarted += onGameStarted;
            _model.GameRefresh += onGameRefresh;
            _model.GameEnded += onGameEnded;
            _timer.Tick += onLabelTimerTicked;
            

            _model.StartNewGame(15);
        }

        private void onGameStarted(object? sender, EventArguments.GameStartedEventArgs e)
        {
            
            _size = e.BoardSize;
            _board = e.Board;

            buttonTableLayoutPanel.RowCount = _size + 1;
            buttonTableLayoutPanel.ColumnCount = _size + 1;
            buttonTableLayoutPanel.Controls.Clear();

            buttonTableLayoutPanel.RowStyles.Clear();
            buttonTableLayoutPanel.ColumnStyles.Clear();

            for(int i = 0; i < _size; i++)
            {
                buttonTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 1 / Convert.ToSingle(_size)));
                buttonTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 1 / Convert.ToSingle(_size)));
            }

            for (int i = 0; i <_size; i++)
            {
                for (int j = 0; j < _size; j++)
                {
                    Button b = new Button();
                    b.AutoSize = true;
                    b.Dock = DockStyle.Fill;
                    b.Margin = new Padding(0, 0, 0, 0);

                    SetButton(b, _board[i, j]);

                    buttonTableLayoutPanel.Controls.Add(b, j, i);
                }
            }

            startGameToolStripMenuItem.Enabled = true;
            continueToolStripMenuItem.Enabled = false;
            pauseToolStripMenuItem.Enabled = false;
            saveGameToolStripMenuItem.Enabled = true;
            loadGameToolStripMenuItem.Enabled = true;
        }


        private void onGameRefresh(object? sender, EventArguments.GameRefreshEventArgs e)
        {
            _board = e.Board;
            for (int i = 0; i < _size; i++)
            {
                for (int j = 0; j < _size; j++)
                {
                    Button b = buttonTableLayoutPanel.GetControlFromPosition(j, i) as Button;
                    SetButton(b, _board[i, j]);
                }
            }
        }


        private void onGameEnded(object? sender, EventArguments.GameEndedEventArgs e)
        {
            _timer.Stop();
            if (e.Winner.Equals("player"))
            {
                MessageBox.Show
                 (
                     "Congratulations, You win!" + Environment.NewLine + "Your time: " + String.Format("{0}:{1}:{2}", _model.Hour.ToString().PadLeft(2, '0'), _model.Minute.ToString().PadLeft(2, '0'), _model.Second.ToString().PadLeft(2, '0')),
                     "Game Over",
                     MessageBoxButtons.OK,
                     MessageBoxIcon.Information
                 );
            }

            else if (e.Winner.Equals("bomb"))
            {
                MessageBox.Show
                 (
                     "You stepped on a bomb, You Lost!" + Environment.NewLine + "Your time: " + String.Format("{0}:{1}:{2}", _model.Hour.ToString().PadLeft(2, '0'), _model.Minute.ToString().PadLeft(2, '0'), _model.Second.ToString().PadLeft(2, '0')),
                     "Game Over",
                     MessageBoxButtons.OK,
                     MessageBoxIcon.Information
                 );
            }

            else if (e.Winner.Equals("catched"))
            {
                MessageBox.Show
                 (
                     "Enemy catched you, You Lost!" + Environment.NewLine + "Your time: " + String.Format("{0}:{1}:{2}", _model.Hour.ToString().PadLeft(2, '0'), _model.Minute.ToString().PadLeft(2, '0'), _model.Second.ToString().PadLeft(2, '0')),
                     "Game Over",
                     MessageBoxButtons.OK,
                     MessageBoxIcon.Information
                 );
            }

            toolStripStatusLabel1.Text = "00:00:00";

            _model.StartNewGame(15);
        }


        private void onLabelTimerTicked(object? sender, EventArgs e)
        {
            _model.Second++;
            if (_model.Second == 60) { _model.Minute++; _model.Second = 0; }
            if (_model.Minute == 60) { _model.Hour++; _model.Minute = 0; }

            toolStripStatusLabel1.Text = String.Format("{0}:{1}:{2}", _model.Hour.ToString().PadLeft(2, '0'), _model.Minute.ToString().PadLeft(2, '0'), _model.Second.ToString().PadLeft(2, '0'));

            int playerRow = 0;
            int playerCol = 0;

            int enemy1Row = 0;
            int enemy1Col = 0;

            int enemy2Row = 0;
            int enemy2Col = 0;

            for (int i = 0; i < _size; i++)
            {
                for (int j = 0; j < _size; j++)
                {
                    if (_board[i, j] == 1)
                    {
                        playerRow = i;
                        playerCol = j;
                    }
                    else if (_board[i, j] == 2)
                    {
                        enemy1Row = i;
                        enemy1Col = j;
                    }
                    else if (_board[i, j] == 3)
                    {
                        enemy2Row = i;
                        enemy2Col = j;
                    }
                }
            }
            _playerMove++;
            if (_model.Second % 2 == 0) { _model.moveEnemy1(playerRow, playerCol, enemy1Row, enemy1Col);}
            if (_model.Second % 2 == 1 && _model.Second > 1) { _model.moveEnemy2(playerRow, playerCol, enemy2Row, enemy2Col); }

        }


        private void SetButton(Button b, int value)
        {
            switch(value)
            {
                case 0:
                    b.BackColor = Color.White;
                    b.Enabled = false; 
                    break;
                case 1:
                    b.BackColor = Color.Green;
                    b.Enabled = false;
                    break;
                case 2:
                    b.BackColor = Color.Red;
                    b.Enabled = false;
                    break;
                case 3:
                    b.BackColor = Color.Red;
                    b.Enabled = false;
                    break;
                case 4:
                    b.BackColor = Color.Black;
                    b.Enabled = false;
                    break;
            }
        }

        private void easyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _model.StartNewGame(11);
            _model.Hour = 0;
            _model.Minute = 0;
            _model.Second = 0;
        }

        private void mediumToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _model.StartNewGame(15);
            _model.Hour = 0;
            _model.Minute = 0;
            _model.Second = 0;
        }

        private void hardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _model.StartNewGame(21);
            _model.Hour = 0;
            _model.Minute = 0;
            _model.Second = 0;
        }


        private void keyDown(object sender, KeyEventArgs e)
        {
            
            int _playerRow = 0;
            int _playerCol = 0;

            for (int i = 0; i < _size; i++)
            {
                for (int j = 0; j < _size; j++)
                {
                    if (_board[i, j] == 1)
                    {
                        _playerRow = i;
                        _playerCol = j;
                    }
                }
            }
            if (_isStarted && _playerMove % 2 == 0)
            {
                switch (e.KeyCode)
                {
                    case Keys.Up:
                        _model.movePlayer(_playerRow, _playerCol, Keys.Up, _size); _playerMove--; break;
                    case Keys.Down:
                        _model.movePlayer(_playerRow, _playerCol, Keys.Down, _size); _playerMove--; break;
                    case Keys.Left:
                        _model.movePlayer(_playerRow, _playerCol, Keys.Left, _size); _playerMove--; break;
                    case Keys.Right:
                        _model.movePlayer(_playerRow, _playerCol, Keys.Right, _size); _playerMove--; break;
                }
            }
        }

        private void startGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _timer.Start();
            _isStarted = true;


            startGameToolStripMenuItem.Enabled = false;
            pauseToolStripMenuItem.Enabled = true;
            saveGameToolStripMenuItem.Enabled = false;
            loadGameToolStripMenuItem.Enabled = false;
        }


        private void continueToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _timer.Start();
            _isStarted = true;

            continueToolStripMenuItem.Enabled = false;
            pauseToolStripMenuItem.Enabled = true;
            saveGameToolStripMenuItem.Enabled = false;
            loadGameToolStripMenuItem.Enabled = false;
        }

        private void pauseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _timer.Stop();
            _isStarted = false;

            continueToolStripMenuItem.Enabled = true;
            pauseToolStripMenuItem.Enabled = false;
            saveGameToolStripMenuItem.Enabled = true;
            loadGameToolStripMenuItem.Enabled = true;
        }
        
        
        private async void saveGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "txt files (*.txt)|.txt";
            DialogResult result = saveFileDialog.ShowDialog();

            if (result == DialogResult.OK)
            {
                string path = saveFileDialog.FileName;
                try
                {
                    await _model.SaveGame(path);
                }
                catch (DataExceptions)
                {
                    MessageBox.Show
                        (
                            "Error while saving game!",
                            "Error",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error
                        );
                }
            }
        }

        private async void loadGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            DialogResult result = openFileDialog.ShowDialog();

            if (result == DialogResult.OK)
            {
                string path = openFileDialog.FileName;
                try
                {
                    await _model.LoadGame(path);
                    toolStripStatusLabel1.Text = String.Format("{0}:{1}:{2}", _model.Hour.ToString().PadLeft(2, '0'), _model.Minute.ToString().PadLeft(2, '0'), _model.Second.ToString().PadLeft(2, '0'));
                    startGameToolStripMenuItem.Enabled = false;
                    continueToolStripMenuItem.Enabled = true;
                }
                catch (DataExceptions)
                {
                    MessageBox.Show
                        (
                            "Error while loading game!",
                            "Error",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error
                        );
                }
            }
        }
    }
}