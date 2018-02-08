using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoyAvatar : AvatarSystem
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

            InitAvatar();
            ShowHideAvatar(false);
        }
    }
}
