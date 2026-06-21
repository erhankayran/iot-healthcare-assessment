# Değerlendirici Rehberi — Koç Healthcare IoT Assessment

**Buradan başlayın.** Bu belge projenin ne yaptığını, neden bu şekilde tasarlandığını ve tüm assessment maddelerinin ~15 dakikada nasıl doğrulanacağını açıklar.

---

## Hızlı bağlantılar

| Kaynak | Adres / konum |
|--------|---------------|
| **GitHub deposu** | https://github.com/erhankayran/iot-healthcare-assessment |
| **ThingsBoard (cloud UI)** | http://152.70.168.155:8080 |
| **MQTT broker (cloud)** | `152.70.168.155:1883` |
| **Mimari** | [MIMARI.md](MIMARI.md) |
| **Uyum matrisi** | [DEGERLENDIRME_UYUM.md](DEGERLENDIRME_UYUM.md) |
| **Teslim checklist** | [TESLIM.md](TESLIM.md) |

---

## Proje ne sunuyor?

Uçtan uca bir sağlık IoT demosu:

1. **ThingsBoard CE**, **Oracle Cloud** üzerinde çalışır (Docker/Podman + PostgreSQL).
2. **.NET 8 Blazor Server** simülatörü **Kalp Atışı (HR)** ve **SpO2** telemetrisini **MQTT** ile gönderir.
3. Platform **canlı grafikler**, eşik aşımında **Kritik alarmlar** ve **e-posta bildirimleri** üretir.
4. Simülatör **üç GUI modu** destekler: Manuel, Rastgele ve CSV yükleme.

```
┌─────────────────────────┐         MQTT          ┌──────────────────────────────┐
│  Simülatör (.NET 8)     │  ──────────────────►  │  ThingsBoard CE (Oracle VM)  │
│  Blazor Server UI       │     :1883             │  UI :8080  ·  PostgreSQL     │
│  Aday PC'sinde          │                       │  Dashboard · Alarm · E-posta │
└─────────────────────────┘                       └──────────────────────────────┘
```

> **Not:** Cloud'a yalnızca ThingsBoard deploy edilmiştir. Simülatör değerlendirme sırasında aday bilgisayarında çalışır ve cloud MQTT broker'a bağlanır. Bu, assessment hedefindeki “IoT platformu cloud'da” şartını karşılar; .NET arayüzünün ayrıca cloud'a deploy edilmesi gerekmez.

---

## Assessment hedefi eşlemesi

| Assessment isteği | Uygulama | Nasıl doğrulanır |
|-------------------|----------|------------------|
| ThingsBoard cloud'da | Oracle VM `152.70.168.155` | http://152.70.168.155:8080 aç |
| Kişiselleştirme (logo) | Dashboard banner + simülatör markası | Giriş → dashboard / markalama |
| 2 vital simülatör (HR + SpO2) | Tek cihaz, iki telemetri anahtarı | Cihaz → Son telemetri |
| Gerçek zamanlı veri | MQTT streaming | Random mod → canlı grafikler |
| Canlı grafikler | Dashboard widget'ları | **Healthcare Vital Signs** |
| Eşik alarmları | Cihaz profili kuralları | Manuel HR `130` → Kritik alarm |
| Bildirim (e-posta) | SMTP + Bildirim merkezi kuralı | Yeni alarmda e-posta |
| 3 GUI modu | Blazor `Home.razor` | Manuel / Random / CSV sekmeleri |
| Dokümantasyon | `docs/` Markdown | Bu rehber + bağlantılı dosyalar |
| Clean code | Clean Architecture | [MIMARI.md](MIMARI.md) |
| Unit test | xUnit (14 test) | `dotnet test` |

Tam matris: [DEGERLENDIRME_UYUM.md](DEGERLENDIRME_UYUM.md)

---

## Canlı demo ön koşulları

| Araç | Amaç |
|------|------|
| Web tarayıcı | ThingsBoard cloud UI |
| [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) | Simülatörü çalıştırma |
| Git | Depoyu klonlama |

---

## Adım 1 — Otomatik testleri doğrula (2 dk)

```bash
git clone https://github.com/erhankayran/iot-healthcare-assessment.git
cd iot-healthcare-assessment
dotnet test IoTHealthcareAssessment.sln
```

**Beklenen:** 14 test geçer, 0 hata.

| Test projesi | Kapsam |
|--------------|--------|
| `Simulator.Domain.Tests` | Vital sign validasyonu, aralık sabitleri |
| `Simulator.Application.Tests` | Random üretici, CSV ayrıştırıcı, publish servisi |

Detay: [TESTLER.md](TESTLER.md)

---

## Adım 2 — Cloud ThingsBoard'u incele (3 dk)

1. http://152.70.168.155:8080 aç
2. **Tenant admin** ile giriş — kimlik bilgileri aday tarafından değerlendirme e-postasında iletilir.

> `sysadmin@thingsboard.org` kullanmayın — değerlendirme için tenant admin yeterlidir.

3. Kontrol et:
   - **Cihazlar** → `Vital Signs Simulator`
   - **Dashboard'lar** → **Healthcare Vital Signs** (HR + SpO2 grafikleri)
   - **Cihaz profilleri** → **default** → **Alarm kuralları**:
     - `Heart Rate High` — `heartRate > 120`
     - `Heart Rate Low` — `heartRate < 50`
     - `SpO2 Low` — `spo2 < 90`

---

## Adım 3 — Simülatörü cloud'a bağla (5 dk)

```powershell
copy src\Simulator.Web\appsettings.example.json src\Simulator.Web\appsettings.local.json
```

`appsettings.local.json`:

```json
{
  "ThingsBoard": {
    "Host": "152.70.168.155",
    "Port": 1883,
    "AccessToken": "<ThingsBoard UI'dan cihaz token>"
  }
}
```

Token: **Cihazlar** → `Vital Signs Simulator` → **Erişim belirtecini kopyala**

```bash
dotnet run --project src/Simulator.Web/Simulator.Web.csproj
```

http://localhost:5229

### Mod A — Random (canlı akış)

1. **Random** seç
2. **Start streaming** tıkla
3. Cloud dashboard grafikleri ~2 saniyede bir güncellenmeli

### Mod B — Manuel (alarm + e-posta)

1. ThingsBoard **Alarmlar** → aktif alarmları **Temizle**
2. **Manual** seç
3. Kalp atışı: **130**, SpO2: **98**
4. **Send telemetry**
5. Doğrula:
   - Yeni **Heart Rate High** alarmı (Kritik)
   - Uygulama içi bildirim
   - Tenant admin e-posta kutusu

### Mod C — CSV (dosya yükleme)

1. **CSV upload** seç
2. `src/Simulator.Web/wwwroot/samples/vitals-sample.csv` yükle
3. **Stream from CSV** veya **Publish all once**
4. Cloud cihazda telemetriyi doğrula

---

## Adım 4 — E-posta bildirimleri (referans)

E-posta iki katmanda yapılandırılır:

| Katman | Yapılandırma |
|--------|--------------|
| **System Admin** | SMTP (Gmail) — Ayarlar → Posta sunucusu |
| **Tenant Admin** | Bildirim merkezi → Şablon + Alıcılar + Kural |

Rehber: [thingsboard/EMAIL_KURULUM.md](thingsboard/EMAIL_KURULUM.md)

---

## Teknoloji seçimleri (özet)

| Alan | Seçim | Gerekçe |
|------|-------|---------|
| Simülatör | .NET 8, C# | Assessment dil seçimini adaya bırakır; full-stack .NET gösterir |
| UI | Blazor Server | Tek stack, interaktif GUI, MQTT sunucu tarafında |
| IoT protokolü | MQTT (MQTTnet) | ThingsBoard yerel desteği |
| Platform | ThingsBoard CE 4.3 | Zorunlu IoT platformu; kural motoru + dashboard |
| Cloud | Oracle Cloud Free VM | Azure kotası engellendiğinde maliyetsiz deploy |
| Konteyner | Docker Compose (VM'de Podman) | Assessment konteynerizasyon şartı |
| Mimari | Clean Architecture | Domain / Application / Infrastructure / UI ayrımı |
| Test | xUnit | Standart .NET unit test |

Tam gerekçe: [MIMARI.md](MIMARI.md)

---

## Belge indeksi

| Belge | Hedef kitle | İçerik |
|-------|-------------|--------|
| [README.md](../README.md) | Herkes | Proje özeti, hızlı başlangıç |
| [DEGERLENDIRICI_REHBERI.md](DEGERLENDIRICI_REHBERI.md) | Değerlendiriciler | **Bu dosya** |
| [MIMARI.md](MIMARI.md) | Teknik | Tasarım, veri akışı |
| [DEGERLENDIRME_UYUM.md](DEGERLENDIRME_UYUM.md) | Değerlendiriciler | Madde checklist |
| [TESLIM.md](TESLIM.md) | Aday | URL'ler, demo günü |
| [TESTLER.md](TESTLER.md) | Geliştirici | Unit test kapsamı |
| [DEMO_SENARYOSU.md](DEMO_SENARYOSU.md) | Canlı sunum | 5 dk demo |

---

## İletişim

Depo sahibi: **Erhan Kayran** — https://github.com/erhankayran
