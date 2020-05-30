using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace CourseLabirint
{
    public class Game1 : Game
    {
        public struct Cells
        {
            public int X;
            public int Y;

            public Cells(int newX, int newY)
            {
                X = newX;
                Y = newY;
            }
        }
        private SpriteBatch _spriteBatch;
        private readonly Texture2D[,] _maze;
        private static Cells _size;
        private readonly Cells _start;
        private readonly Cells _finish;
        private int _shc;
        private int _fhc;
        private int _whc;
        private int _vhc;
        private int _vpath;
        private readonly List<Cells> _neighbours;
        private readonly Stack<Cells> _path;
        private readonly Queue<Cells> _dfs;
        private Random rand = new Random();
        private int _status;
        private int check_key=0;
        private int check_keyQ = 0;
        int winWidth;
        int winHeight;
        int[,] mat;
        int d;
        bool skipQ = false;
        public Game1(List<Cells> neighbours, Stack<Cells> path)
        {
            _neighbours = neighbours;
            _path = path;
            _dfs = new Queue<Cells>();
            var graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            winWidth = graphics.PreferredBackBufferWidth = 510;
             winHeight = graphics.PreferredBackBufferHeight = 510;
            _size = new Cells(winWidth / 10, winHeight / 10);
            _start = new Cells(1, 1);
            _finish = new Cells(_size.X - 2, _size.Y - 2);
            _maze = new Texture2D[_size.X, _size.Y];
            _path.Push(_start);
            _path.Push(_start);
            IsMouseVisible = true;
            mat = new int[_size.X, _size.Y];
        }

        protected override void Initialize()
        {
            base.Initialize();
        }
        protected override void UnloadContent()
        {

        }

        //Generation
        private void MainPoint()
        {
            _maze[_start.X, _start.Y] = Content.Load<Texture2D>("start");
            _shc = _maze[_start.X, _start.Y].GetHashCode();
            _maze[_finish.X, _finish.Y] = Content.Load<Texture2D>("finish");
            _fhc = _maze[_finish.X, _finish.Y].GetHashCode();
        }
        private void DrawMaze()
        {
            if (_path.Count != 0)
            {
                GetNeighbours(_path.Peek());
                if (_neighbours.Count != 0)
                {
                    var a = _neighbours[rand.Next(0, _neighbours.Count)];
                    rand.Next(-10, -1);
                    RemoteWall(_path.Peek(), a);
                    _path.Push(a);
                    _maze[_path.Peek().X, _path.Peek().Y] = Content.Load<Texture2D>("now");
                    _neighbours.Clear();
                }
                else
                {
                    _maze[_path.Peek().X, _path.Peek().Y] = Content.Load<Texture2D>("visited");
                    _path.Pop();
                    if (_path.Count <= 0) return;
                    _maze[_path.Peek().X, _path.Peek().Y] = Content.Load<Texture2D>("now");
                }
            }
            else
            {
                MainPoint();
                _status = 1;
                _dfs.Enqueue(_start);
                d = 1;
                mat[_start.X, _start.Y] = d;
                _neighbours.Clear();
                skipQ = false;
            }
        }

        private void BreakWalls()
        {
            var rand = new Random();
            for(int k=0;k<150;k++)
            {
                int x = rand.Next(1, _size.X-1);
                int y = rand.Next(1, _size.Y-1);
                if (_maze[x, y].GetHashCode() == _whc)
                {
                    var lul = _maze[x, y].GetHashCode();
                    _maze[x, y] = Content.Load<Texture2D>("visited");
                    var kek = _maze[x, y].GetHashCode();
                }
                else
                    k--;  
            }
            _status = 2;
        }
        private void GetNeighbours(Cells localcell)
        {
            var x = localcell.X;
            var y = localcell.Y;
            const int distance = 2;
            var d = new[]
            {
                new Cells(x, y - distance),
                new Cells(x + distance, y),
                new Cells(x, y + distance),
                new Cells(x - distance, y)
            };
            for (var i = 0; i < 4; i++)
            {
                var s = d[i];
                if (s.X <= 0 || s.X >= _size.X || s.Y <= 0 || s.Y >= _size.Y) continue;
                if (_maze[s.X, s.Y].GetHashCode() == _whc || _maze[s.X, s.Y].GetHashCode() == _vhc) continue;
                _neighbours.Add(s);
            }
        }
        private void RemoteWall(Cells first, Cells second)
        {
            var xDiff = second.X - first.X;
            var yDiff = second.Y - first.Y;
            Cells target;
            Cells newCells;
            var addX = (xDiff != 0) ? xDiff / Math.Abs(xDiff) : 0;
            var addY = (yDiff != 0) ? yDiff / Math.Abs(yDiff) : 0;
            target.X = first.X + addX;
            target.Y = first.Y + addY;
            _maze[target.X, target.Y] = Content.Load<Texture2D>("visited");
            _maze[_path.Peek().X, _path.Peek().Y] = Content.Load<Texture2D>("visited");
            _vhc = _maze[target.X, target.Y].GetHashCode();
            newCells.X = first.X + 2 * addX;
            newCells.Y = first.Y + 2 * addY;
            _path.Push(newCells);
            _maze[_path.Peek().X, _path.Peek().Y] = Content.Load<Texture2D>("visited");
        }
        //Generation

        //Draw Path
        private void Path()
        {
            if (_dfs.Peek().X == _finish.X && _dfs.Peek().Y == _finish.Y)
            {
                _neighbours.Clear();
                _status = 4;
                _dfs.Clear();
                _dfs.Enqueue(_finish);
                skipQ = false;
                return;
            }
            GetNeighboursPath(_dfs.Peek());
            if (_neighbours.Count != 0)
            {
                for (int i = 0; i < _neighbours.Count; i++)
                {
                    _dfs.Enqueue(_neighbours[i]);
                    _maze[_neighbours[i].X, _neighbours[i].Y] = Content.Load<Texture2D>("start");
                    mat[_neighbours[i].X, _neighbours[i].Y] = mat[_dfs.Peek().X,_dfs.Peek().Y] + 1;
                }
                _maze[_dfs.Peek().X, _dfs.Peek().Y] = Content.Load<Texture2D>("vpath");
                _vpath = _maze[_dfs.Peek().X, _dfs.Peek().Y].GetHashCode();
                _dfs.Dequeue();
                _neighbours.Clear();
            }
            else
            {
                _maze[_dfs.Peek().X, _dfs.Peek().Y] = Content.Load<Texture2D>("vpath");
                _dfs.Dequeue();
            }

        }
        private void GetNeighboursPath(Cells localCells)
        {
            var x = localCells.X;
            var y = localCells.Y;
            const int distance = 1;
            var d = new[]
            {
                new Cells(x, y - distance),
                new Cells(x + distance, y),
                new Cells(x, y + distance),
                new Cells(x - distance, y)
            };
            for (var i = 0; i < 4; i++)
            {
                var s = d[i];
                if (s.X <= 0 || s.X >= _size.X || s.Y <= 0 || s.Y >= _size.Y) continue;
                if (_maze[s.X, s.Y].GetHashCode() != _whc && _maze[s.X, s.Y].GetHashCode() != _shc && _maze[s.X, s.Y].GetHashCode() != _vpath)
                {
                    _neighbours.Add(s);
                }
            }
        }

        private void PathBack()
        {
            if (_dfs.Peek().X == _start.X && _dfs.Peek().Y == _start.Y)
            {
                _maze[_dfs.Peek().X, _dfs.Peek().Y] = Content.Load<Texture2D>("now");
                _status = 6;
                skipQ = false;
                return;
            }
            GetNeighboursPathBack(_dfs.Peek());
            _maze[_dfs.Peek().X,_dfs.Peek().Y] = Content.Load<Texture2D>("now");
            _dfs.Dequeue();
        }

        private void GetNeighboursPathBack(Cells localCells)
        {
            var x = localCells.X;
            var y = localCells.Y;
            const int distance = 1;
            var d = new[]
            {
                new Cells(x, y - distance),
                new Cells(x + distance, y),
                new Cells(x, y + distance),
                new Cells(x - distance, y)
            };
            for (var i = 0; i < 4; i++)
            {
                var s = d[i];
                if (s.X <= 0 || s.X >= _size.X || s.Y <= 0 || s.Y >= _size.Y) continue;
                if (mat[s.X, s.Y] + 1 == mat[x, y])
                {
                    _dfs.Enqueue(s);
                    return;
                }
            }
        }

        //Draw Path

        //Main Function
        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            for (var i = 0; i < _size.X; i++)
            {
                for (var j = 0; j < _size.Y; j++)
                {
                    if ((i % 2 != 0 && j % 2 != 0) && (i < _size.Y - 1 || j < _size.X - 1))
                    {
                        _maze[i, j] = Content.Load<Texture2D>("flat");
                    }
                    else
                    {
                        _maze[i, j] = Content.Load<Texture2D>("wall");
                        _whc = _maze[i, j].GetHashCode();
                    }
                }
            }

            GetNeighbours(_path.Peek());
            if (_neighbours.Count != 0)
            {
                var a = _neighbours[new Random().Next(0, _neighbours.Count)];
                RemoteWall(_path.Peek(), a);
                _path.Push(a);
                _maze[_path.Peek().X, _path.Peek().Y] = Content.Load<Texture2D>("now");
                _neighbours.Clear();
            }
            else
            {
                _maze[_path.Peek().X, _path.Peek().Y] = Content.Load<Texture2D>("visited");
                _path.Pop();
                if (_path.Count <= 0) return;
                _maze[_path.Peek().X, _path.Peek().Y] = Content.Load<Texture2D>("now");
            }
        }
        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                this.Exit();
            if (Keyboard.GetState().IsKeyDown(Keys.Space))
                check_key++;
            if (Keyboard.GetState().IsKeyUp(Keys.Space) && check_key > 0)
            {
                _status++;
                check_key = 0;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.F1))
                check_keyQ++;
            if (Keyboard.GetState().IsKeyUp(Keys.F1) && check_keyQ > 0)
            {
                skipQ=true;
                check_keyQ = 0;
            }
            switch (_status)
            {
                case 0:
                    Window.Title = "Draw Maze";
                    DrawMaze();
                    while (skipQ)
                        DrawMaze();
                    break;
                case 1:
                    BreakWalls();
                    break;
                case 3:
                    Window.Title = "Draw Water";
                    Path();
                    while (skipQ)
                        Path();
                    break;
                case 5:
                    Window.Title = "Draw Path";
                    PathBack();
                    while (skipQ)
                        PathBack();
                    break;
                case 6:
                    Window.Title = "Finished";
                    break;
            }
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);
            _spriteBatch.Begin();
                var matrix = new Matrix(_size, _maze);
                matrix.Draw(_spriteBatch);
            _spriteBatch.End();
            base.Draw(gameTime);
        }
        //Main Function
    }
}

