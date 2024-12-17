using UnityEngine;

namespace SFCore.MonoBehaviours;

/// <summary>
/// Patching SceneMap MeshRenderer
/// </summary>
public class SceneMapPatcher : MonoBehaviour
{
    /// <summary>
    /// Texture to apply to all MeshRenderers on this GameObject and children
    /// </summary>
    public Texture tex = null;

    /// <summary>
    /// Unity method.
    /// </summary>
    public void Start()
    {
        foreach (var cMr in gameObject.GetComponentsInChildren<MeshRenderer>(false))
        {
            cMr.material.shader = Shader.Find("tk2d/BlendVertexColor");
            cMr.material.SetTexture(Shader.PropertyToID("_MainTex"), tex);
        }
    }
}