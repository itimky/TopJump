﻿using UnityEngine;
using System.Collections;

public class SortingLayerExposer : MonoBehaviour
{

    public string SortingLayerName = "Default";
    public int SortingOrder = 0;

    void Awake()
    {
        var meshRenderer = gameObject.GetComponent<MeshRenderer>();
        meshRenderer.sortingLayerName = SortingLayerName;
        meshRenderer.sortingOrder = SortingOrder;
    }
}