# BLOCKZARA audio assets

Status: no external sound files are in the project.

`GameAudio` synthesizes short original tones at runtime for click, move, rotate, soft drop, hard drop, landing, line clear, combo, level, pause, resume, and game over. Menu and gameplay beds are short original loops made the same way. They are not licensed recordings and they are not a finished soundtrack.

No `.wav`, `.mp3`, or `.ogg` files were added. If a composed soundtrack is wanted later, it needs an original or licensed file and a separate approval. Until then the synthesizer is the fallback, and the Music and Sound effects settings can silence it.

Vibration uses the platform vibrate call only when the local Vibration setting is on. It is off by default. It fires for a four-line clear and for game over, not for every move.
