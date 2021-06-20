using UnityEngine;
using System.Collections;

namespace Bolt.AdvancedTutorial
{
	public class LineFade : MonoBehaviour
	{
		[SerializeField] private Color color;

        [SerializeField] private float speed = 10f;

		LineRenderer lr;

		void Start ()
		{
			lr = GetComponent<LineRenderer> ();
		}

		void Update ()
		{
			// move towards zero
			color.a = Mathf.Lerp (color.a, 0, Time.deltaTime * speed);

			// update color
			//lr.SetColors (color, color);
			lr.startColor = color;
			lr.endColor = color;
		}
	}
}
