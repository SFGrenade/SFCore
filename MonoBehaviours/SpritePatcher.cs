using System.Collections.Generic;
using UnityEngine;

namespace SFCore.MonoBehaviours
{
    /// <summary>
    ///     Patching Sprites from assetbundles
    /// </summary>
    class SpritePatcher : MonoBehaviour
    {
        private static Dictionary<string, Material> _materials = new Dictionary<string, Material>();

        /// <summary>
        ///     Shader to apply to all SpriteRenderers on this GameObject and children
        /// </summary>
        public string shader = "Sprites/Default";
        /// <summary>
        ///     Scale to apply to all Transforms on this GameObject and children
        /// </summary>
        public float Scale = 1.0f;

        /// <inheritdoc />
        public void Start()
        {
            if (!_materials.ContainsKey(shader))
            {
                var mat = new Material(Shader.Find(shader));
                _materials.Add(shader, mat);
            }

            foreach (SpriteRenderer sr in gameObject.GetComponentsInChildren<SpriteRenderer>(true))
            {
                sr.gameObject.transform.localScale *= Scale;
                sr.material = _materials[shader];
            }
        }
    }
}
