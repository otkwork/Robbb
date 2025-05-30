using UnityEngine;

public class StageChange : MonoBehaviour
{
	[SerializeField] GameObject buttonParent;
	GameObject player;
    // Start is called before the first frame update
    void Start()
    {
		player = GameObject.FindWithTag("Player");
		buttonParent.SetActive(false);	
    }

    // Update is called once per frame
    void Update()
    {
		player.SetActive(!buttonParent.activeSelf);
        if(Input.GetKeyDown(KeyCode.F2))
		{
			buttonParent.SetActive(!buttonParent.activeSelf);
		}
    }
}
