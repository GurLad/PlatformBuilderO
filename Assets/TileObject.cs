using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileObject : MonoBehaviour
{
    public int TileID
    {
        get
        {
            return tile.ID;
        }
    }
    private Tile tile;
    private SpriteRenderer spriteRenderer;
    private new Collider2D collider;
    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        collider = GetComponent<Collider2D>();
        tile = new Tile();
    }
    private void OnMouseOver()
    {
        if (Input.GetButton("Fire1"))
        {
            ChangeTo(GameController.Instance.Selected);
        }
    }
    private void UpdateData()
    {
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
        UpdateData();
    }
}
