using Data;
using Units.Views;
using UnityEngine;

public class PortraitMaker : MonoBehaviour
{
    [SerializeField] private Camera camera;
    [SerializeField] private Transform head;
    [SerializeField] private UnitView unit;

    private Texture2D ToTexture2D(RenderTexture rTex)
    {
        Texture2D tex = new Texture2D(rTex.width, rTex.height, TextureFormat.RGB24, false);
        var old_rt = RenderTexture.active;
        RenderTexture.active = rTex;

        tex.ReadPixels(new Rect(0, 0, rTex.width, rTex.height), 0, 0);
        tex.Apply();

        RenderTexture.active = old_rt;
        return tex;
    }

    public Sprite GetPortrait(Appearance appearance)
    {
        unit.SetAppearance(appearance);
        camera.transform.position = head.transform.position + Vector3.back;
        camera.transform.LookAt(head);
        camera.Render();
        var tex = ToTexture2D(camera.targetTexture);
        return Sprite.Create(tex, new Rect(Vector2.zero, new Vector2(tex.width, tex.height)), Vector2.one * .5f);
    }
}
