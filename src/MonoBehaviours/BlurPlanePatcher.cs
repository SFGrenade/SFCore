using JetBrains.Annotations;
using UnityEngine;

namespace SFCore.MonoBehaviours;

[RequireComponent(typeof(MeshRenderer))]
[UsedImplicitly]
public class BlurPlanePatcher : MonoBehaviour
{
    [UsedImplicitly]
    private static Material[] _blurPlaneMaterials;
    [UsedImplicitly]
    private static bool _initialized = false;

    public void Start()
    {
    }
}