using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayBGM : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        AudioManager.Instance.PlayBgm(AudioSystem.BgmType.MainSceneBgm);
    }

}
