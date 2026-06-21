# Dashboard ve Canlı Grafikler

Simülatörden gelen kalp atışı ve SpO2 telemetrisi için gerçek zamanlı dashboard oluşturma.

## Dashboard oluştur

1. **Tenant Administrator** olarak giriş
2. **Dashboards** → **Add dashboard**
3. Başlık: `Healthcare Vital Signs`
4. Dashboard'u aç → **Edit**

## Kalp atışı grafiği

1. **Add widget**
2. **Charts** → **Time series chart**
3. **Datasource**: `Vital Signs Simulator` cihazı
4. **Series A**:
   - Key: `heartRate`
   - Label: `Heart Rate (bpm)`
   - Renk: kırmızı
5. **Time window**: Last 5 minutes veya Realtime
6. **Aggregation**: None (ham değerler)
7. Widget başlığı: `Heart Rate`

## SpO2 grafiği

Aynı adımlar:

- Key: `spo2`
- Label: `SpO2 (%)`
- Renk: mavi
- Başlık: `Blood Oxygen (SpO2)`

## İsteğe bağlı widget'lar

| Widget | Amaç |
|--------|------|
| **Latest values** | Güncel HR ve SpO2 sayısal gösterim |
| **Alarm status** | Aktif alarmlar (alarm kurulumundan sonra) |
| **Entity table** | Cihaz listesi ve son aktivite |

### Latest values widget

1. **Add widget** → **Cards** → **Latest values**
2. Cihaz ve anahtarlar: `heartRate`, `spo2`

## Önerilen layout

```
┌─────────────────────┬─────────────────────┐
│   Heart Rate Chart  │     SpO2 Chart      │
├─────────────────────┴─────────────────────┤
│        Latest Values / Alarm Status       │
└───────────────────────────────────────────┘
```

## Canlı grafikleri test et

1. Dashboard'u bir sekmede aç
2. Simülatörde **Random → Start streaming**
3. Grafikler ~2 saniyede bir güncellenmeli

## Demo ipuçları

- Sunumda dashboard **tam ekran** (F11)
- **Realtime** zaman penceresi kullan
- Dashboard'u değerlendirme gününden önce oluştur; demo sırasında yalnızca streaming başlat

## Telemetri anahtarları

Simülatörün publish ettiği anahtarlar:

| Key | Tip | Birim |
|-----|-----|-------|
| `heartRate` | integer | bpm |
| `spo2` | integer | % |
| `ts` | timestamp | ms (opsiyonel) |

Widget anahtarları birebir eşleşmeli (büyük/küçük harf duyarlı).
