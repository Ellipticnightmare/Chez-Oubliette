using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NavigationManager : MonoBehaviour
{
    public ScrollRect MyScrollRect;
    public RectTransform dayUI, nightUI, pauseUI;
    public HelpManager mana;
    private void Awake()
    {
        SwapToDay();
        if (!PlayerPrefs.HasKey("loadedBefore"))
            SwapToTutorial();
    }
    public void ScrollToPos(RectTransform childMov)
    {
        if (childMov == pauseUI)
            Time.timeScale = 0;
        else
            Time.timeScale = 1;
        if (childMov != dayUI || (childMov == dayUI && FindObjectOfType<GameManager>().dayTimer > 0))
            MyScrollRect.content.localPosition = MyScrollRect.GetSnapToViewChild(childMov);
    }
    public void SwapToDay()
    {
        MyScrollRect.content.localPosition = MyScrollRect.GetSnapToViewChild(dayUI);
    }
    public void SwapToNight()
    {
        MyScrollRect.content.localPosition = MyScrollRect.GetSnapToViewChild(nightUI);
    }
    public void SwapToTutorial()
    {
        MyScrollRect.content.localPosition = MyScrollRect.GetSnapToViewChild(pauseUI);
        PlayerPrefs.SetString("loadedBefore", "DO I really matter?");
        mana.OpenHelp();
        mana.i = 0;
    }
}
public static class ScrollRectExtensions
{
    public static Vector2 GetSnapToViewChild(this ScrollRect instance, RectTransform child)
    {
        Canvas.ForceUpdateCanvases();
        Vector2 viewportLocalPosition = instance.viewport.localPosition;
        Vector2 childLocalPosition = child.localPosition;
        Vector2 result = new Vector2(
            0 - (viewportLocalPosition.x + childLocalPosition.x),
            0 - (viewportLocalPosition.y + childLocalPosition.y));
        return result;
    }
}