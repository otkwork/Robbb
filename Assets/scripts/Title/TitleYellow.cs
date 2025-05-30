using UnityEngine;

public class TitleYellow : MonoBehaviour
{
	[SerializeField] GameObject[] backGround;
	[SerializeField] float moveSpeed;
	[SerializeField] float outPos;
	[SerializeField] int lastBackNum;
	[SerializeField] float lastBackPos;
	// Start is called before the first frame update
	void Start()
	{

	}

	// Update is called once per frame
	void FixedUpdate()
	{
		for (int i = 0; i < backGround.Length; i++)
		{
			backGround[i].transform.Translate(moveSpeed, 0, 0);

			if(outPos < 0)
			{
				if (backGround[i].transform.position.x <= outPos)
				{
					backGround[i].transform.position =
						backGround[i != 0 ? i - 1 : lastBackNum].transform.position + new Vector3(lastBackPos, 0, 0);
				}
			}
			else
			{
				if (backGround[i].transform.position.x >= outPos)
				{
					backGround[i].transform.position =
						backGround[i != lastBackNum ? i + 1 : 0].transform.position + new Vector3(lastBackPos, 0, 0);
				}
			}
		}
	}
}
