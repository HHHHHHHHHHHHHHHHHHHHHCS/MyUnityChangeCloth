using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainGameManager : MonoBehaviour
{
    public BoyAvator boy;
    public GirlAvator girl;
    private Dictionary<string, AssetBundle> assetBundleDic;

    private void Awake()
    {
        StartCoroutine(Init());
    }

    private IEnumerator Init()
    {
        yield return LocalHandler.CheckAssetBundle();
        assetBundleDic = LocalHandler.LoadAllAssetBundles();
        if(assetBundleDic.Count>0)
        {//这里如果大于0，默认就当加载成功了，否则就是失败了
            //正常要单独判断包
            boy.Init(assetBundleDic[URL.male_dic]);
            girl.Init(assetBundleDic[URL.female_dic]);
        }
    }
}
