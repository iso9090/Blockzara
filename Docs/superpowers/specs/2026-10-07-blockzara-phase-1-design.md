# BLOCKZARA Falling Blocks Design Specification

**Status: approved on 2026-10-08, with the amendments in this document. Implementation has not started. This revision is waiting for a final document review.**

The drag-and-place source and its original spec remain in `D:\Projects\Blockzara\Backups\2026-10-08-drag-and-place\`. That backup stays in the project. It is not deleted and it is not the active design.

## Purpose

BLOCKZARA is an offline portrait Android game. Play is falling blocks: one tetromino falls, the player steers it, and completed horizontal rows clear.

## Approved rules

- The visible board is 10 columns by 20 rows. Two hidden rows sit above it for spawn.
- Pieces are the seven tetrominoes, drawn from a seeded 7-bag. One next piece is shown.
- Pieces fall on their own. Gravity starts at 1.0 second per row and speeds up every 10 cleared lines. The fall interval never goes below 0.10 seconds.
- The player can move left and right, rotate, soft-drop, and hard-drop.
- Rotation uses deterministic SRS-like wall kicks.
- Holding Left, Right, or Soft Drop repeats that action on a fixed schedule.
- A translucent ghost shows where a hard drop would lock.
- Only completed horizontal rows clear.
- Scoring is endless. Best score only increases.
- Portrait touch controls target the Samsung phone.
- Branding stays BLOCKZARA: product name, package `com.blockzara.game`, deep navy field, and the existing title treatment.
- Work stays inside `D:\Projects\Blockzara`. No other project, GitHub, commit, push, deploy, Firebase, AdMob, or billing.

## Platform

| Property | Value |
| --- | --- |
| Engine | Unity 6000.6.0f1 |
| Package | `com.blockzara.game` |
| Orientation | Portrait only |
| Device | Samsung SM-N975F (`RF8M91BJBJW`) |
| Build output | `D:\Projects\Blockzara\Builds\Blockzara-debug.apk` |
| Core rule | Gameplay rules stay in the Core assembly and do not reference `UnityEngine`. |

## Board

The visible field is 10×20. Row 0 is the top and row 19 is the bottom, so Y grows downward and gravity increases Y. Columns are 0 through 9.

Two hidden rows, Y = -1 and Y = -2, are spawn space. They are not drawn as part of the 10×20 well. A piece that locks with any cell still on a hidden row ends the game.

## Pieces

I, O, J, L, S, Z, and T. Each shape is four coordinate offsets on `BlockShape`, with rotation states 0 (spawn), R (clockwise), 2 (180), and L (counterclockwise).

Spawn is centered at the top of the visible field, using the hidden rows when the shape extends above row 0. The O piece does not rotate its cells.

The next piece comes from a 7-bag. A bag is one copy of each tetromino, shuffled with the seeded `IRandomSource`. An empty bag refills and shuffles again. The same seed produces the same sequence. The runtime shows the next piece beside the well.

Hold is not in this version.

## Motion

Gravity moves the active piece down one row when the fall interval elapses and the destination is empty.

Left and right move one column when every destination cell is empty and inside the field, including the hidden rows.

### Held movement and soft drop

The first press of Left, Right, or Soft Drop acts immediately. If the control is still held after 0.16 seconds, that action repeats every 0.05 seconds until the control is released or the move no longer fits. Repeating uses the same core step as a single press. The schedule is fixed, so a seed plus the same hold timings replay the same cells.

Soft drop steps down one row per repeat. It scores 1 point for each row the piece actually moves. It does not lock by itself. When soft drop is held, its 0.05-second repeat replaces gravity until the button is released.

Hard drop moves the piece to the lowest valid row, scores 2 points for each row traveled, and locks immediately.

A piece locks when a gravity step or a soft-drop step cannot move it down, or when hard drop finishes. This version has no lock delay and no move reset.

### Wall kicks

Rotation is deterministic. Offsets are (column, row) with positive row downward. The Rotate button applies one clockwise step: 0→R, R→2, 2→L, or L→0. The return step tries those same offsets with both signs flipped, in the same order. The first offset that keeps all four cells empty and inside the field, including the hidden rows, is applied. If none fit, the piece does not rotate.

J, L, S, T, and Z share this clockwise table:

| Try | 0→R | R→2 | 2→L | L→0 |
| --- | --- | --- | --- | --- |
| 1 | (0, 0) | (0, 0) | (0, 0) | (0, 0) |
| 2 | (-1, 0) | (+1, 0) | (+1, 0) | (-1, 0) |
| 3 | (-1, -1) | (+1, +1) | (+1, -1) | (-1, +1) |
| 4 | (0, +2) | (0, -2) | (0, +2) | (0, -2) |
| 5 | (-1, +2) | (+1, -2) | (+1, +2) | (-1, -2) |

The I piece uses this clockwise table:

| Try | 0→R | R→2 | 2→L | L→0 |
| --- | --- | --- | --- | --- |
| 1 | (0, 0) | (0, 0) | (0, 0) | (0, 0) |
| 2 | (-2, 0) | (-1, 0) | (+2, 0) | (+1, 0) |
| 3 | (+1, 0) | (+2, 0) | (-1, 0) | (-2, 0) |
| 4 | (-2, +1) | (-1, -2) | (+2, -1) | (+1, +2) |
| 5 | (+1, -2) | (+2, +1) | (-1, +2) | (-2, -1) |

The O piece treats rotation as a no-op and does not kick. The same piece, rotation, and board always select the same try. Tests name that try.

## Ghost piece

The ghost is the active piece drawn at the lowest row a hard drop would reach, in the active rotation. It uses the same flat color at 35% opacity. It does not occupy cells, score, or change collision. It updates whenever the active piece moves or rotates. No new image file is used.

## Lines and speed

After a lock, every completely filled row from Y 0 through Y 19 is removed in one step. Rows above the gap drop down to close it. Hidden rows are not cleared as lines. Columns are never scanned.

Speed level starts at 0. The fall interval is `max(0.10, 1.0 × 0.85 ^ level)` seconds per row. Every 10 cleared rows adds 1 to the level. At level 0 the interval is 1.0 second. It never goes below 0.10 seconds. Soft drop stays on its own 0.05-second repeat and is not clamped to that floor.

## Score and feedback

There is no 1000-point target. Best score loads and saves only through `LocalProgressService`, and only when the new score is higher.

| Event | Points |
| --- | --- |
| Soft drop, per row moved | 1 |
| Hard drop, per row traveled | 2 |
| 1 row | 100, callout `CLEAR!` |
| 2 rows | 250, callout `DOUBLE!` |
| 3 rows | 400, callout `AMAZING!` |
| 4 rows | 800, callout `TETRIS!` |

A lock that clears at least one row sets the combo to 1, or increments it when the previous lock also cleared. A lock that clears nothing resets the combo to 0. When the combo is 2 or higher, add `25 × combo` and append `COMBO xN` to the callout.

I, O, and T use three extra flat colors from the approved navy, gold, green, red, and purple direction. The other pieces keep cyan, yellow, and green. No new image files.

## Input

Portrait play uses on-screen controls: Left, Right, Rotate, Soft Drop, and Hard Drop. Left, Right, and Soft Drop honor the held-repeat schedule while the finger stays down. Hard Drop and Rotate fire once per press. Editor tests call the same core methods. The drag-from-tray path is not part of this design.

## End of game

The game ends when a new piece cannot be placed on its spawn cells, or when a piece locks while any of its cells is on a hidden row. Retry starts a new run and keeps the best score. The main menu remains the BLOCKZARA title and Play button.

## Not in this version

Hold, lock delay, move reset, T-spins, garbage rows, world maps, ads, accounts, and the 8×8 three-piece tray.

## Document review

Implementation stays stopped until the owner finishes reviewing this revision. The backup at `D:\Projects\Blockzara\Backups\2026-10-08-drag-and-place\` remains in place.
