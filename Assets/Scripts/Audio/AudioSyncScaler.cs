using UnityEngine;
using System.Collections;

public class AudioSyncScaler : MonoBehaviour
{
	[SerializeField]
	private int frequencyBand;
	[SerializeField]
	private Vector3 startScale;
	[SerializeField]
	private Vector3 maxScale;

	private void Update()
	{
		transform.localScale = maxScale * AudioSpectrumManager.Instance.FrequencyBands[frequencyBand] + startScale;
	}
}
