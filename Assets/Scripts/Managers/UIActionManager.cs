using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PicoGames.Panes;

public class UIActionManager : MonoBehaviour
{
    public void LoadGameScene()
    {
        SoundManager.Instance.PlayEffect("uiclick");
        UIPaneManager.Hide("mainmenu");
    }
}
