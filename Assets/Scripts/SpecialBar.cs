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
        attack = CharacterController2D.instance.gameObject.GetComponent<Attack>();
    }
    void Update()
    {//was UpdateBar function, this implementation may tank performance
        specialBarImage.fillAmount = ((attack.specialCooldown) / attack.specialMaxCooldown);
    }
}
