using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

namespace SFCore.Utils
{
    public static class MiscCreator
    {
        private static AudioMixer _musicAm = null;
        private static AudioMixer _atmosAm = null;
        private static AudioMixer _enviroAm = null;
        private static AudioMixer _actorAm = null;
        private static AudioMixer _shadeAm = null;

        private static void InitAudioMixers()
        {
            if (_musicAm == null)
                _musicAm = Resources.FindObjectsOfTypeAll<AudioMixer>().First(x => x.name == "Music");
            if (_atmosAm == null)
                _atmosAm = Resources.FindObjectsOfTypeAll<AudioMixer>().First(x => x.name == "Atmos");
            if (_enviroAm == null)
                _enviroAm = Resources.FindObjectsOfTypeAll<AudioMixer>().First(x => x.name == "EnviroEffects");
            if (_actorAm == null)
                _actorAm = Resources.FindObjectsOfTypeAll<AudioMixer>().First(x => x.name == "Actors");
            if (_shadeAm == null)
                _shadeAm = Resources.FindObjectsOfTypeAll<AudioMixer>().First(x => x.name == "ShadeMixer");
        }

        public static void ResetSceneManagerAudio(SceneManager sm)
        {
            InitAudioMixers();

            sm.SetAttr("musicSnapshot", _musicAm.FindSnapshot("Silent"));
            sm.atmosSnapshot = _atmosAm.FindSnapshot("at None");
            sm.enviroSnapshot = _enviroAm.FindSnapshot("en Silent");
            sm.actorSnapshot = _actorAm.FindSnapshot("On");
            sm.shadeSnapshot = _shadeAm.FindSnapshot("Away");

            sm.SetAttr("atmosCue", Resources.FindObjectsOfTypeAll<AtmosCue>().First(x => x.name == "None"));
            sm.SetAttr("musicCue", Resources.FindObjectsOfTypeAll<MusicCue>().First(x => x.name == "None"));
            sm.SetAttr("infectedMusicCue", Resources.FindObjectsOfTypeAll<MusicCue>().First(x => x.name == "None"));
        }

        public static void Set(this Vector3 self, double x, double y, double z)
        {
            self.x = (float)x;
            self.y = (float)y;
            self.y = (float)z;
        }
    }
}
