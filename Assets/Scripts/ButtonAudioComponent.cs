using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonAudioComponent : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler
{
    [SerializeField] public AudioSource audioSource;
    [SerializeField] public AudioClip hoverSound;
    [SerializeField] public AudioClip pressSound;

    public void OnPointerEnter(PointerEventData eventData){

        if (audioSource == null || hoverSound == null) {return;}

        audioSource.PlayOneShot(hoverSound);
    }

    public void OnPointerClick(PointerEventData eventData){

        if (audioSource == null || pressSound == null) {return;}

        audioSource.PlayOneShot(pressSound);

    }

}
