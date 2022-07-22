using UnityEngine;
using Logger = Modding.Logger;
using UScene = UnityEngine.SceneManagement.Scene;

namespace SFCore.Utils
{
    /// <summary>
    ///     Utils specifically for Unity Scenes and GameObjects.
    /// </summary>
    public static class USceneUtil
    {
        /// <summary>
        ///     Finds a GameObject in a given scene at the root level.
        /// </summary>
        /// <param name="scene">The scene to search in</param>
        /// <param name="name">The name of the GameObject</param>
        /// <returns>The found GameObject, null if none is found.</returns>
        public static GameObject FindRoot(this UScene scene, string name)
        {
            if (scene.IsValid())
            {
                var rootGos = scene.GetRootGameObjects();
                var rootGosCount = rootGos.Length;
                for (int i = 0; i < rootGosCount; i++)
                {
                    if (rootGos[i].name == name)
                    {
                        return rootGos[i];
                    }
                }
            }
            return null;
        }

        /// <summary>
        ///     Finds a GameObject in a given scene.
        /// </summary>
        /// <param name="scene">The scene to search in</param>
        /// <param name="name">The name of the GameObject</param>
        /// <returns>The found GameObject, null if none is found.</returns>
        public static GameObject Find(this UScene scene, string name)
        {
            if (scene.IsValid())
            {
                GameObject retGo;
                var rootGos = scene.GetRootGameObjects();
                var rootGosCount = rootGos.Length;
                for (int i = 0; i < rootGosCount; i++)
                {
                    if (rootGos[i].name == name)
                    {
                        return rootGos[i];
                    }
                    retGo = rootGos[i].Find(name);
                    if (retGo != null)
                    {
                        return retGo;
                    }
                }
            }

            return null;
        }

        /// <summary>
        ///     Finds a child GameObject of a given GameObject.
        /// </summary>
        /// <param name="o">The GameObject to start the search from</param>
        /// <param name="name">The name of the GameObject</param>
        /// <returns>The found GameObject, null if none is found.</returns>
        public static GameObject FindGameObjectInChildren(this GameObject o, string name) => o.Find(name);
        /// <inheritdoc cref="FindGameObjectInChildren(GameObject, string)"/>
        public static GameObject Find(this GameObject o, string name)
        {
            if (o == null)
            {
                return null;
            }
            var count = o.transform.childCount;
            for (int i = 0; i < count; i++)
            {
                var tmp = o.transform.GetChild(i).gameObject;
                if (name == tmp.name)
                {
                    return tmp;
                }
            }

            for (int i = 0; i < o.transform.childCount; i++)
            {
                GameObject ret = o.transform.GetChild(i).gameObject.Find(name);
                if (ret != null)
                {
                    return ret;
                }
            }
            return null;
        }

        /// <summary>
        ///     Logs a scene with all GameObjects and Components on them.
        /// </summary>
        /// <param name="scene">The scene to log</param>
        public static void Log(this UScene scene)
        {
            Logger.LogDebug($"[SceneLog] - Scene \"{scene.name}\"");
            foreach (var go in scene.GetRootGameObjects())
                go.transform.Log();
        }

        /// <summary>
        ///     Logs a transform and all children with their Components.
        /// </summary>
        /// <param name="go">The Transform to log</param>
        /// <param name="n">The indentation to use</param>
        public static void Log(this Transform go, string n = "\t")
        {
            Logger.LogDebug($"[SceneLog] - {n}\"{go.name}\"");
            foreach (var comp in go.GetComponents<Component>())
                Logger.LogDebug($"[SceneLog] - {n} => \"{comp.GetType()}\": {comp}");
            for (var i = 0; i < go.childCount; i++)
                go.GetChild(i).Log($"{n}\t");
        }
    }
}