using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Window1 : EditorWindow
{

    Transform TF1;
    Transform TF2;

    //菜单工具
    [MenuItem("Tools/坐标差")]
    static void ShowWindow()
    {
        GetWindow<Window1>().Show();
    }



private void OnGUI()
    {
        GUILayout.Label("B-A");


        EditorGUILayout.Space();
        TF1 = EditorGUILayout.ObjectField(TF1, typeof(Transform), true)as Transform;
        TF2 = EditorGUILayout.ObjectField(TF2, typeof(Transform), true) as Transform;

        EditorGUILayout.Space();
        if (GUILayout.Button("坐标差"))
        {
            if(TF1 && TF2)
            {
                CopyStr((TF2.position - TF1.position).ToString());
                Debug.Log("坐标差"+(TF2.position - TF1.position));
            }
        }


        GUILayout.FlexibleSpace();//插入一个弹性空白  

    }

    private void CopyStr(string value)
    {
        TextEditor te = new TextEditor();
        te.text = value;
        te.SelectAll(); //等效于te.OnFocus();
        te.Copy();
    }

}
