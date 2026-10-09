# BLOCKZARA optional modes

Status: specification only. Classic endless is the live mode. These modes are not implemented.

## Classic endless

The current game. A run continues until spawn or a lock in the hidden rows fails. Score, level, and best persist as they do now. This spec does not change those rules.

## Target challenge

- The player must clear a chosen line target: 10, 20, or 40.
- Failure is the same game-over condition as classic.
- Success ends the run when lifetime lines in that run reach the target. The board does not grant extra score beyond the existing line awards.
- The target and the result are stored locally. Daily Challenge and World Map stay locked.

## Timed challenge

- A local timer of 120 seconds runs only while the piece can fall.
- Pause stops the timer. Game over ends it early.
- The score uses the same classic awards. When time expires the run ends without an extra bonus unless one is approved later.

## Daily challenge

- One local seed per calendar day, derived from the device date as `year * 1000 + dayOfYear`.
- The seed feeds the existing 7-bag. No server.
- The menu tile stays locked until the mode can start, finish, and record a result.
- Missing a day does not invent a streak. A streak needs an approved rule before it is shown.

No mode here is available from the menu.
