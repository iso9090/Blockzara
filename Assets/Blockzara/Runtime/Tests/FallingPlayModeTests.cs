using System.Collections;
using Blockzara.Core;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Blockzara.Runtime.Tests
{
    public sealed class FallingPlayModeTests
    {
        [UnityTest]
        public IEnumerator HoldRight_MovesMoreThanOneColumn()
        {
            var app = Create();
            try
            {
                app.StartRun(4);
                var start = app.Game.X;
                var moves = app.SimulateHoldRight(0.30f);
                Assert.That(moves, Is.GreaterThanOrEqualTo(2));
                Assert.That(app.Game.X, Is.EqualTo(start + moves));
                yield return null;
            }
            finally
            {
                Object.Destroy(app.gameObject);
            }
        }

        [UnityTest]
        public IEnumerator HoldSoftDrop_MovesMoreThanOneRow()
        {
            var app = Create();
            try
            {
                app.StartRun(4);
                var start = app.Game.Y;
                var moves = app.SimulateHoldSoft(0.30f);
                Assert.That(moves, Is.GreaterThanOrEqualTo(2));
                Assert.That(app.Game.Y, Is.EqualTo(start + moves));
                Assert.That(app.Game.Score, Is.EqualTo(moves));
                yield return null;
            }
            finally
            {
                Object.Destroy(app.gameObject);
            }
        }

        [UnityTest]
        public IEnumerator Rotate_UsesAWallKick()
        {
            var app = Create();
            try
            {
                app.StartRun(4);
                Assert.That(app.Game.BeginPiece(Tetromino.T, Rotation.Zero, 0, 5), Is.True);
                app.Game.Board.Set(1, 7, 1);
                Assert.That(app.PressRotate(), Is.EqualTo(2));
                Assert.That(app.Game.X, Is.EqualTo(-1));
                Assert.That(app.Game.ActiveRotation, Is.EqualTo(Rotation.Right));
                yield return null;
            }
            finally
            {
                Object.Destroy(app.gameObject);
            }
        }

        [UnityTest]
        public IEnumerator HardDrop_LandsOnTheGhostRow()
        {
            var app = Create();
            try
            {
                app.StartRun(4);
                var ghost = app.Game.GhostY;
                var cells = app.Game.AbsoluteCells(app.Game.X, ghost);
                var rows = app.PressHardDrop();
                Assert.That(rows, Is.GreaterThan(0));
                Assert.That(app.Game.Score, Is.EqualTo(rows * 2));
                foreach (var cell in cells)
                    Assert.That(app.Game.Board.IsOccupied(cell.X, cell.Y), Is.True);
                yield return null;
            }
            finally
            {
                Object.Destroy(app.gameObject);
            }
        }

        [UnityTest]
        public IEnumerator HorizontalClear_ScoresASingle()
        {
            var app = Create();
            try
            {
                app.StartRun(4);
                Assert.That(app.Game.BeginPiece(Tetromino.I, Rotation.Zero, 6, 18), Is.True);
                var covered = new System.Collections.Generic.HashSet<BoardCell>(app.Game.AbsoluteCells(6, 18));
                for (var x = 0; x < BoardState.Width; x++)
                    if (!covered.Contains(new BoardCell(x, 19))) app.Game.Board.Set(x, 19, 1);
                app.PressHardDrop();
                Assert.That(app.Game.LinesCleared, Is.EqualTo(1));
                Assert.That(app.Game.Score, Is.EqualTo(100));
                Assert.That(app.Game.Callout, Is.EqualTo("CLEAR!"));
                Assert.That(app.Game.Board.IsOccupied(0, 19), Is.False);
                yield return null;
            }
            finally
            {
                Object.Destroy(app.gameObject);
            }
        }

        static BlockzaraApp Create()
        {
            var go = new GameObject("BLOCKZARA");
            return go.AddComponent<BlockzaraApp>();
        }
    }
}
