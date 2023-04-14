using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VariableManager : MonoBehaviour
{
	[Header("Time")]
	public float timeScale;
	public float predictionTimeScale;
	public int stepAmount;
	public float stepSpeed;

	[Header("Prediction")]
	public float _simulationDuration = 1;
	public Color _simulationColor;

	[Header("Indicator")]
	public float selectedScale;
	public float unSelectedScale;
	#region Sprite
	public Sprite holdSprite;
	public Sprite relaxSprite;
	public Sprite rotateSprite;
	public Sprite rotateTowartsSprite;
	#endregion
}
