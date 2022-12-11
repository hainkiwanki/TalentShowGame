using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public enum EVisibleState
{
    Visible,
    Hidden
}

public enum EAnimationStyle
{
    None,
    Slide,
    Scale,
    Appear,
}

public enum EAnimationDirection
{
    None,
    Top,
    Left,
    Right,
    Bottom
}

public class UIElement : SerializedMonoBehaviour
{
    [OnValueChanged("OnTriggerChildrenChanged")]
    public bool triggerChildren = false;
    [ShowIf("triggerChildren")]
    public bool playOwnAnimation = true;
    public EVisibleState onstart = EVisibleState.Visible;

    [ShowIfGroup("playOwnAnimation")]
    [BoxGroup("playOwnAnimation/Animation Settings")]
    public EAnimationStyle style = EAnimationStyle.None;

    [BoxGroup("playOwnAnimation/Animation Settings")]
    [ShowIf("style", EAnimationStyle.Slide), HideIf("style", EAnimationStyle.None), HideIf("style", EAnimationStyle.Scale)]
    public EAnimationDirection direction = EAnimationDirection.None;

    [BoxGroup("playOwnAnimation/Animation Settings")]
    [ShowIf("style", EAnimationStyle.Scale)]
    public bool shrink = true;

    [BoxGroup("playOwnAnimation/Animation Settings")]
    [ShowIf("style", EAnimationStyle.Scale), HideIf("shrink")]
    public float scaleFactor = 1.5f;

    [BoxGroup("playOwnAnimation/Animation Settings"), LabelText("Time")]
    [HideIf("style", EAnimationStyle.None)]
    public float animationTime = 0.2f;

    [BoxGroup("playOwnAnimation/Animation Settings")]
    [HorizontalGroup("playOwnAnimation/Animation Settings/Ease"), LabelText("In"), LabelWidth(50.0f)]
    [HideIf("style", EAnimationStyle.None)]
    public Ease easeIn = Ease.OutQuint;

    [BoxGroup("playOwnAnimation/Animation Settings")]
    [HorizontalGroup("playOwnAnimation/Animation Settings/Ease"), LabelText("Out"), LabelWidth(50.0f)]
    [HideIf("style", EAnimationStyle.None)]
    public Ease easeOut = Ease.InQuint;

    [SerializeField]
    private bool m_isHidden = false;
    public bool isHidden => m_isHidden;
    private Vector2 beginPos;
    public float extraOffsetWhenHiding = 0.0f;
    public UnityEvent onShowCallback, onHideCallback;
    public RectTransform rectTransform
    {
        get
        {
            if (m_rectTransform == null)
            {
                m_rectTransform = GetComponent<RectTransform>();
            }
            return m_rectTransform;
        }
    }
    private RectTransform m_rectTransform;
    public Vector2 size
    {
        get
        {
            if(m_size == -Vector2.one)
            {
                m_size = new Vector2(rectTransform.rect.width, rectTransform.rect.height);
            }
            return m_size;
        }
    }
    private Vector2 m_size = -Vector2.one;
    public Image image
    {
        get
        {
            if(m_image == null)
            {
                Button button = GetComponent<Button>();
                if(button == null)
                    m_image = GetComponent<Image>();
            }
            return m_image;
        }
    }
    private Image m_image;
    public TextMeshProUGUI text
    {
        get
        {
            if(m_text == null)
            {
                m_text = GetComponent<TextMeshProUGUI>();
            }
            return m_text;
        }
    }
    private TextMeshProUGUI m_text;
    private float m_margin = 20.0f;

    protected virtual void Awake()
    {
        beginPos = rectTransform.anchoredPosition;
        if (onstart == EVisibleState.Hidden && !m_isHidden)
        {
            Hide(0.0f);
        }
    }

    private void OnTriggerChildrenChanged()
    {
        if (!triggerChildren)
            playOwnAnimation = true;
    }

    [Button]
    public void Show()
    {
        Show(animationTime);
    }

    public void Show(float _time)
    {
        Tween showTween = null;
        onShowCallback?.Invoke();
        switch (style)
        {
            case EAnimationStyle.Slide:
                switch (direction)
                {
                    case EAnimationDirection.None:
                        rectTransform.localScale = Vector2.one;
                        break;
                    case EAnimationDirection.Left:
                    case EAnimationDirection.Right:
                        showTween = rectTransform.DOAnchorPosX(beginPos.x, _time);
                        //rectTransform.DOAnchorPosX(beginPos.x, animationTime).SetEase(easeIn);
                        break;
                    case EAnimationDirection.Top:
                    case EAnimationDirection.Bottom:
                        showTween = rectTransform.DOAnchorPosY(beginPos.y, _time);
                        break;
                    default:
                        break;
                }
                break;
            case EAnimationStyle.Scale:
                showTween = rectTransform.DOScale(Vector2.one, _time);
                break;
            case EAnimationStyle.Appear:
                if (image != null)
                {
                    showTween = image.DOFade(1.0f, _time);
                    break;
                }
                if (text != null)
                {
                    showTween = text.DOFade(1.0f, _time);
                    break;
                }
                showTween = transform.DOScale(Vector3.one, _time);
                break;
            case EAnimationStyle.None:
            default:
                m_isHidden = false;
                OnShow();
                break;
        }

        if (playOwnAnimation && showTween != null)
        {
            showTween
                .SetUpdate(UpdateType.Normal, true)
                .SetEase(easeIn)
                .OnComplete(() =>
                {
                    m_isHidden = false;
                    OnShow();
                })
                .Play();
        }

        if (triggerChildren)
        {
            foreach (Transform t in transform)
            {
                UIElement child = t.GetComponent<UIElement>();
                if (child != null)
                    child.Show();
            }
        }
    }

    [Button]
    public void Hide()
    {
        Hide(animationTime);
    }

    public void Hide(float _time)
    {
        Tween hideTween = null;
        switch (style)
        {
            case EAnimationStyle.Slide:
                switch (direction)
                {
                    case EAnimationDirection.None:
                        rectTransform.localScale = Vector2.zero;
                        break;
                    case EAnimationDirection.Top:
                        hideTween = rectTransform.DOAnchorPosY(size.y + m_margin + extraOffsetWhenHiding, _time);
                        break;
                    case EAnimationDirection.Left:
                        hideTween = rectTransform.DOAnchorPosX(size.x + m_margin + extraOffsetWhenHiding, _time);
                        break;
                    case EAnimationDirection.Right:
                        hideTween = rectTransform.DOAnchorPosX(-size.x - m_margin - extraOffsetWhenHiding, _time);
                        break;
                    case EAnimationDirection.Bottom:
                        hideTween = rectTransform.DOAnchorPosY(-size.y - m_margin - extraOffsetWhenHiding, _time);
                        break;
                    default:
                        break;
                }
                break;
            case EAnimationStyle.Scale:
                Vector2 scale = (shrink) ? Vector2.zero : Vector2.one * scaleFactor;
                hideTween = rectTransform.DOScale(scale, _time);
                break;
            case EAnimationStyle.Appear:
                if (image != null)
                {
                    hideTween = image.DOFade(0.0f, _time);
                    break;
                }
                if (text != null)
                {
                    hideTween = text.DOFade(0.0f, _time);
                    break;
                }
                hideTween = transform.DOScale(Vector3.zero, _time);
                break;
            case EAnimationStyle.None:
            default:
                m_isHidden = true;
                OnHide();
                break;
        }

        if (playOwnAnimation && hideTween != null)
        {
            hideTween
                .SetUpdate(true)
                .SetEase(easeOut)
                .OnComplete(() =>
                {
                    onHideCallback?.Invoke();
                    m_isHidden = true;
                    OnHide();
                })
                .Play();
        }

        if (triggerChildren)
        {
            foreach (Transform t in transform)
            {
                UIElement child = t.GetComponent<UIElement>();
                if (child != null)
                    child.Hide();
            }
        }
    }

    public void Toggle()
    {
        if (isHidden)
            Show();
        else
            Hide();
    }

    protected virtual void OnHide() { }
    protected virtual void OnShow() { }
}
