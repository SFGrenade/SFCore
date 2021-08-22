using System.Linq;
using SFCore.Utils;
using UnityEngine;
using UnityEngine.Audio;
using Logger = Modding.Logger;

namespace SFCore.MonoBehaviours
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class PatchMusicRegions : MonoBehaviour
    {
        private static AudioMixer _am = null;

        public bool useAlts = false;
        public static bool altMusic = false;

        public string SnapshotName = "Silent";
        public int SnapshotIndex = 9;

        public string MusicRegionSet = "";

        public AudioClip Main;
        public AudioClip Action;
        public AudioClip Sub;
        public AudioClip Tension;
        public AudioClip MainAlt;
        public AudioClip Extra;

        public AudioClip Main2;
        public AudioClip Action2;
        public AudioClip Sub2;
        public AudioClip Tension2;
        public AudioClip MainAlt2;
        public AudioClip Extra2;

        public bool Dirtmouth = false;
        public bool MinesDelay = false;
        public string EnterTrackEvent = "CROSSROADS";
        public float EnterTransitionTime = 3f;

        public void Start()
        {
            gameObject.SetActive(false);
            if (_am == null)
            {
                _am = Resources.FindObjectsOfTypeAll<AudioMixer>().First(x => x.name == "Music");
                Log(_am);
            }

            var snapshot = _am.FindSnapshot(SnapshotName);

            Log(snapshot);

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

        private static void Log(string message)
        {
            Logger.Log($"[SFCore]:[MonoBehaviours]:[PatchMusicRegions] - {message}");
        }
        private static void Log(object message)
        {
            Logger.Log($"[SFCore]:[MonoBehaviours]:[PatchMusicRegions] - {message}");
        }
    }
}
