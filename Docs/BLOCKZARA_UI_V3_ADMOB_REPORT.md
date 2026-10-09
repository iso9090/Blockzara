# BLOCKZARA — UI V3 + ADMOB REPORT

Date: 2026-10-08. Project: `D:\Projects\Blockzara`. Unity 6000.6.0f1. No commit, no push, no deploy, no Google Play upload.

Owner visual approval: REQUIRED.

## Main Menu PLAY

The green PLAY button is unchanged as navigation (`StartFromMenu`). The white triangle sprite was replaced by a generated circular badge: gold ring, emerald center, white play triangle, highlight. Press scales the badge to 0.96. The approved logo and scenic background were not replaced. The badge is generated in `UiSprites.PlayBadge`. It is not one of the six missing owner-original PNGs.

Device: `outputs/ui-v3/01-home.png`, `outputs/ui-v3/11-home-return.png`.

## Pause Panel

Centered navy card with a gold frame, titled GAME PAUSED. Buttons: RESUME (play badge), RESTART (circular arrow), SETTINGS (gear), MAIN MENU (house). One panel. Gameplay IMGUI is not drawn while this overlay is open, so the panel is not covered by the board. The overlay draws the existing scenic background behind a dim.

The first device build drew the panel under the gameplay IMGUI, so only the top PAUSE/RESUME label changed. That build was replaced. The installed build shows the panel: `outputs/ui-v3/05-pause.png`, `outputs/ui-v3/08-after-restart-no.png`.

## Resume

RESUME closes the panel and continues the same run. The top control returns to PAUSE. Device: `outputs/ui-v3/10-resumed.png`.

## Restart Confirmation

RESTART opens "Restart this game?" with YES and NO. NO returns to GAME PAUSED and does not start a new run. YES starts a new run and keeps BEST. Device: `outputs/ui-v3/06-restart-confirm.png`, `outputs/ui-v3/08-after-restart-no.png`, `outputs/ui-v3/12-after-restart-yes.png` (empty board, new piece, BEST still 2042).

## Main Menu Navigation

MAIN MENU opens "Leave this game and return to Main Menu?" YES returns to Home and shows the Home banner. NO stays on the pause panel. Android Back from gameplay opens the pause panel. Android Back from a confirmation or from pause Settings returns to the pause panel. Android Back on the pause panel resumes. Device: `outputs/ui-v3/09-leave-confirm.png`, `outputs/ui-v3/11-home-return.png`.

## Settings

Home Settings and pause Settings use navy rows and gold text. Music, Sound effects, and Vibration persist through `LocalProgressService`. Existing synthesized `GameAudio` is unchanged. No external audio SDK. Pause Settings BACK returns to the pause panel, not Home. Home Settings BACK returns to Home. Device: `outputs/ui-v3/03-settings.png`, `outputs/ui-v3/07-pause-settings.png`.

## Themes

The Themes nav button opens a Themes screen. Current theme is Coastal, with a preview of `scenic-background-v1` and SELECTED. Harbor and Night are LOCKED / COMING SOON. No coins, purchases, or unlocks. The live background is unchanged. BACK returns to Home. Device: `outputs/ui-v3/02-themes.png`.

## Gameplay Controls

Unchanged order and bindings: LEFT blue, ROTATE gold, RIGHT blue, SOFT DROP green, HARD DROP red. Large targets, pressed darkening, safe-area inset. Falling Blocks rules, scoring, difficulty, and board size were not changed.

## HUD Clipping

SCORE, BEST, LINES, and LEVEL sit inside the gold/navy chips on the 720x1520 device capture. NEXT stays visible above the board. Device: `outputs/ui-v3/10-resumed.png`, `outputs/ui-v3/12-after-restart-yes.png`.

## AdMob Plugin

Official package `com.google.ads.mobile` 11.5.0 from OpenUPM (`https://package.openupm.com`, scope `com.google`), plus `com.google.external-dependency-manager` 1.2.187. No second ads SDK. Standard GMA Android SDK. Test App ID `ca-app-pub-3940256099942544~3347511713`. Anchored adaptive test banner `ca-app-pub-3940256099942544/9214589741`. No production IDs. No interstitials. No rewarded ads.

## Home Banner

Bottom anchored adaptive test banner. Settings, Stats, Achievements, and Themes stay above it and were tapped successfully. The banner is shown only while Home is the active screen. Device: `outputs/ui-v3/01-home.png`, `outputs/ui-v3/11-home-return.png`.

## Gameplay Banner

Bottom anchored adaptive test banner, below LEFT / ROTATE / RIGHT / SOFT DROP / HARD DROP, not over the board. A 110px reserve is kept even before the ad loads. Device: `outputs/ui-v3/10-resumed.png`, `outputs/ui-v3/12-after-restart-yes.png`. The label on the creative was "AdMob Adaptive Banner" / "Test Ad".

## Test Ad Loading

On Samsung SM-N975F the SDK logged `loadAd()` and `show()`. Home and gameplay captures show a Google test creative. Debug only.

## Banner Lifecycle

Home to gameplay replaces the Home banner with one gameplay banner. Pause and pause Settings keep that one banner. Gameplay to Home replaces it with one Home banner. `BannerAdManager` hides on app pause and shows again on resume when a surface is active and the last load did not fail.

## Safe Area

Banners sit in the bottom band above the gesture area. Menu nav and the five gameplay controls remain fully on screen and tappable at 720x1520.

## Ad Load Failure

`BannerPlacement` keeps the requested surface and records the error. EditMode covers a failed load and a surface switch that does not load twice. A failed or missing ad does not block navigation in that state machine. Offline was not run on the phone; the phone was online and the test creative loaded. The development player also logged a non-fatal Google Mobile Ads telemetry failure: `UnityWebRequest` is not ready when `GlobalExceptionHandler` uploads, and `GoogleMobileAdsNative.Common` is missing during init. The test banner still loaded. The development console draws that exception over the lower edge of Home, Settings, and Themes. It is a development-build overlay, not a second banner.

## IL2CPP

Android scripting backend is IL2CPP (`scriptingBackend.Android: 1`).

## ARM64

The project architecture is still ARMv7 only (`AndroidTargetArchitectures: 1`). A separate local debug APK was built with ARM64 and then the project setting was restored to ARMv7.

- Built: `Builds/Blockzara-debug-ui-v3-arm64.apk`, 39,762,043 bytes, 2026-10-08 22:06.
- Contains `lib/arm64-v8a/libil2cpp.so`, `libmain.so`, `libunity.so`.
- Not installed on the Samsung. Device screenshots are from the ARMv7 APK.

## Play Asset Delivery

`ClassNotFoundException: com.google.android.play.core.assetpacks.AssetPackManager` still appears on launch. The app reached Home and ads. Play Core was not added.

## Android Build

IL2CPP development build succeeded. Log: `Reports/tests/android-ui-v3-pause.log` (`Build Finished, Result: Success`). Min SDK 26. Target SDK automatic (`AndroidTargetSdkVersion: 0`). Signing and version code were not changed. The previous known-good APK was not overwritten.

## APK Path

Tested on device: `Builds/Blockzara-debug-ui-v3.apk`.

Known-good left in place: `Builds/Blockzara-debug.apk` (34,873,061 bytes, 2026-10-08 21:05).

## APK Size

`Builds/Blockzara-debug-ui-v3.apk`: 47,521,258 bytes, 2026-10-08 21:52. ARMv7 (`armeabi-v7a`).

## EditMode

32 passed, 0 failed. `Reports/tests/editmode.xml`, start 2026-10-08 17:54:48Z. Log `Reports/tests/editmode-ui-v3.log`: "Test run completed. Exiting with code 0 (Ok). Run completed."

## PlayMode

5 passed, 0 failed. `Reports/tests/playmode.xml`, start 2026-10-08 18:01:57Z. Log `Reports/tests/playmode-ui-v3.log`: "Test run completed. Exiting with code 0 (Ok). Run completed."

## Samsung Install

`adb install -r -d -t` Success on RF8M91BJBJW (SM-N975F, Android 12). Package `com.blockzara.game`.

## Samsung Screenshots

Real device captures, 720x1520, under `outputs/ui-v3/`:

- `01-home.png` Home with test banner
- `02-themes.png` Themes
- `03-settings.png` Home Settings
- `04-gameplay.png` Gameplay
- `05-pause.png` and `08-after-restart-no.png` GAME PAUSED
- `06-restart-confirm.png` Restart confirmation
- `07-pause-settings.png` Settings opened from pause
- `09-leave-confirm.png` Main Menu confirmation
- `10-resumed.png` resumed gameplay with test banner
- `11-home-return.png` Home after leaving the run
- `12-after-restart-yes.png` gameplay after confirmed restart

## Files Changed

- `Assets/Blockzara/Runtime/BlockzaraApp.cs`
- `Assets/Blockzara/Runtime/BlockzaraMenuV5.cs`
- `Assets/Blockzara/Runtime/BlockzaraPauseOverlay.cs`
- `Assets/Blockzara/Runtime/PauseFlow.cs`
- `Assets/Blockzara/Runtime/BannerAdManager.cs`
- `Assets/Blockzara/Runtime/BannerPlacement.cs`
- `Assets/Blockzara/Runtime/UiSprites.cs`
- `Assets/Blockzara/Editor/BlockzaraBuild.cs`
- `Assets/Blockzara/Editor/AdMobTestConfig.cs`
- `Assets/Blockzara/Editor/Tests/UiV3EditTests.cs`
- `Assets/Blockzara/Scenes/Bootstrap.unity`
- `Assets/GoogleMobileAds/Resources/GoogleMobileAdsSettings.asset`
- `Packages/manifest.json`
- `Packages/packages-lock.json`

Backup before this work: `Backups/2026-10-08-before-ui-v3`. This folder is not a git repository.

## Known Issues

- The six owner-original menu PNGs are still missing. Generated menu art and `PlayBadge` are not those files.
- Google Mobile Ads telemetry throws `UnityWebRequest` / missing `GoogleMobileAdsNative.Common` during development startup. The development console draws the message over the lower screen. Test ads still load. Navigation still works.
- Play Asset Delivery `AssetPackManager` ClassNotFoundException remains on the development player.
- The phone was online, so a live offline ad failure was not captured. The failure path is covered by EditMode only.
- UMP consent and a privacy policy are not in this build. Required before any production ad unit.
- ARM64 was built as a separate APK and was not installed.

## Google Play Readiness

NOT READY. The tested APK is ARMv7 development, test ads only, no production consent, no release signing, and Play Asset Delivery still logs a missing class.

## OWNER VISUAL APPROVAL

REQUIRED.
