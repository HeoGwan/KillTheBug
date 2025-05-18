using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CESCO;


// ���� ���� â
public class BuyItem : MonoBehaviour
{
    public Image buyItemImage;
    public TextMeshProUGUI ItemName;
    public TextMeshProUGUI ItemRate;
    public TextMeshProUGUI ItemRadius;
    public TextMeshProUGUI ItemSpeed;
    public TextMeshProUGUI ItemPrice;
    public TextMeshProUGUI ItemInfo;
    public GameObject toolInfoObj;

    private TOOL itemToolType;
    private int itemPrice;
    private bool isShowInfo = false;

    private void Awake()
    {
        toolInfoObj.SetActive(isShowInfo);
    }

    public void Init(GameObject buyItemObj)
    {
        itemToolType = buyItemObj.GetComponent<ShopItem>().ToolType;
        itemPrice = buyItemObj.GetComponent<ShopItem>().ItemPrice;

        // buyItem�� ù��° �ڽ��� ItemImage�̴�.
        // buyItem.transform.GetChild(0).gameObject.GetComponent<Image>()
        // �����Ϸ��� �������� �̹��� ����
        Tool toolInfo = buyItemObj.GetComponent<ShopItem>().ToolInfo;

        buyItemImage.sprite = toolInfo.ToolImage;

        // buyItem.GetComponent<Tool>()
        // �����Ϸ��� �������� ����
        ItemName.text = toolInfo.ToolName;
        ItemRate.text = toolInfo.GetRateText();
        ItemRadius.text = toolInfo.GetRadiusText();
        ItemSpeed.text = toolInfo.GetSpeedText();
        ItemPrice.text = itemPrice + "��";
        ItemInfo.text = toolInfo.ToolInfo;
    }

    public void Buy()
    {
        // ���� ��ư�� �����
        GameManager.instance.shopManager.BuyItem(itemToolType, itemPrice);
    }

    public void Cancel()
    {
        // ��� ��ư�� �����
        gameObject.SetActive(false);
    }

    public void ShowToolInfo()
    {
        isShowInfo = !isShowInfo;
        toolInfoObj.SetActive(isShowInfo);
    }
}
