using UnityEngine;
using System.Collections;

public class AstralPlatformBehavior : TogglePlatform
{
    public float AstralTransparency;

    public bool IsAstral { get; private set; }

    protected override void Toggle()
    {
        var newTransparency = IsAstral ? 1f : AstralTransparency;
        SetTransparency(newTransparency);
        IsAstral = !IsAstral;
        GetComponent<BoxCollider2D>().isTrigger = IsAstral;
    }

    void SetTransparency(float alpha)
    {
        var renderer = GetComponent<Renderer>();
        var mat = renderer.material;
        renderer.material.color = new Color(mat.color.r, mat.color.g, mat.color.b, alpha);
    }
}
