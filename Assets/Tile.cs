using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum TileType { None, Start, Spike, Win }
[System.Serializable]
public class Tile
{
    public Sprite TileSprite;
    public bool Solid;
    public bool Background;
    public bool CustomCollider;
    public TileType TileType;
    [HideInInspector]
    public int ID;
    [HideInInspector]
    public int BackgroundID = -1;
    public Vector2Int Pos;
}
