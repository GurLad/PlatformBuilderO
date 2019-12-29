using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class Tile
{
    public Sprite TileSprite;
    public bool Solid;
    [HideInInspector]
    public int ID;
}
