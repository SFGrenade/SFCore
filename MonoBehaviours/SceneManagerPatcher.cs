using GlobalEnums;
using SFCore.Utils;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

namespace SFCore.MonoBehaviours;

/// <summary>
/// Patching SceneManager
/// </summary>
public class SceneManagerPatcher : MonoBehaviour
{
    private static AudioMixer _musicAm = null;
    private static AudioMixer _atmosAm = null;
    private static AudioMixer _enviroAm = null;
    private static AudioMixer _actorAm = null;
    private static AudioMixer _shadeAm = null;

    private static GameObject _borderPrefab = null;
    private static GameObject _hollowShadePrefab = null;
    private static GameObject _dreamgatePrefab = null;

    internal static void LoadPrefabs(SceneManager template)
    {
        _borderPrefab = template.borderPrefab;
        _hollowShadePrefab = template.hollowShadeObject;
        _dreamgatePrefab = template.dreamgateObject;
    }

    /// <summary>
    /// The area of the map this scene belongs to.
    /// </summary>
    [Header("Gameplay Scene Settings")]
    [Tooltip("The area of the map this scene belongs to.")]
    [Space(6f)]
    public MapZone mapZone = MapZone.TOWN;
    /// <summary>
    /// Determines if this area is currently windy.
    /// </summary>
    [Tooltip("Determines if this area is currently windy.")]
    public bool isWindy = false;
    /// <summary>
    /// Determines if this level experiences tremors.
    /// </summary>
    [Tooltip("Determines if this level experiences tremors.")]
    public bool isTremorZone = false;
    /// <summary>
    /// Set environment type on scene entry.
    /// </summary>
    [Tooltip("Set environment type on scene entry.")]
    public EnviromentType environmentType = EnviromentType.Dust;
    /// <summary>
    /// Set darkness level on scene entry.
    /// </summary>
    [Tooltip("Set darkness level on scene entry.")]
    public DarknessLevel darknessLevel = DarknessLevel.Normal;
    /// <summary>
    /// Determines if the lantern is deactivated.
    /// </summary>
    [Tooltip("Determines if the lantern is deactivated.")]
    public bool noLantern = false;

    /// <summary>
    /// Saturation
    /// </summary>
    [Header("Camera Color Correction Curves")]
    [Range(0f, 5f)]
    public float saturation = 0.7f;
    /// <summary>
    /// Ignore Platform Saturation Modifiers
    /// </summary>
    public bool ignorePlatformSaturationModifiers = false;
    /// <summary>
    /// Red Channel
    /// </summary>
    public AnimationCurve redChannel = AnimationCurve.Linear(0, 0, 1, 1);
    /// <summary>
    /// Green Channel
    /// </summary>
    public AnimationCurve greenChannel = AnimationCurve.Linear(0, 0, 1, 1);
    /// <summary>
    /// Blue Channel
    /// </summary>
    public AnimationCurve blueChannel = AnimationCurve.Linear(0, 0, 1, 1);

    /// <summary>
    /// The default ambient light colour for this scene.
    /// </summary>
    [Header("Ambient Light")]
    [Tooltip("The default ambient light colour for this scene.")]
    [Space(6f)]
    public Color defaultColor = new Color(1, 1, 1, 1);
    /// <summary>
    /// The intensity of the ambient light in this scene.
    /// </summary>
    [Tooltip("The intensity of the ambient light in this scene.")]
    [Range(0f, 1f)]
    public float defaultIntensity = 0.8f;

    /// <summary>
    /// Color of the hero's light gradient (not point lights).
    /// </summary>
    [Header("Hero Light")]
    [Tooltip("Color of the hero's light gradient (not point lights)")]
    [Space(6f)]
    public Color heroLightColor = new Color(1, 1, 1, 0.48f);

    /// <summary>
    /// No Particles
    /// </summary>
    [Header("Scene Particles")]
    public bool noParticles = false;
    /// <summary>
    /// Override Particles With
    /// </summary>
    public MapZone overrideParticlesWith = MapZone.NONE;

    /// <summary>
    /// Atmos Cue Set
    /// </summary>
    [Header("Audio Snapshots")]
    [Space(6f)]
    public string AtmosCueSet = "";
    /// <summary>
    /// Atmos Cue Snapshot Name
    /// </summary>
    public string AtmosCueSnapshotName = "at Surface"; // AtmosCue
    /// <summary>
    /// Atmos Cue Snapshot Index
    /// </summary>
    public AtmosChoices AtmosCueSnapshotIndex = AtmosChoices.Surface; // AtmosCue
    /// <summary>
    /// Atmos Cue Is Channel Enabled
    /// </summary>
    [ArrayForEnum(typeof(AtmosChannels))]
    public bool[] AtmosCueIsChannelEnabled = new[] { false, true, false, true, false, true, true, false, false, false, false, false, false, false, false, false }; // AtmosCue
    /// <summary>
    /// Music Cue Set
    /// </summary>
    public string MusicCueSet = "";
    /// <summary>
    /// Music Cue Snapshot Name
    /// </summary>
    public string MusicCueSnapshotName = "Silent"; // MusicCue
    /// <summary>
    /// Music Cue Snapshot Index
    /// </summary>
    public MusicChoices MusicCueSnapshotIndex = MusicChoices.Silent; // MusicCue
    /// <summary>
    /// Music Cue Channel Info Clips
    /// </summary>
    [ArrayForEnum(typeof(MusicChannels))]
    public AudioClip[] MusicCueChannelInfoClips = new AudioClip[] { null, null, null, null, null, null }; // MusicCue
    /// <summary>
    /// Music Cue Channel Info Syncs
    /// </summary>
    [ArrayForEnum(typeof(MusicChannels))]
    public MusicChannelSync[] MusicCueChannelInfoSyncs = new[] { MusicChannelSync.Implicit, MusicChannelSync.Implicit, MusicChannelSync.Implicit, MusicChannelSync.Implicit, MusicChannelSync.Implicit, MusicChannelSync.Implicit }; // MusicCue
    /// <summary>
    /// Infected Music Cue Set
    /// </summary>
    public string InfectedMusicCueSet = "";
    /// <summary>
    /// Infected Music Cue Snapshot Name
    /// </summary>
    public string InfectedMusicCueSnapshotName = "Silent"; // InfectedMusicCue
    /// <summary>
    /// Infected Music Cue Snapshot Index
    /// </summary>
    public MusicChoices InfectedMusicCueSnapshotIndex = MusicChoices.Silent; // InfectedMusicCue
    /// <summary>
    /// Infected Music Cue Channel Info Clips
    /// </summary>
    [ArrayForEnum(typeof(MusicChannels))]
    public AudioClip[] InfectedMusicCueChannelInfoClips = new AudioClip[] { null, null, null, null, null, null }; // InfectedMusicCue
    /// <summary>
    /// Infected Music Cue Channel Info Syncs
    /// </summary>
    [ArrayForEnum(typeof(MusicChannels))]
    public MusicChannelSync[] InfectedMusicCueChannelInfoSyncs = new[] { MusicChannelSync.Implicit, MusicChannelSync.Implicit, MusicChannelSync.Implicit, MusicChannelSync.Implicit, MusicChannelSync.Implicit, MusicChannelSync.Implicit }; // InfectedMusicCue
    /// <summary>
    /// Music Snapshot Snapshot Name
    /// </summary>
    public string MsSnapshotName = "Silent"; // MusicSnapshot
    /// <summary>
    /// Music Snapshot Snapshot Index
    /// </summary>
    public MusicChoices MsSnapshotIndex = MusicChoices.Silent; // MusicSnapshot
    /// <summary>
    /// Music Delay Time
    /// </summary>
    public float musicDelayTime = 0;
    /// <summary>
    /// Music Transition Time
    /// </summary>
    public float musicTransitionTime = 3;
    /// <summary>
    /// Atmos Snapshot Snapshot Name
    /// </summary>
    public string AtsSnapshotName = "at Surface"; // AtmosSnapshot
    /// <summary>
    /// Atmos Snapshot Snapshot Index
    /// </summary>
    public AtmosChoices AtsSnapshotIndex = AtmosChoices.Surface; // AtmosSnapshot
    /// <summary>
    /// Enviro Snapshot Snapshot Name
    /// </summary>
    public string EsSnapshotName = "en Cliffs"; // EnviroSnapshot
    /// <summary>
    /// Enviro Snapshot Snapshot Index
    /// </summary>
    public EnviroChoices EsSnapshotIndex = EnviroChoices.Cliffs; // EnviroSnapshot
    /// <summary>
    /// Actor Snapshot Snapshot Name
    /// </summary>
    public string AcsSnapshotName = "On"; // ActorSnapshot
    /// <summary>
    /// Actor Snapshot Snapshot Index
    /// </summary>
    public ActorChoices AcsSnapshotIndex = ActorChoices.On; // ActorSnapshot
    /// <summary>
    /// Shade Snapshot Snapshot Name
    /// </summary>
    public string SsSnapshotName = "Away"; // ShadeSnapshot
    /// <summary>
    /// Shade Snapshot Snapshot Index
    /// </summary>
    public ShadeChoices SsSnapshotIndex = ShadeChoices.Away; // ShadeSnapshot
    /// <summary>
    /// Transition Time
    /// </summary>
    public float transitionTime = 0.5f;

    /// <summary>
    /// Manual Map Trigger
    /// </summary>
    [Header("Mapping")]
    [Space(6f)]
    public bool manualMapTrigger = false;

    /// <summary>
    /// Unity method.
    /// </summary>
    public void Awake()
    {
        if (_musicAm == null)
        {
            var ams = Resources.FindObjectsOfTypeAll<AudioMixer>();
            _musicAm = ams.First(x => x.name == "Music");
            _atmosAm = ams.First(x => x.name == "Atmos");
            _enviroAm = ams.First(x => x.name == "EnviroEffects");
            _actorAm = ams.First(x => x.name == "Actors");
            _shadeAm = ams.First(x => x.name == "ShadeMixer");
        }

        GameObject smGo = new GameObject("_SceneManager");
        smGo.SetActive(false);
        var sm = smGo.AddComponent<SceneManager>();
        sm.sceneType = SceneType.GAMEPLAY;
        sm.mapZone = mapZone;
        sm.isWindy = isWindy;
        sm.isTremorZone = isTremorZone;
        sm.environmentType = (int) environmentType;
        sm.darknessLevel = (int) darknessLevel;
        sm.noLantern = noLantern;
        sm.saturation = saturation;
        sm.ignorePlatformSaturationModifiers = ignorePlatformSaturationModifiers;
        sm.redChannel = redChannel;
        sm.greenChannel = greenChannel;
        sm.blueChannel = blueChannel;
        sm.defaultColor = defaultColor;
        sm.defaultIntensity = defaultIntensity;
        sm.heroLightColor = heroLightColor;
        sm.noParticles = noParticles;
        sm.overrideParticlesWith = overrideParticlesWith;
        sm.SetAttr("atmosCue", CueHolder.GetAtmosCue(AtmosCueSet, _atmosAm.FindSnapshot(AtmosCueSnapshotName), AtmosCueIsChannelEnabled));
        sm.SetAttr("musicCue", CueHolder.GetMusicCue(MusicCueSet, _musicAm.FindSnapshot(MusicCueSnapshotName), MusicCueChannelInfoClips, MusicCueChannelInfoSyncs));
        sm.SetAttr("infectedMusicCue", CueHolder.GetMusicCue(InfectedMusicCueSet, _musicAm.FindSnapshot(InfectedMusicCueSnapshotName), InfectedMusicCueChannelInfoClips, InfectedMusicCueChannelInfoSyncs));
        sm.SetAttr("musicSnapshot", _musicAm.FindSnapshot(MsSnapshotName));
        sm.SetAttr("musicDelayTime", musicDelayTime);
        sm.SetAttr("musicTransitionTime", musicTransitionTime);
        Modding.Logger.Log($"AtsSnapshotName: {AtsSnapshotName}");
        sm.atmosSnapshot = _atmosAm.FindSnapshot(AtsSnapshotName);
        Modding.Logger.Log($"EsSnapshotName: {EsSnapshotName}");
        sm.enviroSnapshot = _enviroAm.FindSnapshot(EsSnapshotName);
        Modding.Logger.Log($"AcsSnapshotName: {AcsSnapshotName}");
        sm.actorSnapshot = _actorAm.FindSnapshot(AcsSnapshotName);
        Modding.Logger.Log($"SsSnapshotName: {SsSnapshotName}");
        sm.shadeSnapshot = _shadeAm.FindSnapshot(SsSnapshotName);
        sm.transitionTime = transitionTime;
        sm.borderPrefab = _borderPrefab;
        sm.manualMapTrigger = manualMapTrigger;
        sm.hollowShadeObject = _hollowShadePrefab;
        sm.dreamgateObject = _dreamgatePrefab;

        smGo.SetActive(true);
    }

    /// <summary>
    /// Replacement for private ingame enums
    /// </summary>
    public enum EnviromentType
    {
        /// <inheritdoc />
        Dust = 0,
        /// <inheritdoc />
        Grass,
        /// <inheritdoc />
        Bone,
        /// <inheritdoc />
        Spa,
        /// <inheritdoc />
        Metal,
        /// <inheritdoc />
        NoEffect,
        /// <inheritdoc />
        Wet
    }
    /// <summary>
    /// Replacement for private ingame enums
    /// </summary>
    public enum DarknessLevel
    {
        /// <inheritdoc />
        Undark1 = -1,
        /// <inheritdoc />
        Normal = 0,
        /// <inheritdoc />
        Dark1,
        /// <inheritdoc />
        Dark2
    }
    /// <summary>
    /// Replacement for AudioMixerSnapshots
    /// </summary>
    public enum MusicChoices
    {
        /// <inheritdoc />
        Normal,
        /// <inheritdoc />
        NormalAlt,
        /// <inheritdoc />
        NormalSoft,
        /// <inheritdoc />
        NormalSofter,
        /// <inheritdoc />
        NormalFlange,
        /// <inheritdoc />
        NormalFlangier,
        /// <inheritdoc />
        Action,
        /// <inheritdoc />
        ActionAndSub,
        /// <inheritdoc />
        SubArea,
        /// <inheritdoc />
        Silent,
        /// <inheritdoc />
        SilentFlange,
        /// <inheritdoc />
        Off,
        /// <inheritdoc />
        TensionOnly,
        /// <inheritdoc />
        NormalGramaphone,
        /// <inheritdoc />
        ActionOnly,
        /// <inheritdoc />
        MainOnly,
        /// <inheritdoc />
        HK_DECLINE2,
        /// <inheritdoc />
        HK_DECLINE3,
        /// <inheritdoc />
        HK_DECLINE4,
        /// <inheritdoc />
        HK_DECLINE5,
        /// <inheritdoc />
        HK_DECLINE6
    }
    /// <summary>
    /// Replacement for AudioMixerSnapshots
    /// </summary>
    public enum AtmosChoices
    {
        /// <inheritdoc />
        None,
        /// <inheritdoc />
        Cave,
        /// <inheritdoc />
        Surface,
        /// <inheritdoc />
        SurfaceInterior,
        /// <inheritdoc />
        SurfaceBasement,
        /// <inheritdoc />
        SurfaceNook,
        /// <inheritdoc />
        RainyIndoors,
        /// <inheritdoc />
        RainyOutdoors,
        /// <inheritdoc />
        DistantRain,
        /// <inheritdoc />
        DistantRainRoom,
        /// <inheritdoc />
        Greenpath,
        /// <inheritdoc />
        QueensGardens,
        /// <inheritdoc />
        Fungus,
        /// <inheritdoc />
        FogCanyon,
        /// <inheritdoc />
        WaterwaysFlowing,
        /// <inheritdoc />
        Waterways,
        /// <inheritdoc />
        GreenpathInterior,
        /// <inheritdoc />
        FogCanyonMinor,
        /// <inheritdoc />
        MinesCrystal,
        /// <inheritdoc />
        MinesMachinery,
        /// <inheritdoc />
        Deepnest,
        /// <inheritdoc />
        DeepnestQuiet,
        /// <inheritdoc />
        WindTunnel,
        /// <inheritdoc />
        MiscWind
    }
    /// <summary>
    /// Replacement for AudioMixerSnapshots
    /// </summary>
    public enum EnviroChoices
    {
        /// <inheritdoc />
        Cave,
        /// <inheritdoc />
        Spa,
        /// <inheritdoc />
        Cliffs,
        /// <inheritdoc />
        Room,
        /// <inheritdoc />
        Arena,
        /// <inheritdoc />
        Sewerpipe,
        /// <inheritdoc />
        FogCanyon,
        /// <inheritdoc />
        Dream,
        /// <inheritdoc />
        Silent
    }
    /// <summary>
    /// Replacement for AudioMixerSnapshots
    /// </summary>
    public enum ActorChoices
    {
        /// <inheritdoc />
        On,
        /// <inheritdoc />
        Off
    }
    /// <summary>
    /// Replacement for AudioMixerSnapshots
    /// </summary>
    public enum ShadeChoices
    {
        /// <inheritdoc />
        Away,
        /// <inheritdoc />
        Close
    }
    /// <summary>
    /// Replacement for private ingame enums
    /// </summary>
    public enum AtmosChannels
    {
        /// <inheritdoc />
        CaveWind,
        /// <inheritdoc />
        SurfaceWind1,
        /// <inheritdoc />
        GrassyWind,
        /// <inheritdoc />
        SurfaceWind2,
        /// <inheritdoc />
        CaveNoises,
        /// <inheritdoc />
        RainIndoor,
        /// <inheritdoc />
        RainOutdoor,
        /// <inheritdoc />
        Greenpath,
        /// <inheritdoc />
        Fungus,
        /// <inheritdoc />
        FogCanyon,
        /// <inheritdoc />
        Waterways,
        /// <inheritdoc />
        WaterfallMed,
        /// <inheritdoc />
        MinesCrystal,
        /// <inheritdoc />
        MinesMachinery,
        /// <inheritdoc />
        Deepnest,
        /// <inheritdoc />
        MiscWind
    }
    /// <summary>
    /// Replacement for private ingame enums
    /// </summary>
    public enum MusicChannelSync
    {
        /// <inheritdoc />
        Implicit,
        /// <inheritdoc />
        ExplicitOn,
        /// <inheritdoc />
        ExplicitOff
    }
}