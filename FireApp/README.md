# FireApp 🔥

![Language](https://img.shields.io/badge/Language-C%23-blue) ![Framework](https://img.shields.io/badge/Framework-.NET%209.0-purple) ![Type](https://img.shields.io/badge/Type-Windows%20Forms-green)

**(Bahasa Indonesia & English version available below)**

---

## 🇮🇩 Bahasa Indonesia

**FireApp** adalah aplikasi simulasi efek api prosedural klasik (mirip dengan efek api di game DOOM jadul) yang ditulis menggunakan C# dan Windows Forms. Aplikasi ini mendemonstrasikan manipulasi piksel performa tinggi menggunakan GDI+.

### Fitur Utama
- **Algoritma Doom Fire**: Simulasi penyebaran panas dari bawah ke atas dengan efek pendinginan (decay) acak untuk simulasi angin.
- **Rendering Cepat**: Menggunakan `Bitmap.LockBits` dan `Marshal.Copy` untuk menulis data piksel secara langsung ke dalam memori (jauh lebih cepat daripada `SetPixel`).
- **Tampilan Retro**: Resolusi internal 320x240 yang diperbesar (upscaled) ke 800x600 menggunakan interpolasi `NearestNeighbor` untuk memberikan kesan pixel art yang tajam.
- **Palet Warna Kustom**: Menggunakan 37 tingkatan warna dari hitam, merah, oranye, kuning, hingga putih.

### Cara Menjalankan
1. Pastikan Anda memiliki **.NET 9.0 SDK** terinstal.
2. Buka terminal di folder project.
3. Jalankan perintah:
   ```bash
   dotnet run
   ```
4. Atau buka file `.csproj` menggunakan Visual Studio 2022 dan tekan F5.

### Struktur Kode
- **Program.cs**: Titik masuk aplikasi (Entry Point).
- **Form1.cs**: Logika utama simulasi api, loop timer, dan rendering grafis.

### Kredit
Dibuat dengan ❤️ oleh **Jacky the Code Bender** dari **Gravicode Studios**.

---

## 🇬🇧 English

**FireApp** is a procedural fire simulation application (implementing the classic DOOM fire algorithm) written in C# and Windows Forms. It demonstrates high-performance pixel manipulation using GDI+.

### Key Features
- **Doom Fire Algorithm**: Simulates heat propagation from bottom to top with random decay to simulate wind effects.
- **High Performance Rendering**: Uses `Bitmap.LockBits` and `Marshal.Copy` for direct memory pixel manipulation (significantly faster than `SetPixel`).
- **Retro Look**: Internal resolution of 320x240 upscaled to 800x600 using `NearestNeighbor` interpolation for a crisp pixel art aesthetic.
- **Custom Color Palette**: Utilizes a 37-color gradient ranging from black, red, orange, yellow, to white.

### How to Run
1. Ensure you have **.NET 9.0 SDK** installed.
2. Open a terminal in the project folder.
3. Run the command:
   ```bash
   dotnet run
   ```
4. Alternatively, open the `.csproj` file in Visual Studio 2022 and press F5.

### Code Structure
- **Program.cs**: Application Entry Point.
- **Form1.cs**: Contains the main fire logic, simulation loop, and graphics rendering.

### Credits
Crafted with ❤️ by **Jacky the Code Bender** from **Gravicode Studios**.
