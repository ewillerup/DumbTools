using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class SceneTransitioner : MonoBehaviour
{
    [SerializeField] private float transitionDuration = 1f;
    [SerializeField] private RectTransform rect;

    public async Task ShowLoadingScreen()
    {
        AnimateIn();

        // return after transition duration
        int delayMilliseconds = (int)(transitionDuration * 1000);
        await Task.Delay(delayMilliseconds);
    }

    public async Task HideLoadingScreen()
    {
        AnimateOut();

        int delayMilliseconds = (int)(transitionDuration * 1000);
        await Task.Delay(delayMilliseconds);

        await SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene()); // unload my own scene
    }

    private void AnimateIn()
    {
        int distance = Screen.height;
        rect.anchoredPosition = new Vector2(0, -distance);
        rect.DOAnchorPosY(0, transitionDuration).SetEase(Ease.InOutSine);
    }

    private void AnimateOut()
    {
        int distance = Screen.height;
        rect.DOAnchorPosY(-distance, transitionDuration).SetEase(Ease.InOutSine);
    }
}