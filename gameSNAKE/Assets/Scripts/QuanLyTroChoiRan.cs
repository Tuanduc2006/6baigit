using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.EventSystems;

public class QuanLyTroChoiRan : MonoBehaviour
{
    [Header("Cấu hình bàn chơi")]
    [SerializeField] private int soCot = 15;
    [SerializeField] private int soHang = 22;
    [SerializeField] private float kichThuocO = 0.38f;
    [SerializeField] private float thoiGianMoiBuoc = 0.16f;

    private readonly List<Vector2Int> cacDotRan = new List<Vector2Int>();
    private readonly List<GameObject> hinhDotRan = new List<GameObject>();

    private Vector2Int huongDi = Vector2Int.right;
    private Vector2Int huongCho = Vector2Int.right;
    private Vector2Int viTriMoi;

    private bool dangChoi;
    private bool daChet;
    private int diem;
    private float demThoiGian;

    private Camera cameraChinh;
    private Transform nhomRan;
    private Transform nhomMoi;
    private Sprite spriteVuong;
    private Sprite spriteTron;
    private Sprite spriteDauRan;
    private Sprite spriteThanRan;
    private Sprite spriteMoi;

    private Canvas canvas;
    private GameObject manHinhBatDau;
    private GameObject manHinhKetThuc;
    private Text chuDiem;
    private Text chuKetThuc;

    private Vector2 viTriChamBatDau;
    private bool dangCham;

    private Color mauNen = new Color(0.11f, 0.17f, 0.22f);
    private Color mauTuong = new Color(0.26f, 0.39f, 0.50f);
    private Color mauDauRan = new Color(0.20f, 0.90f, 0.30f);
    private Color mauThanRan = new Color(0.12f, 0.65f, 0.22f);
    private Color mauMoi = new Color(1.00f, 0.20f, 0.18f);

    private void Start()
    {
        KhoiTaoTroChoi();
    }

    private void Update()
    {
        if (!dangChoi)
        {
            if (!daChet && KiemTraBamBatDau())
                BatDauChoi();
            else if (daChet && KiemTraBamChoiLai())
                ChoiLai();
            return;
        }

        DocDieuKhien();
        demThoiGian += Time.deltaTime;

        if (demThoiGian >= thoiGianMoiBuoc)
        {
            demThoiGian = 0f;
            DiChuyenRan();
        }
    }

    private void KhoiTaoTroChoi()
    {
        cameraChinh = Camera.main;
        if (cameraChinh == null)
        {
            GameObject objCamera = new GameObject("Camera Chính");
            cameraChinh = objCamera.AddComponent<Camera>();
            objCamera.AddComponent<AudioListener>();
            objCamera.tag = "MainCamera";
        }

        cameraChinh.orthographic = true;
        cameraChinh.orthographicSize = 5.1f;
        cameraChinh.backgroundColor = mauNen;
        cameraChinh.transform.position = new Vector3(0f, 0f, -10f);

        spriteVuong = TaoSpriteVuong();
        spriteTron = TaoSpriteTron();
        spriteDauRan = Resources.Load<Sprite>("AnhRan/dau_ran");
        spriteThanRan = Resources.Load<Sprite>("AnhRan/than_ran");
        spriteMoi = Resources.Load<Sprite>("AnhRan/moi");

        nhomRan = new GameObject("Nhóm rắn").transform;
        nhomMoi = new GameObject("Nhóm mồi").transform;

        TaoNenVaTuong();
        TaoGiaoDien();
        HienManHinhBatDau();
    }

    private void TaoNenVaTuong()
    {
        TaoOTrangTri("Nền bàn chơi", new Vector2(0f, 0f), soCot + 1.4f, soHang + 1.4f, new Color(0.16f, 0.24f, 0.30f), -20);
        TaoOTrangTri("Vùng chơi", new Vector2(0f, 0f), soCot, soHang, new Color(0.08f, 0.13f, 0.17f), -19);

        float rong = soCot * kichThuocO;
        float cao = soHang * kichThuocO;
        TaoOTrangTri("Tường trên", new Vector2(0f, cao / 2f + kichThuocO / 2f), soCot + 2, 1f, mauTuong, -10);
        TaoOTrangTri("Tường dưới", new Vector2(0f, -cao / 2f - kichThuocO / 2f), soCot + 2, 1f, mauTuong, -10);
        TaoOTrangTri("Tường trái", new Vector2(-rong / 2f - kichThuocO / 2f, 0f), 1f, soHang + 2, mauTuong, -10);
        TaoOTrangTri("Tường phải", new Vector2(rong / 2f + kichThuocO / 2f, 0f), 1f, soHang + 2, mauTuong, -10);
    }

    private GameObject TaoOTrangTri(string ten, Vector2 viTri, float rongTheoO, float caoTheoO, Color mau, int thuTuSapXep)
    {
        GameObject obj = new GameObject(ten);
        obj.transform.position = new Vector3(viTri.x, viTri.y, 0f);
        obj.transform.localScale = new Vector3(rongTheoO * kichThuocO, caoTheoO * kichThuocO, 1f);
        SpriteRenderer ve = obj.AddComponent<SpriteRenderer>();
        ve.sprite = spriteVuong;
        ve.color = mau;
        ve.sortingOrder = thuTuSapXep;
        return obj;
    }

    private void TaoGiaoDien()
    {
        TaoEventSystemNeuCan();

        GameObject objCanvas = new GameObject("Giao diện người chơi");
        canvas = objCanvas.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        CanvasScaler scaler = objCanvas.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1080f, 1920f);
        objCanvas.AddComponent<GraphicRaycaster>();

        chuDiem = TaoChu("Chữ điểm", "ĐIỂM: 0", canvas.transform, 54, Color.white, TextAnchor.MiddleCenter);
        DatRect(chuDiem.rectTransform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -55f), new Vector2(500f, 90f));

        TaoManHinhBatDau();
        TaoManHinhKetThuc();
    }

    private void TaoEventSystemNeuCan()
    {
        if (FindFirstObjectByType<EventSystem>() != null)
            return;

        GameObject obj = new GameObject("EventSystem");
        obj.AddComponent<EventSystem>();
        obj.AddComponent<InputSystemUIInputModule>();
    }

    private void TaoManHinhBatDau()
    {
        manHinhBatDau = new GameObject("Màn hình bắt đầu");
        manHinhBatDau.transform.SetParent(canvas.transform, false);
        Image nen = manHinhBatDau.AddComponent<Image>();
        nen.color = new Color(0f, 0f, 0f, 0.62f);
        DatRectFull(manHinhBatDau.GetComponent<RectTransform>());

        Text tieuDe = TaoChu("Tiêu đề", "RẮN SĂN MỒI", manHinhBatDau.transform, 76, new Color(0.80f, 1f, 0.42f), TextAnchor.MiddleCenter);
        DatRect(tieuDe.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, 220f), new Vector2(850f, 120f));

        Text huongDan = TaoChu("Hướng dẫn", "Ăn mồi để dài ra\nĐâm tường hoặc cắn thân sẽ chết\nĐiều khiển: WASD / phím mũi tên / vuốt", manHinhBatDau.transform, 34, Color.white, TextAnchor.MiddleCenter);
        DatRect(huongDan.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, 45f), new Vector2(880f, 240f));

        Button nutBatDau = TaoNut("Nút bắt đầu", "BẮT ĐẦU", manHinhBatDau.transform, new Vector2(0f, -190f));
        nutBatDau.onClick.AddListener(BatDauChoi);
    }

    private void TaoManHinhKetThuc()
    {
        manHinhKetThuc = new GameObject("Màn hình kết thúc");
        manHinhKetThuc.transform.SetParent(canvas.transform, false);
        Image nen = manHinhKetThuc.AddComponent<Image>();
        nen.color = new Color(0f, 0f, 0f, 0.62f);
        DatRectFull(manHinhKetThuc.GetComponent<RectTransform>());

        chuKetThuc = TaoChu("Chữ kết thúc", "", manHinhKetThuc.transform, 54, Color.white, TextAnchor.MiddleCenter);
        DatRect(chuKetThuc.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, 110f), new Vector2(850f, 280f));

        Button nutChoiLai = TaoNut("Nút chơi lại", "CHƠI LẠI", manHinhKetThuc.transform, new Vector2(0f, -180f));
        nutChoiLai.onClick.AddListener(ChoiLai);

        manHinhKetThuc.SetActive(false);
    }

    private Button TaoNut(string ten, string noiDung, Transform cha, Vector2 viTri)
    {
        GameObject obj = new GameObject(ten);
        obj.transform.SetParent(cha, false);
        Image anh = obj.AddComponent<Image>();
        anh.color = new Color(0.28f, 0.82f, 0.28f);

        Button nut = obj.AddComponent<Button>();
        ColorBlock mauNut = nut.colors;
        mauNut.normalColor = new Color(0.28f, 0.82f, 0.28f);
        mauNut.highlightedColor = new Color(0.38f, 0.92f, 0.38f);
        mauNut.pressedColor = new Color(0.16f, 0.60f, 0.16f);
        nut.colors = mauNut;

        DatRect(obj.GetComponent<RectTransform>(), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), viTri, new Vector2(420f, 115f));

        Text chu = TaoChu("Chữ " + ten, noiDung, obj.transform, 42, Color.white, TextAnchor.MiddleCenter);
        DatRectFull(chu.rectTransform);
        return nut;
    }

    private Text TaoChu(string ten, string noiDung, Transform cha, int coChu, Color mau, TextAnchor canLe)
    {
        GameObject obj = new GameObject(ten);
        obj.transform.SetParent(cha, false);
        Text chu = obj.AddComponent<Text>();
        chu.text = noiDung;
        chu.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        chu.fontSize = coChu;
        chu.fontStyle = FontStyle.Bold;
        chu.color = mau;
        chu.alignment = canLe;
        chu.raycastTarget = false;
        return chu;
    }

    private void DatRect(RectTransform rect, Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot, Vector2 anchoredPosition, Vector2 sizeDelta)
    {
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.pivot = pivot;
        rect.anchoredPosition = anchoredPosition;
        rect.sizeDelta = sizeDelta;
    }

    private void DatRectFull(RectTransform rect)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }

    private void HienManHinhBatDau()
    {
        dangChoi = false;
        daChet = false;
        manHinhBatDau.SetActive(true);
        manHinhKetThuc.SetActive(false);
    }

    private void BatDauChoi()
    {
        XoaTatCaDoiTuongGame();
        diem = 0;
        huongDi = Vector2Int.right;
        huongCho = Vector2Int.right;
        demThoiGian = 0f;
        dangChoi = true;
        daChet = false;
        manHinhBatDau.SetActive(false);
        manHinhKetThuc.SetActive(false);

        TaoRanBanDau();
        TaoMoiMoi();
        CapNhatDiem();
    }

    private void XoaTatCaDoiTuongGame()
    {
        foreach (GameObject obj in hinhDotRan)
        {
            if (obj != null) Destroy(obj);
        }
        hinhDotRan.Clear();
        cacDotRan.Clear();

        for (int i = nhomMoi.childCount - 1; i >= 0; i--)
            Destroy(nhomMoi.GetChild(i).gameObject);
    }

    private void TaoRanBanDau()
    {
        Vector2Int dau = new Vector2Int(soCot / 2, soHang / 2);
        cacDotRan.Add(dau);
        cacDotRan.Add(dau + Vector2Int.left);
        cacDotRan.Add(dau + Vector2Int.left * 2);

        for (int i = 0; i < cacDotRan.Count; i++)
        {
            GameObject dot = TaoDotRan(i == 0 ? "Đầu rắn" : "Thân rắn", cacDotRan[i], i == 0);
            hinhDotRan.Add(dot);
        }
    }

    private GameObject TaoDotRan(string ten, Vector2Int o, bool laDau)
    {
        GameObject obj = new GameObject(ten);
        obj.transform.SetParent(nhomRan, false);
        obj.transform.position = ChuyenOThanhViTri(o);
        obj.transform.localScale = Vector3.one * kichThuocO * 0.92f;
        SpriteRenderer ve = obj.AddComponent<SpriteRenderer>();
        ve.sprite = laDau ? (spriteDauRan != null ? spriteDauRan : spriteVuong) : (spriteThanRan != null ? spriteThanRan : spriteVuong);
        ve.color = (laDau && spriteDauRan != null) || (!laDau && spriteThanRan != null) ? Color.white : (laDau ? mauDauRan : mauThanRan);
        ve.sortingOrder = laDau ? 3 : 2;
        return obj;
    }

    private void TaoMoiMoi()
    {
        for (int lanThu = 0; lanThu < 500; lanThu++)
        {
            Vector2Int oMoi = new Vector2Int(Random.Range(0, soCot), Random.Range(0, soHang));
            if (cacDotRan.Contains(oMoi))
                continue;

            viTriMoi = oMoi;
            GameObject objMoi = new GameObject("Mồi");
            objMoi.transform.SetParent(nhomMoi, false);
            objMoi.transform.position = ChuyenOThanhViTri(viTriMoi);
            objMoi.transform.localScale = Vector3.one * kichThuocO * 0.80f;
            SpriteRenderer ve = objMoi.AddComponent<SpriteRenderer>();
            ve.sprite = spriteMoi != null ? spriteMoi : spriteTron;
            ve.color = spriteMoi != null ? Color.white : mauMoi;
            ve.sortingOrder = 4;
            return;
        }
    }

    private void DocDieuKhien()
    {
        if (Keyboard.current != null)
        {
            if ((Keyboard.current.upArrowKey.wasPressedThisFrame || Keyboard.current.wKey.wasPressedThisFrame) && huongDi != Vector2Int.down)
                huongCho = Vector2Int.up;
            else if ((Keyboard.current.downArrowKey.wasPressedThisFrame || Keyboard.current.sKey.wasPressedThisFrame) && huongDi != Vector2Int.up)
                huongCho = Vector2Int.down;
            else if ((Keyboard.current.leftArrowKey.wasPressedThisFrame || Keyboard.current.aKey.wasPressedThisFrame) && huongDi != Vector2Int.right)
                huongCho = Vector2Int.left;
            else if ((Keyboard.current.rightArrowKey.wasPressedThisFrame || Keyboard.current.dKey.wasPressedThisFrame) && huongDi != Vector2Int.left)
                huongCho = Vector2Int.right;
        }

        DocVuotManHinh();
    }

    private void DocVuotManHinh()
    {
        if (Touchscreen.current == null)
            return;

        var touch = Touchscreen.current.primaryTouch;
        if (touch.press.wasPressedThisFrame)
        {
            viTriChamBatDau = touch.position.ReadValue();
            dangCham = true;
        }

        if (dangCham && touch.press.wasReleasedThisFrame)
        {
            Vector2 ketThuc = touch.position.ReadValue();
            Vector2 lech = ketThuc - viTriChamBatDau;
            dangCham = false;

            if (lech.magnitude < 40f)
                return;

            if (Mathf.Abs(lech.x) > Mathf.Abs(lech.y))
            {
                if (lech.x > 0 && huongDi != Vector2Int.left) huongCho = Vector2Int.right;
                else if (lech.x < 0 && huongDi != Vector2Int.right) huongCho = Vector2Int.left;
            }
            else
            {
                if (lech.y > 0 && huongDi != Vector2Int.down) huongCho = Vector2Int.up;
                else if (lech.y < 0 && huongDi != Vector2Int.up) huongCho = Vector2Int.down;
            }
        }
    }

    private void DiChuyenRan()
    {
        huongDi = huongCho;
        Vector2Int dauCu = cacDotRan[0];
        Vector2Int dauMoi = dauCu + huongDi;

        bool anMoi = dauMoi == viTriMoi;

        if (DauChamTuong(dauMoi) || DauCanThan(dauMoi, anMoi))
        {
            KetThucTroChoi();
            return;
        }

        cacDotRan.Insert(0, dauMoi);
        GameObject dauMoiObj = TaoDotRan("Đầu rắn", dauMoi, true);
        hinhDotRan.Insert(0, dauMoiObj);

        if (hinhDotRan.Count > 1)
        {
            hinhDotRan[1].name = "Thân rắn";
            SpriteRenderer veThanCu = hinhDotRan[1].GetComponent<SpriteRenderer>();
            veThanCu.sprite = spriteThanRan != null ? spriteThanRan : spriteVuong;
            veThanCu.color = spriteThanRan != null ? Color.white : mauThanRan;
            veThanCu.sortingOrder = 2;
        }

        if (anMoi)
        {
            diem += 10;
            CapNhatDiem();
            XoaMoiCu();
            TaoMoiMoi();
        }
        else
        {
            int cuoi = cacDotRan.Count - 1;
            cacDotRan.RemoveAt(cuoi);
            Destroy(hinhDotRan[cuoi]);
            hinhDotRan.RemoveAt(cuoi);
        }
    }

    private bool DauChamTuong(Vector2Int dau)
    {
        return dau.x < 0 || dau.x >= soCot || dau.y < 0 || dau.y >= soHang;
    }

    private bool DauCanThan(Vector2Int dauMoi, bool anMoi)
    {
        int gioiHanKiemTra = anMoi ? cacDotRan.Count : cacDotRan.Count - 1;
        for (int i = 0; i < gioiHanKiemTra; i++)
        {
            if (cacDotRan[i] == dauMoi)
                return true;
        }
        return false;
    }

    private void XoaMoiCu()
    {
        for (int i = nhomMoi.childCount - 1; i >= 0; i--)
            Destroy(nhomMoi.GetChild(i).gameObject);
    }

    private void KetThucTroChoi()
    {
        dangChoi = false;
        daChet = true;
        manHinhKetThuc.SetActive(true);
        chuKetThuc.text = "GAME OVER\n\nĐIỂM: " + diem + "\n\nBấm CHƠI LẠI hoặc nhấn R";
    }

    private bool KiemTraBamBatDau()
    {
        if (Keyboard.current != null &&
            (Keyboard.current.enterKey.wasPressedThisFrame || Keyboard.current.spaceKey.wasPressedThisFrame))
            return true;

        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
            return true;

        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
            return true;

        return false;
    }

    private bool KiemTraBamChoiLai()
    {
        if (Keyboard.current != null && Keyboard.current.rKey.wasPressedThisFrame)
            return true;

        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
            return true;

        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
            return true;

        return false;
    }

    private void ChoiLai()
    {
        BatDauChoi();
    }

    private void CapNhatDiem()
    {
        if (chuDiem != null)
            chuDiem.text = "ĐIỂM: " + diem;
    }

    private Vector3 ChuyenOThanhViTri(Vector2Int o)
    {
        float x = (o.x - (soCot - 1) / 2f) * kichThuocO;
        float y = (o.y - (soHang - 1) / 2f) * kichThuocO;
        return new Vector3(x, y, 0f);
    }

    private Sprite TaoSpriteVuong()
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

    private Sprite TaoSpriteTron()
    {
        int kichThuoc = 64;
        Texture2D tex = new Texture2D(kichThuoc, kichThuoc, TextureFormat.RGBA32, false);
        Vector2 tam = new Vector2(kichThuoc / 2f, kichThuoc / 2f);
        float banKinh = kichThuoc * 0.45f;

        for (int y = 0; y < kichThuoc; y++)
        {
            for (int x = 0; x < kichThuoc; x++)
            {
                float khoang = Vector2.Distance(new Vector2(x, y), tam);
                tex.SetPixel(x, y, khoang <= banKinh ? Color.white : Color.clear);
            }
        }

        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, kichThuoc, kichThuoc), new Vector2(0.5f, 0.5f), kichThuoc);
    }
}
