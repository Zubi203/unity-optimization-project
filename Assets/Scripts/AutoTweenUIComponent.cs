using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class AutoTweenUIComponent : MonoBehaviour
{
    enum AnimateWhen {
        Start,
        Visible,
        Manual
    }

    enum ScaleFrom {
        TopRight,
        TopMiddle,
        TopLeft,
        CenterRight,
        CenterMiddle,
        CenterLeft,
        BottomRight,
        BottomMiddle,
        BottomLeft
    }

    enum AnimationType {
        Scale,
        SlideInRight,
        SlideInLeft,
        SlideInTop,
        SlideInBottom
    }

    [SerializeField] private AnimateWhen animWhen = AnimateWhen.Start;
    [SerializeField] private ScaleFrom pivotLocation = ScaleFrom.CenterMiddle;
    [SerializeField] private AnimationType animType = AnimationType.Scale;
    [SerializeField] private float slideInDistance = 200.0f;
    [SerializeField] private float duration = 0.2f;
    [SerializeField] private float delayAppear = 0.2f;
    [SerializeField] private float fadeInDuration = 0.01f;
    private Sequence sequence;
    CanvasGroup canvasGroup;

    void Awake()
    {
        canvasGroup = transform.GetComponent<CanvasGroup>();
        PrimeForAnimation();

        if (animWhen == AnimateWhen.Start)
        {
            Appear();
        }
        
    }

    public void Appear()
    {
        if (sequence != null && sequence.IsActive())
        {
            sequence.Kill();
        }
        sequence = DOTween.Sequence();

        if (delayAppear > 0.0)
        {
            sequence.AppendInterval(delayAppear);
        }
        sequence.AppendInterval(0.01f);

        switch (animType)
        {
            case AnimationType.Scale:
                sequence.Join(transform.DOScaleX(1f, duration).SetEase(Ease.OutBack));
                sequence.Join(transform.DOScaleY(1f, duration).SetEase(Ease.OutBack));
                break;
            case AnimationType.SlideInLeft:
                sequence.Join(transform.DOLocalMoveX(transform.localPosition.x, duration).From(transform.localPosition.x - slideInDistance).SetEase(Ease.OutBack));
                break;
            case AnimationType.SlideInRight:
                sequence.Join(transform.DOLocalMoveX(transform.localPosition.x, duration).From(transform.localPosition.x + slideInDistance).SetEase(Ease.OutBack));
                break;
            case AnimationType.SlideInTop:
                sequence.Join(transform.DOLocalMoveY(transform.localPosition.y, duration).From(transform.localPosition.y + slideInDistance).SetEase(Ease.OutBack));
                break;
            case AnimationType.SlideInBottom:
                sequence.Join(transform.DOLocalMoveY(transform.localPosition.y, duration).From(transform.localPosition.y - slideInDistance).SetEase(Ease.OutBack));
                break;
        }
        if (canvasGroup != null) 
        {
            sequence.Join(canvasGroup.DOFade(1.0f, fadeInDuration));
        }
        
    }

    private void PrimeForAnimation()
    {
        switch (animType)
        {
            case AnimationType.Scale:

                transform.localScale = new Vector3(0.0f, 0.0f, 1f);
                SetPivot();
                if (canvasGroup != null)
                {
                    canvasGroup.alpha = 0.0f;
                }
                break;
            case AnimationType.SlideInLeft:
                if (canvasGroup != null)
                {
                    canvasGroup.alpha = 0.0f;
                }
                break;
            case AnimationType.SlideInRight:
                if (canvasGroup != null)
                {
                    canvasGroup.alpha = 0.0f;
                }
                break;
            case AnimationType.SlideInTop:
                if (canvasGroup != null)
                {
                    canvasGroup.alpha = 0.0f;
                }
                break;
            case AnimationType.SlideInBottom:
                if (canvasGroup != null)
                {
                    canvasGroup.alpha = 0.0f;
                }
                break;
        }
    }

    private void SetPivot()
    {
        RectTransform rect_transform = transform.GetComponent<RectTransform>();

        if (rect_transform == null) {return;}

        switch (pivotLocation)
        {
            case ScaleFrom.TopLeft:
                rect_transform.pivot = new Vector2(0.0f, 1.0f);
                break;
            case ScaleFrom.TopMiddle:
                rect_transform.pivot = new Vector2(0.5f, 1.0f);
                break;
            case ScaleFrom.TopRight:
                rect_transform.pivot = new Vector2(1.0f, 1.0f);
                break;
            case ScaleFrom.CenterLeft:
                rect_transform.pivot = new Vector2(0.0f, 0.5f);
                break;
            case ScaleFrom.CenterMiddle:
                rect_transform.pivot = new Vector2(0.5f, 0.5f);
                break;
            case ScaleFrom.CenterRight:
                rect_transform.pivot = new Vector2(1.0f, 0.5f);
                break;
            case ScaleFrom.BottomLeft:
                rect_transform.pivot = new Vector2(0.0f, 0.0f);
                break;
            case ScaleFrom.BottomMiddle:
                rect_transform.pivot = new Vector2(0.5f, 0.0f);
                break;
            case ScaleFrom.BottomRight:
                rect_transform.pivot = new Vector2(1.0f, 0.0f);
                break;
        }
    }

    void OnEnable()
    {
        if (animWhen == AnimateWhen.Visible)
        {
            PrimeForAnimation();
            Appear();
        }
    }

    void OnDestroy()
    {
        sequence?.Kill();
    }

    void DisableContainers()
    {
        VerticalLayoutGroup vBox = transform.GetComponent<VerticalLayoutGroup>();
        HorizontalLayoutGroup hBox = transform.GetComponent<HorizontalLayoutGroup>();
        GridLayoutGroup gridBox = transform.GetComponent<GridLayoutGroup>();
        if (vBox != null) {vBox.enabled = false;}
        if (hBox != null) {hBox.enabled = false;}
        if (gridBox != null) {gridBox.enabled = false;}
        print("kasjdhkajh");
    }
}

