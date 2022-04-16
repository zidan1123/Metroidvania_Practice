using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetDashAbility : MonoBehaviour
{
    private GameObject player;

    private Transform m_Transform;
    private Transform Description_Panel_Transform;

    private SpriteRenderer m_SpriteRenderer;
    private BoxCollider2D m_BoxCollider2D;

    private bool isOpenPanel;

    void Start()
    {
        Init();
    }

    void Update()
    {
        if (isOpenPanel)
        {
            if (Input.GetMouseButtonUp(0) || Input.GetKeyUp(KeyCode.Space))
            {
                FreePlayer();
                Destroy(gameObject);
            }
        }
    }

    private void Init()
    {
        player = GameObject.FindGameObjectWithTag("Player");

        m_Transform = gameObject.GetComponent<Transform>();
        Description_Panel_Transform = m_Transform.Find("Canvas/Description_Panel");

        m_SpriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        m_BoxCollider2D = gameObject.GetComponent<BoxCollider2D>();

        HideDescriptionPanel();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            collision.gameObject.GetComponent<PlayerController>().IsObtainDash = true;
            Destroy(m_SpriteRenderer);
            Destroy(m_BoxCollider2D);
            ShowDescriptionPanel();
            FreezePlayer();
        }
    }

    private void HideDescriptionPanel()
    {
        Description_Panel_Transform.gameObject.SetActive(false);
    }

    private void ShowDescriptionPanel()
    {
        isOpenPanel = true;
        Description_Panel_Transform.gameObject.SetActive(true);
    }
    
    private void FreezePlayer() //这两个方法感觉可以写在PlayerController里
    {
        //player_PlayerController = player.GetComponent<PlayerController>();
        //player_Rigidbody2D = player.GetComponent<Rigidbody2D>();
        //player_Animator = player.GetComponent<Animator>();

        player.GetComponent<PlayerController>().enabled = false;
        player.GetComponent<Rigidbody2D>().velocity = new Vector2(0, player.GetComponent<Rigidbody2D>().velocity.y);
        player.GetComponent<Animator>().SetBool("Walk", false);
        player.GetComponent<Animator>().SetBool("Ground", true);
    }

    private void FreePlayer() //
    {
        player.GetComponent<PlayerController>().enabled = true;
    }
}
