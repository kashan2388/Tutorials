using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MapGenerator))]
public class MapEditor : Editor
{
    public override void OnInspectorGUI()
    {
        MapGenerator map = target as MapGenerator;

        if (DrawDefaultInspector())//인스펙터에서의 값이 경신되었을 때만 true를 반환  
        {
            map.GenerateMap();
        }
        //-> 값이 바뀔 때만 실제로 맵을 재생성하게끔 

        if(GUILayout.Button("Generate Map"))
        {
            map.GenerateMap();
        }


    }
}
