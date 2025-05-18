using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CESCO;
using UnityEngine.Device;

public class UIManager : MonoBehaviour
{
    public GameObject start;
    public GameObject inGame;
    public GameObject pause;
    public GameObject setting;
    public GameObject pay;
    public GameObject reinforce;
    public GameObject shop;
    public GameObject gameOver;
    public GameObject gameRule;
    public GameObject clear;

    [Header("�� Global Button")]
    public GameObject playerButton;

    private List<GameObject> GameUI;
    private Stack<GameObject> GameUIStack;
    [SerializeField] private GameObject GamePauseButton;
        
    void Awake()
    {
        GameUI = new List<GameObject>();
        GameUIStack = new Stack<GameObject>();

        GameUI.Insert((int)SCREEN.START, start);
        GameUI.Insert((int)SCREEN.INGAME, inGame);
        GameUI.Insert((int)SCREEN.PAUSE, pause);
        GameUI.Insert((int)SCREEN.SETTING, setting);
        GameUI.Insert((int)SCREEN.PAY, pay);
        GameUI.Insert((int)SCREEN.REINFORCE, reinforce);
        GameUI.Insert((int)SCREEN.SHOP, shop);
        GameUI.Insert((int)SCREEN.GAMEOVER, gameOver);
        GameUI.Insert((int)SCREEN.GAMERULE, gameRule);
        GameUI.Insert((int)SCREEN.CLEAR, clear);

        // ��� UI�� ��Ȱ��ȭ �� ��
        foreach(GameObject gameUI in GameUI)
        {
            gameUI.SetActive(false);
        }

        // ���� �޴� UI�� Ȱ��ȭ
        Init();
    }

    private void Init()
    {
        GameUIStack.Clear();
        GameUIStack.Push(GameUI[(int)SCREEN.START]);
        GameUIStack.Peek().SetActive(true);
        GamePauseButton.SetActive(false);

        // �÷��̾� ���� ��ư ��Ȱ��ȭ
        playerButton.SetActive(false);
    }

    public void ActiveUI(SCREEN screen)
    {
        // ���� �ֱٿ� ������ UI ��Ȱ��ȭ
        GameUIStack.Peek().SetActive(false);

        // ������ UI Ȱ��ȭ
        GameUIStack.Push(GameUI[(int)screen]);
        GameUIStack.Peek().SetActive(true);
        if (GameManager.instance.GameState != GAME_STATE.START) GamePauseButton.SetActive(true);
        else GamePauseButton.SetActive(false);

        if (screen == SCREEN.PAY) { GameManager.instance.adMobManager.ShowAd(); }
        else { GameManager.instance.adMobManager.HideAd(); }
    }

    public void InActiveUI()
    {
        // ���� �ֱٿ� ������ UI ��Ȱ��ȭ �� ���� ����
        GameObject recentUI = GameUIStack.Pop();
        recentUI.SetActive(false);
        if (recentUI == GameUI[(int)SCREEN.PAY]) { GameManager.instance.adMobManager.HideAd(); }

        GameUIStack.Peek().SetActive(true);
        if (GameUIStack.Peek() == GameUI[(int)SCREEN.PAY]) { GameManager.instance.adMobManager.ShowAd(); }

        if (GameManager.instance.GameState != GAME_STATE.START) GamePauseButton.SetActive(true);
        else GamePauseButton.SetActive(false);
    }

    public void GoMain()
    {
        // ���� ȭ������ ���ư� �� ���� ���� ��
        // �ֱٿ� ��µ� UI ��Ȱ��ȭ ��
        // UI�� ����� ���� �ʱ�ȭ
        if (GameUIStack.Contains(GameUI[(int)SCREEN.PAY])) { GameManager.instance.adMobManager.HideAd(); }
        GameUIStack.Peek().SetActive(false);

        // ȭ��, UI �ʱ�ȭ
        Init();
    }

    public void ActiveGlobalUI()
    {
        if (!playerButton.activeSelf) playerButton.SetActive(true);
    }
}
