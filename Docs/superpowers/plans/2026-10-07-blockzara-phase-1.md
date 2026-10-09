# BLOCKZARA Falling Blocks Implementation Plan

**Status: approved design, not started. Do not write gameplay code until the owner finishes the final document review.**

**Spec:** `D:\Projects\Blockzara\Docs\superpowers\specs\2026-10-07-blockzara-phase-1-design.md`

**Drag-and-place backup, do not delete:** `D:\Projects\Blockzara\Backups\2026-10-08-drag-and-place\`

**Goal:** Replace tray drag-and-place with falling tetrominoes on a 10×20 board, keep BLOCKZARA branding, and verify on Samsung `RF8M91BJBJW`.

**Architecture:** Core stays free of `UnityEngine`. It owns the 10×20 field, the two hidden rows, the active piece, SRS-like kicks, locking, the ghost landing row, row clears, score, combo, speed, and the seeded 7-bag. Runtime draws the navy presentation, the translucent ghost, the next piece, and the touch controls. The Android debug build entry point stays `BlockzaraBuild`.

## Global constraints

- Work only in `D:\Projects\Blockzara`.
- Unity 6000.6.0f1, package `com.blockzara.game`, portrait only.
- Keep `Backups\2026-10-08-drag-and-place\`. Do not delete it during implementation.
- Live drag-and-place files stay until the task that replaces their callers. That task removes them from the live tree only, never from the backup.
- No Firebase, AdMob, billing, backend, GitHub, commit, push, or deploy.
- No new art files. The ghost is the existing flat block color at 35% opacity.
- Tests run with `-runTests` and without `-quit`. A run counts as a pass only when the XML shows a nonzero pass count and zero failures.

## Approved behavior to implement

- Visible 10×20 board plus hidden rows Y = -1 and Y = -2.
- Seven tetrominoes and a seeded 7-bag, with one next-piece preview.
- Horizontal line clears only.
- Endless score and a best score that only increases.
- Gravity starts at 1.0s per row, multiplies by 0.85 every 10 cleared lines, and floors at 0.10s.
- Deterministic SRS-like wall kicks, using the offset tables in the spec.
- Held Left, Right, and Soft Drop: immediate first step, 0.16s delay, then a step every 0.05s.
- Translucent ghost at the hard-drop row.
- Samsung touch controls: Left, Right, Rotate, Soft Drop, Hard Drop.

## Reusable code

| Piece | Why it stays |
| --- | --- |
| `BoardCell`, `BlockShape` | Coordinate pieces still describe a tetromino. |
| `BoardState` occupancy and atomic place | Needed for movement and lock. The fixed 8×8 size changes to 10×20 plus two hidden rows. |
| `PlacementValidator` | Still answers whether a kicked or dropped pose fits. |
| `LineResolver` row scan and `BoardState.Clear` | Horizontal clears stay. The column scan does not. |
| `ScoreSystem.NextCombo` and `CLEAR!`, `DOUBLE!`, `AMAZING!` | Consecutive-clear combo stays. Add `TETRIS!` and the new point table. |
| `IRandomSource`, `SeededRandom` | The 7-bag stays deterministic. |
| `LocalProgressService` | Best score already persists and never decreases. |
| Core and Runtime asmdefs, and the test asmdefs | Boundaries stay. |
| `BlockzaraBuild.CreateSceneAndBuildAndroidDebug` | Still produces the debug APK. |
| Title, Play button, navy background, flat block colors | Branding. The ghost reuses those colors. |
| Unity Test Framework 1.8.0 batch workflow | Already verified. |

## Obsolete for falling blocks

Leave these in the live tree until the task that replaces them. The backup already contains them.

| Piece | Why it drops out |
| --- | --- |
| `BoardMapper` and `DragMappingTests` | Screen-to-cell drag release is the old input model. |
| Tray of three shapes inside `BlockzaraApp` | One active piece and a next bag replace the tray. |
| `BeginDragFromTray`, `ReleaseDrag`, finger-offset drag preview | Drag path. This is separate from the new ghost piece. |
| `DragPlacementPlayModeTests`, `ScoringPlayModeTests` | They drive the tray. |
| Column clearing and row/column intersection scoring | Only horizontal rows clear. |
| `ShapeLibrary` bar / square / corner set, `PieceGenerator`, tray `GameState` | Replaced by the seven tetrominoes and a 7-bag. |
| 10 points per placed cell, target 1000, `StarEvaluator` efficiency stars | Endless high score uses the new table. |
| 8×8 board constants | The field becomes 10×20. |

`BlockzaraApp.OnGUI` is rewritten in place for the well, ghost, HUD, next piece, and five controls when implementation is allowed.

## Tasks after document review

### 1. Resize the core field

Change `BoardState` to 10 visible columns, 20 visible rows, and hidden rows -2 and -1. Tests cover occupancy, a full horizontal clear, and a full column that does not clear.

### 2. Active piece and kicks

Add spawn, left, right, clockwise rotate, soft drop, hard drop, lock, and the ghost row. Rotation tests use the spec tables: a kick that succeeds on a named offset, a kick that fails and leaves the piece unmoved, the I table, and the O no-op. The same board and state must select the same offset every time.

### 3. Held repeat

Core exposes a single-step move. Runtime repeats Left, Right, and Soft Drop with the 0.16s delay and 0.05s interval while the control is held. A PlayMode test holds Right and asserts more than one column of travel, and holds Soft Drop and asserts more than one row.

### 4. Rows, score, speed

Clear every full visible row on lock. Apply the score table, combo, and ghost-independent scoring. Every 10 cleared lines multiplies the fall interval by 0.85 and clamps it to 0.10 seconds. Tests cover a double, a four-line clear, a combo reset, the level-0 interval of 1.0s, and the 0.10s floor.

### 5. Bag and preview

Implement the 7-bag on `IRandomSource`. The same seed must produce the same seven-piece order, then a new bag. Show one next piece.

### 6. Presentation

Draw the 10×20 well, the translucent ghost, SCORE, BEST, the callout, and the next piece. Add Left, Right, Rotate, Soft Drop, and Hard Drop. Keep the BLOCKZARA menu.

### 7. PlayMode and device

PlayMode tests use the button actions: move, rotate into a wall kick, soft drop, hard drop onto the ghost row, and a row clear. Then build a fresh debug APK, install it on `RF8M91BJBJW`, play a real game, and save new screenshots and the device log. Screenshots must show the ghost and a horizontal clear.

## Not in this implementation

Hold, lock delay, move reset, T-spins, and any return to drag-and-place. The backup is retained either way.
