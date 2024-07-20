using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TokenHoles : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("WhiteToken"))
        {
            WhiteToken(other.gameObject, "White");
        }
        else if (other.CompareTag("BlackToken"))
        {
            BlackToken(other.gameObject, "Black");
        }
        else if (other.CompareTag("RedToken"))
        {
            RedToken(other.gameObject, "Red");
        }
        else if (other.CompareTag("Striker"))
        {
            Striker(other.gameObject, "Striker");
        }
    }
    private void WhiteToken(GameObject go, string name)
    {
        GameManager.instance.IncrementScore(20);
        Common(go, name);
        Debug.Log("WhiteEntered");
    }
    private void BlackToken(GameObject go, string name)
    {
        GameManager.instance.IncrementScore(10);
        Common(go, name);
        Debug.Log("BlackEntered");
    }
    private void RedToken(GameObject go, string name)
    {
        GameManager.instance.IncrementScore(50);
        Common(go, name);
        Debug.Log("RedEntered");
    }
    private void Striker(GameObject go,string name)
    {
        Debug.Log("WhiteEntered and its a foul");
        GameManager.instance.DecrementScore(10);
        StartCoroutine(AnimatePos(go, name));
    }
    private void PlayAnimation(GameObject go, string name)
    {
        go.GetComponent<Animator>().Play("InHole_" + name);
    }
    private void Common(GameObject go, string name)
    {
        go.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
        go.GetComponent<CircleCollider2D>().enabled = false;
        StartCoroutine(AnimatePos(go, name));
    }

    IEnumerator AnimatePos(GameObject go, string name)
    {
        float timeelapsed = 0f;
        float duration = 0.3f;

        Vector3 initailPos = go.transform.position;
        Vector3 toPos = transform.position;

        while (timeelapsed < duration)
        {
            if (go != null)
            {
                go.transform.position = Vector3.Lerp(initailPos, toPos, timeelapsed / duration);
                timeelapsed += Time.deltaTime;
            }
            yield return null;
        }

        PlayAnimation(go, name);
        if(name == "Striker")
        {
            GameManager.instance.SpawnStriker();
            Destroy(go);
        }
    }

}
