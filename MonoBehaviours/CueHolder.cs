using System.Collections.Generic;
using SFCore.Utils;
using UnityEngine;
using UnityEngine.Audio;

namespace SFCore.MonoBehaviours;

/// <summary>
/// This class is for managing dynamically created MusicCue and AtmosCue instances
/// </summary>
public static class CueHolder
{
    /// <summary>
    /// Map of MusicCues
    /// </summary>
    public static Dictionary<string, MusicCue> MusicCues = new Dictionary<string, MusicCue>();
    /// <summary>
    /// Map of AtmosCues
    /// </summary>
    public static Dictionary<string, AtmosCue> AtmosCues = new Dictionary<string, AtmosCue>();

    /// <summary>
    /// Get or add an entry to the MusicCue map.
    /// </summary>
    /// <param name="set">The set of MusicCue, will be used to determine if it should be added or retrieved from the map</param>
    /// <param name="snapshot">The Snapshot to use in the cue</param>
    /// <param name="clips">The audio clips for the cue</param>
    /// <param name="syncs">Sync settings for the cue</param>
    /// <returns>The MusicCue.</returns>
    public static MusicCue GetMusicCue(string set, AudioMixerSnapshot snapshot, AudioClip[] clips, SceneManagerPatcher.MusicChannelSync[] syncs)
    {
        if (!MusicCues.ContainsKey(set))
        {
            var tmpMc = ScriptableObject.CreateInstance<MusicCue>();
            tmpMc.SetAttr("originalMusicEventName", "CROSSROADS");
            tmpMc.SetAttr("originalMusicTrackNumber", 2);
            tmpMc.SetAttr("snapshot", snapshot);
            tmpMc.SetAttr<MusicCue, MusicCue.Alternative[]>("alternatives", null);
            MusicCue.MusicChannelInfo[] musicChannelInfos = new[]
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
            tmpMc.SetAttr("channelInfos", musicChannelInfos);
            MusicCues.Add(set, tmpMc);
        }
        return MusicCues[set];
    }
    /// <summary>
    /// Get or add an entry to the AtmosCue map.
    /// </summary>
    /// <param name="set">The set of AtmosCue, will be used to determine if it should be added or retrieved from the map</param>
    /// <param name="snapshot">The Snapshot to use in the cue</param>
    /// <param name="isChannelEnabled">Check if each channel is enabled</param>
    /// <returns>The AtmosCue.</returns>
    public static AtmosCue GetAtmosCue(string set, AudioMixerSnapshot snapshot, bool[] isChannelEnabled)
    {
        if (!AtmosCues.ContainsKey(set))
        {
            var tmpAc = ScriptableObject.CreateInstance<AtmosCue>();
            tmpAc.SetAttr("snapshot", snapshot);
            tmpAc.SetAttr("isChannelEnabled", isChannelEnabled);
            AtmosCues.Add(set, tmpAc);
        }
        return AtmosCues[set];
    }
}