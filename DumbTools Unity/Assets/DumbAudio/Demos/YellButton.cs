using UnityEngine;
using UnityEngine.EventSystems;

public class YellButton : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private AudioClip yellClip;

    public void OnPointerClick(PointerEventData eventData)
    {
        DumbAudio.Instance.PlaySound(yellClip);

        // TODO: add animation
    }
}