using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;
    public float Speed;
    public float JumpForce;
    public Sprite WinSprite;
    private Sprite BaseSprite;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rigidbody;
    private void Start()
    {
        Instance = this;
        rigidbody = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        BaseSprite = spriteRenderer.sprite;
    }
    private void LateUpdate()
    {
        if (Input.GetButton("Cancel"))
        {
            OnlineLevelController.Instance.ExitLevel();
            SceneManager.LoadScene("LevelSelect");
        }
        float vY = rigidbody.velocity.y;
        if (Input.GetButton("Jump") && IsGrounded())
        {
            vY = JumpForce;
        }
        rigidbody.velocity = new Vector2(Speed * Input.GetAxis("Horizontal"), vY);
        if (rigidbody.IsSleeping())
        {
            transform.position = new Vector3(Mathf.Round(transform.position.x * 16) / 16, Mathf.Round(transform.position.y * 16) / 16, -0.9f);
        }
    }
    private bool IsGrounded()
    {
        Collider2D[] colliders = Physics2D.OverlapBoxAll(transform.position - new Vector3(0, transform.localScale.y / 2), new Vector2(transform.localScale.x - 0.04f, 0.1f), 0);
        foreach (var item in colliders)
        {
            if (item.gameObject.layer != 8)
            {
                //Debug.Log(item.gameObject);
                return true;
            }
        }
        return false;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        TileObject tileObject = collision.gameObject.GetComponent<TileObject>();
        if (tileObject != null)
        {
            switch (tileObject.TileType)
            {
                case TileType.None:
                    break;
                case TileType.Start:
                    break;
                case TileType.Spike:
                    GameController.Instance.SpawnPlayer();
                    break;
                case TileType.Win:
                    ChangeWinState(true);
                    break;
                default:
                    break;
            }
        }
    }
    public void ChangeWinState(bool won)
    {
        if (won)
        {
            spriteRenderer.sprite = WinSprite;
        }
        else
        {
            spriteRenderer.sprite = BaseSprite;
        }
    }
}
