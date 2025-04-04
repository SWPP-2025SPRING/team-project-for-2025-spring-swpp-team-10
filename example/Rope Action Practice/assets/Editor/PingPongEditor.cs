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
        // serializedObject.Update();

        // // PingPong 스크립트 가져오기
        // PingPong script = (PingPong)target;

        // // inputType Enum 표시
        // SerializedProperty inputTypeProp = serializedObject.FindProperty("inputType");
        // EditorGUILayout.PropertyField(inputTypeProp);

        // // 선택된 타입에 따라 다른 속성 보이기
        // if ((PingPong.Type)inputTypeProp.enumValueIndex == PingPong.Type.Transform)
        // {
        //     EditorGUILayout.PropertyField(serializedObject.FindProperty("start"));
        //     EditorGUILayout.PropertyField(serializedObject.FindProperty("end"));
        // }
        // else
        // {
        //     EditorGUILayout.PropertyField(serializedObject.FindProperty("startVec"));
        //     EditorGUILayout.PropertyField(serializedObject.FindProperty("endVec"));
        // }

        // // 나머지 속성 표시
        // EditorGUILayout.PropertyField(serializedObject.FindProperty("speed"));
        // EditorGUILayout.PropertyField(serializedObject.FindProperty("offset"));

        // serializedObject.ApplyModifiedProperties();

        

        p.inputType = (PingPong.Type)EditorGUILayout.EnumPopup("Input Type", p.inputType);
        if (p.inputType == PingPong.Type.Transform) {
            p.start = (Transform)EditorGUILayout.ObjectField("Start", p.start, typeof(Object), true);
            p.end = (Transform)EditorGUILayout.ObjectField("End", p.end, typeof(Object), true);
        }
        else {
            p.startVec = EditorGUILayout.Vector3Field("Start Vec", p.startVec);
            p.endVec = EditorGUILayout.Vector3Field("End Vec", p.endVec);
        }
        p.moveTime = EditorGUILayout.FloatField("Move Time", p.moveTime);
        // GUIContent 이용하면 툴팁 사용 가능
        p.offset = EditorGUILayout.FloatField(new GUIContent("Offset", "다른 움직이는 오브젝트와의 핑퐁 타이밍 조절"), p.offset);
    }
}
