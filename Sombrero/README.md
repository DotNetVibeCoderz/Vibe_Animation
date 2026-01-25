# Sombrero

## Description (English)
Sombrero is a graphical simulation application built using C# and Windows Forms. This project visualizes a 3D surface animation resembling a "Sombrero" wave or a spinning ripple. It calculates wave height using mathematical functions and projects points into 3D space with a perspective view, implementing a painter's algorithm for proper depth rendering (hidden line removal).

## Deskripsi (Bahasa Indonesia)
Sombrero adalah aplikasi simulasi grafis yang dibuat menggunakan C# dan Windows Forms. Proyek ini memvisualisasikan animasi permukaan 3D yang menyerupai gelombang "Sombrero" atau riak yang berputar. Aplikasi ini menghitung tinggi gelombang menggunakan fungsi matematika dan memproyeksikan titik-titik ke dalam ruang 3D dengan tampilan perspektif, serta mengimplementasikan algoritma painter's untuk perenderan kedalaman yang tepat (penghapusan garis tersembunyi).

---

## Features / Fitur

### English
*   **3D Wave Simulation**: Generates a dynamic ripple surface using cosine and exponential decay functions.
*   **Real-time Rendering**: Runs at 60 FPS using GDI+ for smooth animation.
*   **Dynamic Coloring**: Colors the mesh lines based on the wave amplitude (height).
*   **Depth Sorting**: Implements quad-based sorting to ensure distant parts of the mesh are drawn before closer parts (Painter's Algorithm).
*   **Spinning Animation**: The entire grid rotates around the vertical axis.

### Bahasa Indonesia
*   **Simulasi Gelombang 3D**: Menghasilkan permukaan riak dinamis menggunakan fungsi kosinus dan peluruhan eksponensial.
*   **Render Real-time**: Berjalan pada 60 FPS menggunakan GDI+ untuk animasi yang halus.
*   **Pewarnaan Dinamis**: Mewarnai garis jaring berdasarkan amplitudo (tinggi) gelombang.
*   **Pengurutan Kedalaman**: Mengimplementasikan pengurutan berbasis quad untuk memastikan bagian jaring yang jauh digambar sebelum bagian yang lebih dekat (Algoritma Pelukis).
*   **Animasi Berputar**: Seluruh grid berputar di sekitar sumbu vertikal (sumbu Z).

---

## Prerequisites / Prasyarat
*   Windows OS (Operating System / Sistem Operasi Windows).
*   .NET 9.0 SDK or Runtime.

## How to Run / Cara Menjalankan

### Using Visual Studio
1.  Open `Sombrero.csproj` in Visual Studio 2022.
2.  Press `F5` or click **Start**.

### Using Command Line (CLI)
1.  Open terminal/command prompt in the project folder.
2.  Run the command:
    ```bash
    dotnet run
    ```

---

## About / Tentang
This project was lovingly crafted by **Jacky the Code Bender**.
Designed by the awesome team at **Gravicode Studios**, led by the one and only **Kang Fadhil**.

If you enjoy the code, feel free to treat me to some 'pulsa' (credit):  
> [https://studios.gravicode.com/products/budax](https://studios.gravicode.com/products/budax) 😉

Happy Coding!
