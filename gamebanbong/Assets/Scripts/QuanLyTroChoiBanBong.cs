
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class QuanLyTroChoiBanBong : MonoBehaviour
{
    [Header("Cấu hình lưới bóng")]
    [SerializeField] private int soHang = 12;
    [SerializeField] private int soCot = 8;
    [SerializeField] private int soHangBanDau = 5;
    [SerializeField] private float banKinhBong = 0.32f;
    [SerializeField] private float tocDoBan = 8.5f;

    [Header("Khu vực chơi")]
    [SerializeField] private float gioiHanTrai = -2.65f;
    [SerializeField] private float gioiHanPhai = 2.65f;
    [SerializeField] private float viTriDinhLuoi = 4.05f;
    [SerializeField] private Vector2 viTriSung = new Vector2(0f, -4.15f);

    private const int KhongCoBong = -1;
    private readonly Color[] bangMauBong =
    {
        new Color(1.00f, 0.20f, 0.18f),
        new Color(0.18f, 0.54f, 1.00f),
        new Color(0.15f, 0.82f, 0.34f),
        new Color(1.00f, 0.88f, 0.12f),
        new Color(0.75f, 0.27f, 1.00f)
    };

    private Bong[,] luoiBong;
    private Sprite[] spriteBong;
    private Camera cameraChinh;
    private LineRenderer duongNgam;
    private LineRenderer thanSung;
    private Bong bongDangBan;
    private Bong bongTiepTheo;
    private Vector2 huongBan = Vector2.up;
    private bool dangBay;
    private bool daKetThuc;
    private int diem;

    private Canvas canvasGiaoDien;
    private Text chuDiem;
    private Text chuHuongDan;
    private Text chuBongTiepTheo;
    private GameObject bangKetThuc;
    private Text chuKetThuc;

    private Material vatLieuLine;

    private void Start()
    {
        KhoiTaoTroChoi();
    }

    private void Update()
    {
        if (daKetThuc)
        {
            if (CoBamChoiLai())
            {
                ChoiLai();
            }
            return;
        }

        CapNhatDieuKhien();
        CapNhatBongDangBay();
    }

    private void KhoiTaoTroChoi()
    {
        Random.InitState(System.DateTime.Now.Millisecond);

        cameraChinh = Camera.main;
        if (cameraChinh == null)
        {
            GameObject doiTuongCamera = new GameObject("Camera Chính");
            cameraChinh = doiTuongCamera.AddComponent<Camera>();
            doiTuongCamera.AddComponent<AudioListener>();
            doiTuongCamera.tag = "MainCamera";
        }

        cameraChinh.orthographic = true;
        cameraChinh.orthographicSize = 5.15f;
        cameraChinh.backgroundColor = new Color(0.55f, 0.78f, 0.96f);
        cameraChinh.transform.position = new Vector3(0f, 0f, -10f);

        luoiBong = new Bong[soHang, soCot];
        spriteBong = TaoTatCaSpriteBong();
        vatLieuLine = new Material(Shader.Find("Sprites/Default"));

        TaoNenTrangTri();
        TaoSungBan();
        TaoGiaoDien();
        TaoLuoiBongBanDau();
        TaoBongMoiDeBan();
        CapNhatDiem();
    }

    private void TaoNenTrangTri()
    {
        TaoHinhChuNhat("Nền trời", new Vector3(0f, 0f, 1f), new Vector2(6.2f, 10.5f), new Color(0.55f, 0.80f, 0.98f));
        TaoHinhChuNhat("Thanh tường trái", new Vector3(gioiHanTrai - 0.1f, 0f, -0.1f), new Vector2(0.12f, 10.2f), new Color(0.95f, 0.86f, 0.35f));
        TaoHinhChuNhat("Thanh tường phải", new Vector3(gioiHanPhai + 0.1f, 0f, -0.1f), new Vector2(0.12f, 10.2f), new Color(0.95f, 0.86f, 0.35f));
        TaoHinhChuNhat("Sàn bắn", new Vector3(0f, -4.75f, -0.1f), new Vector2(6.2f, 0.55f), new Color(0.99f, 0.78f, 0.28f));
    }

    private GameObject TaoHinhChuNhat(string ten, Vector3 viTri, Vector2 kichThuoc, Color mau)
    {
        GameObject doiTuong = new GameObject(ten);
        doiTuong.transform.position = viTri;
        doiTuong.transform.localScale = new Vector3(kichThuoc.x, kichThuoc.y, 1f);
        SpriteRenderer ve = doiTuong.AddComponent<SpriteRenderer>();
        ve.sprite = TaoSpriteHinhVuong();
        ve.color = mau;
        ve.sortingOrder = -20;
        return doiTuong;
    }

    private void TaoSungBan()
    {
        GameObject than = new GameObject("Súng bắn bóng");
        thanSung = than.AddComponent<LineRenderer>();
        thanSung.material = vatLieuLine;
        thanSung.positionCount = 2;
        thanSung.startWidth = 0.18f;
        thanSung.endWidth = 0.12f;
        thanSung.startColor = new Color(1f, 0.55f, 0.08f);
        thanSung.endColor = new Color(1f, 0.85f, 0.15f);
        thanSung.sortingOrder = 5;

        GameObject deSung = TaoBongDoHoa("Đế súng", new Vector3(viTriSung.x, viTriSung.y - 0.18f, 0f), new Color(1f, 0.58f, 0.05f), 0.55f);
        deSung.GetComponent<SpriteRenderer>().sortingOrder = 6;

        GameObject duong = new GameObject("Đường ngắm");
        duongNgam = duong.AddComponent<LineRenderer>();
        duongNgam.material = vatLieuLine;
        duongNgam.positionCount = 2;
        duongNgam.startWidth = 0.04f;
        duongNgam.endWidth = 0.04f;
        duongNgam.startColor = Color.white;
        duongNgam.endColor = new Color(1f, 1f, 1f, 0.15f);
        duongNgam.sortingOrder = 4;
    }

    private void TaoGiaoDien()
    {
        GameObject canvasObj = new GameObject("Giao diện người chơi");
        canvasGiaoDien = canvasObj.AddComponent<Canvas>();
        canvasGiaoDien.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasObj.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasObj.GetComponent<CanvasScaler>().referenceResolution = new Vector2(1080, 1920);
        canvasObj.AddComponent<GraphicRaycaster>();

        chuDiem = TaoChuUI("Chữ điểm", "ĐIỂM: 0", new Vector2(0f, -65f), 54, TextAnchor.MiddleCenter, Color.white);
        DatNeo(chuDiem.rectTransform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(480f, 90f));

        chuHuongDan = TaoChuUI("Chữ hướng dẫn", "Kéo để ngắm - thả/click để bắn", new Vector2(0f, 95f), 32, TextAnchor.MiddleCenter, new Color(0.18f, 0.22f, 0.32f));
        DatNeo(chuHuongDan.rectTransform, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(780f, 70f));

        chuBongTiepTheo = TaoChuUI("Chữ bóng tiếp theo", "BÓNG TIẾP THEO", new Vector2(350f, 180f), 26, TextAnchor.MiddleCenter, new Color(0.18f, 0.22f, 0.32f));
        DatNeo(chuBongTiepTheo.rectTransform, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(300f, 60f));

        TaoBangKetThuc();
    }

    private Text TaoChuUI(string ten, string noiDung, Vector2 viTri, int coChu, TextAnchor canLe, Color mau)
    {
        GameObject obj = new GameObject(ten);
        obj.transform.SetParent(canvasGiaoDien.transform, false);
        Text chu = obj.AddComponent<Text>();
        chu.text = noiDung;
        chu.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        chu.fontSize = coChu;
        chu.alignment = canLe;
        chu.color = mau;
        chu.fontStyle = FontStyle.Bold;
        RectTransform rect = chu.rectTransform;
        rect.anchoredPosition = viTri;
        rect.sizeDelta = new Vector2(700f, 90f);
        return chu;
    }

    private void DatNeo(RectTransform rect, Vector2 neoMin, Vector2 neoMax, Vector2 diemTua, Vector2 kichThuoc)
    {
        rect.anchorMin = neoMin;
        rect.anchorMax = neoMax;
        rect.pivot = diemTua;
        rect.sizeDelta = kichThuoc;
    }

    private void TaoBangKetThuc()
    {
        bangKetThuc = new GameObject("Bảng kết thúc");
        bangKetThuc.transform.SetParent(canvasGiaoDien.transform, false);
        Image nen = bangKetThuc.AddComponent<Image>();
        nen.color = new Color(0f, 0f, 0f, 0.55f);
        RectTransform rect = bangKetThuc.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        GameObject hop = new GameObject("Khung thông báo");
        hop.transform.SetParent(bangKetThuc.transform, false);
        Image anhHop = hop.AddComponent<Image>();
        anhHop.color = new Color(1f, 0.86f, 0.35f, 0.96f);
        RectTransform rectHop = hop.GetComponent<RectTransform>();
        rectHop.anchorMin = new Vector2(0.5f, 0.5f);
        rectHop.anchorMax = new Vector2(0.5f, 0.5f);
        rectHop.pivot = new Vector2(0.5f, 0.5f);
        rectHop.anchoredPosition = Vector2.zero;
        rectHop.sizeDelta = new Vector2(760f, 520f);

        GameObject objChu = new GameObject("Chữ kết thúc");
        objChu.transform.SetParent(hop.transform, false);
        chuKetThuc = objChu.AddComponent<Text>();
        chuKetThuc.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        chuKetThuc.fontSize = 54;
        chuKetThuc.fontStyle = FontStyle.Bold;
        chuKetThuc.alignment = TextAnchor.MiddleCenter;
        chuKetThuc.color = new Color(0.28f, 0.12f, 0.02f);
        RectTransform rectChu = chuKetThuc.rectTransform;
        rectChu.anchorMin = Vector2.zero;
        rectChu.anchorMax = Vector2.one;
        rectChu.offsetMin = new Vector2(45f, 45f);
        rectChu.offsetMax = new Vector2(-45f, -45f);

        bangKetThuc.SetActive(false);
    }

    private void TaoLuoiBongBanDau()
    {
        for (int hang = 0; hang < soHangBanDau; hang++)
        {
            for (int cot = 0; cot < soCot; cot++)
            {
                int maMau = Random.Range(0, bangMauBong.Length);
                Vector3 viTri = LayViTriO(hang, cot);
                Bong bong = TaoBong("Bóng trong lưới", maMau, viTri, true);
                bong.Hang = hang;
                bong.Cot = cot;
                luoiBong[hang, cot] = bong;
            }
        }
    }

    private void TaoBongMoiDeBan()
    {
        if (bongTiepTheo == null)
        {
            bongTiepTheo = TaoBong("Bóng tiếp theo", Random.Range(0, bangMauBong.Length), new Vector3(2.05f, viTriSung.y + 0.15f, 0f), false);
            bongTiepTheo.transform.localScale = Vector3.one * banKinhBong * 1.55f;
        }

        int mauMoi = bongTiepTheo.MaMau;
        Destroy(bongTiepTheo.gameObject);
        bongTiepTheo = TaoBong("Bóng tiếp theo", Random.Range(0, bangMauBong.Length), new Vector3(2.05f, viTriSung.y + 0.15f, 0f), false);
        bongTiepTheo.transform.localScale = Vector3.one * banKinhBong * 1.55f;

        bongDangBan = TaoBong("Bóng đang bắn", mauMoi, new Vector3(viTriSung.x, viTriSung.y, 0f), false);
        dangBay = false;
    }

    private Bong TaoBong(string ten, int maMau, Vector3 viTri, bool namTrongLuoi)
    {
        GameObject obj = TaoBongDoHoa(ten, viTri, bangMauBong[maMau], banKinhBong * 2f);
        Bong bong = obj.AddComponent<Bong>();
        bong.MaMau = maMau;
        bong.Hang = KhongCoBong;
        bong.Cot = KhongCoBong;
        SpriteRenderer ve = obj.GetComponent<SpriteRenderer>();
        ve.sprite = spriteBong[maMau];
        ve.sortingOrder = namTrongLuoi ? 1 : 10;
        return bong;
    }

    private GameObject TaoBongDoHoa(string ten, Vector3 viTri, Color mau, float duongKinh)
    {
        GameObject obj = new GameObject(ten);
        obj.transform.position = viTri;
        obj.transform.localScale = Vector3.one * duongKinh;
        SpriteRenderer ve = obj.AddComponent<SpriteRenderer>();
        ve.sprite = TaoSpriteTron(mau);
        ve.sortingOrder = 1;
        CircleCollider2D vaCham = obj.AddComponent<CircleCollider2D>();
        vaCham.radius = 0.5f;
        return obj;
    }

    private void CapNhatDieuKhien()
    {
        if (bongDangBan == null || dangBay)
            return;

        Vector3 diemManHinh;
        bool coDauVao = LayViTriDauVao(out diemManHinh);
        if (coDauVao)
        {
            Vector3 diemTheGioi = cameraChinh.ScreenToWorldPoint(diemManHinh);
            diemTheGioi.z = 0f;
            Vector2 huong = ((Vector2)diemTheGioi - viTriSung).normalized;
            if (huong.y > 0.2f)
            {
                huongBan = huong;
            }
        }

        Vector3 dau = new Vector3(viTriSung.x, viTriSung.y, 0f);
        Vector3 cuoi = dau + (Vector3)(huongBan * 2.0f);
        thanSung.SetPosition(0, dau);
        thanSung.SetPosition(1, dau + (Vector3)(huongBan * 0.75f));
        duongNgam.SetPosition(0, dau + (Vector3)(huongBan * 0.75f));
        duongNgam.SetPosition(1, cuoi);

        if (CoBamBan())
        {
            BanBong();
        }
    }

    private bool LayViTriDauVao(out Vector3 viTriManHinh)
    {
        viTriManHinh = Vector3.zero;

        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed)
        {
            Vector2 viTriCham = Touchscreen.current.primaryTouch.position.ReadValue();
            viTriManHinh = new Vector3(viTriCham.x, viTriCham.y, 0f);
            return true;
        }

        if (Mouse.current != null)
        {
            Vector2 viTriChuot = Mouse.current.position.ReadValue();
            viTriManHinh = new Vector3(viTriChuot.x, viTriChuot.y, 0f);
            return true;
        }

        return false;
    }

    private bool CoBamChoiLai()
    {
        if (Keyboard.current != null && Keyboard.current.rKey.wasPressedThisFrame)
            return true;

        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
            return true;

        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
            return true;

        return false;
    }

    private bool CoBamBan()
    {
        if (Mouse.current != null && (Mouse.current.leftButton.wasPressedThisFrame || Mouse.current.leftButton.wasReleasedThisFrame))
            return true;

        if (Touchscreen.current != null && (Touchscreen.current.primaryTouch.press.wasPressedThisFrame || Touchscreen.current.primaryTouch.press.wasReleasedThisFrame))
            return true;

        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
            return true;

        return false;
    }

    private void BanBong()
    {
        if (bongDangBan == null || dangBay)
            return;

        dangBay = true;
        bongDangBan.HuongBay = huongBan.normalized;
        bongDangBan.gameObject.name = "Bóng đang bay";
    }

    private void CapNhatBongDangBay()
    {
        if (!dangBay || bongDangBan == null)
            return;

        Vector3 viTri = bongDangBan.transform.position;
        Vector2 huong = bongDangBan.HuongBay;
        viTri += (Vector3)(huong * tocDoBan * Time.deltaTime);

        if (viTri.x <= gioiHanTrai + banKinhBong)
        {
            viTri.x = gioiHanTrai + banKinhBong;
            huong.x = Mathf.Abs(huong.x);
        }
        else if (viTri.x >= gioiHanPhai - banKinhBong)
        {
            viTri.x = gioiHanPhai - banKinhBong;
            huong.x = -Mathf.Abs(huong.x);
        }

        bongDangBan.transform.position = viTri;
        bongDangBan.HuongBay = huong.normalized;

        if (viTri.y >= viTriDinhLuoi + banKinhBong * 0.25f || ChamVaoBongTrongLuoi(viTri))
        {
            GanBongVaoLuoi(viTri);
        }
    }

    private bool ChamVaoBongTrongLuoi(Vector3 viTri)
    {
        float khoangCham = banKinhBong * 1.75f;
        for (int h = 0; h < soHang; h++)
        {
            for (int c = 0; c < soCot; c++)
            {
                Bong bong = luoiBong[h, c];
                if (bong == null)
                    continue;

                if (Vector2.Distance(viTri, bong.transform.position) <= khoangCham)
                    return true;
            }
        }
        return false;
    }

    private void GanBongVaoLuoi(Vector3 viTri)
    {
        Vector2Int oGanNhat = TimOTrongGanNhat(viTri);
        if (oGanNhat.x < 0)
        {
            KetThucTroChoi("HẾT CHỖ BẮN!");
            return;
        }

        int hang = oGanNhat.x;
        int cot = oGanNhat.y;
        bongDangBan.Hang = hang;
        bongDangBan.Cot = cot;
        bongDangBan.transform.position = LayViTriO(hang, cot);
        bongDangBan.GetComponent<SpriteRenderer>().sortingOrder = 1;
        luoiBong[hang, cot] = bongDangBan;

        List<Vector2Int> cum = TimCumCungMau(hang, cot, bongDangBan.MaMau);
        if (cum.Count >= 3)
        {
            XoaCumBong(cum, 10);
            XoaBongRoiTuDo();
        }

        bongDangBan = null;
        dangBay = false;

        if (KiemTraThua())
        {
            KetThucTroChoi("GAME OVER");
            return;
        }

        if (DemSoBongConLai() == 0)
        {
            KetThucTroChoi("BẠN ĐÃ THẮNG!");
            return;
        }

        TaoBongMoiDeBan();
    }

    private Vector2Int TimOTrongGanNhat(Vector3 viTri)
    {
        float khoangGanNhat = float.MaxValue;
        Vector2Int oTotNhat = new Vector2Int(-1, -1);

        for (int h = 0; h < soHang; h++)
        {
            for (int c = 0; c < soCot; c++)
            {
                if (luoiBong[h, c] != null)
                    continue;

                Vector3 viTriO = LayViTriO(h, c);
                float khoang = Vector2.Distance(viTri, viTriO);
                if (khoang < khoangGanNhat)
                {
                    khoangGanNhat = khoang;
                    oTotNhat = new Vector2Int(h, c);
                }
            }
        }

        return oTotNhat;
    }

    private Vector3 LayViTriO(int hang, int cot)
    {
        float khoangCach = banKinhBong * 2.05f;
        float khoangDoc = khoangCach * 0.86f;
        float batDauX = -((soCot - 1) * khoangCach) / 2f;
        float leHang = (hang % 2 == 0) ? 0f : khoangCach * 0.5f;
        float x = batDauX + cot * khoangCach + leHang;
        float y = viTriDinhLuoi - hang * khoangDoc;
        return new Vector3(x, y, 0f);
    }

    private List<Vector2Int> TimCumCungMau(int hangBatDau, int cotBatDau, int maMau)
    {
        List<Vector2Int> ketQua = new List<Vector2Int>();
        bool[,] daTham = new bool[soHang, soCot];
        Queue<Vector2Int> hangDoi = new Queue<Vector2Int>();
        hangDoi.Enqueue(new Vector2Int(hangBatDau, cotBatDau));
        daTham[hangBatDau, cotBatDau] = true;

        while (hangDoi.Count > 0)
        {
            Vector2Int o = hangDoi.Dequeue();
            Bong bong = luoiBong[o.x, o.y];
            if (bong == null || bong.MaMau != maMau)
                continue;

            ketQua.Add(o);
            foreach (Vector2Int keBen in LayCacOKeBen(o.x, o.y))
            {
                if (!HopLe(keBen.x, keBen.y) || daTham[keBen.x, keBen.y])
                    continue;
                daTham[keBen.x, keBen.y] = true;
                hangDoi.Enqueue(keBen);
            }
        }

        return ketQua;
    }

    private void XoaCumBong(List<Vector2Int> cum, int diemMoiBong)
    {
        foreach (Vector2Int o in cum)
        {
            Bong bong = luoiBong[o.x, o.y];
            if (bong != null)
                Destroy(bong.gameObject);
            luoiBong[o.x, o.y] = null;
            diem += diemMoiBong;
        }
        CapNhatDiem();
    }

    private void XoaBongRoiTuDo()
    {
        bool[,] duocTreo = new bool[soHang, soCot];
        Queue<Vector2Int> hangDoi = new Queue<Vector2Int>();

        for (int c = 0; c < soCot; c++)
        {
            if (luoiBong[0, c] != null)
            {
                duocTreo[0, c] = true;
                hangDoi.Enqueue(new Vector2Int(0, c));
            }
        }

        while (hangDoi.Count > 0)
        {
            Vector2Int o = hangDoi.Dequeue();
            foreach (Vector2Int keBen in LayCacOKeBen(o.x, o.y))
            {
                if (!HopLe(keBen.x, keBen.y) || duocTreo[keBen.x, keBen.y] || luoiBong[keBen.x, keBen.y] == null)
                    continue;

                duocTreo[keBen.x, keBen.y] = true;
                hangDoi.Enqueue(keBen);
            }
        }

        List<Vector2Int> roi = new List<Vector2Int>();
        for (int h = 0; h < soHang; h++)
        {
            for (int c = 0; c < soCot; c++)
            {
                if (luoiBong[h, c] != null && !duocTreo[h, c])
                    roi.Add(new Vector2Int(h, c));
            }
        }

        XoaCumBong(roi, 5);
    }

    private List<Vector2Int> LayCacOKeBen(int hang, int cot)
    {
        bool hangLe = hang % 2 == 1;
        int[,] buocChan = { { 0, -1 }, { 0, 1 }, { -1, -1 }, { -1, 0 }, { 1, -1 }, { 1, 0 } };
        int[,] buocLe = { { 0, -1 }, { 0, 1 }, { -1, 0 }, { -1, 1 }, { 1, 0 }, { 1, 1 } };
        int[,] buoc = hangLe ? buocLe : buocChan;

        List<Vector2Int> ketQua = new List<Vector2Int>();
        for (int i = 0; i < 6; i++)
        {
            ketQua.Add(new Vector2Int(hang + buoc[i, 0], cot + buoc[i, 1]));
        }
        return ketQua;
    }

    private bool HopLe(int hang, int cot)
    {
        return hang >= 0 && hang < soHang && cot >= 0 && cot < soCot;
    }

    private bool KiemTraThua()
    {
        for (int h = soHang - 2; h < soHang; h++)
        {
            for (int c = 0; c < soCot; c++)
            {
                if (luoiBong[h, c] != null)
                    return true;
            }
        }
        return false;
    }

    private int DemSoBongConLai()
    {
        int dem = 0;
        for (int h = 0; h < soHang; h++)
        {
            for (int c = 0; c < soCot; c++)
            {
                if (luoiBong[h, c] != null)
                    dem++;
            }
        }
        return dem;
    }

    private void KetThucTroChoi(string tieuDe)
    {
        daKetThuc = true;
        dangBay = false;
        bangKetThuc.SetActive(true);
        chuKetThuc.text = tieuDe + "\n\nĐIỂM: " + diem + "\n\nNhấn R hoặc chạm để chơi lại";
    }

    private void ChoiLai()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
    }

    private void CapNhatDiem()
    {
        if (chuDiem != null)
            chuDiem.text = "ĐIỂM: " + diem;
    }

    private Sprite[] TaoTatCaSpriteBong()
    {
        // Nếu có ảnh trong Assets/Resources/AnhBong thì game sẽ dùng ảnh đó.
        // Nếu thiếu ảnh, game tự tạo bóng tròn đơn giản để vẫn chạy được.
        Sprite[] anhCoSan = Resources.LoadAll<Sprite>("AnhBong");
        Sprite[] sprites = new Sprite[bangMauBong.Length];

        for (int i = 0; i < sprites.Length; i++)
        {
            if (anhCoSan != null && i < anhCoSan.Length && anhCoSan[i] != null)
                sprites[i] = anhCoSan[i];
            else
                sprites[i] = TaoSpriteTron(bangMauBong[i]);
        }

        return sprites;
    }

    private Sprite TaoSpriteTron(Color mau)
    {
        int kichThuoc = 128;
        Texture2D tex = new Texture2D(kichThuoc, kichThuoc, TextureFormat.RGBA32, false);
        tex.filterMode = FilterMode.Bilinear;
        Vector2 tam = new Vector2(kichThuoc / 2f, kichThuoc / 2f);
        float banKinh = kichThuoc * 0.46f;

        for (int y = 0; y < kichThuoc; y++)
        {
            for (int x = 0; x < kichThuoc; x++)
            {
                float khoang = Vector2.Distance(new Vector2(x, y), tam);
                if (khoang > banKinh)
                {
                    tex.SetPixel(x, y, Color.clear);
                    continue;
                }

                float tiLe = khoang / banKinh;
                Color mauDiem = Color.Lerp(Color.white, mau, Mathf.Clamp01(tiLe * 0.85f));
                if (tiLe > 0.86f)
                    mauDiem = Color.Lerp(mauDiem, Color.black, 0.18f);
                tex.SetPixel(x, y, mauDiem);
            }
        }

        // Vệt sáng nhỏ ở góc trên trái giúp bóng giống kiểu cổ điển hơn.
        Vector2 diemSang = new Vector2(kichThuoc * 0.33f, kichThuoc * 0.68f);
        for (int y = 0; y < kichThuoc; y++)
        {
            for (int x = 0; x < kichThuoc; x++)
            {
                float khoang = Vector2.Distance(new Vector2(x, y), diemSang);
                if (khoang < kichThuoc * 0.12f)
                {
                    Color cu = tex.GetPixel(x, y);
                    if (cu.a > 0f)
                        tex.SetPixel(x, y, Color.Lerp(cu, Color.white, 0.55f));
                }
            }
        }

        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, kichThuoc, kichThuoc), new Vector2(0.5f, 0.5f), kichThuoc);
    }

    private Sprite TaoSpriteHinhVuong()
    {
        Texture2D tex = new Texture2D(8, 8, TextureFormat.RGBA32, false);
        for (int y = 0; y < 8; y++)
        {
            for (int x = 0; x < 8; x++)
                tex.SetPixel(x, y, Color.white);
        }
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, 8, 8), new Vector2(0.5f, 0.5f), 8);
    }
}

public class Bong : MonoBehaviour
{
    public int Hang;
    public int Cot;
    public int MaMau;
    public Vector2 HuongBay;
}
