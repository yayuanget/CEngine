using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using DotLiquid;
using CEngine;

namespace CEngineEditor
{
    class CompileSettingEditorWindow : EditorWindow
    {
        [MenuItem("Custom/CompileSetting Window")]
        static void CompileSettingEditor()
        {
            CompileSettingEditorWindow.Init();
        }

        public static void Init()
        {
            // Get existing open window or if none, make a new one:
            var window = EditorWindow.GetWindow<CompileSettingEditorWindow>(true, "CompileSetting");
            window.SelfInit();
        }

        public void SelfInit()
        {
            Populate();
        }

        void OnSelectionChange()
        {
            Populate();
            Repaint();
        }

        void OnEnable()
        {
            Populate();
        }

        void OnFocus()
        {
            Populate();
        }

        List<TSVFile> tsvs = new List<TSVFile>();
        string outpath = "Scripts/Settings";
        //string outpath = "Scripts/LuaNiuNiu/Settings";
        string findDir;

        private void Populate()
        {
            tsvs.Clear();
            string rootpath = System.IO.Path.GetDirectoryName(Application.dataPath);
            findDir = System.IO.Path.Combine(AppSetting.localDataPath, "setting");  //捕鱼
            var allFiles = Directory.GetFiles(findDir, "*.*", SearchOption.AllDirectories);
            foreach (var file in allFiles)
            {
                TSVFile tsv = new TSVFile(file);
                tsv.hash = GetHash(tsv.fileName);    //捕鱼
                Debug.LogError(tsv.fileName + "     " + file);
                Debug.LogError(tsv.hash);
                tsvs.Add(tsv);

            }
        }

        Vector2 v1 = Vector2.zero;
        public void OnGUI()
        {
            GUILayout.Label("Output Path:" + outpath, GUILayout.MaxWidth(this.position.width));

            EditorCommonFunc.Layout_DrawSeparator(Color.white);

            GUILayout.Space(16);
            EditorGUILayout.BeginVertical(GUILayout.Height(150));
            {

                GUILayout.Label("Find All Tsv files in  " + findDir);

                v1 = EditorGUILayout.BeginScrollView(v1, GUILayout.Height(500));
                {
                    foreach (var tsv in tsvs)
                    {
                        string pathShow = tsv.fileName;//Replace(pp.path.Substring(Application.dataPath.Length + 1));

                        tsv.isChecked = GUILayout.Toggle(tsv.isChecked, pathShow);

                        EditorCommonFunc.Layout_DrawSeparator(Color.gray);
                    }

                    EditorGUILayout.BeginHorizontal(GUILayout.Height(150));
                    {
                        if (GUILayout.Button("ExportLua", GUILayout.MaxWidth(100)))
                            ExportCheckedLua();

                        if (GUILayout.Button("Export", GUILayout.MaxWidth(100)))
                            ExportChecked();

                        if (GUILayout.Button("All", GUILayout.MaxWidth(100)))
                        {
                            foreach (var tsv in tsvs)
                                tsv.isChecked = true;
                        }

                        if (GUILayout.Button("None", GUILayout.MaxWidth(100)))
                        {
                            foreach (var tsv in tsvs)
                                tsv.isChecked = false;
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.EndScrollView();


            }
            EditorGUILayout.EndVertical();

        }

        void ExportCheckedLua()
        {
            var allFilesCount = tsvs.Count;
            int progress = -1;
            foreach (var tsv in tsvs)
            {
                progress++;
                if (tsv.fileName.Contains(".meta"))
                {
                    allFilesCount--;
                    continue;
                }
                if (!tsv.isChecked)
                    continue;

                Template template = Template.Parse(TemplateString.GenCodeTemplateLua);
                string content = template.Render(Hash.FromAnonymousObject(new { file = tsv.hash }));
                string outpath = System.IO.Path.Combine(Application.dataPath, "Scripts/Lua/Setting");  //捕鱼
                                                                                                       //string outpath = System.IO.Path.Combine(Application.dataPath, "Scripts/LuaNiuNiu/Setting");  //牛牛
                if (!System.IO.Directory.Exists(outpath))
                    System.IO.Directory.CreateDirectory(outpath);

                Debug.Log(outpath + "     " + template);
                Debug.LogError(outpath + "   1   " + tsv.fileName + "Setting.cs" + "  2   " + content);
                File.WriteAllText(System.IO.Path.Combine(outpath, tsv.fileName + "Settings.lua"), content);

                EditorUtility.DisplayProgressBar("Compiling Table...", "", progress / (float)allFilesCount);
            }

            AssetDatabase.Refresh();
            EditorUtility.ClearProgressBar();
        }

        void ExportChecked()
        {
            var allFilesCount = tsvs.Count;
            int progress = -1;
            foreach (var tsv in tsvs)
            {
                progress++;
                if (tsv.fileName.Contains(".meta"))
                {
                    allFilesCount--;
                    continue;
                }
                if (!tsv.isChecked)
                    continue;

                Template template = Template.Parse(TemplateString.GenCodeTemplateOne);
                string content = template.Render(Hash.FromAnonymousObject(new { file = tsv.hash }));
                string outpath = System.IO.Path.Combine(Application.dataPath, this.outpath);
                if (!System.IO.Directory.Exists(outpath))
                    System.IO.Directory.CreateDirectory(outpath);

                Debug.Log(outpath);
                Debug.LogError(outpath + "   1   " + tsv.fileName + "Setting.cs" + "  2   " + content);
                File.WriteAllText(System.IO.Path.Combine(outpath, tsv.fileName + "Setting.cs"), content);

                EditorUtility.DisplayProgressBar("Compiling Table...", "", progress / (float)allFilesCount);
            }

            AssetDatabase.Refresh();
            EditorUtility.ClearProgressBar();
        }

        static Hash GetHash(string className)
        {
            string fileName = className + ".tsv";
            SettingParser parser = new SettingParser(fileName);

            parser.Parse(true);

            Hash hash = new Hash();
            hash["ClassName"] = className;

            List<Hash> headsHash = new List<Hash>();

            foreach (var h in parser.heads)
            {
                HeadInfo headInfo = h.Value;
                Hash head = new Hash();

                head["name"] = headInfo.name;
                head["type"] = headInfo.type;
                head["meta"] = headInfo.meta;

                //Debug.Log("head name " + headInfo.name);
                headsHash.Add(head);
            }

            hash["heads"] = headsHash;

            return hash;
        }

        static Hash ChessGetHash(string className)
        {
            string fileName = className + ".tsv";
            SettingParser parser = new SettingParser(fileName);
            parser.Parse(true);

            Hash hash = new Hash();
            hash["ClassName"] = className;

            List<Hash> headsHash = new List<Hash>();

            foreach (var h in parser.heads)
            {
                HeadInfo headInfo = h.Value;
                Hash head = new Hash();

                head["name"] = headInfo.name;
                head["type"] = headInfo.type;
                head["meta"] = headInfo.meta;

                Debug.Log("head name " + headInfo.name);
                headsHash.Add(head);
            }

            hash["heads"] = headsHash;

            return hash;
        }

    }

}

