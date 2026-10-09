using System.Collections.Generic;
using UnityEngine;

namespace Blockzara.Runtime
{
    public static class GameAudio
    {
        static AudioSource music;
        static AudioSource effects;
        static readonly Dictionary<string, AudioClip> clips = new Dictionary<string, AudioClip>();
        static readonly Dictionary<string, float> lastPlayed = new Dictionary<string, float>();
        static bool pausedBySystem;
        static bool ducked;
        static float suppressLandUntil;

        public static bool HasExternalFiles => false;

        public static void Ensure(GameObject host)
        {
            if (music != null) return;
            if (host.GetComponent<AudioListener>() == null) host.AddComponent<AudioListener>();
            music = host.AddComponent<AudioSource>();
            effects = host.AddComponent<AudioSource>();
            music.loop = true;
            music.playOnAwake = false;
            music.spatialBlend = 0f;
            effects.playOnAwake = false;
            effects.loop = false;
            effects.spatialBlend = 0f;
            effects.volume = 1f;
            var click = Resources.Load<AudioClip>("Sfx/click");
            var pop = Resources.Load<AudioClip>("Sfx/pop");
            clips["click"] = click != null ? click : Tick("click", 1680f, 0.045f, 0.72f);
            clips["move"] = pop != null ? pop : Tick("move", 1240f, 0.032f, 0.42f);
            clips["rotate"] = pop != null ? pop : Tick("rotate", 980f, 0.05f, 0.5f);
            clips["soft"] = Tick("soft", 760f, 0.03f, 0.28f);
            var whoosh = Resources.Load<AudioClip>("Sfx/whoosh");
            clips["hard"] = whoosh != null ? whoosh : Drop("hard", 0.14f, 0.8f, true);
            clips["land"] = whoosh != null ? whoosh : Drop("land", 0.09f, 0.7f, false);
            clips["clear"] = Chime("clear", new[] { 523f, 659f, 784f }, 0.09f, 0.62f);
            clips["tetris"] = Chime("tetris", new[] { 523f, 659f, 784f, 1046f }, 0.1f, 0.7f);
            clips["combo"] = Tone("combo", 990f, 0.1f, 0.18f);
            clips["level"] = Tone("level", 1040f, 0.14f, 0.2f);
            clips["pause"] = Tone("pause", 300f, 0.06f, 0.14f);
            clips["resume"] = Tone("resume", 480f, 0.06f, 0.14f);
            var gameOver = Resources.Load<AudioClip>("Sfx/game-over");
            clips["over"] = gameOver != null ? gameOver : Sweep("over", 440f, 140f, 0.28f, 0.22f);
            var recordedMenu = Resources.Load<AudioClip>("Music/menu-acoustic");
            var recordedPlay = Resources.Load<AudioClip>("Music/play-focus");
            clips["menu"] = recordedMenu != null ? recordedMenu : Loop("menu", new[] { 392f, 494f, 440f }, 1.6f, 0.16f);
            clips["play"] = recordedPlay != null ? recordedPlay : Loop("play", new[] { 330f, 392f, 349f, 440f }, 1.8f, 0.14f);
            if (recordedMenu != null) Debug.Log("BZ menu music=" + recordedMenu.length.ToString("0.0"));
            if (recordedPlay != null) Debug.Log("BZ play music=" + recordedPlay.length.ToString("0.0"));
            if (whoosh != null) Debug.Log("BZ whoosh=" + whoosh.length.ToString("0.00"));
            if (click != null) Debug.Log("BZ click=" + click.length.ToString("0.00"));
            if (pop != null) Debug.Log("BZ pop=" + pop.length.ToString("0.00"));
            if (gameOver != null) Debug.Log("BZ over=" + gameOver.length.ToString("0.00"));
        }

        public static void Play(string id)
        {
            if (!LocalProgressService.SfxEnabled || effects == null || pausedBySystem) return;
            if (!clips.TryGetValue(id, out var clip) || clip == null) return;
            var now = Time.unscaledTime;
            if (id == "land" && now < suppressLandUntil) return;
            if (lastPlayed.TryGetValue(id, out var previous) && now - previous < 0.045f) return;
            lastPlayed[id] = now;
            if (id == "hard") suppressLandUntil = now + 0.16f;
            effects.PlayOneShot(clip, 1f);
        }

        public static void PlayBed(bool menuBed)
        {
            if (music == null) return;
            var next = menuBed ? clips["menu"] : clips["play"];
            if (!LocalProgressService.MusicEnabled || pausedBySystem)
            {
                music.Stop();
                return;
            }
            if (music.clip == next && music.isPlaying) return;
            music.clip = next;
            SetBedVolume(menuBed);
            music.Play();
        }

        public static void ApplySettings()
        {
            if (music == null) return;
            if (!LocalProgressService.MusicEnabled || pausedBySystem) music.Stop();
            else if (!music.isPlaying && music.clip != null) music.Play();
            SetBedVolume(music.clip == clips["menu"]);
        }

        static void SetBedVolume(bool menuBed)
        {
            music.volume = menuBed ? (ducked ? 0.1f : 0.32f) : (ducked ? 0.1f : 0.28f);
        }

        public static void Duck(bool enabled)
        {
            ducked = enabled;
            ApplySettings();
        }

        public static void OnAppPause(bool paused)
        {
            pausedBySystem = paused;
            if (music == null) return;
            if (paused) music.Pause();
            else if (LocalProgressService.MusicEnabled) music.UnPause();
        }

        public static void Pulse(bool strong)
        {
            if (!LocalProgressService.VibrationEnabled || pausedBySystem) return;
            if (Application.platform != RuntimePlatform.Android) return;
            Handheld.Vibrate();
        }

        static AudioClip Tick(string name, float freq, float seconds, float volume)
        {
            return Fill(name, seconds, volume, (t, length) =>
            {
                var i = Mathf.FloorToInt(t * 22050f);
                var noise = Hash(i) * Mathf.Exp(-t * 80f);
                var tone = Mathf.Sin(Mathf.PI * 2f * freq * t) * Mathf.Exp(-t * 42f);
                return tone * 0.7f + noise * 0.45f;
            });
        }

        static AudioClip Drop(string name, float seconds, float volume, bool whoosh)
        {
            return Fill(name, seconds, volume, (t, length) =>
            {
                var i = Mathf.FloorToInt(t * 22050f);
                var freq = Mathf.Lerp(whoosh ? 320f : 160f, 68f, Mathf.Clamp01(t / length));
                var body = Mathf.Sin(Mathf.PI * 2f * freq * t) * Mathf.Exp(-t * (whoosh ? 10f : 16f));
                var air = whoosh ? Hash(i) * Mathf.Exp(-t * 18f) * 0.35f : Hash(i) * Mathf.Exp(-t * 46f) * 0.3f;
                return body * 0.85f + air;
            });
        }

        static AudioClip Chime(string name, float[] notes, float noteSeconds, float volume)
        {
            var seconds = noteSeconds + (notes.Length - 1) * noteSeconds * 0.7f;
            return Fill(name, seconds, volume, (t, length) =>
            {
                var sample = 0f;
                for (var n = 0; n < notes.Length; n++)
                {
                    var local = t - n * noteSeconds * 0.7f;
                    if (local < 0f || local > noteSeconds) continue;
                    var env = Mathf.Clamp01(local / 0.012f) * Mathf.Exp(-local * 7f);
                    sample += Mathf.Sin(Mathf.PI * 2f * notes[n] * t) * env;
                    sample += Mathf.Sin(Mathf.PI * 2f * notes[n] * 2f * t) * env * 0.22f;
                }
                return sample * 0.55f;
            });
        }

        static float Hash(int i)
        {
            var n = (i * 1103515245 + 12345) & 0x7fffffff;
            return n / (float)0x3fffffff - 1f;
        }

        static AudioClip Tone(string name, float freq, float seconds, float volume)
        {
            return Fill(name, seconds, volume, (t, length) => Mathf.Sin(Mathf.PI * 2f * freq * t) * Envelope(t, length));
        }

        static AudioClip Sweep(string name, float from, float to, float seconds, float volume)
        {
            return Fill(name, seconds, volume, (t, length) =>
            {
                var freq = Mathf.Lerp(from, to, t / length);
                return Mathf.Sin(Mathf.PI * 2f * freq * t) * Envelope(t, length);
            });
        }

        static AudioClip Loop(string name, float[] notes, float seconds, float volume)
        {
            return Fill(name, seconds, volume, (t, length) =>
            {
                var index = Mathf.Clamp(Mathf.FloorToInt(t / length * notes.Length), 0, notes.Length - 1);
                return Mathf.Sin(Mathf.PI * 2f * notes[index] * t) * 0.8f;
            });
        }

        static AudioClip Fill(string name, float seconds, float volume, System.Func<float, float, float> sample)
        {
            const int rate = 22050;
            var count = Mathf.Max(1, Mathf.CeilToInt(rate * seconds));
            var data = new float[count];
            for (var i = 0; i < count; i++)
            {
                var t = i / (float)rate;
                data[i] = Mathf.Clamp(sample(t, seconds) * volume, -1f, 1f);
            }
            var clip = AudioClip.Create(name, count, 1, rate, false);
            clip.SetData(data, 0);
            return clip;
        }

        static float Envelope(float t, float length)
        {
            var attack = Mathf.Clamp01(t / 0.008f);
            var release = Mathf.Clamp01((length - t) / 0.02f);
            return attack * release;
        }
    }
}
