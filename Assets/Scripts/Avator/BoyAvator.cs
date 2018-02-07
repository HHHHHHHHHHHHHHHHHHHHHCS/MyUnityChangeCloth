using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoyAvator : AvatorSystem
{
    public void Init(AssetBundle ab)
    {
        if(ab)
        {
            assetBundle = ab;
            var source = InitSource("Assets/Prefabs/MaleModel.prefab");
            InitTarget("Assets/Prefabs/MaleModelTarget.prefab");
            InitData(source);
            InitJson();

            InitAvator();
            ShowHideAvator(false);
        }
    }
}
