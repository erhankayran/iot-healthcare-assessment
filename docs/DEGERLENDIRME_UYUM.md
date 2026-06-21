# Assessment Uyum Checklist

Assessment maddeleri ile proje durumu (güncel denetim).

**Son test:** `dotnet test` — 14 test geçti, 0 hata, 0 uyarı.

---

## Zorunlu gereksinimler

| # | Gereksinim | Durum | Kanıt |
|---|------------|-------|-------|
| 1 | GitHub/Bitbucket/GitLab/Azure repo URL | **TAMAM** | https://github.com/erhankayran/iot-healthcare-assessment |
| 2 | ThingsBoard IoT platformu cloud'da | **TAMAM** | http://152.70.168.155:8080 — [ORACLE_CLOUD_DEPLOY.md](thingsboard/ORACLE_CLOUD_DEPLOY.md) |
| 3 | Platform kişiselleştirme (logo vb.) | **TAMAM** | Dashboard banner + simülatör markası — [MARKALAMA.md](thingsboard/MARKALAMA.md) |
| 4 | 2 sağlık vital simülatörü (HR + SpO2) | **TAMAM** | `Simulator.Domain` + MQTT payload (`heartRate`, `spo2`) |
| 5 | Simülatör dili serbest seçim | **TAMAM** | .NET 8 C# |
| 6 | Simülatör dokümante + mimari Markdown | **TAMAM** | `README.md`, `docs/MIMARI.md`, `docs/thingsboard/*` |
| 7 | 3 mod: Manuel / Dosya / Random (GUI) | **TAMAM** | Blazor Server `Home.razor` |
| 8 | Uyarı: e-posta, SMS veya iletişim | **TAMAM** | SMTP + bildirim kuralı — [EMAIL_KURULUM.md](thingsboard/EMAIL_KURULUM.md) |
| 9 | Değerlendirme sırasında gerçek zamanlı veri | **TAMAM** | MQTT streaming + canlı dashboard |
| 10 | Clean code | **TAMAM** | Clean Architecture (Domain / Application / Infrastructure / Web) |
| 11 | Unit testing | **TAMAM** | xUnit — 14 test — [TESTLER.md](TESTLER.md) |

---

## Assessment hedefi (Objective)

| Hedef | Durum |
|-------|-------|
| Konteynerizasyon | Docker Compose (TB + PostgreSQL) |
| Back-end geliştirme | .NET Application + Infrastructure |
| Front-end geliştirme | Blazor Server UI |
| Üçüncü parti platform | ThingsBoard CE cloud'da |
| Canlı grafikler | Healthcare Vital Signs dashboard |
| Eşik alarmları | 3 cihaz profili kuralı (Kritik) |

---

## Solution dosyası

| Dosya | Amaç |
|-------|------|
| `IoTHealthcareAssessment.sln` | Visual Studio / `dotnet sln` (6 proje) |
| `IoTHealthcareAssessment.slnx` | Visual Studio 2022+ XML solution |

```bash
dotnet build IoTHealthcareAssessment.sln
dotnet test IoTHealthcareAssessment.sln
```

---

## Manuel doğrulama (değerlendirici)

Canlı ortamda test edilmiş:

- [x] Cloud ThingsBoard UI erişilebilir
- [x] Simülatör Manual mod → cloud telemetri
- [x] Simülatör Random mod → canlı grafikler
- [x] Simülatör CSV mod → satır publish
- [x] HR 130 → Kritik alarm
- [x] Yeni alarm → e-posta + web bildirimi
- [x] `dotnet test` — 14/14 geçti

Adım adım: [DEGERLENDIRICI_REHBERI.md](DEGERLENDIRICI_REHBERI.md)

---

## Güçlü yönler

- Uçtan uca .NET simülatör + MQTT
- ThingsBoard Docker stack (local + cloud)
- Canlı dashboard (HR + SpO2)
- Üç cihaz profili alarm kuralı (Kritik)
- E-posta + web bildirimleri
- Türkçe dokümantasyon
- Unit testler
- Markalama (simülatör + dashboard)
- Gizliler git dışında (`appsettings.local.json`)
