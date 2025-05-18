using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Reinforce : MonoBehaviour
{
    /*
     * ��ȭ�� �ص� ���� ������ �ִ� ������ �ݿ��� �ȵ� -> Call by ������ ����
    */
    public TextMeshProUGUI showPlayerMoney;
    public GameObject toolDetail;
    public GameObject canSelectToolObj;
    public GameObject hasToolPrefab;

    private List<GameObject> hasPlayerTools;
    private ShowToolDetail showToolDetail;

    private void Awake()
    {
        hasPlayerTools = new List<GameObject>();

        showToolDetail = toolDetail.GetComponent<ShowToolDetail>();
    }

    public void AddTool(GameObject tool)
    {
        /*
         * ex) �Ǽո� ���� ä �ĸ�ä ���� �� �ĸ�ä�� ��ȭ�ϸ�
         * �Ǽյ� ���� ��ȭ�Ǵ� ���� �߻�
        */
        //GameObject showTool = Instantiate(hasToolPrefab, canSelectToolObj.transform);

        GameObject showTool = GameManager.instance.prefabManager.GetHasTool();
        showTool.transform.SetParent(canSelectToolObj.transform);
        showTool.transform.localScale = Vector3.one;

        showTool.transform.GetChild(0).GetComponent<Image>().sprite =
            tool.GetComponent<SpriteRenderer>().sprite;

        showTool.GetComponent<Button>().onClick.AddListener(() =>
        {
            toolDetail.GetComponent<ShowToolDetail>().SelectTool(tool);
            GameManager.instance.soundManager.UIEffectPlay(1);
        });


        hasPlayerTools.Add(tool);
    }

    public void RemoveTool(GameObject tool)
    {
        int index = hasPlayerTools.IndexOf(tool);
        hasPlayerTools.Remove(tool);
        GameManager.instance.prefabManager.PutBackObj(canSelectToolObj.transform.GetChild(index).gameObject);
    }

    public void ShowMoney()
    {
        showPlayerMoney.text = GameManager.instance.CurrentPlayer.Money + "��";
    }

    public Price ShowToolInfo()
    {
        showToolDetail.ShowAttributes();
        return showToolDetail.ShowPrices();
    }

    public void ShowWindow()
    {
        ShowMoney();

        if (hasPlayerTools.Count > 0)
        {
            GameObject selectTool = hasPlayerTools[0];
            showToolDetail.SelectTool(selectTool);
        }

        ShowToolInfo();
    }

    public void Init()
    {
        hasPlayerTools.Clear();
        foreach (Transform child in canSelectToolObj.transform)
        {
            GameManager.instance.prefabManager.PutBackObj(child.gameObject);
        }
    }
}
