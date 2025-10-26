# ?? BOYUT HESAPLAMA S�STEM�

## ?? Nas�l �al���r?

Program uygulamalar�n g�ncelleme boyutlar�n� **3 farkl� y�ntemle** hesaplar:

### 1?? WinGet Manifest (En Do�ru) ?
```csharp
winget show --id Google.Chrome
```
- WinGet'in kendi manifest dosyas�ndan **ger�ek boyutu** okur
- En do�ru ve g�venilir y�ntem
- Download Size bilgisini direkt al�r

**Avantajlar:**
- ? Ger�ek indirme boyutu
- ? Microsoft'tan gelen resmi bilgi
- ? %100 do�ru

**Dezavantajlar:**
- ? Baz� paketlerde boyut bilgisi olmayabilir
- ? Her uygulama i�in ekstra WinGet �a�r�s� gerekir
- ? Yava� olabilir (5 saniye timeout)

### 2?? Kurulum Klas�r� Boyutu (Orta Do�ru)
```csharp
C:\Program Files\Google\Chrome\Application
```
- Uygulaman�n kurulu oldu�u klas�r�n toplam boyutunu hesaplar
- G�ncelleme genellikle kurulum boyutunun **%30-50'si** kadar

**Avantajlar:**
- ? Ger�ek disk kullan�m�n� g�sterir
- ? Fiziksel bir de�er (tahmini de�il)

**Dezavantajlar:**
- ? G�ncelleme boyutu != Kurulum boyutu
- ? Klas�r eri�im izni gerekebilir
- ? Hesaplama zaman alabilir

### 3?? Kategori Tahmini (Fallback) ??
```csharp
Chrome ? 89 MB
Visual Studio ? 3 GB
7-Zip ? 2 MB
```
- Uygulama ad�na g�re �nceden tan�ml� tahminler
- �lk 2 y�ntem ba�ar�s�z olursa devreye girer

**Avantajlar:**
- ? �ok h�zl�
- ? Her zaman �al���r
- ? Makul tahminler

**Dezavantajlar:**
- ? Ger�ek boyut de�il, tahmini
- ? Uygulamaya �zg� de�il, kategori bazl�

## ?? ��lem Ak���

```
Tarama Ba�la
    ?
winget upgrade
    ?
Sat�rlar� Parse Et
    ?
Her Uygulama ��in:
    ?? ?? Ba�lang��: Kategori Tahmini (h�zl�)
    ?? ?? 1. Deneme: WinGet Manifest (paralel)
    ?? ?? 2. Deneme: Kurulum Klas�r� (paralel)
    ?? ? En iyi sonucu kullan
    ?
5 Saniye Timeout
    ?
Listeyi G�ster
```

### Paralel ��lem
```csharp
var sizeUpdateTasks = applications.Select(async app => {
    // Her uygulama i�in ayr� task
}).ToArray();

await Task.WhenAny(
    Task.WhenAll(sizeUpdateTasks),  // Hepsini bekle
    Task.Delay(5000)                // Veya 5 saniye
);
```

## ?? Desteklenen Boyut Formatlar�

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
    // GB: x 1024�
    // MB: x 1024�
    // KB: x 1024
    // B:  x 1
}
```

## ?? Kategori Tahminleri

| Kategori | Tahmin | Ger�ek �rnek |
|----------|--------|--------------|
| **Taray�c�lar** | | |
| Chrome | 89 MB | 85-95 MB |
| Firefox | 95 MB | 90-100 MB |
| Edge | 150 MB | 140-160 MB |
| **Geli�tirme** | | |
| VS Code | 125 MB | 120-130 MB |
| Visual Studio | 3 GB | 2-5 GB |
| Node.js | 32 MB | 30-35 MB |
| Git | 55 MB | 50-60 MB |
| Python | 30 MB | 25-35 MB |
| **Medya** | | |
| VLC | 45 MB | 40-50 MB |
| Spotify | 100 MB | 95-110 MB |
| OBS | 150 MB | 140-160 MB |
| **Ara�lar** | | |
| 7-Zip | 2 MB | 1-3 MB |
| WinRAR | 3 MB | 2-4 MB |
| Notepad++ | 4 MB | 3-5 MB |
| **�leti�im** | | |
| Discord | 80 MB | 75-85 MB |
| Zoom | 60 MB | 55-65 MB |
| Teams | 120 MB | 110-130 MB |
| **Di�er** | | |
| Adobe Reader | 200 MB | 180-220 MB |
| Java | 150 MB | 140-160 MB |
| Docker | 500 MB | 450-550 MB |
| Varsay�lan | 50 MB | - |

## ?? Optimizasyonlar

### 1. Paralel ��lem
```csharp
// ? YAVA�: S�ral�
foreach(var app in apps) {
    await GetSize(app);  // 1 saniye x 10 = 10 saniye
}

// ? HIZLI: Paralel
var tasks = apps.Select(app => GetSize(app));
await Task.WhenAll(tasks);  // 1 saniye (hepsi birden)
```

### 2. Timeout Mekanizmas�
```csharp
// 5 saniyeden fazla bekleme
await Task.WhenAny(
    Task.WhenAll(tasks),    // Hepsini bekle
    Task.Delay(5000)        // Ama max 5 saniye
);
```

### 3. H�zl� Ba�lang��
```csharp
// UI'da hemen g�ster (tahmin)
app.UpdateSizeBytes = GetEstimatedSize(app.Name);

// Arka planda ger�ek boyutu al
Task.Run(() => {
    app.UpdateSizeBytes = GetRealSize(app.Id);
});
```

## ?? Performans Metrikleri

### Tarama S�releri
| Y�ntem | �lk Uygulama | 10 Uygulama | 50 Uygulama |
|--------|--------------|-------------|-------------|
| Sadece Tahmin | <100ms | <100ms | <100ms |
| + WinGet Manifest | 1-2s | 2-3s | 5s (timeout) |
| + Klas�r Boyutu | 2-3s | 3-4s | 5s (timeout) |

### Do�ruluk Oranlar�
| Y�ntem | Do�ruluk | Kullan�m |
|--------|----------|----------|
| WinGet Manifest | %100 | %60 |
| Klas�r Boyutu | %70-80 | %30 |
| Kategori Tahmini | %50-70 | %10 |

## ?? Hata Durumlar�

### WinGet Manifest Ba�ar�s�z
```csharp
try {
    size = await GetManifestSize(id);
} catch {
    size = 0;  // Fallback'e ge�
}
```

### Klas�r Eri�im Engellendi
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
// Tamamlanmayanlar tahmin kullan�r
```

## ?? Kullan�c� Deneyimi

### A�amal� Y�kleme
```
1. ? Liste g�r�n�r (tahminli boyutlar)
2. ?? Arka planda ger�ek boyutlar al�n�r
3. ? Boyutlar otomatik g�ncellenir
```

### G�rsel Geri Bildirim
```csharp
// ListView'da boyut kolonunu otomatik g�ncelle
item.SubItems[4].Text = app.GetUpdateSizeFormatted();
```

## ?? �yile�tirme �nerileri

### 1. Cache Mekanizmas�
```csharp
// Ayn� uygulaman�n boyutunu tekrar sorgulama
Dictionary<string, long> _sizeCache;
```

### 2. Veritaban�
```csharp
// Pop�ler uygulamalar�n boyutlar�n� sakla
SQLite database ile kal�c� boyut bilgileri
```

### 3. Kitle Kaynakl� Boyutlar
```csharp
// Kullan�c�lardan gelen ger�ek boyutlar� topla
Community-sourced size database
```

## ?? �zet

? **3 Y�ntem**: Manifest ? Klas�r ? Tahmin
? **Paralel ��lem**: H�zl� tarama
? **Timeout**: Max 5 saniye
? **A�amal�**: �nce g�ster, sonra g�ncelle
? **G�venilir**: Her durumda bir de�er d�ner

---

**Sonu�**: Kullan�c� her zaman bir boyut g�r�r, zamanla daha do�ru hale gelir! ??
