# BLOCKZARA ARM64 proposal

Status: not applied. `ProjectSettings` still has `AndroidTargetArchitectures: 1` (ARMv7 only).

## Current package

- Identifier `com.blockzara.game`
- Version `0.1.0`, version code 1
- Minimum SDK 26
- Target SDK 0, which means the editor’s selected automatic target
- Scripting backend IL2CPP
- Orientation portrait
- The IL2CPP Android build warns that this setup has no 64-bit native library

## Proposed change, after approval

- Set Android architectures to ARMv7 plus ARM64 (value 3), or ARM64 only if 32-bit installs are no longer required.
- Confirm the installed Android SDK target meets the current Play target requirement before changing `AndroidTargetSdkVersion`.
- Rebuild a debug APK and install it on the Samsung before any release build.

This file does not change the build settings.
