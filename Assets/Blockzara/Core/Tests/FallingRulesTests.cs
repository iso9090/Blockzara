using System.Collections.Generic;
using NUnit.Framework;

namespace Blockzara.Core.Tests
{
    public sealed class FallingRulesTests
    {
        static FallingGame Game() => new FallingGame(new SeededRandom(4), 0);

        [Test]
        public void Board_IsTenByTwenty_WithTwoHiddenRows()
        {
            var game = Game();
            Assert.That(BoardState.Width, Is.EqualTo(10));
            Assert.That(BoardState.VisibleRows, Is.EqualTo(20));
            Assert.That(BoardState.MinY, Is.EqualTo(-2));
            Assert.That(BoardState.MaxY, Is.EqualTo(19));
            game.Board.Set(-1, 0, 1);
            game.Board.Set(10, 0, 1);
            game.Board.Set(0, -3, 1);
            game.Board.Set(0, 20, 1);
            Assert.That(game.Board.IsOccupied(0, 0), Is.False);
            Assert.That(game.Board.IsInside(0, -2), Is.True);
            Assert.That(game.Board.IsInside(9, 19), Is.True);
        }

        [Test]
        public void Spawn_UsesHiddenRows_AndCenteredColumns()
        {
            var game = Game();
            Assert.That(game.HasPiece, Is.True);
            Assert.That(game.GameOver, Is.False);
            Assert.That(game.X, Is.EqualTo(Tetrominoes.SpawnX(game.Active)));
            Assert.That(game.Y, Is.EqualTo(Tetrominoes.SpawnY(game.Active)));
            Assert.That(Tetrominoes.SpawnX(Tetromino.O), Is.EqualTo(4));
            Assert.That(Tetrominoes.SpawnX(Tetromino.I), Is.EqualTo(3));
            Assert.That(Tetrominoes.SpawnY(Tetromino.I), Is.EqualTo(-2));
            var hidden = false;
            foreach (var cell in game.AbsoluteCells(game.X, game.Y))
            {
                Assert.That(game.Board.IsInside(cell.X, cell.Y), Is.True);
                Assert.That(game.Board.IsOccupied(cell.X, cell.Y), Is.False);
                if (cell.Y < 0) hidden = true;
            }
            Assert.That(hidden, Is.True);
        }

        [Test]
        public void Bag_DealsSevenUnique_ThenRepeatsForTheSameSeed()
        {
            var first = new SevenBag(new SeededRandom(9));
            var second = new SevenBag(new SeededRandom(9));
            var seen = new HashSet<Tetromino>();
            for (var i = 0; i < 14; i++)
            {
                if (i % 7 == 0) seen.Clear();
                var piece = first.Next();
                Assert.That(seen.Add(piece), Is.True);
                Assert.That(second.Next(), Is.EqualTo(piece));
            }
        }

        [Test]
        public void NextPiece_StaysUntilTheActivePieceLocks()
        {
            var game = Game();
            var next = game.Next;
            game.MoveLeft();
            Assert.That(game.Next, Is.EqualTo(next));
            game.HardDrop();
            Assert.That(game.Active, Is.EqualTo(next));
        }

        [Test]
        public void Move_StopsAtTheWalls()
        {
            var game = Game();
            Assert.That(game.BeginPiece(Tetromino.T, Rotation.Zero, 0, 5), Is.True);
            Assert.That(game.MoveLeft(), Is.False);
            Assert.That(game.X, Is.EqualTo(0));
            Assert.That(game.MoveRight(), Is.True);
            Assert.That(game.X, Is.EqualTo(1));
            Assert.That(game.BeginPiece(Tetromino.I, Rotation.Zero, 6, 8), Is.True);
            Assert.That(game.MoveRight(), Is.False);
            Assert.That(game.X, Is.EqualTo(6));
        }

        [Test]
        public void Rotate_O_IsANoOp()
        {
            var game = Game();
            Assert.That(game.BeginPiece(Tetromino.O, Rotation.Zero, 4, 8), Is.True);
            var before = game.AbsoluteCells(game.X, game.Y);
            Assert.That(game.RotateClockwise(), Is.EqualTo(0));
            Assert.That(game.ActiveRotation, Is.EqualTo(Rotation.Zero));
            Assert.That(game.AbsoluteCells(game.X, game.Y), Is.EqualTo(before));
        }

        [Test]
        public void Rotate_T_UsesTheSecondKick_WhenTheBasicPoseIsBlocked()
        {
            var game = Game();
            Assert.That(game.BeginPiece(Tetromino.T, Rotation.Zero, 0, 5), Is.True);
            game.Board.Set(1, 7, 1);
            Assert.That(game.RotateClockwise(), Is.EqualTo(2));
            Assert.That(game.X, Is.EqualTo(-1));
            Assert.That(game.Y, Is.EqualTo(5));
            Assert.That(game.ActiveRotation, Is.EqualTo(Rotation.Right));
        }

        [Test]
        public void Rotate_T_DoesNothing_WhenEveryKickIsBlocked()
        {
            var game = Game();
            Assert.That(game.BeginPiece(Tetromino.T, Rotation.Zero, 0, 5), Is.True);
            game.Board.Set(1, 7, 1);
            game.Board.Set(0, 7, 1);
            game.Board.Set(0, 4, 1);
            Assert.That(game.RotateClockwise(), Is.EqualTo(0));
            Assert.That(game.X, Is.EqualTo(0));
            Assert.That(game.ActiveRotation, Is.EqualTo(Rotation.Zero));
        }

        [Test]
        public void Rotate_I_UsesTheSecondKick()
        {
            var game = Game();
            Assert.That(game.BeginPiece(Tetromino.I, Rotation.Zero, 3, 5), Is.True);
            game.Board.Set(5, 5, 1);
            Assert.That(game.RotateClockwise(), Is.EqualTo(2));
            Assert.That(game.X, Is.EqualTo(1));
            Assert.That(game.ActiveRotation, Is.EqualTo(Rotation.Right));
        }

        [Test]
        public void SoftDrop_ScoresOnePerRow_AndLocksWhenBlocked()
        {
            var game = Game();
            var y = game.Y;
            Assert.That(game.SoftDrop(), Is.True);
            Assert.That(game.Y, Is.EqualTo(y + 1));
            Assert.That(game.Score, Is.EqualTo(1));
            Assert.That(game.LastLock, Is.False);
            Assert.That(game.BeginPiece(Tetromino.O, Rotation.Zero, 4, 18), Is.True);
            Assert.That(game.SoftDrop(), Is.False);
            Assert.That(game.LastLock, Is.True);
            Assert.That(game.Score, Is.EqualTo(1));
            Assert.That(game.LinesCleared, Is.EqualTo(0));
        }

        [Test]
        public void HardDrop_ScoresTwoPerRow_AndLandsOnTheGhostRow()
        {
            var game = Game();
            var start = game.Y;
            var ghost = game.GhostY;
            var cells = game.AbsoluteCells(game.X, ghost);
            foreach (var cell in cells)
                Assert.That(game.Board.IsOccupied(cell.X, cell.Y), Is.False);
            var rows = game.HardDrop();
            Assert.That(rows, Is.EqualTo(ghost - start));
            Assert.That(rows, Is.GreaterThan(0));
            Assert.That(game.Score, Is.EqualTo(rows * 2));
            foreach (var cell in cells)
                Assert.That(game.Board.IsOccupied(cell.X, cell.Y), Is.True);
        }

        [Test]
        public void Clear_RemovesFullRows_AndIgnoresAFullColumn()
        {
            var game = Game();
            for (var y = 0; y < 20; y++) game.Board.Set(0, y, 1);
            Assert.That(game.BeginPiece(Tetromino.O, Rotation.Zero, 4, 18), Is.True);
            game.HardDrop();
            Assert.That(game.LinesCleared, Is.EqualTo(0));
            Assert.That(game.Board.IsOccupied(0, 10), Is.True);
            Assert.That(ClearRows(game, Tetromino.I, Rotation.Zero, 6, 18), Is.EqualTo(1));
            Assert.That(game.Score, Is.EqualTo(100));
            Assert.That(game.Callout, Is.EqualTo("CLEAR!"));
            Assert.That(game.Combo, Is.EqualTo(1));
            Assert.That(game.LastClearCount, Is.EqualTo(1));
            Assert.That(game.LastClearRow(0), Is.GreaterThanOrEqualTo(0).And.LessThan(BoardState.VisibleRows));
        }

        [Test]
        public void Clear_ScoresDoubleAmazingAndTetris()
        {
            var game = Game();
            Assert.That(ClearRows(game, Tetromino.O, Rotation.Zero, 8, 18), Is.EqualTo(2));
            Assert.That(game.Score, Is.EqualTo(250));
            Assert.That(game.Callout, Is.EqualTo("DOUBLE!"));
            game.Restart();
            Assert.That(ClearRows(game, Tetromino.T, Rotation.Right, 7, 17), Is.EqualTo(3));
            Assert.That(game.Score, Is.EqualTo(400));
            Assert.That(game.Callout, Is.EqualTo("AMAZING!"));
            game.Restart();
            Assert.That(ClearRows(game, Tetromino.I, Rotation.Right, 7, 16), Is.EqualTo(4));
            Assert.That(game.Score, Is.EqualTo(800));
            Assert.That(game.Callout, Is.EqualTo("TETRIS!"));
            Assert.That(game.Combo, Is.EqualTo(1));
            Assert.That(game.LastClearCount, Is.EqualTo(4));
        }

        [Test]
        public void Combo_IncrementsAcrossClears_AndResetsOnAQuietLock()
        {
            var game = Game();
            ClearRows(game, Tetromino.I, Rotation.Zero, 6, 18);
            Assert.That(game.Combo, Is.EqualTo(1));
            ClearRows(game, Tetromino.I, Rotation.Zero, 6, 18);
            Assert.That(game.Combo, Is.EqualTo(2));
            Assert.That(game.Score, Is.EqualTo(250));
            Assert.That(game.Callout, Is.EqualTo("CLEAR!  COMBO x2"));
            Assert.That(game.BeginPiece(Tetromino.O, Rotation.Zero, 0, 0), Is.True);
            var before = game.Score;
            var travel = game.HardDrop();
            Assert.That(game.Combo, Is.EqualTo(0));
            Assert.That(game.Callout, Is.EqualTo(""));
            Assert.That(game.Score, Is.EqualTo(before + travel * 2));
            ClearRows(game, Tetromino.I, Rotation.Zero, 6, 18);
            Assert.That(game.Combo, Is.EqualTo(1));
            Assert.That(game.Callout, Is.EqualTo("CLEAR!"));
        }

        [Test]
        public void FallInterval_StartsAtOneSecond_AndFloorsAtOneTenth()
        {
            Assert.That(FallSpeed.IntervalSeconds(0), Is.EqualTo(1.0).Within(1e-9));
            Assert.That(FallSpeed.IntervalSeconds(9), Is.EqualTo(1.0).Within(1e-9));
            Assert.That(FallSpeed.IntervalSeconds(10), Is.EqualTo(0.85).Within(1e-9));
            Assert.That(FallSpeed.IntervalSeconds(140), Is.GreaterThan(0.10));
            Assert.That(FallSpeed.IntervalSeconds(150), Is.EqualTo(0.10).Within(1e-9));
            Assert.That(FallSpeed.IntervalSeconds(1000), Is.EqualTo(0.10).Within(1e-9));
        }

        [Test]
        public void Pause_BlocksGravityAndMovement()
        {
            var game = Game();
            var y = game.Y;
            var x = game.X;
            game.SetPaused(true);
            Assert.That(game.StepGravity(), Is.False);
            Assert.That(game.MoveRight(), Is.False);
            Assert.That(game.SoftDrop(), Is.False);
            Assert.That(game.Y, Is.EqualTo(y));
            Assert.That(game.X, Is.EqualTo(x));
            game.SetPaused(false);
            Assert.That(game.StepGravity(), Is.True);
            Assert.That(game.Y, Is.EqualTo(y + 1));
        }

        [Test]
        public void Restart_ClearsTheRun_AndKeepsTheBestScore()
        {
            var game = new FallingGame(new SeededRandom(3), 40);
            game.HardDrop();
            Assert.That(game.Score, Is.GreaterThan(0));
            var best = game.Best;
            Assert.That(best, Is.GreaterThanOrEqualTo(40));
            game.Restart();
            Assert.That(game.Score, Is.EqualTo(0));
            Assert.That(game.Best, Is.EqualTo(best));
            Assert.That(game.LinesCleared, Is.EqualTo(0));
            Assert.That(game.Combo, Is.EqualTo(0));
            Assert.That(game.GameOver, Is.False);
            Assert.That(game.HasPiece, Is.True);
        }

        [Test]
        public void SpawnIntoFilledCells_EndsTheGame()
        {
            var game = Game();
            for (var x = 0; x < BoardState.Width; x++)
            {
                game.Board.Set(x, -2, 1);
                game.Board.Set(x, -1, 1);
                game.Board.Set(x, 0, 1);
            }
            Assert.That(game.TrySpawnNext(), Is.False);
            Assert.That(game.GameOver, Is.True);
            Assert.That(game.HasPiece, Is.False);
        }

        [Test]
        public void LockingOnAHiddenRow_EndsTheGame()
        {
            var game = Game();
            Assert.That(game.BeginPiece(Tetromino.I, Rotation.Zero, 3, -2), Is.True);
            for (var x = 3; x <= 6; x++) game.Board.Set(x, 0, 1);
            Assert.That(game.StepGravity(), Is.False);
            Assert.That(game.GameOver, Is.True);
            Assert.That(game.HasPiece, Is.False);
            Assert.That(game.Board.IsOccupied(3, -1), Is.True);
        }

        [Test]
        public void ContinueOnce_ClearsTheTop_AndKeepsTheScore()
        {
            var game = Game();
            Assert.That(game.BeginPiece(Tetromino.I, Rotation.Zero, 3, -2), Is.True);
            for (var x = 3; x <= 6; x++) game.Board.Set(x, 0, 1);
            game.Board.Set(1, 19, 2);
            Assert.That(game.StepGravity(), Is.False);
            Assert.That(game.GameOver, Is.True);
            var score = game.Score;
            var lines = game.LinesCleared;
            var best = game.Best;
            Assert.That(game.ContinueOnce(), Is.True);
            Assert.That(game.GameOver, Is.False);
            Assert.That(game.HasPiece, Is.True);
            Assert.That(game.Score, Is.EqualTo(score));
            Assert.That(game.LinesCleared, Is.EqualTo(lines));
            Assert.That(game.Best, Is.EqualTo(best));
            Assert.That(game.Board.Get(1, 19), Is.EqualTo(2));
            Assert.That(game.Board.IsOccupied(3, -1), Is.False);
            Assert.That(game.ContinueOnce(), Is.False);
        }

        [Test]
        public void HoldScheduler_FiresImmediately_ThenAfterTheDelay()
        {
            var hold = new HoldScheduler();
            Assert.That(hold.Tick(true, 0f), Is.EqualTo(1));
            Assert.That(hold.Tick(true, 0.10f), Is.EqualTo(0));
            Assert.That(hold.Tick(true, 0.05f), Is.EqualTo(0));
            Assert.That(hold.Tick(true, 0.02f), Is.EqualTo(1));
            Assert.That(hold.Tick(true, 0.05f), Is.EqualTo(1));
            Assert.That(hold.Tick(false, 0f), Is.EqualTo(0));
            Assert.That(hold.Tick(true, 1f), Is.EqualTo(1));
            var burst = new HoldScheduler();
            burst.Tick(true, 0f);
            Assert.That(burst.Tick(true, 0.40f), Is.EqualTo(5));
        }

        static int ClearRows(FallingGame game, Tetromino type, Rotation rotation, int x, int y)
        {
            Assert.That(game.BeginPiece(type, rotation, x, y), Is.True);
            var cells = game.AbsoluteCells(x, y);
            var covered = new HashSet<BoardCell>(cells);
            var rows = new HashSet<int>();
            foreach (var cell in cells) rows.Add(cell.Y);
            foreach (var row in rows)
            for (var column = 0; column < BoardState.Width; column++)
                if (!covered.Contains(new BoardCell(column, row))) game.Board.Set(column, row, 1);
            var before = game.LinesCleared;
            Assert.That(game.HardDrop(), Is.EqualTo(0));
            return game.LinesCleared - before;
        }
    }
}
