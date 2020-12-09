﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System.Diagnostics;
using LBMG.Tools;
using MonoGame.Extended.Tiled;

namespace LBMG.Player
{
    public class CharacterDrawer
    {
        public List<Character> Characters { get; set; }

        private readonly List<string> _texturePaths;
        private readonly List<Texture2D> _textures;
        private List<Rectangle> _rectangles;
        private int _activePlayer;
        private Vector2 _playerPos;
        const float TileSize = Constants.TileSize;
        private double _counter;
        private SpriteBatch _sb;

        public CharacterDrawer(List<Character> characters, List<string> paths, List<Rectangle> rectangles)
        {
            Characters = characters;
            _texturePaths = paths;
            _rectangles = rectangles;
            _textures = new List<Texture2D>();
        }

        public void Initialize(GraphicsDevice gd, ContentManager cm, GameWindow window)
        {
            _playerPos.X = window.ClientBounds.Width / 2;
            _playerPos.Y = window.ClientBounds.Height / 2;

            _activePlayer = 0;
            _counter = TileSize;
            _sb = new SpriteBatch(gd);

            foreach (string path in _texturePaths)
            {
                Texture2D text = cm.Load<Texture2D>(path);
                _textures.Add(text);
            }
        }

        public void Update(GameTime gameTime/*, Camera<Vector2> camera*/)
        {
            AnimateSprite();
            MoveToCase(gameTime);
            if (_counter <= 0)
            {
                Characters[_activePlayer].IsMoving = false;
                Characters[_activePlayer].Move();
                //Debug.WriteLine("x : " + Characters[_activePlayer].Position.X + " y : " + Characters[_activePlayer].Position.Y);
                _counter = TileSize;
            }
        }

        public void Draw(GameTime gameTime/*, Matrix transformMatrix*/)
        {
            _sb.Begin(samplerState: SamplerState.PointClamp);

            _sb.Draw(_textures[_activePlayer], _playerPos, _rectangles[_activePlayer], Color.White, default,
                new Vector2((float) _rectangles[_activePlayer].Width / 2, (float) _rectangles[_activePlayer].Height / 2), 1, default,
                default);

            _sb.End();
        }

        public void SetActivePlayer(int val)
        {
            _activePlayer = val;
        }

        private void AnimateSprite()
        {
            Rectangle rect = _rectangles[_activePlayer];
            Direction dir = Characters[_activePlayer].Direction;

            rect.Y = GetYRectVal(dir);
            rect.X = GetXRectVal();
            rect.Size = new Point(50, AdjustSizeY(dir));
            _rectangles[_activePlayer] = rect;
        }

        private int GetYRectVal(Direction dir)
            => dir switch
            {
                Direction.Left => 72,
                Direction.Top => 216,
                Direction.Right => 144,
                Direction.Bottom => 0,
                _ => throw new ArgumentOutOfRangeException(nameof(dir), dir, null)
            };

        private int AdjustSizeY(Direction dir)
            => dir switch
            {
                Direction.Left => 69,
                Direction.Top => 60,
                Direction.Right => 69,
                Direction.Bottom => 69,
                _ => throw new ArgumentOutOfRangeException(nameof(dir), dir, null)
            };

        private int GetXRectVal()
        {
            if (Characters[_activePlayer].IsMoving == false)
                return 50;
            if (_counter <= TileSize && _counter > TileSize - TileSize / 4)
                return -2;
            if (_counter <= TileSize - TileSize / 4 && _counter > TileSize - 2 * TileSize / 4)
                return 50;
            if (_counter <= TileSize - 2 * TileSize / 4 && _counter > TileSize - 3 * TileSize / 4)
                return 102;
            if (_counter <= TileSize - 3 * TileSize / 4)
                return 50;
            return -2;
        }

        private void MoveToCase(GameTime gameTime)
        {
            if (Characters[_activePlayer].IsMoving == true) // TODO Review this calculation
            {
                _counter -= (Characters[_activePlayer].Speed * gameTime.ElapsedGameTime.TotalSeconds);
                Debug.WriteLine(_counter);
            }
        }
    }
}
