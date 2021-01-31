using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoverUi : MonoBehaviour
{
	public Image fadeImage;
	public AnimationCurve fadeCurve;


	public void FadeOn(float duration)
	{
		StartCoroutine(FadeOnCoroutine(duration));
	}

	public void FadeOff(float duration)
	{
		StartCoroutine(FadeOffCoroutine(duration));
	}

	private IEnumerator FadeOnCoroutine(float duration)
	{
		float time = Time.time;

		while (Time.time - time < duration)
		{
			var deltaTime = (Time.time - time) / duration;
			fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, fadeCurve.Evaluate(deltaTime));
			yield return null;
		}
	}

	private IEnumerator FadeOffCoroutine(float duration)
	{
		float time = Time.time;

		while (Time.time - time < duration)
		{
			var deltaTime = 1 - (Time.time - time) / duration;
			fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, fadeCurve.Evaluate(deltaTime));
			yield return null;
		}
	}
}