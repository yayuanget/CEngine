using UnityEngine;
using System.Collections;
using System.IO;
using System.Security.Cryptography;
using System;
namespace CEngine
{
    public class LoadPath
    {
        public LoadPath(string rootPath, string fileName)
        {
            this.path = rootPath + "/" + fileName;
            this.usewww = UseWWW(fileName);
        }

        private bool UseWWW(string flieName)
        {
            //if (!AppConfig.isRemote)
            //    return false;
            //else
            return Filter(flieName) ? true : false;
        }

        private bool Filter(string fileName)
        {
            for (int i = 0; i < filters.Length; i++)
            {
                if (fileName.Contains(filters[i]))
                    return true;
            }
            return false;
        }
        private string[] filters = new string[] { ".mp3", ".ogg", ".wav", @"\scene" };
        public bool usewww;
        public string path;
    }
}
