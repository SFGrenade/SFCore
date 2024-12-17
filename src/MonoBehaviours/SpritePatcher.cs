using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace SFCore.MonoBehaviours;

[UsedImplicitly]
public class SpritePatcher : MonoBehaviour
{
    [UsedImplicitly]
    public string shader = "Sprites/Default";
    [UsedImplicitly]
    public float Scale = 1.0f;

    public void Start()
    {
    }
}