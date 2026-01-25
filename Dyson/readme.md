# Dyson Ring Visualization

![Language](https://img.shields.io/badge/Language-C%23-blue) ![Platform](https://img.shields.io/badge/Platform-Windows%20(WinForms)-lightgrey) ![Framework](https://img.shields.io/badge/.NET-9.0-purple)

[Bahasa Indonesia](#visualisasi-cincin-dyson) | [English](#english-description)

---

## <a name="visualisasi-cincin-dyson"></a>Visualisasi Cincin Dyson (Bahasa Indonesia)

**Dyson** adalah aplikasi simulasi visual sederhana yang dibuat menggunakan C# dan Windows Forms. Proyek ini mendemonstrasikan bagaimana membuat mesin rendering 3D sederhana dari nol menggunakan GDI+ (`System.Drawing`) tanpa bantuan library grafis eksternal seperti OpenGL atau DirectX.

Aplikasi ini memvisualisasikan struktur megastruktur futuristik "Dyson Ring" yang berputar mengelilingi bintang pusat, lengkap dengan efek pencahayaan kota dan transparansi material.

### Fitur Utama
*   **3D Rendering Engine:** Mengimplementasikan transformasi matriks 3D (Rotasi, Proyeksi) secara manual menggunakan `System.Numerics`.
*   **Generasi Mesh Prosedural:** Membentuk segmen cincin melengkung secara matematis.
*   **Efek Visual:**
    *   Latar belakang bintang yang berkelap-kelip.
    *   Efek cahaya kota/stasiun pada permukaan cincin.
    *   Rendering semi-transparan untuk membedakan sisi depan dan belakang cincin.
    *   Matahari buatan di tengah dengan efek *glow*.
*   **Interaktif:** Pengguna dapat memutar sudut pandang kamera menggunakan mouse.

### Struktur Kode
*   `Form1.cs`: Mengatur *game loop*, input mouse, dan logika rendering utama (GDI+).
*   `DysonMath.cs`: Berisi logika matematika untuk perhitungan vektor, matriks, proyeksi 3D ke 2D, konfigurasi mesh, dan pewarnaan (HSL to RGB).

### Cara Menjalankan
1.  Pastikan Anda telah menginstal **.NET 9.0 SDK**.
2.  Buka terminal/command prompt di folder proyek.
3.  Jalankan perintah berikut:
    ```bash
    dotnet run
    ```
4.  Atau buka file `.csproj` menggunakan Visual Studio dan tekan *Start*.

### Kontrol
*   **Klik Kiri + Tahan & Geser Mouse:** Mengubah sudut pandang kamera (rotasi Pitch dan Yaw).

---

## <a name="english-description"></a>Dyson Ring Visualization (English)

**Dyson** is a visual simulation application built with C# and Windows Forms. This project demonstrates how to create a scratch-built 3D rendering engine using GDI+ (`System.Drawing`) without relying on external graphics libraries like OpenGL or DirectX.

This application visualizes a futuristic "Dyson Ring" megastructure rotating around a central star, complete with city lighting effects and material transparency.

### Key Features
*   **3D Rendering Engine:** Implements 3D matrix transformations (Rotation, Projection) manually using `System.Numerics`.
*   **Procedural Mesh Generation:** Mathematically generates curved ring segments.
*   **Visual Effects:**
    *   Twinkling starfield background.
    *   City/station lights scattered on the ring surface.
    *   Semi-transparent rendering to distinguish between the front and back faces of the ring.
    *   Central artificial sun with a glow effect.
*   **Interactive:** Users can rotate the camera view using mouse interaction.

### Code Structure
*   `Form1.cs`: Handles the game loop, mouse input, and main rendering logic (GDI+).
*   `DysonMath.cs`: Contains mathematical logic for vector calculations, matrix operations, 3D-to-2D projection, mesh configuration, and color utilities (HSL to RGB).

### How to Run
1.  Ensure you have **.NET 9.0 SDK** installed.
2.  Open a terminal/command prompt in the project folder.
3.  Run the following command:
    ```bash
    dotnet run
    ```
4.  Alternatively, open the `.csproj` file in Visual Studio and click *Start*.

### Controls
*   **Left Click + Hold & Drag Mouse:** Rotate the camera view (Pitch and Yaw).
