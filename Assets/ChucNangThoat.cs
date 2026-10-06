using UnityEngine;

public class ChucNangThoat : MonoBehaviour
{
    public void ThoatKhoiGame()
    {
        // In ra dòng thông báo để biết nút có hoạt động khi test trên Unity
        Debug.Log("Đã bấm nút Thoát Game!");

        // Lệnh này sẽ tắt hẳn game khi bạn Build ra file .exe hoặc .apk
        Application.Quit();
    }
}