using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UIElements;

public class EffectManager : MonoBehaviour
{
    public GameManager gameManager;
    public Canvas canvas;
    public ParticleSystem gradientBurstEffect;

    public void GradientBurstAtVE(VisualElement ve, GColor gColor)
    {
        ParticleSystem ps = Instantiate(gradientBurstEffect);

        Vector2 location = new Vector2(ve.worldBound.center.x, canvas.GetComponent<RectTransform>().rect.height - ve.worldBound.center.y);// + ve.worldBound.yMin / 2);

        ps.transform.position = location;
        ps.transform.SetParent(canvas.transform);
        
        var main = ps.main;
        main.startColor = new ParticleSystem.MinMaxGradient(gColor.ToColor32(), gColor.ToColor32());
        
        Destroy(ps.gameObject, ps.main.duration);
    }
}
