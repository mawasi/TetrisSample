using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;


/*
memo

参考
https://noobtuts.com/unity/2d-tetris-game/
https://ja.wikipedia.org/wiki/%E3%83%86%E3%83%88%E3%83%AA%E3%82%B9
http://2dgames.jp/2012/05/22/%E3%83%86%E3%83%88%E3%83%AA%E3%82%B9%E3%81%AE%E4%BD%9C%E3%82%8A%E6%96%B9/

*/

public class Ingame : MonoBehaviour
{

	#region definition

	[Serializable]
	public class Position
	{
		public int x = 0;
		public int y = 0;
	}


	// ボードのサイズ
	public const int X_SIZE = 10;
	public const int Y_SIZE = 20;

	#endregion definition


	#region inspector

	/// <summary>
	/// シェイプのリスト
	/// </summary>
	public List<GameObject>	Shapes = new List<GameObject>();

	/// <summary>
	/// シェイプ出現位置
	/// </summary>
	public Position		SpawnPosition = new Position();

	#endregion inspector


	#region field

	/// <summary>
	/// ゲームエリア
	/// </summary>
	/// <remarks>
	/// Boardの底をY=0とする
	/// </remarks>
	[NonSerialized]
	public static Transform[,] Board = new Transform[X_SIZE, Y_SIZE];


	/// <summary>
	/// 形状生成リクエスト行うか
	/// </summary>
	public static bool IsRequestSpawnShape = false;

	/// <summary>
	/// ゲームオーバーリクエスト
	/// </summary>
	public static bool IsRequestGameOver = false;

	#endregion field


	// Start is called before the first frame update
	void Start()
    {
        // Boardの初期化
		for(int x = 0; x < X_SIZE; x++){
			for(int y = 0; y < Y_SIZE; y++){
				Board[x, y] = null;
			}
		}

		spawnShape();
    }

    // Update is called once per frame
    void Update()
    {
		if(IsRequestGameOver){
			SceneManager.LoadScene("Ingame");
			IsRequestGameOver = false;
			return;
		}

        if(IsRequestSpawnShape){
			spawnShape();
			IsRequestSpawnShape = false;
		}

		deleteFullRow();
    }

	private void OnDestroy()
	{
		IsRequestSpawnShape = false;
		IsRequestGameOver = false;
	}


	/// <summary>
	/// 形状の生成
	/// </summary>
	void spawnShape()
	{
		int i = UnityEngine.Random.Range(0, Shapes.Count);

		Instantiate(Shapes[i], new Vector3(SpawnPosition.x, SpawnPosition.y), Quaternion.identity);
	}


	/// <summary>
	/// 列が埋まってるか
	/// </summary>
	/// <param name="y"></param>
	/// <returns></returns>
	bool isFullRow(int y)
	{
		if(y < 0 || y >= Y_SIZE){
			Debug.LogError($"行数の指定が不正です。 y = {y}");
			return false;
		}

		for(int x = 0; x < X_SIZE; x++){
			if(Board[x,y] == null){
				return false;
			}
		}

		return true;
	}


	/// <summary>
	/// 列の削除
	/// </summary>
	/// <param name="y"></param>
	void deleteRow(int y)
	{
		for(int x = 0; x < X_SIZE; x++){
			if(Board[x,y] != null){
				Destroy(Board[x,y].gameObject);
				Board[x,y] = null;
			}
		}
	}

	/// <summary>
	/// 指定のブロックの行を一段下げる
	/// </summary>
	/// <param name="y"></param>
	void decreaseRow(int y)
	{
		for(int x = 0; x < X_SIZE; x++){
			if(Board[x,y] != null){
				Board[x,y-1] = Board[x,y];
				Board[x,y] = null;
				Board[x,y-1].position -= new Vector3(0.0f, 1.0f);
			}
		}
	}


	/// <summary>
	/// 指定の行以降のブロックの行を1段づつ下げる
	/// </summary>
	/// <param name="y"></param>
	void decreaseRowAbove(int y)
	{
		for(int i = y; i < Y_SIZE; i++){
			decreaseRow(i);
		}
	}


	void deleteFullRow()
	{
		for(int y = 0; y < Y_SIZE; y++){
			if(isFullRow(y)){
				deleteRow(y);
				decreaseRowAbove(y);
				y--;
			}
		}
	}


	/// <summary>
	/// ボード内かどうか
	/// </summary>
	/// <param name="pos"></param>
	/// <returns></returns>
	public static bool insideBorad(Vector3 pos)
	{
		if((int)pos.x >= 0 && (int)pos.x < X_SIZE && (int)pos.y >= 0){
			return true;
		}

		return false;
	}


	/// <summary>
	/// 形状生成のリクエスト
	/// </summary>
	public static void requestSpawnShape()
	{
		IsRequestSpawnShape = true;
	}


	/// <summary>
	/// ゲームオーバーリクエスト
	/// </summary>
	public static void requestGameOver()
	{
		IsRequestGameOver = true;
	}
}
