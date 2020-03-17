using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TileObject : MonoBehaviour
{
    public int TileID
    {
        get
        {
            return tile.ID;
        }
    }
    public int TileBGID
    {
        get
        {
            return tile.BackgroundID;
        }
        set
        {
            tile.BackgroundID = value;
        }
    }
    public TileType TileType
    {
        get
        {
            return tile.TileType;
        }
    }
    public SpriteRenderer BGRenderer;
    public Vector2Int Pos
    {
        get
        {
            return tile.Pos;
        }
        set
        {
            tile.Pos = value;
        }
    }
    private Tile tile;
    private SpriteRenderer spriteRenderer;
    private new Collider2D collider;
    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        collider = GetComponent<Collider2D>();
        if (tile == null)
        {
            tile = new Tile();
        }
    }
    private void OnMouseOver()
    {
        if (Input.GetButton("Fire1"))
        {
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                ChangeTo(GameController.Instance.Selected);
                if (GameController.Instance.Selected.Background)
                {
                    TileBGID = GameController.Instance.Selected.ID;
                    UpdateData();
                }
                OnlineLevelController.Instance.SendTile(tile);
            }
        }
        else if (Input.GetButton("Fire2") && GameController.Instance.Selected.Background)
        {
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                if (tile.Background)
                {
                    ChangeTo(GameController.Instance.Selected);
                }
                TileBGID = GameController.Instance.Selected.ID;
                UpdateData();
                OnlineLevelController.Instance.SendTile(tile);
            }
        }
    }
    public void UpdateData()
    {
        if (tile.BackgroundID != -1)
        {
            BGRenderer.sprite = GameController.Instance.PossibleTiles[tile.BackgroundID].TileSprite;
        }
        else
        {
            BGRenderer.sprite = null;
        }
        spriteRenderer.sprite = tile.TileSprite;
        collider.isTrigger = !tile.Solid;
        if (collider.isTrigger)
        {
            gameObject.layer = 8;
        }
        else
        {
            gameObject.layer = 0;
        }
    }
    public void ChangeTo(Tile other)
    {
        if (tile == null)
        {
            Start();
        }
        tile.TileSprite = other.TileSprite;
        tile.Solid = other.Solid;
        tile.ID = other.ID;
        tile.Background = other.Background;
        tile.TileType = other.TileType;
        UpdateData();
        if (tile.CustomCollider != other.CustomCollider)
        {
            if (other.CustomCollider)
            {
                Destroy(collider);
                collider = gameObject.AddComponent<PolygonCollider2D>();
                ((PolygonCollider2D)collider).autoTiling = true;
            }
            else
            {
                Destroy(collider);
                collider = gameObject.AddComponent<BoxCollider2D>();
                ((BoxCollider2D)collider).size = new Vector2(0.98f, 0.98f);
            }
        }
        tile.CustomCollider = other.CustomCollider;
        collider.isTrigger = !tile.Solid;
    }
}
