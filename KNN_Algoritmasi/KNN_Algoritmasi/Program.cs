using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KNN_Algoritmasi
{
    class Program
    {
        List<double> uzakliklar = new List<double>(); // girilen verinin tüm verilere sırayla uzaklıklarını tutar
        string[,] dataSet; // txt dosyasından okunan veri 2 boyutlu bu diziye aktarılır
        double[] degerler = new double[4]; // kullanıcıdan alınacak 4 değeri tutan dizi
        int k; // k değeri
        int TP = 1, FP = 1, FN = 1, TN = 1, P = 1, N = 1;
        string sonuc = "";
        int veriSayisi = 0; // veri sayısını tutar
        int topSet = 0, topVer = 0, topVir = 0;
        int irisSetosa = 0, irisVersicolor = 0, irisVirginica = 0; // hesaplamalar sonucu minimum uzaklıklardaki sınıfların sayılarını tutan değer

        static void Main(string[] args)
        {
            Program baslat = new Program();
            baslat.degerAl(); //degeraAl fonksiyonu çağrıldı
            baslat.Hesapla(); //hesapla fonksiyonu çağrıldı

            Console.ReadLine();
        }
        
        private double sensitivity()
        {
            return TP / (TP + FN);
        }

        private double specificity()
        {
            return TN / (TN + FP);
        }

        private double accuracy()
        {
            return (TP + TN) / (P + N);
        }

        private double fOlcumu()
        {
            return 2 * TP / (2 * TP + FP + FN);
        }

        private double errorRate()
        {
            return (FP + FN) / (P + N);
        }

        private void Hesapla()
        {
            double[] dataDegerler = new double[5]; // dataSet içerisindeki 4 veri değerini tutacak olan değişken
            double minimum = double.MaxValue; // minimum hesaplamak için oluşturulan değişken
            int belirtec = 0; // hangi sıradaki veri tipi minimum onu tutmak için

            // veri sayisi kadar döngü
            for (int i = 0; i < veriSayisi; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    dataDegerler[j] = Convert.ToDouble(dataSet[i, j]); //dataSet in ilk 4 değeri degerler dizisine atandı
                }
                uzakliklar.Add(double.Parse(oklidUzaklık(dataDegerler, degerler).ToString("0.0000")));
                // uzaklıklar listesine sırayla oklidUzaklik fonksiyonunun döndüğü değer "0.0000" formatında eklendi
            }
            // k kadar döngü
            for (int i = 0; i < k; i++)
            {
                // uzaklıklar listesindeki sayı kadar döngü
                for (int j = 0; j < uzakliklar.Count; j++)
                {
                    if (minimum >= uzakliklar[j])
                    {
                        minimum = uzakliklar[j];
                        belirtec = j; // kaçıncı sıradaysa işaretle (index)
                    }
                }
                uzakliklar.RemoveAt(belirtec); // minimum olanı listeden çıkart
                sinifBelirle(belirtec); // sınıfBelirle fonksiyonuyla minimum olan hangi sınıf bul 
                belirtec = 0; // bir sonraki adım için belirteci sıfırla
            }
            // hangi sınıf fazlaysa onu ekrana yazdır
            if (irisSetosa > irisVersicolor)
            {
                if (irisSetosa > irisVirginica)
                {
                    Console.WriteLine("Veri kümesinin sınıfı: Iris-setosa");
                    sonuc = "Iris-setosa";
                }
                else
                {
                    Console.WriteLine("Veri kümesinin sınıfı: Iris-Virginica");
                    sonuc = "Iris-virginica";
                }
            }
            else
            {
                if (irisVirginica < irisVersicolor)
                {
                    Console.WriteLine("Veri kümesinin sınıfı: Iris-versicolor");
                    sonuc = "Iris-versicolor";
                }
                else
                {
                    Console.WriteLine("Veri kümesinin sınıfı: Iris-verginica");
                    sonuc = "Iris-verginica";
                }
            }

            Console.WriteLine("\nDuyarlılık oranı: " + sensitivity().ToString("0.00000"));
            Console.WriteLine("Kesinlik oranı: " + specificity().ToString("0.00000"));
            Console.WriteLine("Dogruluk oranı: " + accuracy().ToString("0.00000"));
            Console.WriteLine("Hata oranı: " + errorRate().ToString("0.00000"));
            Console.WriteLine("F-Ölçümü: " + fOlcumu().ToString("0.00000"));
        }
        // sinifi bulmak için sinif sayılarını toplayan fonksiyon
        private void sinifBelirle(int index)
        {
            if (dataSet[index, 4] == "Iris-setosa")
            {
                irisVersicolor++;
            }
            else if (dataSet[index, 4] == "Iris-versicolor")
            {
                irisVersicolor++;
            }
            else
            {
                irisVirginica++;
            }
        }
        // verilen noktalar arası uzaklık hesaplama fonksiyonu
        private double oklidUzaklık(double[] dataDegerler, double[] verilenDegerler)
        {
            double toplam = 0.0;
            // 4 data değeri ile 4 kullanıcı değeri arasındaki uzaklık toplanıyor
            for (int i = 0; i < 4; i++)
            {
                toplam += Math.Pow(dataDegerler[i] - verilenDegerler[i], 2);
            }
            return Math.Sqrt(toplam); // toplamın karekökü uzaklığı verir
        }
        // kullanıcıdan değer alan fonksiyon
        private void degerAl()
        {
            Console.Write("k değerini giriniz: ");
            k = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine("Sınıfı bulunacak dört değeri giriniz:");

            for (int i = 0; i < 4; i++)
            {
                Console.Write((i + 1) + ". değer: ");
                degerler[i] = Convert.ToDouble(Console.ReadLine()); //alınan değerler double veri tipine dönüştürülüp degerler dizisine atıldı
            }
            // datasetOku fonksiyonu çağrıldı
            datasetOku();
        }
        // text dosyasından verileri dataSet dizisine aktaran fonksiyon
        private void datasetOku()
        {
            string dosyaYolu = @"C:\iris.data.txt"; // dataset dosyasının yolunu tutan değişken
            string[] satirlar = File.ReadAllLines(dosyaYolu); // metin dosyasının her satırını tutan string dizi(satirlar[0] ilk satırdaki tüm karakterleri tutar)
            int index = 0; // satırdan değerleri okuyabilmemiz için başlangıç konumunu tutan değişken
            veriSayisi = satirlar.Length; // satır sayısı kadar veri kümemiz var
            dataSet = new string[veriSayisi, 5]; // dataset satır ve sutun sayısı ayarlandı
            // ilk döngü satır sayısı kadar döner, ikinci döngü sütun sayısı kadar döner
            for (int i = 0; i < satirlar.Length; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    if (j == 4) //5. sutunsa gerçekleştir
                    {
                        dataSet[i, j] = satirlar[i].Substring(index); // iris datanın sonundaki sınıf isimlerini alır. Sadece index belirttiğimiz için indexten sonra tüm karakterleri alır
                        index = 0; // index sıfırlayıp continue yazarak bir alttakı kod bloğunu atlamasını sağlıyoruz
                        continue;
                    }
                    dataSet[i, j] = satirlar[i].Substring(index, 3); // index başlangıç karakteri, 3 indexten sonra kaç karakter alınsın anlamına gelir
                    index += 4; // index 4 attırıldı çünkü iris data da alacağımız değerler 3 haneli
                    //örneğin ilk satır 5,1/3,5/1,4/0,2/Iris-setosa
                    //5,1 okuduktan sonra index i 4 attırmalıyız ki bir sonraki satırda / tan hemen sonraki karakteri okusun
                    //4 veri  de 3 karakter olduğundan ve her değerin sonunda 1 karakter / olduğundan 4 birim index arttırılır
                }
            }
        }
    }
}
