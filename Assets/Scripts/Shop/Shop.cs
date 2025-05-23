using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using CESCO;
using UnityEngine.UI;
using UnityEngine.Events;

// 상점 창
public class Shop : MonoBehaviour
{
    public TextMeshProUGUI showPlayerMoney;
    public GameObject ToolObjs; // 실제 전시되는(구매 하는) 아이템을 모아놓은 오브젝트
    public GameObject BuyItemObj;
    public ShopItem[] Items; // 베이스가 되는 도구들

    private List<GameObject> shopItems;

    private void Start()
    {
#if UNITY_EDITOR
        print("Shop 호출");
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
        shopItems[(int)tool].transform.GetChild(1).gameObject.SetActive(false); // Lock 이미지 삭제
        shopItems[(int)tool].GetComponent<Button>().onClick.RemoveAllListeners(); // 버튼 이벤트 삭제
    }

    public void Lock(TOOL tool)
    {
        shopItems[(int)tool].GetComponent<ShopItem>().Lock();
        shopItems[(int)tool].transform.GetChild(1).gameObject.SetActive(true); // Lock 이미지 보여줌
        shopItems[(int)tool].GetComponent<Button>().onClick.AddListener(() =>
        {
            BuyItemObj.GetComponent<BuyItem>().Init(shopItems[(int)tool]);
            BuyItemObj.SetActive(true);
        }); // 버튼 추가
    }

    public void ShowShopInfo()
    {
        ShowItems();
    }

    public void ShowMoney()
    {
        showPlayerMoney.text = GameManager.instance.CurrentPlayer.Money + "원";
    }

    public void ShowItems()
    {
        ShowMoney();
        BuyItemObj.SetActive(false);
    }
}
