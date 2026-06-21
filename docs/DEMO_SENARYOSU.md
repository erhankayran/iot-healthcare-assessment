# Canlı Demo Senaryosu (~5 dakika)

Koç Healthcare değerlendirme oturumunda kullanılacak akış.

---

## Hazırlık (görüşme öncesi)

| Öğe | Durum |
|-----|-------|
| Cloud ThingsBoard | http://152.70.168.155:8080 |
| Simülatör yapılandırması | `appsettings.local.json` → cloud host + token |
| Simülatör çalışıyor | http://localhost:5229 |
| Tarayıcı sekmeleri | Sekme 1: cloud dashboard · Sekme 2: simülatör |

---

## Senaryo

### 1. Giriş (30 sn)

> "Uçtan uca bir sağlık IoT platformu geliştirdim. ThingsBoard Oracle Cloud'da çalışıyor; .NET simülatörü kalp atışı ve SpO2 verisini MQTT ile gönderiyor. Repoda Clean Architecture, 14 unit test ve Türkçe Markdown dokümantasyon var."

Göster: GitHub → `docs/DEGERLENDIRICI_REHBERI.md`

---

### 2. Mimari (45 sn)

> "Simülatör dört katmanlı: Domain, Application, Infrastructure ve Blazor Web. ThingsBoard depolama, dashboard, alarm ve e-postayı yönetiyor."

Göster: [MIMARI.md](MIMARI.md) diyagramı veya cloud dashboard.

---

### 3. Canlı akış — Random mod (1 dk)

1. Simülatör → **Random** → **Start streaming**
2. Cloud dashboard → **Healthcare Vital Signs**
3. HR ve SpO2 grafiklerinin gerçek zamanlı güncellendiğini göster

> "Telemetri yaklaşık iki saniyede bir MQTT ile cloud broker'a gidiyor."

---

### 4. Dosya modu — CSV (45 sn)

1. Simülatör → **CSV upload**
2. `vitals-sample.csv` yükle
3. **Stream from CSV**

> "Assessment üç mod istiyor — hepsi GUI'den seçilebiliyor: Manuel, dosya ve rastgele."

---

### 5. Alarm + e-posta — Manual mod (1 dk 30 sn)

1. ThingsBoard → **Alarmlar** → aktif alarmları **Temizle**
2. Simülatör → **Manual**
3. Kalp atışı: **130**, SpO2: **98**
4. **Send telemetry**
5. Göster:
   - Yeni **Heart Rate High** alarmı (Kritik)
   - Uygulama içi bildirim zili
   - E-posta gelen kutusu

> "Eşikler: HR 50 altı veya 120 üstü, SpO2 90 altı. E-posta SMTP ve Bildirim merkezi kurallarıyla gidiyor."

---

### 6. Testler ve kod kalitesi (45 sn)

```bash
dotnet test IoTHealthcareAssessment.sln
```

> "14 unit test validasyon, CSV ayrıştırma, random üretim ve publish mantığını kapsıyor."

---

### 7. Kapanış (15 sn)

> "Repo URL, cloud URL ve değerlendirici rehberi TESLIM.md'de. İstediğiniz katmana derinleşebiliriz."

---

## MQTT sorun çıkarsa

1. `appsettings.local.json` içindeki cihaz token'ını kontrol et
2. Port 1883: `Test-NetConnection 152.70.168.155 -Port 1883`
3. Yapılandırma değişikliğinden sonra simülatörü yeniden başlat

---

## Eşik referansı

| Metrik | Alarm koşulu |
|--------|--------------|
| Kalp atışı | `< 50` veya `> 120` bpm |
| SpO2 | `< 90` % |

Simülatör geçerli girdi aralıkları: HR 40–200, SpO2 85–100.
