using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

namespace SFCore.Utils
{
    public static class MiscCreator
    {
        private static AudioMixer musicAM = null;
        private static AudioMixer atmosAM = null;
        private static AudioMixer enviroAM = null;
        private static AudioMixer actorAM = null;
        private static AudioMixer shadeAM = null;

        private static void InitAudioMixers()
        {
            if (musicAM == null)
                musicAM = Resources.FindObjectsOfTypeAll<AudioMixer>().First(x => x.name == "Music");
            if (atmosAM == null)
                atmosAM = Resources.FindObjectsOfTypeAll<AudioMixer>().First(x => x.name == "Atmos");
            if (enviroAM == null)
                enviroAM = Resources.FindObjectsOfTypeAll<AudioMixer>().First(x => x.name == "EnviroEffects");
            if (actorAM == null)
                actorAM = Resources.FindObjectsOfTypeAll<AudioMixer>().First(x => x.name == "Actors");
            if (shadeAM == null)
                shadeAM = Resources.FindObjectsOfTypeAll<AudioMixer>().First(x => x.name == "ShadeMixer");
        }

        public static void ResetSceneManagerAudio(SceneManager sm)
        {
            InitAudioMixers();

            sm.SetAttr<SceneManager, AudioMixerSnapshot>("musicSnapshot", musicAM.FindSnapshot("Silent"));
            sm.atmosSnapshot = atmosAM.FindSnapshot("at None");
            sm.enviroSnapshot = enviroAM.FindSnapshot("en Silent");
            sm.actorSnapshot = actorAM.FindSnapshot("On");
            sm.shadeSnapshot = shadeAM.FindSnapshot("Away");

            sm.SetAttr<SceneManager, AtmosCue>("atmosCue", Resources.FindObjectsOfTypeAll<AtmosCue>().First(x => x.name == "None"));
            sm.SetAttr<SceneManager, MusicCue>("musicCue", Resources.FindObjectsOfTypeAll<MusicCue>().First(x => x.name == "None"));
            sm.SetAttr<SceneManager, MusicCue>("infectedMusicCue", Resources.FindObjectsOfTypeAll<MusicCue>().First(x => x.name == "None"));
        }

        public static void Set(this Vector3 self, double x, double y, double z)
        {
            self.x = (float)x;
            self.y = (float)y;
            self.y = (float)z;
        }
    }
}
