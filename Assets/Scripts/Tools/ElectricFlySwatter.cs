using CESCO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 * ���� �ĸ�ä : Hit ��ư�� ���� �ð����� ������ �̾��.
 * �������� ���� ���� ���� �ð��� �����ش�.
 * �������� ���� �� ���� �ð��� ä������.
*/

public class ElectricFlySwatter : Tool
{
    float attackTime = 0;
    float attackDurationTime = 0;
    [SerializeField] float maxAttackTime; // �ִ�� ������ ���� �� �ִ� �ð�
    [SerializeField] float attackDuration; // ������ ������ ���� �ֱ�

    [SerializeField] GameObject gaugeCanvas;
    [SerializeField] float yOffset;
    GameObject gaugeBGObj = null;
    GameObject gaugeObj = null;

    private new void Awake()
    {
        base.Awake();

        if (gaugeBGObj == null)
        {
            gaugeCanvas.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceCamera;
            gaugeCanvas.GetComponent<Canvas>().worldCamera = GameObject.Find("Main Camera").GetComponent<Camera>();

            gaugeBGObj = GameManager.instance.prefabManager.RequestInstantiate(OBJ_TYPE.TOOL_GAUGE_BG_IMAGE, gaugeCanvas.transform);
            gaugeObj = GameManager.instance.prefabManager.RequestInstantiate(OBJ_TYPE.TOOL_GAUGE_IMAGE, gaugeCanvas.transform);
        }
    }

    private new void OnEnable()
    {
        base.OnEnable();
        InitGauge();
    }

    private new void FixedUpdate()
    {
        base.FixedUpdate();

        gaugeBGObj.transform.position = gaugeObj.transform.position =
            (new Vector2(transform.position.x, transform.position.y + radius + yOffset));
    }

    private void Update()
    {
        if (!isPressed) { return; }

        attackTime += Time.deltaTime;
        attackDurationTime += Time.deltaTime;
        gaugeObj.GetComponent<Image>().fillAmount = attackTime / maxAttackTime;

        if (attackTime >= maxAttackTime)
        {
            // ���� ����
            StopAttack();
        }

        if (attackDurationTime >= attackDuration)
        {
            attackDurationTime = 0;
            // ���� ����
            ElectricAttack();
            GameManager.instance.soundManager.EffectPlay(tool);
        }
    }

    void InitGauge()
    {
        gaugeObj.GetComponent<Image>().fillAmount = 0;
    }

    void StopAttack()
    {
        // ������ ��Ÿ��
        canHit = false;
        StartCoroutine(HitDelay());
        GameManager.instance.CurrentPlayer.CurrentHitPos.GetComponent<ShowHitPos>().StopHitAnimation();

        isPressed = false;
        attackTime = 0;
        InitGauge();
    }

    void ElectricAttack()
    {
        // ���� ��Ҵ��� Ȯ���ϴ� ������Ʈ
        GameObject checkHitObj = GameManager.instance.prefabManager.GetHit(HIT_OBJ_TYPE.CHECK_HIT);
        checkHitObj.GetComponent<HitCheckScript>().ChangeInfo(radius, damage, tool, attackDuration);
        checkHitObj.GetComponent<HitCheckScript>().Show(transform.position);

        // ������ �ִϸ��̼� ���
        GameManager.instance.CurrentPlayer.CurrentHitPos.GetComponent<ShowHitPos>().PlayHitAnimation(tool, 0.1f);
    }

    public override void HitButtonDown()
    {
        isPressed = true;
        GameManager.instance.CurrentPlayer.CurrentHitPos.GetComponent<ShowHitPos>().StartFollowHitAnimation();
    }

    public override void HitButtonUp()
    {
       StopAttack();
    }
}
