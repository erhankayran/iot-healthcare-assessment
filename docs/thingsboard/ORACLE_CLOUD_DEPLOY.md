# Oracle Cloud Free — ThingsBoard Deploy (Adım Adım)

ThingsBoard'u **Oracle Cloud Always Free** katmanında deploy etme. Azure kotası engellendiğinde assessment için önerilen yöntem.

**Maliyet:** $0/ay (Always Free limitleri içinde)

**Canlı demo:** http://152.70.168.155:8080

---

## Always Free kaynaklar

| Kaynak | Ücretsiz limit |
|--------|----------------|
| Ampere A1 VM | Toplam **4 OCPU** + **24 GB RAM** |
| Public IP | 1 (VM çalışırken) |
| Block storage | 200 GB |

Önerilen shape: **2 OCPU, 4 GB RAM** (ThingsBoard için yeterli).

> Not: Bu projede 1 GB Micro denendi; TB init zorlandı. 2 OCPU / 4 GB önerilir.

---

## Bölüm 1 — Oracle hesabı

1. https://www.oracle.com/cloud/free/
2. **Start for free** → kayıt (kart doğrulama; Always Free limitlerinde ücret yok)
3. Home region seç (ör. **Germany Central**) — sonradan değiştirmek zor

---

## Bölüm 2 — VM oluştur

1. https://cloud.oracle.com giriş
2. ☰ → **Compute** → **Instances** → **Create instance**

| Alan | Değer |
|------|--------|
| Name | `tb-healthcare-demo` |
| Image | Ubuntu 22.04/24.04 veya Oracle Linux 9 |
| Shape | **VM.Standard.A1.Flex** — 2 OCPU, 4 GB |
| Public IPv4 | **Checked** |
| SSH key | Key pair indir ve sakla |

3. **Create** → **Running** olunca **Public IP** kopyala

---

## Bölüm 3 — Firewall portları (kritik)

### 3a. Security list (VCN)

Subnet → **Default security list** → **Add ingress rules**:

| Source CIDR | Protocol | Port |
|-------------|----------|------|
| `0.0.0.0/0` | TCP | 22 |
| `0.0.0.0/0` | TCP | 8080 |
| `0.0.0.0/0` | TCP | 1883 |

### 3b. OS firewall (SSH sonrası)

```bash
sudo firewall-cmd --permanent --add-port=8080/tcp
sudo firewall-cmd --permanent --add-port=1883/tcp
sudo firewall-cmd --reload
```

Ubuntu'da iptables alternatifi de gerekebilir.

---

## Bölüm 4 — SSH (Windows)

```powershell
ssh -i "C:\path\to\ssh-key.key" opc@152.70.168.155
```

> Oracle Linux varsayılan kullanıcı: **`opc`**. Ubuntu: **`ubuntu`**.

---

## Bölüm 5 — Docker / Podman kur

Oracle Linux örneği:

```bash
sudo dnf install -y podman podman-compose git
# veya Docker alternatifi — get.docker.com OL'de çalışmayabilir
```

Ubuntu:

```bash
curl -fsSL https://get.docker.com | sh
sudo usermod -aG docker $USER
sudo apt-get install -y docker-compose-plugin git
```

---

## Bölüm 6 — ThingsBoard deploy

```bash
git clone https://github.com/erhankayran/iot-healthcare-assessment.git
cd iot-healthcare-assessment/docker
cp .env.example .env
bash scripts/init-thingsboard.sh
```

5–10 dakika bekleyin. Tarayıcı:

```
http://YOUR_PUBLIC_IP:8080
```

Giriş: `sysadmin@thingsboard.org` / `sysadmin`

1 GB RAM VM'de `docker-compose.yml` içine eklenebilir:

```yaml
JAVA_OPTS: "-Xms256m -Xmx512m"
```

---

## Bölüm 7 — ThingsBoard yapılandırması

| Adım | Rehber |
|------|--------|
| Tenant + cihaz | [KURULUM.md](KURULUM.md) |
| Dashboard | [DASHBOARD.md](DASHBOARD.md) |
| Alarmlar | [ALARMLAR.md](ALARMLAR.md) |
| E-posta | [EMAIL_KURULUM.md](EMAIL_KURULUM.md) |

Cloud cihazdan **access token** kopyalayın.

---

## Bölüm 8 — Simülatörü cloud'a bağla

Windows'ta `src/Simulator.Web/appsettings.local.json`:

```json
{
  "ThingsBoard": {
    "Host": "152.70.168.155",
    "Port": 1883,
    "AccessToken": "CLOUD_DEVICE_TOKEN"
  }
}
```

```powershell
dotnet run --project src/Simulator.Web/Simulator.Web.csproj
```

Random streaming → cloud telemetri kontrol.

---

## Teslim metni örneği

```
Depo: https://github.com/erhankayran/iot-healthcare-assessment
ThingsBoard: http://152.70.168.155:8080
Tenant Admin: (değerlendirme e-postasında iletilir)
Platform: Oracle Cloud Always Free
Simülatör: Yerel Windows → cloud MQTT
```

---

## Sorun giderme

| Sorun | Çözüm |
|-------|-------|
| :8080 açılmıyor | Security list + OS firewall |
| SSH reddedildi | Port 22 ingress; doğru `.key` |
| Ampere kapasite yok | Başka availability domain veya sonra tekrar dene |
| TB init OOM | Shape'i 4 OCPU / 8 GB'e çıkar |
| Podman compose yok | `pip install podman-compose` |

---

## Maliyet güvenliği

- **Always Free** shape limitlerinde kal
- Demo dışında instance **Stop**
- Ücretli load balancer / block volume oluşturma
