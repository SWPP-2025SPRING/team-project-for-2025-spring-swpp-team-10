using UnityEditor;
using UnityEngine;

public class ConvertLegacyDiffuseToURPLit : MonoBehaviour
{
    [MenuItem("Tools/Convert Legacy Diffuse to URP Lit")]
    static void Convert()
    {
        // 전체 프로젝트의 머티리얼을 검색
        string[] guids = AssetDatabase.FindAssets("t:Material");
        int count = 0;

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            Material mat = AssetDatabase.LoadAssetAtPath<Material>(path);

            if (mat.shader.name == "Legacy Shaders/Diffuse")
            {
                Color mainColor = mat.color;

                // URP Lit 셰이더로 변경
                mat.shader = Shader.Find("Universal Render Pipeline/Lit");
                mat.SetColor("_BaseColor", mainColor);

                count++;
                Debug.Log($"Converted: {path}");
            }
        }

        Debug.Log($"변환 완료: {count}개 매테리얼 처리됨.");
    }
}
