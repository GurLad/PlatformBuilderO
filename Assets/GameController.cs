using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        GenerateEmptyMap(MapSize);
    }
    private void Update()
    {
        for (int i = 0; i < 10; i++)
        {
            if (Input.GetKeyDown((KeyCode)(48 + i)))
            {
                selected = i;
            }
        }
        if (Input.GetButtonUp("Fire2"))
        {
            saveState = ToSaveData();
        }
        if (Input.GetButtonUp("Fire3"))
        {
            FromSaveData(saveState);
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
                result += Map[i, j].TileID;
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
                Map[i, j].ChangeTo(PossibleTiles[int.Parse(row[j])]);
            }
        }
        Debug.Log("Retrieved!\r\n" + data);
    }
}
