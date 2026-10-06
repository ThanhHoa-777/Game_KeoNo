using UnityEngine;

namespace LopGiaoDien
{
    public class QuanLyAmThanh : MonoBehaviour
    {
        [Header("Danh sách Loa")]
        public AudioSource loaNhacNen;
        public AudioSource loaHieuUng;

        [Header("File Âm thanh")]
        public AudioClip tiengClick;
        public AudioClip tiengNoKeo;

        public void PhatTiengClick()
        {
            if (tiengClick != null)
            {
                loaHieuUng.PlayOneShot(tiengClick);
            }
        }

        public void PhatTiengNo()
        {
            if (tiengNoKeo != null)
            {
                loaHieuUng.PlayOneShot(tiengNoKeo);
            }
        }
    }
}