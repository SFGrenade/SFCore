using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

namespace SFCore.Utils
{
    /// <summary>
    ///     Utils specifically for creating or resetting Hollow Knight MonoBehaviours.
    /// </summary>
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

        /// <summary>
        ///     Resets AudioMixerSnapshots and Cues on a SceneManager.
        /// </summary>
        /// <param name="sm">The SceneManager</param>
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

        /// <summary>
        ///     Sets X/Y/Z on a given Vector3.
        /// </summary>
        /// <param name="self">The Vector3</param>
        /// <param name="x">X</param>
        /// <param name="y">Y</param>
        /// <param name="z">Z</param>
        public static void Set(this Vector3 self, double x, double y, double z)
        {
            self.x = (float)x;
            self.y = (float)y;
            self.y = (float)z;
        }

        /// <summary>
        ///     Sets the original music event name of a MusicCue.
        /// </summary>
        /// <param name="cue">The Cue</param>
        /// <param name="name">The name</param>
        public static void setOriginalMusicEventName(this MusicCue cue, string name)
        {
            cue.SetAttr<MusicCue, string>("originalMusicEventName", name);
        }
        /// <summary>
        ///     Sets the original music track number of a MusicCue.
        /// </summary>
        /// <param name="cue">The Cue</param>
        /// <param name="number">The number</param>
        public static void setOriginalMusicTrackNumber(this MusicCue cue, int number)
        {
            cue.SetAttr<MusicCue, int>("originalMusicTrackNumber", number);
        }
        /// <summary>
        ///     Sets the AudioMixerSnapshot of a MusicCue.
        /// </summary>
        /// <param name="cue">The Cue</param>
        /// <param name="snapshot">The AudioMixerSnapshot</param>
        public static void setSnapshot(this MusicCue cue, AudioMixerSnapshot snapshot)
        {
            cue.SetAttr<MusicCue, AudioMixerSnapshot>("snapshot", snapshot);
        }
        /// <summary>
        ///     Gets the channel infos of a MusicCue.
        /// </summary>
        /// <param name="cue">The Cue</param>
        /// <returns>The MusicChannelInfos.</returns>
        public static MusicCue.MusicChannelInfo[] getChannelInfos(this MusicCue cue)
        {
            return cue.GetAttr<MusicCue, MusicCue.MusicChannelInfo[]>("channelInfos");
        }
        /// <summary>
        ///     Sets the channel infos of a MusicCue.
        /// </summary>
        /// <param name="cue">The Cue</param>
        /// <param name="infos">The infos</param>
        public static void setChannelInfos(this MusicCue cue, MusicCue.MusicChannelInfo[] infos)
        {
            cue.SetAttr<MusicCue, MusicCue.MusicChannelInfo[]>("channelInfos", infos);
        }
    }
}
