using System.Collections.Generic;
using GlucoseWar.Core;
using UnityEngine;

namespace GlucoseWar
{
    public enum SfxId { Shoot, Hit, Explosion, Pickup, Upgrade, Ultimate, UIClick, LowHealth }

    /// <summary>音频：程序生成的占位音效 + 简易低频氛围 BGM。无需外部音频文件。</summary>
    public class AudioManager : Singleton<AudioManager>
    {
        private readonly Dictionary<SfxId, AudioClip> clips = new Dictionary<SfxId, AudioClip>();
        private AudioSource[] sfxSources;
        private int sfxIndex;
        private AudioSource bgmSource;
        private float lastShoot;

        [Range(0f, 1f)] public float masterVolume = 0.6f;
        [Range(0f, 1f)] public float sfxVolume = 0.5f;
        [Range(0f, 1f)] public float bgmVolume = 0.3f;

        protected override void Awake()
        {
            base.Awake();
            DontDestroyOnLoad(gameObject);

            sfxSources = new AudioSource[8];
            for (int i = 0; i < sfxSources.Length; i++)
            {
                sfxSources[i] = gameObject.AddComponent<AudioSource>();
                sfxSources[i].playOnAwake = false;
            }
            bgmSource = gameObject.AddComponent<AudioSource>();
            bgmSource.loop = true;
            bgmSource.playOnAwake = false;

            BuildClips();
        }

        private void BuildClips()
        {
            clips[SfxId.Shoot] = Tone(880f, 0.06f, 0.25f, WaveKind.Square);
            clips[SfxId.Hit] = Tone(160f, 0.18f, 0.5f, WaveKind.Noise);
            clips[SfxId.Explosion] = Tone(90f, 0.3f, 0.6f, WaveKind.Noise);
            clips[SfxId.Pickup] = Tone(1320f, 0.12f, 0.4f, WaveKind.Sine);
            clips[SfxId.Upgrade] = Sweep(660f, 1320f, 0.25f, 0.5f);
            clips[SfxId.Ultimate] = Sweep(220f, 1760f, 0.5f, 0.7f);
            clips[SfxId.UIClick] = Tone(520f, 0.05f, 0.4f, WaveKind.Square);
            clips[SfxId.LowHealth] = Tone(440f, 0.2f, 0.45f, WaveKind.Sine);
        }

        private enum WaveKind { Sine, Square, Noise }

        private AudioClip Tone(float freq, float duration, float amp, WaveKind kind)
        {
            int rate = 44100;
            int samples = Mathf.Max(1, (int)(rate * duration));
            var clip = AudioClip.Create($"t{freq}", samples, 1, rate, false);
            float[] data = new float[samples];
            for (int i = 0; i < samples; i++)
            {
                float t = (float)i / rate;
                float env = Mathf.Exp(-6f * (t / duration));
                float v;
                switch (kind)
                {
                    case WaveKind.Square: v = Mathf.Sign(Mathf.Sin(2f * Mathf.PI * freq * t)); break;
                    case WaveKind.Noise: v = Random.Range(-1f, 1f); break;
                    default: v = Mathf.Sin(2f * Mathf.PI * freq * t); break;
                }
                data[i] = v * amp * env;
            }
            clip.SetData(data, 0);
            return clip;
        }

        private AudioClip Sweep(float fromHz, float toHz, float duration, float amp)
        {
            int rate = 44100;
            int samples = Mathf.Max(1, (int)(rate * duration));
            var clip = AudioClip.Create("sweep", samples, 1, rate, false);
            float[] data = new float[samples];
            float phase = 0f;
            for (int i = 0; i < samples; i++)
            {
                float n = (float)i / samples;
                float f = Mathf.Lerp(fromHz, toHz, n);
                phase += 2f * Mathf.PI * f / rate;
                float env = Mathf.Sin(Mathf.PI * n);
                data[i] = Mathf.Sin(phase) * amp * env;
            }
            clip.SetData(data, 0);
            return clip;
        }

        public void PlaySfx(SfxId id)
        {
            if (id == SfxId.Shoot)
            {
                if (Time.unscaledTime - lastShoot < 0.05f) return;
                lastShoot = Time.unscaledTime;
            }
            if (!clips.TryGetValue(id, out var clip) || clip == null) return;
            var src = sfxSources[sfxIndex];
            sfxIndex = (sfxIndex + 1) % sfxSources.Length;
            src.volume = masterVolume * sfxVolume * (id == SfxId.Shoot ? 0.5f : 1f);
            src.PlayOneShot(clip);
        }

        // BGM：用低频正弦 drone 作占位氛围
        public void PlayMenuBgm() => PlayDrone(110f, 0.18f);
        public void PlayLevelBgm(int levelIndex) => PlayDrone(70f + levelIndex * 12f, 0.2f);
        public void PlayBossBgm() => PlayDrone(55f, 0.25f);

        private void PlayDrone(float baseHz, float amp)
        {
            int rate = 44100;
            int samples = rate * 2;
            var clip = AudioClip.Create("drone", samples, 1, rate, false);
            float[] data = new float[samples];
            for (int i = 0; i < samples; i++)
            {
                float t = (float)i / rate;
                float v = Mathf.Sin(2f * Mathf.PI * baseHz * t) * 0.6f
                        + Mathf.Sin(2f * Mathf.PI * baseHz * 1.5f * t) * 0.3f
                        + Mathf.Sin(2f * Mathf.PI * (baseHz + 2f) * t) * 0.2f;
                data[i] = v * amp;
            }
            clip.SetData(data, 0);
            bgmSource.clip = clip;
            bgmSource.volume = masterVolume * bgmVolume;
            bgmSource.Play();
        }

        public void SetVolumes(float master, float sfx, float bgm)
        {
            masterVolume = master; sfxVolume = sfx; bgmVolume = bgm;
            if (bgmSource != null) bgmSource.volume = masterVolume * bgmVolume;
        }
    }
}
