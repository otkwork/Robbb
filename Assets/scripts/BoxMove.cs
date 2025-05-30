using UnityEngine;

public class BoxMove : MonoBehaviour
{
	GameObject player;
	private bool onPlayer;
	private bool isMove;
	private Vector3 target;
	Vector2[] addVector = {new Vector2(0, 2), new Vector2(2, 0), new Vector2(0, -2), new Vector2(-2, 0) };
	enum Duration
	{
		Up,
		Right,
		Down,
		Left,
	}
	Duration m_duration;
	Board m_board;
    // Start is called before the first frame update
    void Start()
    {
		onPlayer = false;
		isMove = false;
		player = GameObject.FindWithTag("Player");
		m_board = GameObject.FindWithTag("Board").GetComponent<Board>();
		transform.position = new Vector3(Mathf.Round(transform.position.x), 0.5f, Mathf.Round(transform.position.z));
	}

    // Update is called once per frame
    void Update()
    {
		if (!player.activeSelf) return;
		if(onPlayer)
		{
			InputSpace();
		}
		if(isMove)
		{
			transform.position = Vector3.MoveTowards(transform.position, target, 10 * Time.deltaTime);
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if(other.CompareTag("Player"))
		{
			onPlayer = true;
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			onPlayer = false;
		}
	}

	private void InputSpace()
	{
		if (Input.GetKeyDown(KeyCode.Space))
		{
			int angle = (int)player.transform.localEulerAngles.y;
			if (angle >= 315 && 360 >= angle || angle >= 0 && 45 > angle)   // è„
			{
				angle = 0;
				m_duration = Duration.Up;
			}
			else if (angle >= 45 && 135 > angle)    // âE
			{
				angle = 90;
				m_duration = Duration.Right;
			}
			else if (angle >= 135 && 225 > angle)   // â∫
			{
				angle = 180;
				m_duration = Duration.Down;
			}
			else if (angle >= 225 && 315 > angle)   // ç∂
			{
				angle = 270;
				m_duration = Duration.Left;
			}
			if(!m_board.InBoard(transform.position.x, transform.position.z, (int)m_duration, gameObject)) return;
			isMove = true;
			MoveBoxPos();
		}
	}

	private void MoveBoxPos()
	{
		Vector2 boxPos = m_board.ScreenToBoard(
				transform.position.x + addVector[(int)m_duration].x,
				transform.position.z + addVector[(int)m_duration].y);
		target = m_board.BoardToScreen((int)boxPos.x, (int)boxPos.y);
	}

}
