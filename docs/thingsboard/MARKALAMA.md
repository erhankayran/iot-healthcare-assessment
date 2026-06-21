# Markalama / Logo Özelleştirme

## ThingsBoard CE sınırlaması

**ThingsBoard Community Edition (CE) White labeling menüsünü içermez.** Bu özellik ThingsBoard PE (Professional Edition)'da mevcuttur.

Koç Healthcare assessment için aşağıdaki alternatifler kullanılmıştır.

---

## Seçenek A — Dashboard logosu (ThingsBoard, önerilen)

Logoyu **Healthcare Vital Signs** dashboard'una ekleyin.

### 1. Görsel kaynağı yükle

1. **Tenant Administrator** giriş
2. **Kaynaklar** → **Resim kitaplığı** / **Images**
3. **+** → `assets/branding/logo.svg` yükle (SVG olmazsa PNG)
4. Ad: `koc-healthcare-logo`

### 2. Dashboard'a ekle

1. **Healthcare Vital Signs** dashboard → **Edit**
2. **Add widget** → **Markdown/HTML card** veya **Image**
3. Yüklenen logoyu seç
4. Üst kısma yerleştir
5. Kaydet

---

## Seçenek B — Simülatör markası (repoda hazır)

Blazor simülatörü proje logosunu gösterir:

- UI: `src/Simulator.Web/wwwroot/branding/logo.svg`
- Kaynak: `assets/branding/logo.svg`

```bash
dotnet run --project src/Simulator.Web/Simulator.Web.csproj
```

---

## Seçenek C — README / teslim notu

> Logo ve markalama .NET simülatör UI ve ThingsBoard dashboard'una uygulandı. Tam platform white-labeling ThingsBoard PE gerektirir; assessment kapsamında açık kaynak CE kullanıldı.

---

## Marka dosyaları

```
assets/branding/
  logo.svg       # Ana logo
  favicon.svg    # İsteğe bağlı ikon
```

---

## Değerlendirme checklist

- [x] Logo simülatör UI'da görünür
- [x] Logo ThingsBoard dashboard'unda (görsel widget)
- [x] Dashboard başlığı: `Healthcare Vital Signs`
- [x] README markalama yaklaşımını açıklıyor
