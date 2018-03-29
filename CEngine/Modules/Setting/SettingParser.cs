using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System;
using CEngine.Tool;

namespace CEngine
{
    /// <summary>
    /// 配置表解析器 目前支持TSV
    /// </summary>
    public class SettingParser
    {
        public SettingParser(string fileName)
        {
            this.fileName = fileName;
            rows = new Dictionary<string, TableRow>();
            heads = new Dictionary<string, HeadInfo>();
        }

        public string fileName;
        public char[] separators = new char[] { '\t' };

        //key 主键
        public Dictionary<string, TableRow> rows;
        public Dictionary<string, HeadInfo> heads;


        public TableRow GetRow(string primaryKey)
        {
            if (rows.ContainsKey(primaryKey))
                return rows[primaryKey];

            return null;

        }

        /// <summary>
        /// 加载并解析表
        /// </summary>
        public void Parse(bool useLocal = false)
        {
            string url;
            if (useLocal || !AppSetting.isRemote)
            {
                url = System.IO.Path.Combine(AppSetting.SourceDataPath, "setting/" + fileName);

                CDebug.Log("load tsv fileName " + url);
                //string content = LoadSettingNoLock(url);
                string content = ByteConvert.BytesToString(LoadSetting(url));
                //CDebug.Log(content);
                ParseString(content);
            }
            else
            {
                //CDebug.Log("--------------- setting/" + "setting/" + fileName);
                if (TaskManager.instance.localResPaths.ContainsKey("setting/" + fileName))
                {
                    var localPath = TaskManager.instance.localResPaths["setting/" + fileName];
                    TaskManager.instance.AddTask(localPath, "", (data) =>
                    {
                        string content = ByteConvert.BytesToString(data.bytes);
                        ParseString(content);
                    });
                }

            }

        }

        public void ParseString(string content)
        {
            content = content.Trim();
            heads = new Dictionary<string, HeadInfo>();
            using (var oReader = new StringReader(content))
            {
                var headLine = oReader.ReadLine();
                var typeLine = oReader.ReadLine();
                var metaLine = oReader.ReadLine();
                var headStrings = headLine.Split(separators);
                var typeStrings = typeLine.Split(separators);
                var metaStrings = metaLine.Split(separators);
                //CDebug.Log(headStrings.Length + " " + metaStrings.Length);
                for (int i = 0; i < headStrings.Length; i++)
                {
                    HeadInfo head = new HeadInfo()
                    {
                        name = headStrings[i],
                        type = typeStrings[i],
                        meta = metaStrings[i],
                        index = i,

                    };

                    heads.Add(head.name, head);

                    //CDebug.Log("head " + i + " " + headStrings[i]);
                }
                var rowLine = "";
                while (rowLine != null)
                {
                    rowLine = oReader.ReadLine();
                    if (rowLine == null) break;
                    var rowStrings = rowLine.Split(separators);
                    var primaryKey = rowStrings[0];
                    //CDebug.LogError(rowLine);
                    if (rows.ContainsKey(primaryKey))
                    {
                        CDebug.LogError("rows.ContainsKey true " + primaryKey + "   !" + fileName);
                    }
                    else
                    {
                        rows.Add(primaryKey, new TableRow(rowStrings, heads));
                    }

                }
            }
        }
        // 不会锁死, 允许其它程序打开
        public string LoadSettingNoLock(string url)
        {
            if (!StringCache.Instance.Contains(url))
            {
                Encoding encoding = Encoding.UTF8; // Encoding.GetEncoding("GBK"); // default encoding

                // 不会锁死, 允许其它程序打开
                using (FileStream fileStream = new FileStream(url, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    StreamReader oReader = new StreamReader(fileStream, encoding);

                    StringCache.Instance.Add(url, oReader.ReadToEnd());
                }
            }

            return StringCache.Instance.Get(url);
        }

        public byte[] LoadSetting(string url)
        {
            if (!BytesCache.Instance.Contains(url))
            {
                using (System.IO.Stream s = System.IO.File.OpenRead(url))
                {
                    byte[] bytes = new byte[s.Length];
                    s.Read(bytes, 0, (int)s.Length);
                    s.Close();
                    Byte[] tmp = bytes;
                    if (AppSetting.isRemote)
                        StringEncryption.Encypt(ref tmp);
                    BytesCache.Instance.Add(url, tmp);
                }
            }
            //GameObject go = new GameObject();
            return BytesCache.Instance.Get(url);
        }

    }

    public class HeadInfo
    {
        public string name;
        public string meta;//描述
        public string type;//类型
        public int index;
    }

    public class TableRow
    {
        public TableRow(string[] values, Dictionary<string, HeadInfo> heads)
        {
            this.values = values;
            this.heads = heads;
        }

        public string primaryKey
        {
            get { return values[0]; }
        }

        public Dictionary<string, HeadInfo> heads;
        public string[] values;

        public string Get(int i)
        {
            return values[i];
        }

        public object Get(string name)
        {
            //CDebug.LogError("name " + name);
            HeadInfo head = null;
            heads.TryGetValue(name, out head);
            if (head != null)
            {
                switch (head.type)
                {
                    case "int":
                        //CDebug.LogError(head.index);
                        //CDebug.LogError(values[head.index]);
                        return int.Parse(values[head.index]);
                    case "string":
                        return values[head.index].ToString();
                    case "double":
                        return double.Parse(values[head.index]);
                    case "Vector3":
                        return RowConvert.StringToVector3(values[head.index]);
                    case "Vector4":
                        return RowConvert.StringToVector4(values[head.index]);
                    case "Vector3[]":
                        return RowConvert.StringToVector3Array(values[head.index]);
                    case "int[]":
                        return RowConvert.StringToIntArray(values[head.index]);
                    case "bool":
                        return RowConvert.StringToBool(values[head.index]);
                    case "long":
                        return long.Parse(values[head.index]);
                    case "float":
                        return float.Parse(values[head.index]);
                    default:
                        break;
                }
            }

            return null;
        }


    }

    //Tsv表格式约定
    class RowConvert
    {
        //逗号
        private const char COMMA = ',';
        //分号
        private const char SEMICOLON = ';';

        //Vector3 例： 0,0,0
        public static Vector3 StringToVector3(string context)
        {
            //CDebug.Log(context);
            string[] s = context.Split(COMMA);
            return new Vector3(float.Parse(s[0]), float.Parse(s[1]), float.Parse(s[2]));
        }

        //Vector4 例： 0,0,0,0
        public static Vector4 StringToVector4(string context)
        {
            //CDebug.Log(context);
            string[] s = context.Split(COMMA);
            return new Vector4(float.Parse(s[0]), float.Parse(s[1]), float.Parse(s[2]));
        }

        //Vector3[] 例： 0,0,0;1,1,1;
        public static Vector3[] StringToVector3Array(string context)
        {
            //CDebug.Log(context);
            string[] s = context.Split(SEMICOLON);
            Vector3[] vectors = new Vector3[s.Length - 1];
            //CDebug.Log("vectors.Length " + vectors.Length);
            for (int i = 0; i < s.Length - 1; i++)
            {
                if (string.IsNullOrEmpty(s[i]))
                    continue;

                vectors[i] = StringToVector3(s[i]);
                //CDebug.Log(" StringToVector3Array " + vectors[i].ToString() + " i " + i);
            }

            return vectors;
        }

        //int[] 例： 0,1,2,3,4;
        public static int[] StringToIntArray(string context)
        {
            // CDebug.Log(context);
            string[] s = context.Split(COMMA);
            int[] ints = new int[s.Length];
            for (int i = 0; i < s.Length; i++)
            {
                if (string.IsNullOrEmpty(s[i]))
                    continue;
                ints[i] = int.Parse(s[i]);
            }

            return ints;
        }

        public static bool StringToBool(string context)
        {
            context = context.Trim();

            if (context.Equals("0"))
            {
                return false;
            }

            return true;
        }

    }
}

