using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Corrupted : MonoBehaviour
{
    public Sprite milfPlacedSprite;

    public void MilfPlaced()
    {
        var mainSprite = transform.GetChild(0).GetComponent<SpriteRenderer>();
        mainSprite.sprite = milfPlacedSprite;
    }
}
