using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoyAvator : AvatorSystem
{
    private Transform target;

    protected override void Awake()
    {
        var source = InitSource("Prefabs/MaleModel");
        target = InitTarget("Prefabs/MaleModelTarget",1);
        InitData(source, target);

        InitAvator();
    }
}
