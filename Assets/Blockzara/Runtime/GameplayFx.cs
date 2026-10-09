using Blockzara.Core;
using UnityEngine;

namespace Blockzara.Runtime
{
    public sealed class GameplayFx
    {
        readonly float[] clearFlash = new float[BoardState.VisibleRows];
        readonly Color[] clearColor = new Color[BoardState.VisibleRows];
        readonly int[] landX = new int[4];
        readonly int[] landY = new int[4];
        int landCount;
        float landPulse;
        float visualX;
        float spawn;
        float rotateAngle;
        float softFlash;
        float hardTrail;
        int hardFromY;
        int hardToY;
        int hardX;
        Tetromino hardType;
        Rotation hardRotation;
        int seenSerial = -1;
        float clearHold;
        public float Overlay;
        public string Banner = "";
        public float BannerUntil;
        public Color BannerColor = Color.white;

        public float VisualColumn => visualX;
        public float ActiveScale => 1f + spawn * 0.18f + (rotateAngle != 0f ? 0.04f : 0f);
        public float RotateDegrees => rotateAngle;
        public float SoftFlash => softFlash;
        public float HardTrail => hardTrail;
        public int HardFromY => hardFromY;
        public int HardToY => hardToY;
        public int HardX => hardX;
        public Tetromino HardType => hardType;
        public Rotation HardRotation => hardRotation;
        public int LandCount => landCount;
        public float LandPulse => landPulse;
        public float ClearHold => clearHold;

        public void Reset(FallingGame game)
        {
            visualX = game == null ? 0f : game.X;
            seenSerial = game == null ? -1 : game.PieceSerial;
            spawn = 0f;
            rotateAngle = 0f;
            softFlash = 0f;
            hardTrail = 0f;
            landPulse = 0f;
            landCount = 0;
            Overlay = 0f;
            Banner = "";
            clearHold = 0f;
            for (var i = 0; i < clearFlash.Length; i++) clearFlash[i] = 0f;
        }

        public void Tick(FallingGame game, float dt, bool reduced)
        {
            if (game == null) return;
            if (reduced)
            {
                visualX = game.X;
                spawn = 0f;
                rotateAngle = 0f;
                softFlash = 0f;
                hardTrail = 0f;
                landPulse = 0f;
                clearHold = 0f;
                for (var i = 0; i < clearFlash.Length; i++) clearFlash[i] = 0f;
            }
            else
            {
                if (game.HasPiece && game.PieceSerial != seenSerial)
                {
                    seenSerial = game.PieceSerial;
                    visualX = game.X;
                    spawn = 1f;
                }
                visualX = Mathf.MoveTowards(visualX, game.X, dt * 16f);
                spawn = Mathf.MoveTowards(spawn, 0f, dt * 6f);
                rotateAngle = Mathf.MoveTowards(rotateAngle, 0f, dt * 160f);
                softFlash = Mathf.MoveTowards(softFlash, 0f, dt * 4f);
                hardTrail = Mathf.MoveTowards(hardTrail, 0f, dt * 3.2f);
                landPulse = Mathf.MoveTowards(landPulse, 0f, dt * 4.5f);
                clearHold = Mathf.MoveTowards(clearHold, 0f, dt * 3.1f);
                for (var i = 0; i < clearFlash.Length; i++)
                    clearFlash[i] = Mathf.MoveTowards(clearFlash[i], 0f, dt * 1.15f);
            }
            var target = game.Paused || game.GameOver ? 1f : 0f;
            Overlay = Mathf.MoveTowards(Overlay, target, dt * 5f);
        }

        public void OnRotate() => rotateAngle = -16f;

        public void OnSoft() => softFlash = 1f;

        public void OnHard(Tetromino type, Rotation rotation, int x, int fromY, int toY)
        {
            hardType = type;
            hardRotation = rotation;
            hardX = x;
            hardFromY = fromY;
            hardToY = toY;
            hardTrail = 1f;
        }

        public void OnLock(FallingGame game, System.Collections.Generic.IReadOnlyList<BoardCell> cells, int bestBefore, int levelBefore, Color pieceColor)
        {
            if (game == null || !game.LastLock) return;
            seenSerial = game.PieceSerial;
            if (game.HasPiece) visualX = game.X;
            landCount = 0;
            if (cells != null && game.LastClearCount == 0)
            {
                for (var i = 0; i < cells.Count && landCount < landX.Length; i++)
                {
                    if (cells[i].Y < 0 || cells[i].Y >= BoardState.VisibleRows) continue;
                    landX[landCount] = cells[i].X;
                    landY[landCount] = cells[i].Y;
                    landCount++;
                }
            }
            landPulse = landCount > 0 ? 1f : 0f;
            var lines = game.LastClearCount;
            for (var i = 0; i < lines && i < 4; i++)
            {
                var row = game.LastClearRow(i);
                if (row >= 0 && row < clearFlash.Length)
                {
                    clearFlash[row] = 1f;
                    clearColor[row] = pieceColor;
                }
            }
            if (lines <= 0)
            {
                Banner = "";
                return;
            }
            Banner = game.Callout;
            BannerUntil = Time.unscaledTime + (lines >= 4 ? 1.6f : 1.15f);
            BannerColor = lines >= 4 ? new Color(1f, 0.86f, 0.35f) : lines == 3 ? new Color(0.55f, 0.9f, 1f) : lines == 2 ? new Color(1f, 0.82f, 0.45f) : Color.white;
            if (game.LinesCleared / 10 + 1 > levelBefore)
            {
                Banner = "LEVEL " + (game.LinesCleared / 10 + 1);
                BannerColor = new Color(0.7f, 1f, 0.75f);
                BannerUntil = Time.unscaledTime + 1.4f;
            }
            if (game.Best > bestBefore)
            {
                Banner = "NEW BEST";
                BannerColor = new Color(1f, 0.9f, 0.45f);
                BannerUntil = Time.unscaledTime + 1.5f;
            }
        }

        public void BeginClearHold() => clearHold = 1f;

        public float ClearAlpha(int y) => y >= 0 && y < clearFlash.Length ? clearFlash[y] : 0f;

        public Color ClearColor(int y) => y >= 0 && y < clearColor.Length ? clearColor[y] : Color.white;

        public bool LandCell(int index, out int x, out int y)
        {
            x = 0;
            y = 0;
            if (index < 0 || index >= landCount) return false;
            x = landX[index];
            y = landY[index];
            return true;
        }
    }
}
