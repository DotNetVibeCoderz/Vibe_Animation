# ModelViewer3D - Desktop 3D Viewer

![WPF + Blazor](https://img.shields.io/badge/Stack-WPF%20%2B%20Blazor-purple)
![Three.js](https://img.shields.io/badge/3D-Three.js-black)

**ModelViewer3D** adalah aplikasi desktop modern yang menggabungkan kekuatan **WPF** dan **Blazor Hybrid** untuk merender model 3D interaktif. Aplikasi ini memungkinkan pengguna untuk memuat model `.glb` dan mengontrolnya dalam lingkungan 3D (desa prosedural) dengan tampilan *Third Person*.

## 🌟 Fitur
- **Hybrid Technology**: Menggunakan `Microsoft.AspNetCore.Components.WebView.Wpf` untuk performa UI web di desktop.
- **3D Rendering**: Ditenagai oleh **Three.js**.
- **Custom Model Loader**: Memuat file `.glb` dari folder `Assets`.
- **Procedural Scene**: Desa sederhana yang digenerate secara acak setiap kali dijalankan.
- **3rd Person Control**: Kontrol karakter menggunakan Keyboard (WASD) dan Mouse.
- **Sound**: Background music ambience.

## 🎮 Cara Menggunakan (User Guide)

### 1. Persiapan
Pastikan Anda memiliki **.NET 8.0 SDK** terinstall.

### 2. Menjalankan Aplikasi
Buka terminal di folder project dan jalankan perintah:
```bash
dotnet run
```
Atau buka solution di Visual Studio dan tekan `F5`.

### 3. Kontrol
- **W / A / S / D**: Bergerak (Maju, Kiri, Mundur, Kanan).
- **Mouse**: Mengarahkan kamera/pandangan.
- **Menu**: Klik tombol "Open Menu" di pojok kiri bawah untuk membuka menu utama.

### 4. Menambahkan Model Sendiri
1. Siapkan file model 3D Anda dalam format `.glb` atau `.gltf`.
2. Copy file tersebut ke folder `wwwroot/assets/models/`.
3. Restart aplikasi atau buka menu "Choose Model", model Anda akan muncul di list.

---

## 🇺🇸 English Description

**ModelViewer3D** is a modern desktop application combining **WPF** and **Blazor Hybrid** to render interactive 3D models. It allows users to load `.glb` models and control them in a 3D environment (procedural village) with a *Third Person* view.

### Key Features
- **Hybrid Tech**: Uses WPF WebView2 to run Blazor locally.
- **Three.js Engine**: High-performance 3D rendering.
- **Dynamic Loading**: Loads `.glb` files from local `Assets` folder.
- **3rd Person Controls**: Standard WASD + Mouse movement.
- **Audio**: Ambient background music.

### How to Use
1. **Run**: Use `dotnet run`.
2. **Controls**: WASD to move, Mouse to look.
3. **Custom Models**: Place your `.glb` files in `wwwroot/assets/models/` and restart the app.

---

## 👨‍💻 Author
Created by **Jacky The Code Bender** from **Gravicode Studios**.

Jika aplikasi ini bermanfaat, jangan lupa traktiran pulsanya ya! Bisa kirim lewat [sini](https://studios.gravicode.com/products/budax). 😉
Happy Coding!
