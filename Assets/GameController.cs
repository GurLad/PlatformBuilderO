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
    }
    public void GenerateEmptyMap(Vector2Int newSize)
    {
        if (Map != null)
        {
            for (int i = 0; i < MapSize.x; i++)
            {
                for (int j = 0; j < MapSize.y; j++)
                {
                    Destroy(Map[i, j]);
                }
            }
        }
        MapSize = newSize;
        Map = new TileObject[MapSize.x, MapSize.y];
        for (int i = 0; i < MapSize.x; i++)
        {
            for (int j = 0; j < MapSize.y; j++)
            {
                TileObject current = Instantiate(BaseTileObject);
                current.transform.position = new Vector2((i - (MapSize.x - 1) / 2) * current.transform.localScale.x, (j - (MapSize.y - 1) / 2) * current.transform.localScale.y);
                if (i == 0 || j == 0 || i == MapSize.x - 1 || j == MapSize.y - 1)
                {
                    current.ChangeTo(PossibleTiles[BaseGround]);
                }
                else
                {
                    current.ChangeTo(PossibleTiles[BaseBG]);
                }
                Instance.Map[i, j] = current;
                current.gameObject.SetActive(true);
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
        return result;
    }
    public void FromSaveData(string data)
    {
        string sizeString = data.Split('\n')[0];
    }
}
