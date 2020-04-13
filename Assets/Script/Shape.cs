using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;


/// <summary>
/// テトリスの形状クラス
/// テトリミノ(tetrimino)
/// </summary>
public class Shape : MonoBehaviour
{

	#region inspector

	public Color BlockColor;

	/// <summary>
	/// 回転不可形状か
	/// </summary>
	public bool DisableRotate = false;

	/// <summary>
	/// 落下インターバル(秒)
	/// </summary>
	const float ThresholdInterval = 1.0f;

	#endregion inspector


	#region field

	/// <summary>
	/// ブロックのtransform
	/// </summary>
	[NonSerialized]
	public List<Transform>	Blocks = new List<Transform>();

	public Transform MyTransform = null;

	/// <summary>
	/// 落下インターバル(秒)
	/// </summary>
	float IntervalCount = 0.0f;

	#endregion field

	private void Awake()
	{
		// ブロックの色を指定の色に変更
		MeshRenderer[] renderers = gameObject.GetComponentsInChildren<MeshRenderer>(true);
		foreach(var renderer in renderers){
			renderer.material.color = BlockColor;
		}

		// シェイプが抱えるブロックを収集
		Blocks = gameObject.GetComponentsInChildren<Transform>(true).ToList();

		// 自身のトランスフォームをキャッシュ
		MyTransform = gameObject.transform;
	}

	// Start is called before the first frame update
	void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
		// S押してる間落下間隔早くする
		if(Input.GetKey(KeyCode.S)){
			IntervalCount += Time.deltaTime * 10.0f;
		}
		else{
			IntervalCount += Time.deltaTime;
		}

		#region debug

		if (Input.GetKey(KeyCode.W)){
			IntervalCount = 0.0f;
		}

		#endregion debug

		if (Input.GetKeyDown(KeyCode.A)){
			MyTransform.position -= new Vector3(1.0f, 0.0f);
			if(!isValidPos(MyTransform.position)){
				// 座標を戻す
				MyTransform.position += new Vector3(1.0f, 0.0f);
			}
		}
		if(Input.GetKeyDown(KeyCode.D)){
			MyTransform.position += new Vector3(1.0f, 0.0f);
			if(!isValidPos(MyTransform.position)){
				// 座標を戻す
				MyTransform.position -= new Vector3(1.0f, 0.0f);
			}
		}
		if(Input.GetKeyDown(KeyCode.Space)){
			MyTransform.rotation = Quaternion.Euler(0.0f, 0.0f, 90.0f) * MyTransform.rotation;
			if(!isValidPos(MyTransform.position)){
				MyTransform.rotation = Quaternion.Euler(0.0f, 0.0f, -90.0f) * MyTransform.rotation;
			}
		}

		if(IntervalCount >= ThresholdInterval){
			MyTransform.position -= new Vector3(0.0f, 1.0f);
			IntervalCount = 0.0f;
			if(!isValidPos(MyTransform.position)){
				// 座標を戻す
				MyTransform.position += new Vector3(0.0f, 1.0f);

				// 終了処理
				finalize();
			}
		}

    }


	private void OnDestroy()
	{
		// 子ブロックとの親子関係を切る
		foreach(var block in Blocks){
			block.parent = null;
		}
	}


	/// <summary>
	/// 正常位置の確認
	/// </summary>
	/// <param name="pos"></param>
	/// <returns></returns>
	bool isValidPos(Vector3 pos)	// 引数いらんかも
	{
		bool result = true;

		// 近い整数値に丸める
		MyTransform.position = Utility.roundVector(MyTransform.position);

		foreach(var block in Blocks){
			// 近い整数値に丸める
			block.position = Utility.roundVector(block.position);
			// ボード内かどうかチェック
			if(!Ingame.insideBorad(block.position)){
				return false;
			}

			// ボードの対応マスにすでにブロックが入っている場合はそこは行けないところ
			int x = (int)(block.position.x + 0.5f);
			int y = (int)(block.position.y + 0.5f);

			if(y < Ingame.Y_SIZE){
				if(Ingame.Board[x,y] != null){
					return false;
				}
			}
		}

		return result;
	}


	/// <summary>
	/// shape終了処理
	/// </summary>
	void finalize()
	{
		// ボードの対応マスにブロックを登録
		foreach(var block in Blocks){
			// 近い整数値に丸める
			block.position = Utility.roundVector(block.position);
			int x = (int)(block.position.x + 0.5f);
			int y = (int)(block.position.y + 0.5f);

			// 終了処理のタイミングでブロックがボードの範囲外だった場合はゲームオーバー
			if(y >= Ingame.Y_SIZE){
				Ingame.requestGameOver();
				break;
			}
			
			Ingame.Board[x,y] = block;
		}

		// 自身を削除
		Destroy(this.gameObject);
		// 次の形状を生成リクエスト
		Ingame.requestSpawnShape();

	}

}
