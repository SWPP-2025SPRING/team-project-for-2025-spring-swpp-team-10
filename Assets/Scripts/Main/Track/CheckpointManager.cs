using UnityEngine;
using System.Collections.Generic;
using Hampossible.Utils;

public class CheckpointManager : RuntimeSingleton<CheckpointManager>
{
    //public static CheckpointManager Instance { get; private set; }

    [Tooltip("여기에 모든 체크포인트 게임 오브젝트를 원하는 활성화 순서대로 할당해주세요.")]
    [SerializeField] private List<CheckpointController> orderedCheckpoints = new List<CheckpointController>();

    [Tooltip("플레이어의 시작 지점입니다. 체크포인트가 설정되지 않은 경우 이 위치로 리스폰됩니다.")]
    [SerializeField] private Transform initialSpawnPoint;

    // 마지막으로 저장된 체크포인트 위치
    private Vector3 _lastCheckpointPosition;
    private bool _hasCheckpointBeenSet = false; // 체크포인트가 한 번이라도 설정되었는지 여부
    private int _currentCheckpointIndex = -1;

    // 옵저버 패턴 : 옵저버 목록
    private List<INextCheckpointObserver> _checkpointObservers = new List<INextCheckpointObserver>();

    // 옵저버 패턴: 옵저버 등록 메소드
    public void RegisterObserver(INextCheckpointObserver observer)
    {
        if (!_checkpointObservers.Contains(observer))
        {
            _checkpointObservers.Add(observer);
        }
    }

    // 옵저버 패턴: 옵저버 해지 메소드
    public void UnregisterObserver(INextCheckpointObserver observer)
    {
        if (_checkpointObservers.Contains(observer))
        {
            _checkpointObservers.Remove(observer);
        }
    }

    protected override void Awake()
    {
        base.Awake();

        // 초기 스폰 포인트 설정
        if (initialSpawnPoint != null)
        {
            _lastCheckpointPosition = initialSpawnPoint.position;
            HLogger.General.Info($"초기 체크포인트 위치가 {initialSpawnPoint.name}의 위치({_lastCheckpointPosition})로 설정되었습니다.");
        }
        else
        {
            HLogger.General.Warning("CheckpointManager: 초기 스폰 포인트(Initial Spawn Point)가 설정되지 않았습니다. 첫 번째 체크포인트 활성화 전까지 기본 위치를 사용합니다.");
        }

        
    }

    private void Start()
    {
        // 게임 시작 시 다음 체크포인트 정보를 알립니다.
        // orderedCheckpoints가 비어있지 않고, 아직 어떤 체크포인트도 활성화되지 않았다면
        // 첫번째 체크포인트가 다음 체크포인트가 됩니다.
        if (orderedCheckpoints.Count > 0 && _currentCheckpointIndex == -1)
        {
             NotifyNextCheckpointUpdated(orderedCheckpoints[0].transform.position);
        }
        else if (orderedCheckpoints.Count == 0)
        {
            NotifyNextCheckpointUpdated(null); // 알릴 체크포인트 없음
        }
        // 이미 로드된 세션에서 _currentCheckpointIndex가 유효한 값을 가질 수 있으므로,
        // 해당 상황도 고려하여 다음 체크포인트를 알릴 수 있습니다. 
        else if (_currentCheckpointIndex >= -1) // 일반적인 상황에도 다음 체크포인트 위치를 알리도록 합니다.
        {
            NotifyNextCheckpointUpdated(GetNextCheckpointPosition());
        }

    }

    public void CheckpointActivated(CheckpointController activatedCheckpoint)
    {
        if (activatedCheckpoint == null)
        {
            HLogger.General.Warning("CheckpointActivated가 null 체크포인트와 함께 호출되었습니다.", gameObject);
            return;
        }

        // 활성화된 체크포인트가 orderedCheckpoints 리스트에서 몇 번째인지 찾습니다.
        int activatedIndex = orderedCheckpoints.IndexOf(activatedCheckpoint);

        if (activatedIndex == -1)
        {
            // 만약 순서 목록에 없는 체크포인트라면, 리스폰 지점만 갱신하고 순서에는 영향을 주지 않습니다.
            HLogger.General.Warning($"체크포인트 '{activatedCheckpoint.gameObject.name}'가 순서 목록에 없습니다. 리스폰 지점은 설정되지만, 순서 로직에는 영향을 주지 않습니다.", activatedCheckpoint.gameObject);
            _lastCheckpointPosition = activatedCheckpoint.transform.position;
            _hasCheckpointBeenSet = true;
            return;
        }

        // 현재까지 진행한 체크포인트보다 더 앞선 체크포인트일 경우에만 순서를 업데이트합니다.
        // _currentCheckpointIndex가 -1 (시작 전)일 때 activatedIndex가 0 (첫번째 체크포인트)이면 진행으로 처리
        if (activatedIndex > _currentCheckpointIndex)
        {
            _currentCheckpointIndex = activatedIndex;
            _lastCheckpointPosition = activatedCheckpoint.transform.position; // 리스폰 위치도 갱신
            _hasCheckpointBeenSet = true;
            HLogger.General.Info($"체크포인트 '{activatedCheckpoint.gameObject.name}' (인덱스: {_currentCheckpointIndex}) 활성화됨. 위치: {_lastCheckpointPosition}");

            Vector3? nextCheckpointPos = GetNextCheckpointPosition(); // 다음 체크포인트 위치 가져오기
            NotifyNextCheckpointUpdated(nextCheckpointPos); // 옵저버에게 알림
        }
        else
        {
            // 이전 체크포인트를 다시 활성화하는 경우, 리스폰 위치만 설정하고 순서는 현재 진행된 곳을 유지합니다.
            _lastCheckpointPosition = activatedCheckpoint.transform.position;
            _hasCheckpointBeenSet = true;
            HLogger.General.Info($"체크포인트 '{activatedCheckpoint.gameObject.name}' 재활성화됨. 리스폰 위치: {_lastCheckpointPosition}. 시퀀스는 인덱스 {_currentCheckpointIndex} 유지.");
        }
    }

    /// <summary>
    /// 순서상 다음 체크포인트의 위치를 반환합니다.
    /// 현재 체크포인트가 마지막이거나 정의된 체크포인트가 없으면 null을 반환합니다.
    /// </summary>
    public Vector3? GetNextCheckpointPosition()
    {
        if (orderedCheckpoints.Count == 0)
        {
            HLogger.General.Info("orderedCheckpoints 리스트에 정의된 체크포인트가 없습니다.");
            return null;
        }

        // 현재 _currentCheckpointIndex가 마지막 체크포인트를 가리키고 있다면, 다음은 없습니다.
        if (_currentCheckpointIndex >= orderedCheckpoints.Count - 1)
        {
            HLogger.General.Info("현재 체크포인트가 시퀀스의 마지막입니다.");
            return null;
        }

        int nextIndex = _currentCheckpointIndex + 1;
        // 다음 인덱스가 리스트 범위 내에 있고, 해당 체크포인트가 null이 아닌지 확인합니다.
        if (nextIndex < orderedCheckpoints.Count && orderedCheckpoints[nextIndex] != null)
        {
            return orderedCheckpoints[nextIndex].transform.position;
        }
        else
        {
            return null; // 다음 체크포인트 없음
        }
    }

    /// <summary>
    /// 플레이어가 리스폰할 위치를 반환합니다.
    /// </summary>
    public Vector3 GetLastCheckpointPosition()
    {
        if (_hasCheckpointBeenSet)
        {
            return _lastCheckpointPosition;
        }

        if (initialSpawnPoint != null)
        {
            return initialSpawnPoint.position;
        }

        if (orderedCheckpoints.Count > 0 && orderedCheckpoints[0] != null)
        {
            HLogger.General.Warning("활성화된 체크포인트가 없고 초기 스폰 지점도 없습니다. 리스폰을 위해 리스트의 첫 번째 체크포인트를 사용합니다.");
            return orderedCheckpoints[0].transform.position;
        }

        HLogger.General.Error("리스폰 위치를 결정할 수 없습니다. 활성화된 체크포인트, 초기 스폰 지점, 또는 순서 목록의 첫 체크포인트가 없습니다. Vector3.zero를 반환합니다.");
        return Vector3.zero;
    }

    public bool HasCheckpointBeenSet()
    {
        return _hasCheckpointBeenSet;
    }
    
    /// <summary>
    /// UI 업데이트를 위한 이벤트를 호출하는 헬퍼 메소드입니다.
    /// </summary>
    
    // 옵저버 패턴: 모든 옵저버에게 다음 체크포인트 정보 알림
    private void NotifyNextCheckpointUpdated(Vector3? nextPosition)
    {
        // HLogger를 사용한 로그는 옵저버의 OnNextCheckpointUpdated 내부에서 처리하거나, 여기서 필요한 경우에만 남깁니다.
        if (nextPosition.HasValue)
        {
            HLogger.General.Debug($"알림: 다음 체크포인트 위치는 {nextPosition.Value} 입니다.");
        }
        else
        {
            HLogger.General.Debug("알림: 다음 체크포인트가 없거나 시퀀스가 종료되었습니다.");
        }

        // 리스트 복사 후 순회 (알림 중 옵저버 목록이 변경될 경우에 대비)
        foreach (INextCheckpointObserver observer in new List<INextCheckpointObserver>(_checkpointObservers))
        {
            observer.OnNextCheckpointUpdated(nextPosition);
        }
    }
}