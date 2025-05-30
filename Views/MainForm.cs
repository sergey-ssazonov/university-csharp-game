// Views/MainForm.cs

using System;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using HardwareKiller.Controllers;
using Timer = System.Windows.Forms.Timer;

namespace HardwareKiller.Views
{
    public partial class MainForm : Form
    {
        public static MainForm Instance;
        private readonly Timer _timer;
        private GameController _ctrl;

        private Panel _menuPanel, _endPanel;
        private Button _btnStart, _btnRetry;
        private Label _lblScore, _lblResult;
        
        private Panel _infoPanel;
        private Label _scoreLabel;


        public MainForm(GameController controller)
        {
            Instance = this;
            _ctrl = controller;

            InitializeComponent();
            this.KeyPreview = true;
            this.KeyDown += (s, e) => _ctrl.OnKeyDown(e.KeyCode);
            this.KeyUp += (s, e) => _ctrl.OnKeyUp(e.KeyCode);

            this.BackColor = Color.FromArgb(240, 240, 250);
            DoubleBuffered = true;
            ClientSize = new Size(640, 480);
            Text = "Hardware Killer";
            
            // Информационная панель сверху
            _infoPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 30,
                BackColor = Color.LightGray
            };
            _scoreLabel = new Label
            {
                Text = "Score: 0",
                AutoSize = true,
                Location = new Point(10, 5),
                Font = new Font("Segoe UI", 10, FontStyle.Regular)
            };
            _infoPanel.Controls.Add(_scoreLabel);
            Controls.Add(_infoPanel);


            _menuPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = this.BackColor
            };
            _btnStart = new Button
            {
                Text = "Start Game",
                Width = 120,
                Height = 40,
                Location = new Point((ClientSize.Width - 120) / 2, (ClientSize.Height - 40) / 2),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.CornflowerBlue,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 12, FontStyle.Regular)
            };
            _btnStart.FlatAppearance.BorderSize = 0;
            {
                var gp = new System.Drawing.Drawing2D.GraphicsPath();
                int r = 10;
                gp.AddArc(0, 0, r, r, 180, 90);
                gp.AddArc(_btnStart.Width - r, 0, r, r, 270, 90);
                gp.AddArc(_btnStart.Width - r, _btnStart.Height - r, r, r, 0, 90);
                gp.AddArc(0, _btnStart.Height - r, r, r, 90, 90);
                gp.CloseFigure();
                _btnStart.Region = new Region(gp);
            }
            _btnStart.Click += (s, e) =>
            {
                _menuPanel.Visible = false;
                _ctrl.StartGame();
                _timer.Start(); 
            };
            _menuPanel.Controls.Add(_btnStart);
            Controls.Add(_menuPanel);

            _endPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = this.BackColor,
                Visible = false
            };
            _lblResult = new Label
            {
                AutoSize = true,
                ForeColor = Color.Black,
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                Location = new Point((ClientSize.Width - 200) / 2, ClientSize.Height / 2 - 60)
            };
            _lblScore = new Label
            {
                AutoSize = true,
                ForeColor = Color.Black,
                Font = new Font("Segoe UI", 14, FontStyle.Regular),
                Location = new Point((ClientSize.Width - 200) / 2, ClientSize.Height / 2 - 20)
            };
            _btnRetry = new Button
            {
                Text = "Play Again",
                Width = 120,
                Height = 40,
                Location = new Point((ClientSize.Width - 120) / 2, ClientSize.Height / 2 + 20),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.CornflowerBlue,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 12, FontStyle.Regular)
            };
            _btnRetry.FlatAppearance.BorderSize = 0;
            {
                var gp = new System.Drawing.Drawing2D.GraphicsPath();
                int r = 10;
                gp.AddArc(0, 0, r, r, 180, 90);
                gp.AddArc(_btnRetry.Width - r, 0, r, r, 270, 90);
                gp.AddArc(_btnRetry.Width - r, _btnRetry.Height - r, r, r, 0, 90);
                gp.AddArc(0, _btnRetry.Height - r, r, r, 90, 90);
                gp.CloseFigure();
                _btnRetry.Region = new Region(gp);
            }
            _btnRetry.Click += (s, e) =>
            {
                _endPanel.Visible = false;
                _ctrl.RestartGame();
                _timer.Start(); 
            };
            _endPanel.Controls.AddRange(new Control[] { _lblResult, _lblScore, _btnRetry });
            _endPanel.BackgroundImage = Image.FromFile("Images\\lose.png");
            _endPanel.BackgroundImageLayout = ImageLayout.Stretch;
            Controls.Add(_endPanel);

            _timer = new Timer { Interval = 10 };
            _timer.Tick += (s,e) =>
            {
                if (_ctrl.State == GameState.Playing)
                {
                    _ctrl.Update();
                    _scoreLabel.Text = $"Score: {_ctrl.Score}";
                    Invalidate();
                }
                else
                {
                    _timer.Stop();
                    if (_ctrl.State == GameState.GameOver)
                        _endPanel.BackgroundImage = Image.FromFile("Images\\lose.png");
                    else // Won
                        _endPanel.BackgroundImage = null;
                    _lblResult.Text = _ctrl.State == GameState.Won ? "You Won!" : "Game Over!";
                    _lblScore.Text  = $"Score: {_ctrl.Score}";
                    _endPanel.BringToFront();
                    _endPanel.Visible = true;
                }
            };

            _timer.Start();
        }


        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            e.Graphics.TranslateTransform(0, _infoPanel.Height);
            _ctrl.Draw(e.Graphics);
            e.Graphics.ResetTransform();
        }

        protected override bool IsInputKey(Keys keyData)
        {
            if (keyData == Keys.Up || keyData == Keys.Down
                                   || keyData == Keys.Left || keyData == Keys.Right)
                return true;
            return base.IsInputKey(keyData);
        }
    }
}