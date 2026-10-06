using UnityEngine;
using System.IO; // Thư viện để đọc/ghi file

// Lớp này dùng để định nghĩa các thông tin của người chơi sẽ được lưu lại
[System.Serializable]
public class DuLieuNguoiChoi
{
    public int levelHienTai = 1;
    public int diemCaoNhat = 0;
    public int tienVang = 0;
    public int mangChoi = 5; // Mặc định cho 5 mạng khi mới chơi
}

public class HeThongLuuTru : MonoBehaviour
{
    // Biến chứa dữ liệu hiện tại đang chơi
    public DuLieuNguoiChoi duLieu;

    // Đường dẫn bí mật lưu file trên máy
    private string duongDanFile;

    void Awake()
    {
        // Khởi tạo đường dẫn lưu file an toàn, dùng được cho cả Windows và Android/iOS
        duongDanFile = Application.persistentDataPath + "/DuLieuKeoSaga.json";

        // Vừa vào game là tự động tải dữ liệu cũ lên ngay
        TaiDuLieu();
    }

    // Hàm gọi khi cần LƯU game (ví dụ: khi qua màn)
    public void LuuDuLieu()
    {
        // Chuyển dữ liệu thành chuỗi JSON
        string json = JsonUtility.ToJson(duLieu, true);

        // Ghi xuống ổ cứng
        File.WriteAllText(duongDanFile, json);

        Debug.Log("Đã lưu dữ liệu tại: " + duongDanFile);
    }

    // Hàm gọi khi cần TẢI game (khi mở ứng dụng)
    public void TaiDuLieu()
    {
        if (File.Exists(duongDanFile))
        {
            // Nếu tìm thấy file cũ, đọc file và cập nhật vào game
            string json = File.ReadAllText(duongDanFile);
            duLieu = JsonUtility.FromJson<DuLieuNguoiChoi>(json);
            Debug.Log("Tải dữ liệu thành công! Bạn đang ở level: " + duLieu.levelHienTai);
        }
        else
        {
            // Nếu không tìm thấy file (chơi lần đầu), tạo dữ liệu mới tinh
            duLieu = new DuLieuNguoiChoi();
            Debug.Log("Chưa có file lưu. Tạo dữ liệu người chơi mới.");
        }
    }
}