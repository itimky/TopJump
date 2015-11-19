using UnityEngine;
using System.Collections;

public static class LayerManager
{
    public static int FallingBonuses = LayerMask.NameToLayer("FallingBonuses");
    public static int Treasures = LayerMask.NameToLayer("Treasures");
    public static int PlatformBonuses = LayerMask.NameToLayer("PlatformBonuses");
}
