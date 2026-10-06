using UnityEngine;

public class TuongTacKeo : MonoBehaviour
{
    public int toaDoX;
    public int toaDoY;
    public QuanLyBanCo banCo;

    public enum LoaiKeoDacBiet { BinhThuong, BomThuong, BomGoc, BomCauVong }
    public LoaiKeoDacBiet loaiHienTai = LoaiKeoDacBiet.BinhThuong;
    public Color mauGoc;

    private Vector3 kichThuocGoc;

    [Header("Hình Ảnh Các Loại Bom")]
    public Sprite hinhBomThuong;  // Dùng cho kẹo nổ 4
    public Sprite hinhBomGoc;     // Dùng cho kẹo nổ L, T
    public Sprite hinhBomCauVong; // Dùng cho kẹo nổ 5

    void Awake()
    {
        // Bắt giữ ngay kích thước chuẩn khi viên kẹo vừa được sinh ra trên bàn cờ
        kichThuocGoc = transform.localScale;
    }

    void OnMouseDown()
    {
        if (banCo != null) banCo.XuLyClickKeo(this);
    }

    public void BienThanhKeoDacBiet(LoaiKeoDacBiet loaiMoi)
    {
        loaiHienTai = loaiMoi;
        SpriteRenderer sr = GetComponent<SpriteRenderer>();

        if (loaiMoi == LoaiKeoDacBiet.BomCauVong)
        {
            // Bom 5: Thay bằng ảnh Bom Cầu Vồng, để màu Trắng để giữ nguyên màu gốc của bức ảnh
            sr.sprite = hinhBomCauVong;
            sr.color = Color.white;
            transform.localScale = new Vector3(kichThuocGoc.x * 1.25f, kichThuocGoc.y * 1.25f, 1f);
        }
        else
        {
            // Bom 4 và Bom Góc: Thay đổi hình dạng (sprite) thành hình dáng mới
            if (loaiMoi == LoaiKeoDacBiet.BomThuong)
            {
                sr.sprite = hinhBomThuong;
                transform.localScale = new Vector3(kichThuocGoc.x * 1.1f, kichThuocGoc.y * 1.1f, 1f);
            }
            else if (loaiMoi == LoaiKeoDacBiet.BomGoc)
            {
                sr.sprite = hinhBomGoc;
                transform.localScale = new Vector3(kichThuocGoc.x * 1.15f, kichThuocGoc.y * 1.15f, 1f);
            }

            // Lệnh phép thuật: Nhuộm màu của viên kẹo cũ lên hình dạng mới để dễ nhận biết
            sr.color = mauGoc;
        }
    }

    public void ChuyenTrangThaiChon(bool duocChon)
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();

        if (duocChon)
        {
            // Phóng to lên khi click chọn
            transform.localScale = new Vector3(kichThuocGoc.x * 1.2f, kichThuocGoc.y * 1.2f, 1f);

            // Làm mờ đi một chút
            if (loaiHienTai == LoaiKeoDacBiet.BinhThuong || loaiHienTai == LoaiKeoDacBiet.BomCauVong)
                sr.color = new Color(1f, 1f, 1f, 0.6f);
            else
                sr.color = new Color(mauGoc.r, mauGoc.g, mauGoc.b, 0.6f); // Kẹo màu thì phải mờ theo đúng màu gốc
        }
        else
        {
            // Trả về kích thước tùy thuộc vào nó đang là loại kẹo gì
            if (loaiHienTai == LoaiKeoDacBiet.BomThuong)
                transform.localScale = new Vector3(kichThuocGoc.x * 1.1f, kichThuocGoc.y * 1.1f, 1f);
            else if (loaiHienTai == LoaiKeoDacBiet.BomGoc)
                transform.localScale = new Vector3(kichThuocGoc.x * 1.15f, kichThuocGoc.y * 1.15f, 1f);
            else if (loaiHienTai == LoaiKeoDacBiet.BomCauVong)
                transform.localScale = new Vector3(kichThuocGoc.x * 1.25f, kichThuocGoc.y * 1.25f, 1f);
            else
                transform.localScale = kichThuocGoc;

            // Trả về màu sắc nét chuẩn
            if (loaiHienTai == LoaiKeoDacBiet.BinhThuong || loaiHienTai == LoaiKeoDacBiet.BomCauVong)
                sr.color = Color.white;
            else
                sr.color = mauGoc; // Phục hồi lại đúng màu sắc của Bom 4, Bom Góc
        }
    }
}