using UnityEngine;
using UnityEngine.SceneManagement; // Thư viện bắt buộc để chuyển Scene

namespace LopGiaoDien
{
    public class QuanLyMenu : MonoBehaviour
    {
        // Hàm này sẽ được gọi khi bấm nút "Chơi Ngay"
        public void VaoGame()
        {
            // Tải màn chơi có tên là "SampleScene" (Nơi chứa bàn cờ của bạn)
            SceneManager.LoadScene("SampleScene");
        }

        // Hàm này dùng cho nút "Thoát Game" (Nếu bạn muốn làm thêm)
        public void ThoatGame()
        {
            Application.Quit();
            Debug.Log("Đã thoát game!");
        }
    }
}