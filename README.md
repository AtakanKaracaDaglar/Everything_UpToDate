# 🚀 Everything UpToDate

<div align="center">

![Version](https://img.shields.io/badge/version-1.0.0-blue.svg)
![.NET Framework](https://img.shields.io/badge/.NET%20Framework-4.7.2-purple.svg)
![Platform](https://img.shields.io/badge/platform-Windows-lightgrey.svg)
![License](https://img.shields.io/badge/license-MIT-green.svg)
![Build](https://img.shields.io/badge/build-passing-brightgreen.svg)

**Tüm uygulamalarınızı tek tıkla güncel tutun! 🎯**

*Windows için güçlü, modern ve kullanıcı dostu bir uygulama güncelleyici*

[📥 İndir](#-kurulum) • [📖 Dökümanlar](#-özellikler) • [🐛 Hata Bildir](#-destek) • [⭐ Yıldız Ver](#)

</div>

---

## 📸 Ekran Görüntüleri

<div align="center">

### 🌟 Ana Ekran - Light Theme
*Modern ve temiz arayüz ile tüm güncellemeleri tek bakışta görün*

### 🌙 Dark Mode
*Göz yorucu olmayan karanlık tema desteği*

### 📊 Güncelleme Geçmişi ve İstatistikler
*Detaylı güncelleme logları ve analiz araçları*

</div>

---

## ✨ Özellikler

### 🎯 Temel Özellikler

| Özellik | Açıklama |
|---------|----------|
| 🔄 **Otomatik Tarama** | WinGet entegrasyonu ile tüm kurulu uygulamaları anında tarar |
| ⚡ **Hızlı Güncelleme** | Tek tıkla veya toplu güncelleme desteği |
| 📏 **Akıllı Boyut Hesaplama** | 3 katmanlı sistem ile doğru boyut tahmini |
| 🔍 **Gelişmiş Arama** | Real-time filtreleme ve sütun sıralama |
| 🎨 **Dark/Light Theme** | Göz dostu tema değiştirme |
| 📊 **Güncelleme Geçmişi** | CSV tabanlı database ile tüm kayıtlar |
| 🔔 **Sistem Tepsisi** | Arka planda çalışma ve bildirimler |
| ⚙️ **Özelleştirilebilir Ayarlar** | Detaylı kontrol ve tercihler |

### 🚀 İleri Seviye Özellikler

#### 📊 Güncelleme Geçmişi & İstatistikler
- ✅ **Otomatik Logging**: Her güncelleme detaylı kaydedilir
- ✅ **İstatistikler**: Başarı oranları, toplam boyut, süre analizi
- ✅ **Trend Analizi**: Aylık güncelleme grafikleri
- ✅ **Top 5**: En çok güncellenen uygulamalar
- ✅ **Export**: CSV formatında dışa aktarma

#### 🔔 Sistem Tepsisi & Bildirimler
- ✅ **System Tray**: Arka planda çalışma modu
- ✅ **Toast Notifications**: Windows bildirimleri
- ✅ **Minimize to Tray**: X'e basınca tray'e geçiş
- ✅ **Otomatik Tarama**: 6 saatte bir arka plan kontrolü
- ✅ **Akıllı Çıkış**: 3 seçenekli çıkış sistemi

#### 💾 Akıllı Boyut Hesaplama
1. **WinGet Manifest** → Resmi boyut bilgisi (%100 doğru)
2. **Kurulum Klasörü** → Fiziksel disk boyutu (%70-80 doğru)
3. **Kategori Tahmini** → 20+ popüler uygulama profili

---

## 📋 Sistem Gereksinimleri

| Gereksinim | Minimum | Önerilen |
|------------|---------|----------|
| **İşletim Sistemi** | Windows 10 (1809+) | Windows 11 |
| **.NET Framework** | 4.7.2 | 4.8 |
| **RAM** | 256 MB | 512 MB |
| **Disk Alanı** | 50 MB | 100 MB |
| **Ekran Çözünürlüğü** | 1024x768 | 1920x1080 |
| **WinGet** | v1.0+ | v1.6+ |

### 🔧 Gerekli Bileşenler
- **Windows Package Manager (WinGet)** - Otomatik kontrol edilir
- **App Installer** - Microsoft Store'dan yüklenebilir

---

## 📥 Kurulum

### Yöntem 1: Hazır Kurulum (Önerilen)

1. [**Releases**](../../releases) sayfasına gidin
2. En son versiyonu indirin (`EverythingUpToDate-v1.0.0.zip`)
3. ZIP dosyasını çıkarın
4. `EverythingUpToDate.exe` dosyasını çalıştırın
5. İlk çalıştırmada WinGet kontrolü yapılacak

### Yöntem 2: Kaynak Koddan Derleme

```bash
# Repository'yi klonlayın
git clone https://github.com/AtakanKaracaDaglar/everything-uptodate.git
cd everything-uptodate

# Visual Studio ile açın
start Everything_UpToDate.sln

# Release mode'da derleyin
# Build → Build Solution (Ctrl+Shift+B)
```

### WinGet Yükleme

Eğer WinGet yüklü değilse (uygulama size söyleyecektir):

#### Otomatik Yükleme:
1. Microsoft Store'u açın
2. "App Installer" arayın
3. Yükleyin

#### Manuel Yükleme:
```powershell
# PowerShell (Admin) açın
Add-AppxPackage -RegisterByFamilyName -MainPackage Microsoft.DesktopAppInstaller_8wekyb3d8bbwe
```

---

## 🎮 Kullanım

### 🚀 Hızlı Başlangıç (30 saniye!)

```
1️⃣ "🔄 Uygulamaları Tara" butonuna tıklayın
2️⃣ Listede güncellenecek uygulamaları görün
3️⃣ İstediğiniz uygulamaları seçin veya "⚡ Tümünü Güncelle"
4️⃣ İlerlemeyi izleyin, otomatik tamamlanacak!
5️⃣ "📊 Geçmişi Görüntüle" ile istatistikleri inceleyin
```

### 🎯 Detaylı Kullanım

#### Ana Butonlar

| Buton | İşlev | İpucu |
|-------|-------|-------|
| 🔄 **Uygulamaları Tara** | Tüm uygulamaları tarar | İlk açılışta otomatik çalışır |
| ✓ **Seçilenleri Güncelle** | İşaretli uygulamaları günceller | Checkbox ile seçim yapın |
| ⚡ **Tümünü Güncelle** | Hepsini günceller | Toplu güncelleme için ideal |
| 📊 **Geçmişi Görüntüle** | İstatistikleri gösterir | Database kayıtlarını görün |
| 🌙 **Dark Mode** | Temayı değiştirir | Tercihiniz kaydedilir |

#### Arama ve Filtreleme

```
🔍 Arama Kutusu:
   ├─ Uygulama adına göre ara
   ├─ Uygulama ID'sine göre ara
   ├─ Büyük/küçük harf duyarsız
   └─ Anlık sonuçlar

📊 Sütun Başlıklarına Tıklayın:
   ├─ Uygulama Adı → Alfabetik sıralama
   ├─ Versiyon → Versiyon sıralama
   └─ Boyut → Boyuta göre sıralama
```

#### System Tray (Sistem Tepsisi)

```
🖱️ Çift Tık:
   └─ Pencereyi aç/kapat

🖱️ Sağ Tık Menüsü:
   ├─ 📂 Aç → Ana pencereyi göster
   ├─ 🔄 Uygulamaları Tara → Arka planda tara
   └─ ❌ Çıkış → Uygulamayı kapat

❌ X Butonuna Basınca:
   ├─ EVET → Tamamen kapat
   ├─ HAYIR → Tray'e küçült
   └─ İPTAL → Hiçbir şey yapma
```

---

## 📊 Database ve Geçmiş Sistemi

### 📁 Dosya Konumu

Tüm veriler güvenli bir şekilde burada saklanır:

```
C:\Users\[KULLANICI_ADI]\AppData\Roaming\EverythingUpToDate\
├── 📄 update_history.csv    ← Güncelleme kayıtları
└── ⚙️ settings.ini          ← Uygulama ayarları
```

### 📈 Örnek İstatistikler

```
═══════════════════════════════════════
  GÜNCELLEME GEÇMİŞİ VE İSTATİSTİKLER
═══════════════════════════════════════

📊 GENEL İSTATİSTİKLER:
   Toplam Güncelleme: 150
   ✅ Başarılı: 142 (94.6%)
   ❌ Başarısız: 8 (5.3%)
   💾 Toplam İndirilen: 12.5 GB

📋 SON 10 GÜNCELLEME:

✅ Google Chrome
   120.0.6099 → 121.0.6167
   26.01.2025 10:30 - 89.50 MB

✅ Visual Studio Code
   1.85.0 → 1.86.0
   26.01.2025 11:15 - 125.00 MB

🏆 EN ÇOK GÜNCELLENEN UYGULAMALAR:
   1. Google Chrome (15 kez)
   2. Visual Studio Code (12 kez)
   3. Node.js (10 kez)
   4. Firefox (8 kez)
   5. Git (7 kez)
```

### 🔍 Database'i Görüntüleme

#### Yöntem 1: Uygulama İçinden (En Kolay)
```
"📊 Geçmişi Görüntüle" butonuna tıklayın
```

#### Yöntem 2: Windows Gezgini
```
Win + R → %APPDATA%\EverythingUpToDate → Enter
```

#### Yöntem 3: PowerShell
```powershell
Get-Content "$env:APPDATA\EverythingUpToDate\update_history.csv"
```

#### Yöntem 4: Excel'de Analiz
```
update_history.csv dosyasını Excel'de açın
Pivot Table oluşturun
Grafikler çizin
```

---

## 🏗️ Teknik Detaylar

### 📦 Proje Yapısı

```
Everything_UpToDate/
├── 📁 Models/                      # Veri modelleri
│   ├── ApplicationInfo.cs          # Uygulama modeli
│   ├── AppSettings.cs              # Ayarlar modeli
│   └── UpdateHistoryEntry.cs       # Geçmiş kayıt modeli
│
├── 📁 Services/                    # İş mantığı servisleri
│   ├── UpdateService.cs            # Ana güncelleme servisi
│   ├── DatabaseService.cs          # CSV database yönetimi
│   ├── ThemeService.cs             # Tema yönetimi
│   ├── SettingsService.cs          # Ayar yönetimi
│   ├── NotificationService.cs      # Bildirim servisi
│   ├── BackgroundScanService.cs    # Arka plan tarama
│   └── StartupService.cs           # Windows başlangıç
│
├── 📁 Forms/                       # UI formları
│   ├── Form1.cs                    # Ana form
│   └── Form1.Designer.cs           # Form tasarımı
│
└── 📁 Docs/                        # Dökümanlar
    ├── README.md                   # Bu dosya
    ├── DATABASE_KULLANIM.md        # Database kılavuzu
    ├── KULLANIM_KILAVUZU.md        # Detaylı kullanım
    └── BOYUT_HESAPLAMA.md          # Boyut sistemi açıklaması
```

### 🔧 Kullanılan Teknolojiler

| Teknoloji | Versiyon | Kullanım Alanı |
|-----------|----------|----------------|
| **C#** | 7.3 | Ana programlama dili |
| **.NET Framework** | 4.7.2 | Framework |
| **Windows Forms** | - | UI Framework |
| **WinGet CLI** | 1.0+ | Paket yönetimi |
| **CSV** | - | Database formatı |
| **INI** | - | Ayar dosyası formatı |
| **Async/Await** | - | Asenkron işlemler |
| **LINQ** | - | Veri sorgulama |

### 🎨 Tasarım Desenleri

- **Service Layer Pattern**: Servislerin modüler ayrıştırılması
- **Observer Pattern**: Event-driven notification sistemi
- **Singleton Pattern**: Servis instance yönetimi
- **Strategy Pattern**: Boyut hesaplama stratejileri
- **Dependency Injection**: Servis bağımlılıkları

### 📊 Kod İstatistikleri

```
📂 Toplam Dosya: 17
📄 Kod Satırı: ~3,500
🔧 Servis Sayısı: 7
📝 Model Sayısı: 3
⏱️ Geliştirme Süresi: 2 hafta
```

---

## 🔮 Gelecek Özellikler (Roadmap)

### 🎯 v1.1.0 - Q1 2025
- [ ] 🎨 **HistoryForm**: Detaylı geçmiş görüntüleme formu
- [ ] 📈 **Grafikler**: Aylık trend grafikleri (ChartJS)
- [ ] 🔍 **Gelişmiş Filtreleme**: Tarih aralığı, durum bazlı
- [ ] 🌐 **Çoklu Dil**: İngilizce ve Türkçe dil desteği
- [ ] 📤 **Export**: JSON ve Excel export seçenekleri

### 🚀 v1.2.0 - Q2 2025
- [ ] 🍫 **Chocolatey Desteği**: Alternatif paket yöneticisi
- [ ] 🎮 **Steam Entegrasyonu**: Oyun güncellemeleri
- [ ] 📦 **Microsoft Store**: Store app güncellemeleri
- [ ] ⚙️ **Ayarlar Formu**: Detaylı ayar paneli
- [ ] 🔄 **Auto-Update**: Kendi kendini güncelleme

### ⭐ v2.0.0 - Q3 2025
- [ ] 🌐 **Web Dashboard**: Local web arayüzü (localhost:8080)
- [ ] 📱 **Mobil Uygulama**: iOS/Android uzaktan kontrol
- [ ] 🤖 **AI Önerileri**: Makine öğrenmesi ile akıllı öneriler
- [ ] ☁️ **Cloud Sync**: Ayarların bulut senkronizasyonu
- [ ] 🔐 **Yedekleme/Geri Yükleme**: Sistem geri yükleme noktaları

---

## 🤝 Katkıda Bulunma

Katkılarınızı çok isteriz! Her türlü katkı değerlidir. 🎉

### 🐛 Hata Bildirimi

1. [Issues](../../issues) sayfasına gidin
2. "New Issue" butonuna tıklayın
3. Hatayı detaylı açıklayın:
   - Ne yaptınız?
   - Ne bekliyordunuz?
   - Ne oldu?
   - Ekran görüntüsü (varsa)
   - Windows versiyonu
   - WinGet versiyonu

### 💡 Özellik Önerisi

1. [Discussions](../../discussions) sayfasını açın
2. "New Discussion" oluşturun
3. Önerinizi detaylı anlatın
4. Kullanım senaryoları verin

### 🔧 Pull Request Süreci

```bash
# 1. Repository'yi fork edin (GitHub'da Fork butonuna tıklayın)

# 2. Fork'u klonlayın
git clone https://github.com/SIZIN_KULLANICI_ADI/everything-uptodate.git
cd everything-uptodate

# 3. Yeni branch oluşturun
git checkout -b feature/amazing-feature

# 4. Değişikliklerinizi yapın ve commit edin
git add .
git commit -m 'feat: Add amazing feature'

# 5. Fork'unuza push edin
git push origin feature/amazing-feature

# 6. Pull Request açın (GitHub'da otomatik öneri gelecek)
```

### 📝 Commit Mesaj Kuralları

```
feat: Yeni özellik ekleme
fix: Hata düzeltme
docs: Dokümantasyon değişikliği
style: Kod formatı, noktalama vb.
refactor: Kod iyileştirme
test: Test ekleme veya düzeltme
chore: Build, ayar dosyaları vb.

Örnek:
feat: Add Chocolatey support for package management
fix: Resolve theme switching bug in dark mode
docs: Update installation instructions
```

---

## 📄 Lisans

Bu proje **MIT Lisansı** ile lisanslanmıştır. Detaylar için [LICENSE](LICENSE) dosyasına bakın.

```
MIT License

Copyright (c) 2025 Atakan Karaca Dağlar

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software...
```

**Özet**: Bu yazılımı istediğiniz gibi kullanabilir, değiştirebilir ve dağıtabilirsiniz!

---

## 👨‍💻 Geliştirici

<div align="center">

### **Atakan Karaca Dağlar**

*Full Stack Developer & Open Source Enthusiast*

[![GitHub](https://img.shields.io/badge/GitHub-100000?style=for-the-badge&logo=github&logoColor=white)](https://github.com/AtakanKaracaDaglar)
[![LinkedIn](https://img.shields.io/badge/LinkedIn-0077B5?style=for-the-badge&logo=linkedin&logoColor=white)](https://linkedin.com/in/atakan-karaca-daglar)
[![Twitter](https://img.shields.io/badge/Twitter-1DA1F2?style=for-the-badge&logo=twitter&logoColor=white)](https://twitter.com/AtakanKaracaDaglar)
[![Email](https://img.shields.io/badge/Email-D14836?style=for-the-badge&logo=gmail&logoColor=white)](mailto:atakankaracadaglar@gmail.com)

</div>

---

## 🙏 Teşekkürler

Bu proje aşağıdaki harika projelerden ilham aldı:

- **[WinGet](https://github.com/microsoft/winget-cli)** - Microsoft'un resmi paket yöneticisi
- **[Chocolatey](https://chocolatey.org/)** - Windows için paket yöneticisi
- **.NET Framework** - Microsoft'un güçlü uygulama framework'ü

### 🌟 Özel Teşekkürler

- 🙌 Tüm beta test kullanıcılarına
- 💡 Öneride bulunanlara
- 🐛 Hata bildirenlere
- ⭐ Yıldız verenlere
- 🔧 Katkıda bulunanlara

---

## 📞 Destek

Yardıma mı ihtiyacınız var? İletişime geçmekten çekinmeyin!

### 📧 İletişim Kanalları

- **📧 Email**: support@everythinguptodate.com
- **💬 Discord**: [Sunucumuza Katılın](#)
- **📖 Wiki**: [Detaylı Dökümanlar](../../wiki)
- **🐛 Issues**: [Sorun Bildirin](../../issues)
- **💡 Discussions**: [Tartışmalara Katılın](../../discussions)

### ❓ SSS (Sık Sorulan Sorular)

<details>
<summary><b>WinGet bulunamadı hatası alıyorum?</b></summary>

1. Microsoft Store'u açın
2. "App Installer" arayın
3. Yükleyin ve bilgisayarı yeniden başlatın
4. `winget --version` komutu ile kontrol edin
</details>

<details>
<summary><b>Güncelleme başarısız oluyor?</b></summary>

1. Yönetici olarak çalıştırmayı deneyin (Sağ tık → "Yönetici olarak çalıştır")
2. İnternet bağlantınızı kontrol edin
3. WinGet'i güncelleyin: `winget upgrade --id Microsoft.AppInstaller`
4. Geçmişte hata mesajını kontrol edin
</details>

<details>
<summary><b>Database dosyalarını nasıl yedeklerim?</b></summary>

```powershell
# PowerShell'de çalıştırın
$source = "$env:APPDATA\EverythingUpToDate"
$backup = "C:\Yedek\EverythingUpToDate_$(Get-Date -Format 'yyyy-MM-dd')"
Copy-Item -Path $source -Destination $backup -Recurse
```
</details>

<details>
<summary><b>Arka plan taraması çalışmıyor?</b></summary>

1. Ayarlar → AutoScanEnabled = True kontrol edin
2. settings.ini dosyasını inceleyin
3. Uygulamanın arka planda çalıştığından emin olun (Tray icon)
4. Windows Görev Zamanlayıcı'da izinleri kontrol edin
</details>

---

## 📊 Proje İstatistikleri

<div align="center">

![GitHub Stars](https://img.shields.io/github/stars/AtakanKaracaDaglar/everything-uptodate?style=for-the-badge&logo=github)
![GitHub Forks](https://img.shields.io/github/forks/AtakanKaracaDaglar/everything-uptodate?style=for-the-badge&logo=github)
![GitHub Issues](https://img.shields.io/github/issues/AtakanKaracaDaglar/everything-uptodate?style=for-the-badge&logo=github)
![GitHub Pull Requests](https://img.shields.io/github/issues-pr/AtakanKaracaDaglar/everything-uptodate?style=for-the-badge&logo=github)

![Downloads](https://img.shields.io/github/downloads/AtakanKaracaDaglar/everything-uptodate/total?style=for-the-badge&logo=download)
![Code Size](https://img.shields.io/github/languages/code-size/AtakanKaracaDaglar/everything-uptodate?style=for-the-badge)
![Last Commit](https://img.shields.io/github/last-commit/AtakanKaracaDaglar/everything-uptodate?style=for-the-badge&logo=git)
![Contributors](https://img.shields.io/github/contributors/AtakanKaracaDaglar/everything-uptodate?style=for-the-badge&logo=github)

</div>

---

<div align="center">

## ⭐ Beğendiyseniz Yıldız Verin! ⭐

**Made with ❤️ and ☕ by Atakan Karaca Dağlar**

*"Güncel kalmak hiç bu kadar kolay olmamıştı!"*

---

[![View on GitHub](https://img.shields.io/badge/View%20on-GitHub-181717?style=for-the-badge&logo=github&logoColor=white)](https://github.com/AtakanKaracaDaglar/everything-uptodate)
[![Download Latest](https://img.shields.io/badge/Download-Latest%20Release-blue?style=for-the-badge&logo=download&logoColor=white)](https://github.com/AtakanKaracaDaglar/everything-uptodate/releases)
[![Read Docs](https://img.shields.io/badge/Read-Documentation-green?style=for-the-badge&logo=readthedocs&logoColor=white)](https://github.com/AtakanKaracaDaglar/everything-uptodate/wiki)
[![Join Discord](https://img.shields.io/badge/Join-Discord-5865F2?style=for-the-badge&logo=discord&logoColor=white)](#)

</div>

---

<div align="center">

### 🎉 **Everything UpToDate** 🎉

**Version 1.0.0** • **Released 2025**

*Her şey güncel, her zaman!*

</div>
