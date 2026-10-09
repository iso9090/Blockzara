# BLOCKZARA ads plan

Status: plan only. No AdMob app, no ad-unit ids, no SDK, and no ad requests were added.

## Placement, if later approved

- A banner may sit only in the gap above the system gesture area and below the hard-drop control. It must not cover the well or the five play buttons.
- An interstitial may run only after game over, after the score is visible, and only if the player taps a continue control. It must not run while a piece is falling.
- A rewarded ad, if added, may offer a defined cosmetic or a defined coin grant from the approved economy. The reward text has to match the grant. Closing the ad early grants nothing.
- Cap: at most one interstitial per three completed runs, and not within 90 seconds of the previous one. Rewarded ads are player-initiated only.

## Privacy and audience

- The current game has no account and no age gate. Before any live ad request, decide whether the app is for a general audience or a child-directed audience and implement the matching consent flow.
- Test ads would be a separate build flag. Production ids would not be used for that pass.

Nothing in this plan is active.
