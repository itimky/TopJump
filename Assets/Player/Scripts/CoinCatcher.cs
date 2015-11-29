using UnityEngine;
using System.Collections;

public class CoinCatcher : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag(TagManager.Coin))
            CoinManager.AddGold(other.gameObject);
    }
}
