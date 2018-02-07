using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GirlAvator : AvatorSystem
{
    private Transform target;

    public void Init(AssetBundle ab)
    {
        if(ab)
        {
            assetBundle = ab;
            var source = InitSource("Assets/Prefabs/FemaleModel.prefab");
            target = InitTarget("Assets/Prefabs/FemaleModelTarget.prefab");
            InitData(source, target);

            InitAvator();
            ShowHideAvator(false);
        }
    }
}
