using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Harjoitustyo_kirjakauppa
{
    class Program
    {
        [Serializable]
        struct Kirja
        {
            public Kirja(string title, string author, int vuosi, string genre, double hinta)
            {
                Title = title;
                Author = author;
                Vuosi = vuosi;
                Genre = genre;
                Hinta = hinta;
            }
            public override string ToString()
            {
                return Title + ";" + Author + ";" + Vuosi + ";" + Genre + ";" + Hinta;
            }
            public string Title;
            public string Author;
            public int Vuosi;
            public string Genre;
            public double Hinta;
        }

        static void Main(string[] args)
        {
            //Alkualustukset tiedostonimille
            string basketfile = "ostettu.txt";
            string walletfile = "lompakko.bin";
            string inventory = "inventory.txt";
            int valinta = 1;
            double lompakko = 100;
            //kansioiden ja polkujen alustukset
            string folderName = "Tarmon kauppa";
            string mydocpath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            mydocpath = Path.Combine(mydocpath, folderName);  //Luo tarmon kaupppa kansion mydocuments kansioon
            string korisijainti = Path.Combine(mydocpath, basketfile);
            string lompakkosijainti = Path.Combine(mydocpath, walletfile);
            string inventorysijainti = Path.Combine(mydocpath, inventory);
            DateTime LastVisit = File.GetLastAccessTime(lompakkosijainti);
            
            if (File.Exists(korisijainti)) //Tutkii ostoskorin olemassaolon
            {
                Console.WriteLine("* Ostoskori löydetty *");
            }
            else
            {
                Console.WriteLine("* Ostoskori luotu *");
                CreateBasket(korisijainti);
            }

            if (File.Exists(lompakkosijainti)) //Tutkii lompakon olemassaolon
            {
                Console.WriteLine("* Lompakko löydetty *");
            }
            else
            {
                Console.WriteLine("* Lompakko luotu *");
                CreateWalletInFile(lompakkosijainti);
            }
            List<Kirja> kirjat = CreateInventory(inventorysijainti);
            Console.WriteLine("Asioit meillä viimeksi {0}", LastVisit);

            while (valinta != 0) //Toistaa päämenua kunnes käyttäjä tahtoo poistua
            {

                MainMenu();
                string format = "* Väärä syöte, anna numero uudestaan *";
                string overflow = "* Liian suuri valinta, anna numero uudestaan *";
                string except = null;
                string romaani = "Romaani";
                string dekkari = "Dekkari";
                string lasten = "Lasten";
                MenuValinta(ref valinta, format, overflow, ref except);
                switch (valinta)
                {                    
                    case 1: //Kirjojen selailu
                        int kategoria = 1;
                        while (kategoria != 0)
                        {
                            Menu();
                            MenuValinta(ref kategoria, format, overflow, ref except);

                            Console.Clear();

                            switch (kategoria)
                            {
                                case 1:
                                    ListBooks(kirjat, romaani);
                                    PressEnter();
                                    break;

                                case 2:
                                    ListBooks(kirjat, dekkari);
                                    PressEnter();
                                    break;

                                case 3:
                                    ListBooks(kirjat, lasten);
                                    PressEnter();
                                    break;

                                case 4:
                                    ListAllBooks(kirjat);
                                    PressEnter();
                                    break;
                                case 0:
                                    Console.Clear();
                                    break;
                                case 404:
                                    continue;
                                default:
                                    Console.WriteLine("Et antanut valintaa, yritä uudestaan.");
                                    break;
                            }
                        }
                        break;

                    case 2: //Kirjojen osto
                        int ostovalinta = 1;
                        while (ostovalinta != 0)
                        {
                            {
                                Menu();
                                MenuValinta(ref ostovalinta, format, overflow, ref except);
                                int i = 0;
                                switch (ostovalinta)
                                {

                                    case 1: 
                                        ListBooks(kirjat, romaani);
                                        Console.WriteLine("Kirjoita kirjan nimi minkä tahdot ostaa,\nTai 'peruuta' peruutaaksesi oston");
                                        string osto = Console.ReadLine();
                                        Console.Clear();

                                        if (osto == "peruuta")
                                        {
                                            Console.WriteLine("Peruutit oston");
                                            break;
                                        }
                                        for (i = 0; i < kirjat.Count; i++)
                                        {
                                            if (kirjat[i].Title == osto)
                                            {
                                                if (kirjat[i].Hinta < lompakko)
                                                {
                                                    Console.Clear();
                                                    Console.WriteLine(" + " + osto + " ostettu. Hinta: " + kirjat[i].Hinta + "e");
                                                    double vahennys = kirjat[i].Hinta;
                                                    lompakko -= vahennys;
                                                    Console.WriteLine(" - Rahaa jäljellä: " + lompakko + "e");
                                                    StreamWriter kirjoita = null;
                                                    try
                                                    {
                                                        kirjoita = new StreamWriter(korisijainti, true);
                                                        kirjoita.WriteLine(kirjat[i]);                                                        
                                                        kirjat.Remove(kirjat[i]);
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        Console.WriteLine("Omiin tiedostoihin kirjoitus epäonnistui" + ex.Message);
                                                    }
                                                    finally
                                                    {                                                        
                                                        kirjoita.Close();
                                                        WriteInventory(kirjat, inventorysijainti);
                                                        WriteWallet(lompakko, lompakkosijainti);
                                                    }

                                                    break;
                                                }
                                                else
                                                {
                                                    Console.Clear();
                                                    Console.WriteLine("Sinulla ei ole valitettavasti tarpeeksi rahaa.");
                                                    Console.WriteLine("Lompakkosi saldo on {0}e ja kirjasi maksaa {1}e", lompakko, kirjat[i].Hinta);
                                                    PressEnter();
                                                    break;
                                                }
                                            }
                                            else if (kirjat.Count == i)
                                            {
                                                Console.WriteLine("Hakemaasi kirjaa ei löytnyt.");
                                                break;
                                            }
                                            else
                                            {
                                                continue;
                                            }
                                        }
                                        if (kirjat.Count == i)
                                        {
                                            Console.WriteLine("Hakemaasi kirjaa ei löytnyt.");
                                            break;
                                        }

                                        break;

                                    case 2:
                                        ListBooks(kirjat, dekkari);

                                        Console.WriteLine("Kirjoita kirjan nimi minkä tahdot ostaa,\nTai 'peruuta' peruutaaksesi oston");
                                        osto = Console.ReadLine();
                                        i = 0;
                                        Console.Clear();

                                        if (osto == "peruuta")
                                        {
                                            Console.WriteLine("Peruutit oston");
                                            break;
                                        }
                                        for (i = 0; i < kirjat.Count; i++)
                                        {
                                            if (kirjat[i].Title == osto)
                                            {
                                                if (kirjat[i].Hinta < lompakko)
                                                {
                                                    Console.Clear();
                                                    Console.WriteLine(" + " + osto + " ostettu. Hinta: " + kirjat[i].Hinta + "e");
                                                    double vahennys = kirjat[i].Hinta;
                                                    lompakko -= vahennys;
                                                    Console.WriteLine(" - Rahaa jäljellä: " + lompakko + "e");
                                                    StreamWriter kirjoita = null;
                                                    try
                                                    {
                                                        kirjoita = new StreamWriter(korisijainti, true);
                                                        kirjoita.WriteLine(kirjat[i]);
                                                        kirjat.Remove(kirjat[i]);
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        Console.WriteLine("Omiin tiedostoihin kirjoitus epäonnistui" + ex.Message);
                                                    }
                                                    finally
                                                    {
                                                        kirjoita.Close();
                                                        WriteInventory(kirjat, inventorysijainti);
                                                        WriteWallet(lompakko, lompakkosijainti);
                                                    }
                                                    break;
                                                }
                                                else
                                                {
                                                    Console.Clear();
                                                    Console.WriteLine("Sinulla ei ole valitettavasti tarpeeksi rahaa.");
                                                    Console.WriteLine("Lompakkosi saldo on {0}e ja kirjasi maksaa {1}e", lompakko, kirjat[i].Hinta);
                                                    PressEnter();
                                                    break;
                                                }

                                            }
                                            else if (kirjat.Count == i)
                                            {
                                                Console.WriteLine("Hakemaasi kirjaa ei löytnyt.");
                                                break;
                                            }
                                            else
                                            {
                                                continue;
                                            }
                                        }
                                        if (kirjat.Count == i)
                                        {
                                            Console.WriteLine("Hakemaasi kirjaa ei löytnyt.");
                                            break;
                                        }
                                        break;

                                    case 3:
                                        ListBooks(kirjat, lasten);
                                        Console.WriteLine("Kirjoita kirjan nimi minkä tahdot ostaa,\nTai 'peruuta' peruutaaksesi oston");
                                        osto = Console.ReadLine();
                                        i = 0;
                                        Console.Clear();

                                        if (osto == "peruuta")
                                        {
                                            Console.WriteLine("Peruutit oston");
                                            break;
                                        }
                                        for (i = 0; i < kirjat.Count; i++)
                                        {
                                            if (kirjat[i].Title == osto)
                                            {
                                                if (kirjat[i].Hinta < lompakko)
                                                {
                                                    Console.Clear();
                                                    Console.WriteLine(" + " + osto + " ostettu. Hinta: " + kirjat[i].Hinta + "e");
                                                    double vahennys = kirjat[i].Hinta;
                                                    lompakko -= vahennys;
                                                    Console.WriteLine(" - Rahaa jäljellä: " + lompakko + "e");
                                                    StreamWriter kirjoita = null;
                                                    try
                                                    {
                                                        kirjoita = new StreamWriter(korisijainti, true);
                                                        kirjoita.WriteLine(kirjat[i]);
                                                        kirjat.Remove(kirjat[i]);
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        Console.WriteLine("Omiin tiedostoihin kirjoitus epäonnistui" + ex.Message);
                                                    }
                                                    finally
                                                    {
                                                        kirjoita.Close();
                                                        WriteInventory(kirjat, inventorysijainti);
                                                        WriteWallet(lompakko, lompakkosijainti);
                                                    }

                                                    break;
                                                }
                                                else
                                                {
                                                    Console.Clear();
                                                    Console.WriteLine("Sinulla ei ole valitettavasti tarpeeksi rahaa.");
                                                    Console.WriteLine("Lompakkosi saldo on {0}e ja kirjasi maksaa {1}e", lompakko, kirjat[i].Hinta);
                                                    PressEnter();
                                                    break;
                                                }

                                            }
                                            else if (kirjat.Count == i)
                                            {
                                                Console.WriteLine("Hakemaasi kirjaa ei löytnyt.");
                                                break;
                                            }
                                            else
                                            {
                                                continue;
                                            }
                                        }
                                        if (kirjat.Count == i)
                                        {
                                            Console.WriteLine("Hakemaasi kirjaa ei löytnyt.");
                                            break;
                                        }
                                        break;
                                    case 4:
                                        ListAllBooks(kirjat);
                                        Console.WriteLine("Kirjoita kirjan nimi minkä tahdot ostaa,\nTai 'peruuta' peruutaaksesi oston");
                                        osto = Console.ReadLine();
                                        i = 0;
                                        Console.Clear();

                                        if (osto == "peruuta")
                                        {
                                            Console.WriteLine("Peruutit oston");
                                            break;
                                        }
                                        for (i = 0; i < kirjat.Count; i++)
                                        {
                                            if (kirjat[i].Title == osto)
                                            {
                                                if (kirjat[i].Hinta < lompakko)
                                                {
                                                    Console.Clear();
                                                    Console.WriteLine(" + " + osto + " ostettu. Hinta: " + kirjat[i].Hinta + "e");
                                                    double vahennys = kirjat[i].Hinta;
                                                    lompakko -= vahennys;
                                                    Console.WriteLine(" - Rahaa jäljellä: " + lompakko + "e");
                                                    StreamWriter kirjoita = null;
                                                    try
                                                    {
                                                        kirjoita = new StreamWriter(korisijainti, true);
                                                        kirjoita.WriteLine(kirjat[i]);
                                                        kirjat.Remove(kirjat[i]);
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        Console.WriteLine("Omiin tiedostoihin kirjoitus epäonnistui" + ex.Message);
                                                    }
                                                    finally
                                                    {
                                                        kirjoita.Close();
                                                        WriteInventory(kirjat, inventorysijainti);
                                                        WriteWallet(lompakko, lompakkosijainti);
                                                    }
                                                    break;
                                                }
                                                else
                                                {
                                                    Console.Clear();
                                                    Console.WriteLine("Sinulla ei ole valitettavasti tarpeeksi rahaa.");
                                                    Console.WriteLine("Lompakkosi saldo on {0}e ja kirjasi maksaa {1}e", lompakko, kirjat[i].Hinta);
                                                    PressEnter();
                                                    break;
                                                }
                                            }
                                            else if (kirjat.Count == i)
                                            {
                                                Console.WriteLine("Hakemaasi kirjaa ei löytnyt.");
                                                continue;
                                            }
                                            else
                                            {
                                                continue;
                                            }
                                        }
                                        if (kirjat.Count == i)
                                        {
                                            Console.WriteLine("Hakemaasi kirjaa ei löytnyt.");
                                            break;
                                        }
                                        break;
                                    case 404:
                                        Console.WriteLine("Virheellinen syöte, anna valinta uudestaan");
                                        continue;
                                }
                            }
                            break;
                        }
                        break;
                    
                    case 3: //Kirjan myynti
                        bool finished = false;
                        if (new FileInfo(korisijainti).Length < 4) //Tarkistaa onko ostoskori tyhjä (Jos alle 4 byte)
                        {
                            Console.WriteLine("* Sinulla ei ole vielä kirjoja! *");
                        }
                        else
                        {
                            while (finished == false) // Pitää myynnin käynnissä kunnes myynti on valmis
                            {
                                Console.WriteLine("* * * * * * * * * * * * * * * * * * * * ");
                                Console.WriteLine("* Kauppa ostaa kirjanne 80% arvolla! ");
                                Console.WriteLine("* Minkä kirjan haluat myydä?");
                                ReadFromMyDocs3(korisijainti); // Luettelee mitä on ostettu
                                Console.WriteLine("Kirjoita myytävän kirjan nimi tai 'peruuta' peruttaaksesi myynnin:");
                                string myyntikirja = Console.ReadLine(); //Käyttäjä kirjoittaa nimen minkä tahtoo myydä

                                StreamReader lue = null;
                                try //kirjoitetaan käyttäjän kirjat arrayhin, katsotaan onko haettu kirja ostoskorissa
                                {
                                    lue = new StreamReader(korisijainti);
                                    while (!lue.EndOfStream) //Luetaan kirja käyttäjän tiedostosta arrayhin
                                    {
                                        string kirja = lue.ReadLine();
                                        string[] temp = kirja.Split(';');
                                        int.TryParse(temp[2], out int tempint);
                                        double.TryParse(temp[4], out double tempdouble);
                                        if (myyntikirja == temp[0]) //Mikäli kirja löytyy, luodaan kirja kauppaan
                                        {
                                            kirjat.Add(new Kirja(temp[0], temp[1], tempint, temp[3], tempdouble));
                                        }
                                        else
                                        {
                                            continue;
                                        }
                                    }
                                }
                                catch (FileNotFoundException ex)
                                {
                                    Console.WriteLine("Tiedostoa ei löytynyt! " + ex.Message);
                                }
                                finally
                                {
                                    lue.Close();
                                }
                                if (myyntikirja == "peruuta")
                                {
                                    finished = true;
                                    Console.Clear();
                                    Console.WriteLine("* Peruutit oston *");
                                    break;
                                }
                                for (int i = 0; i != kirjat.Count; i++) //Etsii luodun kirjan kaupan kirjastosta
                                {
                                    if (kirjat[i].Title == myyntikirja) //vertaa kirjoja kunnes löytää oikean ja toteuttaa kauppaan myynnin
                                    {
                                        double valitys = 0.2;
                                        double kauppahinta = (kirjat[i].Hinta - kirjat[i].Hinta * valitys);
                                        Console.WriteLine("Kirja {0} myyty, hintaan {1}e. ", kirjat[i].Title, kauppahinta);
                                        PressEnter();
                                        StreamReader luepois = null;
                                        string[] temparray = null;
                                        try
                                        {
                                            luepois = new StreamReader(korisijainti);
                                            int booksnmb = File.ReadAllLines(korisijainti).Length;
                                            temparray = new string[booksnmb]; //alustaa uuden stringi stackin
                                            string temp; //temp string lukemista varten

                                            for (int x = 0; !luepois.EndOfStream; x++) //lukee arrayhin kirjat mitä on ostanut
                                            {
                                                temp = luepois.ReadLine();
                                                temparray[x] = temp;
                                            }
                                        }
                                        catch (Exception)
                                        {
                                            Console.WriteLine("Tiedostoihin kirjoitus epäonnistui!");
                                        }
                                        finally
                                        {
                                            luepois.Close();
                                        }
                                        int k = 0;
                                        string myyntikirjaarrayhin = kirjat[i].ToString(); // Kertoo mikä kirja myydään

                                        foreach (string book in temparray) //Loop kirjat läpi arraystä 
                                        {
                                            if (temparray[k] == myyntikirjaarrayhin) //Kunnes myytävä kirja
                                            {
                                                string stringRemoval = temparray[k]; //Alustetaan kirja poistettavaksi
                                                temparray = temparray.Where(val => val != stringRemoval).ToArray(); //String poisto
                                                StreamWriter kirjoita = null;
                                                try
                                                {
                                                    kirjoita = new StreamWriter(korisijainti); //Aukaistaan kirjoitusyhteys
                                                    foreach (string line in temparray) //Kirjoitetaan koko array takaisin tiedostoon vanhan päälle
                                                    {
                                                        kirjoita.WriteLine(line);
                                                    }
                                                }
                                                catch (Exception)
                                                {
                                                    Console.WriteLine("Tiedostoihin kirjoitus epäonnistui!");
                                                }
                                                finally
                                                {
                                                    kirjoita.Close(); 
                                                }
                                                WriteInventory(kirjat, inventorysijainti);
                                                finished = true;
                                            }
                                            else
                                            {
                                                k++; //Siirtyy aina seuraavaan arrayn jäseneen
                                                continue;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (i == kirjat.Count - 1)
                                        {
                                            Console.WriteLine();
                                            Console.WriteLine("* Myytävää kirjaa ei löytynyt! *");
                                            PressEnter();
                                        }
                                        continue;
                                    }
                                    break;
                                }
                            }
                        }
                        break;

                    case 4: //Lompakon sisältö
                        ReadWallet(lompakkosijainti);
                        Console.Clear();
                        break;               
                        
                    case 5: //Ostoskorin selailu

                        if (new FileInfo(korisijainti).Length < 4) //Tarkistaa onko ostoskori tyhjä (Jos alle 4 byte)
                        {
                            Console.WriteLine("* Sinulla ei ole vielä kirjoja! *");
                        }
                        else
                        {
                            Console.WriteLine("Ostamasi kirjat: ");
                            ReadFromMyDocs3(korisijainti);

                            Console.WriteLine("* Paina mitä tahansa näppäintä jatkaaksesi. \n" +
                             "Jos haluat tyhjentää ostoskorin, kirjoita 'tyhjennä' *");

                            string vastaus = Console.ReadLine();
                            if (vastaus == "tyhjennä")
                            {
                                try
                                {
                                    File.WriteAllText(korisijainti, string.Empty); //Tyhjennetään ostoskori
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine("Tyhjennys epäonnistui!" + ex.Message);
                                    break;
                                }
                                finally
                                {
                                    Console.Clear();
                                    Console.WriteLine("* Ostoskori tyhjennetty *");
                                }
                            }
                            else
                            {
                                Console.Clear();
                                break;
                            }
                        }
                        break;

                    case 6: //Antaa lisää rahaa arvalla
                        Random rnd = new Random();
                        Console.WriteLine("* * * * * * * * * * * ");
                        Console.WriteLine("* Pyydetään apurahaa valtiolta. \n" +
                         "* Paina mitä tahansa näppäintä lähettääksesi hakemuksen. ");
                        Console.ReadKey();
                        string uusinta = null;
                        while (uusinta != "ei")
                        {
                            int arpa = rnd.Next(0, 100);
                            if (arpa <= 25)
                            {
                                Console.WriteLine("* Valtio myönti sinulle apurahan, onneksi olkoon!\n* 100e LISÄTTY LOMPAKKOOSI *");
                                lompakko += 100;
                                uusinta = "ei";
                                Console.ReadKey();
                                WriteWallet(lompakko, lompakkosijainti);
                                Console.Clear();
                                break;
                            }
                            else
                            {
                                Console.WriteLine("* Valitettavasti unohdit liitteen hakemuksestasi.");
                                Console.WriteLine("* Kirjoita 'kyllä' tehdäksesi uusintapyynnön ");
                                uusinta = Console.ReadLine();
                                if (uusinta != "kyllä")
                                {
                                    Console.Clear();
                                    Console.WriteLine("* Kiitos asioinnistasi tukirahakeskuksessa! *");
                                    break;
                                }
                            }
                        }
                        break;
                   
                    case 7:
                        CreateBooks(inventorysijainti); //Luo kaupan sisällön uudestaan
                        Console.WriteLine("* Kaupan sisältö luotu uudestaan* ");
                        break;

                    case 404: //Virheitä varten
                        Console.WriteLine(except);
                        break;
                    
                    case 0: //QUIT 
                        PrintLogo();
                        Console.WriteLine("Kiitos käynnistä ja tervetuloa uudelleen!");
                        break;

                    default:
                        Console.WriteLine("Et antanut valintaa!");
                        break;
                }

            }
        }

        private static void PressEnter()
        {
            Console.WriteLine("Paina mitä tahansa näppäintä jatkaaksesi");
            Console.ReadKey();
            Console.Clear();
        }

        private static void MenuValinta(ref int valinta, string format, string overflow, ref string except)
        {
            try //Estää enterpainalluksen virheen sekä int arvon ylityksen
            {
                valinta = Convert.ToInt32(Console.ReadLine());
            }
            catch (Exception ex)
            {
                if (ex is FormatException)
                {
                    except = format;
                    valinta = 404;
                }
                else if (ex is OverflowException)
                {
                    except = overflow;
                    valinta = 404;
                }
                else
                {
                    Console.WriteLine("* Tapahtui virhe, anna numero uudestaan");
                }
            }
            finally
            {
                Console.Clear();

            }
        }

        private static List<Kirja> CreateInventory(string inventorysijainti)
        {
            List<Kirja> kirjat = null;
            bool librarystate = File.Exists(inventorysijainti);
            if (librarystate == true)
            {
                int filesize = File.ReadAllText(inventorysijainti).Length;
                if (filesize < 4) //jos tiedosto jostain syystä tyhjä
                {
                    kirjat = CreateBooks(inventorysijainti); //Luodaan uusiksi kirjasto
                }
                else //Jos tiedosto olemassa, mutta ei tyhjä
                {

                    StreamReader luepois = null;
                    try
                    {
                        kirjat = new List<Kirja>();
                        luepois = new StreamReader(inventorysijainti);
                        for (int i = 0; !luepois.EndOfStream; i++)
                        {
                            string kirja = luepois.ReadLine();
                            string[] temp = kirja.Split(';');
                            int.TryParse(temp[2], out int tempint);
                            double.TryParse(temp[4], out double tempdouble);
                            kirjat.Add(new Kirja(temp[0], temp[1], tempint, temp[3], tempdouble));
                        }
                        Console.WriteLine("* Inventaario haettu *");

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Kirjaston luonti epäonnistui! " + ex.Message);
                    }
                    finally
                    {
                        luepois.Close();
                        WriteInventory(kirjat, inventorysijainti);
                    }
                }
            }
            else //Jos ei olemassa
            {
                kirjat = CreateBooks(inventorysijainti);
            }
            return kirjat;
        }

        private static List<Kirja> CreateBooks(string inventorysijainti) //Luo kirjaston alkuperäisen sisällön
        {
            List<Kirja> kirjat = new List<Kirja>

    {
     new Kirja("Bolla", "Pajtim Statovci", 1998, "Dekkari", 19.90),
     new Kirja("Veitsi", "Jo Nesbo", 2019, "Dekkari", 24.95),
     new Kirja("Kremlin nyrkki", "Ilkka Remes", 2019, "Dekkari", 24.95),
     new Kirja("Karhuryhmä", "Harri Gustafsberg & Heidi Holmavuo", 2017, "Dekkari", 27.95),
     new Kirja("Valapatto", "Leena Lehtolainen", 2019, "Dekkari", 27.15),
     new Kirja("Ensimmäinen nainen", "Johanna Venho", 2019, "Romaani", 27.95),
     new Kirja("Koirapuisto", "Sofi Oksanen", 2019, "Romaani", 28.95),
     new Kirja("Mielensäpahoittaja", "Tuomas Kyrö", 2016, "Romaani", 12.50),
     new Kirja("Kemisti", "Stephenie Meyer", 2019, "Romaani", 2.95),
     new Kirja("Koiramäen lapset", "Mauri Kunnas", 2016, "Romaani", 17.95),
     new Kirja("Tohtori toonika", "Pasi Heikkilä & Vellu Halkosalmi", 2019, "Lasten", 34.95),
     new Kirja("Risto Räppääjä", "Sinikka Nopola & Tiina Nopola", 2019, "Lasten", 17.95),
     new Kirja("Muumipappa ja meri", "Tove Jansson", 1993, "Lasten", 19.90),
     new Kirja("Ella ja kaverit", "Timo Parvela", 2019, "Lasten", 17.95),
     new Kirja("Tatu ja Patu", "Aino Havukainen & Sami Toivonen", 2018, "Lasten", 21.95)
    };
            WriteInventory(kirjat, inventorysijainti);
            Console.WriteLine("* Inventaario luotu *");
            return kirjat;
        }

        private static void WriteInventory(List<Kirja> kirjat, string inventorysijainti) //Kirjoittaa kirjat tiedostoon
        {
            StreamWriter kirjInv = null;
            try
            {
                kirjInv = new StreamWriter(inventorysijainti);

                foreach (Kirja book in kirjat)
                {
                    kirjInv.WriteLine(book);

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to write in mydocs" + ex.Message);
            }
            finally
            {
                kirjInv.Close();
            }
        }

        private static void ListAllBooks(List<Kirja> kirjat) //Metodi listaa kaikki kirjat
        {
            Console.WriteLine("* Tässä koko valikoimamme:");
            Console.WriteLine("* Nimi, Kirjoittaja, Vuosi");
            Console.WriteLine("* Hinta (e)");
            Console.WriteLine("-------------------------------------");
            foreach (Kirja kirja in kirjat)
            {
                {
                    Console.WriteLine(kirja.Title + ", " + kirja.Author + ", " + kirja.Vuosi);
                    Console.WriteLine(kirja.Hinta + "e");
                    Console.WriteLine("-------------------------------------");
                }
            }
        }

        private static void ListBooks(List<Kirja> kirjat, string kirjaluokka) //Listaa tietyn kategorian kirjat
        {
            Console.WriteLine(kirjaluokka);
            Console.WriteLine("* Tässä valikoimamme:");
            Console.WriteLine("* Nimi, Kirjoittaja, Vuosi");
            Console.WriteLine("* Hinta (e)");
            Console.WriteLine("-------------------------------------");


            for (int i = 0; i < kirjat.Count; i++)
            {
                if (kirjat[i].Genre == kirjaluokka)
                {
                    Console.WriteLine(kirjat[i].Title + ", " + kirjat[i].Author + ", " + kirjat[i].Vuosi);
                    Console.WriteLine(kirjat[i].Hinta + "e");
                    Console.WriteLine("-------------------------------------");
                }

            }

        }

        private static void CreateWalletInFile(string lompakkosijainti) //Luo lompakon tiedostoihin jos lompakkoa ei löydy
        {
            double lompakko = 100;
            BinaryWriter kirjoita = null;
            try
            {
                kirjoita = new BinaryWriter(File.Open(lompakkosijainti, FileMode.Create));
                kirjoita.Write(lompakko);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lompakon luonti epäonnistui" + ex.Message);
            }
            finally
            {
                kirjoita.Close();
            }

        }
        private static void WriteWallet(double lompakko, string lompakkosijainti) //Kirjoittaa ostotapahtumat lompakkoon
        {
            BinaryWriter kirjoita2 = null;
            try
            {
                kirjoita2 = new BinaryWriter(File.Open(lompakkosijainti, FileMode.Create));
                kirjoita2.Write(lompakko);

            }
            catch (Exception ex)
            {
                Console.WriteLine("Lompakkoon kirjoitus epäonnistui" + ex.Message);
            }
            finally
            {
                kirjoita2.Close();
            }
        }
        private static void ReadWallet(string lompakkosijainti) //Lukee tiedot lompakosta
        {
            Console.WriteLine(":: Lompakossasi on rahaa ::");
            BinaryReader lue = null;
            try
            {
                lue = new BinaryReader(File.Open(lompakkosijainti, FileMode.Open));
                double funds = lue.ReadDouble();
                Console.WriteLine(":: {0}e ::", funds);
                Console.WriteLine("Paina mitä tahansa näppäintä poistuaksesi");
                Console.ReadKey();

            }
            catch (Exception ex)
            {
                Console.WriteLine("Omiin tiedostoihin kirjoitus epäonnistui" + ex.Message);
            }
            finally
            {
                lue.Close();
            }

        }

        private static void CreateBasket(string korisijainti)
        {
            StreamWriter kirj = null;
            try
            {
                kirj = new StreamWriter(korisijainti);

            }
            catch (Exception ex)
            {
                Console.WriteLine("Tiedoston luonti epäonnistui " + ex.Message);
            }
            finally
            {
                kirj.Close();
            }
            //File.WriteAllText(korisijainti, string.Empty);
        }

        public static void ReadFromMyDocs3(string korisijainti) //Lukee My documents kansion tiedostosta kirjat
        {
            Console.WriteLine("* Nimi, Kirjoittaja, Vuosi");
            Console.WriteLine("* Hinta (e)");
            Console.WriteLine("-------------------------------------");
            StreamReader lue = null;
            try
            {
                lue = new StreamReader(korisijainti);
                while (!lue.EndOfStream)
                {
                    string kirja = lue.ReadLine();
                    string[] temp = kirja.Split(';');
                    int.TryParse(temp[2], out int tempint);
                    double.TryParse(temp[4], out double tempdouble);
                    Console.WriteLine("- {0}, {1}, {2}, \n- {3}e", temp[0], temp[1], tempint, tempdouble);
                    Console.WriteLine("-------------------");

                }
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine("File not found " + ex.Message);
            }
            finally
            {
                lue.Close();
            }
        }
    
        private static void MainMenu()
        {
            PrintLogo();
            Console.WriteLine("* * * * * * * * * * * * * * * * * * * * * * ");
            Console.WriteLine("*  Tarmon lähikirjakauppa mitä saisi olla? *");
            Console.WriteLine("*  1. Haluan selata kirjoja                *");
            Console.WriteLine("*  2. Haluan ostaa kirjan                  *");
            Console.WriteLine("*  3. Haluan myydä kirjan                  *");
            Console.WriteLine("*  4. Haluan tarkistaa rahatilanteeni      *");
            Console.WriteLine("*  5. Haluan katsoa ostoksiani             *");
            Console.WriteLine("*  6. Haluan lisää rahaa                   *");
            Console.WriteLine("*  7. Päivitä kaupan kirjavalikoima        *");
            Console.WriteLine("*  0. Haluan poistua kaupasta              *");
            Console.WriteLine("* * * * * * * * * * * * * * * * * * * * * * ");
            Console.WriteLine("Valitse antamalla numero: ");
        }

        private static void Menu()
        {
            Console.WriteLine("* * * * * * * * * * * *");
            Console.WriteLine("*  Valitse kategoria: *");
            Console.WriteLine("*  1. Romaani         *");
            Console.WriteLine("*  2. Dekkari         *");
            Console.WriteLine("*  3. Lasten          *");
            Console.WriteLine("*  4. Luettele kaikki *");
            Console.WriteLine("*  0. <- Takaisin <-  *");
            Console.WriteLine("* * * * * * * * * * * *");
            Console.WriteLine("Valitse antamalla numero: ");
        }

        private static void PrintLogo()
        {
            Console.WriteLine(" _____                          ");
            Console.WriteLine("|_   _|___ ___ _____ ___ ___    ");
            Console.WriteLine("  | | | .'|  _|     | . |   |   ");
            Console.WriteLine("  |_| |__,|_| |_|_|_|___|_|_|   ");
            Console.WriteLine(" _   _     _     _                        ");
            Console.WriteLine("| |_|_|___|_|___| |_ ___ _ _ ___ ___ ___  ");
            Console.WriteLine("| '_| |  _| | .'| '_| .'| | | . | . | .'| ");
            Console.WriteLine("|_,_|_|_|_| |__,|_,_|__,|___|  _|  _|__,| ");
            Console.WriteLine("        |___|               |_| |_|       ");
        }
    }
}