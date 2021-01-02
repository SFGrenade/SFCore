using System.Collections.Generic;
using UnityEngine;

namespace SFCore.MonoBehaviours
{
    class SpritePatcher : MonoBehaviour
    {
        private static Dictionary<string, Material> materials = new Dictionary<string, Material>();

        public string shader = "Sprites/Default";
        public float Scale = 1.0f;

        public void Start()
        {
            if (!materials.ContainsKey(shader))
            {
                var mat = new Material(Shader.Find(shader));
                materials.Add(shader, mat);
            }

            foreach (SpriteRenderer sr in gameObject.GetComponentsInChildren<SpriteRenderer>(true))
            {
                sr.gameObject.transform.localScale *= this.Scale;
                sr.material = materials[shader];
            }
        }
    }
}
