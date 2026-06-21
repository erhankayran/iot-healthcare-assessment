# IoT Healthcare Assessment

Koç Healthcare Full Stack Developer değerlendirme projesi — **ThingsBoard** IoT platformu, **.NET 8 Blazor Server** vital signs simülatörü, canlı dashboard, eşik alarmları ve e-posta bildirimleri.

> **Değerlendiriciler için:** [docs/DEGERLENDIRICI_REHBERI.md](docs/DEGERLENDIRICI_REHBERI.md) dosyasından başlayın.

---

## Canlı ortam

| Kaynak | Adres |
|--------|--------|
| **GitHub** | https://github.com/erhankayran/iot-healthcare-assessment |
| **ThingsBoard (cloud)** | http://152.70.168.155:8080 |
| **MQTT broker** | `152.70.168.155:1883` |
| **Tenant admin** | Değerlendirme e-postasında iletilir |
| **Simülatör UI** | `http://localhost:5229` (değerlendirme sırasında aday PC'sinde) |

---

## Proje özeti

| Bileşen | Teknoloji | Nerede çalışır |
|---------|-----------|----------------|
| IoT platformu | ThingsBoard CE 4.3 + PostgreSQL | Oracle Cloud VM |
| Simülatör | .NET 8, Blazor Server, MQTTnet | Aday bilgisayarı |
| Konteyner | Docker Compose (Podman) | Cloud VM |
| Testler | xUnit (14 test) | Yerel / CI |

---

## Mimari

```
┌─────────────────────────┐         MQTT          ┌──────────────────────────────┐
│  Simülatör (.NET 8)     │  ──────────────────►  │  ThingsBoard CE (Oracle VM)   │
│  Blazor Server UI       │     :1883             │  UI :8080  ·  PostgreSQL    │
│  Aday PC'sinde          │                       │  Dashboard · Alarm · E-posta │
└─────────────────────────┘                       └──────────────────────────────┘
```

Detaylı mimari: [docs/MIMARI.md](docs/MIMARI.md)

---

## Hızlı başlangıç

### 1. Depoyu klonla

```powershell
git clone https://github.com/erhankayran/iot-healthcare-assessment.git
cd iot-healthcare-assessment
```

### 2. Simülatör yapılandırması

```powershell
copy src\Simulator.Web\appsettings.example.json src\Simulator.Web\appsettings.local.json
```

`appsettings.local.json` (git'e girmez):

```json
{
  "ThingsBoard": {
    "Host": "152.70.168.155",
    "Port": 1883,
    "AccessToken": "CIHAZ_TOKEN_BURAYA"
  }
}
```

Token: ThingsBoard → **Cihazlar** → `Vital Signs Simulator` → **Erişim belirtecini kopyala**

### 3. Simülatörü çalıştır

```powershell
dotnet run --project src/Simulator.Web/Simulator.Web.csproj
```

Tarayıcı: http://localhost:5229

### 4. Testleri çalıştır

```powershell
dotnet test IoTHealthcareAssessment.sln
```

**Beklenen:** 14 test geçer, 0 hata.

---

## Simülatör modları

| Mod | Açıklama |
|-----|----------|
| **Manual** | Kullanıcı HR ve SpO2 değerini girer |
| **Random** | Gerçekçi aralıkta rastgele değer üretir (~2 sn aralık) |
| **CSV** | `HeartRate,SpO2` sütunlu dosya yükler |

Örnek CSV: `src/Simulator.Web/wwwroot/samples/vitals-sample.csv`

---

## ThingsBoard rehberleri

| Rehber | Konu |
|--------|------|
| [KURULUM.md](docs/thingsboard/KURULUM.md) | Yerel Docker kurulumu |
| [DASHBOARD.md](docs/thingsboard/DASHBOARD.md) | Canlı HR + SpO2 grafikleri |
| [ALARMLAR.md](docs/thingsboard/ALARMLAR.md) | Eşik alarm kuralları |
| [EMAIL_KURULUM.md](docs/thingsboard/EMAIL_KURULUM.md) | SMTP + e-posta bildirimleri |
| [MARKALAMA.md](docs/thingsboard/MARKALAMA.md) | Logo ve markalama |
| [ORACLE_CLOUD_DEPLOY.md](docs/thingsboard/ORACLE_CLOUD_DEPLOY.md) | Oracle Cloud deploy |
| [CLOUD_DEPLOY.md](docs/thingsboard/CLOUD_DEPLOY.md) | Azure alternatifi |

---

## Dokümantasyon indeksi

| Dosya | Hedef kitle | İçerik |
|-------|-------------|--------|
| [DEGERLENDIRICI_REHBERI.md](docs/DEGERLENDIRICI_REHBERI.md) | Değerlendiriciler | Doğrulama adımları (~15 dk) |
| [MIMARI.md](docs/MIMARI.md) | Teknik | Tasarım, veri akışı, kararlar |
| [DEGERLENDIRME_UYUM.md](docs/DEGERLENDIRME_UYUM.md) | Değerlendiriciler | Assessment maddeleri |
| [TESLIM.md](docs/TESLIM.md) | Aday | URL'ler, demo checklist |
| [TESTLER.md](docs/TESTLER.md) | Geliştirici | Unit test kapsamı |
| [DEMO_SENARYOSU.md](docs/DEMO_SENARYOSU.md) | Canlı sunum | 5 dakikalık demo akışı |

---

## Solution yapısı

```
src/
  Simulator.Domain/          # Modeller, validasyon, sabitler
  Simulator.Application/     # Manuel / random / CSV mantığı
  Simulator.Infrastructure/  # MQTTnet → ThingsBoard
  Simulator.Web/             # Blazor Server UI
docker/
  docker-compose.yml         # ThingsBoard CE + PostgreSQL
docs/                        # Türkçe dokümantasyon
tests/                       # xUnit testleri
assets/branding/             # Logo dosyaları
```

Solution dosyası: `IoTHealthcareAssessment.sln`

---

## Telemetri formatı

```json
{ "heartRate": 72, "spo2": 98, "ts": 1718820000000 }
```

---

## Alarm eşikleri (ThingsBoard)

| Metrik | Alarm koşulu |
|--------|--------------|
| Kalp atışı | `< 50` veya `> 120` bpm |
| SpO2 | `< 90` % |

Simülatör doğrulama aralıkları (girdi kontrolü): HR 40–200, SpO2 85–100.

---

## Assessment uyumu

Tüm zorunlu maddeler tamamlandı. Detay: [docs/DEGERLENDIRME_UYUM.md](docs/DEGERLENDIRME_UYUM.md)

---

## Lisans

Koç Healthcare işe alım değerlendirme projesi.
