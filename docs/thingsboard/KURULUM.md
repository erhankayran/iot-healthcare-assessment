# ThingsBoard Kurulum Rehberi

Bu rehber ThingsBoard'u yerelde Docker ile çalıştırmayı, .NET simülatörünü bağlamayı ve dashboard/alarm yapılandırmasına geçişi kapsar.

## Ön koşullar

- Docker Desktop (Windows/macOS) veya Docker Engine (Linux)
- Docker için en az **4 GB RAM**
- Host'ta **8080** (UI) ve **1883** (MQTT) portları boş

## Hızlı başlangıç (yerel)

### 1. Ortam yapılandırması (isteğe bağlı)

```bash
cd docker
cp .env.example .env
# Özel port veya SMTP için .env düzenle
```

### 2. ThingsBoard'u başlat

**Windows (PowerShell, repo kökünden):**

```powershell
.\docker\scripts\init-thingsboard.ps1
```

**Linux / macOS:**

```bash
bash docker/scripts/init-thingsboard.sh
```

**Manuel adımlar:**

```bash
cd docker
docker compose up -d postgres
docker compose run --rm -e INSTALL_TB=true -e LOAD_DEMO=false thingsboard-ce
docker compose up -d
```

Hazır olana kadar bekle:

```bash
docker compose logs -f thingsboard-ce
```

Şunu ara: `Started ThingsBoardServerApplication`

### 3. Giriş

[http://localhost:8080](http://localhost:8080)

| Rol | E-posta | Şifre |
|-----|---------|-------|
| System Administrator | sysadmin@thingsboard.org | sysadmin |

> Cloud deploy öncesi varsayılan şifreleri değiştirin.

## Tenant ve cihaz oluşturma

### Adım 1 — Tenant (System Admin)

1. **System Administrator** olarak giriş
2. **Tenants** → **Add tenant**
3. Ad: `Koç Healthcare Demo`
4. Kaydet

### Adım 2 — Tenant Admin girişi

1. Tenant satırı → **Manage tenant admin**
2. Tenant admin bilgilerini kullan veya oluştur
3. Çıkış yap → **Tenant Administrator** olarak giriş

### Adım 3 — Cihaz oluştur

1. **Entities** → **Devices** → **Add device**
2. Ad: `Vital Signs Simulator`
3. Profil: `default`
4. Kaydet
5. Cihazı aç → **Copy access token**

### Adım 4 — Simülatör yapılandırması

`src/Simulator.Web/appsettings.local.json`:

```json
{
  "ThingsBoard": {
    "Host": "localhost",
    "Port": 1883,
    "AccessToken": "CIHAZ_TOKEN_BURAYA"
  }
}
```

Simülatörü çalıştır:

```bash
dotnet run --project src/Simulator.Web/Simulator.Web.csproj
```

**Random → Start streaming** ile canlı telemetri gönder.

## MQTT bağlantısını doğrula

ThingsBoard'da:

1. Cihazı aç
2. **Latest telemetry**
3. `heartRate` ve `spo2` anahtarlarının zaman damgasıyla göründüğünü doğrula

## Durdur / yeniden başlat

```bash
cd docker
docker compose down        # konteynerleri durdur
docker compose up -d       # tekrar başlat (veri volume'da kalır)
```

## Sorun giderme

| Sorun | Çözüm |
|-------|-------|
| Port 8080 kullanımda | `docker/.env` içinde `TB_HTTP_PORT=8081` |
| MQTT bağlantı reddedildi | ThingsBoard konteyneri çalışıyor mu; port 1883 |
| Boş telemetri | `appsettings.local.json` token doğru mu |
| Init başarısız | Fresh DB için `docker compose down -v` (veriyi siler) |

## Sonraki adımlar

- [Dashboard & canlı grafikler](DASHBOARD.md)
- [Eşik alarmları](ALARMLAR.md)
- [Markalama / logo](MARKALAMA.md)
- [Cloud deploy](ORACLE_CLOUD_DEPLOY.md)
