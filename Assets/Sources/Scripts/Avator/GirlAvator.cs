using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GirlAvator : AvatorSystem
{
    private Transform target;

    protected override void Awake()
    {
        var source = InitSource("Prefabs/FemaleModel");
        target = InitTarget("Prefabs/FemaleModelTarget");
        InitData(source, target);

        InitAvator();
    }
}
