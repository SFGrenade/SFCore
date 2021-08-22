using System.Collections.Generic;
using UnityEngine;

namespace SFCore.MonoBehaviours
{
    class SpritePatcher : MonoBehaviour
    {
        private static Dictionary<string, Material> _materials = new Dictionary<string, Material>();

        public string shader = "Sprites/Default";
        public float Scale = 1.0f;

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
