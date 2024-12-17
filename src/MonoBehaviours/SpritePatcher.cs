using System.Collections.Generic;
using UnityEngine;

namespace SFCore.MonoBehaviours;

/// <summary>
/// Patching Sprites from assetbundles
/// </summary>
public class SpritePatcher : MonoBehaviour
{
    /// <summary>
    /// Shader to apply to all SpriteRenderers on this GameObject and children
    /// </summary>
    public string shader = "Sprites/Default";
    /// <summary>
    /// Scale to apply to all Transforms on this GameObject and children
    /// </summary>
    public float Scale = 1.0f;

    /// <summary>
    /// Unity method.
    /// </summary>
    public void Start()
    {
        foreach (SpriteRenderer sr in gameObject.GetComponentsInChildren<SpriteRenderer>(true))
        {
            sr.gameObject.transform.localScale *= Scale;
            sr.material.shader = Shader.Find(shader);
        }
    }
}