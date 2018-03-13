using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace StackableDecorator
{
    public enum MessageType { None, Info, Warning, Error }

    public static class Utils
    {
#if UNITY_EDITOR
        private static Dictionary<string, Texture> s_Imagecache = new Dictionary<string, Texture>();
        private static Dictionary<string, Texture> s_GUIDImagecache = new Dictionary<string, Texture>();

        public static Texture GetImage(this string image)
        {
            if (s_Imagecache.ContainsKey(image))
                return s_Imagecache[image];
            var tex = Resources.Load<Texture>(image);
            if (tex == null)
                tex = EditorGUIUtility.Load(image) as Texture;
            if (tex == null)
                tex = AssetDatabase.LoadAssetAtPath<Texture>(image);
            s_Imagecache[image] = tex;
            return tex;
        }

        public static Texture GUIDToImage(this string guid)
        {
            if (s_GUIDImagecache.ContainsKey(guid))
                return s_GUIDImagecache[guid];
            var path = AssetDatabase.GUIDToAssetPath(guid);
            var tex = AssetDatabase.LoadAssetAtPath<Texture>(path);
            s_GUIDImagecache[guid] = tex;
            return tex;
        }

        public static IEnumerable<Scene> GetAllLoadedScene()
        {
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                var scene = SceneManager.GetSceneAt(i);
                if (scene.isLoaded && scene.IsValid())
                    yield return scene;
            }
        }
#endif
        public static bool IsPowerOfTwo(this long number)
        {
            return number != 0 && (number & (number - 1)) == 0;
        }

        public static IEnumerable<T> Yield<T>(this T item)
        {
            yield return item;
        }

        public static TValue Get<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, TValue def) where TValue : struct
        {
            TValue result;
            if (!dict.TryGetValue(key, out result))
                dict[key] = result = def;
            return result;
        }

        public static TValue Get<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key) where TValue : new()
        {
            TValue result;
            if (!dict.TryGetValue(key, out result))
                dict[key] = result = new TValue();
            return result;
        }
    }
}