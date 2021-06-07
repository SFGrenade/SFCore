using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HutongGames.PlayMaker.Actions;
using SFCore.Utils;
using UnityEngine;
using UnityEngine.Audio;

namespace SFCore.MonoBehaviours
{
    internal static class CueHolder
    {
        public static Dictionary<string, MusicCue> MusicCues = new Dictionary<string, MusicCue>();
        public static Dictionary<string, AtmosCue> AtmosCues = new Dictionary<string, AtmosCue>();

        public static MusicCue GetMusicCue(string set, AudioMixerSnapshot snapshot, AudioClip[] clips, SceneManagerPatcher.MusicChannelSync[] syncs)
        {
            if (!MusicCues.ContainsKey(set))
            {
                var tmpMC = ScriptableObject.CreateInstance<MusicCue>();
                tmpMC.SetAttr<MusicCue, string>("originalMusicEventName", "CROSSROADS");
                tmpMC.SetAttr<MusicCue, int>("originalMusicTrackNumber", 2);
                tmpMC.SetAttr<MusicCue, AudioMixerSnapshot>("snapshot", snapshot);
                tmpMC.SetAttr<MusicCue, MusicCue.Alternative[]>("alternatives", null);
                MusicCue.MusicChannelInfo[] musicChannelInfos = new MusicCue.MusicChannelInfo[]
                {
                    new MusicCue.MusicChannelInfo(), new MusicCue.MusicChannelInfo(),
                    new MusicCue.MusicChannelInfo(), new MusicCue.MusicChannelInfo(),
                    new MusicCue.MusicChannelInfo(), new MusicCue.MusicChannelInfo()
                };
                musicChannelInfos[(int) MusicChannels.Main].SetAttr("clip", clips[(int) MusicChannels.Main]);
                musicChannelInfos[(int) MusicChannels.Action].SetAttr("clip", clips[(int) MusicChannels.Action]);
                musicChannelInfos[(int) MusicChannels.Sub].SetAttr("clip", clips[(int) MusicChannels.Sub]);
                musicChannelInfos[(int) MusicChannels.Tension].SetAttr("clip", clips[(int) MusicChannels.Tension]);
                musicChannelInfos[(int) MusicChannels.MainAlt].SetAttr("clip", clips[(int) MusicChannels.MainAlt]);
                musicChannelInfos[(int) MusicChannels.Extra].SetAttr("clip", clips[(int) MusicChannels.Extra]);
                musicChannelInfos[(int) MusicChannels.Main].SetAttr("sync", (MusicChannelSync) syncs[(int) MusicChannels.Main]);
                musicChannelInfos[(int) MusicChannels.Action].SetAttr("sync", (MusicChannelSync) syncs[(int) MusicChannels.Action]);
                musicChannelInfos[(int) MusicChannels.Sub].SetAttr("sync", (MusicChannelSync) syncs[(int) MusicChannels.Sub]);
                musicChannelInfos[(int) MusicChannels.Tension].SetAttr("sync", (MusicChannelSync) syncs[(int) MusicChannels.Tension]);
                musicChannelInfos[(int) MusicChannels.MainAlt].SetAttr("sync", (MusicChannelSync) syncs[(int) MusicChannels.MainAlt]);
                musicChannelInfos[(int) MusicChannels.Extra].SetAttr("sync", (MusicChannelSync) syncs[(int) MusicChannels.Extra]);
                tmpMC.SetAttr<MusicCue, MusicCue.MusicChannelInfo[]>("channelInfos", musicChannelInfos);
                MusicCues.Add(set, tmpMC);
            }
            return MusicCues[set];
        }
        public static AtmosCue GetAtmosCue(string set, AudioMixerSnapshot snapshot, bool[] isChannelEnabled)
        {
            if (!AtmosCues.ContainsKey(set))
            {
                var tmpAC = ScriptableObject.CreateInstance<AtmosCue>();
                tmpAC.SetAttr("snapshot", snapshot);
                tmpAC.SetAttr("isChannelEnabled", isChannelEnabled);
                AtmosCues.Add(set, tmpAC);
            }
            return AtmosCues[set];
        }
    }
}
