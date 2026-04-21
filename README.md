# Boom Bang - Case Study

**Boom Bang**, Good Job Games teknik case çalışması için hazırlanmış,bir blast bulmaca oyunu demosudur.

## 🚀 Proje Özeti
Oyun, aynı renkteki bitişik bloklara tıklayarak tahtayı temizleme (tap-to-blast) mekaniği üzerine kuruludur. Geliştirme sürecinde temiz kod mimarisi, akıcı oynanış ve performans optimizasyonu ön planda tutulmuştur. Oyunun müzikleri de bu projeye özel olarak sıfırdan üretilmiştir.

## 🎮 Temel Özellikler
* **Blast Mekaniği:** Aynı renkteki iki veya daha fazla bitişik bloğun oluşturduğu gruplar, tek bir dokunuşla patlatılabilir. Grup ne kadar büyükse efekt o kadar etkili olur.
* **Dinamik Grid (Izgara) Yönetimi:** Patlayan blokların boşluğu yerçekimi etkisiyle doldurulur ve üstten yeni bloklar gelerek tahta otomatik olarak güncellenir.
* **Gelişmiş Grup Tespiti:** Bitişik aynı renkli tüm blokları (2 ve üzeri) anında ve performanslı bir şekilde tespit eden recursive/iterative arama algoritmaları kullanılmıştır.
* **Özgün Ses ve Müzik:** Oyun içi tüm ses efektleri ve atmosferik müzikler FL Studio kullanılarak bu projeye özel tasarlanmıştır.

## 🛠️ Teknik Detaylar
* **Oyun Motoru:** Unity
* **Programlama Dili:** C#
* **Ses & Müzik Prodüksiyonu:** FL Studio
* **Performans:** Bellek yönetimini optimize etmek ve FPS düşüşlerini engellemek için **Object Pooling** (Nesne Havuzu) deseni kullanılmıştır.

## ⚙️ Nasıl Çalıştırılır?
1. Repoyu bilgisayarınıza klonlayın: 
   `git clone https://github.com/shug1n/BoomBlast.git`
2. Projeyi Unity Hub üzerinden açın.
3. `Scenes` klasöründeki `SampleScene` sahnesini açın.
4. Unity Editor'de **Play** butonuna basarak oynayabilirsiniz.
