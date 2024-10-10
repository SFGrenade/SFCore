using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Audio;

namespace SFCore.MonoBehaviours;

[RequireComponent(typeof(BoxCollider2D))]
[UsedImplicitly]
public class PatchMusicRegions : MonoBehaviour
{
    [UsedImplicitly]
    private static AudioMixer _am = null;

    [UsedImplicitly]
    public bool useAlts = false;
    [UsedImplicitly]
    public static bool altMusic = false;

    [UsedImplicitly]
    public string SnapshotName = "Silent";
    [UsedImplicitly]
    public Choices SnapshotIndex = Choices.Silent;

    [UsedImplicitly]
    public string MusicRegionSet = "";

    [UsedImplicitly]
    public AudioClip Main;
    [UsedImplicitly]
    public AudioClip Action;
    [UsedImplicitly]
    public AudioClip Sub;
    [UsedImplicitly]
    public AudioClip Tension;
    [UsedImplicitly]
    public AudioClip MainAlt;
    [UsedImplicitly]
    public AudioClip Extra;

    [UsedImplicitly]
    public AudioClip Main2;
    [UsedImplicitly]
    public AudioClip Action2;
    [UsedImplicitly]
    public AudioClip Sub2;
    [UsedImplicitly]
    public AudioClip Tension2;
    [UsedImplicitly]
    public AudioClip MainAlt2;
    [UsedImplicitly]
    public AudioClip Extra2;

    [UsedImplicitly]
    public bool Dirtmouth = false;
    [UsedImplicitly]
    public bool MinesDelay = false;
    [UsedImplicitly]
    public string EnterTrackEvent = "CROSSROADS";
    [UsedImplicitly]
    public float EnterTransitionTime = 3f;

    public void Start()
    {
    }

    [UsedImplicitly]
    public enum Choices
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
}