# ?? KULLANIM KILAVUZU - Everything UpToDate (WinGet Edition)

## ?? �NEML�: Bu Ger�ek Bir G�ncelleme Arac�d�r!

Bu program **DEMO DE��L**dir. Yapt���n�z g�ncellemeler **ger�ekten** uygulamalar�n�z� g�ncelleyecektir!

## ?? Gereksinimler

### Mutlaka Gerekli
? **Windows 10/11** (1809 veya �zeri)  
? **WinGet (Windows Package Manager)** kurulu olmal�

### WinGet Nas�l Y�klenir?

#### Windows 11 Kullan�c�lar�
- Zaten y�kl�! Hi�bir �ey yapman�za gerek yok.

#### Windows 10 Kullan�c�lar�
1. **Microsoft Store**'u a��n
2. **"App Installer"** aray�n
3. **Y�kle** butonuna bas�n
4. Bilgisayar� **yeniden ba�lat�n**
5. Terminal a��p `winget --version` yaz�n - versiyon g�r�yorsan�z haz�r!

## ?? H�zl� Ba�lang��

### �lk �al��t�rma
```
1. Program� ba�lat�n
2. WinGet kontrol� otomatik yap�l�r
3. E�er WinGet yoksa uyar� g�r�rs�n�z
4. WinGet varsa otomatik tarama ba�lar (10-30 saniye s�rebilir, sab�rl� olun!)
5. Uygulamalar listelenir
```

## ?? Ekran A��klamas�

```
???????????????????????????????????????????????????????????????????
?  ?? Uygulamalar� Tara    [6 g�ncelleme mevcut]                  ?
???????????????????????????????????????????????????????????????????
?  ? Google Chrome        ? 120.0  ? 121.0 ? G�ncelleme Var  ? 89MB ? ??
?  ? VS Code              ? 1.85   ? 1.86  ? G�ncelleme Var  ?125MB ? ??
?  ? Node.js              ? 18.17  ? 20.11 ? G�ncelleme Var  ? 32MB ? ??
?  ? Adobe Reader         ? 23.006 ? 23.006? G�ncel ?        ?  -   ? ??
???????????????????????????????????????????????????????????????????
?  [? Se�ilenleri G�ncelle]  [? T�m�n� G�ncelle]                ?
???????????????????????????????????????????????????????????????????
?  ?????????????????????????????????? 60%                        ?
?  Chrome: �ndiriliyor... 60%                                     ?
???????????????????????????????????????????????????????????????????
?  G�NCELLEME DETAYI:                                             ?
?  ???????????????????????????????????????????????????????????    ?
?  ?? Uygulama Ad�      : Google Chrome                           ?
?  ?? Mevcut Versiyon   : 120.0.6099.109                         ?
?  ?? Yeni Versiyon     : 121.0.6167.85                          ?
?  ?? Kurulum Yolu      : C:\Program Files\Google\Chrome\...     ?
?  ?? G�ncelleme Boyutu : 89.00 MB                               ?
?  ?? Son Kontrol       : 27.12.2024 14:30:45                    ?
?  ???????????????????????????????????????????????????????????    ?
?  Web taray�c�s� - G�venlik g�ncellemesi ve performans          ?
?  iyile�tirmeleri                                                ?
???????????????????????????????????????????????????????????????????
```

## ?? Buton A��klamalar�

### ?? Uygulamalar� Tara
- **Ne yapar**: WinGet ile sistemi tarar (GER�EK!)
- **S�re**: 10-30 saniye (ilk taramada daha uzun)
- **Ne bulur**: WinGet ile y�klenen t�m uygulamalar
- **Ne zaman kullan�l�r**: �lk a��l��ta veya listeyi yenilemek istedi�inizde
- **Renk**: Mavi

### ? Se�ilenleri G�ncelle
- **Ne yapar**: Checkbox'lar� i�aretli uygulamalar� GER�EKTEN g�nceller
- **Uyar�**: Bu i�lem geri al�namaz!
- **�zin**: Baz� uygulamalar y�netici izni isteyebilir
- **Ne zaman kullan�l�r**: Belirli uygulamalar� g�ncellemek istedi�inizde
- **Renk**: Ye�il
- **Not**: En az 1 uygulama se�ilmelidir

### ? T�m�n� G�ncelle
- **Ne yapar**: G�ncelleme gerektiren T�M uygulamalar� GER�EKTEN g�nceller
- **Uyar�**: Bu i�lem �ok uzun s�rebilir ve geri al�namaz!
- **�zin**: Kesinlikle y�netici izni gerekir
- **Ne zaman kullan�l�r**: T�m g�ncellemeleri bir seferde yapmak istedi�inizde
- **Renk**: K�rm�z�
- **Tavsiye**: �nce �nemli dosyalar�n�z� kaydedin!

## ?? Durum G�stergeleri

### Renk Kodlar�
- ?? **Sar� Arka Plan**: G�ncelleme mevcut (WinGet'ten gelen ger�ek veri)
- ?? **Ye�il Arka Plan**: G�ncel (WinGet onaylad�)

### Durum Mesajlar�
- "G�ncelleme Mevcut ?": Yeni versiyon var (WinGet tespit etti)
- "G�ncel ?": En g�ncel versiyon kurulu
- "Kontrol ediliyor...": WinGet kontrol ediyor
- "�ndiriliyor... X%": WinGet indiriyor (GER�EK!)
- "Kuruluyor... X%": WinGet y�kl�yor (GER�EK!)
- "Tamamland� ?": G�ncelleme ba�ar�l�

## ?? �pu�lar�

### 1. Tek Uygulama G�ncellemek ��in
```
1. "Uygulamalar� Tara" butonuna t�klay�n (10-30 saniye bekleyin)
2. G�ncellemek istedi�iniz uygulaman�n checkbox'�n� i�aretleyin
3. "Se�ilenleri G�ncelle" butonuna t�klay�n
4. Onaylay�n (GER� ALINAMAZ!)
5. ��lem bitene kadar bekleyin
```

### 2. Birden Fazla Uygulama G�ncellemek ��in
```
1. "Uygulamalar� Tara" butonuna t�klay�n
2. G�ncellemek istedi�iniz T�M uygulamalar�n checkbox'lar�n� i�aretleyin
3. "Se�ilenleri G�ncelle" butonuna t�klay�n
4. Onaylay�n
5. S�rayla hepsi g�ncellenecek (uzun s�rebilir)
```

### 3. T�m�n� G�ncellemek ��in
```
?? UYARI: Bu i�lem �OK uzun s�rebilir!

1. "Uygulamalar� Tara" butonuna t�klay�n
2. "T�m�n� G�ncelle" butonuna t�klay�n
3. Onaylay�n (checkbox i�aretlemeye gerek yok!)
4. Kahve molas� verin ? (30-60 dakika s�rebilir)
5. ��lem bitene kadar bilgisayar� kapatmay�n!
```

### 4. Uygulama Detaylar�n� G�rmek ��in
```
1. Listeden bir uygulamaya tek t�klay�n (checkbox'a de�il, sat�ra)
2. Alt panelde detaylar g�r�nt�lenir
3. Versiyon numaralar� WinGet'ten gelir
```

## ?? �OK �NEML� UYARILAR

### G�ncelleme Yapmadan �nce

#### ? YAPMANIZ GEREKENLER:
1. **�nemli dosyalar�n�z� kaydedin**
2. **A��k t�m uygulamalar� kapat�n**
3. **�nternet ba�lant�n�z� kontrol edin**
4. **Yeterli disk alan� oldu�undan emin olun** (en az 5 GB)
5. **Bilgisayar�n�z�n prize tak�l� oldu�undan emin olun** (laptop kullan�c�lar�)

#### ? YAPMAMANIZ GEREKENLER:
1. **G�ncelleme s�ras�nda bilgisayar� kapatmay�n**
2. **Program� kapatmay�n**
3. **�nterneti kesmeyin**
4. **Ba�ka g�ncellemeleri ayn� anda ba�latmay�n**

### Y�netici �zni

Baz� uygulamalar **y�netici izni** gerektirebilir:
- Google Chrome
- Mozilla Firefox
- Node.js
- Python
- Git
- ve daha fazlas�...

**��z�m**: Program� **Y�netici olarak �al��t�r�n**
1. Program ikonuna sa� t�klay�n
2. "Y�netici olarak �al��t�r" se�in
3. UAC penceresinde "Evet" deyin

## ?? S�k Kullan�m Senaryolar�

### Senaryo 1: Haftal�k Rutin G�ncelleme
```
? �nerilen Y�ntem:

1. Program� y�netici olarak ba�lat�n
2. "Uygulamalar� Tara" butonuna t�klay�n (10-30 saniye)
3. Kritik uygulamalar� se�in (Chrome, Firefox, vs.)
4. "Se�ilenleri G�ncelle" butonuna t�klay�n
5. Onaylay�n ve bekleyin

? S�re: 5-15 dakika
```

### Senaryo 2: Kritik G�venlik G�ncellemesi
```
?? Acil Durum:

1. Program� ba�lat�n
2. �lk tarama otomatik ba�lar
3. Sar� arka planl� (g�ncelleme var) kritik uygulamalar� bulun
4. Sadece o uygulamalar�n checkbox'lar�n� i�aretleyin
5. "Se�ilenleri G�ncelle" butonuna t�klay�n
6. Hemen i�lem ba�lar

? S�re: 2-10 dakika
```

### Senaryo 3: Ayda Bir B�y�k Temizlik
```
? Toplu G�ncelleme:

1. Program� y�netici olarak ba�lat�n
2. "Uygulamalar� Tara" butonuna t�klay�n
3. "T�m�n� G�ncelle" butonuna t�klay�n
4. ONAYLAYIN (geri d�n�� yok!)
5. Kahve molas� verin ???

? S�re: 30-60 dakika (uygulama say�s�na g�re)
```

## ?? Sorun Giderme

### Problem: "WinGet Bulunamad�" Uyar�s�
**Sebep**: WinGet sisteminizde kurulu de�il

**��z�m**:
```
1. Microsoft Store'u a��n
2. "App Installer" aray�n
3. Y�kle butonuna bas�n
4. Bilgisayar� yeniden ba�lat�n
5. Terminal a��p "winget --version" yaz�n
6. Versiyon g�r�yorsan�z haz�r!
7. Program� yeniden ba�lat�n
```

### Problem: "Y�netici �zni Gerekiyor" Hatas�
**Sebep**: Uygulama y�netici izni gerektiriyor

**��z�m**:
```
1. Program� kapat�n
2. Sa� t�k > "Y�netici olarak �al��t�r"
3. UAC penceresinde "Evet" deyin
4. G�ncellemeyi tekrar deneyin
```

### Problem: "Uygulama Listesi Bo�"
**Sebep**: WinGet hen�z taramay� tamamlamad�

**��z�m**:
```
1. 30 saniye bekleyin (tarama uzun s�rebilir)
2. "Uygulamalar� Tara" butonuna tekrar t�klay�n
3. Hala bo�sa: Terminal a��p "winget list" yaz�n
4. Terminal'de uygulama g�r�yorsan�z programda bug var
5. G�rm�yorsan�z WinGet sorunu var
```

### Problem: "G�ncelleme Ba�ar�s�z" Mesaj�
**Sebep**: �e�itli nedenler olabilir

**��z�m**:
```
1. �nternet ba�lant�n�z� kontrol edin
2. Disk alan�n�z� kontrol edin
3. Uygulamay� kapat�n (�al���yorsa g�ncellenemez)
4. Y�netici izniyle tekrar deneyin
5. Bilgisayar� yeniden ba�lat�n
```

### Problem: Tarama 30 Saniyeden Fazla S�r�yor
**Sebep**: Normal olabilir, �zellikle ilk taramada

**��z�m**:
```
1. Sab�rl� olun - WinGet yava� olabilir
2. 2-3 dakika bekleyin
3. Hala devam ediyorsa program� kapat�n
4. Terminal a��p "winget list" yaz�n
5. Terminal'de de yava�sa WinGet sorunu var
```

## ?? Performans ve Beklentiler

### Tarama S�releri
- **�lk Tarama**: 30-60 saniye (normal)
- **Sonraki Taramalar**: 10-30 saniye
- **�ok Fazla Uygulama**: 1-2 dakika

### G�ncelleme S�releri
| Uygulama Tipi | Boyut | S�re |
|--------------|-------|------|
| K���k (7-Zip) | 2 MB | 30 saniye |
| Orta (Chrome) | 89 MB | 2-5 dakika |
| B�y�k (VS Code) | 125 MB | 5-10 dakika |
| �ok B�y�k (Adobe) | 200+ MB | 10-20 dakika |

### �nternet H�z� Etkisi
- **Yava� (1 Mbps)**: Her 10 MB i�in ~2 dakika
- **Orta (10 Mbps)**: Her 10 MB i�in ~10 saniye
- **H�zl� (100 Mbps)**: Her 10 MB i�in ~1 saniye

## ?? �leri Seviye Kullan�m

### WinGet Komut Sat�r�

Program� kullanmak istemiyorsan�z, do�rudan WinGet kullanabilirsiniz:

```bash
# T�m g�ncellemeleri g�ster
winget upgrade

# Tek uygulama g�ncelle
winget upgrade --id Google.Chrome

# T�m g�ncellemeleri yap (D�KKATL�!)
winget upgrade --all

# Sessiz g�ncelleme
winget upgrade --id Google.Chrome --silent

# Belirli bir uygulamay� ara
winget search "Chrome"

# Uygulama bilgilerini g�ster
winget show --id Google.Chrome
```

### Hangi Uygulamalar WinGet'te?

```bash
# Sistemdeki WinGet uygulamalar�n� listele
winget list

# Belirli bir uygulamay� ara
winget search "Visual Studio Code"

# T�m WinGet uygulamalar�n� g�ster
winget search --source winget
```

### Program �zelle�tirme

Form1.cs > Form1_Load metodunda WinGet kontrol�n� devre d��� b�rakabilirsiniz:

```csharp
// Bu sat�rlar� yorum sat�r� yap�n:
// bool wingetAvailable = await _updateService.CheckWinGetAvailabilityAsync();
// if (!wingetAvailable) { ... }
```

## ?? G�venlik

### Bu Program G�venli mi?
? **EVET**
- WinGet Microsoft'un resmi arac�d�r
- Program sadece WinGet komutlar�n� �al��t�r�r
- Kendi sunucular�m�z yok
- Kendi g�ncelleme sistemimiz yok
- Her �ey WinGet �zerinden

### Hangi �zinler Gerekli?
- **Y�netici**: Baz� uygulamalar i�in
- **�nternet**: G�ncelleme indirme i�in
- **Disk**: Ge�ici dosyalar i�in

### Gizlilik
- Hi�bir veri toplam�yoruz
- Sunucuya ba�lanm�yoruz
- WinGet Microsoft'a telemetri g�nderebilir (Microsoft politikas�)

## ?? Destek

### Hata Bildirimi
1. Hata mesaj�n� not al�n
2. Ekran g�r�nt�s� al�n
3. `winget --version` ��kt�s�n� al�n
4. `winget list` ��kt�s�n� al�n

### Faydal� Linkler
- [WinGet Dok�mantasyon](https://docs.microsoft.com/windows/package-manager/)
- [WinGet GitHub](https://github.com/microsoft/winget-cli)
- [App Installer �ndir](https://www.microsoft.com/store/productId/9NBLGGH4NNS1)

---

**Ba�ar�l� G�ncellemeler! ??**

**Unutmay�n**: Bu program GER�EK g�ncellemeler yapar. Dikkatli olun!
