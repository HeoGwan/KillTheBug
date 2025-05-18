using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using CESCO;
using UnityEngine.UI;
using UnityEngine.Events;

// ���� â
public class Shop : MonoBehaviour
{
    public TextMeshProUGUI showPlayerMoney;
    public GameObject ToolObjs; // ���� ���õǴ�(���� �ϴ�) �������� ��Ƴ��� ������Ʈ
    public GameObject BuyItemObj;
    public ShopItem[] Items; // ���̽��� �Ǵ� ������

    private List<GameObject> shopItems;

    private void Start()
    {
#if UNITY_EDITOR
        print("Shop ȣ��");
#endif
        shopItems = new List<GameObject>();
        GetItems();
    }

    public void Init()
    {
        shopItems.Clear();
        foreach (Transform child in ToolObjs.transform)
        {
            //Destroy(child.gameObject);
            GameManager.instance.prefabManager.PutBackObj(child.gameObject);
        }
        GetItems();
    }

    public void GetItems()
    {
        foreach (ShopItem item in Items)
        {
            //GameObject shopItem = Instantiate(item, ToolObjs.transform);

            //Instantiate(LockObj, shopItem.transform);

            GameObject shopItem = GameManager.instance.prefabManager.GetShopItem(item);
            shopItem.transform.SetParent(ToolObjs.transform);
            shopItem.transform.localScale = Vector3.one;

            shopItem.GetComponent<Button>().onClick.AddListener(() =>
            {
                GameManager.instance.soundManager.UIEffectPlay(1);
                BuyItemObj.GetComponent<BuyItem>().Init(shopItem);
                BuyItemObj.SetActive(true);
            });

            shopItems.Add(shopItem);
        }
    }

    public void UnLock(TOOL tool)
    {
        shopItems[(int)tool].GetComponent<ShopItem>().UnLock();
        shopItems[(int)tool].transform.GetChild(1).gameObject.SetActive(false); // Lock �̹��� ����
        shopItems[(int)tool].GetComponent<Button>().onClick.RemoveAllListeners(); // ��ư �̺�Ʈ ����
    }

    public void Lock(TOOL tool)
    {
        shopItems[(int)tool].GetComponent<ShopItem>().Lock();
        shopItems[(int)tool].transform.GetChild(1).gameObject.SetActive(true); // Lock �̹��� ������
        shopItems[(int)tool].GetComponent<Button>().onClick.AddListener(() =>
        {
            BuyItemObj.GetComponent<BuyItem>().Init(shopItems[(int)tool]);
            BuyItemObj.SetActive(true);
        }); // ��ư �߰�
    }

    public void ShowShopInfo()
    {
        ShowItems();
    }

    public void ShowMoney()
    {
        showPlayerMoney.text = GameManager.instance.CurrentPlayer.Money + "��";
    }

    public void ShowItems()
    {
        ShowMoney();
        BuyItemObj.SetActive(false);
    }
}
