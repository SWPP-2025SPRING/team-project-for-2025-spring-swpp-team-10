using UnityEngine;

// Track > MovingPlatform에 있는 스크립트에서 오브젝트를 '비주얼용'과 '물리 충돌용'으로 분리합니다.
// 이 컴포넌트는 해당 오브젝트가 그렇게 분리된 플랫폼 중 하나임을 나타내며,
// 자신이 물리 충돌용인지 여부와 반대편 오브젝트를 참조합니다.
public class SplitMovingPlatform : MonoBehaviour
{
    [Tooltip("이 오브젝트가 물리 충돌(Rigidbody + Collider) 처리용인지 여부")]
    public bool isPhysics;

    [Tooltip("자신과 짝이 되는 반대쪽 오브젝트 (비주얼 또는 물리)")]
    public GameObject otherSideObject;



    /// <summary>
    /// 원본 오브젝트를 '비주얼용'과 '물리 충돌용'으로 분리하여 하나의 루트 오브젝트 하위에 구성합니다.
    /// - 원본 오브젝트는 위치 동기화만 담당 (Transform 전용)
    /// - 복제된 오브젝트는 충돌 처리를 담당 (Rigidbody + Collider 전용)
    /// </summary>
    /// <param name="original">분리 대상이 되는 원본 GameObject</param>
    /// <returns>충돌 처리용으로 생성된 physicsPlatform 오브젝트</returns>
    public static GameObject SplitPlatform(GameObject original)
    {
        string originalName = original.name;

        // Root 오브젝트 생성: 비주얼/물리용 오브젝트의 부모로 사용
        GameObject root = new GameObject(originalName + " Root");
        root.transform.parent = original.transform.parent; // 원래 부모 유지
        root.transform.SetSiblingIndex(original.transform.GetSiblingIndex()); // 하이어라키 순서 유지
        root.transform.position = Vector3.zero;
        root.transform.rotation = Quaternion.identity;
        root.transform.localScale = Vector3.one;

        // 원본을 복제해서 충돌 처리용 오브젝트 생성
        GameObject physicsPlatform = Instantiate(original);
        physicsPlatform.layer = LayerMask.NameToLayer("MovingPlatformPhysics");

        // 이름 변경
        physicsPlatform.name = originalName + " Physics";
        original.name = originalName + " Transform";

        // 부모 설정
        physicsPlatform.transform.parent = root.transform;
        original.transform.parent = root.transform;

        // 필요한 컴포넌트만 남기고 나머지는 제거, 또는 필요한 컴포넌트 추가
        KeepOnlyCertainComponents(physicsPlatform, original, true); // 물리 충돌용
        KeepOnlyCertainComponents(original, physicsPlatform, false);

        return physicsPlatform;
    }

    /// <summary>
    /// GameObject에서 지정한 타입에 따라 필요한 컴포넌트만 유지하고 나머지를 제거합니다.
    /// 또한 SplitPlatform 상태를 나타내는 컴포넌트를 추가합니다.
    /// </summary>
    /// <param name="obj">정리 대상 GameObject</param>
    /// <param name="otherSideObj">짝이 되는 반대편 GameObject</param>
    /// <param name="isPhysics">물리 충돌용 여부 (true면 Rigidbody + Collider 유지)</param>
    private static void KeepOnlyCertainComponents(GameObject obj, GameObject otherSideObj, bool isPhysics)
    {
        Component[] components = obj.GetComponents<Component>();

        // 컴포넌트 삭제
        foreach (Component comp in components)
        {
            bool physicsProp = comp is Collider || comp is Rigidbody
                            || comp is ObjectProperties;

            // 자신이 가지고 있어야 할 컴포넌트라면 continue
            if (comp is Transform || isPhysics && physicsProp || !isPhysics && !physicsProp)
                continue;

            Destroy(comp);
        }

        // 컴포넌트 추가
        SplitMovingPlatform split = obj.AddComponent<SplitMovingPlatform>();
        if (isPhysics && !obj.TryGetComponent(out Rigidbody physicsPlatformRb))
        {
            physicsPlatformRb = obj.AddComponent<Rigidbody>();
            physicsPlatformRb.isKinematic = true;
        }

        split.isPhysics = isPhysics;
        split.otherSideObject = otherSideObj;
    }
}