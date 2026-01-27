# P5Net - Library P5.js untuk C# Windows Forms

![P5Net](https://img.shields.io/badge/P5Net-v1.0-blue)
![.NET](https://img.shields.io/badge/.NET-Framework-purple)
![License](https://img.shields.io/badge/license-MIT-green)

## 📝 Deskripsi

P5Net adalah library untuk membuat animasi dan visualisasi interaktif di C# Windows Forms, terinspirasi dari P5.js. Library ini memudahkan pembuatan sketch kreatif dengan API yang familiar dan mudah dipahami.

## ✨ Fitur

- **P5Canvas**: Base class untuk membuat sketch seperti P5.js
- **Vector2D**: Class untuk operasi vektor 2D
- **Drawing Functions**: Ellipse, Circle, Rect, Line, Triangle, Point, Bezier
- **Transformation**: Translate, Rotate, Scale, PushMatrix, PopMatrix
- **Color Management**: Fill, Stroke, NoFill, NoStroke
- **Math Helpers**: Map, Lerp, Constrain, Dist, Random
- **Double Buffering**: Untuk animasi yang smooth tanpa flicker

## 🎨 Contoh Animasi

### 1. Orbit Control Animation
Simulasi planet yang mengorbit mengelilingi matahari dengan moon dan ring.

**Fitur:**
- Multiple planets dengan kecepatan orbit berbeda
- Moon yang mengorbit planet
- Ring planet
- Smooth animation

### 2. Kaleidoscope Animation
Pola simetris yang indah dan interaktif mengikuti pergerakan mouse.

**Fitur:**
- 12 simetri radial
- Warna rainbow yang dinamis
- Interactive dengan mouse
- Trail effect

### 3. Recursive Tree Animation
Pohon fractal yang tumbuh menggunakan algoritma rekursif.

**Fitur:**
- Fractal tree generation
- Angle adjustment dengan mouse
- Seasonal color animation (daun berubah warna)
- Branch thickness variation

### 4. Bezier Curve Animation
Kurva Bezier yang dinamis dengan control points yang bergerak.

**Fitur:**
- Multiple bezier curves dengan warna berbeda
- Animated control points
- Points traveling along curves
- Trail particles
- Background wave effect

### 5. Flocking Bird Animation (Boids)
Simulasi kawanan burung menggunakan Boids algorithm.

**Fitur:**
- 50 birds dengan AI flocking behavior
- Alignment, Cohesion, Separation rules
- Predator avoidance (mouse)
- Wing flapping animation
- Individual bird colors

## 🚀 Cara Menggunakan

### Menjalankan Program

1. Compile dan jalankan project
2. Pilih salah satu contoh animasi dari menu
3. Enjoy the animation! 🎉

### Membuat Animasi Sendiri

```csharp
using P5Net;

public class MyAnimation : P5Canvas
{
    public MyAnimation()
    {
        Width = 800;
        Height = 600;
        this.ClientSize = new System.Drawing.Size(Width, Height);
        this.Text = "My Animation";
    }
    
    public override void Setup()
    {
        // Inisialisasi (dipanggil sekali)
        Background(0);
    }
    
    public override void Draw()
    {
        // Loop animasi (dipanggil setiap frame)
        Background(0);
        
        Fill(255, 0, 0);
        Circle(Width / 2, Height / 2, 50);
    }
}
```

## 📚 API Reference

### Drawing Functions

- `Background(r, g, b)` - Set background color
- `Fill(r, g, b, a)` - Set fill color
- `Stroke(r, g, b, a)` - Set stroke color
- `NoFill()` - Disable fill
- `NoStroke()` - Disable stroke
- `StrokeWeight(weight)` - Set stroke width
- `Circle(x, y, diameter)` - Draw circle
- `Ellipse(x, y, w, h)` - Draw ellipse
- `Rect(x, y, w, h)` - Draw rectangle
- `Line(x1, y1, x2, y2)` - Draw line
- `Triangle(x1, y1, x2, y2, x3, y3)` - Draw triangle
- `Bezier(x1, y1, x2, y2, x3, y3, x4, y4)` - Draw bezier curve

### Transformation Functions

- `PushMatrix()` - Save current transformation
- `PopMatrix()` - Restore previous transformation
- `Translate(x, y)` - Move origin
- `Rotate(angle)` - Rotate (angle in radians)
- `Scale(s)` - Scale uniformly
- `Scale(x, y)` - Scale non-uniformly

### Math Functions

- `Map(value, start1, stop1, start2, stop2)` - Map value from one range to another
- `Lerp(start, stop, amt)` - Linear interpolation
- `Constrain(value, min, max)` - Constrain value
- `Dist(x1, y1, x2, y2)` - Calculate distance
- `Random(max)` - Random number 0 to max
- `Random(min, max)` - Random number min to max

### Properties

- `Width, Height` - Canvas dimensions
- `FrameCount` - Current frame number
- `MouseX, MouseY` - Current mouse position
- `PMMouseX, PMMouseY` - Previous mouse position
- `PI, TWO_PI, HALF_PI, QUARTER_PI` - Math constants

### Vector2D Class

```csharp
Vector2D v = new Vector2D(x, y);
v.Add(other);      // Addition
v.Sub(other);      // Subtraction
v.Mult(scalar);    // Multiplication
v.Div(scalar);     // Division
v.Mag();           // Magnitude
v.Normalize();     // Normalize
v.SetMag(mag);     // Set magnitude
v.Limit(max);      // Limit magnitude
v.Heading();       // Get angle
v.Dist(other);     // Distance to other vector
v.Dot(other);      // Dot product
```

## 🎯 Algoritma yang Digunakan

### Boids Algorithm (Flocking)
Algoritma Boids memiliki 3 aturan utama:
1. **Alignment**: Burung mengikuti arah rata-rata tetangganya
2. **Cohesion**: Burung bergerak menuju posisi rata-rata tetangganya
3. **Separation**: Burung menjaga jarak dari tetangganya

### Recursive Tree
Menggunakan rekursi untuk membuat cabang pohon:
```
function drawBranch(length):
    if length < 4: draw leaf and return
    draw line
    move to end of line
    rotate right and drawBranch(length * 0.67)
    rotate left and drawBranch(length * 0.67)
```

### Bezier Curve
Kurva Bezier cubic menggunakan 4 control points:
```
B(t) = (1-t)³P₀ + 3(1-t)²tP₁ + 3(1-t)t²P₂ + t³P₃
```

## 🎮 Kontrol Interaktif

- **Kaleidoscope**: Gerakkan mouse untuk mengubah pola
- **Recursive Tree**: Gerakkan mouse left-right untuk mengubah sudut cabang
- **Flocking Birds**: Gerakkan mouse untuk menakuti burung

## 🛠️ Requirements

- .NET Framework 4.7.2 atau lebih tinggi
- Windows Forms
- System.Drawing

## 👨‍💻 Dibuat Oleh

**Gravicode Studios**
- Dipimpin oleh Kang Fadhil
- Website: https://studios.gravicode.com

## 💰 Support

Jika kamu suka dengan project ini, boleh dong traktir pulsa! 😊
Kirim ke: https://studios.gravicode.com/products/budax

## 📄 License

MIT License - Feel free to use and modify!

## 🎓 Pembelajaran

Project ini cocok untuk:
- Belajar algoritma animasi
- Memahami transformasi 2D
- Implementasi Boids algorithm
- Recursive programming
- Bezier curves
- Game development basics

## 🔮 Future Improvements

- [ ] 3D rendering support
- [ ] Audio integration
- [ ] Particle system
- [ ] Physics engine
- [ ] More examples
- [ ] Performance optimization

---

**Selamat berkreasi dengan P5Net!** 🎨✨

Jangan lupa star repository ini jika bermanfaat! ⭐
