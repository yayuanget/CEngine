using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace CEngine.Tool
{
    public class ByteConvert
    {
        /// <summary>
        ///  bytes 转 Texture2D
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static Texture2D BytesToTexture2D(byte[] bytes)
        {
            Texture2D tex = new Texture2D(1, 1, TextureFormat.ARGB32, false);
            tex.LoadImage(bytes);

            return tex;
        }

        /// <summary>
        /// Texture2D 转 Sprite
        /// </summary>
        /// <param name="tex"></param>
        /// <returns></returns>
        public static Sprite CreateImage(Texture2D tex)
        {
            return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0, 0));
        }

        /// <summary>
        /// Bytes 转 String UTF8
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string BytesToString(byte[] bytes)
        {
            string str = "";
            str = System.Text.Encoding.UTF8.GetString(bytes, 0, bytes.Length);
            if (string.IsNullOrEmpty(str))
                return str;

            if ((UInt16)str[0] == 0xFEFF)
            {
                str = str.Substring(1);
            }

            return str;
        }
    }
}
