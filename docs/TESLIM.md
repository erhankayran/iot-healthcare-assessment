# Teslim Rehberi — Koç Healthcare Assessment

---

## Depo URL (zorunlu)

Assessment formuna ve bu belgeye ekleyin:

```
https://github.com/erhankayran/iot-healthcare-assessment
```

---

## Cloud URL (assessment hedefi)

```
ThingsBoard UI: http://152.70.168.155:8080
MQTT:           152.70.168.155:1883
Platform:       Oracle Cloud Always Free (Frankfurt)
Simülatör:      Aday PC → cloud MQTT
```

Deploy rehberi: [thingsboard/ORACLE_CLOUD_DEPLOY.md](thingsboard/ORACLE_CLOUD_DEPLOY.md)  
Azure alternatifi: [thingsboard/CLOUD_DEPLOY.md](thingsboard/CLOUD_DEPLOY.md)

---

## Demo kimlik bilgileri (değerlendiriciler için)

Tenant admin erişimi verin — **sysadmin şifresini paylaşmayın**.

| Rol | E-posta | Şifre |
|-----|---------|-------|
| Tenant Admin | `erhan.kayran37@gmail.com` | *(değerlendirme davet e-postasında iletilir)* |

**Giriş:** http://152.70.168.155:8080 → tenant admin bilgileriyle giriş yapın.  
**Not:** `sysadmin@thingsboard.org` yalnızca platform yönetimi içindir; değerlendirme için tenant admin yeterlidir.

---

## Değerlendirme günü checklist

- [x] ThingsBoard cloud VM'de çalışıyor
- [x] Konteynerler sağlıklı
- [x] Simülatör local'de cloud MQTT host ile yapılandırıldı
- [x] Dashboard açık — **Healthcare Vital Signs**
- [x] **Random → Start streaming** canlı demo
- [x] Alarm demo: Manuel HR `130` → e-posta + web bildirimi
- [x] Repo URL hazır
- [x] `dotnet test` — 14 test geçti

---

## Değerlendirici hızlı başlangıç

1. [DEGERLENDIRICI_REHBERI.md](DEGERLENDIRICI_REHBERI.md) oku
2. Cloud UI: http://152.70.168.155:8080
3. Depoyu klonla → `dotnet test`
4. Simülatör: `appsettings.local.json` + `dotnet run`
5. Canlı demo: [DEMO_SENARYOSU.md](DEMO_SENARYOSU.md)

---

## Token kurulumu (clone sonrası)

1. `src/Simulator.Web/appsettings.example.json` → `appsettings.local.json` kopyala
2. Cloud cihazdan `AccessToken` al
3. `Host`: `152.70.168.155`, `Port`: `1883`
4. Token git'e commit edilmez

---

## Assessment özeti

| Madde | Durum |
|-------|-------|
| 7 zorunlu madde + hedef | Tamam |
| ThingsBoard cloud | http://152.70.168.155:8080 |
| Repo URL | GitHub'da |

Tam matris: [DEGERLENDIRME_UYUM.md](DEGERLENDIRME_UYUM.md)

---

## Assessment formuna yapıştırılacak özet metin

```
GitHub: https://github.com/erhankayran/iot-healthcare-assessment
ThingsBoard: http://152.70.168.155:8080
MQTT: 152.70.168.155:1883
Tenant Admin: (değerlendirme e-postasında iletilir)

Proje: .NET 8 Blazor Server vital signs simülatörü (Manuel/Random/CSV) +
ThingsBoard CE cloud platformu (dashboard, alarmlar, e-posta bildirimleri).
Clean Architecture, 14 xUnit test, Türkçe dokümantasyon docs/ klasöründe.
Değerlendirici rehberi: docs/DEGERLENDIRICI_REHBERI.md
```
