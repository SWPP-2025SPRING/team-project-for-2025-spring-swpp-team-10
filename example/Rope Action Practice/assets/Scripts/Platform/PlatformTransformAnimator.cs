#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using DG.Tweening;
using DG.Tweening.Core.Easing;

// 현재 방식은 Position, Rotation, Scale 중 하나를 전제하고 Vector3값을 조정하여 애니메이션을 출력하는 방식이다.
// ith 애니메이션 각각에서 Position, Rotation, Scale 중 하나를 선택하고,
// Vector3값 자체가 아닌 x, y, z 중 1~3가지만 골라서 그 값만 바꾸게 하는 함수가 필요해 보인다.
// position에서 (x,z)와 y가 다른 방식으로 움직이거나, 이동->회전->이동이 가능하게끔 하는 것이 필요해 보인다.
public class PlatformTransformAnimator : MonoBehaviour
{
    public enum Type { Position, Rotation, Scale }
    [Tooltip("Trnasform에서 움직일 프로퍼티 설정")]
    public Type modifyType;
    //[Header("moveTime 동안 움직인 뒤 interval 동안 체류 \n0th -> 1th -> ... -> nth -> 0th -> ...")]
    [Tooltip("해당 시간만큼 대기하다가 이동 시작")]
    public float startDelay;
    public MoveSequence[] seqs;

    void Start()
    {
        Init();
        Invoke("SeqStart", startDelay);
    }

    void Init()
    {
        switch (modifyType) {
            case Type.Position:
                transform.position = seqs[0].value;
                break;
            case Type.Rotation:
                transform.rotation = Quaternion.Euler(seqs[0].value);
                break;
            case Type.Scale:  
                transform.localScale = seqs[0].value;
                break;
        }
    }

    void SeqStart()
    {
        Sequence seq = DOTween.Sequence();
        for (int i = 1; i < seqs.Length; i++) {
            seq.Append(Do(i))
               .AppendInterval(seqs[i].interval);
        }
        seq.Append(Do(0))
           .AppendInterval(seqs[0].interval);
        seq.SetLoops(-1, LoopType.Restart);
    }

    // modifyType에 맞는 행동 반환
    Tween Do(int i)
    {
        switch (modifyType) {
            case Type.Position:
                return CustomSetEase(transform.DOMove(seqs[i].value, seqs[i].moveTime), i);
            case Type.Rotation:
                return CustomSetEase(transform.DORotate(seqs[i].value, seqs[i].moveTime, RotateMode.FastBeyond360), i);
            default: // Type.Scale:
                return CustomSetEase(transform.DOScale(seqs[i].value, seqs[i].moveTime), i);
        }
    }

    // Tween에 customEase또는 ease를 입힘
    Tween CustomSetEase(Tween tw, int i)
    {
        if (seqs[i].isCustomCurve) return tw.SetEase(seqs[i].customEase);
        else return tw.SetEase(seqs[i].ease);
    }
}

[System.Serializable]
public class MoveSequence {
    public Vector3 value;
    public float moveTime;
    public float interval;
    public bool isCustomCurve;
    public DG.Tweening.Ease ease;
    public AnimationCurve customEase; // 직접 조절 가능한 ease
}

#if UNITY_EDITOR
// ChatGPT 활용
[CustomPropertyDrawer(typeof(MoveSequence))]
public class MoveSequenceDrawer : PropertyDrawer
{
    const float padding = 2f;

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        int lines = 5; // 기본 라인 수
        return EditorGUIUtility.singleLineHeight * lines + padding * (lines - 1);
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        float y = position.y;
        float lineHeight = EditorGUIUtility.singleLineHeight;

        var valueProp = property.FindPropertyRelative("value");
        var moveTimeProp = property.FindPropertyRelative("moveTime");
        var intervalProp = property.FindPropertyRelative("interval");
        var isCustomCurveProp = property.FindPropertyRelative("isCustomCurve");
        var easeProp = property.FindPropertyRelative("ease");
        var customEaseProp = property.FindPropertyRelative("customEase");

        // 기본 필드들
        EditorGUI.PropertyField(new Rect(position.x, y, position.width, lineHeight), valueProp);
        y += lineHeight + padding;

        EditorGUI.PropertyField(new Rect(position.x, y, position.width, lineHeight), moveTimeProp);
        y += lineHeight + padding;

        EditorGUI.PropertyField(new Rect(position.x, y, position.width, lineHeight), intervalProp);
        y += lineHeight + padding;

        EditorGUI.PropertyField(new Rect(position.x, y, position.width, lineHeight), isCustomCurveProp);
        y += lineHeight + padding;

        // 조건부 필드들
        if (isCustomCurveProp.boolValue) {
            var buttonRect = new Rect(position.x + position.width - 75f, y, 75f, lineHeight);

            // 배경 강조 색상
            EditorGUI.DrawRect(new Rect(position.x, y, position.width, lineHeight), new Color(0.2f, 0.4f, 0.2f, 0.2f));
            EditorGUI.PropertyField(new Rect(position.x, y, position.width - 75f, lineHeight), customEaseProp);
            if (GUI.Button(buttonRect, "Init")) {
                AnimationCurve converted = GetLinearEaseCurve();
                customEaseProp.animationCurveValue = converted;
            }
        }
        else {
            // 배경 강조 색상
            EditorGUI.DrawRect(new Rect(position.x, y, position.width, lineHeight), new Color(0.2f, 0.3f, 0.6f, 0.2f));
            EditorGUI.PropertyField(new Rect(position.x, y, position.width, lineHeight), easeProp);
        }

        EditorGUI.EndProperty();
    }

    private AnimationCurve GetLinearEaseCurve()
    {
        Keyframe[] keys = new Keyframe[2];
        keys[0] = new Keyframe(0, 0);
        keys[1] = new Keyframe(1, 1);
        return new AnimationCurve(keys);
    }
}


[CustomEditor(typeof(PlatformTransformAnimator))]
public class PlatformTransformAnimatorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // 기본 속성 가져오기
        SerializedProperty modifyTypeProp = serializedObject.FindProperty("modifyType");
        SerializedProperty startDelayProp = serializedObject.FindProperty("startDelay");
        SerializedProperty seqsProp = serializedObject.FindProperty("seqs");

        // Modify Type
        EditorGUILayout.PropertyField(modifyTypeProp);

        // ✅ HelpBox로 설명 출력
        EditorGUILayout.HelpBox("moveTime 동안 움직인 뒤 interval 동안 체류\n0th -> 1th -> ... -> nth -> 0th -> ...", MessageType.Info);

        // Start Delay
        EditorGUILayout.PropertyField(startDelayProp);

        // Seqs 배열
        EditorGUILayout.PropertyField(seqsProp, true);

        serializedObject.ApplyModifiedProperties();
    }
}
#endif