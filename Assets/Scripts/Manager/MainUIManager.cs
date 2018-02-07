using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainUIManager : MonoBehaviour
{
    [SerializeField]
    private Toggle boyToggle, girlToggle;

    private void Awake()
    {
        boyToggle.onValueChanged.AddListener(ChangeBoyAvotor);
        girlToggle.onValueChanged.AddListener(ChangeGirlAvotor);
    }

    private void ChangeBoyAvotor(bool isShow)
    {
        MainGameManager.Instance.ShowHideBoy(isShow);
    }

    private void ChangeGirlAvotor(bool isShow)
    {
        MainGameManager.Instance.ShowHideGirl(isShow);
    }
}
