using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class Board : MonoBehaviour
{
	private GameObject[,] m_board;
	[SerializeField] GameObject boxParent;  // 箱の親オブジェクト
	[SerializeField] int Column = 9;    // 横
	[SerializeField] int Row = 9;       // 縦
	List<GameObject> m_box = new List<GameObject>();
	const int EraseCount = 3;   // 消すときの最低数
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
		boxAmount = boxParent.transform.childCount;	// 邪魔も含めた箱の数
		for(int i = 0;i < boxAmount;++i)
		{
			m_box.Add(boxParent.transform.GetChild(i).gameObject);
		}

        for(int z = 0;z < Row;++z)	// 縦
		{
			for(int x = 0; x < Column; ++x)	// 横
			{
				m_board[x, z] = null;
			}
		}

		foreach (GameObject box in m_box)
		{
			// -2, 2
			// ポジション 0～にしたうえででm_boardに入れる
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

		if(gameClear)	// クリア
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
		Vector2 nextPos = ScreenToBoard(posX, posY) + m_duration[duration]; // 今のポジションに動かしたときに移動する方向
		// 枠内かどうか
		if (nextPos.x >= Column) return false;
		if (nextPos.y >= Row) return false;
		if (nextPos.x < 0) return false;
		if (nextPos.y < 0) return false;
		
        // 空いているかどうか
        if (!IsEmpty(nextPos)) return false;
		// boardに入れる
		FixPlay(thisPos, nextPos, box);

		return true;
	}

	private bool IsEmpty(Vector2 nextPos)
	{
		// 指定したマスがnullかどうか
		if (m_board[(int)nextPos.x, (int)nextPos.y] !=　null) return false;	// nullじゃない
		return true;	// null
	}

	// m_boardの中の箱を動かす
	private void FixPlay(Vector2 thisPos, Vector2 nextPos, GameObject box)
	{
		m_board[(int)thisPos.x, (int)thisPos.y] = null; // m_boardのもともと箱があった場所をnull
        m_board[(int)nextPos.x, (int)nextPos.y] = box;	// 動かした先のm_boardに箱を入れる
		// 消すものがあった時
		if (EraseBox())
		{
			// 邪魔を除く箱を全部消したとき
			if (boxAmount == ObstacleCount) gameClear = true;
		}
	}

	// 動かした後に消すものがあるか探す
	private bool EraseBox()
	{
		bool result = false;
		
		for (int y = 0; y < Row; ++y)
		{
			for (int x = 0; x < Column; ++x)
			{
				// このマスがnullならcontinue
				if (IsEmpty(new Vector2(x, y))) continue;
				// このマスが邪魔ならcontinue
				if (m_board[x, y].GetComponent<Box>().GetColor() == Box.BoxColor.None) continue;
				
				List<GameObject> connectBox = new List<GameObject>();
				CheckConnect(x, y, connectBox);
				if (connectBox.Count >= EraseCount)
				{
					result = true;

					foreach(GameObject box in connectBox)
					{
						// 盤面から削除
						Vector2 boardPos = ScreenToBoard(box.transform.position.x, box.transform.position.z);
						m_board[(int)boardPos.x, (int)boardPos.y] = null;

						// ぷよの削除
						Destroy(box, 0.2f);
						boxAmount--;
					}
				}
			}
		}

		return result;
	}

	// 消すものがないか探す再帰関数
	private void CheckConnect(int x, int y, List<GameObject> connectBox)
	{
		// 削除予定リストに登録
		connectBox.Add(m_board[x, y]);

		// enumで色指定予定
		int color = (int)m_board[x, y].GetComponent<Box>().GetColor();

		// 自身を盤面から一時的に除外する
		GameObject box = m_board[x, y];
		m_board[x, y] = null;

		// 右隣りが同じ色？
		if (x + 1 < Column && m_board[x + 1, y] && (int)m_board[x + 1, y].GetComponent<Box>().GetColor() == color)
		{
			CheckConnect(x + 1, y, connectBox);
		}

		// 左隣りが同じ色？
		if (x - 1 >= 0 && m_board[x - 1, y] && (int)m_board[x - 1, y].GetComponent<Box>().GetColor() == color)
		{
			CheckConnect(x - 1, y, connectBox);
		}

		// 下隣りが同じ色？
		if (y + 1 < Row && m_board[x, y + 1] && (int)m_board[x, y + 1].GetComponent<Box>().GetColor() == color)
		{
			CheckConnect(x, y + 1, connectBox);
		}

		// 上隣りが同じ色？
		if (y - 1 >= 0 && m_board[x, y - 1] && (int)m_board[x, y - 1].GetComponent<Box>().GetColor() == color)
		{
			CheckConnect(x, y - 1, connectBox);
		}

		// 自身を盤面に戻す
		m_board[x, y] = box;
	}
}