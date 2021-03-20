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
                foreach (var go in scene.GetRootGameObjects())
                    if (go.name == name)
                        return go;
            return null;
        }

        public static GameObject Find(this UScene scene, string name)
        {
            if (scene.IsValid())
            {
                GameObject retGo;
                foreach (var go in scene.GetRootGameObjects())
                {
                    if (go.name == name) return go;
                    retGo = go.Find(name);
                    if (retGo != null) return retGo;
                }
            }

            return null;
        }

        public static GameObject Find(this GameObject o, string name)
        {
            if (o == null) return null;
            for (int i = 0; i < o.transform.childCount; i++)
                if (name.Equals(o.transform.GetChild(i).gameObject.name))
                    return o.transform.GetChild(i).gameObject;

            for (int i = 0; i < o.transform.childCount; i++)
            {
                GameObject ret = o.transform.GetChild(i).gameObject.Find(name);
                if (ret != null) return ret;
            }
            return null;
        }

        public static GameObject FindGameObjectInChildren(this GameObject o, string name)
        {
            return o.Find(name);
        }

        public static void Log(this UScene scene)
        {
            Logger.Log($"[SceneLog] - Scene \"{scene.name}\"");
            foreach (var go in scene.GetRootGameObjects()) go.transform.Log();
        }

        public static void Log(this Transform go, string n = "\t")
        {
            Transform c;
            Logger.Log($"[SceneLog] - {n}\"{go.name}\"");
            foreach (var comp in go.GetComponents<Component>())
                Logger.Log($"[SceneLog] - {n} => \"{comp.GetType()}\": {comp}");
            for (var i = 0; i < go.childCount; i++) go.GetChild(i).Log($"{n}\t");
        }
    }
}