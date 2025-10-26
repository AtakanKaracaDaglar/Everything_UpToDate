# ?? KULLANIM KILAVUZU - Everything UpToDate (WinGet Edition)

## ?? ÖNEMLÝ: Bu Gerçek Bir Güncelleme Aracýdýr!

Bu program **DEMO DEÐÝL**dir. Yaptýðýnýz güncellemeler **gerçekten** uygulamalarýnýzý güncelleyecektir!

## ?? Gereksinimler

### Mutlaka Gerekli
? **Windows 10/11** (1809 veya üzeri)  
? **WinGet (Windows Package Manager)** kurulu olmalý

### WinGet Nasýl Yüklenir?

#### Windows 11 Kullanýcýlarý
- Zaten yüklü! Hiçbir þey yapmanýza gerek yok.

#### Windows 10 Kullanýcýlarý
1. **Microsoft Store**'u açýn
2. **"App Installer"** arayýn
3. **Yükle** butonuna basýn
4. Bilgisayarý **yeniden baþlatýn**
5. Terminal açýp `winget --version` yazýn - versiyon görüyorsanýz hazýr!

## ?? Hýzlý Baþlangýç

### Ýlk Çalýþtýrma
```
1. Programý baþlatýn
2. WinGet kontrolü otomatik yapýlýr
3. Eðer WinGet yoksa uyarý görürsünüz
4. WinGet varsa otomatik tarama baþlar (10-30 saniye sürebilir, sabýrlý olun!)
5. Uygulamalar listelenir
```

## ?? Ekran Açýklamasý

```
???????????????????????????????????????????????????????????????????
?  ?? Uygulamalarý Tara    [6 güncelleme mevcut]                  ?
???????????????????????????????????????????????????????????????????
?  ? Google Chrome        ? 120.0  ? 121.0 ? Güncelleme Var  ? 89MB ? ??
?  ? VS Code              ? 1.85   ? 1.86  ? Güncelleme Var  ?125MB ? ??
?  ? Node.js              ? 18.17  ? 20.11 ? Güncelleme Var  ? 32MB ? ??
?  ? Adobe Reader         ? 23.006 ? 23.006? Güncel ?        ?  -   ? ??
???????????????????????????????????????????????????????????????????
?  [? Seçilenleri Güncelle]  [? Tümünü Güncelle]                ?
???????????????????????????????????????????????????????????????????
?  ?????????????????????????????????? 60%                        ?
?  Chrome: Ýndiriliyor... 60%                                     ?
???????????????????????????????????????????????????????????????????
?  GÜNCELLEME DETAYI:                                             ?
?  ???????????????????????????????????????????????????????????    ?
?  ?? Uygulama Adý      : Google Chrome                           ?
?  ?? Mevcut Versiyon   : 120.0.6099.109                         ?
?  ?? Yeni Versiyon     : 121.0.6167.85                          ?
?  ?? Kurulum Yolu      : C:\Program Files\Google\Chrome\...     ?
?  ?? Güncelleme Boyutu : 89.00 MB                               ?
?  ?? Son Kontrol       : 27.12.2024 14:30:45                    ?
?  ???????????????????????????????????????????????????????????    ?
?  Web tarayýcýsý - Güvenlik güncellemesi ve performans          ?
?  iyileþtirmeleri                                                ?
???????????????????????????????????????????????????????????????????
```

## ?? Buton Açýklamalarý

### ?? Uygulamalarý Tara
- **Ne yapar**: WinGet ile sistemi tarar (GERÇEK!)
- **Süre**: 10-30 saniye (ilk taramada daha uzun)
- **Ne bulur**: WinGet ile yüklenen tüm uygulamalar
- **Ne zaman kullanýlýr**: Ýlk açýlýþta veya listeyi yenilemek istediðinizde
- **Renk**: Mavi

### ? Seçilenleri Güncelle
- **Ne yapar**: Checkbox'larý iþaretli uygulamalarý GERÇEKTEN günceller
- **Uyarý**: Bu iþlem geri alýnamaz!
- **Ýzin**: Bazý uygulamalar yönetici izni isteyebilir
- **Ne zaman kullanýlýr**: Belirli uygulamalarý güncellemek istediðinizde
- **Renk**: Yeþil
- **Not**: En az 1 uygulama seçilmelidir

### ? Tümünü Güncelle
- **Ne yapar**: Güncelleme gerektiren TÜM uygulamalarý GERÇEKTEN günceller
- **Uyarý**: Bu iþlem çok uzun sürebilir ve geri alýnamaz!
- **Ýzin**: Kesinlikle yönetici izni gerekir
- **Ne zaman kullanýlýr**: Tüm güncellemeleri bir seferde yapmak istediðinizde
- **Renk**: Kýrmýzý
- **Tavsiye**: Önce önemli dosyalarýnýzý kaydedin!

## ?? Durum Göstergeleri

### Renk Kodlarý
- ?? **Sarý Arka Plan**: Güncelleme mevcut (WinGet'ten gelen gerçek veri)
- ?? **Yeþil Arka Plan**: Güncel (WinGet onayladý)

### Durum Mesajlarý
- "Güncelleme Mevcut ?": Yeni versiyon var (WinGet tespit etti)
- "Güncel ?": En güncel versiyon kurulu
- "Kontrol ediliyor...": WinGet kontrol ediyor
- "Ýndiriliyor... X%": WinGet indiriyor (GERÇEK!)
- "Kuruluyor... X%": WinGet yüklüyor (GERÇEK!)
- "Tamamlandý ?": Güncelleme baþarýlý

## ?? Ýpuçlarý

### 1. Tek Uygulama Güncellemek Ýçin
```
1. "Uygulamalarý Tara" butonuna týklayýn (10-30 saniye bekleyin)
2. Güncellemek istediðiniz uygulamanýn checkbox'ýný iþaretleyin
3. "Seçilenleri Güncelle" butonuna týklayýn
4. Onaylayýn (GERÝ ALINAMAZ!)
5. Ýþlem bitene kadar bekleyin
```

### 2. Birden Fazla Uygulama Güncellemek Ýçin
```
1. "Uygulamalarý Tara" butonuna týklayýn
2. Güncellemek istediðiniz TÜM uygulamalarýn checkbox'larýný iþaretleyin
3. "Seçilenleri Güncelle" butonuna týklayýn
4. Onaylayýn
5. Sýrayla hepsi güncellenecek (uzun sürebilir)
```

### 3. Tümünü Güncellemek Ýçin
```
?? UYARI: Bu iþlem ÇOK uzun sürebilir!

1. "Uygulamalarý Tara" butonuna týklayýn
2. "Tümünü Güncelle" butonuna týklayýn
3. Onaylayýn (checkbox iþaretlemeye gerek yok!)
4. Kahve molasý verin ? (30-60 dakika sürebilir)
5. Ýþlem bitene kadar bilgisayarý kapatmayýn!
```

### 4. Uygulama Detaylarýný Görmek Ýçin
```
1. Listeden bir uygulamaya tek týklayýn (checkbox'a deðil, satýra)
2. Alt panelde detaylar görüntülenir
3. Versiyon numaralarý WinGet'ten gelir
```

## ?? ÇOK ÖNEMLÝ UYARILAR

### Güncelleme Yapmadan Önce

#### ? YAPMANIZ GEREKENLER:
1. **Önemli dosyalarýnýzý kaydedin**
2. **Açýk tüm uygulamalarý kapatýn**
3. **Ýnternet baðlantýnýzý kontrol edin**
4. **Yeterli disk alaný olduðundan emin olun** (en az 5 GB)
5. **Bilgisayarýnýzýn prize takýlý olduðundan emin olun** (laptop kullanýcýlarý)

#### ? YAPMAMANIZ GEREKENLER:
1. **Güncelleme sýrasýnda bilgisayarý kapatmayýn**
2. **Programý kapatmayýn**
3. **Ýnterneti kesmeyin**
4. **Baþka güncellemeleri ayný anda baþlatmayýn**

### Yönetici Ýzni

Bazý uygulamalar **yönetici izni** gerektirebilir:
- Google Chrome
- Mozilla Firefox
- Node.js
- Python
- Git
- ve daha fazlasý...

**Çözüm**: Programý **Yönetici olarak çalýþtýrýn**
1. Program ikonuna sað týklayýn
2. "Yönetici olarak çalýþtýr" seçin
3. UAC penceresinde "Evet" deyin

## ?? Sýk Kullaným Senaryolarý

### Senaryo 1: Haftalýk Rutin Güncelleme
```
? Önerilen Yöntem:

1. Programý yönetici olarak baþlatýn
2. "Uygulamalarý Tara" butonuna týklayýn (10-30 saniye)
3. Kritik uygulamalarý seçin (Chrome, Firefox, vs.)
4. "Seçilenleri Güncelle" butonuna týklayýn
5. Onaylayýn ve bekleyin

? Süre: 5-15 dakika
```

### Senaryo 2: Kritik Güvenlik Güncellemesi
```
?? Acil Durum:

1. Programý baþlatýn
2. Ýlk tarama otomatik baþlar
3. Sarý arka planlý (güncelleme var) kritik uygulamalarý bulun
4. Sadece o uygulamalarýn checkbox'larýný iþaretleyin
5. "Seçilenleri Güncelle" butonuna týklayýn
6. Hemen iþlem baþlar

? Süre: 2-10 dakika
```

### Senaryo 3: Ayda Bir Büyük Temizlik
```
? Toplu Güncelleme:

1. Programý yönetici olarak baþlatýn
2. "Uygulamalarý Tara" butonuna týklayýn
3. "Tümünü Güncelle" butonuna týklayýn
4. ONAYLAYIN (geri dönüþ yok!)
5. Kahve molasý verin ???

? Süre: 30-60 dakika (uygulama sayýsýna göre)
```

## ?? Sorun Giderme

### Problem: "WinGet Bulunamadý" Uyarýsý
**Sebep**: WinGet sisteminizde kurulu deðil

**Çözüm**:
```
1. Microsoft Store'u açýn
2. "App Installer" arayýn
3. Yükle butonuna basýn
4. Bilgisayarý yeniden baþlatýn
5. Terminal açýp "winget --version" yazýn
6. Versiyon görüyorsanýz hazýr!
7. Programý yeniden baþlatýn
```

### Problem: "Yönetici Ýzni Gerekiyor" Hatasý
**Sebep**: Uygulama yönetici izni gerektiriyor

**Çözüm**:
```
1. Programý kapatýn
2. Sað týk > "Yönetici olarak çalýþtýr"
3. UAC penceresinde "Evet" deyin
4. Güncellemeyi tekrar deneyin
```

### Problem: "Uygulama Listesi Boþ"
**Sebep**: WinGet henüz taramayý tamamlamadý

**Çözüm**:
```
1. 30 saniye bekleyin (tarama uzun sürebilir)
2. "Uygulamalarý Tara" butonuna tekrar týklayýn
3. Hala boþsa: Terminal açýp "winget list" yazýn
4. Terminal'de uygulama görüyorsanýz programda bug var
5. Görmüyorsanýz WinGet sorunu var
```

### Problem: "Güncelleme Baþarýsýz" Mesajý
**Sebep**: Çeþitli nedenler olabilir

**Çözüm**:
```
1. Ýnternet baðlantýnýzý kontrol edin
2. Disk alanýnýzý kontrol edin
3. Uygulamayý kapatýn (çalýþýyorsa güncellenemez)
4. Yönetici izniyle tekrar deneyin
5. Bilgisayarý yeniden baþlatýn
```

### Problem: Tarama 30 Saniyeden Fazla Sürüyor
**Sebep**: Normal olabilir, özellikle ilk taramada

**Çözüm**:
```
1. Sabýrlý olun - WinGet yavaþ olabilir
2. 2-3 dakika bekleyin
3. Hala devam ediyorsa programý kapatýn
4. Terminal açýp "winget list" yazýn
5. Terminal'de de yavaþsa WinGet sorunu var
```

## ?? Performans ve Beklentiler

### Tarama Süreleri
- **Ýlk Tarama**: 30-60 saniye (normal)
- **Sonraki Taramalar**: 10-30 saniye
- **Çok Fazla Uygulama**: 1-2 dakika

### Güncelleme Süreleri
| Uygulama Tipi | Boyut | Süre |
|--------------|-------|------|
| Küçük (7-Zip) | 2 MB | 30 saniye |
| Orta (Chrome) | 89 MB | 2-5 dakika |
| Büyük (VS Code) | 125 MB | 5-10 dakika |
| Çok Büyük (Adobe) | 200+ MB | 10-20 dakika |

### Ýnternet Hýzý Etkisi
- **Yavaþ (1 Mbps)**: Her 10 MB için ~2 dakika
- **Orta (10 Mbps)**: Her 10 MB için ~10 saniye
- **Hýzlý (100 Mbps)**: Her 10 MB için ~1 saniye

## ?? Ýleri Seviye Kullaným

### WinGet Komut Satýrý

Programý kullanmak istemiyorsanýz, doðrudan WinGet kullanabilirsiniz:

```bash
# Tüm güncellemeleri göster
winget upgrade

# Tek uygulama güncelle
winget upgrade --id Google.Chrome

# Tüm güncellemeleri yap (DÝKKATLÝ!)
winget upgrade --all

# Sessiz güncelleme
winget upgrade --id Google.Chrome --silent

# Belirli bir uygulamayý ara
winget search "Chrome"

# Uygulama bilgilerini göster
winget show --id Google.Chrome
```

### Hangi Uygulamalar WinGet'te?

```bash
# Sistemdeki WinGet uygulamalarýný listele
winget list

# Belirli bir uygulamayý ara
winget search "Visual Studio Code"

# Tüm WinGet uygulamalarýný göster
winget search --source winget
```

### Program Özelleþtirme

Form1.cs > Form1_Load metodunda WinGet kontrolünü devre dýþý býrakabilirsiniz:

```csharp
// Bu satýrlarý yorum satýrý yapýn:
// bool wingetAvailable = await _updateService.CheckWinGetAvailabilityAsync();
// if (!wingetAvailable) { ... }
```

## ?? Güvenlik

### Bu Program Güvenli mi?
? **EVET**
- WinGet Microsoft'un resmi aracýdýr
- Program sadece WinGet komutlarýný çalýþtýrýr
- Kendi sunucularýmýz yok
- Kendi güncelleme sistemimiz yok
- Her þey WinGet üzerinden

### Hangi Ýzinler Gerekli?
- **Yönetici**: Bazý uygulamalar için
- **Ýnternet**: Güncelleme indirme için
- **Disk**: Geçici dosyalar için

### Gizlilik
- Hiçbir veri toplamýyoruz
- Sunucuya baðlanmýyoruz
- WinGet Microsoft'a telemetri gönderebilir (Microsoft politikasý)

## ?? Destek

### Hata Bildirimi
1. Hata mesajýný not alýn
2. Ekran görüntüsü alýn
3. `winget --version` çýktýsýný alýn
4. `winget list` çýktýsýný alýn

### Faydalý Linkler
- [WinGet Dokümantasyon](https://docs.microsoft.com/windows/package-manager/)
- [WinGet GitHub](https://github.com/microsoft/winget-cli)
- [App Installer Ýndir](https://www.microsoft.com/store/productId/9NBLGGH4NNS1)

---

**Baþarýlý Güncellemeler! ??**

**Unutmayýn**: Bu program GERÇEK güncellemeler yapar. Dikkatli olun!
