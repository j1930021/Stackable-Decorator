using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace StackableDecorator
{
    public static class RectUtils
    {
        public static float defaultSpacing = 2;

        public static Rect X(this Rect rect, float x)
        {
            rect.x = x;
            return rect;
        }

        public static Rect Y(this Rect rect, float y)
        {
            rect.y = y;
            return rect;
        }

        public static Rect Position(this Rect rect, float x, float y)
        {
            rect.x = x;
            rect.y = y;
            return rect;
        }

        public static Rect Size(this Rect rect, float width, float height)
        {
            rect.width = width;
            rect.height = height;
            return rect;
        }

#if UNITY_EDITOR
        public static Rect indent(this Rect rect, bool indented = true)
        {
            return indented ? UnityEditor.EditorGUI.IndentedRect(rect) : new Rect(rect);
        }
#endif

        #region Fix width and height
        public static Rect Width(this Rect rect, float width)
        {
            rect.width = width;
            return rect;
        }

        public static Rect WidthFromRight(this Rect rect, float width)
        {
            rect.xMin = rect.xMax - width;
            return rect;
        }

        public static Rect Height(this Rect rect, float height)
        {
            rect.height = height;
            return rect;
        }

        public static Rect HeightFromBottom(this Rect rect, float height)
        {
            rect.yMin = rect.yMax - height;
            return rect;
        }
        #endregion

        #region Safe width and height
        public static Rect Left(this Rect rect, float width)
        {
            rect.width = Mathf.Min(rect.width, width);
            return rect;
        }

        public static Rect Right(this Rect rect, float width)
        {
            rect.xMin = rect.xMax - Mathf.Min(rect.width, width);
            return rect;
        }

        public static Rect Top(this Rect rect, float height)
        {
            rect.height = Mathf.Min(rect.height, height);
            return rect;
        }

        public static Rect Bottom(this Rect rect, float height)
        {
            rect.yMin = rect.yMax - Mathf.Min(rect.height, height);
            return rect;
        }
        #endregion

        #region Cut width and height
        public static Rect CutLeft(this Rect rect, float width)
        {
            rect.xMin = Mathf.Min(rect.xMin + width, rect.xMax);
            return rect;
        }

        public static Rect CutRight(this Rect rect, float width)
        {
            rect.xMax = Mathf.Max(rect.xMin, rect.xMax - width);
            return rect;
        }

        public static Rect CutTop(this Rect rect, float height)
        {
            rect.yMin = Mathf.Min(rect.yMin + height, rect.yMax);
            return rect;
        }

        public static Rect CutBottom(this Rect rect, float height)
        {
            rect.yMax = Mathf.Max(rect.yMin, rect.yMax - height);
            return rect;
        }
        #endregion

        #region Move
        public static Rect MoveLeft(this Rect rect)
        {
            return MoveLeft(rect, defaultSpacing);
        }

        public static Rect MoveLeft(this Rect rect, float spacing)
        {
            if (rect.width > 0)
                rect.x -= rect.width + spacing;
            return rect;
        }

        public static Rect MoveRight(this Rect rect)
        {
            return MoveRight(rect, defaultSpacing);
        }

        public static Rect MoveRight(this Rect rect, float spacing)
        {
            if (rect.width > 0)
                rect.x += rect.width + spacing;
            return rect;
        }

        public static Rect MoveUp(this Rect rect)
        {
            return MoveUp(rect, defaultSpacing);
        }

        public static Rect MoveUp(this Rect rect, float spacing)
        {
            if (rect.height > 0)
                rect.y -= rect.height + spacing;
            return rect;
        }

        public static Rect MoveDown(this Rect rect)
        {
            return MoveDown(rect, defaultSpacing);
        }

        public static Rect MoveDown(this Rect rect, float spacing)
        {
            if (rect.height > 0)
                rect.y += rect.height + spacing;
            return rect;
        }
        #endregion

        #region Enlarge and Shrink
        public static Rect Enlarge(this Rect rect, float size)
        {
            rect.xMin -= size;
            rect.xMax += size;
            rect.yMin -= size;
            rect.yMax += size;
            return rect;
        }

        public static Rect Enlarge(this Rect rect, float hsize, float vsize)
        {
            rect.xMin -= hsize;
            rect.xMax += hsize;
            rect.yMin -= vsize;
            rect.yMax += vsize;
            return rect;
        }

        public static Rect Enlarge(this Rect rect, float left, float right, float top, float bottom)
        {
            rect.xMin -= left;
            rect.xMax += right;
            rect.yMin -= top;
            rect.yMax += bottom;
            return rect;
        }

        public static Rect Shrink(this Rect rect, float size)
        {
            rect.xMin += size;
            rect.xMax -= size;
            rect.yMin += size;
            rect.yMax -= size;
            return rect;
        }

        public static Rect Shrink(this Rect rect, float hsize, float vsize)
        {
            rect.xMin += hsize;
            rect.xMax -= hsize;
            rect.yMin += vsize;
            rect.yMax -= vsize;
            return rect;
        }

        public static Rect Shrink(this Rect rect, float left, float right, float top, float bottom)
        {
            rect.xMin += left;
            rect.xMax -= right;
            rect.yMin += top;
            rect.yMax -= bottom;
            return rect;
        }
        #endregion

        #region Distribute
        public static IEnumerable<Rect> HorizontalDistribute(this Rect rect, int count)
        {
            return HorizontalDistribute(rect, defaultSpacing, Enumerable.Repeat(-1f, count));
        }

        public static IEnumerable<Rect> HorizontalDistribute(this Rect rect, int count, float spacing)
        {
            return HorizontalDistribute(rect, spacing, Enumerable.Repeat(-1f, count));
        }

        public static IEnumerable<Rect> RowDistribute(this Rect rect, IEnumerable<float> widths)
        {
            return HorizontalDistribute(rect, defaultSpacing, widths);
        }

        public static IEnumerable<Rect> HorizontalDistribute(this Rect rect, float spacing, IEnumerable<float> widths)
        {
            var list = widths.ToList();
            var total = rect.width - spacing * (list.Count(w => w != 0) - 1);

            float weight = 0;
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i] < 0)
                    weight += -list[i];
                else
                {
                    if (list[i] <= 1)
                        list[i] *= rect.width;
                    list[i] = Mathf.Clamp(list[i], 0, total);
                    total -= list[i];
                }
            }
            total /= weight;

            Rect current = new Rect(rect);
            for (int i = 0; i < list.Count; i++)
            {
                var width = list[i] < 0 ? list[i] *= -total : list[i];
                current.width = width;
                yield return current;
                current = current.MoveRight(spacing);
            }
        }

        public static IEnumerable<Rect> VerticalDistribute(this Rect rect, int count)
        {
            return VerticalDistribute(rect, defaultSpacing, Enumerable.Repeat(-1f, count));
        }

        public static IEnumerable<Rect> VerticalDistribute(this Rect rect, int count, float spacing)
        {
            return VerticalDistribute(rect, spacing, Enumerable.Repeat(-1f, count));
        }

        public static IEnumerable<Rect> VerticalDistribute(this Rect rect, IEnumerable<float> heights)
        {
            return VerticalDistribute(rect, defaultSpacing, heights);
        }

        public static IEnumerable<Rect> VerticalDistribute(this Rect rect, float spacing, IEnumerable<float> heights)
        {
            rect = new Rect(rect.y, rect.x, rect.height, rect.width);
            return HorizontalDistribute(rect, spacing, heights).Select(r => new Rect(r.y, r.x, r.height, r.width));
        }

        public static IEnumerable<Rect> GridDistribute(this Rect rect, int row, int column)
        {
            return GridDistribute(rect, -1, -1, row, column, defaultSpacing);
        }

        public static IEnumerable<Rect> GridDistribute(this Rect rect, int row, int column, float spacing)
        {
            return GridDistribute(rect, -1, -1, row, spacing, column, spacing);
        }

        public static IEnumerable<Rect> GridDistribute(this Rect rect, int row, float vSpacing, int column, float hSpacing)
        {
            return GridDistribute(rect, -1, -1, row, vSpacing, column, hSpacing);
        }

        public static IEnumerable<Rect> GridDistribute(this Rect rect, float width, float height, int row, int column, float spacing)
        {
            return GridDistribute(rect, width, height, row, spacing, column, spacing);
        }

        public static IEnumerable<Rect> GridDistribute(this Rect rect, float width, float height, int row, float vSpacing, int column, float hSpacing)
        {
            var rows = height < 0 ? VerticalDistribute(rect, row, vSpacing) : VerticalDistribute(rect, vSpacing, Enumerable.Repeat(height, row));
            foreach (var r in rows)
            {
                var columns = width < 0 ? HorizontalDistribute(r, column, hSpacing) : HorizontalDistribute(r, hSpacing, Enumerable.Repeat(width, column));
                foreach (var c in columns)
                    yield return c;
            }
        }
        #endregion
    }
}