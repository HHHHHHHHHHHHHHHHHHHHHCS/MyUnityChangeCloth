using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainUIManager : MonoBehaviour
{
    [SerializeField]
    private Toggle ItemTogglePrefab;
    [SerializeField]
    private ToggleGroup tabGroup;
    [SerializeField]
    private Transform contentGroupTS;
    [SerializeField]
    private Toggle boyToggle, girlToggle;

    private string activeTab;
    private ToggleGroup contentGroup;

    private void Awake()
    {
        var tabToggles = tabGroup.GetComponentsInChildren<Toggle>();
        foreach (var item in tabToggles)
        {
            item.onValueChanged.AddListener(ChangeTab);
        }

        boyToggle.onValueChanged.AddListener(ChangeBoyAvotor);
        girlToggle.onValueChanged.AddListener(ChangeGirlAvotor);
    }

    private void ChangeTab(bool isShow)
    {
        if (!isShow)
        {
            return;
        }

        foreach (var item in tabGroup.ActiveToggles())
        {
            activeTab = item.name;
            break;
        }

        CreateNewContentGroup();

        var sprites = MainGameManager.Instance.GetSpritesByPart(activeTab);
        if (sprites != null)
        {
            Toggle t0 = null;
            for (int i = 0; i < sprites.Count; i++)
            {
                Toggle newToggle = Instantiate(ItemTogglePrefab, contentGroupTS);
                newToggle.group = contentGroup;
                newToggle.transform.Find(AssetBundleNames.itemSprite)
                    .GetComponent<Image>().sprite = sprites[i];
                newToggle.GetComponent<ItemTogglePrefab>().Init(activeTab, (i + 1).ToString());
                if (i == 0)
                {
                    t0 = newToggle;
                }
            }
            if (t0)
            {
                contentGroup.NotifyToggleOn(t0);
            }
        }
    }

    /// <summary>
    /// 删除老的ContentGroup 创建新的取代
    /// </summary>
    private void CreateNewContentGroup()
    {
        if (contentGroup)
        {
            foreach (Transform item in contentGroup.transform)
            {
                Destroy(item.gameObject);
            }
            DestroyImmediate(contentGroup);
        }
        contentGroup = contentGroupTS.gameObject.AddComponent<ToggleGroup>();
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
