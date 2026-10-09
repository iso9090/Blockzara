using System;
using System.Collections.Generic;

namespace Blockzara.Core
{
    public readonly struct BoardCell : IEquatable<BoardCell>
    {
        public readonly int X;
        public readonly int Y;
        public BoardCell(int x, int y) { X = x; Y = y; }
        public bool Equals(BoardCell other) => X == other.X && Y == other.Y;
        public override bool Equals(object obj) => obj is BoardCell other && Equals(other);
        public override int GetHashCode() => X * 31 + Y;
    }

    public enum Tetromino { I = 0, O = 1, J = 2, L = 3, S = 4, Z = 5, T = 6 }

    public enum Rotation { Zero = 0, Right = 1, Two = 2, Left = 3 }

    public interface IRandomSource { int Next(int max); }

    public sealed class SeededRandom : IRandomSource
    {
        readonly Random random;
        public SeededRandom(int seed) { random = new Random(seed); }
        public int Next(int max) => random.Next(max);
    }

    public sealed class BoardState
    {
        public const int Width = 10;
        public const int VisibleRows = 20;
        public const int HiddenRows = 2;
        public const int MinY = -2;
        public const int MaxY = 19;
        readonly int[,] kinds = new int[Width, VisibleRows + HiddenRows];

        public bool IsInside(int x, int y) => x >= 0 && x < Width && y >= MinY && y <= MaxY;
        public bool IsOccupied(int x, int y) => IsInside(x, y) && kinds[x, y - MinY] != 0;
        public int Get(int x, int y) => IsInside(x, y) ? kinds[x, y - MinY] : 0;

        public void Set(int x, int y, int kind)
        {
            if (IsInside(x, y)) kinds[x, y - MinY] = kind;
        }

        public void ClearAll()
        {
            for (var y = MinY; y <= MaxY; y++)
            for (var x = 0; x < Width; x++)
                kinds[x, y - MinY] = 0;
        }

        public bool IsVisibleRowFull(int y)
        {
            if (y < 0 || y > MaxY) return false;
            for (var x = 0; x < Width; x++)
                if (!IsOccupied(x, y)) return false;
            return true;
        }

        public int ClearVisibleRows()
        {
            var cleared = 0;
            for (var y = MaxY; y >= 0; y--)
            {
                if (!IsVisibleRowFull(y)) continue;
                for (var source = y - 1; source >= MinY; source--)
                    CopyRow(source, source + 1);
                Erase(MinY);
                cleared++;
                y++;
            }
            return cleared;
        }

        void CopyRow(int fromY, int toY)
        {
            for (var x = 0; x < Width; x++)
                kinds[x, toY - MinY] = kinds[x, fromY - MinY];
        }

        void Erase(int y)
        {
            for (var x = 0; x < Width; x++)
                kinds[x, y - MinY] = 0;
        }
    }

    public static class Tetrominoes
    {
        static readonly BoardCell[][][] states = Build();

        public static IReadOnlyList<BoardCell> Cells(Tetromino type, Rotation rotation) => states[(int)type][(int)rotation];

        public static int SpawnX(Tetromino type) => type == Tetromino.O ? 4 : 3;

        public static int SpawnY(Tetromino type)
        {
            var min = int.MaxValue;
            foreach (var cell in Cells(type, Rotation.Zero))
                if (cell.Y < min) min = cell.Y;
            return -1 - min;
        }

        static BoardCell[][][] Build()
        {
            var all = new BoardCell[7][][];
            all[(int)Tetromino.T] = Around(new[] { new BoardCell(1, 0), new BoardCell(0, 1), new BoardCell(1, 1), new BoardCell(2, 1) });
            all[(int)Tetromino.J] = Around(new[] { new BoardCell(0, 0), new BoardCell(0, 1), new BoardCell(1, 1), new BoardCell(2, 1) });
            all[(int)Tetromino.L] = Around(new[] { new BoardCell(2, 0), new BoardCell(0, 1), new BoardCell(1, 1), new BoardCell(2, 1) });
            all[(int)Tetromino.S] = Around(new[] { new BoardCell(1, 0), new BoardCell(2, 0), new BoardCell(0, 1), new BoardCell(1, 1) });
            all[(int)Tetromino.Z] = Around(new[] { new BoardCell(0, 0), new BoardCell(1, 0), new BoardCell(1, 1), new BoardCell(2, 1) });
            var square = new[] { new BoardCell(0, 0), new BoardCell(1, 0), new BoardCell(0, 1), new BoardCell(1, 1) };
            all[(int)Tetromino.O] = new[] { square, square, square, square };
            all[(int)Tetromino.I] = new[]
            {
                new[] { new BoardCell(0, 1), new BoardCell(1, 1), new BoardCell(2, 1), new BoardCell(3, 1) },
                new[] { new BoardCell(2, 0), new BoardCell(2, 1), new BoardCell(2, 2), new BoardCell(2, 3) },
                new[] { new BoardCell(0, 2), new BoardCell(1, 2), new BoardCell(2, 2), new BoardCell(3, 2) },
                new[] { new BoardCell(1, 0), new BoardCell(1, 1), new BoardCell(1, 2), new BoardCell(1, 3) }
            };
            return all;
        }

        static BoardCell[][] Around(BoardCell[] spawn)
        {
            var result = new BoardCell[4][];
            result[0] = spawn;
            for (var i = 1; i < 4; i++) result[i] = Clockwise(result[i - 1]);
            return result;
        }

        static BoardCell[] Clockwise(BoardCell[] cells)
        {
            var next = new BoardCell[cells.Length];
            for (var i = 0; i < cells.Length; i++)
            {
                var dx = cells[i].X - 1;
                var dy = cells[i].Y - 1;
                next[i] = new BoardCell(1 - dy, 1 + dx);
            }
            return next;
        }
    }

    public static class WallKicks
    {
        static readonly int[,] Jlstz =
        {
            { 0, 0, -1, 0, -1, -1, 0, 2, -1, 2 },
            { 0, 0, 1, 0, 1, 1, 0, -2, 1, -2 },
            { 0, 0, 1, 0, 1, -1, 0, 2, 1, 2 },
            { 0, 0, -1, 0, -1, 1, 0, -2, -1, -2 }
        };

        static readonly int[,] IPiece =
        {
            { 0, 0, -2, 0, 1, 0, -2, 1, 1, -2 },
            { 0, 0, -1, 0, 2, 0, -1, -2, 2, 1 },
            { 0, 0, 2, 0, -1, 0, 2, -1, -1, 2 },
            { 0, 0, 1, 0, -2, 0, 1, 2, -2, -1 }
        };

        public static bool Try(Tetromino type, Rotation from, bool clockwise, int originX, int originY, Func<Tetromino, Rotation, int, int, bool> fits, out Rotation rotation, out int x, out int y, out int tryIndex)
        {
            rotation = from;
            x = originX;
            y = originY;
            tryIndex = 0;
            if (type == Tetromino.O) return false;

            var to = clockwise ? (Rotation)(((int)from + 1) % 4) : (Rotation)(((int)from + 3) % 4);
            var column = clockwise ? (int)from : ((int)from + 3) % 4;
            var sign = clockwise ? 1 : -1;
            var table = type == Tetromino.I ? IPiece : Jlstz;
            for (var attempt = 0; attempt < 5; attempt++)
            {
                var kickX = originX + sign * table[column, attempt * 2];
                var kickY = originY + sign * table[column, attempt * 2 + 1];
                if (!fits(type, to, kickX, kickY)) continue;
                rotation = to;
                x = kickX;
                y = kickY;
                tryIndex = attempt + 1;
                return true;
            }
            return false;
        }
    }

    public sealed class SevenBag
    {
        static readonly Tetromino[] Order = { Tetromino.I, Tetromino.O, Tetromino.J, Tetromino.L, Tetromino.S, Tetromino.Z, Tetromino.T };
        readonly IRandomSource random;
        readonly Tetromino[] bag = new Tetromino[7];
        int index = 7;

        public SevenBag(IRandomSource random) { this.random = random; }

        public Tetromino Next()
        {
            if (index >= 7) Shuffle();
            return bag[index++];
        }

        void Shuffle()
        {
            for (var i = 0; i < 7; i++) bag[i] = Order[i];
            for (var i = 6; i > 0; i--)
            {
                var swap = random.Next(i + 1);
                var held = bag[i];
                bag[i] = bag[swap];
                bag[swap] = held;
            }
            index = 0;
        }
    }

    public static class FallSpeed
    {
        public static double IntervalSeconds(int linesCleared)
        {
            var level = Math.Max(0, linesCleared) / 10;
            var interval = Math.Pow(0.85, level);
            return interval < 0.10 ? 0.10 : interval;
        }
    }

    public static class ClearScore
    {
        public static int LinePoints(int lines)
        {
            switch (lines)
            {
                case 1: return 100;
                case 2: return 250;
                case 3: return 400;
                case 4: return 800;
                default: return 0;
            }
        }

        public static string Callout(int lines, int combo)
        {
            var word = lines >= 4 ? "TETRIS!" : lines == 3 ? "AMAZING!" : lines == 2 ? "DOUBLE!" : lines == 1 ? "CLEAR!" : "";
            if (word.Length == 0 || combo < 2) return word;
            return word + "  COMBO x" + combo;
        }
    }

    public sealed class HoldScheduler
    {
        public const float InitialDelay = 0.16f;
        public const float RepeatInterval = 0.05f;
        bool armed;
        float time;
        float nextRepeat;

        public int Tick(bool held, float dt)
        {
            if (!held)
            {
                armed = false;
                time = 0f;
                nextRepeat = InitialDelay;
                return 0;
            }
            if (!armed)
            {
                armed = true;
                time = 0f;
                nextRepeat = InitialDelay;
                return 1;
            }
            time += dt;
            var count = 0;
            while (time + 0.00001f >= nextRepeat && count < 8)
            {
                count++;
                nextRepeat += RepeatInterval;
            }
            return count;
        }
    }

    public sealed class FallingGame
    {
        readonly IRandomSource random;
        readonly BoardState board = new BoardState();
        SevenBag bag;
        Tetromino activeType;
        Tetromino nextType;
        Rotation activeRotation;
        int activeX;
        int activeY;
        bool hasPiece;

        public FallingGame(IRandomSource random, int best)
        {
            this.random = random;
            Best = best;
            Restart();
        }

        public BoardState Board => board;
        public int Score { get; private set; }
        public int Best { get; private set; }
        public int Combo { get; private set; }
        public int LinesCleared { get; private set; }
        public bool Paused { get; private set; }
        public bool GameOver { get; private set; }
        public bool HasPiece => hasPiece;
        public Tetromino Active => activeType;
        public Tetromino Next => nextType;
        public Rotation ActiveRotation => activeRotation;
        public int X => activeX;
        public int Y => activeY;
        public int GhostY { get; private set; }
        public string Callout { get; private set; } = "";
        public int CalloutId { get; private set; }
        public int PieceSerial { get; private set; }
        public bool LastLock { get; private set; }
        public int LastClearCount { get; private set; }
        readonly int[] lastClearRows = { -1, -1, -1, -1 };
        readonly int[] clearHold = new int[BoardState.Width * BoardState.VisibleRows];
        public int LastClearRow(int index) => index >= 0 && index < lastClearRows.Length ? lastClearRows[index] : -1;
        public bool ClearHoldReady { get; private set; }

        public int ClearHoldCell(int x, int y)
        {
            if (x < 0 || y < 0 || x >= BoardState.Width || y >= BoardState.VisibleRows) return 0;
            return clearHold[y * BoardState.Width + x];
        }
        public double FallInterval => FallSpeed.IntervalSeconds(LinesCleared);

        public void Restart()
        {
            Score = 0;
            Combo = 0;
            LinesCleared = 0;
            Paused = false;
            GameOver = false;
            Callout = "";
            CalloutId++;
            LastLock = false;
            LastClearCount = 0;
            ClearHoldReady = false;
            for (var i = 0; i < lastClearRows.Length; i++) lastClearRows[i] = -1;
            board.ClearAll();
            bag = new SevenBag(random);
            nextType = bag.Next();
            SpawnFromNext();
        }

        public void SetPaused(bool paused)
        {
            if (!GameOver) Paused = paused;
        }

        public bool MoveLeft() => Move(-1, 0);
        public bool MoveRight() => Move(1, 0);

        public bool SoftDrop()
        {
            LastLock = false;
            if (!CanControl()) return false;
            if (!Move(0, 1))
            {
                Lock();
                return false;
            }
            Score += 1;
            RaiseBest();
            return true;
        }

        public int HardDrop()
        {
            LastLock = false;
            if (!CanControl()) return 0;
            var rows = 0;
            while (Move(0, 1)) rows++;
            Score += rows * 2;
            RaiseBest();
            Lock();
            return rows;
        }

        public bool StepGravity()
        {
            LastLock = false;
            if (!CanControl()) return false;
            if (Move(0, 1)) return true;
            Lock();
            return false;
        }

        public int RotateClockwise()
        {
            LastLock = false;
            if (!CanControl() || activeType == Tetromino.O) return 0;
            if (!WallKicks.Try(activeType, activeRotation, true, activeX, activeY, Fits, out var rotation, out var x, out var y, out var tryIndex))
                return 0;
            activeRotation = rotation;
            activeX = x;
            activeY = y;
            RefreshGhost();
            return tryIndex;
        }

        public bool BeginPiece(Tetromino type, Rotation rotation, int x, int y)
        {
            if (!Fits(type, rotation, x, y)) return false;
            activeType = type;
            activeRotation = rotation;
            activeX = x;
            activeY = y;
            hasPiece = true;
            GameOver = false;
            RefreshGhost();
            return true;
        }

        public bool TrySpawnNext()
        {
            SpawnFromNext();
            return hasPiece && !GameOver;
        }

        public bool ContinueOnce()
        {
            if (!GameOver) return false;
            for (var y = BoardState.MinY; y <= 3; y++)
            for (var x = 0; x < BoardState.Width; x++)
                board.Set(x, y, 0);
            GameOver = false;
            Paused = false;
            LastLock = false;
            Callout = "";
            SpawnFromNext();
            return hasPiece && !GameOver;
        }

        public IReadOnlyList<BoardCell> AbsoluteCells(int originX, int originY)
        {
            var local = Tetrominoes.Cells(activeType, activeRotation);
            var cells = new BoardCell[local.Count];
            for (var i = 0; i < local.Count; i++)
                cells[i] = new BoardCell(originX + local[i].X, originY + local[i].Y);
            return cells;
        }

        bool Move(int dx, int dy)
        {
            if (!CanControl()) return false;
            if (!Fits(activeType, activeRotation, activeX + dx, activeY + dy)) return false;
            activeX += dx;
            activeY += dy;
            RefreshGhost();
            return true;
        }

        bool CanControl() => hasPiece && !Paused && !GameOver;

        void SpawnFromNext()
        {
            activeType = nextType;
            activeRotation = Rotation.Zero;
            activeX = Tetrominoes.SpawnX(activeType);
            activeY = Tetrominoes.SpawnY(activeType);
            nextType = bag.Next();
            PieceSerial++;
            if (!Fits(activeType, activeRotation, activeX, activeY))
            {
                hasPiece = false;
                GameOver = true;
                Paused = false;
                return;
            }
            hasPiece = true;
            RefreshGhost();
        }

        void Lock()
        {
            var cells = AbsoluteCells(activeX, activeY);
            var hidden = false;
            foreach (var cell in cells)
            {
                board.Set(cell.X, cell.Y, (int)activeType + 1);
                if (cell.Y < 0) hidden = true;
            }
            RememberFullRows();
            RememberClearPicture();
            var cleared = board.ClearVisibleRows();
            if (LastClearCount != cleared) LastClearCount = cleared;
            LinesCleared += cleared;
            if (cleared > 0)
            {
                Combo = Combo < 1 ? 1 : Combo + 1;
                Score += ClearScore.LinePoints(cleared);
                if (Combo >= 2) Score += 25 * Combo;
                Callout = ClearScore.Callout(cleared, Combo);
            }
            else
            {
                Combo = 0;
                Callout = "";
            }
            CalloutId++;
            RaiseBest();
            LastLock = true;
            hasPiece = false;
            if (hidden)
            {
                GameOver = true;
                Paused = false;
                return;
            }
            SpawnFromNext();
        }

        void RememberFullRows()
        {
            LastClearCount = 0;
            for (var i = 0; i < lastClearRows.Length; i++) lastClearRows[i] = -1;
            for (var y = BoardState.MaxY; y >= 0; y--)
            {
                if (!board.IsVisibleRowFull(y)) continue;
                if (LastClearCount < lastClearRows.Length) lastClearRows[LastClearCount] = y;
                LastClearCount++;
            }
        }

        void RememberClearPicture()
        {
            ClearHoldReady = LastClearCount > 0;
            if (!ClearHoldReady) return;
            for (var y = 0; y < BoardState.VisibleRows; y++)
            for (var x = 0; x < BoardState.Width; x++)
                clearHold[y * BoardState.Width + x] = board.Get(x, y);
        }

        void RefreshGhost()
        {
            var y = activeY;
            while (Fits(activeType, activeRotation, activeX, y + 1)) y++;
            GhostY = y;
        }

        bool Fits(Tetromino type, Rotation rotation, int originX, int originY)
        {
            foreach (var cell in Tetrominoes.Cells(type, rotation))
            {
                var x = originX + cell.X;
                var y = originY + cell.Y;
                if (!board.IsInside(x, y) || board.IsOccupied(x, y)) return false;
            }
            return true;
        }

        void RaiseBest()
        {
            if (Score > Best) Best = Score;
        }
    }
}
