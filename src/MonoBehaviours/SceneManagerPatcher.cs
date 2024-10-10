using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Audio;

namespace SFCore.MonoBehaviours;

[UsedImplicitly]
public class SceneManagerPatcher : MonoBehaviour
{
    [UsedImplicitly]
    private static AudioMixer _musicAm = null;
    [UsedImplicitly]
    private static AudioMixer _atmosAm = null;
    [UsedImplicitly]
    private static AudioMixer _enviroAm = null;
    [UsedImplicitly]
    private static AudioMixer _actorAm = null;
    [UsedImplicitly]
    private static AudioMixer _shadeAm = null;

    [Header("Gameplay Scene Settings")]
    [Tooltip("The area of the map this scene belongs to.")]
    [Space(6f)]
    [UsedImplicitly]
    public MapZone mapZone = MapZone.TOWN;
    [Tooltip("Determines if this area is currently windy.")]
    [UsedImplicitly]
    public bool isWindy = false;
    [Tooltip("Determines if this level experiences tremors.")]
    [UsedImplicitly]
    public bool isTremorZone = false;
    [Tooltip("Set environment type on scene entry.")]
    [UsedImplicitly]
    public EnviromentType environmentType = EnviromentType.Dust;
    [Tooltip("Set darkness level on scene entry.")]
    [UsedImplicitly]
    public DarknessLevel darknessLevel = DarknessLevel.Normal;
    [Tooltip("Determines if the lantern is deactivated.")]
    [UsedImplicitly]
    public bool noLantern = false;

    [Header("Camera Color Correction Curves")]
    [Range(0f, 5f)]
    [UsedImplicitly]
    public float saturation = 0.7f;
    [UsedImplicitly]
    public bool ignorePlatformSaturationModifiers = false;
    [UsedImplicitly]
    public AnimationCurve redChannel = AnimationCurve.Linear(0, 0, 1, 1);
    [UsedImplicitly]
    public AnimationCurve greenChannel = AnimationCurve.Linear(0, 0, 1, 1);
    [UsedImplicitly]
    public AnimationCurve blueChannel = AnimationCurve.Linear(0, 0, 1, 1);

    [Header("Ambient Light")]
    [Tooltip("The default ambient light colour for this scene.")]
    [Space(6f)]
    [UsedImplicitly]
    public Color defaultColor = new(1, 1, 1, 1);
    [Tooltip("The intensity of the ambient light in this scene.")]
    [Range(0f, 1f)]
    [UsedImplicitly]
    public float defaultIntensity = 0.8f;

    [Header("Hero Light")]
    [Tooltip("Color of the hero's light gradient (not point lights)")]
    [Space(6f)]
    [UsedImplicitly]
    public Color heroLightColor = new(1, 1, 1, 0.48f);

    [Header("Scene Particles")]
    [UsedImplicitly]
    public bool noParticles = false;
    [UsedImplicitly]
    public MapZone overrideParticlesWith = MapZone.NONE;

    [Header("Audio Snapshots")]
    [Space(6f)]
    [UsedImplicitly]
    public string AtmosCueSet = "";
    [UsedImplicitly]
    public string AtmosCueSnapshotName = "at Surface"; // AtmosCue
    [UsedImplicitly]
    public AtmosChoices AtmosCueSnapshotIndex = AtmosChoices.Surface; // AtmosCue
    [ArrayForEnum(typeof(AtmosChannels))]
    [UsedImplicitly]
    public bool[] AtmosCueIsChannelEnabled = { false, true, false, true, false, true, true, false, false, false, false, false, false, false, false, false }; // AtmosCue
    [UsedImplicitly]
    public string MusicCueSet = "";
    [UsedImplicitly]
    public string MusicCueSnapshotName = "Silent"; // MusicCue
    [UsedImplicitly]
    public MusicChoices MusicCueSnapshotIndex = MusicChoices.Silent; // MusicCue
    [ArrayForEnum(typeof(MusicChannels))]
    [UsedImplicitly]
    public AudioClip[] MusicCueChannelInfoClips = { null, null, null, null, null, null }; // MusicCue
    [ArrayForEnum(typeof(MusicChannels))]
    [UsedImplicitly]
    public MusicChannelSync[] MusicCueChannelInfoSyncs = { MusicChannelSync.Implicit, MusicChannelSync.Implicit, MusicChannelSync.Implicit, MusicChannelSync.Implicit, MusicChannelSync.Implicit, MusicChannelSync.Implicit }; // MusicCue
    [UsedImplicitly]
    public string InfectedMusicCueSet = "";
    [UsedImplicitly]
    public string InfectedMusicCueSnapshotName = "Silent"; // InfectedMusicCue
    [UsedImplicitly]
    public MusicChoices InfectedMusicCueSnapshotIndex = MusicChoices.Silent; // InfectedMusicCue
    [ArrayForEnum(typeof(MusicChannels))]
    [UsedImplicitly]
    public AudioClip[] InfectedMusicCueChannelInfoClips = { null, null, null, null, null, null }; // InfectedMusicCue
    [ArrayForEnum(typeof(MusicChannels))]
    [UsedImplicitly]
    public MusicChannelSync[] InfectedMusicCueChannelInfoSyncs = { MusicChannelSync.Implicit, MusicChannelSync.Implicit, MusicChannelSync.Implicit, MusicChannelSync.Implicit, MusicChannelSync.Implicit, MusicChannelSync.Implicit }; // InfectedMusicCue
    [UsedImplicitly]
    public string MsSnapshotName = "Silent"; // MusicSnapshot
    [UsedImplicitly]
    public MusicChoices MsSnapshotIndex = MusicChoices.Silent; // MusicSnapshot
    [UsedImplicitly]
    public float musicDelayTime = 0;
    [UsedImplicitly]
    public float musicTransitionTime = 3;
    [UsedImplicitly]
    public string AtsSnapshotName = "at Surface"; // AtmosSnapshot
    [UsedImplicitly]
    public AtmosChoices AtsSnapshotIndex = AtmosChoices.Surface; // AtmosSnapshot
    [UsedImplicitly]
    public string EsSnapshotName = "en Cliffs"; // EnviroSnapshot
    [UsedImplicitly]
    public EnviroChoices EsSnapshotIndex = EnviroChoices.Cliffs; // EnviroSnapshot
    [UsedImplicitly]
    public string AcsSnapshotName = "On"; // ActorSnapshot
    [UsedImplicitly]
    public ActorChoices AcsSnapshotIndex = ActorChoices.On; // ActorSnapshot
    [UsedImplicitly]
    public string SsSnapshotName = "Away"; // ShadeSnapshot
    [UsedImplicitly]
    public ShadeChoices SsSnapshotIndex = ShadeChoices.Away; // ShadeSnapshot
    [UsedImplicitly]
    public float transitionTime = 0.5f;

    [Header("Mapping")]
    [Space(6f)]
    [UsedImplicitly]
    public bool manualMapTrigger = false;

    public void Awake()
    {
    }

    [UsedImplicitly]
    public enum EnviromentType
    {
        Dust = 0,
        Grass,
        Bone,
        Spa,
        Metal,
        NoEffect,
        Wet
    }
    [UsedImplicitly]
    public enum DarknessLevel
    {
        Undark1 = -1,
        Normal = 0,
        Dark1,
        Dark2
    }
    [UsedImplicitly]
    public enum MusicChoices
    {
        Normal,
        NormalAlt,
        NormalSoft,
        NormalSofter,
        NormalFlange,
        NormalFlangier,
        Action,
        ActionAndSub,
        SubArea,
        Silent,
        SilentFlange,
        Off,
        TensionOnly,
        NormalGramaphone,
        ActionOnly,
        MainOnly,
        HK_DECLINE2,
        HK_DECLINE3,
        HK_DECLINE4,
        HK_DECLINE5,
        HK_DECLINE6
    }
    [UsedImplicitly]
    public enum AtmosChoices
    {
        None,
        Cave,
        Surface,
        SurfaceInterior,
        SurfaceBasement,
        SurfaceNook,
        RainyIndoors,
        RainyOutdoors,
        DistantRain,
        DistantRainRoom,
        Greenpath,
        QueensGardens,
        Fungus,
        FogCanyon,
        WaterwaysFlowing,
        Waterways,
        GreenpathInterior,
        FogCanyonMinor,
        MinesCrystal,
        MinesMachinery,
        Deepnest,
        DeepnestQuiet,
        WindTunnel,
        MiscWind
    }
    [UsedImplicitly]
    public enum EnviroChoices
    {
        Cave,
        Spa,
        Cliffs,
        Room,
        Arena,
        Sewerpipe,
        FogCanyon,
        Dream,
        Silent
    }
    [UsedImplicitly]
    public enum ActorChoices
    {
        On,
        Off
    }
    [UsedImplicitly]
    public enum ShadeChoices
    {
        Away,
        Close
    }
    [UsedImplicitly]
    public enum AtmosChannels
    {
        CaveWind,
        SurfaceWind1,
        GrassyWind,
        SurfaceWind2,
        CaveNoises,
        RainIndoor,
        RainOutdoor,
        Greenpath,
        Fungus,
        FogCanyon,
        Waterways,
        WaterfallMed,
        MinesCrystal,
        MinesMachinery,
        Deepnest,
        MiscWind
    }
    [UsedImplicitly]
    public enum MusicChannels
    {
        Main,
        MainAlt,
        Action,
        Sub,
        Tension,
        Extra
    }
    [UsedImplicitly]
    public enum MusicChannelSync
    {
        Implicit,
        ExplicitOn,
        ExplicitOff
    }
    [UsedImplicitly]
    public enum MapZone
    {
        NONE,
        TEST_AREA,
        KINGS_PASS,
        CLIFFS,
        TOWN,
        CROSSROADS,
        GREEN_PATH,
        ROYAL_GARDENS,
        FOG_CANYON,
        WASTES,
        DEEPNEST,
        HIVE,
        BONE_FOREST,
        PALACE_GROUNDS,
        MINES,
        RESTING_GROUNDS,
        CITY,
        DREAM_WORLD,
        COLOSSEUM,
        ABYSS,
        ROYAL_QUARTER,
        WHITE_PALACE,
        SHAMAN_TEMPLE,
        WATERWAYS,
        QUEENS_STATION,
        OUTSKIRTS,
        KINGS_STATION,
        MAGE_TOWER,
        TRAM_UPPER,
        TRAM_LOWER,
        FINAL_BOSS,
        SOUL_SOCIETY,
        ACID_LAKE,
        NOEYES_TEMPLE,
        MONOMON_ARCHIVE,
        MANTIS_VILLAGE,
        RUINED_TRAMWAY,
        DISTANT_VILLAGE,
        ABYSS_DEEP,
        ISMAS_GROVE,
        WYRMSKIN,
        LURIENS_TOWER,
        LOVE_TOWER,
        GLADE,
        BLUE_LAKE,
        PEAK,
        JONI_GRAVE,
        OVERGROWN_MOUND,
        CRYSTAL_MOUND,
        BEASTS_DEN,
        GODS_GLORY,
        GODSEEKER_WASTE
    }
}