﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public static GameController Instance;
    public Tile Selected
    {
        get
        {
            return PossibleTiles[selected];
        }
    }
    public Tile[] PossibleTiles;
    public TileObject BaseTileObject;
    [Header("Initial generation")]
    public Vector2Int MapSize;
    public int BaseBG;
    public int BaseGround;
    public RectTransform TileButton;
    [HideInInspector]
    public TileObject[,] Map;
    private int selected;
    private string saveState;
    private void Awake()
    {
        Instance = this;
        for (int i = 0; i < PossibleTiles.Length; i++)
        {
            PossibleTiles[i].ID = i;
        }
    }
    private void Start()
    {
        if (NetworkController.Instance.CurrentLevel != "")
        {
            NetworkController.Instance.LoadLevel(NetworkController.Instance.CurrentLevel);
        }
        else
        {
            GenerateEmptyMap(MapSize);
        }
        for (int i = 0; i < PossibleTiles.Length; i++)
        {
            RectTransform newTileButton = Instantiate(TileButton.gameObject, TileButton.transform.parent).GetComponent<RectTransform>();
            newTileButton.anchoredPosition = new Vector2(16 * i, 0);
            newTileButton.GetComponent<Image>().sprite = PossibleTiles[i].TileSprite;
            newTileButton.GetComponent<SelectTile>().ID = i;
            newTileButton.gameObject.SetActive(true);
        }
    }
    public void GenerateEmptyMap(Vector2Int newSize)
    {
        if (Map != null)
        {
            for (int i = 0; i < MapSize.x; i++)
            {
                for (int j = 0; j < MapSize.y; j++)
                {
                    Destroy(Map[i, j].gameObject);
                }
            }
        }
        MapSize = newSize;
        Map = new TileObject[MapSize.x, MapSize.y];
        for (int i = 0; i < MapSize.x; i++)
        {
            for (int j = 0; j < MapSize.y; j++)
            {
                Map[i, j] = Instantiate(BaseTileObject, transform);
                Map[i, j].transform.position = new Vector2((i - (MapSize.x - 1) / 2) * Map[i, j].transform.localScale.x, (j - (MapSize.y - 1) / 2) * Map[i, j].transform.localScale.y);
                Map[i, j].gameObject.SetActive(true);
                if (i == 0 || j == 0 || i == MapSize.x - 1 || j == MapSize.y - 1)
                {
                    Map[i, j].ChangeTo(PossibleTiles[BaseGround]);
                }
                else
                {
                    Map[i, j].ChangeTo(PossibleTiles[BaseBG]);
                }
                Map[i, j].TileBGID = BaseBG;
                Map[i, j].Pos = new Vector2Int(i, j);
            }
        }
    }
    public string ToSaveData()
    {
        string result = MapSize.x + "," + MapSize.y + "\n";
        for (int i = 0; i < MapSize.x; i++)
        {
            for (int j = 0; j < MapSize.y; j++)
            {
                result += Map[i, j].TileID + "." + Map[i, j].TileBGID;
                if (j != MapSize.y - 1)
                {
                    result += ",";
                }
            }
            result += ";";
        }
        Debug.Log("Saved!\r\n" + result);
        return result;
    }
    public void FromSaveData(string data)
    {
        data = data.Split('~')[1];
        string sizeString = data.Split('\n')[0];
        string[] size = sizeString.Split(',');
        GenerateEmptyMap(new Vector2Int(int.Parse(size[0]), int.Parse(size[1])));
        string[] rows = data.Split('\n')[1].Split(';');
        for (int i = 0; i < MapSize.x; i++)
        {
            string[] row = rows[i].Split(',');
            for (int j = 0; j < MapSize.y; j++)
            {
                string[] tileData = row[j].Split('.');
                Map[i, j].TileBGID = int.Parse(tileData[1]);
                Map[i, j].ChangeTo(PossibleTiles[int.Parse(tileData[0])]);
            }
        }
        Debug.Log("Retrieved!\r\n" + data);
        SpawnPlayer();
    }
    public void SelectTile(int id)
    {
        selected = id;
    }
    public void SpawnPlayer()
    {
        PlayerController.Instance.transform.position = new Vector3(0, 0, -0.9f);
        for (int i = 0; i < MapSize.x; i++)
        {
            for (int j = 0; j < MapSize.y; j++)
            {
                if (Map[i, j].TileType == TileType.Start)
                {
                    PlayerController.Instance.transform.position = new Vector3(i - MapSize.x / 2 + 1, j - MapSize.y / 2 + 1, -0.9f);
                }
            }
        }
        PlayerController.Instance.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        PlayerController.Instance.ChangeWinState(false);
    }
    public void SetTile(int x, int y, int id, int bgID)
    {
        Map[x, y].ChangeTo(PossibleTiles[id]);
        Map[x, y].TileBGID = bgID;
        Map[x, y].UpdateData();
    }
}
