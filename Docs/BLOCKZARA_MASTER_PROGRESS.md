# BLOCKZARA master progress

Local development only. No commit, push, deploy, or Play upload.

## Approved checkpoint — 2026-10-09 14:43 (UTC+4)

The owner approved this version as the final saved build. The adopted debug build is `Builds\Blockzara-debug-ui-v3-26.apk` (59,097,069 bytes, 2026-10-09 14:39), installed on Samsung `RF8M91BJBJW`. A frozen copy is `Backups\2026-10-09-approved-final-v326`, with `APPROVED.txt`.

Level 2 warms the coastal light. The stage counter counts placed pieces, starting at 10. The rewarded continue resumes the same round after the ad. Stats, Settings, Achievements, and Themes use the coastal gold card. `Builds\ui-v3-26-build.log` records Build Finished, Result: Success. Test ads only. ARMv7 only. Google Play remains not ready. The 11:51 v3-10 backup and the older approved APKs were not replaced.

## Approved checkpoint — 2026-10-09 11:51 (UTC+4)

The owner approved this version and asked for it to be saved. The adopted debug build is `Builds\Blockzara-debug-ui-v3-10.apk` (54,904,171 bytes, 2026-10-09 11:47:01), installed on Samsung `RF8M91BJBJW`. A frozen copy is `Backups\2026-10-09-approved-ui-v310`, with `APPROVED.txt`.

The launcher icon is the block Z over the coast. Daily and Skins are open and local. World Map stays locked. Themes are coastal, harbor, and night, with level lighting from level 5. Languages are Arabic, English, and Urdu. The green soft-drop button is at the top right, the wide yellow rotate button is under left and right, and the red hard drop is under the green button. `Builds\ui-v3-10-build.log` records Build Finished, Result: Success. The last EditMode 32 and PlayMode 5 runs were at 07:11:32Z and 07:12:00Z, before the final button swap and the icon, and were not repeated for this APK. Launcher frame: `outputs\icons\drawer-v310.png`. Test ads only. ARMv7 only. Google Play remains not ready. The 09:45 polish APK and the older debug APKs were not replaced.

## Approved checkpoint — 2026-10-09 09:45 (UTC+4)

The owner approved the play-polish build. The adopted debug build is `Builds\Blockzara-debug-ui-v3-4.apk` (54,901,086 bytes, 2026-10-09 09:43:22), installed on Samsung `RF8M91BJBJW`. A frozen copy is `Backups\2026-10-09-approved-play-polish`, with `APPROVED.txt`.

NEXT is larger. The falling piece has a light rim and locked cells stay solid. A cleared row flashes in the locking piece's color without a scoring change. The settings option is named Shadow and stays on unless turned off. EditMode 32 passed and PlayMode 5 passed (`Reports\tests\editmode-play-polish.log`, `Reports\tests\playmode-play-polish.log`). Unity frame: `outputs\ui-v32\play-polish.png`. Test ads only. ARMv7 only. Google Play remains not ready. The 09:32, 09:15, and original debug APKs were not replaced.

## Saved checkpoint — 2026-10-09 09:32 (UTC+4)

The owner asked to save the build with the optional piece shadow. The saved debug build is `Builds\Blockzara-debug-ui-v3-3.apk` (54,901,086 bytes, 2026-10-09 09:30:12), installed on Samsung `RF8M91BJBJW`. A frozen copy is `Backups\2026-10-09-saved-ghost-setting`, with `SAVED.txt`.

The approved square PLAY menu remains. "Piece shadow" is in main-menu Settings and pause Settings, on by default. Turning it off hides the landing ghost and does not change the fall or the hard drop. Blocks stay solid. EditMode 32 passed and PlayMode 5 passed (`Reports\tests\editmode-ghost-setting.log`, `Reports\tests\playmode-ghost-setting.log`). Test ads only. ARMv7 only. Google Play remains not ready. The 09:15 menu APK and `Builds\Blockzara-debug.apk` were not replaced.

## Approved checkpoint — 2026-10-09 09:15 (UTC+4)

The owner approved the square PLAY menu. The adopted debug build is `Builds\Blockzara-debug-ui-v3-2.apk` (54,901,086 bytes, 2026-10-09 09:10:47), installed on Samsung `RF8M91BJBJW`. A frozen copy is `Backups\2026-10-09-approved-menu-v32`, with `APPROVED.txt`.

PLAY is a square using `play-button.png`, slightly transparent. The three feature tiles stay locked and are more transparent. Falling Blocks rules were not changed. EditMode 32 passed and PlayMode 5 passed after this menu (`Reports\tests\editmode-ui-v32.log`, `Reports\tests\playmode-ui-v32.log`). Device frame: `outputs\ui-v32\01-home-later.png`. Test ads only. ARMv7 only. Google Play remains not ready. `Builds\Blockzara-debug.apk` was not replaced.

Checkpoint: 2026-10-08 14:10 (UTC+4).

## Baseline

| Item | Recorded value |
| --- | --- |
| Workspace | `D:\Projects\Blockzara` |
| Unity | 6000.6.0f1 (`f7f8ed4d1e24`) |
| Package | `com.blockzara.game` |
| Product | BLOCKZARA 0.1.0, version code 1 |
| Orientation | Portrait |
| Minimum SDK | 26 |
| Target SDK | 0 (automatic) |
| Scripting | IL2CPP |
| CPU architecture | ARMv7 only (`AndroidTargetArchitectures: 1`). Not changed. |
| APK | `Builds\Blockzara-debug.apk`, 34,872,772 bytes, 2026-10-08 14:07:28. Build log: `Reports\tests\android-v3.log`, “Build Finished, Result: Success.” |
| EditMode | 26 passed, 0 failed. `Reports\tests\editmode.xml`, start 2026-10-08 09:47:59Z. Log: “Test run completed. Exiting with code 0 (Ok). Run completed.” |
| PlayMode | 5 passed, 0 failed. `Reports\tests\playmode.xml`, start 2026-10-08 10:04:49Z. Same completion line. This run compiled the final runtime scripts. |
| Editor frames | `outputs\visual-v2-review\01-main-menu.png` through `05-game-over.png`, 720×1520, 2026-10-08 14:02–14:03 |
| Device | Samsung `RF8M91BJBJW`. Physical 1440×3040, override 720×1520, density override 280. Install Success. |
| Device frames | `outputs\device-v3\01-menu.png`, `02-gameplay.png`, `03-pause.png`, each 720×1520 |
| Baseline backup | `Backups\2026-10-08-baseline-v51` left unchanged |
| Drag-and-place backup | `Backups\2026-10-08-drag-and-place` left unchanged |
| Test runner | `-runTests` was not combined with `-quit`. The APK build used `-quit`. |

The windowed capture launch stalled after licensing with no frames. The frames above were captured from an editor play session started with `-batchmode` and without `-nographics`. They were inspected after the files were written.

## Active code

- `Assets\Blockzara\Core\FallingCore.cs` — same falling rules. `LastClearCount` and `LastClearRow` report the rows already cleared by the existing lock. They do not change score.
- `Assets\Blockzara\Runtime\BlockzaraApp.cs` — input timings unchanged. Drawing, HUD, themes, skins, and the daily run live here.
- `Assets\Blockzara\Runtime\GameplayFx.cs` — visual motion, clear flash, landing pulse, banners. Reduced effects snaps them off.
- `Assets\Blockzara\Runtime\GameAudio.cs` — synthesized tones, including the four-line clear. No external audio files.
- `Assets\Blockzara\Runtime\LocalProgressService.cs` — best score, career stats, settings, language, theme, skin, daily streak, and eight local achievements.
- `Assets\Blockzara\Runtime\UiLanguage.cs` — Arabic, English, and Urdu labels. Arabic is the default.
- `Assets\Blockzara\Runtime\BoardMood.cs` — theme and level lighting colors.
- `Assets\Blockzara\Runtime\BlockzaraMenuV5.cs` — square PLAY menu. Settings, Stats, Achievements, Themes, Daily, and Skins open. World Map stays locked.
- `Assets\Blockzara\Icons\app-icon.png` — approved launcher icon, the block Z over the coast.

## Missing owner assets

These six original PNGs are still not in the project. The menu still uses the sprites already in `Assets\Resources\MainMenuV5`. Those files were not relabeled as the owner originals.

1. BLOCKZARA 3D logo PNG — MISSING OWNER ASSET
2. Glossy green PLAY button PNG — MISSING OWNER ASSET
3. Gold World Map icon PNG — MISSING OWNER ASSET
4. Gold Achievements trophy PNG — MISSING OWNER ASSET
5. Themes palette PNG — MISSING OWNER ASSET
6. Purple horizontal button PNG — MISSING OWNER ASSET

## Phase checklist

- [x] Phase 0 — Baseline resumed. Backup preserved.
- [x] Phase 1 — Menu kept working. PLAY opens Falling Blocks. Three locked squares remain. Owner originals are missing, so this is not a final-art import.
- [x] Phase 2 — Gameplay visual pass on the existing rules. Editor and device frames captured.
- [x] Phase 3 — Presentation effects only. A test checks that the effect object does not change score or grid position.
- [x] Phase 4 — Audio events, local Music / SFX / vibration settings, and synthesized fallback. See `Docs\BLOCKZARA_AUDIO_ASSETS.md`.
- [x] Phase 5 — Local stats and achievements. Settings, Stats, and Achievements navigation opens. Themes stays locked.
- [x] Phase 6 — Economy design written. Not implemented. `Docs\BLOCKZARA_ECONOMY_DESIGN.md`.
- [x] Phase 7 — Mode specs written. Not implemented. `Docs\BLOCKZARA_MODES_SPEC.md`.
- [x] Phase 8 — Ads plan written. No SDK and no ad request. `Docs\BLOCKZARA_ADS_PLAN.md`.
- [x] Phase 9 — Debug APK built and installed. ARM64 proposal only. `Docs\BLOCKZARA_ARM64_PROPOSAL.md`.
- [x] Phase 10 — Owner review written. `Docs\BLOCKZARA_OWNER_REVIEW.md`.

## Owner decisions still required

- Supply the six original PNGs, or tell the project to keep the current menu sprites.
- ARM64, and whether to keep ARMv7.
- Target Challenge and Timed Challenge rules. The local 20-line daily challenge is already in the approved build.
- Local coin rewards. The three skins are free and already in the build. There is no shop.
- Production ad-unit ids. The approved debug build uses test banners only.
- A signed release and Play upload.

## Recommendation

NOT READY for Google Play. The approved debug APK is installed on the Samsung. Do not publish, push, or replace the test banners with production ads.
