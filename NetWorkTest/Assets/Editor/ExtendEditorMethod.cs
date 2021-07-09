using UnityEngine;
using UnityEditor;

public class ExtendEditorMethod
{
    /// <summary>
    /// 快速复制物体坐标
    /// </summary>
    [MenuItem("CONTEXT/Transform/获取坐标")] 
    static void DoIt(MenuCommand cmd)
    { 
        var obj = cmd.context as Transform;
        TextEditor te = new TextEditor();
        te.text = obj.position.ToString();
        //te.SelectAll();
        te.OnFocus();
        te.Copy();
        Debug.Log(obj.position);
       
    }
}