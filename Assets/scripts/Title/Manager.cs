using UnityEngine;

public class Manager : MonoBehaviour
{
	[SerializeField] AudioSource startSound;
	[SerializeField] AudioSource bgm;
    // Start is called before the first frame update
    void Start()
    {
        bgm.Play();
    }

    // Update is called once per frame
    void Update()
    {
		if(Input.GetKeyDown(KeyCode.Return))
		{
			startSound.Play();
		}
    }
}
