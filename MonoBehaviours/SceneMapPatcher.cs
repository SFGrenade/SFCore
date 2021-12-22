using UnityEngine;

namespace SFCore.MonoBehaviours
{
    /// <summary>
    ///     Patching SceneMap MeshRenderer
    /// </summary>
    class SceneMapPatcher : MonoBehaviour
    {
        private Material _sceneMapMaterial;
        private bool _initialized = false;
        /// <summary>
        ///     Texture to apply to all MeshRenderers on this GameObject and children
        /// </summary>
        public Texture tex = null;

        /// <inheritdoc />
        public void Start()
        {
            if (!_initialized)
            {
                _sceneMapMaterial = new Material(Shader.Find("tk2d/BlendVertexColor"));
                _sceneMapMaterial.SetTexture(Shader.PropertyToID("_MainTex"), tex);

                _initialized = true;
            }

            foreach (var cMr in gameObject.GetComponentsInChildren<MeshRenderer>(false))
            {
                cMr.material = _sceneMapMaterial;
            }
        }
    }
}
