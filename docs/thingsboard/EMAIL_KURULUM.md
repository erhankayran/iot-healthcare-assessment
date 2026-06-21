# E-posta Alarm Kurulumu (ThingsBoard CE)

Assessment, e-posta, SMS veya herhangi bir iletişim kanalı ister. Yalnızca uygulama içi bildirim tek başına yeterli olmayabilir — aşağıdaki e-posta kurulumunu yapın.

## Genel bakış

| Adım | Kim | Ne |
|------|-----|-----|
| 1 | System Admin | SMTP posta sunucusu |
| 2 | Tenant Admin | Alarm bildirim kuralı |
| 3 | Siz | Simülatörle test |

Cihaz profili alarmları (Heart Rate High vb.) **Kritik** alarm oluşturur. E-posta **bildirim kuralı** ile tetiklenir.

---

## Ön koşul: Gmail App Password

Normal Gmail şifresi **çalışmaz**.

1. [Google Hesap → Güvenlik](https://myaccount.google.com/security)
2. **2 Adımlı Doğrulama** açık olmalı
3. **Uygulama şifreleri** → Mail + Windows → 16 haneli şifre

---

## Adım 1 — SMTP (System Administrator)

1. Çıkış → **sysadmin@thingsboard.org** / **sysadmin**
2. **Ayarlar** → **Posta sunucusu**
3. Gmail örneği:

| Alan | Değer |
|------|--------|
| Gönderen | `ThingsBoard <sizin@gmail.com>` |
| SMTP sağlayıcı | **Gmail** |
| SMTP sunucusu | `smtp.gmail.com` |
| Port | `587` |
| TLS | Açık |
| Kullanıcı adı | `sizin@gmail.com` |
| Parola | App Password (boşluksuz) |

4. **Test e-postası gönder** → alıcı: **gerçek Gmail adresiniz** (`sysadmin@thingsboard.org` yazmayın)
5. **Kaydet**

### Docker alternatifi

`docker/.env`:

```env
SMTP_HOST=smtp.gmail.com
SMTP_PORT=587
SMTP_USERNAME=sizin@gmail.com
SMTP_PASSWORD=app-password
SMTP_FROM=sizin@gmail.com
```

```powershell
cd docker
docker compose up -d --force-recreate thingsboard-ce
```

---

## Adım 2 — Bildirim şablonu (Tenant Admin)

1. Tenant admin giriş
2. **Bildirim merkezi** → **Şablonlar** → **+**

| Alan | Değer |
|------|--------|
| Ad | `Vital Signs Alarm Email` |
| Tür | **Alarm** |
| E-posta konusu | `[ALERT] ${alarmType} - ${deviceName}` |
| E-posta mesajı | `Device: ${deviceName}, Alarm: ${alarmType}, Severity: ${alarmSeverity}` |

**E-posta** sekmesi dolu olmalı — yalnızca Web doluysa mail gitmez.

---

## Adım 3 — Alıcı grubu

1. **Bildirim merkezi** → **Alıcılar** → **+**
2. Ad: `Tenant Admins`
3. Tür: **Kiracı yöneticileri**
4. Kendi kullanıcınız listede olsun

---

## Adım 4 — Bildirim kuralı

1. **Bildirim merkezi** → **Kurallar** → **+**

| Alan | Değer |
|------|--------|
| Ad | `Vital Signs Email Alert` |
| Tetikleyici | **Alarm** |
| Olay | **Alarm oluşturuldu** |
| Alarm türleri | Heart Rate High, Heart Rate Low, SpO2 Low |
| Şablon | `Vital Signs Alarm Email` |
| Alıcı grubu | `Tenant Admins` |
| Etkin | Açık |

> Varsayılan **New alarm** kuralı genelde yalnızca **Web** bildirimi gönderir. E-posta için ayrı kural + e-posta şablonu şart.

---

## Adım 5 — Test

1. ThingsBoard **Alarmlar** → aktif alarmları **Temizle**
2. Simülatör → **Manual** → HR **130**, SpO2 **98**
3. **Send telemetry**
4. Doğrula:
   - [ ] Alarm aktif
   - [ ] E-posta geldi

---

## Sorun giderme

| Sorun | Çözüm |
|-------|-------|
| SMTP test başarısız | App Password; 2FA açık |
| Alarm var, mail yok | Kural etkin mi; e-posta şablonu mu; eski alarm temizlendi mi |
| Sadece web bildirimi | Şablon teslimat yöntemi **E-posta** |
| `sysadmin@... bulunamadı` bounce | Test alıcısı gerçek e-posta olmalı — SMTP yine de çalışıyor olabilir |

---

## Assessment notu

> Uyarılar: ThingsBoard cihaz profili alarm kuralları + Bildirim merkezi e-posta kuralı. SMTP System Admin posta sunucusu ayarlarında yapılandırıldı.
