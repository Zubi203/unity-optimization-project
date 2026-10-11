using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

public class AnimatedButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{

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

    private Vector2 baseScale = new Vector2(1.0f, 1.0f);
    private float baseRotationDegrees = 0.0f;
    [SerializeField] private ScaleFrom pivotLocation = ScaleFrom.CenterMiddle;
    [SerializeField] private float hoverScaleIncrease = 0.15f;
    [SerializeField] private float pressedScaleIncrease = 0.2f;
    [SerializeField] private bool scaleX = true;
    [SerializeField] private bool scaleY = true;
    [SerializeField] private bool scaleWithWidth = true;
    [SerializeField] private bool rotateOnHover = true;
    [SerializeField] private float hoverRotationAmount = 1.0f;
    [SerializeField] private float widthFullRotation = 300.0f;
    [SerializeField] private float animationSpeedScale = 1.0f;
    [SerializeField] private Transform targetTransform = null;
    Sequence sequence;

    void Awake()
    {
        baseScale = Vector2.one;
        baseRotationDegrees = transform.rotation.z;
        
    }

    void Start()
    {
        if (targetTransform == null){
            targetTransform = transform;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        SetPivot();

        float scaleRatio = Mathf.Clamp(widthFullRotation / targetTransform.GetComponent<RectTransform>().rect.width, 0.5f, 1.0f);
        if (!scaleWithWidth)
        {
            scaleRatio = 1.0f;
        }
        Vector2 scaleTarget = baseScale + new Vector2(hoverScaleIncrease * scaleRatio, hoverScaleIncrease * scaleRatio);

        if (sequence != null && sequence.IsActive())
        {
            sequence.Kill();
        }
        sequence = DOTween.Sequence();
        if (scaleX)
        {
        sequence.Append(targetTransform.DOScaleX(scaleTarget.x, animationSpeedScale * 0.2f).SetEase(Ease.OutElastic));
        }

        if (scaleY)
        {
        sequence.Join(targetTransform.DOScaleY(scaleTarget.y, animationSpeedScale * 0.35f).SetEase(Ease.OutElastic));
        }

        if (rotateOnHover) {
            float[] randomRotation = {-hoverRotationAmount, hoverRotationAmount};
            float rot = randomRotation[Random.Range(0, randomRotation.Length)];
            sequence.Join(targetTransform.DORotate(new Vector3(0.0f, 0.0f, 2.0f * scaleRatio * (baseRotationDegrees + rot)), animationSpeedScale * 0.1f).SetEase(Ease.OutBack));
            sequence.Join(targetTransform.DORotate(new Vector3(0.0f, 0.0f, baseRotationDegrees), animationSpeedScale * 0.1f).SetEase(Ease.Linear).SetDelay(0.1f * animationSpeedScale));
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        SetPivot();

        if (sequence != null && sequence.IsActive())
        {
            sequence.Kill();
        }
        sequence = DOTween.Sequence();
        sequence.Append(targetTransform.DOScaleX(baseScale.x, animationSpeedScale * 0.2f).SetEase(Ease.OutExpo));
        sequence.Join(targetTransform.DOScaleY(baseScale.y, animationSpeedScale * 0.2f).SetEase(Ease.OutExpo));
        sequence.Join(targetTransform.DORotate(new Vector3(0.0f, 0.0f, baseRotationDegrees), animationSpeedScale * 0.1f).SetEase(Ease.OutExpo));
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        SetPivot();

        float scaleRatio = Mathf.Clamp(widthFullRotation / targetTransform.GetComponent<RectTransform>().rect.width, 0.5f, 1.0f);
        if (!scaleWithWidth)
        {
            scaleRatio = 1.0f;
        }
        Vector2 scaleTarget = baseScale + new Vector2(pressedScaleIncrease * scaleRatio, pressedScaleIncrease * scaleRatio);

        if (sequence != null && sequence.IsActive())
        {
            sequence.Kill();
        }
        sequence = DOTween.Sequence();

        if (scaleX)
        {
        sequence.Append(targetTransform.DOScaleX(scaleTarget.x, animationSpeedScale * 0.1f).SetEase(Ease.OutBack));
        }

        if (scaleY)
        {
        sequence.Join(targetTransform.DOScaleY(scaleTarget.y, animationSpeedScale * 0.1f).SetEase(Ease.OutBack));
        }

        sequence.Append(targetTransform.DOScaleX(baseScale.x, animationSpeedScale * 0.2f).SetEase(Ease.OutExpo));
        sequence.Join(targetTransform.DOScaleY(baseScale.y, animationSpeedScale * 0.2f).SetEase(Ease.OutExpo));
    }

    private void SetPivot()
    {
        RectTransform rect_transform = targetTransform.GetComponent<RectTransform>();

        if (rect_transform == null) {return;}

        switch (pivotLocation)
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

    private void ChangePivotKeepPosition(RectTransform rectTransform, Vector2 newPivot)
    {
        if (rectTransform == null) {return;}

        Vector2 pivotDifference = newPivot - rectTransform.pivot;
        Vector3 positionOffset = new Vector3(pivotDifference.x * rectTransform.rect.width, pivotDifference.y * rectTransform.rect.height, 0f);
        Vector3 globalOffset = rectTransform.TransformVector(positionOffset);
        rectTransform.pivot = newPivot;
        rectTransform.position += globalOffset;

    }
}
