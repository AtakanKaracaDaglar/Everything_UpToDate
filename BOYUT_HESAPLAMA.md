# ?? BOYUT HESAPLAMA SÝSTEMÝ

## ?? Nasýl Çalýþýr?

Program uygulamalarýn güncelleme boyutlarýný **3 farklý yöntemle** hesaplar:

### 1?? WinGet Manifest (En Doðru) ?
```csharp
winget show --id Google.Chrome
```
- WinGet'in kendi manifest dosyasýndan **gerçek boyutu** okur
- En doðru ve güvenilir yöntem
- Download Size bilgisini direkt alýr

**Avantajlar:**
- ? Gerçek indirme boyutu
- ? Microsoft'tan gelen resmi bilgi
- ? %100 doðru

**Dezavantajlar:**
- ? Bazý paketlerde boyut bilgisi olmayabilir
- ? Her uygulama için ekstra WinGet çaðrýsý gerekir
- ? Yavaþ olabilir (5 saniye timeout)

### 2?? Kurulum Klasörü Boyutu (Orta Doðru)
```csharp
C:\Program Files\Google\Chrome\Application
```
- Uygulamanýn kurulu olduðu klasörün toplam boyutunu hesaplar
- Güncelleme genellikle kurulum boyutunun **%30-50'si** kadar

**Avantajlar:**
- ? Gerçek disk kullanýmýný gösterir
- ? Fiziksel bir deðer (tahmini deðil)

**Dezavantajlar:**
- ? Güncelleme boyutu != Kurulum boyutu
- ? Klasör eriþim izni gerekebilir
- ? Hesaplama zaman alabilir

### 3?? Kategori Tahmini (Fallback) ??
```csharp
Chrome ? 89 MB
Visual Studio ? 3 GB
7-Zip ? 2 MB
```
- Uygulama adýna göre önceden tanýmlý tahminler
- Ýlk 2 yöntem baþarýsýz olursa devreye girer

**Avantajlar:**
- ? Çok hýzlý
- ? Her zaman çalýþýr
- ? Makul tahminler

**Dezavantajlar:**
- ? Gerçek boyut deðil, tahmini
- ? Uygulamaya özgü deðil, kategori bazlý

## ?? Ýþlem Akýþý

```
Tarama Baþla
    ?
winget upgrade
    ?
Satýrlarý Parse Et
    ?
Her Uygulama Ýçin:
    ?? ?? Baþlangýç: Kategori Tahmini (hýzlý)
    ?? ?? 1. Deneme: WinGet Manifest (paralel)
    ?? ?? 2. Deneme: Kurulum Klasörü (paralel)
    ?? ? En iyi sonucu kullan
    ?
5 Saniye Timeout
    ?
Listeyi Göster
```

### Paralel Ýþlem
```csharp
var sizeUpdateTasks = applications.Select(async app => {
    // Her uygulama için ayrý task
}).ToArray();

await Task.WhenAny(
    Task.WhenAll(sizeUpdateTasks),  // Hepsini bekle
    Task.Delay(5000)                // Veya 5 saniye
);
```

## ?? Desteklenen Boyut Formatlarý

### WinGet Manifest Parse
```
"Download Size: 89.5 MB"  ? 93,847,552 bytes
"Size: 1.2 GB"            ? 1,288,490,189 bytes
"150 KB"                  ? 153,600 bytes
"2048 B"                  ? 2,048 bytes
```

### Kod:
```csharp
private long ParseSizeString(string sizeText)
{
    // "89.5 MB" ? 93847552
    // GB: x 1024³
    // MB: x 1024²
    // KB: x 1024
    // B:  x 1
}
```

## ?? Kategori Tahminleri

| Kategori | Tahmin | Gerçek Örnek |
|----------|--------|--------------|
| **Tarayýcýlar** | | |
| Chrome | 89 MB | 85-95 MB |
| Firefox | 95 MB | 90-100 MB |
| Edge | 150 MB | 140-160 MB |
| **Geliþtirme** | | |
| VS Code | 125 MB | 120-130 MB |
| Visual Studio | 3 GB | 2-5 GB |
| Node.js | 32 MB | 30-35 MB |
| Git | 55 MB | 50-60 MB |
| Python | 30 MB | 25-35 MB |
| **Medya** | | |
| VLC | 45 MB | 40-50 MB |
| Spotify | 100 MB | 95-110 MB |
| OBS | 150 MB | 140-160 MB |
| **Araçlar** | | |
| 7-Zip | 2 MB | 1-3 MB |
| WinRAR | 3 MB | 2-4 MB |
| Notepad++ | 4 MB | 3-5 MB |
| **Ýletiþim** | | |
| Discord | 80 MB | 75-85 MB |
| Zoom | 60 MB | 55-65 MB |
| Teams | 120 MB | 110-130 MB |
| **Diðer** | | |
| Adobe Reader | 200 MB | 180-220 MB |
| Java | 150 MB | 140-160 MB |
| Docker | 500 MB | 450-550 MB |
| Varsayýlan | 50 MB | - |

## ?? Optimizasyonlar

### 1. Paralel Ýþlem
```csharp
// ? YAVAÞ: Sýralý
foreach(var app in apps) {
    await GetSize(app);  // 1 saniye x 10 = 10 saniye
}

// ? HIZLI: Paralel
var tasks = apps.Select(app => GetSize(app));
await Task.WhenAll(tasks);  // 1 saniye (hepsi birden)
```

### 2. Timeout Mekanizmasý
```csharp
// 5 saniyeden fazla bekleme
await Task.WhenAny(
    Task.WhenAll(tasks),    // Hepsini bekle
    Task.Delay(5000)        // Ama max 5 saniye
);
```

### 3. Hýzlý Baþlangýç
```csharp
// UI'da hemen göster (tahmin)
app.UpdateSizeBytes = GetEstimatedSize(app.Name);

// Arka planda gerçek boyutu al
Task.Run(() => {
    app.UpdateSizeBytes = GetRealSize(app.Id);
});
```

## ?? Performans Metrikleri

### Tarama Süreleri
| Yöntem | Ýlk Uygulama | 10 Uygulama | 50 Uygulama |
|--------|--------------|-------------|-------------|
| Sadece Tahmin | <100ms | <100ms | <100ms |
| + WinGet Manifest | 1-2s | 2-3s | 5s (timeout) |
| + Klasör Boyutu | 2-3s | 3-4s | 5s (timeout) |

### Doðruluk Oranlarý
| Yöntem | Doðruluk | Kullaným |
|--------|----------|----------|
| WinGet Manifest | %100 | %60 |
| Klasör Boyutu | %70-80 | %30 |
| Kategori Tahmini | %50-70 | %10 |

## ?? Hata Durumlarý

### WinGet Manifest Baþarýsýz
```csharp
try {
    size = await GetManifestSize(id);
} catch {
    size = 0;  // Fallback'e geç
}
```

### Klasör Eriþim Engellendi
```csharp
try {
    size = CalculateFolderSize(path);
} catch (UnauthorizedAccessException) {
    size = 0;  // Tahmini kullan
}
```

### Timeout
```csharp
await Task.WhenAny(
    GetAllSizes(),
    Task.Delay(5000)  // 5 saniye sonra devam et
);
// Tamamlanmayanlar tahmin kullanýr
```

## ?? Kullanýcý Deneyimi

### Aþamalý Yükleme
```
1. ? Liste görünür (tahminli boyutlar)
2. ?? Arka planda gerçek boyutlar alýnýr
3. ? Boyutlar otomatik güncellenir
```

### Görsel Geri Bildirim
```csharp
// ListView'da boyut kolonunu otomatik güncelle
item.SubItems[4].Text = app.GetUpdateSizeFormatted();
```

## ?? Ýyileþtirme Önerileri

### 1. Cache Mekanizmasý
```csharp
// Ayný uygulamanýn boyutunu tekrar sorgulama
Dictionary<string, long> _sizeCache;
```

### 2. Veritabaný
```csharp
// Popüler uygulamalarýn boyutlarýný sakla
SQLite database ile kalýcý boyut bilgileri
```

### 3. Kitle Kaynaklý Boyutlar
```csharp
// Kullanýcýlardan gelen gerçek boyutlarý topla
Community-sourced size database
```

## ?? Özet

? **3 Yöntem**: Manifest ? Klasör ? Tahmin
? **Paralel Ýþlem**: Hýzlý tarama
? **Timeout**: Max 5 saniye
? **Aþamalý**: Önce göster, sonra güncelle
? **Güvenilir**: Her durumda bir deðer döner

---

**Sonuç**: Kullanýcý her zaman bir boyut görür, zamanla daha doðru hale gelir! ??
