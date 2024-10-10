using JetBrains.Annotations;
using UnityEngine;

namespace SFCore.MonoBehaviours;

[UsedImplicitly]
class SceneMapPatcher : MonoBehaviour
{
    [UsedImplicitly]
    private Material _sceneMapMaterial;
    [UsedImplicitly]
    private bool _initialized = false;
    [UsedImplicitly]
    public Texture tex;

    public void Start()
    {
    }
}