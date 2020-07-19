using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    public Image greenHealthFill;
    public Image redHealthFill;
    public GameObject comboCounterObj;
    public Text comboCounter;
    float greenHealth;
    float redHealth;
    float toValue_red;
    float fromValue_red;
    TweenID redHealthTween;
    float greenFillAmount => greenHealth / MatchManager.maxHP;
    float redFillAmount => redHealth / MatchManager.maxHP;
    float redHealthMeterUpdateTime = 1f;
    float comboHitsTaken;
    float minComboMod = .25f;
    float maxScalingTicks = 35;
    float scalingTicks;

    public void InitUI()
    {
        greenHealth = MatchManager.maxHP;
        redHealth = MatchManager.maxHP;
        UpdateGreenHealthMeter();
        ResetCombo();
    }

    public void TakeDamage(float damage, float scaling)
    {
        greenHealth -= (damage * ScalingMod());
        UpdateGreenHealthMeter();
        if (redHealthTween.isTweening)
        {
            redHealthTween.Cancel();
        }
        Debug.Log("HIT " + damage * ScalingMod());
        comboHitsTaken++;
        UpdateComboCounter();
        scalingTicks += scaling;
    }

    public bool IsDead()
    {
        return greenHealth <= 0;
    }

    void UpdateGreenHealthMeter()
    {
        greenHealthFill.fillAmount = greenFillAmount;
    }

    public void UpdateRedHealthMeter()
    {
        fromValue_red = redHealth;
        toValue_red = Mathf.Lerp(greenHealth, fromValue_red, 0.25f);
        LeanTween.value(0, 1, redHealthMeterUpdateTime)
            .setOnUpdate(RedHealthMeterUpdate)
            .setOnComplete(RedHealthMeterComplete)
            .setTweenId(ref redHealthTween);
    }

    void RedHealthMeterUpdate(float t)
    {
        float value = Mathf.Lerp(fromValue_red, toValue_red, t);
        redHealth = value;
        redHealthFill.fillAmount = redFillAmount;
    }

    void RedHealthMeterComplete()
    {
        redHealth = toValue_red;
        redHealthFill.fillAmount = redFillAmount;
    }

    float ScalingMod()
    {
        float percentage = scalingTicks / maxScalingTicks;
        return Mathf.Lerp(1, minComboMod, percentage);
    }

    public void ResetCombo()
    {
        comboHitsTaken = 0;
        UpdateComboCounter();
        UpdateRedHealthMeter();
    }

   void UpdateComboCounter()
    {
        comboCounterObj.gameObject.SetActive(comboHitsTaken > 1);
        comboCounter.text = comboHitsTaken.ToString();
    }
}
