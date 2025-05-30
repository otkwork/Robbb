using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class Board : MonoBehaviour
{
	private GameObject[,] m_board;
	[SerializeField] GameObject boxParent;  // ���̐e�I�u�W�F�N�g
	[SerializeField] int Column = 9;    // ��
	[SerializeField] int Row = 9;       // �c
	List<GameObject> m_box = new List<GameObject>();
	const int EraseCount = 3;   // �����Ƃ��̍Œᐔ
	private bool gameClear = false;
	private int boxAmount;
	private float delta;

	[SerializeField] int ObstacleCount;
	[SerializeField] string nextStageName;

	private Vector2[] m_duration = {new Vector2(0,1), new Vector2(1,0), new Vector2(0,-1), new Vector2(-1,0) };
    // Start is called before the first frame update
    void Start()
    {
		m_board = new GameObject[Column, Row];
		boxAmount = boxParent.transform.childCount;	// �ז����܂߂����̐�
		for(int i = 0;i < boxAmount;++i)
		{
			m_box.Add(boxParent.transform.GetChild(i).gameObject);
		}

        for(int z = 0;z < Row;++z)	// �c
		{
			for(int x = 0; x < Column; ++x)	// ��
			{
				m_board[x, z] = null;
			}
		}

		foreach (GameObject box in m_box)
		{
			// -2, 2
			// �|�W�V���� 0�`�ɂ��������ł�m_board�ɓ����
			m_board[(int)(Mathf.Round(box.transform.position.x) / 2 + (Column - 1) / 2),
					(int)(Mathf.Round(box.transform.position.z) / 2 + (Column - 1) / 2)] = box;
		}
		delta = 0;
    }

    // Update is called once per frame
    void Update()
	{
		if (Input.GetKeyDown(KeyCode.F3))
		{
			SceneManager.LoadScene(SceneManager.GetActiveScene().name);
		}

		if(gameClear)	// �N���A
		{
			delta += Time.deltaTime;
			if (delta < 1.0f) return;

			SceneManager.LoadScene(nextStageName);
		}
	}

	public Vector3 BoardToScreen(int x, int y)
	{
		return new Vector3(2 * x - (Column - 1), 0.5f, 2 * y - (Row - 1));
	}

	public Vector2 ScreenToBoard(float x, float y)
	{
		int rX = -1;
		int rY = -1;
		for(int i = 0; i <= Column; ++i) 
		{
			if(x <= (-Column + 2) + 2 * i)
			{
				rX = i; 
				break;
			}
		}
		for (int i = 0; i <= Row; ++i)
		{
			if (y <= (-Row + 2) + 2 * i)
			{
				rY = i;
				break;
			}
		}
		return new Vector2(rX, rY);
	}

	public bool InBoard(float posX, float posY, int duration, GameObject box)
	{
		Vector2 thisPos = ScreenToBoard(posX, posY);
		Vector2 nextPos = ScreenToBoard(posX, posY) + m_duration[duration]; // ���̃|�W�V�����ɓ��������Ƃ��Ɉړ��������
		// �g�����ǂ���
		if (nextPos.x >= Column) return false;
		if (nextPos.y >= Row) return false;
		if (nextPos.x < 0) return false;
		if (nextPos.y < 0) return false;
		
        // �󂢂Ă��邩�ǂ���
        if (!IsEmpty(nextPos)) return false;
		// board�ɓ����
		FixPlay(thisPos, nextPos, box);

		return true;
	}

	private bool IsEmpty(Vector2 nextPos)
	{
		// �w�肵���}�X��null���ǂ���
		if (m_board[(int)nextPos.x, (int)nextPos.y] !=�@null) return false;	// null����Ȃ�
		return true;	// null
	}

	// m_board�̒��̔��𓮂���
	private void FixPlay(Vector2 thisPos, Vector2 nextPos, GameObject box)
	{
		m_board[(int)thisPos.x, (int)thisPos.y] = null; // m_board�̂��Ƃ��Ɣ����������ꏊ��null
        m_board[(int)nextPos.x, (int)nextPos.y] = box;	// �����������m_board�ɔ�������
		// �������̂���������
		if (EraseBox())
		{
			// �ז�����������S���������Ƃ�
			if (boxAmount == ObstacleCount) gameClear = true;
		}
	}

	// ����������ɏ������̂����邩�T��
	private bool EraseBox()
	{
		bool result = false;
		
		for (int y = 0; y < Row; ++y)
		{
			for (int x = 0; x < Column; ++x)
			{
				// ���̃}�X��null�Ȃ�continue
				if (IsEmpty(new Vector2(x, y))) continue;
				// ���̃}�X���ז��Ȃ�continue
				if (m_board[x, y].GetComponent<Box>().GetColor() == Box.BoxColor.None) continue;
				
				List<GameObject> connectBox = new List<GameObject>();
				CheckConnect(x, y, connectBox);
				if (connectBox.Count >= EraseCount)
				{
					result = true;

					foreach(GameObject box in connectBox)
					{
						// �Ֆʂ���폜
						Vector2 boardPos = ScreenToBoard(box.transform.position.x, box.transform.position.z);
						m_board[(int)boardPos.x, (int)boardPos.y] = null;

						// �Ղ�̍폜
						Destroy(box, 0.2f);
						boxAmount--;
					}
				}
			}
		}

		return result;
	}

	// �������̂��Ȃ����T���ċA�֐�
	private void CheckConnect(int x, int y, List<GameObject> connectBox)
	{
		// �폜�\�胊�X�g�ɓo�^
		connectBox.Add(m_board[x, y]);

		// enum�ŐF�w��\��
		int color = (int)m_board[x, y].GetComponent<Box>().GetColor();

		// ���g��Ֆʂ���ꎞ�I�ɏ��O����
		GameObject box = m_board[x, y];
		m_board[x, y] = null;

		// �E�ׂ肪�����F�H
		if (x + 1 < Column && m_board[x + 1, y] && (int)m_board[x + 1, y].GetComponent<Box>().GetColor() == color)
		{
			CheckConnect(x + 1, y, connectBox);
		}

		// ���ׂ肪�����F�H
		if (x - 1 >= 0 && m_board[x - 1, y] && (int)m_board[x - 1, y].GetComponent<Box>().GetColor() == color)
		{
			CheckConnect(x - 1, y, connectBox);
		}

		// ���ׂ肪�����F�H
		if (y + 1 < Row && m_board[x, y + 1] && (int)m_board[x, y + 1].GetComponent<Box>().GetColor() == color)
		{
			CheckConnect(x, y + 1, connectBox);
		}

		// ��ׂ肪�����F�H
		if (y - 1 >= 0 && m_board[x, y - 1] && (int)m_board[x, y - 1].GetComponent<Box>().GetColor() == color)
		{
			CheckConnect(x, y - 1, connectBox);
		}

		// ���g��Ֆʂɖ߂�
		m_board[x, y] = box;
	}
}