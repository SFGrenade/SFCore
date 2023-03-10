using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UObject = UnityEngine.Object;

namespace SFCore.Utils
{
    /// <summary>
    /// Utils specifically for Sprites and Textures.
    /// </summary>
    public static class SpriteUtil
    {
        private delegate Color Texture2D_GetPixelImpl(int image, int x, int y);
        private static MethodInfo tex_GetPixelImpl = typeof(Texture2D).GetMethod("GetPixelImpl", BindingFlags.NonPublic | BindingFlags.Instance);

        private static Texture2D_GetPixelImpl getGetPixel(Texture2D tex)
        {
            return (Texture2D_GetPixelImpl) tex_GetPixelImpl.CreateDelegate(typeof(Texture2D_GetPixelImpl), tex);
        }

        /// <summary>
        /// Calculates the area between 3 integer points.
        /// </summary>
        /// <param name="a">A point of the triangle</param>
        /// <param name="b">A point of the triangle</param>
        /// <param name="c">A point of the triangle</param>
        /// <returns>The area of the triangle.</returns>
        public static float CalcTriangleArea(Vector2Int a, Vector2Int b, Vector2Int c)
        {
            return Mathf.Abs(((a.x * (b.y - c.y)) + (b.x * (c.y - a.y)) + (c.x * (a.y - b.y))) / 2f);
        }

        /// <summary>
        /// Returns a new Texture2D that is cropped.
        /// </summary>
        /// <param name="tex">The texture to crop</param>
        /// <param name="x">Left offset</param>
        /// <param name="y">Top offset</param>
        /// <param name="width">Width of the new texture</param>
        /// <param name="height">Height of the new texture</param>
        /// <returns>The cropped Texture2D.</returns>
        public static Texture2D GetReadableCroppedTexture(Texture2D tex, int x, int y, int width, int height)
        {
            float xScale = (float)width / tex.width;
            float yScale = (float)height / tex.height;
            float xOffset = (float)x / tex.width;
            float yOffset = (float)y / tex.height;
            Texture2D ret = new Texture2D(width, height);
            RenderTexture tempRt = RenderTexture.GetTemporary(width, height, 0, RenderTextureFormat.Default, RenderTextureReadWrite.Linear);
            Graphics.Blit(tex, tempRt, new Vector2(xScale, yScale), new Vector2(xOffset, yOffset));
            RenderTexture tmpActiveRt = RenderTexture.active;
            RenderTexture.active = tempRt;
            ret.ReadPixels(new Rect(0, 0, ret.width, ret.height), 0, 0);
            ret.Apply();
            RenderTexture.active = tmpActiveRt;
            RenderTexture.ReleaseTemporary(tempRt);
            return ret;
        }

        /// <summary>
        /// Extracts a sprite from a texture, returns a new texture which is cropped to only the sprite according to its UV values.
        /// </summary>
        /// <param name="sprite">The sprite to extract</param>
        /// <returns>A texture with just the sprite on it.</returns>
        public static Texture2D ExtractTextureFromSprite(Sprite sprite)
        {
            var spriteRect = (sprite.texture.width, sprite.texture.height);
            List<Vector2Int> texUVs = new List<Vector2Int>();
            List<(Vector2Int, Vector2Int, Vector2Int)> tmpTriangles = new List<(Vector2Int, Vector2Int, Vector2Int)>();
            int i;
            bool[][] contents;
            float triangleArea;
            float pab, pbc, pac;
            Vector2Int p;
            int x, y;
            int minX, maxX, minY, maxY;
            int width, height;
            Texture2D outTex;

            foreach (var item in sprite.uv)
            {
                texUVs.Add(new Vector2Int(Mathf.RoundToInt(item.x * (spriteRect.width - 1)), Mathf.RoundToInt(item.y * (spriteRect.height - 1))));
            }
            for (i = 0; i < sprite.triangles.Length; i += 3)
            {
                tmpTriangles.Add((texUVs[sprite.triangles[i]], texUVs[sprite.triangles[i + 1]], texUVs[sprite.triangles[i + 2]]));
            }

            minX = texUVs.Min(uv => uv.x);
            maxX = texUVs.Max(uv => uv.x);
            minY = texUVs.Min(uv => uv.y);
            maxY = texUVs.Max(uv => uv.y);
            width = maxX - minX + 1;
            height = maxY - minY + 1;

            contents = new bool[height][];
            for (i = 0; i < contents.Length; i++)
                contents[i] = new bool[width];
            foreach (var item in tmpTriangles)
            {
                triangleArea = CalcTriangleArea(item.Item1, item.Item2, item.Item3);
                for (x = 0; x < width; x++)
                {
                    for (y = 0; y < height; y++)
                    {
                        p = new Vector2Int(minX + x, minY + y);
                        pab = CalcTriangleArea(item.Item1, item.Item2, p);
                        pbc = CalcTriangleArea(p, item.Item2, item.Item3);
                        pac = CalcTriangleArea(item.Item1, p, item.Item3);
                        if ((pab + pbc + pac) == triangleArea)
                        {
                            contents[y][x] = true;
                        }
                    }
                }

            }

            outTex = GetReadableCroppedTexture(sprite.texture, minX, minY, width, height);

            for (x = 0; x < width; x++)
            {
                for (y = 0; y < height; y++)
                {
                    if (!contents[y][x])
                        outTex.SetPixel(x, y, new Color(0, 0, 0, 0));
                }
            }
            outTex.Apply();

            return outTex;
        }
    }
}
