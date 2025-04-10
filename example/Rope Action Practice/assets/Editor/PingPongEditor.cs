using UnityEditor;
using UnityEngine;

// https://goraniunity2d.blogspot.com/2020/06/blog-post_25.html

[CustomEditor(typeof(PingPong))]
public class PingPongEditor : Editor
{
    PingPong p;

    void OnEnable() // 큐브를 비활성화->활성화할 때마다, 큐브 인스펙터가 켜질 때마다 발동
    {
        p = target as PingPong;
    }

    public override void OnInspectorGUI()
    {
        p.inputType = (PingPong.Type)EditorGUILayout.EnumPopup("Input Type", p.inputType);
        if (p.inputType == PingPong.Type.Transform) {
            p.start = (Transform)EditorGUILayout.ObjectField("Start", p.start, typeof(Object), true);
            p.end = (Transform)EditorGUILayout.ObjectField("End", p.end, typeof(Object), true);
        }
        else {
            p.startVec = EditorGUILayout.Vector3Field("Start Vec", p.startVec);
            p.endVec = EditorGUILayout.Vector3Field("End Vec", p.endVec);
        }
        // GUIContent 이용하면 툴팁 사용 가능
        p.startDelay = EditorGUILayout.FloatField(new GUIContent("Start Delay", "해당 시간 후에 시작 위치(startVec)로 오게 됨"), p.startDelay);
        p.moveTime = EditorGUILayout.FloatField(new GUIContent("Move Time", "Start -> End까지 가는 데 걸리는 시간"), p.moveTime);
    }
}
