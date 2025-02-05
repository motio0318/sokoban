using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    //タイルの種類
    private enum TileType
    {
        NONE,   //何もない
        GROUND, //地面
        TARGET, //目的地
        PLAYER, //プレイヤー
        BLOCK,  //ブロック

        PLAYER_ON_TARGET,//プレイヤー(目的地の上)
        BLOCK_ON_TARGET, //ブロック(目的地の上)
    }

    public TextAsset stageFile;//ステージ構造が記述されたテキストファイル
    public float tileSize;//タイルのサイズ

    public Sprite groundSprite; //地面のスプライト
    public Sprite targetSprite; //目的地のスプライト
    public Sprite playerSprite; //プレイヤーのスプライト
    public Sprite blockSprite;  //ブロックのスプライト

    private GameObject player;    //プレイヤーのゲームオブジェクト
    private Vector2 middleOffset; //中心位置
    private int blockCount;       //ブロックの数

    private int rows;   //行数
    private int columns;//列数
    private TileType[,] tileList;//タイル情報を管理する二次元配列

    //各位置に存在するゲームオブジェクトを管理する連想配列
    private Dictionary<GameObject, Vector2Int> gameObjectPosTable = new Dictionary<GameObject, Vector2Int>();


    private void Start()
    {
        LoadTileDate(); //タイル情報を読み込む
        CreateStage();  //ステージを作成
    }


    //タイル情報を読み込む
    private void LoadTileDate()
    {

        //タイルの情報を一行ごとに分割
        var lines = stageFile.text.Split
        (
            new[] { '\r', '\n' },
            System.StringSplitOptions.RemoveEmptyEntries
        );

        //タイルの列数を計算
        var nums = lines[0].Split(new[] { ',' });

        //タイルの列数と行列を保持

        rows = lines.Length;//行数
        columns = nums.Length;//列数

        //タイル情報を int型の2次元配列で保持
        tileList = new TileType[columns, rows];
        for(int y = 0; y < rows; y++)
        {

            //一文字ずつ取得
            var st = lines[y];
            nums = st.Split(new[] { ',' });
            for(int x = 0; x < columns;x++)
            {
                //読み込んだ文字を数値に変数を保持
                tileList[x, y] = (TileType)int.Parse(nums[x]);
            }

        }

    }

    //ステージを作成
    private void CreateStage()
    {
        //ステージの中心位置を計算
        middleOffset.x = columns * tileSize * 0.5f - tileSize * 0.5f;
        middleOffset.y = rows * tileSize * 0.5f - tileSize * 0.5f; 

        for(int y= 0; y < rows; y++)
        {
            for (int x = 0; x < columns; x++)
            {
                var val = tileList[x, y];

                //何もない場所は無視
                if (val == TileType.NONE) continue;

                //タイルの名前に行番号と列番号を付与
                var name = "title" + y + "_" + x;

                //タイルのゲームオブジェクトを作成
                var tile = new GameObject(name);

                //タイルにスプライトを描画する機能を追加
                var sr = tile.AddComponent<SpriteRenderer>();

                //タイルのスプライト設定
                sr.sprite = groundSprite;

                //タイルの位置を設定
                tile.transform.position = GetDisplayPosition(x, y);

                //目的地の場合
                if(val == TileType.TARGET)
                {
                    //目的地のゲームオブジェクトを作成
                    var destination = new GameObject("destination");

                    //目的地にスプライトを描画する機能を追加
                    sr = destination.AddComponent<SpriteRenderer>();

                    //目的地のスプライトを設定
                    sr.sprite = targetSprite;

                    //目的地の描画順を手前にする
                    sr.sortingOrder = 1;

                    //目的の位置を設定
                    destination.transform.position = GetDisplayPosition(x, y);
                }

                //プレイヤーの場合
                if(val == TileType.PLAYER)
                {

                    //プレイヤーのゲームオブジェクトを作成
                    player = new GameObject("player");

                    //プレイヤーにスプライトを描画する機能を追加
                    sr = player.AddComponent<SpriteRenderer>();

                    //プレイヤーのスプライトの設定
                    sr.sprite = playerSprite;

                    //プレイヤーの描画順を手前にする
                    sr.sortingOrder = 2;

                    //プレイヤーの位置を設定
                    player.transform.position = GetDisplayPosition(x, y);

                    //プレイヤーを連想配列に追加
                    gameObjectPosTable.Add(player, new Vector2Int(x, y));
                }

                //ブロックの場合
                else if(val == TileType.BLOCK)
                {
                    //ブロックの数を増やす
                    blockCount++;

                    //ブロックのゲームオブジェクトを作成
                    var block = new GameObject("block" + blockCount);

                    //ブロックにスプライトを描画する機能を追加
                    sr = block.AddComponent<SpriteRenderer>();

                    //ブロックのスプライトの設定
                    sr.sprite = blockSprite;

                    //ブロックの描画順を手前にする
                    sr.sortingOrder = 2;

                    //ブロックの位置を設定
                    block.transform.position = GetDisplayPosition(x, y);

                    //ブロックを連想配列に追加
                    gameObjectPosTable.Add(block, new Vector2Int(x, y));
                }

            }
        }
    }

    //指定された行番号と列番号からスプライトの表示位置を計算しています
    private Vector2 GetDisplayPosition(int x,int y)
    {
        return  new Vector2
        (
            x * tileSize - middleOffset.x,
            y * tileSize - middleOffset.y
        );
    }

}
