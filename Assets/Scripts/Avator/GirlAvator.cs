using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GirlAvator : AvatorSystem
{

    public void Init(AssetBundle ab)
    {
        if(ab)
        {
            assetBundle = ab;
            var source = InitSource("Assets/Prefabs/FemaleModel.prefab");
            InitTarget("Assets/Prefabs/FemaleModelTarget.prefab");
            InitData(source);
            InitJson();

            InitAvator();
            ShowHideAvator(false);
        }
    }
}
