using System.Linq;
using SFCore.Utils;
using UnityEngine;
using UnityEngine.Audio;
using Logger = Modding.Logger;

namespace SFCore.MonoBehaviours
{
    /// <summary>
    ///     Patching MusicRegion
    /// </summary>
    [RequireComponent(typeof(BoxCollider2D))]
    public class PatchMusicRegions : MonoBehaviour
    {
        private static AudioMixer _am = null;

        /// <summary>
        ///     Use alternative background music.
        /// </summary>
        public bool useAlts = false;
        /// <summary>
        ///     State of alternative background music.
        /// </summary>
        public static bool altMusic = false;

        /// <summary>
        ///     Snapshot Name.
        /// </summary>
        public string SnapshotName = "Silent";
        /// <summary>
        ///     Snapshot Index.
        /// </summary>
        public int SnapshotIndex = 9;

        /// <summary>
        ///     Music Region Set.
        /// </summary>
        public string MusicRegionSet = "";

        /// <summary>
        ///     Main.
        /// </summary>
        public AudioClip Main;
        /// <summary>
        ///     Action.
        /// </summary>
        public AudioClip Action;
        /// <summary>
        ///     Sub.
        /// </summary>
        public AudioClip Sub;
        /// <summary>
        ///     Tension.
        /// </summary>
        public AudioClip Tension;
        /// <summary>
        ///     Main Alt.
        /// </summary>
        public AudioClip MainAlt;
        /// <summary>
        ///     Extra.
        /// </summary>
        public AudioClip Extra;

        /// <summary>
        ///     Alternative Main.
        /// </summary>
        public AudioClip Main2;
        /// <summary>
        ///     Alternative Action.
        /// </summary>
        public AudioClip Action2;
        /// <summary>
        ///     Alternative Sub.
        /// </summary>
        public AudioClip Sub2;
        /// <summary>
        ///     Alternative Tension.
        /// </summary>
        public AudioClip Tension2;
        /// <summary>
        ///     Alternative Main Alt.
        /// </summary>
        public AudioClip MainAlt2;
        /// <summary>
        ///     Alternative Extra.
        /// </summary>
        public AudioClip Extra2;

        /// <summary>
        ///     Dirtmouth.
        /// </summary>
        public bool Dirtmouth = false;
        /// <summary>
        ///     Mines Delay.
        /// </summary>
        public bool MinesDelay = false;
        /// <summary>
        ///     Enter Track Event.
        /// </summary>
        public string EnterTrackEvent = "CROSSROADS";
        /// <summary>
        ///     Enter Transition Time.
        /// </summary>
        public float EnterTransitionTime = 3f;

        /// <summary>
        ///     Unity method.
        /// </summary>
        public void Start()
        {
            gameObject.SetActive(false);
            if (_am == null)
            {
                _am = Resources.FindObjectsOfTypeAll<AudioMixer>().First(x => x.name == "Music");
            }

            var snapshot = _am.FindSnapshot(SnapshotName);

            var mr = gameObject.AddComponent<MusicRegion>();
            mr.enterMusicSnapshot = snapshot;
            mr.dirtmouth = Dirtmouth;
            mr.minesDelay = MinesDelay;
            mr.enterTrackEvent = EnterTrackEvent;
            mr.enterTransitionTime = EnterTransitionTime;
            mr.exitMusicCue = null;
            mr.exitMusicSnapshot = null;
            mr.exitTrackEvent = "";
            mr.exitTransitionTime = 0;
            if (!CueHolder.MusicCues.ContainsKey(MusicRegionSet))
            {
                var tmpMc = ScriptableObject.CreateInstance<MusicCue>();
                tmpMc.SetAttr("originalMusicEventName", EnterTrackEvent);
                tmpMc.SetAttr("originalMusicTrackNumber", 2);
                tmpMc.SetAttr("snapshot", snapshot);
                tmpMc.SetAttr<MusicCue, MusicCue.Alternative[]>("alternatives", null);
                CueHolder.MusicCues.Add(MusicRegionSet, tmpMc);
            }
            mr.enterMusicCue = CueHolder.MusicCues[MusicRegionSet];

            MusicCue.MusicChannelInfo[] musicChannelInfos = new[]
            {
                new MusicCue.MusicChannelInfo(), new MusicCue.MusicChannelInfo(),
                new MusicCue.MusicChannelInfo(), new MusicCue.MusicChannelInfo(),
                new MusicCue.MusicChannelInfo(), new MusicCue.MusicChannelInfo()
            };

            musicChannelInfos[(int)MusicChannels.Main].SetAttr("sync", MusicChannelSync.Implicit);
            musicChannelInfos[(int)MusicChannels.Action].SetAttr("sync", MusicChannelSync.ExplicitOn);
            musicChannelInfos[(int)MusicChannels.Sub].SetAttr("sync", MusicChannelSync.ExplicitOn);
            musicChannelInfos[(int)MusicChannels.Tension].SetAttr("sync", MusicChannelSync.ExplicitOn);
            musicChannelInfos[(int)MusicChannels.MainAlt].SetAttr("sync", MusicChannelSync.ExplicitOn);
            musicChannelInfos[(int)MusicChannels.Extra].SetAttr("sync", MusicChannelSync.ExplicitOn);

            if (!altMusic || !useAlts)
            {
                musicChannelInfos[(int)MusicChannels.Main].SetAttr("clip", Main);
                musicChannelInfos[(int)MusicChannels.Action].SetAttr("clip", Action);
                musicChannelInfos[(int)MusicChannels.Sub].SetAttr("clip", Sub);
                musicChannelInfos[(int)MusicChannels.Tension].SetAttr("clip", Tension);
                musicChannelInfos[(int)MusicChannels.MainAlt].SetAttr("clip", MainAlt);
                musicChannelInfos[(int)MusicChannels.Extra].SetAttr("clip", Extra);
            }
            else if (useAlts)
            {
                musicChannelInfos[(int)MusicChannels.Main].SetAttr("clip", Main2);
                musicChannelInfos[(int)MusicChannels.Action].SetAttr("clip", Action2);
                musicChannelInfos[(int)MusicChannels.Sub].SetAttr("clip", Sub2);
                musicChannelInfos[(int)MusicChannels.Tension].SetAttr("clip", Tension2);
                musicChannelInfos[(int)MusicChannels.MainAlt].SetAttr("clip", MainAlt2);
                musicChannelInfos[(int)MusicChannels.Extra].SetAttr("clip", Extra2);
            }
            mr.enterMusicCue.SetAttr("channelInfos", musicChannelInfos);
            gameObject.SetActive(true);
        }
    }
}
