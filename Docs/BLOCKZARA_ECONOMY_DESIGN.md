# BLOCKZARA local economy design

Status: proposal only. Not implemented. No billing, no backend, no real-money claims.

## Proposed rules for approval

- Coins are a local integer, key `blockzara.coins`, starting at 0.
- A classic run awards coins only when the run ends: 1 coin per 500 score, plus 5 coins for a four-line clear during that run. No coins for unfinished pauses.
- The shop sells cosmetic block skins, well frames, and drop trails. Prices are data on the device, starting suggestion: 40, 80, and 120 coins.
- Buying marks an item owned. Equipping changes presentation only. The falling rules, bag, kicks, and scoring stay the same.
- Owned items and the equipped ids are stored in PlayerPrefs and migrate with `blockzara.save.version`.
- The Skins and Themes buttons stay locked until this design is approved and the screens actually equip a cosmetic.

## Not in this design

- Real-money purchases, Google Play Billing, receipts, or a remote balance.
- Ads as a coin source. That stays in the ads plan until separately approved.
- Limited-time items that the client cannot verify.
