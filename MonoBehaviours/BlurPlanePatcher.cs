using UnityEngine;

namespace SFCore.MonoBehaviours
{
    [RequireComponent(typeof(MeshRenderer))]
    public class BlurPlanePatcher : MonoBehaviour
    {
        private static Material[] blurPlaneMaterials;
        private static bool initialized = false;

        public void Start()
        {
            if (!initialized)
            {
                initialized = true;

                blurPlaneMaterials = new Material[1];
                blurPlaneMaterials[0] = new Material(Shader.Find("UI/Blur/UIBlur"));
                blurPlaneMaterials[0].SetColor(Shader.PropertyToID("_TintColor"), new Color(1.0f, 1.0f, 1.0f, 0.0f));
                blurPlaneMaterials[0].SetFloat(Shader.PropertyToID("_Size"), 53.7f);
                //blurPlaneMaterials[0].SetFloat(Shader.PropertyToID("_Size"), 107.4f);
                blurPlaneMaterials[0].SetFloat(Shader.PropertyToID("_Vibrancy"), 0.2f);
                //blurPlaneMaterials[0].SetFloat(Shader.PropertyToID("_Vibrancy"), 1.0f);
                blurPlaneMaterials[0].SetInt(Shader.PropertyToID("_StencilComp"), 8);
                blurPlaneMaterials[0].SetInt(Shader.PropertyToID("_Stencil"), 0);
                blurPlaneMaterials[0].SetInt(Shader.PropertyToID("_StencilOp"), 0);
                blurPlaneMaterials[0].SetInt(Shader.PropertyToID("_StencilWriteMask"), 255);
                blurPlaneMaterials[0].SetInt(Shader.PropertyToID("_StencilReadMask"), 255);
            }

            var bp = gameObject.AddComponent<BlurPlane>();
            var mr = gameObject.GetComponent<MeshRenderer>();
            mr.materials = blurPlaneMaterials;
            mr.material = blurPlaneMaterials[0];
            bp.SetPlaneMaterial(mr.materials[0]);
            bp.SetPlaneVisibility(true);
        }
    }
}
