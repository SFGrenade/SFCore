using UnityEngine;

namespace SFCore.MonoBehaviours
{
    class SceneMapPatcher : MonoBehaviour
    {
        private Material _sceneMapMaterial;
        private bool _initialized = false;
        public Texture tex;

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
