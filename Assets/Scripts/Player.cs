using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using CESCO;
using UnityEngine.UI;
using UnityEditor;

/*
 * ��� �����ִ� ������ ��� �� �ٸ� ������ ��� ��� �� ���� �߻�
 * �̺�Ʈ �����ʸ� ������ �ʾ� �߻��� ����
*/

public class Player : MonoBehaviour
{
    [SerializeField] private TOOL toolIndex = TOOL.HAND;
    [SerializeField] private GameObject joyStickObj;
    [SerializeField] private Button hitButton;
    [SerializeField] private GameObject showSelectTools;
    [SerializeField] private GameObject showHitPos;
    [SerializeField] private GameObject showToolsButton;
    public GameObject toolListObj;

    private JoyStick joyStick;
    [SerializeField] private List<GameObject> hasTools;
    [SerializeField] private List<TOOL> hasToolTypes;
    private GameObject selectTool = null;
    private Tool curTool;

    private int money;
    private EventTrigger.Entry buttonDown;
    private EventTrigger.Entry buttonUp;

    private bool[] isTouch;

    public JoyStick JoyStick { get { return joyStick; } }

    public List<GameObject> HasTools { get { return hasTools; } }

    public int Money { get { return money; } set { money = value; } }

    public GameObject CurrentSelectTool { get { return selectTool; } }

    public GameObject CurrentHitPos { get { return showHitPos; } }

    void Awake()
    {
        isTouch = new bool[]{ false, false };
        hasTools = new List<GameObject>();
        hasToolTypes = new List<TOOL>();
        money = 0;

        buttonDown = new EventTrigger.Entry();
        buttonDown.eventID = EventTriggerType.PointerDown;
        buttonUp = new EventTrigger.Entry();
        buttonUp.eventID = EventTriggerType.PointerUp;

        ChangeTool();

        joyStick = joyStickObj.GetComponent<JoyStick>();

        showSelectTools.SetActive(false);
        showHitPos.SetActive(false);
        joyStickObj.SetActive(false);
    }

    private void OnEnable()
    {
        if (GameManager.instance.GameState == GAME_STATE.RUNNING)
        {
            toolIndex = TOOL.HAND;
            BringTool();
            transform.position = Vector3.zero;
            showHitPos.SetActive(true);
        }
    }

    private void OnDisable()
    {
        if (GameManager.instance.GameState == GAME_STATE.START) Init();
    }

    public void Init()
    {
        // �÷��̾� ���� �ʱ�ȭ
        transform.position = Vector3.zero;
        hasTools.Clear();
        hasToolTypes.Clear();
        money = 0;
        showSelectTools.SetActive(false);
        showHitPos.SetActive(false);
        showToolsButton.GetComponent<ShowToolsButton>().Init();
        selectTool = null;

        // ������ �ִ� ���� ����
        int hasToolsCount = toolListObj.transform.childCount;
        for (int i = 0; i < hasToolsCount; ++i)
        {
            //Destroy(toolListObj.transform.GetChild(i).gameObject);
            GameObject returnTool = toolListObj.transform.GetChild(0).gameObject;
            GameManager.instance.toolManager.ReturnTool(returnTool);
        }

        for (int i = 0; i < showSelectTools.transform.childCount; ++i)
        {
            GameObject toolButton = showSelectTools.transform.GetChild(i).gameObject;
            toolButton.GetComponent<Button>().onClick.RemoveAllListeners();
            GameManager.instance.prefabManager.PutBackObj(toolButton);
        }
    }
    
    public void BringTool()
    {
        // toolManager���Լ� ������ �޾ƿ��� shopManager���� ������ �޾ƿ� ����� �˷���
        GetTool(toolIndex);
        GameManager.instance.shopManager.UnLockTool(toolIndex); // shopManager���� ������ �ִٰ� �˷���
        SelectTool();
    }

    public GameObject GetTool(TOOL tool, GameObject toolPref=null)
    {
        GameObject toolObj = toolPref == null ? GameManager.instance.toolManager.GiveTool(tool) : toolPref;
        // � �������� ������ ���޹��� �� �ش� ������ �ν��Ͻ�ȭ �ϴ� ����
        toolObj.transform.SetParent(toolListObj.transform);

        // �ش� ������ �÷��̾ ������ �ִٴ� ���� �˷���
        toolObj.GetComponent<Tool>().HasPlayer();
        
        // ��ȭ�� �� � ������ ������ �ִ��� �˷���
        GameManager.instance.reinforceManager.AddTool(toolObj);

        // �÷��̾ ������ �ִ� ������ �����Ѵ�.
        hasTools.Add(toolObj);
        hasToolTypes.Add(tool);

        // �÷��̾ ������ �� �ֵ��� ��ư�� �߰��Ѵ�.
        GameObject toolButton = GameManager.instance.prefabManager.GetSelectTool();
        toolButton.transform.SetParent(showSelectTools.transform);
        toolButton.transform.localScale = Vector3.one;

        // ��ư �̹��� ����
        toolButton.transform.GetChild(0).GetComponent<Image>().sprite =
            toolObj.GetComponent<Tool>().ToolImage;

        // ��ư Ŭ�� �� �ش� ������ ����
        toolButton.GetComponent<Button>().onClick.AddListener(() =>
        {
            if (GameManager.instance.GameState == GAME_STATE.RUNNING) { SelectTool(tool); }
        });

        return toolObj;
    }

    void ChangeTool()
    {
        EventTrigger hitButtonTrigger = hitButton.GetComponent<EventTrigger>();
        hitButtonTrigger.triggers.Clear();

        buttonDown.callback.AddListener((data) =>
        {
            if (GameManager.instance.GameState == GAME_STATE.RUNNING)
            {
                curTool.HitButtonDown();
            }
        });
        buttonUp.callback.AddListener((data) =>
        {
            if (GameManager.instance.GameState == GAME_STATE.RUNNING)
            {
                curTool.HitButtonUp();
            }
        });

        hitButtonTrigger.triggers.Add(buttonDown);
        hitButtonTrigger.triggers.Add(buttonUp);
    }
    
    void SelectTool(Vector3 prevPos)
    {
        if (selectTool != null)
        {
            selectTool.SetActive(false);
        }

        ShowHitPos hitPos = showHitPos.GetComponent<ShowHitPos>();

        //int index = hasToolTypes.IndexOf(toolIndex);
        int index = hasToolTypes.Count - 1;
        selectTool = hasTools[index];

        curTool = selectTool.GetComponent<Tool>();
        curTool.ChangePos(prevPos);
        curTool.SetTool();
        selectTool.SetActive(true);

        // Hit ��ư�� ���� ����
        ChangeTool();

        // ������ ������ Hit ��ǥ�� ������
        hitPos.SetCurTool(selectTool);
        // ������ ����Ǿ��ٴ� ���� �˷���
        hitPos.ToolChange(curTool);
    }
    
    void SelectTool()
    {
        Vector3 prevPos = Vector3.zero;
        if (selectTool != null)
        {
            prevPos = selectTool.transform.position;
            selectTool.SetActive(false);
        }

        ShowHitPos hitPos = showHitPos.GetComponent<ShowHitPos>();

        int index = hasToolTypes.Count - 1;
        selectTool = hasTools[index];

        curTool = selectTool.GetComponent<Tool>();
        curTool.ChangePos(prevPos);
        curTool.SetTool();
        selectTool.SetActive(true);

        // Hit ��ư�� ���� ����
        ChangeTool();

        // ������ ������ Hit ��ǥ�� ������
        hitPos.SetCurTool(selectTool);
        // ������ ����Ǿ��ٴ� ���� �˷���
        hitPos.ToolChange(curTool);
    }
    
    void SelectTool(TOOL tool)
    {
        if (curTool.ToolType == tool) return;

        Vector3 prevPos = Vector3.zero;
        if (selectTool != null)
        {
            prevPos = selectTool.transform.position;
            selectTool.SetActive(false);
        }

        ShowHitPos hitPos = showHitPos.GetComponent<ShowHitPos>();

        int index = hasToolTypes.IndexOf(tool);
        selectTool = hasTools[index];

        curTool = selectTool.GetComponent<Tool>();
        curTool.ChangePos(prevPos);
        curTool.SetTool();
        selectTool.SetActive(true);

        // Hit ��ư�� ���� ����
        ChangeTool();

        // ������ ������ Hit ��ǥ�� ������
        hitPos.SetCurTool(selectTool);
        // ������ ����Ǿ��ٴ� ���� �˷���
        hitPos.ToolChange(curTool);
    }

    public void ChangeMoney(int money)
    {
        this.money += money;
    }

    public List<GameObject> GetHasTools()
    {
        return hasTools;
    }

    public bool hasTool(TOOL tool)
    {
        return hasToolTypes.IndexOf(tool) != -1 ? true : false;
    }

    public void Hit()
    {
        curTool.Hit();
    }
    
    public void CantUse(GameObject toolObj)
    {
        // ������ ���̻� ����� �� �����Ƿ� �ش� ������ ������ ���� ���� ���·� ����� ������ �̸� �˷��ش�
        TOOL tool = toolObj.GetComponent<Tool>().ToolType;
        Vector3 pos = toolObj.transform.position;
        int index = hasToolTypes.IndexOf(tool);
#if UNITY_EDITOR
        print("index: " + index);
#endif

        // ������ �ִ� �������� ����
        hasToolTypes.Remove(tool);
        hasTools.RemoveAt(index);

        // ���� ������ �ִ� ������ ������Ʈ ����
        GameManager.instance.toolManager.ReturnTool(toolListObj.transform.GetChild(index).gameObject);

        // ���� ���� ��ư�� ����鼭 �߰��� �̺�Ʈ �����ʸ� �����ش�.
        GameObject toolButton = showSelectTools.transform.GetChild(index).gameObject;
        toolButton.GetComponent<Button>().onClick.RemoveAllListeners();
        GameManager.instance.prefabManager.PutBackObj(toolButton);

        // ������ ��ȭ �Ŵ������� ������ ����� �� ���ٴ� ���� �˸�
        GameManager.instance.shopManager.LockTool(tool);
        GameManager.instance.reinforceManager.RemoveTool(toolObj);

        SelectTool(pos);
    }
}