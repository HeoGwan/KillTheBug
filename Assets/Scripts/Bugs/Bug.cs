using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using CESCO;
using UnityEngine.UI;

public class Bug : MonoBehaviour
{
    [Header("Bug Infos")]
    [SerializeField] private float minSpeed;
    [SerializeField] private float maxSpeed;
    [SerializeField] private float backSpeed = 1.5f;
    [SerializeField] private float size = 1f;
    [SerializeField] private float sizeDifference;
    [SerializeField] protected float cycle;
    [SerializeField] protected float height;
    [SerializeField] private BUG_TYPE bugType;
    [SerializeField] private float deathDelay = 0.5f;
    [SerializeField] private Animator anim;
    [SerializeField] private float[] hp = { 10, 20, 30, 40, 50 };
    [SerializeField] private float ouchDelay;
    [SerializeField] private GameObject hpCanvas;
    [SerializeField] private AudioClip moveAudio;
    [SerializeField] private AudioClip[] deathAudio;

    public GameObject HpCanvas { get { return hpCanvas; } }

    private bool isCollision = false;
    private bool isMoving = true;
    protected SpriteRenderer sprite;
    GameObject hpImage;
    GameObject hpBackgroundImage;
    AudioSource audioSource;

    ParticleSystem ps;
    protected Vector3 direction;
    protected Vector3 dirVec;
    protected float angle;
    protected float speed;
    protected float prevSpeed;
    protected float speedDelay;

    private float healthPoint;
    public float HP { get { return healthPoint; } }
    private int hpIndex = 0;

    private WaitForSeconds waitCollision;
    private WaitForSeconds waitDeath;
    private WaitForSeconds waitOuch;
    private WaitForSeconds waitStop;

    public BUG_TYPE BugType
    {
        get { return bugType; }
    }

    //protected virtual void Init()
    //{
    //    //// ī�޶� �ش��ϴ� ��ǥ ������
    //    //float yPos = Camera.main.orthographicSize;
    //    //float xPos = yPos * Camera.main.aspect;

    //    //// ī�޶� ������ ������ ��ǥ�� ���� ����
    //    //float randomX = Random.Range(-xPos, xPos);
    //    //float randomY = Random.Range(-yPos, yPos);
    //    //transform.position = new Vector2(randomX, randomY);
    //    transform.localScale = new Vector2(this.size, this.size);

    //    // �ʿ��� ���� �ʱ�ȭ
    //    isCollision = false;
    //    isMoving = true;
    //    ps = GetComponent<ParticleSystem>();
    //    direction = transform.position - target.position;
    //    dirVec = direction.normalized;
    //    angle = 0.0f;
    //}

    public GameObject SetBug(Vector2 position)
    {
        // ���� ��ȯ �� �����ϴ� �κ�
        transform.position = position;

        float bugSize = Random.Range(size - sizeDifference, size + sizeDifference);

        transform.localScale = new Vector2(bugSize, bugSize);

        direction = transform.position - GameManager.instance.CurrentTarget.transform.position;
        dirVec = direction.normalized;

        hpIndex = GameManager.instance.Level / GameManager.instance.LevelStep < hp.Length ?
            GameManager.instance.Level / GameManager.instance.LevelStep : hp.Length - 1;
        healthPoint = hp[hpIndex];

        return gameObject;
    }

    private void Awake()
    {
        // �ʿ��� ���� �ʱ�ȭ
        isCollision = false;
        isMoving = true;
        ps = GetComponent<ParticleSystem>();
        angle = 0.0f;
        sprite = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        AudioSetting(false);
        prevSpeed = speed = Random.Range(minSpeed, maxSpeed);
        waitCollision = new WaitForSeconds(0.5f);
        waitDeath = new WaitForSeconds(deathDelay);
        waitOuch = new WaitForSeconds(speedDelay);
        waitStop = new WaitForSeconds(speedDelay);
    }

    private void Update()
    {
        if (!isMoving) { return; }

        direction = transform.position - GameManager.instance.CurrentTarget.transform.position;
        dirVec = direction.normalized;

        sprite.flipY = transform.position.x < GameManager.instance.CurrentTarget.transform.position.x ? true : false;

        // �浹 �˻� �� ������
        if (isCollision)
        {
            // Ÿ�ٿ� �ε����� ���
            Vector2 _target = transform.position + direction;
            transform.position = Vector2.MoveTowards(transform.position, _target, speed * backSpeed * Time.deltaTime);
        }
        else
        {
            // ������
            Move();
        }

        hpBackgroundImage.transform.position = hpImage.transform.position =
            (new Vector2(transform.position.x, transform.position.y + 0.8f));
    }

    protected virtual void Move()
    {
        transform.position =
            Vector2.MoveTowards(transform.position,
            GameManager.instance.CurrentTarget.transform.position, speed * Time.deltaTime);
        LookTarget();
    }

    protected virtual void LookTarget()
    {
        angle = Mathf.Atan2(dirVec.y, dirVec.x) * Mathf.Rad2Deg;
        Quaternion angleAxis = Quaternion.AngleAxis(angle, Vector3.forward);
        Quaternion rotation = Quaternion.Slerp(transform.rotation, angleAxis, 1);
        transform.rotation = rotation;
    }

    private void OnEnable()
    {
        GetComponent<BoxCollider2D>().enabled = true;
        isCollision = false;
        isMoving = true;
        anim.SetBool("Death", false);
        prevSpeed = speed = Random.Range(minSpeed, maxSpeed);

        AudioSetting(false);
        audioSource.Play();

        if (hpImage != null) hpImage.GetComponent<Image>().fillAmount = 1;
    }

    private void OnDisable()
    {
        audioSource.Stop();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Target")
        {
            isCollision = true;
            StartCoroutine(Collision());
        }
    }

    public void HitDamage(float damage, TOOL toolType, float speedDelay)
    {
        this.speedDelay = damage == 0 ? speedDelay : ouchDelay;

        if (toolType == TOOL.TRAP)
        {
            StartCoroutine(Stop());
            return;
        }

        ps.Play();
        healthPoint -= damage;
        hpImage.GetComponent<Image>().fillAmount = healthPoint / hp[hpIndex];

        if (healthPoint <= 0)
        {
            // ü���� �� ������ ��� �� 
            isMoving = false;
            GameManager.instance.scoreManager.PlusScore();
            GetComponent<BoxCollider2D>().enabled = false;
            anim.SetBool("Death", true);

            AudioSetting(true);
            audioSource.Play();

            StartCoroutine(Death());
        }
        else
        {
            StartCoroutine(Ouch());
        }
    }

    public void SetHPCanvas()
    {
        // ü�¹� ����
        hpCanvas = transform.GetChild(0).gameObject;
        hpCanvas.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceCamera;
        hpCanvas.GetComponent<Canvas>().worldCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
    }

    public void SetHPBar(GameObject hpBGObj, GameObject hpObj)
    {
        hpBackgroundImage = hpBGObj;
        hpImage = hpObj;
    }

    private void AudioSetting(bool isDeath)
    {
        // ����� ��� AudioSource�� ������ ���� ��� ���� �� �������� �ϳ��� ����Ѵ�.
        if (isDeath)
        {
            int audioIndex = Random.Range(0, deathAudio.Length);
            audioSource.loop = false;
            audioSource.clip = deathAudio[audioIndex];
        }
        else
        {
            // ��ҿ��� audioSource�� ������ �Ѱ� �Ϲ� ���带 ����Ѵ�.
            audioSource.loop = true;
            audioSource.clip = moveAudio;
        }
    }

    public void AudioPause()
    {
        audioSource.Pause();
    }

    public void AudioPlay()
    {
        audioSource.Play();
    }

    public void SetAudioVolume(float volume)
    {
        if (bugType == BUG_TYPE.MOSQUITO || bugType == BUG_TYPE.FLY)
        {
            audioSource.volume = volume;
        } else
        {
            audioSource.volume = volume * 2;
        }
    }

    public float GetVolume()
    {
        return audioSource.volume;
    }

    IEnumerator Collision()
    {
        yield return waitCollision;
        isCollision = false;
    }

    IEnumerator Death()
    {
        yield return waitDeath;
        gameObject.SetActive(false);
    }

    IEnumerator Ouch()
    {
        speed /= 2;

        yield return waitOuch;

        speed = prevSpeed;
    }

    IEnumerator Stop()
    {
        speed = 0;

        yield return waitStop;

        speed = prevSpeed;
    }
}
