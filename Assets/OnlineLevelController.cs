using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.UI;
using static SocketFunctions;

public class OnlineLevelController : MonoBehaviour
{
    public Rigidbody2D Player;
    public float ChecksPerSecond = 1;
    public GameObject Canvas;
    [HideInInspector]
    public List<Rigidbody2D> Players;
    private float count;
    private int playerID;
    private bool online = false;
    private void Start()
    {
        if (NetworkController.Instance.JoinOnline)
        {
            JoinOnline();
        }
    }
    public void JoinOnline()
    {
        Canvas.SetActive(false);
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
    public void TurnOnline()
    {
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
            count += Time.deltaTime;
            if (count >= ChecksPerSecond)
            {
                count -= ChecksPerSecond;
                Socket sender = Connect();
                sender.SendOne("MOVE_PLAYER");
                sender.SendOne(NetworkController.Instance.CurrentLevel);
                sender.SendOne(playerID.ToString());
                sender.SendOne(PlayerToString(Player));
                ShowPlayers();
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
                Players.Add(newPlayer);
            }
            else if (Players.Count > id && Players[id] == null)
            {
                Rigidbody2D newPlayer = Instantiate(Player.gameObject, transform).GetComponent<Rigidbody2D>();
                newPlayer.GetComponent<SpriteRenderer>().material.color = new Color(0, 1, 1, 0.5f);
                Destroy(newPlayer.gameObject.GetComponent<PlayerController>());
                Destroy(newPlayer.gameObject.GetComponentInChildren<Camera>());
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
