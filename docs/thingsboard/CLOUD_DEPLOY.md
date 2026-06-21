# Azure VM — ThingsBoard Cloud Deploy (Adım Adım)

ThingsBoard'u Azure'a deploy etme (Oracle Cloud'a alternatif).

## Ön koşullar

- [Azure hesabı](https://azure.microsoft.com/free/)
- SSH istemcisi (Windows PowerShell `ssh`)

---

## Bölüm 1 — VM oluştur (Azure Portal)

1. [portal.azure.com](https://portal.azure.com)
2. **Create a resource** → **Virtual machine**

| Alan | Değer |
|------|--------|
| Resource group | `rg-healthcare-iot` |
| VM name | `tb-healthcare-demo` |
| Image | Ubuntu Server 22.04 LTS |
| Size | **Standard_B2s** (2 vCPU, 4 GB) |
| Auth | SSH public key |

3. **Networking** → inbound: **SSH (22), HTTP (8080), Custom (1883)**
4. **Create** → **Public IP** not al

---

## Bölüm 2 — Portlar (NSG)

VM → **Networking** → **Add inbound port rule**:

| Port | Protokol | Amaç |
|------|----------|------|
| 8080 | TCP | ThingsBoard UI |
| 1883 | TCP | MQTT |

---

## Bölüm 3 — Docker kur (VM'de)

```bash
ssh azureuser@YOUR_VM_IP
curl -fsSL https://get.docker.com | sh
sudo usermod -aG docker $USER
exit
ssh azureuser@YOUR_VM_IP
sudo apt-get update
sudo apt-get install -y docker-compose-plugin git
```

---

## Bölüm 4 — ThingsBoard deploy

```bash
git clone https://github.com/erhankayran/iot-healthcare-assessment.git
cd iot-healthcare-assessment/docker
cp .env.example .env
bash scripts/init-thingsboard.sh
```

```
http://YOUR_VM_IP:8080
```

Giriş: `sysadmin@thingsboard.org` / `sysadmin`

---

## Bölüm 5 — Cloud yapılandırması (yerel ile aynı)

1. Tenant + tenant admin
2. Cihaz **Vital Signs Simulator** → token kopyala
3. Dashboard oluştur
4. Cihaz profili alarm kuralları
5. Sysadmin → SMTP
6. Bildirim merkezi e-posta kuralı

Rehberler: [KURULUM.md](KURULUM.md), [DASHBOARD.md](DASHBOARD.md), [ALARMLAR.md](ALARMLAR.md), [EMAIL_KURULUM.md](EMAIL_KURULUM.md)

---

## Bölüm 6 — Simülatörü cloud'a yönlendir

Windows `appsettings.local.json`:

```json
{
  "ThingsBoard": {
    "Host": "YOUR_VM_IP",
    "Port": 1883,
    "AccessToken": "CLOUD_DEVICE_TOKEN"
  }
}
```

Telemetri akışı: **Yerel simülatör → Cloud ThingsBoard**

---

## Bölüm 7 — Güvenlik (URL paylaşmadan önce)

- [ ] Varsayılan sysadmin şifresini değiştir
- [ ] PostgreSQL şifresi `.env`'de güçlü olsun
- [ ] Değerlendiriciye yalnızca tenant admin ver
- [ ] İsteğe bağlı: NSG'yi değerlendirici IP'lerine kısıtla

---

## Maliyet tasarrufu

- Kullanılmadığında VM **Stop (deallocate)**
- Proje bitince resource group sil: `rg-healthcare-iot`

---

## Sorun giderme

| Sorun | Çözüm |
|-------|-------|
| :8080 açılmıyor | NSG + `docker compose ps` |
| MQTT başarısız | Port 1883; token doğru mu |
| VM yavaş | Minimum B2s; gerekirse B2ms |
| Init başarısız | `docker compose logs thingsboard-ce` |

---

## Teslim metni örneği

```
Depo: https://github.com/erhankayran/iot-healthcare-assessment
ThingsBoard: http://YOUR_VM_IP:8080
Tenant Admin: (değerlendirme e-postasında iletilir)
Simülatör: yerel çalışır, cloud MQTT'ye bağlanır
```

> **Not:** Bu proje Oracle Cloud deploy ile teslim edilmiştir. Azure rehberi alternatif olarak sağlanmıştır.
