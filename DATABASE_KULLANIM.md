# ?? DATABASE VE LOGLama S�STEM� - NASIL KULLANILIR?

## ?? NEREDEK� DOSYALAR?

### Database Konumu:
```
C:\Users\[KULLANICI ADI]\AppData\Roaming\EverythingUpToDate\
```

### Dosyalar:
1. **update_history.csv** - T�m g�ncelleme kay�tlar�
2. **settings.ini** - Uygulama ayarlar�

---

## ?? DATABASE'� G�R�NT�LEME

### 1. Uygulama ��inden (EN KOLAY)
1. Uygulamay� a�
2. **"?? Ge�mi�i G�r�nt�le"** butonuna t�kla
3. A��lan pencerede:
   - Toplam g�ncelleme say�s�
   - Ba�ar�l�/Ba�ar�s�z oranlar
   - Toplam indirilen boyut
   - Son 10 g�ncelleme
   - En �ok g�ncellenen uygulamalar

### 2. Dosya Gezgini'nden
1. Win + R tu�lar�na bas
2. �unu yaz: `%APPDATA%\EverythingUpToDate`
3. Enter'a bas
4. `update_history.csv` dosyas�n� a� (Excel veya Notepad ile)

### 3. PowerShell'den
```powershell
# T�m kay�tlar� g�r�nt�le
Get-Content "$env:APPDATA\EverythingUpToDate\update_history.csv" | Format-Table

# Son 5 kayd� g�r�nt�le
Get-Content "$env:APPDATA\EverythingUpToDate\update_history.csv" | Select-Object -Last 5
```

---

## ?? OTOMAT�K LOGLama NASIL �ALI�IR?

### Her G�ncelleme Otomatik Kaydedilir!

Bir uygulama g�ncelledi�inde **otomatik olarak** �u bilgiler kaydedilir:

```csv
Id,AppName,AppId,FromVersion,ToVersion,UpdateDate,Success,UpdateSizeBytes,DurationSeconds,ErrorMessage
1,Google Chrome,Google.Chrome,120.0,121.0,2025-01-26 10:30:00,True,93847552,45,""
```

### Kaydedilen Bilgiler:
- ? Uygulama Ad� (Google Chrome)
- ? Uygulama ID'si (Google.Chrome)
- ? Eski Versiyon (120.0.6099)
- ? Yeni Versiyon (121.0.6167)
- ? G�ncelleme Tarihi ve Saati
- ? Ba�ar�l� m�? (True/False)
- ? �ndirilen Boyut (byte cinsinden)
- ? G�ncelleme S�resi (saniye)
- ? Hata Mesaj� (varsa)

---

## ?? KULLANIM �RNEKLER�

### �rnek 1: Ba�ar�l� G�ncelleme
```
? Google Chrome
   120.0.6099 ? 121.0.6167
   26.01.2025 10:30 - 89.50 MB
   S�re: 45 saniye
```

### �rnek 2: Ba�ar�s�z G�ncelleme
```
? Node.js
   20.10.0 ? 20.11.0
   26.01.2025 12:00 - 0 MB
   Hata: WinGet exit code: -1978335189
```

---

## ?? �STAT�ST�KLER

Database Service �u istatistikleri sa�lar:

### 1. Genel �statistikler
```csharp
var stats = _databaseService.GetStatistics();
// Toplam: 150 g�ncelleme
// Ba�ar�l�: 142 (94.6%)
// Ba�ar�s�z: 8 (5.3%)
// Toplam �ndirilen: 12.5 GB
```

### 2. En �ok G�ncellenen Uygulamalar (Top 5)
```csharp
var topApps = _databaseService.GetMostUpdatedApps(5);
// 1. Google Chrome (15 kez)
// 2. VS Code (12 kez)
// 3. Node.js (10 kez)
// 4. Firefox (8 kez)
// 5. Git (7 kez)
```

### 3. Ayl�k Trend
```csharp
var monthly = _databaseService.GetMonthlyStatistics();
// 2025-01: 45 g�ncelleme
// 2024-12: 38 g�ncelleme
// 2024-11: 52 g�ncelleme
```

---

## ??? �LER� SEV�YE ��LEMLER

### Excel'de Analiz
1. `update_history.csv` dosyas�n� Excel'de a�
2. Pivot Table olu�tur
3. Ayl�k/Haftal�k analiz yap
4. Grafikler olu�tur

### Database Temizleme
```csharp
// 90 g�nden eski kay�tlar� sil
_databaseService.DeleteOldRecords(90);

// T�m ge�mi�i temizle
_databaseService.ClearHistory();
```

### Export
```csharp
// Yedek al
_databaseService.ExportToCsv(@"C:\Yedek\guncellemeler.csv");
```

---

## ?? SORUN G�DERME

### "Ge�mi� bo� g�r�n�yor"
**Neden:** Hen�z hi� g�ncelleme yapmad�n�z.
**��z�m:** Bir uygulama g�ncelleyin, otomatik kaydedilecek!

### "Dosya bulunamad�"
**Neden:** �lk �al��t�rmada dosya hen�z olu�mam��.
**��z�m:** Bir kez tarama yap�n, klas�r otomatik olu�ur.

### "CSV bozuk g�r�n�yor"
**Neden:** Uygulama �al���rken dosya a��ksa.
**��z�m:** Uygulamay� kapat�p tekrar deneyin.

---

## ?? �PU�LARI

### 1. D�zenli Yedekleme
Her ay database'i yedekleyin:
```powershell
Copy-Item "$env:APPDATA\EverythingUpToDate\update_history.csv" "C:\Yedek\history_$(Get-Date -Format 'yyyy-MM').csv"
```

### 2. Performans
- 1000+ kay�t olunca eski kay�tlar� silin
- Export ile yedekleyip temizleyin

### 3. Analiz
- Excel PivotTable kullan�n
- Power BI ile g�rselle�tirin
- Python ile analiz yap�n

---

## ?? EKRAN G�R�NT�LER�

### Ge�mi� Penceresi:
```
???????????????????????????????????????
  G�NCELLEME GE�M��� VE �STAT�ST�KLER
???????????????????????????????????????

?? GENEL �STAT�ST�KLER:
   Toplam G�ncelleme: 3
   ? Ba�ar�l�: 2
   ? Ba�ar�s�z: 1
   ?? Toplam �ndirilen: 214.50 MB

?? SON 10 G�NCELLEME:

? Google Chrome
   120.0.6099 ? 121.0.6167
   26.01.2025 10:30 - 89.50 MB

? Visual Studio Code
   1.85.0 ? 1.86.0
   26.01.2025 11:15 - 125.00 MB

? Node.js
   20.10.0 ? 20.11.0
   26.01.2025 12:00 - 0.00 B
   Hata: WinGet exit code: -1978335189

?? EN �OK G�NCELLENEN UYGULAMALAR:
   1. Google Chrome (1 kez)
   2. Visual Studio Code (1 kez)
   3. Node.js (1 kez)

???????????????????????????????????????
```

---

## ?? �ZET

? **Otomatik:** Her g�ncelleme kaydedilir
? **Kolay:** Tek butonla g�r�nt�le
? **Detayl�:** T�m bilgiler saklan�r
? **G�venli:** CSV format�nda, kolayca yedeklenebilir
? **Analiz:** Excel, Power BI, Python ile analiz edilebilir

---

**Keyifli G�ncellemeler!** ??
