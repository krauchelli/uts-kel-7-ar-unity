# Unity AR Client for UTS - Kelompok 7

Ini adalah repositori untuk proyek *client-side* Augmented Reality (AR) yang dibangun menggunakan Unity. Aplikasi ini bertanggung jawab untuk menangani semua interaksi pengguna, fungsionalitas AR, dan komunikasi dengan layanan backend.

## Deskripsi Proyek

Tugas utama dari aplikasi Unity ini adalah:
1.  Menggunakan **AR Foundation** untuk mendeteksi dan melacak wajah pengguna secara *real-time*.
2.  Mengimplementasikan alur kerja UI yang memungkinkan pengguna memicu proses *scan*.
3.  Mengirim *request* ke layanan backend yang berisi data gambar wajah.
4.  Menerima hasil klasifikasi dari backend.
5.  Menampilkan filter AR atau elemen visual lainnya berdasarkan hasil klasifikasi tersebut.

---

## Prasyarat

Sebelum memulai, pastikan perangkat Anda telah terinstal:
* **Unity Hub**
* **Unity Editor** versi yang disepakati tim. Menggunakan versi yang sama sangat penting untuk menghindari konflik.
* **Modul Android Build Support** (diinstal melalui Unity Hub).
* `git`

---

## Panduan Setup Lokal

Ikuti langkah-langkah berikut untuk membuka dan menjalankan proyek di mesin lokal Anda.

### 1. Clone Repositori
```bash
git clone [https://github.com/user/uts-kel-7-ar-unity.git](https://github.com/user/uts-kel-7-ar-unity.git)
```

### 2. Buka Proyek di Unity Hub
**PENTING:** Jangan membuat proyek baru. Ikuti langkah ini:
1.  Buka **Unity Hub**.
2.  Klik tombol **"Open"**.
3.  Arahkan ke folder `uts-kel-7-ar-unity` yang baru saja Anda *clone*.
4.  Unity akan secara otomatis memuat proyek dan mengimpor semua aset. Proses ini mungkin memakan waktu beberapa menit saat pertama kali.

### 3. Instalasi & Verifikasi Packages
1.  Setelah proyek terbuka, buka **Window > Package Manager**.
2.  Pastikan *packages* berikut sudah terinstal (jika belum, cari dan instal):
    * **AR Foundation**

### 4. Konfigurasi Player Settings
1.  Buka **Edit > Project Settings**.
2.  Masuk ke tab **XR Plug-in Management**.
3.  Pastikan *checkbox* untuk **ARCore** (di tab Android) sudah dicentang.

---

## Struktur Proyek yang Disarankan

Untuk menjaga agar proyek tetap rapi, gunakan struktur folder berikut di dalam direktori `/Assets`:
* `/Assets/Scenes/`: Untuk menyimpan semua *file scene*.
* `/Assets/Scripts/`: Untuk semua *script* C#. Bisa dibagi lagi menjadi:
    * `/API/`: Untuk *script* yang berhubungan dengan komunikasi backend.
    * `/AR/`: Untuk *script* logika AR.
    * `/UI/`: Untuk *script* manajemen UI.
* `/Assets/Prefabs/`: Untuk menyimpan semua *prefab*, terutama filter AR.
* `/Assets/Materials/`: Untuk menyimpan semua *material*.
* `/Assets/Models/`: Untuk menyimpan aset model 3D.

---
## Konfigurasi API
Di dalam `/Assets/Scripts/API/`, akan ada sebuah *script* (misal: `APIManager.cs`) yang mengatur komunikasi. Untuk keperluan development, URL backend dapat diatur di dalam *script* ini.

```csharp
// Contoh di dalam APIManager.cs
public class APIManager : MonoBehaviour
{
    // Ganti URL ini dengan URL dari tim backend saat integrasi
    private string baseURL = "[http://127.0.0.1:5000/classify](http://127.0.0.1:5000/classify)"; 
    
    // ...
}
```
Selama backend belum siap, kita akan menggunakan **Mock Service** yang datanya di-*hardcode* sesuai **Kontrak API** yang disepakati.