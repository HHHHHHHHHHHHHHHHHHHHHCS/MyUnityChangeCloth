using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainGameManager : MonoBehaviour
{
    public static MainGameManager Instance { get; private set; }


    public BoyAvatar boy;
    public GirlAvatar girl;

    private Dictionary<string, AssetBundle> assetBundleDic;
    private AvatarSystem currentAvatar;

    private void Awake()
    {
        Instance = this;
        StartCoroutine(Init());
    }

    private IEnumerator Init()
    {
        yield return LocalHandler.CheckAssetBundle();
        assetBundleDic = LocalHandler.LoadAllAssetBundles();
        if (assetBundleDic != null && assetBundleDic.Count > 0)
        {//这里如果大于0，默认就当加载成功了，否则就是失败了
            //正常要单独判断包
            boy.Init(assetBundleDic[AssetBundleNames.male_dic]);
            girl.Init(assetBundleDic[AssetBundleNames.female_dic]);
        }
        ShowHideBoy(true);
    }

    public void ShowHideBoy(bool isShow)
    {
        boy.ShowHideAvatar(isShow);
        if (isShow)
        {
            currentAvatar = boy;
        }
    }

    public void ShowHideGirl(bool isShow)
    {
        girl.ShowHideAvatar(isShow);
        if (isShow)
        {
            currentAvatar = girl;
        }
    }

    public List<Sprite> GetSpritesByPart(string part)
    {
        return currentAvatar ? currentAvatar.GetSpritesByPart(part) : null;
    }

    public void ChangeCurrentPart(string part,string index)
    {
        currentAvatar.ChangeMesh(part, index);
    }
}
