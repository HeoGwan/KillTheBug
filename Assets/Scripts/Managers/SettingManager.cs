using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameSound = CESCO.SOUND;
using CESCO;

public class SettingManager : MonoBehaviour
{
    public Slider BGM;
    public Image BGMHandle;

    public Slider Effect;
    public Image EffectHandle;

    public Slider BugEffect;
    public Image BugEffectHandle;

    [SerializeField] private List<Sprite> speakerSprites;
    private float soundStandard = 0.25f;

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
        GameManager.instance.soundManager.ChangeVolume(GameSound.BGM, BGM.value);
        GameManager.instance.soundManager.ChangeVolume(GameSound.EFFECT, Effect.value);
        GameManager.instance.soundManager.ChangeVolume(GameSound.BUG_EFFECT, BugEffect.value);

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
}
