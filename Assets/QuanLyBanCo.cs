using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

[System.Serializable] // Lệnh này giúp hiển thị class này ra ngoài màn hình Unity
public class ThongTinManChoi
{
    public string tenManChoi = "Màn 1";
    public int soLuotDi = 20;
    public int chiSoMauMucTieu = 0; // 0: Đỏ, 1: Tím, 2: Xanh... (tùy mảng màu của bạn)
    public int soLuongMucTieu = 20;
}

public class QuanLyBanCo : MonoBehaviour
{
    public int chieuRong = 8;
    public int chieuCao = 8;
    public GameObject vienKeoMau;
    public Color[] mangMauSac;
    public string[] tenMauSac;
    public Sprite[] mangHinhAnh;
    public GameObject hieuUngNoPrefab;

    // Danh sách các màn chơi sẽ cấu hình trên Unity
    public ThongTinManChoi[] danhSachManChoi;

    // Biến lưu màn hiện tại đang chơi
    private int manHienTai = 0;

    public LopGiaoDien.QuanLyGiaoDien giaoDien;
    public LopGiaoDien.QuanLyAmThanh heThongAmThanh;

    public int soLuotDi = 20;
    [Header("Nhiệm vụ màn chơi")]
    public int chiSoMauMucTieu = 0; // 0 tương ứng với màu đầu tiên trong mảng mangMauSac (VD: màu Đỏ)
    public int soLuongMucTieu = 20; // Cần ăn 20 viên
    private int soKeoDaThuThap = 0; // Đếm số kẹo đã ăn được
    private bool daKetThuc = false;

    private GameObject[,] luoiKeo;
    private TuongTacKeo keoDaChon = null;

    void Start()
    {
        // 1. Đọc bộ nhớ xem đang ở màn mấy (mặc định là 0 nếu mới tải game)
        manHienTai = PlayerPrefs.GetInt("ManChoiHienTai", 0);

        // Đảm bảo không bị lỗi nếu người chơi vượt qua màn cuối cùng
        if (manHienTai >= danhSachManChoi.Length)
        {
            manHienTai = 0; // Quay lại màn 1 hoặc bạn có thể hiện bảng "Phá đảo"
            PlayerPrefs.SetInt("ManChoiHienTai", 0);
        }

        // 2. Lấy dữ liệu của màn đó ra
        ThongTinManChoi manNay = danhSachManChoi[manHienTai];

        // 3. Gán dữ liệu vào các biến hệ thống hiện tại của bạn
        soLuotDi = manNay.soLuotDi;
        chiSoMauMucTieu = manNay.chiSoMauMucTieu;
        soLuongMucTieu = manNay.soLuongMucTieu;

        // Cập nhật lên Giao diện (GUI)
        if (giaoDien != null)
        {
            giaoDien.CapNhatNhiemVu(tenMauSac[chiSoMauMucTieu], 0, soLuongMucTieu);
            giaoDien.CapNhatLuotDi(soLuotDi);
            // Bạn có thể thêm lệnh giaoDien.CapNhatTenMan(manNay.tenManChoi) nếu có UI Text
        }

        TaoBanCo();
    }

    void TaoBanCo()
    {
        luoiKeo = new GameObject[chieuRong, chieuCao];
        for (int x = 0; x < chieuRong; x++)
        {
            for (int y = 0; y < chieuCao; y++)
            {
                Vector2 toaDo = new Vector2(x, y);
                GameObject keoMoi = Instantiate(vienKeoMau, toaDo, Quaternion.identity);
                keoMoi.name = "Keo_" + x + "_" + y;
                keoMoi.transform.parent = this.transform;

                int chiSoMau = ChonMauAnToan(x, y);
                Color mauDuocChon = mangMauSac[chiSoMau];

                // Gán hình ảnh kẹo tương ứng
                keoMoi.GetComponent<SpriteRenderer>().sprite = mangHinhAnh[chiSoMau];
                // Đặt màu nền về trắng tinh để bức ảnh giữ nguyên màu thật
                keoMoi.GetComponent<SpriteRenderer>().color = Color.white;

                // Gán dữ liệu Logic
                TuongTacKeo scriptTuongTac = keoMoi.GetComponent<TuongTacKeo>();
                scriptTuongTac.toaDoX = x;
                scriptTuongTac.toaDoY = y;
                scriptTuongTac.banCo = this;
                scriptTuongTac.mauGoc = mauDuocChon; // <-- Bơm dữ liệu màu vào bộ nhớ

                luoiKeo[x, y] = keoMoi;
            }
        }
    }

    int ChonMauAnToan(int x, int y)
    {
        int mauNgauNhien;
        bool mauHopLe;
        do
        {
            mauNgauNhien = Random.Range(0, mangMauSac.Length);
            mauHopLe = true;

            if (x >= 2)
            {
                // Bây giờ so sánh bằng màu gốc (Logic) thay vì Sprite (GUI)
                Color mauTrai1 = luoiKeo[x - 1, y].GetComponent<TuongTacKeo>().mauGoc;
                Color mauTrai2 = luoiKeo[x - 2, y].GetComponent<TuongTacKeo>().mauGoc;
                if (mangMauSac[mauNgauNhien] == mauTrai1 && mangMauSac[mauNgauNhien] == mauTrai2)
                    mauHopLe = false;
            }
            if (y >= 2)
            {
                Color mauDuoi1 = luoiKeo[x, y - 1].GetComponent<TuongTacKeo>().mauGoc;
                Color mauDuoi2 = luoiKeo[x, y - 2].GetComponent<TuongTacKeo>().mauGoc;
                if (mangMauSac[mauNgauNhien] == mauDuoi1 && mangMauSac[mauNgauNhien] == mauDuoi2)
                    mauHopLe = false;
            }
        } while (!mauHopLe);

        return mauNgauNhien;
    }

    public void XuLyClickKeo(TuongTacKeo keoVuaClick)
    {
        if (daKetThuc) return;

        // Nếu chưa chọn kẹo nào trước đó
        if (keoDaChon == null)
        {
            keoDaChon = keoVuaClick;

            // Bật hiệu ứng cho viên kẹo vừa chọn
            keoDaChon.ChuyenTrangThaiChon(true);

            if (heThongAmThanh != null) heThongAmThanh.PhatTiengClick();
        }
        else
        {
            // Dù người chơi click vào đâu tiếp theo, cũng phải tắt hiệu ứng của viên kẹo cũ đi
            keoDaChon.ChuyenTrangThaiChon(false);

            // Nếu người chơi click lại đúng viên kẹo cũ -> Hủy chọn và thoát
            if (keoDaChon == keoVuaClick)
            {
                keoDaChon = null;
                return;
            }

            // Tính toán khoảng cách để đổi chỗ
            int khoangCachX = Mathf.Abs(keoDaChon.toaDoX - keoVuaClick.toaDoX);
            int khoangCachY = Mathf.Abs(keoDaChon.toaDoY - keoVuaClick.toaDoY);

            // Nếu là 2 viên kẹo đứng cạnh nhau (khoảng cách = 1)
            if (khoangCachX + khoangCachY == 1)
            {
                StartCoroutine(KichBanDoiCho(keoDaChon, keoVuaClick));
            }

            // Xóa bộ nhớ kẹo đã chọn để chuẩn bị cho lượt click tiếp theo
            keoDaChon = null;
        }
    }

    void DoiChoKeo(TuongTacKeo keo1, TuongTacKeo keo2)
    {
        Vector2 viTriKeo1 = keo1.transform.position;
        Vector2 viTriKeo2 = keo2.transform.position;

        StartCoroutine(TruotKeo(keo1.gameObject, viTriKeo2, 0.3f));
        StartCoroutine(TruotKeo(keo2.gameObject, viTriKeo1, 0.3f));

        luoiKeo[keo1.toaDoX, keo1.toaDoY] = keo2.gameObject;
        luoiKeo[keo2.toaDoX, keo2.toaDoY] = keo1.gameObject;

        int xTam = keo1.toaDoX;
        int yTam = keo1.toaDoY;

        keo1.toaDoX = keo2.toaDoX;
        keo1.toaDoY = keo2.toaDoY;

        keo2.toaDoX = xTam;
        keo2.toaDoY = yTam;
    }

    IEnumerator TruotKeo(GameObject keo, Vector2 diemDen, float thoiGian)
    {
        Vector2 viTriBanDau = keo.transform.position;
        float thoiGianDaTroiQua = 0f;

        while (thoiGianDaTroiQua < thoiGian)
        {
            thoiGianDaTroiQua += Time.deltaTime;
            float tyLe = thoiGianDaTroiQua / thoiGian;
            keo.transform.position = Vector2.Lerp(viTriBanDau, diemDen, tyLe);
            yield return null;
        }
        keo.transform.position = diemDen;
    }

    bool KiemTraVaXoaChuoi()
    {
        List<GameObject> keoCanXoa = new List<GameObject>();
        // Hai từ điển để ghi nhớ chiều dài chuỗi cắt ngang/dọc qua viên kẹo
        Dictionary<GameObject, int> chieuDaiNgang = new Dictionary<GameObject, int>();
        Dictionary<GameObject, int> chieuDaiDoc = new Dictionary<GameObject, int>();

        // 1. QUÉT NGANG: Tìm các chuỗi từ 3 viên trở lên
        for (int y = 0; y < chieuCao; y++)
        {
            for (int x = 0; x < chieuRong - 2; x++)
            {
                if (luoiKeo[x, y] == null) continue;
                Color mau = luoiKeo[x, y].GetComponent<TuongTacKeo>().mauGoc;
                int dem = 1;
                while (x + dem < chieuRong && luoiKeo[x + dem, y] != null && luoiKeo[x + dem, y].GetComponent<TuongTacKeo>().mauGoc == mau) dem++;

                if (dem >= 3)
                {
                    for (int i = 0; i < dem; i++)
                    {
                        GameObject k = luoiKeo[x + i, y];
                        if (!keoCanXoa.Contains(k)) keoCanXoa.Add(k);
                        // Cập nhật chiều dài đi qua viên này
                        if (!chieuDaiNgang.ContainsKey(k) || chieuDaiNgang[k] < dem) chieuDaiNgang[k] = dem;
                    }
                    x += dem - 1; // Bỏ qua các viên đã quét
                }
            }
        }

        // 2. QUÉT DỌC: Tìm các chuỗi từ 3 viên trở lên
        for (int x = 0; x < chieuRong; x++)
        {
            for (int y = 0; y < chieuCao - 2; y++)
            {
                if (luoiKeo[x, y] == null) continue;
                Color mau = luoiKeo[x, y].GetComponent<TuongTacKeo>().mauGoc;
                int dem = 1;
                while (y + dem < chieuCao && luoiKeo[x, y + dem] != null && luoiKeo[x, y + dem].GetComponent<TuongTacKeo>().mauGoc == mau) dem++;

                if (dem >= 3)
                {
                    for (int i = 0; i < dem; i++)
                    {
                        GameObject k = luoiKeo[x, y + i];
                        if (!keoCanXoa.Contains(k)) keoCanXoa.Add(k);
                        if (!chieuDaiDoc.ContainsKey(k) || chieuDaiDoc[k] < dem) chieuDaiDoc[k] = dem;
                    }
                    y += dem - 1;
                }
            }
        }

        if (keoCanXoa.Count == 0) return false;

        // 3. XÉT DUYỆT TẠO KẸO ĐẶC BIỆT (L, T, Hàng 5)

        // MỚI: Tạo một danh sách để ghi nhớ màu nào đã được cấp Bom
        List<Color> mauDaTaoBom = new List<Color>();

        foreach (GameObject keo in keoCanXoa.ToArray())
        {
            if (!keoCanXoa.Contains(keo)) continue;

            TuongTacKeo script = keo.GetComponent<TuongTacKeo>();

            // MỚI: Nếu nhóm màu này đã được tạo Bom rồi thì bỏ qua, để các viên còn lại bị nổ
            if (mauDaTaoBom.Contains(script.mauGoc)) continue;

            int ngang = chieuDaiNgang.ContainsKey(keo) ? chieuDaiNgang[keo] : 0;
            int doc = chieuDaiDoc.ContainsKey(keo) ? chieuDaiDoc[keo] : 0;

            // Ưu tiên 1: Hàng 5 (Ngang hoặc Dọc) -> Bom Cầu Vồng
            if (ngang >= 5 || doc >= 5)
            {
                script.BienThanhKeoDacBiet(TuongTacKeo.LoaiKeoDacBiet.BomCauVong);
                keoCanXoa.Remove(keo);
                mauDaTaoBom.Add(script.mauGoc); // Đánh dấu đã cấp Bom cho màu này
            }
            // Ưu tiên 2: Có điểm giao nhau (Ngang >= 3 VÀ Dọc >= 3) -> Chữ L hoặc T -> Bom Góc
            else if (ngang >= 3 && doc >= 3)
            {
                script.BienThanhKeoDacBiet(TuongTacKeo.LoaiKeoDacBiet.BomGoc);
                keoCanXoa.Remove(keo);
                mauDaTaoBom.Add(script.mauGoc);
            }
            // Ưu tiên 3: Hàng 4 bình thường -> Bom Thường
            else if (ngang == 4 || doc == 4)
            {
                script.BienThanhKeoDacBiet(TuongTacKeo.LoaiKeoDacBiet.BomThuong);
                keoCanXoa.Remove(keo);
                mauDaTaoBom.Add(script.mauGoc);
            }
        }

        // 4. KÍCH HOẠT HIỆU ỨNG NỔ CỦA KẸO ĐẶC BIỆT
        List<GameObject> keoBiBomNo = new List<GameObject>();
        foreach (GameObject keo in keoCanXoa)
        {
            TuongTacKeo script = keo.GetComponent<TuongTacKeo>();

            if (script.loaiHienTai == TuongTacKeo.LoaiKeoDacBiet.BomThuong)
            {
                // Bom thường: Nổ ô vuông 3x3 (phạm vi 1 xung quanh)
                for (int i = -1; i <= 1; i++)
                {
                    for (int j = -1; j <= 1; j++)
                    {
                        int xXungQuanh = script.toaDoX + i;
                        int yXungQuanh = script.toaDoY + j;

                        if (xXungQuanh >= 0 && xXungQuanh < chieuRong && yXungQuanh >= 0 && yXungQuanh < chieuCao)
                        {
                            GameObject nanNhan = luoiKeo[xXungQuanh, yXungQuanh];
                            if (nanNhan != null && !keoCanXoa.Contains(nanNhan) && !keoBiBomNo.Contains(nanNhan))
                            {
                                keoBiBomNo.Add(nanNhan);
                            }
                        }
                    }
                }
            }
            else if (script.loaiHienTai == TuongTacKeo.LoaiKeoDacBiet.BomGoc)
            {
                // Bom Góc (L, T): Nổ hình chữ thập quét sạch nguyên hàng dọc và hàng ngang

                // 1. Tia nổ dọc (Giữ nguyên toaDoX, quét mọi toaDoY từ trên xuống dưới)
                for (int y = 0; y < chieuCao; y++)
                {
                    GameObject nanNhan = luoiKeo[script.toaDoX, y];
                    if (nanNhan != null && !keoCanXoa.Contains(nanNhan) && !keoBiBomNo.Contains(nanNhan))
                    {
                        keoBiBomNo.Add(nanNhan);
                    }
                }

                // 2. Tia nổ ngang (Giữ nguyên toaDoY, quét mọi toaDoX từ trái sang phải)
                for (int x = 0; x < chieuRong; x++)
                {
                    GameObject nanNhan = luoiKeo[x, script.toaDoY];
                    if (nanNhan != null && !keoCanXoa.Contains(nanNhan) && !keoBiBomNo.Contains(nanNhan))
                    {
                        keoBiBomNo.Add(nanNhan);
                    }
                }
            }
            else if (script.loaiHienTai == TuongTacKeo.LoaiKeoDacBiet.BomCauVong)
            {
                // Bom Cầu Vồng: Quét toàn bàn cờ, xóa sạch mọi viên kẹo có cùng màu gốc với nó
                for (int x = 0; x < chieuRong; x++)
                {
                    for (int y = 0; y < chieuCao; y++)
                    {
                        GameObject nanNhan = luoiKeo[x, y];
                        if (nanNhan != null && nanNhan.GetComponent<TuongTacKeo>().mauGoc == script.mauGoc)
                        {
                            if (!keoCanXoa.Contains(nanNhan) && !keoBiBomNo.Contains(nanNhan))
                            {
                                keoBiBomNo.Add(nanNhan);
                            }
                        }
                    }
                }
            }
        }
        keoCanXoa.AddRange(keoBiBomNo);

        // 5. TIẾN HÀNH XÓA KẸO
        foreach (GameObject keo in keoCanXoa)
        {
            TuongTacKeo script = keo.GetComponent<TuongTacKeo>();

            // Cập nhật nhiệm vụ thu thập kẹo
            if (script.mauGoc == mangMauSac[chiSoMauMucTieu])
            {
                soKeoDaThuThap++;
                if (giaoDien != null)
                {
                    giaoDien.CapNhatNhiemVu(tenMauSac[chiSoMauMucTieu], soKeoDaThuThap, soLuongMucTieu);
                }
            }

            // TẠO HIỆU ỨNG NỔ BẮN HẠT 
            if (hieuUngNoPrefab != null)
            {
                // Sinh ra hiệu ứng tại đúng vị trí viên kẹo
                GameObject no = Instantiate(hieuUngNoPrefab, keo.transform.position, Quaternion.identity);

                // Lấy hệ thống hạt và đổi màu cho giống với viên kẹo bị nổ
                ParticleSystem ps = no.GetComponent<ParticleSystem>();
                var main = ps.main;
                main.startColor = script.mauGoc;

                // Tự động xóa rác (hiệu ứng) khỏi bộ nhớ sau 1 giây
                Destroy(no, 1f);
            }

            // Xóa kẹo khỏi bàn cờ
            luoiKeo[script.toaDoX, script.toaDoY] = null;
            Destroy(keo);
        }

        if (giaoDien != null) giaoDien.CongDiem(keoCanXoa.Count * 10);
        if (heThongAmThanh != null) heThongAmThanh.PhatTiengNo();

        return true;
    }

    IEnumerator KeoRoiXuong()
    {
        bool coKeoRoi = false;
        for (int x = 0; x < chieuRong; x++)
        {
            for (int y = 0; y < chieuCao; y++)
            {
                if (luoiKeo[x, y] == null)
                {
                    for (int k = y + 1; k < chieuCao; k++)
                    {
                        if (luoiKeo[x, k] != null)
                        {
                            GameObject keoTra = luoiKeo[x, k];
                            luoiKeo[x, y] = keoTra;
                            luoiKeo[x, k] = null;
                            keoTra.GetComponent<TuongTacKeo>().toaDoY = y;
                            StartCoroutine(TruotKeo(keoTra, new Vector2(x, y), 0.2f));
                            coKeoRoi = true;
                            break;
                        }
                    }
                }
            }
        }
        if (coKeoRoi) yield return new WaitForSeconds(0.2f);
    }

    IEnumerator DienKeoMoi()
    {
        bool coTaoMoi = false;
        for (int x = 0; x < chieuRong; x++)
        {
            for (int y = 0; y < chieuCao; y++)
            {
                if (luoiKeo[x, y] == null)
                {
                    Vector2 toaDoBatDau = new Vector2(x, y + 8);
                    GameObject keoMoi = Instantiate(vienKeoMau, toaDoBatDau, Quaternion.identity);
                    keoMoi.name = "Keo_" + x + "_" + y;
                    keoMoi.transform.parent = this.transform;

                    int chiSoMau = Random.Range(0, mangMauSac.Length);
                    Color mauDuocChon = mangMauSac[chiSoMau];

                    keoMoi.GetComponent<SpriteRenderer>().sprite = mangHinhAnh[chiSoMau];
                    keoMoi.GetComponent<SpriteRenderer>().color = Color.white;

                    TuongTacKeo script = keoMoi.GetComponent<TuongTacKeo>();
                    script.toaDoX = x;
                    script.toaDoY = y;
                    script.banCo = this;
                    script.mauGoc = mauDuocChon; // <-- Bơm dữ liệu màu vào kẹo mới

                    luoiKeo[x, y] = keoMoi;

                    StartCoroutine(TruotKeo(keoMoi, new Vector2(x, y), 0.2f));
                    coTaoMoi = true;
                }
            }
        }
        if (coTaoMoi) yield return new WaitForSeconds(0.2f);
    }

    IEnumerator KichBanDoiCho(TuongTacKeo keo1, TuongTacKeo keo2)
    {
        

        soLuotDi--;
        if (giaoDien != null) giaoDien.CapNhatLuotDi(soLuotDi);

        DoiChoKeo(keo1, keo2);
        yield return new WaitForSeconds(0.3f);

        // --- BƯỚC MỚI: KIỂM TRA ĐỔI CHỖ CÓ CHỨA BOM CẦU VỒNG KHÔNG ---
        bool laBomCauVong = false;
        if (keo1.loaiHienTai == TuongTacKeo.LoaiKeoDacBiet.BomCauVong || keo2.loaiHienTai == TuongTacKeo.LoaiKeoDacBiet.BomCauVong)
        {
            laBomCauVong = true;

            // Phân biệt đâu là viên Bom, đâu là viên kẹo thường bị nhắm mục tiêu
            TuongTacKeo bom = (keo1.loaiHienTai == TuongTacKeo.LoaiKeoDacBiet.BomCauVong) ? keo1 : keo2;
            TuongTacKeo mucTieu = (keo1.loaiHienTai == TuongTacKeo.LoaiKeoDacBiet.BomCauVong) ? keo2 : keo1;

            // Kích nổ ngay lập tức toàn bộ màu của viên mục tiêu
            KichHoatBomCauVong(bom, mucTieu.mauGoc);
        }

        // --- CẬP NHẬT ĐIỀU KIỆN RƠI KẸO ---
        bool coDiem = false;
        if (!laBomCauVong)
        {
            // Trượt kẹo thường thì dùng thuật toán quét hàng 3 cũ
            coDiem = KiemTraVaXoaChuoi();
        }
        else
        {
            // Trượt trúng Bom 5 thì chắc chắn bàn cờ có khoảng trống để rơi kẹo xuống
            coDiem = true;
        }

        if (!coDiem)
        {
            DoiChoKeo(keo1, keo2);
            yield return new WaitForSeconds(0.3f);
        }
        else
        {
            while (coDiem)
            {
                yield return new WaitForSeconds(0.1f);
                yield return StartCoroutine(KeoRoiXuong());
                yield return StartCoroutine(DienKeoMoi());

                yield return new WaitForSeconds(0.7f);

                coDiem = KiemTraVaXoaChuoi();
            }
        }

        // KẾT THÚC VÒNG LẶP: Đánh giá kết quả Thắng/Thua
        if (!daKetThuc)
        {
            if (soKeoDaThuThap >= soLuongMucTieu)
            {
                daKetThuc = true;
                PlayerPrefs.SetInt("ManChoiHienTai", manHienTai + 1);
                if (giaoDien != null) giaoDien.HienThiThang();
            }
            else if (soLuotDi <= 0)
            {
                daKetThuc = true;
                if (giaoDien != null) giaoDien.HienThiKetThuc();
            }
        }

        
    }
    void KichHoatBomCauVong(TuongTacKeo bom, Color mauCanXoa)
    {
        List<GameObject> keoCanXoa = new List<GameObject>();

        // Đưa quả bom 5 vào danh sách chờ xóa
        keoCanXoa.Add(bom.gameObject);

        // Quét toàn bàn cờ tìm những viên cùng màu với viên kẹo mục tiêu
        for (int x = 0; x < chieuRong; x++)
        {
            for (int y = 0; y < chieuCao; y++)
            {
                if (luoiKeo[x, y] != null)
                {
                    TuongTacKeo script = luoiKeo[x, y].GetComponent<TuongTacKeo>();
                    if (script.mauGoc == mauCanXoa && luoiKeo[x, y] != bom.gameObject)
                    {
                        keoCanXoa.Add(luoiKeo[x, y]);
                    }
                }
            }
        }

        // Tiến hành hiệu ứng xóa kẹo và tính điểm
        foreach (GameObject keo in keoCanXoa)
        {
            TuongTacKeo script = keo.GetComponent<TuongTacKeo>();

            if (script.mauGoc == mangMauSac[chiSoMauMucTieu])
            {
                soKeoDaThuThap++;
                if (giaoDien != null) giaoDien.CapNhatNhiemVu(tenMauSac[chiSoMauMucTieu], soKeoDaThuThap, soLuongMucTieu);
            }

            if (hieuUngNoPrefab != null)
            {
                GameObject no = Instantiate(hieuUngNoPrefab, keo.transform.position, Quaternion.identity);
                ParticleSystem ps = no.GetComponent<ParticleSystem>();
                var main = ps.main;
                main.startColor = script.mauGoc;
                Destroy(no, 1f);
            }

            luoiKeo[script.toaDoX, script.toaDoY] = null;
            Destroy(keo);
        }

        if (giaoDien != null) giaoDien.CongDiem(keoCanXoa.Count * 10);
        if (heThongAmThanh != null) heThongAmThanh.PhatTiengNo();
    }
    public void ChoiManTiepTheo()
    {
        // Ẩn bảng thắng (nếu cần thiết, dù load scene sẽ tự reset)
        // Lệnh này sẽ tải lại đúng Scene đang mở (tên là SampleScene)
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}   