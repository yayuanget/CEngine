using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using DotLiquid;
using System.IO;

public class TSVFile
{
    public TSVFile(string path)
    {
        this.path = path;
        this.fileName =Path.GetFileNameWithoutExtension(path);
        this.isChecked = false;
    }
    public Hash hash;
    public string path;
    public string fileName;
    public bool isChecked;
}

public class TemplateString
{
    public static string GenCodeTemplateOne =
@"using System.Collections.Generic;
using UnityEngine;
using CEngine;

//代码由 CompileSettingEditor 自动生成
//请不要对其修改

#region {{file.ClassName}}Setting
public partial class {{file.ClassName}}Settings : BaseCache<{{file.ClassName}}Setting>
{
    public static {{file.ClassName}}Settings instance = new {{file.ClassName}}Settings();
    public string fileName;
    public SettingParser parser;
    public List<{{file.ClassName}}Setting> data;

    public {{file.ClassName}}Settings()
    {
        pool = new Dictionary<string, {{file.ClassName}}Setting>();
        data = new List<{{file.ClassName}}Setting>();
        fileName = ""{{file.ClassName}}.tsv"";
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
            {{file.ClassName}}Setting setting = new {{file.ClassName}}Setting(row);
            if (pool.ContainsKey(row.primaryKey))
            {
                Debug.LogError(""settings is contains key "" + row.primaryKey);
                continue;
            }
            pool.Add(row.primaryKey, setting);
            data.Add(setting);
        }
    }
}

public partial class {{file.ClassName}}Setting
{
    public {{file.ClassName}}Setting(TableRow row)
    {
    {% for r in file.heads %}    this.{{r.name}} = ({{r.type}})row.Get(""{{r.name}}""); 
    {% endfor %} 
    }
    {% for r in file.heads %}
    /// <summary>
    /// {{r.meta}}
    /// </summary>
    public {{r.type}} {{r.name}};
    {% endfor %}
}
#endregion
";
    public static string GenCodeTemplateLua =
@"

--代码由 Unity -> Custom/CompileSettingEditor 自动生成
-->> !!!!别改..改了也白改!!!!!!!..-_-||

import ""CommonFunction""
import ""CEngine.TableRow""

{{file.ClassName}}Setting = {

	  {% for r in file.heads %}
      --({{r.type}}){{r.meta}}
      {{r.name}} = 0;{% endfor %}
  
}

{{file.ClassName}}Settings = {};
{{file.ClassName}}Setting_Datas = {}--类似下标数组
{{file.ClassName}}Setting_Datas_Dict = {} --存键值对 <primaryKey ,value> 

function {{file.ClassName}}Settings.Init()
    CommonFunction.Parse(""{{file.ClassName}}.tsv"",""Setting/{{file.ClassName}}Settings.lua"",""{{file.ClassName}}Settings.Add"");
end

local index = 0;
function {{file.ClassName}}Settings.Add(_row)

    local setting = { };

    setmetatable(setting, {{file.ClassName}}Setting);
    {% for r in file.heads %}
    setting.{{r.name}} = _row:Get(""{{r.name}}"") {% endfor %}
  
    setting.__index = index;
    {{file.ClassName}}Setting_Datas[index] = setting;
    {{file.ClassName}}Setting_Datas_Dict[tostring(_row.primaryKey)] = setting;
	index = index + 1;
	
end

function {{file.ClassName}}Settings.Get(_key)
	if not {{file.ClassName}}Setting_Datas_Dict[_key] then
		error(""{{file.ClassName}}Setting_Datas_Dict not Contains key "".._key);

    end
	
	return {{file.ClassName}}Setting_Datas_Dict[_key];
end

--排序 参数1：元表 参数2：排序方法
function {{file.ClassName}}Settings.Sort(_tb,_cmp)
	table.sort(_tb,_cmp);
end
";

}



