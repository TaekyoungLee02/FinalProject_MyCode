using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class AttackChargeBar : MonoBehaviour
{
    private Image chargeBar;
    private float chargeRate;
    private float chargeMax = 0.2f;

    private Coroutine attackChargeCoroutine;

    private void Awake()
    {
        chargeBar = GetComponent<Image>();
    }

    private void Start()
    {
        chargeRate = 0.0f;
        chargeBar.fillAmount = chargeRate * chargeMax;
    }

    public void ChargeStart(float chargeTime)
    {
        attackChargeCoroutine ??= StartCoroutine(OnButtonHold(chargeTime));
    }

    public int ChargeEnd()
    {
        if (attackChargeCoroutine == null) return -1;

        StopCoroutine(attackChargeCoroutine);
        attackChargeCoroutine = null;

        // 1일 시 ChargeAttack, 0일 시 NormalAttack
        if (chargeRate == 0) { return -1; }
        else if (chargeRate < 1) { chargeRate = 0; chargeBar.fillAmount = chargeRate; return 0; }
        else { chargeRate = 0; chargeBar.fillAmount = chargeRate; return 1; }
    }

    private IEnumerator OnButtonHold(float chargeTime)
    {
        while (true)
        {
            chargeRate += Time.deltaTime / chargeTime;
            chargeRate = Mathf.Clamp(chargeRate, 0f, 1.0f);
            chargeBar.fillAmount = chargeRate * chargeMax;
            yield return null;
        }
    }
}