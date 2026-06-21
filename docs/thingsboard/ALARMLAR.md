# Eşik Alarmları

ThingsBoard'da vital sign değerleri güvenli eşikleri aştığında alarm üretme.

## Önerilen eşikler

| Metrik | Koşul | Önem |
|--------|-------|------|
| Kalp atışı | `< 50` veya `> 120` bpm | Kritik |
| SpO2 | `< 90` % | Kritik |

Demo için **Manual** mod veya CSV'de aralık dışı değerler kullanın.

---

## Önerilen yöntem: Cihaz profili alarm kuralları

1. **Profiller** → **Cihaz profilleri** → **default** → **Alarm kuralları**
2. **Alarm kuralı ekle**

### Alarm 1 — Kalp atışı yüksek

| Alan | Değer |
|------|--------|
| Alarm türü | `Heart Rate High` |
| Önem | `Critical` |
| Oluşturma | `heartRate` **>** `120` |
| Temizleme | `heartRate` **≤** `120` |

### Alarm 2 — Kalp atışı düşük

| Alan | Değer |
|------|--------|
| Alarm türü | `Heart Rate Low` |
| Önem | `Critical` |
| Oluşturma | `heartRate` **<** `50` |
| Temizleme | `heartRate` **≥** `50` |

### Alarm 3 — SpO2 düşük

| Alan | Değer |
|------|--------|
| Alarm türü | `SpO2 Low` |
| Önem | `Critical` |
| Oluşturma | `spo2` **<** `90` |
| Temizleme | `spo2` **≥** `90` |

---

## E-posta için SMTP

E-posta ayrı yapılandırma gerektirir: [EMAIL_KURULUM.md](EMAIL_KURULUM.md)

Cihaz profili alarmları **Critical** alarm oluşturur; e-posta **Bildirim merkezi kuralı** ile tetiklenir.

---

## Alternatif: Kural zinciri (script)

TB sürümüne göre kural zinciri editöründe script + Create alarm + Send email de kullanılabilir. CE 4.3'te **cihaz profili alarm kuralları + Bildirim merkezi** yöntemi önerilir.

Örnek eşik script'i:

```javascript
var hr = msg.heartRate;
var spo2 = msg.spo2;
var alarm = false;

if (hr != null && (hr < 50 || hr > 120)) alarm = true;
if (spo2 != null && spo2 < 90) alarm = true;

return alarm ? { msg: msg, metadata: metadata, msgType: msgType } : false;
```

---

## Alarm testi

1. Simülatör → **Manual**
2. Kalp atışı: **130**, SpO2: **98**
3. **Send telemetry**
4. Doğrula:
   - Cihaz **Alarmlar** sekmesinde aktif alarm
   - E-posta geldi (spam klasörüne de bak)

> **Önemli:** Aktif alarm varken aynı koşul tekrarlanırsa yeni alarm oluşmaz → bildirim gitmez. Test öncesi alarmları **Temizle**.

---

## Dashboard alarm widget'ı

Dashboard'a **Alarm table** widget ekleyerek demo sırasında aktif uyarıları gösterin.

---

## Sorun giderme

| Sorun | Kontrol |
|-------|---------|
| E-posta yok | SMTP + bildirim kuralı — [EMAIL_KURULUM.md](EMAIL_KURULUM.md) |
| Alarm yok | Cihaz profili kuralları; telemetri anahtarları eşleşiyor mu |
| Alarm temizlenmiyor | Temizleme koşulu ve alarm türü eşleşmesi |
