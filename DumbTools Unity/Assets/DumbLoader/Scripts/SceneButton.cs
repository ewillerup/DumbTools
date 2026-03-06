using UnityEngine;
using UnityEngine.EventSystems;
using Unity.VisualScripting;

public class SceneButton : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private string sceneName = "End";
    [SerializeField] private AudioClip speechClip;

    public void OnPointerClick(PointerEventData eventData)
    {
        DumbSceneLoader.Instance.GoToScene(sceneName); // replace with the target scene name

        // punch
        Rigidbody rigidbody = transform.AddComponent<Rigidbody>();
        rigidbody.AddForce(Vector3.forward * 50, ForceMode.Impulse);
        rigidbody.AddTorque(new(50, 30, 0), ForceMode.Impulse);
    }
}
