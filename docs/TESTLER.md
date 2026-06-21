# Test Rehberi

Otomatik testler simülatör domain mantığını ve application servislerini doğrular. Canlı ThingsBoard entegrasyonu demo sırasında manuel test edilir ([DEGERLENDIRICI_REHBERI.md](DEGERLENDIRICI_REHBERI.md)).

---

## Tüm testleri çalıştır

```bash
dotnet test IoTHealthcareAssessment.sln
```

**Son doğrulama:** 14 test, 0 hata, 0 uyarı.

```bash
dotnet build IoTHealthcareAssessment.sln
dotnet test IoTHealthcareAssessment.sln --verbosity normal
```

---

## Test projeleri

### Simulator.Domain.Tests (8 test)

| Test | Doğruladığı |
|------|-------------|
| `TryValidate_ReturnsTrue_ForValidValues` | HR 40–200, SpO2 85–100 kabul |
| `TryValidate_ReturnsFalse_ForInvalidHeartRate` | HR aralık dışı reddedilir |
| `TryValidate_ReturnsFalse_ForInvalidSpO2` | SpO2 aralık dışı reddedilir |
| `VitalSignRanges_DefinesExpectedRandomBounds` | Random aralıklar geçerli sınırlar içinde |

**Kaynak:** `VitalTelemetryValidator.cs`, `VitalSignRanges.cs`

### Simulator.Application.Tests (6 test)

| Test | Doğruladığı |
|------|-------------|
| `Generate_ReturnsValuesWithinConfiguredRandomRanges` | Random HR/SpO2 yapılandırılmış sınırlarda |
| `Parse_ReadsRowsWithHeader` | `HeartRate,SpO2` başlıklı CSV ayrıştırma |
| `Parse_ThrowsWhenCsvIsEmpty` | Boş CSV reddedilir |
| `Parse_ThrowsForInvalidRow` | Geçersiz CSV satırı reddedilir |
| `PublishManualAsync_PublishesValidTelemetry` | Manuel publish MQTT publisher'ı çağırır |
| `PublishManualAsync_ThrowsForInvalidValues` | Geçersiz değerler publish öncesi reddedilir |

**Kaynak:** `RandomTelemetryGenerator`, `CsvTelemetryParser`, `TelemetrySimulationService`

---

## Unit test kapsamı dışında kalanlar

| Alan | Neden | Manuel doğrulama |
|------|-------|------------------|
| MQTT → ThingsBoard | Harici broker | Random streaming → cloud telemetri |
| ThingsBoard alarmları | Platform kural motoru | Manuel HR 130 → alarm |
| E-posta teslimi | Harici SMTP | Alarm → gelen kutusu |
| Blazor UI | UI/entegrasyon | Üç mod tarayıcıda |

---

## Manuel uçtan uca checklist

Demo provası veya değerlendirme için:

- [ ] `dotnet test` — 14 geçti
- [ ] Cloud ThingsBoard UI yükleniyor
- [ ] Simülatör Manual → cihazda telemetri
- [ ] Simülatör Random → canlı dashboard grafikleri
- [ ] Simülatör CSV → satırlar publish edildi
- [ ] Manuel HR 130 → Kritik alarm
- [ ] Yeni alarm → e-posta geldi

Adım adım: [DEMO_SENARYOSU.md](DEMO_SENARYOSU.md)
