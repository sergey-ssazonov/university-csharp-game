using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using HardwareKiller.Models;

namespace HardwareKiller.Controllers
{
    public enum GameState { Menu, Playing, GameOver, Won }

    public class GameController
    {
        private Game _game;
        public GameState State { get; private set; } = GameState.Menu;
        private int _initialItemCount;
        public int Score => _initialItemCount - _game.Items.Count;

        // Images
        private readonly Image _bgImg;
        private readonly Image _cursorImg;
        private readonly Image _catImg;
        private readonly Image _catImgLeft;
        private readonly Image _catImgUp;
        private readonly Image _catImgDown;
        private readonly Dictionary<ItemType, Image> _itemImgs;
        private readonly Dictionary<HazardType, Image> _hazardImgs;

        // Animation & movement (cell-based)
        private int _startCellX, _startCellY;
        private int _targetCellX, _targetCellY;
        private float _animOffsetX, _animOffsetY;
        private bool _isMoving;
        private const float StepPerTick = 32f / 2;  // клетка за 2 тика

        public GameController()
        {
            _game = new Game();
            _initialItemCount = _game.Items.Count;

            // Load images
            _bgImg      = Image.FromFile("Images\\windows-bg.jpg");
            _cursorImg  = Image.FromFile("Images\\cursor.png");
            _catImg     = Image.FromFile("Images\\cat.png");
            _catImgLeft = (Image)_catImg.Clone();     _catImgLeft.RotateFlip(RotateFlipType.RotateNoneFlipX);
            _catImgUp   = (Image)_catImg.Clone();     _catImgUp.RotateFlip(RotateFlipType.Rotate270FlipNone);
            _catImgDown = (Image)_catImg.Clone();     _catImgDown.RotateFlip(RotateFlipType.Rotate90FlipNone);

            _itemImgs = new Dictionary<ItemType, Image>
            {
                [ItemType.File]   = Image.FromFile("Images\\file.png"),
                [ItemType.Folder] = Image.FromFile("Images\\folder.png"),
                // [ItemType.Custom] = Image.FromFile("Images\\custom.png")
            };
            _hazardImgs = new Dictionary<HazardType, Image>
            {
                [HazardType.Shootdown]   = Image.FromFile("Images\\shootdown.png"),
                [HazardType.TrashBox]    = Image.FromFile("Images\\trash-box.png"),
                [HazardType.BlueWindows] = Image.FromFile("Images\\blue-windows.png"),
                [HazardType.BurnedPixel] = null
            };

            // Initialize animation to player's start cell
            _startCellX = _targetCellX = _game.Player.X;
            _startCellY = _targetCellY = _game.Player.Y;
            _animOffsetX = _animOffsetY = 0;
            _isMoving = false;
        }

        public void StartGame()
        {
            State = GameState.Playing;
        }

        public void RestartGame()
        {
            _game = new Game();
            _initialItemCount = _game.Items.Count;
            _startCellX = _targetCellX = _game.Player.X;
            _startCellY = _targetCellY = _game.Player.Y;
            _animOffsetX = _animOffsetY = 0;
            _isMoving = false;
            State = GameState.Playing;
        }

        public void OnKeyDown(Keys key)
        {
            if (State != GameState.Playing || _isMoving) return;

            switch (key)
            {
                case Keys.W:
                case Keys.Up:
                    StartMove(0, -1);
                    break;
                case Keys.S:
                case Keys.Down:
                    StartMove(0, 1);
                    break;
                case Keys.A:
                case Keys.Left:
                    StartMove(-1, 0);
                    break;
                case Keys.D:
                case Keys.Right:
                    StartMove(1, 0);
                    break;
            }
        }

        public void Update()
        {
            if (State != GameState.Playing) return;

            if (_isMoving)
            {
                float dirX = Math.Sign(_targetCellX - _startCellX);
                float dirY = Math.Sign(_targetCellY - _startCellY);

                _animOffsetX += dirX * StepPerTick;
                _animOffsetY += dirY * StepPerTick;

                if (Math.Abs(_animOffsetX) >= _game.CellSize ||
                    Math.Abs(_animOffsetY) >= _game.CellSize)
                {
                    // Finish animation
                    _startCellX = _targetCellX;
                    _startCellY = _targetCellY;
                    _game.MovePlayer((int)dirX, (int)dirY);
                    _animOffsetX = _animOffsetY = 0;
                    _isMoving = false;
                    CheckCollisions();
                }
            }
        }

        private void CheckCollisions()
        {
            // Collect items
            var hitItem = _game.Items.FirstOrDefault(i => i.X == _game.Player.X && i.Y == _game.Player.Y);
            if (hitItem != null) _game.Items.Remove(hitItem);

            // Hit hazard → game over
            if (_game.Hazards.Any(h => h.X == _game.Player.X && h.Y == _game.Player.Y))
                State = GameState.GameOver;

            // All items collected → win
            if (!_game.Items.Any())
                State = GameState.Won;
        }

        public void Draw(Graphics g)
        {
            int cs = _game.CellSize;

            // Background
            var clip = g.VisibleClipBounds;
            g.DrawImage(_bgImg, new Rectangle(0, 0, (int)clip.Width, (int)clip.Height));

            // Burned pixels (full cell)
            foreach (var o in _game.Obstacles)
                g.FillRectangle(Brushes.Black, o.X * cs, o.Y * cs, cs, cs);

            // Items
            foreach (var it in _game.Items)
                g.DrawImage(_itemImgs[it.Type], it.X * cs, it.Y * cs, cs, cs);

            // Hazards
            foreach (var hz in _game.Hazards)
            {
                var img = _hazardImgs[hz.Type];
                if (img != null)
                    g.DrawImage(img, hz.X * cs, hz.Y * cs, cs, cs);
                else
                    g.FillRectangle(Brushes.Red, hz.X * cs, hz.Y * cs, cs, cs);
            }

            // Cats
            int catSize = (int)(cs * 1.4f);
            int catOff = (catSize - cs) / 2;
            foreach (var cat in _game.Cats)
            {
                int dx = cat.X - cat.PrevX;
                int dy = cat.Y - cat.PrevY;
                Image cimg = _catImg;
                if (dx < 0)      cimg = _catImgLeft;
                else if (dy < 0) cimg = _catImgUp;
                else if (dy > 0) cimg = _catImgDown;

                g.DrawImage(cimg,
                    cat.X * cs - catOff,
                    cat.Y * cs - catOff,
                    catSize,
                    catSize);
            }

            // Cursor (cell-animation)
            float px = _startCellX * cs + _animOffsetX;
            float py = _startCellY * cs + _animOffsetY;
            g.DrawImage(_cursorImg, (int)px, (int)py, cs, cs);
        }

        private void StartMove(int dx, int dy)
        {
            int tx = _startCellX + dx;
            int ty = _startCellY + dy;
            if (tx < 0 || tx >= _game.GridWidth || ty < 0 || ty >= _game.GridHeight)
                return;
            if (_game.Obstacles.Any(o => o.X == tx && o.Y == ty))
                return;

            _targetCellX = tx;
            _targetCellY = ty;
            _animOffsetX = _animOffsetY = 0;
            _isMoving = true;
        }
    }
}
