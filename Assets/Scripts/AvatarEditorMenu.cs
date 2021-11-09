using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvatarEditorMenu : MonoBehaviour
{
    public ColorCarousel primaryColorCarousel;
    public ColorCarousel secondaryColorCarousel;
    public RangeSelector eyesSelector;
    public RangeSelector mouthSelector;

    public MenuAvatarManager menuAvatarManager;

    private void Start()
    {
        primaryColorCarousel.SelectColor(UserSettings.GetInstance().PrimaryAvatarColor);
        secondaryColorCarousel.SelectColor(UserSettings.GetInstance().SecondaryAvatarColor);
        eyesSelector.SetValue(UserSettings.GetInstance().AvatarEyes);
        mouthSelector.SetValue(UserSettings.GetInstance().AvatarMouth);
    }

    private void OnEnable()
    {
        menuAvatarManager.SetVisible(true);
    }

    private void OnDisable()
    {
        if(menuAvatarManager != null)
            menuAvatarManager.SetVisible(false);
    }
}
