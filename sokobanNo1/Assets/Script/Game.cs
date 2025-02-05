using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    //�^�C���̎��
    private enum TileType
    {
        NONE,   //�����Ȃ�
        GROUND, //�n��
        TARGET, //�ړI�n
        PLAYER, //�v���C���[
        BLOCK,  //�u���b�N

        PLAYER_ON_TARGET,//�v���C���[(�ړI�n�̏�)
        BLOCK_ON_TARGET, //�u���b�N(�ړI�n�̏�)
    }

    public TextAsset stageFile;//�X�e�[�W�\�����L�q���ꂽ�e�L�X�g�t�@�C��
    public float tileSize;//�^�C���̃T�C�Y

    public Sprite groundSprite; //�n�ʂ̃X�v���C�g
    public Sprite targetSprite; //�ړI�n�̃X�v���C�g
    public Sprite playerSprite; //�v���C���[�̃X�v���C�g
    public Sprite blockSprite;  //�u���b�N�̃X�v���C�g

    private GameObject player;    //�v���C���[�̃Q�[���I�u�W�F�N�g
    private Vector2 middleOffset; //���S�ʒu
    private int blockCount;       //�u���b�N�̐�

    private int rows;   //�s��
    private int columns;//��
    private TileType[,] tileList;//�^�C�������Ǘ�����񎟌��z��

    //�e�ʒu�ɑ��݂���Q�[���I�u�W�F�N�g���Ǘ�����A�z�z��
    private Dictionary<GameObject, Vector2Int> gameObjectPosTable = new Dictionary<GameObject, Vector2Int>();


    private void Start()
    {
        LoadTileDate(); //�^�C������ǂݍ���
        CreateStage();  //�X�e�[�W���쐬
    }


    //�^�C������ǂݍ���
    private void LoadTileDate()
    {

        //�^�C���̏�����s���Ƃɕ���
        var lines = stageFile.text.Split
        (
            new[] { '\r', '\n' },
            System.StringSplitOptions.RemoveEmptyEntries
        );

        //�^�C���̗񐔂��v�Z
        var nums = lines[0].Split(new[] { ',' });

        //�^�C���̗񐔂ƍs���ێ�

        rows = lines.Length;//�s��
        columns = nums.Length;//��

        //�^�C������ int�^��2�����z��ŕێ�
        tileList = new TileType[columns, rows];
        for(int y = 0; y < rows; y++)
        {

            //�ꕶ�����擾
            var st = lines[y];
            nums = st.Split(new[] { ',' });
            for(int x = 0; x < columns;x++)
            {
                //�ǂݍ��񂾕����𐔒l�ɕϐ���ێ�
                tileList[x, y] = (TileType)int.Parse(nums[x]);
            }

        }

    }

    //�X�e�[�W���쐬
    private void CreateStage()
    {
        //�X�e�[�W�̒��S�ʒu���v�Z
        middleOffset.x = columns * tileSize * 0.5f - tileSize * 0.5f;
        middleOffset.y = rows * tileSize * 0.5f - tileSize * 0.5f; 

        for(int y= 0; y < rows; y++)
        {
            for (int x = 0; x < columns; x++)
            {
                var val = tileList[x, y];

                //�����Ȃ��ꏊ�͖���
                if (val == TileType.NONE) continue;

                //�^�C���̖��O�ɍs�ԍ��Ɨ�ԍ���t�^
                var name = "title" + y + "_" + x;

                //�^�C���̃Q�[���I�u�W�F�N�g���쐬
                var tile = new GameObject(name);

                //�^�C���ɃX�v���C�g��`�悷��@�\��ǉ�
                var sr = tile.AddComponent<SpriteRenderer>();

                //�^�C���̃X�v���C�g�ݒ�
                sr.sprite = groundSprite;

                //�^�C���̈ʒu��ݒ�
                tile.transform.position = GetDisplayPosition(x, y);

                //�ړI�n�̏ꍇ
                if(val == TileType.TARGET)
                {
                    //�ړI�n�̃Q�[���I�u�W�F�N�g���쐬
                    var destination = new GameObject("destination");

                    //�ړI�n�ɃX�v���C�g��`�悷��@�\��ǉ�
                    sr = destination.AddComponent<SpriteRenderer>();

                    //�ړI�n�̃X�v���C�g��ݒ�
                    sr.sprite = targetSprite;

                    //�ړI�n�̕`�揇����O�ɂ���
                    sr.sortingOrder = 1;

                    //�ړI�̈ʒu��ݒ�
                    destination.transform.position = GetDisplayPosition(x, y);
                }

                //�v���C���[�̏ꍇ
                if(val == TileType.PLAYER)
                {

                    //�v���C���[�̃Q�[���I�u�W�F�N�g���쐬
                    player = new GameObject("player");

                    //�v���C���[�ɃX�v���C�g��`�悷��@�\��ǉ�
                    sr = player.AddComponent<SpriteRenderer>();

                    //�v���C���[�̃X�v���C�g�̐ݒ�
                    sr.sprite = playerSprite;

                    //�v���C���[�̕`�揇����O�ɂ���
                    sr.sortingOrder = 2;

                    //�v���C���[�̈ʒu��ݒ�
                    player.transform.position = GetDisplayPosition(x, y);

                    //�v���C���[��A�z�z��ɒǉ�
                    gameObjectPosTable.Add(player, new Vector2Int(x, y));
                }

                //�u���b�N�̏ꍇ
                else if(val == TileType.BLOCK)
                {
                    //�u���b�N�̐��𑝂₷
                    blockCount++;

                    //�u���b�N�̃Q�[���I�u�W�F�N�g���쐬
                    var block = new GameObject("block" + blockCount);

                    //�u���b�N�ɃX�v���C�g��`�悷��@�\��ǉ�
                    sr = block.AddComponent<SpriteRenderer>();

                    //�u���b�N�̃X�v���C�g�̐ݒ�
                    sr.sprite = blockSprite;

                    //�u���b�N�̕`�揇����O�ɂ���
                    sr.sortingOrder = 2;

                    //�u���b�N�̈ʒu��ݒ�
                    block.transform.position = GetDisplayPosition(x, y);

                    //�u���b�N��A�z�z��ɒǉ�
                    gameObjectPosTable.Add(block, new Vector2Int(x, y));
                }

            }
        }
    }

    //�w�肳�ꂽ�s�ԍ��Ɨ�ԍ�����X�v���C�g�̕\���ʒu���v�Z���Ă��܂�
    private Vector2 GetDisplayPosition(int x,int y)
    {
        return  new Vector2
        (
            x * tileSize - middleOffset.x,
            y * tileSize - middleOffset.y
        );
    }

}
