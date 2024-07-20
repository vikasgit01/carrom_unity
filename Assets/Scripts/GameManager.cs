using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum Token { BLACK, WHITE };
public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [SerializeField] Slider m_strikerSlider;
    public Slider StrikerSlider { get { return m_strikerSlider; } set { m_strikerSlider = value; } }
    [SerializeField] private Image m_playerTimer;
    [SerializeField] private GameObject m_strikerPrefab, m_GameOverPanel;
    [SerializeField] private Transform m_strikerSpawnPoint;
    [SerializeField] private TMP_Text m_scorText, m_messageIndicator, m_messageIndicator1;

    private float m_currentTime, m_totalTime = 10f;
    private int m_totalScore;
    private bool m_stopTimer = false;
    private bool m_gameOver, m_canMoveStriker, m_canSetStrikerValue;
    public bool CanMoveStriker { get { return m_canMoveStriker; } set { m_canMoveStriker = value; } }
    public bool CanSetStrikerValue { get { return m_canSetStrikerValue; } set { m_canSetStrikerValue = value; } }


    [System.NonSerialized] public GameObject m_currentPrefab;

    public CircleCollider2D[] circleCollider2D;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(instance);

        m_strikerSlider.onValueChanged.AddListener(MoveStriker);
        SpawnStriker();
        m_canMoveStriker = true;
    }
    private void Update()
    {
        //if (!m_stopTimer) Timer();
        if (m_totalScore >= 120 && !m_gameOver)
            GameOver();
    }
    private void MoveStriker(float value)
    {
        if (m_currentPrefab != null )
        {
            m_currentPrefab.GetComponent<CircleCollider2D>().isTrigger = true;
            m_currentPrefab.transform.position = new Vector3(value, m_strikerSpawnPoint.position.y, 0);
        }
    }
    private void Timer()
    {
        if (m_currentTime <= m_totalTime)
        {
            m_currentTime += Time.deltaTime;
            m_playerTimer.fillAmount = m_currentTime / m_totalTime;
        }
        else
        {
            Striker.m_chance = false;
        }
    }

    private void GameOver()
    {
        m_stopTimer = true;
        m_playerTimer.fillAmount = 0;
        m_GameOverPanel.SetActive(true);
        m_gameOver = true;
        Debug.Log("GameOver");
    }

    public void SpawnStriker()
    {
        m_strikerSlider.value = 0;
        m_currentPrefab = Instantiate(m_strikerPrefab, m_strikerSpawnPoint);
        m_strikerSlider.interactable = true;

        ResetTimer();
        foreach (var item in circleCollider2D)
            item.enabled = true;
    }
    public void ResetTimer()
    {

        m_currentTime = 0;
        m_stopTimer = false;
        m_playerTimer.fillAmount = m_currentTime / m_totalTime;
    }
    public void IncrementScore(int no)
    {
        if (m_totalScore <= 120)
        {
            m_messageIndicator.text = "+" + no.ToString();
            StartCoroutine(ResetMessage(1f, m_messageIndicator));
            m_totalScore += no;
            m_scorText.text = m_totalScore.ToString() + " / " + "120";
        }
    }


    public void SetStrikerValue()
    {
        if (m_canSetStrikerValue)
        {

            m_strikerSlider.value = 0;
            m_currentPrefab.transform.position = new Vector3(m_strikerSlider.value, m_strikerSpawnPoint.position.y, 0);
            m_currentPrefab.GetComponent<SpriteRenderer>().color = Color.white;

            m_messageIndicator1.text = "Striker And Token Are Overlapping";
            StartCoroutine(ResetMessage(2f, m_messageIndicator1));
        }
    }

    public void DecrementScore(int no)
    {
        if (m_totalScore > 0)
        {
            m_messageIndicator.text = "-" + no.ToString();
            StartCoroutine(ResetMessage(1f, m_messageIndicator));

            m_totalScore -= no;
            m_scorText.text = m_totalScore.ToString() + " / " + "120";
        }
    }
    public void StopTimer() => m_stopTimer = true;

    IEnumerator ResetMessage(float time, TMP_Text text)
    {
        yield return new WaitForSeconds(time);
        text.text = "";
    }
}
