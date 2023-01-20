using System;
using System.Collections;
using System.Collections.Generic;
using Data;
using Units.Views;
using UnityEngine;

public class PortraitMaker : MonoBehaviour
{
    [SerializeField] private Camera camera;
    [SerializeField] private Transform head;
    [SerializeField] private UnitView unit;

    private Queue<Tuple<Appearance, Action<Sprite>>> _queue;
    private Coroutine _coroutine;

    private void Awake()
    {
        _queue = new Queue<Tuple<Appearance, Action<Sprite>>>();
    }

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

    public void GetPortrait(Appearance appearance, Action<Sprite> callback)
    {
        if (_queue.Count == 0 && _coroutine == null)
            _coroutine = StartCoroutine(GetSprite(appearance, callback));
        else
            _queue.Enqueue(new Tuple<Appearance, Action<Sprite>>(appearance, callback));
    }

    IEnumerator GetSprite(Appearance appearance, Action<Sprite> callback)
    {
        yield return null;
        
        unit.SetAppearance(appearance);
        camera.transform.position = head.transform.position + Vector3.back;
        camera.transform.LookAt(head);
        
        yield return null;
        
        camera.Render();
        var tex = ToTexture2D(camera.targetTexture);
        callback?.Invoke(Sprite.Create(tex, new Rect(Vector2.zero, new Vector2(tex.width, tex.height)), Vector2.one * .5f));

        if (_queue.Count > 0)
        {
            var t = _queue.Dequeue();
            _coroutine = StartCoroutine(GetSprite(t.Item1, t.Item2));
        }
        else
        {
            _coroutine = null;
        }
    }
}
