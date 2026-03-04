using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

// plays a clip on loop in a specific area.
// useful if a scene has many ambient audio sources and you want volume falloff while preserving stereo.

public class AmbientAudioArea : MonoBehaviour
{
	public AudioClip audioClip;
	[Tooltip("Window > Audio > Audio Mixer")]
	public AudioMixerGroup mixerGroup;
	public AudioAreaShape shape = AudioAreaShape.Sphere; // TODO: implement box shape in update loop and serialize me
	[Tooltip("The distance where ambience is loudest.")]
	public float closeDistance = 15;
	[Tooltip("The farthest distance where ambience is still audible.")]
	public float farDistance = 30;
	[Tooltip("Ambience fades in over this time when the scene starts.")]
	public float fadeInAtStart = 2;
	[Tooltip("The volume when the camera is within the close distance.")]
	[Range(0, 1)] public float loudestVolume = 1;
	[Tooltip("How much to blend between 2D and 3D audio. Ambience will seem like it's coming from the position of the audio source if higher.")]
	[Range(0, 1)] public float directionality = 0.75f;
	[Range(0, 2)] public float pitch = 1;
	[Range(0, 1)] public float reverbZoneMix = 0;
	public GizmoColor gizmoColor = GizmoColor.Blue;
	[Tooltip("Set to false to hide gizmos when they're not selected.")]
	public bool alwaysShowGizmos = true;

	private AudioSource audioSource;

	public enum AudioAreaShape {
		Sphere,
		Box
	}

	public enum GizmoColor {
		Red,
		Green,
		Blue
	}

	private void Start()
	{
		audioSource = GetComponent<AudioSource>();
		if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();
		audioSource.clip = audioClip;
		audioSource.outputAudioMixerGroup = mixerGroup;
		audioSource.priority = 200;
		audioSource.volume = 0;
		audioSource.pitch = pitch;
		audioSource.spatialBlend = 0;
		audioSource.loop = true;
		audioSource.Play();
		StartCoroutine(FadeInRoutine(fadeInAtStart));
	}

	private IEnumerator FadeInRoutine(float time)
	{
		float t = 0;
		float targetVolume = loudestVolume;
		while (t < time) {
			loudestVolume = Mathf.Lerp(0, targetVolume, t / time);
			t += Time.deltaTime;
			yield return null;
		}
		loudestVolume = targetVolume;
	}

	private void Update()
	{
		var camera = Camera.main ?? FindAnyObjectByType<Camera>();
        if (camera == null) return;
		
		float distanceNormalized = 0;
		switch (shape) {
			case AudioAreaShape.Sphere:
				float cameraDistance = Vector3.Distance(transform.position, camera.transform.position);
				distanceNormalized = 1 - (cameraDistance - closeDistance) / (farDistance - closeDistance);
				break;

			case AudioAreaShape.Box:
				float cameraDistanceX = Mathf.Abs(camera.transform.position.x - transform.position.x);
				float cameraDistanceY = Mathf.Abs(camera.transform.position.y - transform.position.y);
				float cameraDistanceZ = Mathf.Abs(camera.transform.position.z - transform.position.z);
				float furthestDistance = Mathf.Max(
					cameraDistanceX * transform.localScale.x,
					cameraDistanceY * transform.localScale.y,
					cameraDistanceZ * transform.localScale.z
				);
				furthestDistance *= 2; // because the box is 2x2x2 the size of the box
				distanceNormalized = 1 - (furthestDistance - closeDistance) / (farDistance - closeDistance);
				break;
		}

		if (distanceNormalized <= 0) {
			audioSource.volume = 0;
			return;
		}

		audioSource.spatialBlend = Mathf.Lerp(directionality, 0, distanceNormalized);
		audioSource.volume = Mathf.Lerp(0, loudestVolume, distanceNormalized);
		audioSource.pitch = pitch;
		audioSource.reverbZoneMix = reverbZoneMix;
	}

    private void OnValidate()
    {
		if (closeDistance < 0) closeDistance = 0;
		if (farDistance < 0) farDistance = 0;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
	{
		DrawGizmos();
	}

	private void OnDrawGizmos()
	{
		if (alwaysShowGizmos) DrawGizmos();
	}

    private void DrawGizmos()
    {
		float alpha = 0.1f;
		switch (gizmoColor) {
			case GizmoColor.Red:
				Gizmos.color = new(1, 0.5f, 0, alpha);
				break;
			case GizmoColor.Green:
				Gizmos.color = new(0.5f, 1, 0.5f, alpha);
				break;
			case GizmoColor.Blue:
				Gizmos.color = new(0, 0.5f, 1, alpha);
				break;
		}
		switch (shape) {
			case AudioAreaShape.Sphere:
				Gizmos.DrawSphere(transform.position, closeDistance);
				break;
			case AudioAreaShape.Box:
				Gizmos.DrawCube(transform.position, new(
					closeDistance * transform.localScale.x, 
					closeDistance * transform.localScale.y,
					closeDistance * transform.localScale.z)
				);
				break;
		}
		switch (gizmoColor) {
			case GizmoColor.Red:
				Gizmos.color = new(1, 0.2f, 0, alpha);
				break;
			case GizmoColor.Green:
				Gizmos.color = new(0, 1, 0, alpha);
				break;
			case GizmoColor.Blue:
				Gizmos.color = new(0, 0.2f, 1, alpha);
				break;	
		}
		switch (shape) {
			case AudioAreaShape.Sphere:
				Gizmos.DrawSphere(transform.position, farDistance);
				break;
			case AudioAreaShape.Box:
				Gizmos.DrawCube(transform.position, new(
					farDistance * transform.localScale.x, 
					farDistance * transform.localScale.y,
					farDistance * transform.localScale.z)
				);
				break;
		}
    }
#endif
}
