using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

// call SceneLoader.Instance.GoToScene("SceneTo") to load a scene with transition
// don't forget to add all scenes to build settings

public class SceneLoader : MonoBehaviour
{
	private const string TRANSITION_SCENE_NAME = "LoadingScreen"; // change to name of your transition scene

	private Scene _transitionScene;
	
    // singleton
	private static SceneLoader _instance;
	public static SceneLoader Instance{
		get{
			if (_instance == null)
			{
				GameObject obj = new("SceneLoader");
				_instance = obj.AddComponent<SceneLoader>();
				DontDestroyOnLoad(obj);
			}
		return _instance;
		}
	}

	private void Awake()
	{
		_transitionScene = SceneManager.GetSceneByName(TRANSITION_SCENE_NAME);
		if (_transitionScene == null) {
			Debug.LogError("Transition screen not found");
		}
	}

    public async void GoToScene(string newSceneName)
	{
		// show loading screen
		await ShowLoadingScreen();

		// unload old scene
		await SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());

		// load new scene
		await SceneManager.LoadSceneAsync(newSceneName, LoadSceneMode.Additive);
		
		// hide loading screen
		await HideLoadingScreen();

		// set new scene as active
		Scene newScene = SceneManager.GetSceneByName(newSceneName);
		if (newScene != null && newScene.isLoaded) {
			SceneManager.SetActiveScene(newScene);	
		}
	}

	public async Task ShowLoadingScreen()
	{
		await SceneManager.LoadSceneAsync(TRANSITION_SCENE_NAME, LoadSceneMode.Additive);

		SceneTransitioner sceneTransitioner = GetSceneTransitioner();
		if (sceneTransitioner) {
			await sceneTransitioner.ShowLoadingScreen();
		}
	}

	public async Task HideLoadingScreen()
	{
		SceneTransitioner sceneTransitioner = GetSceneTransitioner();
		if (sceneTransitioner) {
			await sceneTransitioner.HideLoadingScreen();
			// unloading transition scene is handled by SceneTransitioner
		}
	}

	private SceneTransitioner GetSceneTransitioner()
	{
		// todo: this could be improved, perhaps by a singleton or static class
		return FindAnyObjectByType<SceneTransitioner>();
	}
}

