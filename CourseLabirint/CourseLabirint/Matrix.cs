using System;
using System.Linq.Expressions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace CourseLabirint
{
    class Matrix
    {
        private struct Size
        {
            public int Width { get; set; }
            public int Height { get; set; }
        }
        readonly Size _size;
        readonly Texture2D[,] _maze;
        public Matrix(Game1.Cells size, Texture2D[,] cell2D)
        {
            _maze = cell2D;
            _size.Width = size.X;
            _size.Height = size.Y;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            for (var i = 0; i < _size.Width; i++)
            {
                for (var j = 0; j < _size.Height; j++)
                {
                    if ((i % 2 != 0 && j % 2 != 0) && (i < _size.Height - 1 && j < _size.Width - 1))
                    {
                        spriteBatch.Draw(_maze[i, j], new Rectangle(i * 10, j * 10, 10, 10), Color.White);
                    }
                    else
                    {
                        spriteBatch.Draw(_maze[i, j], new Rectangle(i * 10, j * 10, 10, 10), Color.White);
                    }
                }
            }
        }
    }
}
