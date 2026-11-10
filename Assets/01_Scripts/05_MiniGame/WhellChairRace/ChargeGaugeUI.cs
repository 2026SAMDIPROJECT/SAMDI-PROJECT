using UnityEngine;
using UnityEngine.UI;

public class ChargeGaugeUI : MonoBehaviour
{
    [SerializeField] private WheelchairController controller;
    [SerializeField] private Image gauge;

    private void Update()
    {
        gauge.fillAmount = controller.ChargeProgress01;
    }
}
