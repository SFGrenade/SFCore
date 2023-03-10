using UnityEngine;

namespace SFCore.MonoBehaviours
{
    /// <summary>
    /// Patching BlurPlane
    /// </summary>
    [RequireComponent(typeof(MeshRenderer))]
    public class BlurPlanePatcher : MonoBehaviour
    {
        private static Material[] _blurPlaneMaterials;
        private static bool _initialized = false;

        /// <summary>
        /// Unity method.
        /// </summary>
        public void Start()
        {
            if (!_initialized)
            {
                _initialized = true;

                _blurPlaneMaterials = new Material[1];
                _blurPlaneMaterials[0] = new Material(Shader.Find("UI/Blur/UIBlur"));
                _blurPlaneMaterials[0].SetColor(Shader.PropertyToID("_TintColor"), new Color(1.0f, 1.0f, 1.0f, 0.0f));
                _blurPlaneMaterials[0].SetFloat(Shader.PropertyToID("_Size"), 53.7f);
                //blurPlaneMaterials[0].SetFloat(Shader.PropertyToID("_Size"), 107.4f);
                _blurPlaneMaterials[0].SetFloat(Shader.PropertyToID("_Vibrancy"), 0.2f);
                //blurPlaneMaterials[0].SetFloat(Shader.PropertyToID("_Vibrancy"), 1.0f);
                _blurPlaneMaterials[0].SetInt(Shader.PropertyToID("_StencilComp"), 8);
                _blurPlaneMaterials[0].SetInt(Shader.PropertyToID("_Stencil"), 0);
                _blurPlaneMaterials[0].SetInt(Shader.PropertyToID("_StencilOp"), 0);
                _blurPlaneMaterials[0].SetInt(Shader.PropertyToID("_StencilWriteMask"), 255);
                _blurPlaneMaterials[0].SetInt(Shader.PropertyToID("_StencilReadMask"), 255);
            }

            var bp = gameObject.GetComponent<BlurPlane>();
            if (bp == null)
                bp = gameObject.AddComponent<BlurPlane>();
            var mr = gameObject.GetComponent<MeshRenderer>();
            mr.materials = _blurPlaneMaterials;
            mr.material = _blurPlaneMaterials[0];
            bp.SetPlaneMaterial(mr.materials[0]);
            bp.SetPlaneVisibility(true);
        }
    }
}
