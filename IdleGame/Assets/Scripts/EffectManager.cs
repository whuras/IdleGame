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
        Vector2 location = new Vector2(ve.worldBound.center.x, canvas.GetComponent<RectTransform>().rect.height - ve.worldBound.center.y);
        if (float.IsNaN(location.x) || float.IsNaN(location.y))
            return;

        float psSize = 256 / gameManager.SizeBasedOnLevel();

        ParticleSystem ps = Instantiate(gradientBurstEffect);

        var main = ps.main;
        main.startColor = new ParticleSystem.MinMaxGradient(gColor.ToColor32(), gColor.ToColor32());
        main.startSize = psSize;

        ParticleSystem.MinMaxCurve rate = new ParticleSystem.MinMaxCurve();
        rate.constantMax = psSize;

        var velOL = ps.limitVelocityOverLifetime;
        velOL.limit = rate;

        ps.transform.position = location;
        ps.transform.SetParent(canvas.transform);

        Destroy(ps.gameObject, ps.main.duration);
    }
}
