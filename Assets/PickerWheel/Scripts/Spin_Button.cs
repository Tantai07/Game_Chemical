using UnityEngine;
using EasyUI.PickerWheelUI;
using UnityEngine.UI;
using DG.Tweening; // Ensure you have DOTween imported

public class Spin_Button : MonoBehaviour
{
    public Button Button;
    [SerializeField] PickerWheel pickerWheel;

    private void Start()
    {
        Button.onClick.AddListener(() => {

            // เริ่มเคลื่อนที่ลง
            pickerWheel.transform.DOLocalMoveY(pickerWheel.transform.localPosition.y - 50f, 0.5f)
                .SetEase(Ease.InOutQuad)
                .OnComplete(() => {
                    // หลังจากเคลื่อนที่เสร็จ เริ่มขยายขนาด
                    pickerWheel.transform.DOScale(Vector3.one * 1.2f, 0.5f)
                        .SetEase(Ease.InOutQuad);
                });

            // หมุนวงล้อ
            pickerWheel.Spin();

            // ปิดปุ่ม
            Button.gameObject.SetActive(false);
        });
    }
}
