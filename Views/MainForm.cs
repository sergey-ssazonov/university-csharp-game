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

        public MainForm(GameController controller)
        {
            Instance = this;
            _ctrl = controller;

            InitializeComponent();
            this.KeyPreview = true;
            // минималистичный пастельный фон
            this.BackColor = Color.FromArgb(240, 240, 250);
            DoubleBuffered = true;
            ClientSize = new Size(640, 480);
            Text = "Hardware Killer";

            // --- Меню ---
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
            // Закруглённые края
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
                _endPanel.Visible = false;
                _ctrl.StartGame();
            };
            _menuPanel.Controls.Add(_btnStart);
            Controls.Add(_menuPanel);

            // --- Панель конца игры ---
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
            // Закругление
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
                Invalidate();
            };
            _endPanel.Controls.AddRange(new Control[] { _lblResult, _lblScore, _btnRetry });
            Controls.Add(_endPanel);

            // --- Таймер ---
            _timer = new Timer { Interval = 30 };
            _timer.Tick += (s, e) =>
            {
                if (_ctrl.State == GameState.Playing)
                {
                    _ctrl.Update();
                    Invalidate();
                }
                else if (_ctrl.State == GameState.GameOver || _ctrl.State == GameState.Won)
                {
                    _timer.Stop();
                    _lblResult.Text = _ctrl.State == GameState.Won ? "You Won!" : "Game Over!";
                    _lblScore.Text = $"Score: {_ctrl.Score}";
                    _endPanel.BringToFront();
                    _endPanel.Visible = true;
                }
            };
            _timer.Start();

            // Подписка на нажатия
            // KeyDown += (s, e) => _ctrl.OnKeyDown(e.KeyCode);
            this.KeyDown += (s, e) => _ctrl.OnKeyDown(e.KeyCode);
        }


        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            _ctrl.Draw(e.Graphics);
        }
        
        protected override bool IsInputKey(Keys keyData)
        {
            // считаем стрелки «игровыми» клавишами
            if (keyData == Keys.Left ||
                keyData == Keys.Right ||
                keyData == Keys.Up ||
                keyData == Keys.Down)
                return true;
            return base.IsInputKey(keyData);
        }

    }
}