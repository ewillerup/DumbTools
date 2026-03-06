using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

// call SceneLoader.Instance.GoToScene("SceneTo") to load a scene with transition
// don't forget to add all scenes to build settings

public class DumbSceneLoader : MonoBehaviour
{
	private const string TRANSITION_SCENE_NAME = "LoadingScreen"; // change to name of your transition scene

	private Scene _transitionScene;
	
    // singleton
	private static DumbSceneLoader _instance;
	public static DumbSceneLoader Instance{
		get{
			if (_instance == null)
			{
				GameObject obj = new("Dumb Scene Loader");
				_instance = obj.AddComponent<DumbSceneLoader>();
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

		DumbLoadingScreen sceneTransitioner = GetLoadingScene();
		if (sceneTransitioner) {
			await sceneTransitioner.ShowLoadingScreen();
		}
	}

	public async Task HideLoadingScreen()
	{
		DumbLoadingScreen sceneTransitioner = GetLoadingScene();
		if (sceneTransitioner) {
			await sceneTransitioner.HideLoadingScreen();
			// unloading transition scene is handled by SceneTransitioner
		}
	}

	private DumbLoadingScreen GetLoadingScene()
	{
		// todo: this could be improved, perhaps by a singleton or static class
		return FindAnyObjectByType<DumbLoadingScreen>();
	}
}

