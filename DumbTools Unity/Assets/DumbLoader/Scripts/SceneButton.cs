using UnityEngine;
using UnityEngine.EventSystems;
using Unity.VisualScripting;
using System;

public class SceneButton : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private string sceneName = "End";
    [SerializeField] private AudioClip speechClip;

    public void OnPointerClick(PointerEventData eventData)
    {
        SceneLoader.Instance.GoToScene(sceneName); // replace with the target scene name

        // punch
        Rigidbody rigidbody = transform.AddComponent<Rigidbody>();
        rigidbody.AddForce(Vector3.forward * 50, ForceMode.Impulse);
        rigidbody.AddTorque(new(50, 30, 0), ForceMode.Impulse);
    }


    public void PlaySpeech(Action onFinished)
    {
        DumbAudio.Instance.PlaySound(speechClip);

        Invoke(nameof(callback), speechClip.length);
        void callback() {
            onFinished?.Invoke();
        }
    }

    public void Start()
    {
        PlaySpeech(() => {
            SceneLoader.Instance.GoToScene(sceneName); // replace with the target scene name
        });
    }
}
