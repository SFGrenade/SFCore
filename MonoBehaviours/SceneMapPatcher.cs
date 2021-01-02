using UnityEngine;

namespace SFCore.MonoBehaviours
{
    class SceneMapPatcher : MonoBehaviour
    {
        private Material sceneMapMaterial;
        private bool initialized = false;
        public Texture tex;

        public void Start()
        {
            if (!initialized)
            {
                sceneMapMaterial = new Material(Shader.Find("tk2d/BlendVertexColor"));
                sceneMapMaterial.SetTexture(Shader.PropertyToID("_MainTex"), tex);

                initialized = true;
            }

            foreach (var cMr in gameObject.GetComponentsInChildren<MeshRenderer>(false))
            {
                cMr.material = sceneMapMaterial;
            }
        }
    }
}
