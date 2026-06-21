# Mimari Dokümantasyon

Bu belge sistemin **neyi, neden ve nasıl** yaptığını açıklar.

---

## Genel bakış

Simülatör **Clean Architecture** ile dört projeye ayrılmıştır:

1. **Simulator.Domain** — modeller, validasyon, sabitler
2. **Simulator.Application** — use case'ler ve soyutlamalar
3. **Simulator.Infrastructure** — MQTT / ThingsBoard entegrasyonu
4. **Simulator.Web** — Blazor Server sunum katmanı

Bağımlılıklar içe doğru akar:

```
Web → Application → Domain
Web → Infrastructure → Application → Domain
```

---

## Sistem mimarisi (cloud + local)

```
┌─────────────────────────────────────────────────────────────────┐
│                        ADAY BİLGİSAYARI                          │
│  ┌───────────────────────────────────────────────────────────┐  │
│  │  Simulator.Web (Blazor Server)                             │  │
│  │  · Manuel / Random / CSV modları                           │  │
│  │  · Home.razor GUI                                          │  │
│  └─────────────────────────┬─────────────────────────────────┘  │
│                            │                                      │
│  ┌─────────────────────────▼─────────────────────────────────┐  │
│  │  Application + Domain + Infrastructure                     │  │
│  │  · Validasyon · CSV parse · Random üretim                  │  │
│  │  · MQTTnet publisher                                       │  │
│  └─────────────────────────┬─────────────────────────────────┘  │
└────────────────────────────┼────────────────────────────────────┘
                             │ MQTT :1883
                             ▼
┌─────────────────────────────────────────────────────────────────┐
│              ORACLE CLOUD VM (152.70.168.155)                    │
│  ┌───────────────────────────────────────────────────────────┐  │
│  │  ThingsBoard CE 4.3  (:8080 UI, :1883 MQTT)               │  │
│  │  · Telemetri depolama · Dashboard · Alarm motoru          │  │
│  │  · Bildirim merkezi · SMTP e-posta                        │  │
│  └─────────────────────────┬─────────────────────────────────┘  │
│  ┌─────────────────────────▼─────────────────────────────────┐  │
│  │  PostgreSQL 16                                             │  │
│  └───────────────────────────────────────────────────────────┘  │
└─────────────────────────────────────────────────────────────────┘
```

**Neden simülatör local?** Assessment cloud'da **ThingsBoard platformunu** ister; simülatör GUI'sinin cloud'da olması zorunlu değildir. Simülatör MQTT ile cloud'a veri gönderir; değerlendirici aynı akışı kendi makinesinde tekrarlayabilir.

---

## Telemetri akışı

1. Kullanıcı Blazor UI'da mod seçer (Manuel / Random / CSV).
2. Application katmanı `VitalTelemetry` oluşturur veya CSV'den ayrıştırır.
3. Domain validasyonu gerçekçi vital aralıklarını zorlar.
4. Infrastructure, JSON telemetriyi MQTTnet ile ThingsBoard'a publish eder.
5. ThingsBoard telemetriyi kaydeder, dashboard günceller, alarm kurallarını değerlendirir.
6. Alarm oluşursa Bildirim merkezi web + e-posta bildirimi tetikler.

---

## Tasarım kararları

| Karar | Seçim | Neden |
|-------|-------|-------|
| UI | Blazor Server (.NET 8) | Ayrı frontend framework gerekmez; full-stack .NET |
| IoT protokolü | MQTT | ThingsBoard yerel desteği, düşük overhead |
| Yapılandırma | `appsettings.json` + `appsettings.local.json` | ASP.NET Core standardı; token git'e girmez |
| Test | xUnit | Yaygın .NET test stack'i |
| IoT platformu | ThingsBoard CE | Assessment gereksinimi; açık kaynak |
| Cloud | Oracle Always Free | Azure kotası engelinde $0 deploy |
| Konteyner | Docker Compose | Tek komutla TB + PostgreSQL |
| Alarm | Cihaz profili kuralları | UI üzerinden yapılandırılabilir, script gerektirmez |
| E-posta | SMTP + Bildirim merkezi | Assessment “e-posta/SMS/iletişim” maddesi |

---

## Katman sorumlulukları

### Simulator.Domain

- `VitalTelemetry` kaydı (HeartRate, SpO2, Timestamp)
- `VitalSignRanges` — kabul edilen ve random aralıklar
- `VitalTelemetryValidator` — geçersiz girdiyi publish öncesi reddeder

### Simulator.Application

- `TelemetrySimulationService` — manuel publish, streaming orchestration
- `RandomTelemetryGenerator` — gerçekçi random değerler
- `CsvTelemetryParser` — `HeartRate,SpO2` CSV ayrıştırma
- `ITelemetryPublisher` soyutlaması

### Simulator.Infrastructure

- `MqttTelemetryPublisher` — MQTTnet ile ThingsBoard'a bağlanır
- `ThingsBoardOptions` — Host, Port, AccessToken, topic, interval

### Simulator.Web

- `Home.razor` — üç modlu GUI, streaming kontrolü, durum mesajları

---

## Vital sign aralıkları

| Metrik | Geçerli aralık (simülatör) | Random üretim aralığı | ThingsBoard alarm |
|--------|---------------------------|----------------------|-------------------|
| Kalp atışı | 40–200 bpm | 55–115 bpm | < 50 veya > 120 |
| SpO2 | 85–100 % | 92–100 % | < 90 % |

Simülatör aralıkları **girdi doğrulama** içindir (assessment: “anlamlı limitler”). Alarm eşikleri ThingsBoard cihaz profilinde ayrı tanımlanır; böylece normal ve alarm senaryoları birlikte demo edilebilir.

---

## MQTT payload

Topic: `v1/devices/me/telemetry`

```json
{
  "heartRate": 72,
  "spo2": 98
}
```

Kimlik doğrulama: cihaz **access token** (MQTT username olarak).

---

## ThingsBoard platformu

```
docker/
  postgres          → kalıcı telemetri depolama
  thingsboard-ce    → UI (:8080), MQTT (:1883), kural motoru, alarmlar
```

Cloud'da Podman + podman-compose ile aynı `docker-compose.yml` kullanılır.

Yapılandırma rehberleri: `docs/thingsboard/`

---

## Güvenlik notları

- Cihaz token'ları `appsettings.local.json` içinde tutulur (`.gitignore`'da).
- Değerlendiricilere **tenant admin** verilir; sysadmin paylaşılmaz.
- Gmail SMTP için **App Password** kullanılır (normal şifre değil).
