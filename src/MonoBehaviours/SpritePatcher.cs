using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace SFCore.MonoBehaviours;

[UsedImplicitly]
class SpritePatcher : MonoBehaviour
{
    private static Dictionary<string, Material> _materials;

    [UsedImplicitly]
    public string shader = "Sprites/Default";
    [UsedImplicitly]
    public float Scale = 1.0f;

    public void Start()
    {
    }
}