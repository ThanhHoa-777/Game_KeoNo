using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

namespace LopGiaoDien
{
    public class QuanLyGiaoDien : MonoBehaviour
    {
        public TextMeshProUGUI txtDiemSo;
        public TextMeshProUGUI txtLuotDi;
        public TextMeshProUGUI txtNhiemVu; // MỚI: Chữ hiển thị tiến độ

        public GameObject panelKetThuc;
        public GameObject panelThang;      // MỚI: Màn hình chiến thắng

        public HeThongLuuTru duLieuGame;

        void Start()
        {
            txtDiemSo.text = "Điểm: " + duLieuGame.duLieu.diemCaoNhat.ToString();
            if (panelKetThuc != null) panelKetThuc.SetActive(false);
            if (panelThang != null) panelThang.SetActive(false);
        }

        public void CongDiem(int diemNhanDuoc)
        {
            duLieuGame.duLieu.diemCaoNhat += diemNhanDuoc;
            txtDiemSo.text = "Điểm: " + duLieuGame.duLieu.diemCaoNhat.ToString();
            duLieuGame.LuuDuLieu();
        }

        public void CapNhatLuotDi(int luotConLai)
        {
            if (txtLuotDi != null) txtLuotDi.text = "Lượt: " + luotConLai.ToString();
        }

        // MỚI: Cập nhật con số tiến độ nhiệm vụ
        public void CapNhatNhiemVu(string tenMau, int daThuThap, int mucTieu)
        {
            if (txtNhiemVu != null)
            {
                txtNhiemVu.text = "Thu thập kẹo " + tenMau + ": " + daThuThap + "/" + mucTieu;
            }
        }

        public void HienThiKetThuc()
        {
            if (panelKetThuc != null) panelKetThuc.SetActive(true);
        }

        // MỚI: Hiển thị bảng Thắng
        public void HienThiThang()
        {
            if (panelThang != null) panelThang.SetActive(true);
        }

        public void ChoiLai()
        {
            string tenManHinh = SceneManager.GetActiveScene().name;
            SceneManager.LoadScene(tenManHinh);
        }
    }
}