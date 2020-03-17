﻿using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;
using static SocketFunctions;

public class OnlineLevelController : MonoBehaviour
{
    public static OnlineLevelController Instance;
    public Rigidbody2D Player;
    public float PlayerChecksPerSecond = 5;
    public float TileChecksPerSecond = 1;
    public GameObject HostOnly;
    public GameObject OfflineOnly;
    [HideInInspector]
    public List<Rigidbody2D> Players;
    private float count;
    private float tileCount;
    private int playerID;
    private bool online = false;
    private List<Tile> tilesToSend = new List<Tile>();
    private void Start()
    {
        Instance = this;
        if (NetworkController.Instance.JoinOnline)
        {
            JoinOnline();
        }
    }
    public void JoinOnline()
    {
        HostOnly.SetActive(false);
        ShowPlayers();
        Socket sender = Connect();
        sender.SendOne("JOIN_LEVEL");
        sender.SendOne(NetworkController.Instance.CurrentLevel);
        playerID = int.Parse(sender.RecieveOne());
        for (int i = 0; i < playerID; i++)
        {
            Players.Add(null);
        }
        Players.Add(Player);
        sender.SendOne(PlayerToString(Player));
        online = true;
    }
    public void SendTile(Tile tile)
    {
        if (online)
        {
            tilesToSend.Add(tile);
        }
    }
    public void TurnOnline()
    {
        OfflineOnly.SetActive(false);
        Socket sender = Connect();
        sender.SendOne("TURN_ONLINE");
        sender.SendOne(NetworkController.Instance.CurrentLevel);
        playerID = 0;
        Players.Add(Player);
        sender.SendOne(PlayerToString(Player));
        online = true;
    }
    private void Update()
    {
        if (online)
        {
            count += Time.deltaTime * PlayerChecksPerSecond;
            if (count >= 1)
            {
                count -= 1;
                Socket sender = Connect();
                sender.SendOne("MOVE_PLAYER");
                sender.SendOne(NetworkController.Instance.CurrentLevel);
                sender.SendOne(playerID.ToString());
                sender.SendOne(PlayerToString(Player));
                ShowPlayers();
            }
            tileCount += Time.deltaTime * TileChecksPerSecond;
            if (tileCount >= 1)
            {
                tileCount -= 1;
                Socket sender = Connect();
                sender.SendOne("SEND_TILES");
                sender.SendOne(NetworkController.Instance.CurrentLevel);
                sender.SendOne(playerID.ToString());
                string toSend = "";
                tilesToSend.ForEach(tile => toSend += tile.Pos.x + "," + tile.Pos.y + ":" + tile.ID + ":" + tile.BackgroundID + ";");
                sender.SendOne(toSend);
                tilesToSend.Clear();
                sender = Connect();
                sender.SendOne("SEEK_TILES");
                sender.SendOne(NetworkController.Instance.CurrentLevel);
                sender.SendOne(playerID.ToString());
                string[] tileChanges = sender.ReceiveLargeData().Split(';');
                foreach (var tileChange in tileChanges)
                {
                    if (tileChange != "")
                    {
                        string[] tile = tileChange.Split(':');
                        string[] pos = tile[0].Split(',');
                        GameController.Instance.SetTile(int.Parse(pos[0]), int.Parse(pos[1]), int.Parse(tile[1]), int.Parse(tile[2]));
                    }
                }
            }
        }
    }
    private void ShowPlayers()
    {
        Socket sender = Connect();
        sender.SendOne("SHOW_PLAYERS");
        sender.SendOne(NetworkController.Instance.CurrentLevel);
        sender.SendOne(playerID.ToString());
        string[] players = sender.RecieveOne().Split('|');
        for (int i = 0; i < players.Length - 1; i++)
        {
            string[] parts = players[i].Split(':');
            int id = int.Parse(parts[0]);
            if (Players.Count == id)
            {
                Rigidbody2D newPlayer = Instantiate(Player.gameObject, transform).GetComponent<Rigidbody2D>();
                newPlayer.GetComponent<SpriteRenderer>().material.color = new Color(0, 1, 1, 0.5f);
                Destroy(newPlayer.gameObject.GetComponent<PlayerController>());
                Destroy(newPlayer.gameObject.GetComponentInChildren<Camera>());
                Destroy(newPlayer.gameObject.GetComponentInChildren<AudioListener>());
                Players.Add(newPlayer);
            }
            else if (Players.Count > id && Players[id] == null)
            {
                Rigidbody2D newPlayer = Instantiate(Player.gameObject, transform).GetComponent<Rigidbody2D>();
                newPlayer.GetComponent<SpriteRenderer>().material.color = new Color(0, 1, 1, 0.5f);
                Destroy(newPlayer.gameObject.GetComponent<PlayerController>());
                Destroy(newPlayer.gameObject.GetComponentInChildren<PixelPerfectCamera>());
                Destroy(newPlayer.gameObject.GetComponentInChildren<Camera>());
                Destroy(newPlayer.gameObject.GetComponentInChildren<AudioListener>());
                Players[id] = newPlayer;
            }
            else if (Players.Count < id)
            {
                throw new System.Exception("Oof: there are " + Players.Count + " players and trying to show player " + id);
            }
            StringToPlayer(parts[1], Players[id]);
        }
    }
    private string PlayerToString(Rigidbody2D player)
    {
        return player.transform.position.x + "," + player.transform.position.y + ";" + player.velocity.x + "," + player.velocity.y;
    }
    private void StringToPlayer(string player, Rigidbody2D playerRB)
    {
        string[] parts = player.Split(';');
        string[] posParts = parts[0].Split(',');
        playerRB.transform.position = new Vector3(float.Parse(posParts[0]), float.Parse(posParts[1]), -0.9f);
        string[] velocityParts = parts[1].Split(',');
        playerRB.velocity = new Vector2(float.Parse(velocityParts[0]), float.Parse(velocityParts[1]));
    }
}
