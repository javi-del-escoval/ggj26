using UnityEngine;
using UnityEngine.UI;

public class MenuButton : MonoBehaviour
{
    public MenuController controller;
    public MenuAction action;
    public Image targetImage;

    public void Invoke()
    {
        if (controller == null) return;
        controller.OnButtonPressed(action);
    }

    public void ApplySprite(Sprite sprite)
    {
        if (sprite == null) return;

        if (targetImage == null)
        {
            targetImage = GetComponent<Image>();
        }

        if (targetImage != null) targetImage.sprite = sprite;
    }
}
