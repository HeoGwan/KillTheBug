using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameSound = CESCO.SOUND;
using CESCO;
using TMPro;

public class SettingManager : MonoBehaviour
{
    public enum FrameType
    {
        Fast = 30,
        Fancy = 60,
    }
    
    public Slider BGM;
    public Image BGMHandle;

    public Slider Effect;
    public Image EffectHandle;

    public Slider BugEffect;
    public Image BugEffectHandle;

    [SerializeField] private List<Sprite> speakerSprites;
    private float soundStandard = 0.25f;

    [Space, Header("Frame")]
    public Button fastButton;
    public Button fancyButton;
    public TextMeshProUGUI fastButtonText;
    public TextMeshProUGUI fancyButtonText;
    
    private FrameType _currentFrame = FrameType.Fancy;
    private Color32 _activateColor = new(0x00, 0x00, 0x00, 0xff);
    private Color32 _deactivateColor = new(0x98, 0x98, 0x98, 0x98);

    private void Start()
    {
        // 기본 프레임 설정
        if (PlayerPrefs.HasKey("Frame"))
        {
            _currentFrame = (FrameType)PlayerPrefs.GetInt("Frame");
        }
        ToggleButton();
        SetFrame();
    }

    private void ShowSetting()
    {
        BGM.value = GameManager.instance.soundManager.BGM.volume;
        OnBGMSliderDown();
        Effect.value = GameManager.instance.soundManager.Effect.volume;
        OnEffectSliderDown();
        BugEffect.value = GameManager.instance.prefabManager.GetBugSound();
        OnBugEffectSliderDown();
    }

    public void Enable()
    {
        ShowSetting();
    }

    public void Disable() { GameManager.instance.screenManager.PrevScreen(); }

    public void Cancel()
    {
        // 설정 취소 버튼 누를 시 동작
        // 설정 화면 닫고 이전 화면 출력
        Disable();
    }

    public void Check()
    {
        // 설정 확인 버튼 누를 시 동작
        // 설정한 값에 맞게 조절
        Apply();
        Disable();
    }

    public void Apply()
    {
        // 사운드 설정
        GameManager.instance.soundManager.ChangeVolume(GameSound.BGM, BGM.value);
        GameManager.instance.soundManager.ChangeVolume(GameSound.EFFECT, Effect.value);
        GameManager.instance.soundManager.ChangeVolume(GameSound.BUG_EFFECT, BugEffect.value);

        // 프레임 설정
        SetFrame();
        
        ShowSetting();
    }

    public void OnBGMSliderDown()
    {
        int spriteIndex;
        if (BGM.value == 0)
        {
            spriteIndex = 0;
        }
        else
        {
            spriteIndex = (int)(BGM.value / soundStandard) + 1;
            spriteIndex = spriteIndex > 4 ? 4 : spriteIndex;
        }

        BGMHandle.sprite = speakerSprites[spriteIndex];
    }

    public void OnEffectSliderDown()
    {
        int spriteIndex;
        if (Effect.value == 0)
        {
            spriteIndex = 0;
        }
        else
        {
            spriteIndex = (int)(Effect.value / soundStandard) + 1;
            spriteIndex = spriteIndex > 4 ? 4 : spriteIndex;
        }

        EffectHandle.sprite = speakerSprites[spriteIndex];
    }

    public void OnBugEffectSliderDown()
    {
        int spriteIndex;
        if (BugEffect.value == 0)
        {
            spriteIndex = 0;
        }
        else
        {
            spriteIndex = (int)(BugEffect.value / soundStandard) + 1;
            spriteIndex = spriteIndex > 4 ? 4 : spriteIndex;
        }

        BugEffectHandle.sprite = speakerSprites[spriteIndex];
    }
    
    // =====프레임 설정=====
    public void SetFast()
    {
        _currentFrame = FrameType.Fast;
        ToggleButton();
    }
    
    public void SetFancy()
    {
        _currentFrame = FrameType.Fancy;
        ToggleButton();
    }
    
    public void ToggleButton()
    {
        if (_currentFrame == FrameType.Fast)
        {
            fastButton.interactable = false;
            fastButtonText.color = _deactivateColor;
            
            fancyButton.interactable = true;
            fancyButtonText.color = _activateColor;
        }
        else if (_currentFrame == FrameType.Fancy)
        {
            fastButton.interactable = true;
            fastButtonText.color = _activateColor;
            
            fancyButton.interactable = false;
            fancyButtonText.color = _deactivateColor;
        }
    }
    
    private void SetFrame()
    {
        Application.targetFrameRate = (int)_currentFrame;
        
        PlayerPrefs.SetInt("Frame", (int)_currentFrame);
    }
}
