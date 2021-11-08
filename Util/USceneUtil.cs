using UnityEngine;
using Logger = Modding.Logger;
using UScene = UnityEngine.SceneManagement.Scene;

namespace SFCore.Utils
{
    public static class USceneUtil
    {
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

        public static GameObject FindGameObjectInChildren(this GameObject o, string name) => o.Find(name);
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

        public static void Log(this UScene scene)
        {
            Logger.Log($"[SceneLog] - Scene \"{scene.name}\"");
            foreach (var go in scene.GetRootGameObjects())
                go.transform.Log();
        }

        public static void Log(this Transform go, string n = "\t")
        {
            Logger.Log($"[SceneLog] - {n}\"{go.name}\"");
            foreach (var comp in go.GetComponents<Component>())
                Logger.Log($"[SceneLog] - {n} => \"{comp.GetType()}\": {comp}");
            for (var i = 0; i < go.childCount; i++)
                go.GetChild(i).Log($"{n}\t");
        }
    }
}