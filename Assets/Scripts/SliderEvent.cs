using UnityEngine;
using UnityEngine.EventSystems;
public class SliderEvent : MonoBehaviour, IPointerUpHandler
{
    public void OnPointerUp(PointerEventData eventData)
    {
        GameManager gm = GameManager.instance;

        if (gm.m_currentPrefab != null)
        {
            gm.SetStrikerValue();
            gm.m_currentPrefab.GetComponent<CircleCollider2D>().isTrigger = false;
        }
    }
}
