using CESCO;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

/*
 * 벌레들의 이동속도가 느려지는 설치기
 * 사용할 수 있는 개수가 정해져 있으며 사용 개수를 다 사용하면
 * 다시 상점에서 구매해야 한다
*/

public class Trap : Tool
{
    [SerializeField] private float stopDuration;
    [SerializeField] private Sprite hitObjImage;

    [Header("▼ 사용량 정보")]
    [SerializeField] private int maxCount;
    [SerializeField] private GameObject showUseCanvas;
    [SerializeField] private TextMeshProUGUI showUseText;
    [SerializeField] private TextMeshProUGUI showMaxUseText;
    [SerializeField] private float xOffset;
    [SerializeField] private float yOffset;

    private int count = 1;

    private WaitForSeconds waitCantDelay;

    private new void Awake()
    {
        base.Awake();

        waitCantDelay = new WaitForSeconds(0.2f);

        // 남은 사용량 알려주는 텍스트 초기화
        showUseCanvas.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceCamera;
        showUseCanvas.GetComponent<Canvas>().worldCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
        showMaxUseText.text = "/ " + maxCount;
        showUseText.text = (maxCount - count + 1).ToString();

        float[] radiusValue = { 1.2f, 1.6f, 2f };
        SetRadiusValue(radiusValue);
    }

    public override void Move(Vector3 movePosition)
    {
        base.Move(movePosition);

        showUseText.transform.position = new Vector2(transform.position.x - xOffset, transform.position.y + yOffset);
        showMaxUseText.transform.position = new Vector2(transform.position.x + xOffset, transform.position.y + yOffset);
    }

    public override void Hit()
    {
        if (!canHit) return;

        // 때린 장소 표시 오브젝트 생성
        GameObject showHitObj = GameManager.instance.prefabManager.GetHit(HIT_OBJ_TYPE.SHOW_HIT);
        HitObjScript showHit = showHitObj.GetComponent<HitObjScript>();
        // 벌레 잡았는지 확인하는 오브젝트 생성
        GameObject checkHitObj = GameManager.instance.prefabManager.GetHit(HIT_OBJ_TYPE.CHECK_HIT);
        HitCheckScript checkHit = checkHitObj.GetComponent<HitCheckScript>();

        // 오브젝트 설정 변경
        showHit.ChangeInfo(radius, stopDuration);
        checkHit.ChangeInfo(radius, damage, tool, stopDuration, 0, false);

        showHit.ChangeImage(hitObjImage);
        checkHit.ChangeImage(hitObjImage);

        // 도구 위치로 오브젝트 이동
        showHit.Show(transform.position);
        checkHit.Show(transform.position);

        // 때리기 쿨타임
        canHit = false;
        StartCoroutine(HitDelay());

        // 때리는 애니메이션 재생
        GameManager.instance.CurrentPlayer.CurrentHitPos.GetComponent<ShowHitPos>().PlayHitAnimation();

        // 때리는 소리 재생
        //GameManager.instance.soundManager.EffectPlay(tool);

        // 플레이어에게 더이상 사용할 수 없음을 알려줌
        if (count >= maxCount)
        {
            StartCoroutine(CantUse());
            count = 1;
        }
        else
        {
            showUseText.text = (maxCount - count).ToString();
            ++count;
        }
    }

    IEnumerator CantUse()
    {
        yield return waitCantDelay;
        GameManager.instance.CurrentPlayer.CantUse(gameObject);
    }
}
