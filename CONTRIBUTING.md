# 🤝 Katkıda Bulunma Rehberi

Everything UpToDate projesine katkıda bulunmak istediğiniz için teşekkür ederiz! 🎉

Bu rehber, katkıda bulunma sürecini basit ve verimli hale getirmek için hazırlanmıştır.

---

## 📋 İçindekiler

- [Davranış Kuralları](#-davranış-kuralları)
- [Nasıl Katkıda Bulunabilirim?](#-nasıl-katkıda-bulunabilirim)
- [Geliştirme Ortamı Kurulumu](#️-geliştirme-ortamı-kurulumu)
- [Pull Request Süreci](#-pull-request-süreci)
- [Kod Standartları](#-kod-standartları)
- [Commit Mesaj Kuralları](#-commit-mesaj-kuralları)

---

## 📜 Davranış Kuralları

Bu projeye katılan herkes saygılı ve kapsayıcı bir ortam yaratmaya yardımcı olmalıdır:

- ✅ Saygılı ve yapıcı olun
- ✅ Farklı görüşlere açık olun
- ✅ Yapıcı eleştiri yapın ve kabul edin
- ❌ Saldırgan veya aşağılayıcı dil kullanmayın
- ❌ Kişisel saldırılardan kaçının

---

## 🎯 Nasıl Katkıda Bulunabilirim?

### 🐛 Hata Bildirimi

1. [Issues](../../issues) sayfasına gidin
2. Benzer bir hata bildirilmiş mi kontrol edin
3. Yeni issue açın ve şunları ekleyin:
   - Açıklayıcı başlık
   - Detaylı açıklama
   - Yeniden oluşturma adımları
   - Beklenen davranış
   - Gerçekleşen davranış
   - Ekran görüntüleri (varsa)
   - Sistem bilgileri (Windows versiyonu, .NET versiyonu)

### 💡 Özellik Önerisi

1. [Discussions](../../discussions) sayfasına gidin
2. "Ideas" kategorisinde yeni tartışma açın
3. Önerinizi detaylı açıklayın:
   - Hangi problemi çözüyor?
   - Nasıl çalışmalı?
   - Örnek kullanım senaryoları
   - Mockup'lar veya tasarımlar (varsa)

### 📖 Dokümantasyon

- Yazım hatalarını düzeltin
- Eksik bölümleri tamamlayın
- Örnekler ekleyin
- Çeviriler yapın

### 💻 Kod Katkısı

- Yeni özellik ekleyin
- Hataları düzeltin
- Performans iyileştirmeleri
- Test coverage artırın

---

## 🛠️ Geliştirme Ortamı Kurulumu

### Gereksinimler

- Windows 10/11
- Visual Studio 2019 veya üzeri (Community Edition yeterli)
- .NET Framework 4.7.2 SDK
- Git
- WinGet (test için)

### Adım 1: Repository'yi Klonlayın

```bash
git clone https://github.com/AtakanKaracaDaglar/everything-uptodate.git
cd everything-uptodate
```

### Adım 2: Solution'ı Açın

```bash
# Visual Studio ile aç
start Everything_UpToDate.sln
```

### Adım 3: Build Edin

```
Build → Build Solution (Ctrl+Shift+B)
```

### Adım 4: Çalıştırın

```
Debug → Start Debugging (F5)
```

---

## 🔄 Pull Request Süreci

### 1. Fork Edin

GitHub'da "Fork" butonuna tıklayın.

### 2. Branch Oluşturun

```bash
# Feature branch
git checkout -b feature/amazing-feature

# Bugfix branch
git checkout -b fix/bug-description

# Docs branch
git checkout -b docs/documentation-improvement
```

### 3. Değişikliklerinizi Yapın

- Kod yazın
- Test edin
- Dokümante edin

### 4. Commit Edin

```bash
git add .
git commit -m "feat: Add amazing feature"
```

### 5. Push Edin

```bash
git push origin feature/amazing-feature
```

### 6. Pull Request Açın

1. GitHub'da repository'nize gidin
2. "Compare & pull request" butonuna tıklayın
3. Değişikliklerinizi açıklayın:
   - Ne yaptınız?
   - Neden yaptınız?
   - Nasıl test ettiniz?
   - İlgili issue var mı? (Closes #123)

### 7. Review Bekleyin

- Maintainer'lar kodunuzu inceleyecek
- Değişiklik isterlerse yapın
- Merge edilmesini bekleyin

---

## 📝 Kod Standartları

### C# Kod Stili

```csharp
// ✅ DOĞRU
public class UpdateService
{
    private readonly DatabaseService _databaseService;
    
    public async Task<bool> UpdateApplicationAsync(ApplicationInfo app)
    {
        // İyi yorumlar
        if (app == null)
            throw new ArgumentNullException(nameof(app));
            
        return await PerformUpdateAsync(app);
    }
}

// ❌ YANLIŞ
public class updateservice {
    DatabaseService db;
    public bool UpdateApp(ApplicationInfo a) {
        if(a==null) return false;
        // Kötü isimler, kötü format
    }
}
```

### Naming Conventions

- **Classes**: `PascalCase` (UpdateService)
- **Methods**: `PascalCase` (GetApplications)
- **Variables**: `camelCase` (applicationList)
- **Constants**: `UPPER_CASE` (MAX_RETRY_COUNT)
- **Private Fields**: `_camelCase` (_databaseService)

### Yorumlar

```csharp
// Tek satır yorumlar için //

/// <summary>
/// XML yorumlar public metodlar için
/// </summary>
/// <param name="app">Parametre açıklaması</param>
/// <returns>Dönüş değeri açıklaması</returns>
public async Task<bool> UpdateApplicationAsync(ApplicationInfo app)
{
    // TODO: Gelecekte yapılacaklar için
    // FIXME: Düzeltilmesi gereken şeyler için
    // HACK: Geçici çözümler için
}
```

---

## 📬 Commit Mesaj Kuralları

### Format

```
<type>(<scope>): <subject>

<body>

<footer>
```

### Type

- `feat`: Yeni özellik
- `fix`: Hata düzeltme
- `docs`: Dokümantasyon
- `style`: Formatla, noktalama vb.
- `refactor`: Kod iyileştirme
- `test`: Test ekleme/düzeltme
- `chore`: Build, konfigürasyon

### Örnekler

```bash
# Yeni özellik
feat(database): Add export to JSON functionality

# Hata düzeltme
fix(theme): Resolve dark mode button colors

# Dokümantasyon
docs(readme): Update installation instructions

# Refactoring
refactor(services): Extract notification logic to separate service

# Test
test(update): Add unit tests for update service

# Chore
chore(deps): Update .NET Framework to 4.8
```

---

## ✅ Checklist

PR göndermeden önce kontrol edin:

- [ ] Kod derlenip çalışıyor mu?
- [ ] Yeni özellikler test edildi mi?
- [ ] Dokümantasyon güncellendi mi?
- [ ] Commit mesajları kurallara uygun mu?
- [ ] Kod standartlarına uygun mu?
- [ ] Breaking change var mı? (Belirtildi mi?)

---

## 🎉 İlk Katkı mı?

İlk katkınız için harika kaynaklar:

- [First Contributions](https://github.com/firstcontributions/first-contributions)
- [How to Contribute to Open Source](https://opensource.guide/how-to-contribute/)
- [GitHub Flow](https://guides.github.com/introduction/flow/)

"Good First Issue" etiketli issue'lara bakın!

---

## 📞 Sorularınız mı var?

- 💬 [Discussions](../../discussions) sayfasında sorun
- 📧 Email: support@everythinguptodate.com
- 🐛 [Issues](../../issues) sayfasında issue açın

---

## 🙏 Teşekkürler!

Katkılarınız için teşekkür ederiz! Her katkı, ne kadar küçük olursa olsun değerlidir. 

**Happy Coding! 🚀**
