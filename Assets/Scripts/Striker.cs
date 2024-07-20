using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Striker : MonoBehaviour
{

    [SerializeField] private Transform m_strikerIndicator, m_foreDirection, m_chanceIndicator;
    [SerializeField] private Token m_token;

    public static bool m_chance;

    private Rigidbody2D m_rb2d;
    private bool m_canMoveIndicator, m_canAddForce, m_addedForce;
    private Vector3 m_mousePosition, m_updatedMousePosition;
    private float m_dis, stopThreshold = 0.5f;
    private Camera m_camera;

    private void Start()
    {
        m_rb2d = GetComponent<Rigidbody2D>();
        m_camera = Camera.main;
        m_canMoveIndicator = false;
        m_foreDirection.gameObject.SetActive(false);
        m_addedForce = false;

        if (m_token == Token.BLACK)
        {
            m_chance = true;
            m_chanceIndicator.gameObject.SetActive(true);
        }
        else if (m_token == Token.WHITE)
        {
            m_chance = false;
            m_chanceIndicator.gameObject.SetActive(false);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("BlackToken") || collision.CompareTag("WhiteToken") || collision.CompareTag("RedToken"))
        {
            GetComponent<SpriteRenderer>().color = Color.red;
            Debug.Log("Striker And Token Overlapping");
            GameManager.instance.CanSetStrikerValue = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("BlackToken") || collision.CompareTag("WhiteToken") || collision.CompareTag("RedToken"))
        {
            GetComponent<SpriteRenderer>().color = Color.white;
            GameManager.instance.CanSetStrikerValue = false;
        }
    }

    private void MouseDown()
    {
        if (m_chance)
        {
            m_canAddForce = true;
            m_canMoveIndicator = true;

            m_foreDirection.gameObject.SetActive(true);
            m_chanceIndicator.gameObject.SetActive(false);

            m_mousePosition = m_camera.ScreenToWorldPoint(Input.mousePosition) + m_strikerIndicator.transform.localScale;
            m_mousePosition.z = 0;
        }
    }
    private void MouseUp()
    {
        if (m_canAddForce)
        {
            m_rb2d.AddForce(
                new Vector2(m_foreDirection.position.x - transform.position.x,
                m_foreDirection.position.y - transform.position.y) * m_dis * 1000
                );
            m_addedForce = true;
            m_chance = false;

            GameManager.instance.StrikerSlider.interactable = false;
            GameManager.instance.m_currentPrefab = null;
            GameManager.instance.StopTimer();

            m_chanceIndicator.gameObject.SetActive(false);
            StrikeCancelledOrReleased();
        }
    }
    private void OnMouseEnter()
    {
        if (m_canMoveIndicator)
        {
            m_chanceIndicator.gameObject.SetActive(true);

            StrikeCancelledOrReleased();
        }
    }
    private void Update()
    {

        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

        if (Input.GetMouseButtonDown(0))
        {
            if (hit.collider != null)
            {
                if (hit.collider.CompareTag("Striker"))
                {
                    MouseDown();
                }
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            MouseUp();
        }

        m_updatedMousePosition = m_camera.ScreenToWorldPoint(Input.mousePosition);
        m_updatedMousePosition.z = 0;

        if (m_canMoveIndicator)
        {
            m_strikerIndicator.LookAt(m_updatedMousePosition);
            m_dis = Vector3.Distance(m_mousePosition, m_updatedMousePosition);
            m_dis = Mathf.Clamp(m_dis, 0f, 3.5f);
            m_strikerIndicator.localScale = new Vector3(m_dis, m_dis, m_dis);
        }
    }
    private void LateUpdate()
    {
        if (m_rb2d.velocity.magnitude <= stopThreshold && m_rb2d.IsSleeping() && m_addedForce)
        {
            Debug.Log("object has mostly stopped.");
            m_addedForce = false;
            GameManager.instance.SpawnStriker();
            Destroy(gameObject);
        }
    }
    public void StrikeCancelledOrReleased()
    {
        m_canMoveIndicator = false;
        m_canAddForce = false;

        m_foreDirection.gameObject.SetActive(false);

        m_strikerIndicator.localScale = Vector3.one;


    }
}