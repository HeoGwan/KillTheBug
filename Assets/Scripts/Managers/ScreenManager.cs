using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CESCO;

public class ScreenManager : MonoBehaviour
{
    private Stack<SCREEN> ScreenStack;

    public SCREEN CurrentScreen() => ScreenStack.Peek();

    private void Awake()
    {
        ScreenStack = new Stack<CESCO.SCREEN>();
        Init();
    }

    public void Init()
    {
        ScreenStack.Clear();
        ScreenStack.Push(SCREEN.START);
        BGMDelayed(ScreenStack.Peek(), 0.8f);
    }

    void BGMDelayed(SCREEN screen, float delay)
    {
        if (screen == SCREEN.START || screen == SCREEN.INGAME)
        {
            // BGM ���
            GameManager.instance.soundManager.BGMPlay(screen, delay);
        }
        else
        {
            // BGM ���
            //GameManager.instance.soundManager.BGMStop();
        }
    }

    void BGM(SCREEN screen)
    {
        if (screen == SCREEN.START || screen == SCREEN.INGAME)
        {
            // BGM ���
            GameManager.instance.soundManager.BGMPlay(screen);
        }
        else
        {
            // BGM ���
            //GameManager.instance.soundManager.BGMStop();
        }
    }

    public void ChangeScreen(SCREEN screen)
    {
        // �ش� ȭ�� ����
        ScreenStack.Push(screen);
        // UI ���
        GameManager.instance.uiManager.ActiveUI(screen);
        GameManager.instance.backgroundManager.ChangeBackground();
        //BGM(ScreenStack.Peek());
        
        if (screen == SCREEN.INGAME)
        {
            // BGM ���
            GameManager.instance.soundManager.RandomBGMPlay();
        }
        else if (screen == SCREEN.GAMERULE)
        {
            GameManager.instance.soundManager.PlayRule();
        }

        if (ScreenStack.Contains(SCREEN.INGAME))
        {
            GameManager.instance.uiManager.ActiveGlobalUI();
        }
    }

    public void PrevScreen()
    {
        ScreenStack.Pop();
        // UI ����
        GameManager.instance.uiManager.InActiveUI();
        GameManager.instance.backgroundManager.ChangeBackground();

        if (GameManager.instance.GameState == GAME_STATE.PAUSE) { return; }
        if (ScreenStack.Peek() == SCREEN.PAY) { GameManager.instance.payManager.ShowMoney(); }
        BGM(ScreenStack.Peek());
    }

    public void GoMain()
    {
        Init();
        GameManager.instance.uiManager.GoMain();
        GameManager.instance.backgroundManager.ChangeBackground();
    }
}