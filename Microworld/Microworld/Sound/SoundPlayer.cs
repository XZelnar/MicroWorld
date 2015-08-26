using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MicroWorld.Sound
{
    public static class SoundPlayer
    {
        public enum MusicType
        {
            Game = 0,
            GUI = 1, 
            FX = 2
        }

        static List<MicroWorld.Sound.EffectInstance> musics = new List<EffectInstance>(),
                                             effects = new List<EffectInstance>();
        public static void PlayBackground()
        {
            if (Main.curState.StartsWith("GUI"))
            {
                if (musics.Count > 0)
                {
                    for (int i = 0; i < musics.Count; i++)
                    {
                        if (musics[i].effect != EffectInstance.Effects.FadeOut)
                        {
                            if (GetMusicType(musics[i]) == MusicType.GUI) return;
                            musics[i].FadeOut();
                        }
                    }
                }
                var a = PlayGUIBG();
                //a.FadeIn();
                a.Volume = 1;
                musics.Add(a);
            }
            else
            {
                if (musics.Count > 0)
                {
                    for (int i = 0; i < musics.Count; i++)
                    {
                        if (musics[i].effect != EffectInstance.Effects.FadeOut)
                        {
                            if (GetMusicType(musics[i]) == MusicType.Game) return;
                            musics[i].FadeOut();
                        }
                    }
                }
                var a = PlayGameBG();
                a.OriginalVolume = 1;
                a.FadeIn();
                musics.Add(a);
            }
        }

        public static void ChangeGameBG()
        {
            if (musics.Count > 0)
            {
                for (int i = 0; i < musics.Count; i++)
                {
                    if (musics[i].effect != EffectInstance.Effects.FadeOut)
                    {
                        musics[i].FadeOut();
                    }
                }
            }
            var a = PlayGameBG();
            a.FadeIn();
            musics.Add(a);
        }

        public static MusicType GetMusicType(MicroWorld.Sound.EffectInstance e)
        {
            //if (e.parent == Sounds.main1 || e.parent == Sounds.main2) return MusicType.GUI;
            if (e.parent == Sounds.main1) return MusicType.GUI;
            if (e.parent == Sounds.game1 || e.parent == Sounds.game2 || e.parent == Sounds.game3) return MusicType.Game;
            return MusicType.FX;
        }

        public static void Update()
        {
            for (int i = 0; i < musics.Count; i++)
            {
                if (musics[i].effect == EffectInstance.Effects.None)
                {
                    musics[i].Volume = Settings.MusicVolume * Settings.MasterVolume;
                }
                if (musics[i].Volume > Settings.MusicVolume * Settings.MasterVolume)
                {
                    musics[i].Volume = Settings.MusicVolume * Settings.MasterVolume;
                }
                if (musics[i].State == Microsoft.Xna.Framework.Audio.SoundState.Stopped)
                {
                    musics.RemoveAt(i);
                    i--;
                }
            }
            for (int i = 0; i < effects.Count; i++)
            {
                if (effects[i].effect == EffectInstance.Effects.None)
                {
                    if (effects[i].Volume > Settings.EffectsVolume * Settings.EffectsVolume)
                        effects[i].Volume = Settings.EffectsVolume * Settings.MasterVolume;
                }
                if (effects[i].Volume > Settings.EffectsVolume * Settings.EffectsVolume)
                {
                    effects[i].Volume = Settings.EffectsVolume * Settings.EffectsVolume;
                }
                if (effects[i].State == Microsoft.Xna.Framework.Audio.SoundState.Stopped)
                {
                    effects.RemoveAt(i);
                    i--;
                }
            }
        }

        #region PlayFuncs
        public static void PlayButtonClick()
        {
            //var a = Sounds.menuClickSound.Play(Settings.EffectsVolume * Settings.MasterVolume,
            //    0.9f + (float)(new Random().NextDouble() - 0.5f) / 5f, 0f);
            var a = Sounds.menuClickSound.Play(Settings.EffectsVolume * Settings.MasterVolume,
                1f, 0f);
            effects.Add(a);
        }

        public static MicroWorld.Sound.EffectInstance PlayGUIBG()
        {
            return Sounds.main1.Play(0, true);
            PUBStart:
            switch (new Random().Next(2))
            {
                case 0:
                    if (!HasBGPlaying(ref Sounds.main1))
                        return Sounds.main1.Play(0, true);
                    goto PUBStart;
                case 1:
                    //if (!HasBGPlaying(ref Sounds.main2))
                    //    return Sounds.main2.Play(0, true);
                    goto PUBStart;
                default:
                    return null;
            }
        }

        public static MicroWorld.Sound.EffectInstance PlayGameBG()
        {
            PBGStart:
            switch (new Random().Next(3))
            {
                case 0:
                    if (!HasBGPlaying(ref Sounds.game1))
                        return Sounds.game1.Play(0, true);
                    goto PBGStart;
                    //return PlayGameBG();
                case 1:
                    if (!HasBGPlaying(ref Sounds.game2))
                        return Sounds.game2.Play(0, true);
                    goto PBGStart;
                //return PlayGameBG();
                case 2:
                    if (!HasBGPlaying(ref Sounds.game3))
                        return Sounds.game3.Play(0, true);
                    goto PBGStart;
                default:
                    return null;
            }
        }

        public static bool HasBGPlaying(ref Sound s)
        {
            for (int i = 0; i < musics.Count; i++)
            {
                if (musics[i].parent == s) return true;
            }
            return false;
        }

        public static void ItemFadeIn()
        {
            var a = Sounds.fadeIn.Play(1f, false);
            a.Volume = Settings.EffectsVolume * Settings.MasterVolume;
            effects.Add(a);
        }

        public static void ItemFadeOut()
        {
            var a = Sounds.fadeOut.Play(1f, false);
            a.Volume = Settings.EffectsVolume * Settings.MasterVolume;
            effects.Add(a);
        }

        public static void MenuMouseOver()
        {
            var a = Sounds.menuMouseOver.Play(1f, false);
            a.Volume = Settings.EffectsVolume * Settings.MasterVolume;
            effects.Add(a);
        }

        public static void ComponentPlaced()
        {
            var a = Sounds.componentPlacement.Play(Settings.EffectsVolume * Settings.MasterVolume, 0.5f, 0f);
            effects.Add(a);
        }
        #endregion

    }
}
