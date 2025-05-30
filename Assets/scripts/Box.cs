using UnityEngine;

public class Box : MonoBehaviour
{
	public enum BoxColor
	{
		Blue,
		Purple,
		Orange,
		None,
	}
	[SerializeField] BoxColor m_color;

	public BoxColor GetColor()
	{
		return m_color;
	}
}
