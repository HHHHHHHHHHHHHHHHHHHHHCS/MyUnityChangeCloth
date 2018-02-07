using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoyAvator : AvatorSystem
{
    private Transform target;

    public void Init(AssetBundle ab)
    {
        if(ab)
        {
            assetBundle = ab;
            var source = InitSource("Assets/Prefabs/MaleModel.prefab");
            target = InitTarget("Assets/Prefabs/MaleModelTarget.prefab");
            InitData(source, target);

            InitAvator();
            ShowHideAvator(false);
        }
    }
}
