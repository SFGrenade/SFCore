using GlobalEnums;
using SFCore.Utils;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

namespace SFCore.MonoBehaviours
{
    public class SceneManagerPatcher : MonoBehaviour
    {
        private static AudioMixer _musicAm = null;
        private static AudioMixer _atmosAm = null;
        private static AudioMixer _enviroAm = null;
        private static AudioMixer _actorAm = null;
        private static AudioMixer _shadeAm = null;

        [Header("Gameplay Scene Settings")]
        [Tooltip("The area of the map this scene belongs to.")]
        [Space(6f)]
        public MapZone mapZone = MapZone.TOWN;
        [Tooltip("Determines if this area is currently windy.")]
        public bool isWindy = false;
        [Tooltip("Determines if this level experiences tremors.")]
        public bool isTremorZone = false;
        [Tooltip("Set environment type on scene entry.")]
        public EnviromentType environmentType = EnviromentType.Dust;
        [Tooltip("Set darkness level on scene entry.")]
        public DarknessLevel darknessLevel = DarknessLevel.Normal;
        [Tooltip("Determines if the lantern is deactivated.")]
        public bool noLantern = false;

        [Header("Camera Color Correction Curves")]
        [Range(0f, 5f)]
        public float saturation = 0.7f;
        public bool ignorePlatformSaturationModifiers = false;
        public AnimationCurve redChannel = AnimationCurve.Linear(0, 0, 1, 1);
        public AnimationCurve greenChannel = AnimationCurve.Linear(0, 0, 1, 1);
        public AnimationCurve blueChannel = AnimationCurve.Linear(0, 0, 1, 1);

        [Header("Ambient Light")]
        [Tooltip("The default ambient light colour for this scene.")]
        [Space(6f)]
        public Color defaultColor = new Color(1, 1, 1, 1);
        [Tooltip("The intensity of the ambient light in this scene.")]
        [Range(0f, 1f)]
        public float defaultIntensity = 0.8f;

        [Header("Hero Light")]
        [Tooltip("Color of the hero's light gradient (not point lights)")]
        [Space(6f)]
        public Color heroLightColor = new Color(1, 1, 1, 0.48f);

        [Header("Scene Particles")]
        public bool noParticles = false;
        public MapZone overrideParticlesWith = MapZone.NONE;

        [Header("Audio Snapshots")]
        [Space(6f)]
        public string AtmosCueSet = "";
        public string AtmosCueSnapshotName = "at Surface"; // AtmosCue
        public AtmosChoices AtmosCueSnapshotIndex = AtmosChoices.Surface; // AtmosCue
        [ArrayForEnum(typeof(AtmosChannels))]
        public bool[] AtmosCueIsChannelEnabled = new[] { false, true, false, true, false, true, true, false, false, false, false, false, false, false, false, false }; // AtmosCue
        public string MusicCueSet = "";
        public string MusicCueSnapshotName = "Silent"; // MusicCue
        public MusicChoices MusicCueSnapshotIndex = MusicChoices.Silent; // MusicCue
        [ArrayForEnum(typeof(MusicChannels))]
        public AudioClip[] MusicCueChannelInfoClips = new AudioClip[] { null, null, null, null, null, null }; // MusicCue
        [ArrayForEnum(typeof(MusicChannels))]
        public MusicChannelSync[] MusicCueChannelInfoSyncs = new[] { MusicChannelSync.Implicit, MusicChannelSync.Implicit, MusicChannelSync.Implicit, MusicChannelSync.Implicit, MusicChannelSync.Implicit, MusicChannelSync.Implicit }; // MusicCue
        public string InfectedMusicCueSet = "";
        public string InfectedMusicCueSnapshotName = "Silent"; // InfectedMusicCue
        public MusicChoices InfectedMusicCueSnapshotIndex = MusicChoices.Silent; // InfectedMusicCue
        [ArrayForEnum(typeof(MusicChannels))]
        public AudioClip[] InfectedMusicCueChannelInfoClips = new AudioClip[] { null, null, null, null, null, null }; // InfectedMusicCue
        [ArrayForEnum(typeof(MusicChannels))]
        public MusicChannelSync[] InfectedMusicCueChannelInfoSyncs = new[] { MusicChannelSync.Implicit, MusicChannelSync.Implicit, MusicChannelSync.Implicit, MusicChannelSync.Implicit, MusicChannelSync.Implicit, MusicChannelSync.Implicit }; // InfectedMusicCue
        public string MsSnapshotName = "Silent"; // MusicSnapshot
        public MusicChoices MsSnapshotIndex = MusicChoices.Silent; // MusicSnapshot
        public float musicDelayTime = 0;
        public float musicTransitionTime = 3;
        public string AtsSnapshotName = "at Surface"; // AtmosSnapshot
        public AtmosChoices AtsSnapshotIndex = AtmosChoices.Surface; // AtmosSnapshot
        public string EsSnapshotName = "en Cliffs"; // EnviroSnapshot
        public EnviroChoices EsSnapshotIndex = EnviroChoices.Cliffs; // EnviroSnapshot
        public string AcsSnapshotName = "On"; // ActorSnapshot
        public ActorChoices AcsSnapshotIndex = ActorChoices.On; // ActorSnapshot
        public string SsSnapshotName = "Away"; // ShadeSnapshot
        public ShadeChoices SsSnapshotIndex = ShadeChoices.Away; // ShadeSnapshot
        public float transitionTime = 0.5f;

        [Header("Mapping")]
        [Space(6f)]
        public bool manualMapTrigger = false;

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
            Modding.Logger.Log($"AtmosCueSnapshotName: {AtmosCueSnapshotName}");
            sm.SetAttr("atmosCue", CueHolder.GetAtmosCue(AtmosCueSet, _atmosAm.FindSnapshot(AtmosCueSnapshotName), AtmosCueIsChannelEnabled));
            Modding.Logger.Log($"MusicCueSnapshotName: {MusicCueSnapshotName}");
            sm.SetAttr("musicCue", CueHolder.GetMusicCue(MusicCueSet, _musicAm.FindSnapshot(MusicCueSnapshotName), MusicCueChannelInfoClips, MusicCueChannelInfoSyncs));
            Modding.Logger.Log($"InfectedMusicCueSnapshotName: {InfectedMusicCueSnapshotName}");
            sm.SetAttr("infectedMusicCue", CueHolder.GetMusicCue(InfectedMusicCueSet, _musicAm.FindSnapshot(InfectedMusicCueSnapshotName), InfectedMusicCueChannelInfoClips, InfectedMusicCueChannelInfoSyncs));
            Modding.Logger.Log($"MsSnapshotName: {MsSnapshotName}");
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
            sm.borderPrefab = GameObject.Find("SceneBorder");
            sm.manualMapTrigger = manualMapTrigger;
            sm.hollowShadeObject = GameObject.Find("Hollow Shade");
            sm.dreamgateObject = GameObject.Find("dream_gate_object");

            smGo.SetActive(true);
        }

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
        public enum DarknessLevel
        {
            Undark1 = -1,
            Normal = 0,
            Dark1,
            Dark2
        }
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
        public enum ActorChoices
        {
            On,
            Off
        }
        public enum ShadeChoices
        {
            Away,
            Close
        }
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
        public enum MusicChannelSync
        {
            Implicit,
            ExplicitOn,
            ExplicitOff
        }
    }
}
