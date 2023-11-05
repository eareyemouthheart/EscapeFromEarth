using UnityEngine;
using System.Collections;

public class GameFinishFlag : MonoBehaviour, ITriggerPlayer
{
    public SpriteRenderer spriteRenderer;
    public Sprite checkedImage;

    public void OnTrigger()
    {
        if (GameManager.Instance.State != GameManager.GameState.Playing)
            return;

        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer)
            spriteRenderer.sprite = checkedImage;

        GameManager.Instance.GameFinish();
    }
}
