using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class ContainerAnimation : MonoBehaviour
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

    enum AppearOrder {
        StartTop,
        StartBottom
    }

    [SerializeField] private AnimateWhen animWhen = AnimateWhen.Start;
    [SerializeField] private ScaleFrom scaleFrom = ScaleFrom.CenterMiddle;
    [SerializeField] private AnimationType animType = AnimationType.Scale;
    [SerializeField] private AppearOrder order = AppearOrder.StartTop;
    [SerializeField] private float slideInDistance = 100.0f;
    [SerializeField] private float duration = 0.2f;
    [SerializeField] private float delayAppear = 0.2f;
    [SerializeField] private float fadeInDuration = 0.01f;
    [SerializeField] private float delayBetweenElements = 0.03f;
    private Sequence sequence;

    
    void Awake()
    {
        PrimeForAnimation();

        if (animWhen == AnimateWhen.Start)
        {
            Appear();
        }
        
    }

    private void PrimeForAnimation()
    {
        switch (animType)
        {
            case AnimationType.Scale:
                foreach(Transform child in transform)
                {
                    child.localScale = new Vector3(0.0f, 0.0f, 1f);
                    SetPivot(child, scaleFrom);
                    CanvasGroup childCanvasGroup = child.GetComponent<CanvasGroup>();
                    if (childCanvasGroup != null)
                    {
                        childCanvasGroup.alpha = 0.0f;
                    }
                }
                break;
            case AnimationType.SlideInLeft:
                foreach(Transform child in transform)
                {
                    CanvasGroup childCanvasGroup = child.GetComponent<CanvasGroup>();
                    if (childCanvasGroup != null)
                    {
                        childCanvasGroup.alpha = 0.0f;
                    }
                }
                break;
            case AnimationType.SlideInRight:
                foreach(Transform child in transform)
                {
                    CanvasGroup childCanvasGroup = child.GetComponent<CanvasGroup>();
                    if (childCanvasGroup != null)
                    {
                        childCanvasGroup.alpha = 0.0f;
                    }
                }
                break;
            case AnimationType.SlideInTop:
                foreach(Transform child in transform)
                {
                    CanvasGroup childCanvasGroup = child.GetComponent<CanvasGroup>();
                    if (childCanvasGroup != null)
                    {
                        childCanvasGroup.alpha = 0.0f;
                    }
                }
                break;
            case AnimationType.SlideInBottom:
                foreach(Transform child in transform)
                {
                    CanvasGroup childCanvasGroup = child.GetComponent<CanvasGroup>();
                    if (childCanvasGroup != null)
                    {
                        childCanvasGroup.alpha = 0.0f;
                    }
                }
                break;
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

        switch (order)
        {
            case AppearOrder.StartTop:

                for (int i = 0; i < transform.childCount; i++)
                {
                    Transform childTransform = transform.GetChild(i);
                    CanvasGroup canvasGroup = childTransform.GetComponent<CanvasGroup>();
                    switch (animType)
                    {
                        case AnimationType.Scale:
                            sequence.Join(childTransform.DOScaleX(1f, duration).SetEase(Ease.OutBack).SetDelay(delayBetweenElements));
                            sequence.Join(childTransform.DOScaleY(1f, duration).SetEase(Ease.OutBack).SetDelay(delayBetweenElements));
                            break;
                        case AnimationType.SlideInLeft:
                            sequence.Join(childTransform.DOLocalMoveX(childTransform.localPosition.x, duration).From(childTransform.localPosition.x - slideInDistance).SetEase(Ease.OutBack).SetDelay(delayBetweenElements));
                            break;
                        case AnimationType.SlideInRight:
                            sequence.Join(childTransform.DOLocalMoveX(childTransform.localPosition.x, duration).From(childTransform.localPosition.x + slideInDistance).SetEase(Ease.OutBack).SetDelay(delayBetweenElements));
                            break;
                        case AnimationType.SlideInTop:
                            sequence.Join(childTransform.DOLocalMoveY(childTransform.localPosition.y, duration).From(childTransform.localPosition.y + slideInDistance).SetEase(Ease.OutBack).SetDelay(delayBetweenElements));
                            break;
                        case AnimationType.SlideInBottom:
                            sequence.Join(childTransform.DOLocalMoveY(childTransform.localPosition.y, duration).From(childTransform.localPosition.y - slideInDistance).SetEase(Ease.OutBack).SetDelay(delayBetweenElements));
                            break;
                    }
                    sequence.Join(canvasGroup.DOFade(1.0f, fadeInDuration).SetDelay(delayBetweenElements));
                }
                break;

            case AppearOrder.StartBottom:

                for (int i = transform.childCount - 1; i >= 0; i--)
                {
                    Transform childTransform = transform.GetChild(i);
                    CanvasGroup canvasGroup = childTransform.GetComponent<CanvasGroup>();
                    switch (animType)
                    {
                        case AnimationType.Scale:
                            sequence.Join(childTransform.DOScaleX(1f, duration).SetEase(Ease.OutBack).SetDelay(delayBetweenElements));
                            sequence.Join(childTransform.DOScaleY(1f, duration).SetEase(Ease.OutBack).SetDelay(delayBetweenElements));
                            break;
                        case AnimationType.SlideInLeft:
                            sequence.Join(childTransform.DOLocalMoveX(childTransform.localPosition.x, duration).From(childTransform.localPosition.x - slideInDistance).SetEase(Ease.OutBack).SetDelay(delayBetweenElements));
                            break;
                        case AnimationType.SlideInRight:
                            sequence.Join(childTransform.DOLocalMoveX(childTransform.localPosition.x, duration).From(childTransform.localPosition.x + slideInDistance).SetEase(Ease.OutBack).SetDelay(delayBetweenElements));
                            break;
                        case AnimationType.SlideInTop:
                            sequence.Join(childTransform.DOLocalMoveY(childTransform.localPosition.y, duration).From(childTransform.localPosition.y + slideInDistance).SetEase(Ease.OutBack).SetDelay(delayBetweenElements));
                            break;
                        case AnimationType.SlideInBottom:
                            sequence.Join(childTransform.DOLocalMoveY(childTransform.localPosition.y, duration).From(childTransform.localPosition.y - slideInDistance).SetEase(Ease.OutBack).SetDelay(delayBetweenElements));
                            break;
                    }
                    sequence.Join(canvasGroup.DOFade(1.0f, fadeInDuration).SetDelay(delayBetweenElements));
                }
                break;

        }
    }

    private void SetPivot(Transform targetObject, ScaleFrom pivotTarget)
    {
        RectTransform rect_transform = targetObject.GetComponent<RectTransform>();

        if (rect_transform == null) {return;}

        switch (pivotTarget)
        {
            case ScaleFrom.TopLeft:
                ChangePivotKeepPosition(rect_transform, new Vector2(0.0f, 1.0f));
                break;
            case ScaleFrom.TopMiddle:
                ChangePivotKeepPosition(rect_transform, new Vector2(0.5f, 1.0f));
                break;
            case ScaleFrom.TopRight:
                ChangePivotKeepPosition(rect_transform, new Vector2(1.0f, 1.0f));
                break;
            case ScaleFrom.CenterLeft:
                ChangePivotKeepPosition(rect_transform, new Vector2(0.0f, 0.5f));
                break;
            case ScaleFrom.CenterMiddle:
                ChangePivotKeepPosition(rect_transform, new Vector2(0.5f, 0.5f));
                break;
            case ScaleFrom.CenterRight:
                ChangePivotKeepPosition(rect_transform, new Vector2(1.0f, 0.5f));
                break;
            case ScaleFrom.BottomLeft:
                ChangePivotKeepPosition(rect_transform, new Vector2(0.0f, 0.0f));
                break;
            case ScaleFrom.BottomMiddle:
                ChangePivotKeepPosition(rect_transform, new Vector2(0.5f, 0.0f));
                break;
            case ScaleFrom.BottomRight:
                ChangePivotKeepPosition(rect_transform, new Vector2(1.0f, 0.0f));
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

    private void ChangePivotKeepPosition(RectTransform rectTransform, Vector2 newPivot)
    {
        if (rectTransform == null) {return;}

        Vector2 pivotDifference = newPivot - rectTransform.pivot;
        Vector3 positionOffset = new Vector3(pivotDifference.x * rectTransform.rect.width, pivotDifference.y * rectTransform.rect.height, 0f);
        Vector3 globalOffset = rectTransform.TransformVector(positionOffset);
        rectTransform.pivot = newPivot;
        rectTransform.position += globalOffset;

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
    }
}
