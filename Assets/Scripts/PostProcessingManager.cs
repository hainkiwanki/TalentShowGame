using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using DG.Tweening;
using Sirenix.OdinInspector;
using SCPE;

public class PostProcessingManager : Singleton<PostProcessingManager>
{
    private Volume m_volume;
    private Vignette m_vignette;
    private LensDistortion m_lensDistortion;
    private PaniniProjection m_paniniProjection;
    private FilmGrain m_filmGrain;
    private ChromaticAberration m_chromaticAberration;
    private DoubleVision m_doubleVision;
    private TiltShift m_tiltShift;
    private HueShift3D m_hueShift3D;
    private Pixelize m_pixelize;
    private Overlay m_overlay;
    private Ripples m_ripples;
    private Posterize m_posterize;
    private Refraction m_refraction;

    private Camera m_cam;
    private Transform m_player;

    private bool m_hasVolume = false;
    private bool m_isUsingVignette = false;
    
    public void Initialize()
    {
        m_volume = FindObjectOfType<Volume>();
        m_hasVolume = false;
        if (m_volume)
        {
            m_hasVolume = true;
            m_volume.profile.TryGet(out m_vignette);
            m_volume.profile.TryGet(out m_lensDistortion);
            m_volume.profile.TryGet(out m_paniniProjection);
            m_volume.profile.TryGet(out m_filmGrain);
            m_volume.profile.TryGet(out m_chromaticAberration);
            m_volume.profile.TryGet(out m_doubleVision);
            m_volume.profile.TryGet(out m_tiltShift);
            m_volume.profile.TryGet(out m_hueShift3D);
            m_volume.profile.TryGet(out m_pixelize);
            m_volume.profile.TryGet(out m_overlay);
            m_volume.profile.TryGet(out m_ripples);
            m_volume.profile.TryGet(out m_posterize);
            m_volume.profile.TryGet(out m_refraction);
        }
        m_player = GameManager.Inst.playerControl.transform;
    }

    private void Update()
    {
        if (m_isUsingVignette)
        {
            SetVignetteCenter();
        }
    }

    [Button]
    public void SetVignette(float _value, float _time = 10.0f)
    {
        if (!m_vignette || !m_hasVolume) return;
        SetVignetteCenter();
        DOTween.To(() => m_vignette.intensity.value, x => m_vignette.intensity.value = x, _value, 1.0f).OnComplete(() =>
        { m_isUsingVignette = true; });
        DOTween.To(() => m_vignette.intensity.value, x => m_vignette.intensity.value = x, 0.0f, 1.0f).OnComplete(() =>
        { m_isUsingVignette = false; });
    }

    private void SetVignetteCenter()
    {
        m_cam = GameManager.Inst.GetActiveCam();
        Vector3 screenPos = m_cam.WorldToScreenPoint(m_player.position);
        screenPos.x /= Screen.width;
        screenPos.y /= Screen.height;
        m_vignette.center.value = new Vector2(screenPos.x, screenPos.y);
    }

    [Button]
    public void SetLensDistortion(float _value, float _time = 10.0f)
    {
        if (!m_hasVolume) return;
        DOTween.To(() => m_lensDistortion.intensity.value, x => m_lensDistortion.intensity.value = x, _value, 1.0f);
        DOTween.To(() => m_lensDistortion.intensity.value, x => m_lensDistortion.intensity.value = x, 0.0f, 1.0f).SetDelay(_time);
    }

    [Button]
    public void SetPaniniProjection(float _value, float _time = 10.0f)
    {
        if (!m_hasVolume) return;
        DOTween.To(() => m_paniniProjection.distance.value, x => m_paniniProjection.distance.value = x, _value, 1.0f);
        DOTween.To(() => m_paniniProjection.distance.value, x => m_paniniProjection.distance.value = x, 0.0f, 1.0f).SetDelay(_time);
    }

    [Button]
    public void SetFilmGrain(float _value, float _time = 10.0f)
    {
        if (!m_hasVolume) return;
        DOTween.To(() => m_filmGrain.intensity.value, x => m_filmGrain.intensity.value = x, _value, 1.0f);
        DOTween.To(() => m_filmGrain.intensity.value, x => m_filmGrain.intensity.value = x, 0.0f, 1.0f).SetDelay(_time);
    }

    [Button]
    public void SetChromaticAberration(float _value, float _time = 10.0f)
    {
        if (!m_hasVolume) return;
        DOTween.To(() => m_chromaticAberration.intensity.value, x => m_chromaticAberration.intensity.value = x, _value, 1.0f);
        DOTween.To(() => m_chromaticAberration.intensity.value, x => m_chromaticAberration.intensity.value = x, 0.0f, 1.0f).SetDelay(_time);
    }

    [Button]
    public void SetDoubleVision(float _value, float _time = 10.0f)
    {
        if (!m_hasVolume) return;
        if (_value > 0.05f)
        {
            DOTween.To(() => m_doubleVision.intensity.value, x => m_doubleVision.intensity.value = x, _value, 1.0f);
            DOTween.To(() => m_doubleVision.intensity.value, x => m_doubleVision.intensity.value = x, 0.05f, _time).SetDelay(1.0f);
            DOTween.To(() => m_doubleVision.intensity.value, x => m_doubleVision.intensity.value = x, 0.0f, 1.0f).SetDelay(_time + 1.0f);
            
        }
        else
        {
            DOTween.To(() => m_doubleVision.intensity.value, x => m_doubleVision.intensity.value = x, _value, 1.0f);
            DOTween.To(() => m_doubleVision.intensity.value, x => m_doubleVision.intensity.value = x, 0.0f, 1.0f).SetDelay(_time);
        }
    }

    [Button]
    public void SetTitlShift(float _value, float _time = 10.0f)
    {
        if (!m_hasVolume) return;
        DOTween.To(() => m_tiltShift.amount.value, x => m_tiltShift.amount.value = x, _value, 1.0f);
        DOTween.To(() => m_tiltShift.amount.value, x => m_tiltShift.amount.value = x, 0.0f, 1.0f).SetDelay(_time);
    }

    [Button]
    public void SetHueShift3D(float _value, float _time = 10.0f)
    {
        if (!m_hasVolume) return;
        DOTween.To(() => m_hueShift3D.intensity.value, x => m_hueShift3D.intensity.value = x, _value, 1.0f);
        DOTween.To(() => m_hueShift3D.intensity.value, x => m_hueShift3D.intensity.value = x, 0.0f, 1.0f).SetDelay(_time);
    }


    [Button]
    public void SetOverlay(float _value, float _time = 10.0f)
    {
        if (!m_hasVolume) return;
        DOTween.To(() => m_overlay.intensity.value, x => m_overlay.intensity.value = x, _value, 1.0f);
        DOTween.To(() => m_overlay.intensity.value, x => m_overlay.intensity.value = x, 0.0f, 1.0f).SetDelay(_time);
    }

    [Button]
    public void SetPosterize(int _value, float _time = 10.0f)
    {
        if (!m_hasVolume) return;
        DOTween.To(() => m_posterize.saturation.value, x => m_posterize.saturation.value = x, _value, 1.0f);
        DOTween.To(() => m_posterize.saturation.value, x => m_posterize.saturation.value = x, 256, 1.0f).SetDelay(_time);
    }    

    [Button]
    public void SetPixelize(float _value, float _time = 10.0f)
    {
        if (!m_hasVolume) return;
        DOTween.To(() => m_pixelize.amount.value, x => m_pixelize.amount.value = x, _value, 1.0f);
        DOTween.To(() => m_pixelize.amount.value, x => m_pixelize.amount.value = x, 0, 1.0f).SetDelay(_time);
    }

    [Button]
    public void SetRipples(float _value, float _time = 10.0f)
    {
        if (!m_hasVolume) return;
        DOTween.To(() => m_ripples.strength.value, x => m_ripples.strength.value = x, _value, 1.0f).OnComplete(() => {
            DOTween.To(() => m_ripples.strength.value, x => m_ripples.strength.value = x, 1.0f, _time).SetDelay(5.0f).OnComplete(() => {
                DOTween.To(() => m_ripples.strength.value, x => m_ripples.strength.value = x, 0.0f, 1.0f);
            });
        });
    }
    
    [Button]
    public void SetRefraction(float _value, float _time = 10.0f)
    {
        if (!m_hasVolume) return;
        DOTween.To(() => m_refraction.amount.value, x => m_refraction.amount.value = x, _value, 1.0f);
        DOTween.To(() => m_refraction.amount.value, x => m_refraction.amount.value = x, 0.0f, 1.0f).SetDelay(_time);
    }
}