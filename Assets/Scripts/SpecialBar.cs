using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SpecialBar : MonoBehaviour
{
    public Image specialBarImage;
    public Attack attack;

    void Start()
    {
        attack = Player.instance.GetComponent<Attack>();
        specialBarImage.fillAmount = ((attack.specialCooldown) / attack.specialMaxCooldown);
    }
    void Update()
    {//was UpdateBar function, this implementation may tank performance
        float idealFillAmt = ((attack.specialCooldown) / attack.specialMaxCooldown);
        specialBarImage.fillAmount = Mathf.Lerp(specialBarImage.fillAmount, idealFillAmt, 0.1f);
    }
}
