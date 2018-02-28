using System.Collections.Generic;
using UnityEngine;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace StackableDecorator
{
#if UNITY_EDITOR
    public class TransformComparer : IComparer<Transform>
    {
        public int Compare(Transform x, Transform y)
        {
            bool prefab1 = EditorUtility.IsPersistent(x);
            bool prefab2 = EditorUtility.IsPersistent(y);
            if (prefab1 && prefab2) return 0;
            if (prefab1) return 1;
            if (prefab2) return -1;

            var parents1 = x.GetAncestors().Reverse().ToList();
            var parents2 = y.GetAncestors().Reverse().ToList();
            int i = 0;
            while (i < parents1.Count && i < parents2.Count && parents1[i] == parents2[i])
                i++;
            if (i >= parents1.Count || i >= parents2.Count)
                return parents1.Count.CompareTo(parents2.Count);
            return parents1[i].GetSiblingIndex().CompareTo(parents2[i].GetSiblingIndex());
        }
    }
#endif
    public static class TransformUtils
    {
        public static IEnumerable<Transform> GetAncestors(this Transform transform, bool self = true)
        {
            var current = self ? transform : transform.parent;
            while (current != null)
            {
                yield return current;
                current = current.parent;
            }
        }

        public static IEnumerable<Transform> GetChildren(this Transform transform, bool self = true)
        {
            if (self) yield return transform;
            int count = transform.childCount;
            for (int i = 0; i < count; i++)
                yield return transform.GetChild(i);
        }

        public static IEnumerable<Transform> GetDescendants(this Transform transform, bool self = true)
        {
            if (self) yield return transform;
            foreach (var child in transform.GetChildren(false))
                foreach (var descendant in child.GetDescendants())
                    yield return descendant;
        }

        public static GameObject GetRoot(this GameObject gameObject)
        {
            return gameObject.transform.root.gameObject;
        }

        public static IEnumerable<GameObject> GetAncestors(this GameObject gameObject, bool self = true)
        {
            return gameObject.transform.GetAncestors(self).Select(t => t.gameObject);
        }

        public static IEnumerable<GameObject> GetChildren(this GameObject gameObject, bool self = true)
        {
            return gameObject.transform.GetChildren(self).Select(t => t.gameObject);
        }

        public static IEnumerable<GameObject> GetDescendants(this GameObject gameObject, bool self = true)
        {
            return gameObject.transform.GetDescendants(self).Select(t => t.gameObject);
        }
    }
}