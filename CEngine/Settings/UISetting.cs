using System.Collections.Generic;
using UnityEngine;
using CEngine;

//代码由 CompileSettingEditor 自动生成
//请不要对其修改

#region UISetting
public partial class UISettings : BaseCache<UISetting>
{
    public static UISettings instance = new UISettings();
    public string fileName;
    public SettingParser parser;
    public List<UISetting> data;

    public UISettings()
    {
        pool = new Dictionary<string, UISetting>();
        data = new List<UISetting>();
        fileName = "UI.tsv";
        Parse();
    }

    public void Parse()
    { 
        parser = new SettingParser(fileName);
        parser.Parse();
        var enumerator = parser.rows.GetEnumerator();
        while (enumerator.MoveNext())
        {
            TableRow row = enumerator.Current.Value;
            UISetting setting = new UISetting(row);
            if (pool.ContainsKey(row.primaryKey))
            {
                CDebug.LogError("settings is contains key " + row.primaryKey);
                continue;
            }
            pool.Add(row.primaryKey, setting);
            data.Add(setting);
        }
    }
}

public partial class UISetting
{
    public UISetting(TableRow row)
    {
        this.uiName = (string)row.Get("uiName"); 
        this.needUpdate = (bool)row.Get("needUpdate"); 
        this.needMask = (bool)row.Get("needMask"); 
        this.preAction = (string)row.Get("preAction"); 
        this.uiLayer = (int)row.Get("uiLayer"); 
     
    }
    
    /// <summary>
    /// 名称
    /// </summary>
    public string uiName;
    
    /// <summary>
    /// 是否需要Update
    /// </summary>
    public bool needUpdate;
    
    /// <summary>
    /// 是否需要遮罩
    /// </summary>
    public bool needMask;
    
    /// <summary>
    /// 前一个UI退出动作 none hide destroy lockui 
    /// </summary>
    public string preAction;
    
    /// <summary>
    /// ui层次 0 BaseLayer 1 OverLayer 2 GuideLayer
    /// </summary>
    public int uiLayer;
    
}
#endregion
