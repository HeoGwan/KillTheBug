using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CESCO;
using UnityEditor;

public class Tool : MonoBehaviour
{
    #region ���� ����
    // ���� �ӵ�
    // �ſ� ����, ����, ����, ����, �ſ� ����
    //     1,     0.7,  0.5,  0.3,     0.1
    [SerializeField] protected float hitDelay = .5f;
    public float Delay { get { return hitDelay; } }
    [SerializeField] private TOOL_RATE toolRate;
    public TOOL_RATE ToolRate
    {
        get { return toolRate; }
        set { toolRate = value; }
    }

    [SerializeField] private int ratePrice;
    public int RatePrice { get { return ratePrice; } }

    // ����
    // ����, ����, ŭ
    //  0.5,  1,   1.5
    [SerializeField] protected float radius = 1f;
    public float Radius { get { return radius; } }
    [SerializeField] private TOOL_RADIUS toolRadius;
    public TOOL_RADIUS ToolRadius
    {
        get { return toolRadius; }
        set { toolRadius = value; }
    }

    [SerializeField] private int radiusPrice;
    public int RadiusPrice { get { return radiusPrice; } }

    // �̵� �ӵ�
    // �ſ� ����, ����, ����, ����, �ſ� ����
    //     0.5,    0.7,   1,   1.3,    1.5
    [SerializeField] protected float speed = 10f;
    public float Speed { get { return speed; } }
    [SerializeField] private TOOL_SPEED toolSpeed;
    public TOOL_SPEED ToolSpeed
    {
        get { return toolSpeed; }
        set { toolSpeed = value; }
    }

    [SerializeField] private int speedPrice;
    public int SpeedPrice { get { return speedPrice; } }

    [SerializeField] protected float damage;
    public float Damage { get { return damage; } }
    [SerializeField] private TOOL_DAMAGE toolDamage;
    public TOOL_DAMAGE ToolDamage
    { 
        get { return toolDamage; }
        set { toolDamage = value; }
    }
    // ������ �ִ� �ܰ�
    [SerializeField] private int maxDamageLevel;

    [SerializeField] private int damagePrice;
    public int DamagePrice { get { return damagePrice; } }


    // �ſ� ����, ����, ����, ����, �ſ� ����
    private float[] rates = { 1f, 0.7f, 0.5f, 0.3f, 0.1f };
    // ����, ����, ŭ
    private float[] radiuses = { 0.8f, 1.2f, 1.6f };
    // �ſ� ����, ����, ����, ����, �ſ� ����
    private float[] speeds = { 3f, 5f, 8f, 10f, 12f };


    [TextArea]
    [SerializeField] private string toolInfo;
    public string ToolInfo { get { return toolInfo; } }
    #endregion

    [SerializeField] protected TOOL tool;
    public TOOL ToolType
    {
        get { return tool; }
    }

    public bool hasPlayer = false;

    protected bool isPressed = false;
    protected bool canHit;
    public bool CanHit { get { return canHit; } }

    [SerializeField] private string toolName;
    public string ToolName
    {
        get { return toolName; }
    }
    [SerializeField] protected Sprite toolImage;
    public Sprite ToolImage { get { return toolImage; } }

    private WaitForSeconds waitHitDelay;


    #region �Ӽ� getter, setter
    public string GetRateText()
    {
        switch (toolRate)
        {
            case TOOL_RATE.SUPER_SLOW:
                return "�ſ� ����";
            case TOOL_RATE.SLOW:
                return "����";
            case TOOL_RATE.NORMAL:
                return "����";
            case TOOL_RATE.FAST:
                return "����";
            case TOOL_RATE.SUPER_FAST:
                return "�ſ� ����";
            default:
                return "";
        }
    }

    public string GetRadiusText()
    {
        switch (toolRadius)
        {
            case TOOL_RADIUS.SMALL:
                return "����";
            case TOOL_RADIUS.MEDIUM:
                return "����";
            case TOOL_RADIUS.LARGE:
                return "ŭ";
            default:
                return "";
        }
    }
    
    public string GetSpeedText()
    {
        switch (toolSpeed)
        {
            case TOOL_SPEED.SUPER_SLOW:
                return "�ſ� ����";
            case TOOL_SPEED.SLOW:
                return "����";
            case TOOL_SPEED.NORMAL:
                return "����";
            case TOOL_SPEED.FAST:
                return "����";
            case TOOL_SPEED.SUPER_FAST:
                return "�ſ� ����";
            default:
                return "";
        }
    }

    public string GetDamageText()
    {
        switch (toolDamage)
        {
            case TOOL_DAMAGE.VERY_WEAK:
                return "�ſ� ����";
            case TOOL_DAMAGE.WEAK:
                return "����";
            case TOOL_DAMAGE.NORMAL:
                return "����";
            case TOOL_DAMAGE.STRONG:
                return "����";
            case TOOL_DAMAGE.VERY_STRONG:
                return "�ſ� ����";
            default:
                return "";
        }
    }
    
    public void SetRate()
    {
        hitDelay = rates[(int)toolRate];
    }

    private void SetRate(TOOL_RATE attr)
    {
        hitDelay = rates[(int)attr];
    }

    public void SetRadius()
    {
        radius = radiuses[(int)toolRadius];
        transform.localScale = new Vector3(radius, radius, radius);
    }

    private void SetRadius(TOOL_RADIUS attr)
    {
        radius = radiuses[(int)attr];
        transform.localScale = new Vector3(radius, radius, radius);
    }

    public void SetSpeed()
    {
        speed = speeds[(int)toolSpeed];
    }

    private void SetSpeed(TOOL_SPEED attr)
    {
        speed = speeds[(int)attr];
    }
    
    public void SetDamage()
    {
        damage *= (int)toolDamage + 1;
        //damageLevel = damageLevel < maxDamageLevel ? ++damageLevel : damageLevel;
    }
    #endregion

    protected void SetRateValue(float[] values)
    {
        if (values.Length != rates.Length) {
#if UNITY_EDITOR
            print("�Ӽ� ���� " + rates.Length + "���� �Ǿ�� �մϴ�.");
#endif
            return;
        }

        rates = values;
    }

    protected void SetRadiusValue(float[] values)
    {
        if (values.Length != radiuses.Length)
        {
#if UNITY_EDITOR
            print("�Ӽ� ���� " + radiuses.Length + "���� �Ǿ�� �մϴ�.");
#endif
            return;
        }

        radiuses = values;
    }

    protected void SetSpeedValue(float[] values)
    {
        if (values.Length != speeds.Length)
        {
#if UNITY_EDITOR
            print("�Ӽ� ���� " + speeds.Length + "���� �Ǿ�� �մϴ�.");
#endif
            return;
        }

        speeds = values;
    }

    private void Init()
    {
        transform.localScale = new Vector3(radius, radius, radius);
        canHit = true;
    }

    protected void Awake()
    {
        waitHitDelay = new WaitForSeconds(hitDelay);
        SetRate();
        SetRadius();
        SetSpeed();
        SetDamage();
        Init();
    }

    protected void FixedUpdate()
    {
        // ������ (��� ������ ����� �κ�)
        if (GameManager.instance.screenManager.CurrentScreen() != SCREEN.INGAME) return;

        if (GameManager.instance.CurrentPlayer.JoyStick.IsTouch)
        {
            Move(GameManager.instance.CurrentPlayer.JoyStick.MovePosition);
        }

        // PC �׽�Ʈ ����
#if UNITY_EDITOR
        Vector2 movePos = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        Move(movePos.normalized);
#endif
    }

#if UNITY_EDITOR
    private void Update()
    {
        // �������� ����� �޶�����.
        if (Input.GetKeyDown(KeyCode.Space)) Hit();
    }
#endif

    public virtual void HitButtonDown()
    {
        // ��� ������ �������� ����
        Hit();

#if UNITY_EDITOR
        print("Hit Button Down");
#endif
    }
    
    public virtual void HitButtonUp()
    {
        // ��� ������ �������� ����
    }

    public virtual void SetTool()
    {
        GameManager.instance.CurrentPlayer.CurrentHitPos.GetComponent<SpriteRenderer>().sprite = toolImage;
    }

    protected void OnEnable()
    {
        Init();
    }

    public virtual void Move(Vector3 movePosition)
    {
        //// ���콺 ��ǥ�� ���� ��ǥ�� ��ȯ
        //Vector3 mPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        //// ���콺 ��ǥ�� ���� ������Ʈ �̵�
        //transform.position = new Vector3(mPos.x * speed, mPos.y * speed, 5);

        // ��ũ�� ����� ���ϰ� ����
        if (transform.position.x < -(GameManager.instance.CameraSize * 2))
        {
            transform.position = new Vector2(-(GameManager.instance.CameraSize * 2), transform.position.y);
        }
        else if (transform.position.x > (GameManager.instance.CameraSize * 2))
        {
            transform.position = new Vector2((GameManager.instance.CameraSize * 2), transform.position.y);
        }

        if (transform.position.y < -GameManager.instance.CameraSize)
        {
            transform.position = new Vector2(transform.position.x, -GameManager.instance.CameraSize);
        }
        else if (transform.position.y > GameManager.instance.CameraSize)
        {
            transform.position = new Vector2(transform.position.x, GameManager.instance.CameraSize);
        }

        transform.position += new Vector3(movePosition.x * speed * Time.deltaTime,
            movePosition.y * speed * Time.deltaTime);
    }

    public void HasPlayer()
    {
        hasPlayer = true;
    }

    public virtual void Hit()
    {
        if (!canHit) return;

        // ���� ��� ǥ�� ������Ʈ ����
        GameObject showHitObj = GameManager.instance.prefabManager.GetHit(HIT_OBJ_TYPE.SHOW_HIT);
        HitObjScript showHit = showHitObj.GetComponent<HitObjScript>();
        // ���� ��Ҵ��� Ȯ���ϴ� ������Ʈ ����
        GameObject checkHitObj = GameManager.instance.prefabManager.GetHit(HIT_OBJ_TYPE.CHECK_HIT);
        HitCheckScript checkHit = checkHitObj.GetComponent<HitCheckScript>();

        // ������Ʈ ũ�� ����
        showHit.ChangeInfo(radius, hitDelay);
        checkHit.ChangeInfo(radius, damage, tool);

        showHit.ChangeImage();

        // ���� ��ġ�� ������Ʈ �̵�
        showHit.Show(transform.position);
        checkHit.Show(transform.position);

        // ������ ��Ÿ��
        canHit = false;
        StartCoroutine(HitDelay());

        // ������ �ִϸ��̼� ���
        GameManager.instance.CurrentPlayer.CurrentHitPos.GetComponent<ShowHitPos>().PlayHitAnimation();

        // ������ �Ҹ� ���
        GameManager.instance.soundManager.EffectPlay(tool);
    }
    
    protected IEnumerator HitDelay()
    {
        yield return waitHitDelay;
        canHit = !canHit;
    }

    public void ChangePos(Vector3 pos)
    {
        transform.position = pos;
    }
}