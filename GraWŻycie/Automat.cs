using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.IO;

namespace GraWŻycie
{
    class Automat
    {
        public struct Komórka
        {
            public bool stan;
            public bool stan_następny;
            public PictureBox pole;
            public void Przepisz()
            {
                stan = stan_następny;
            }
            public void ZaznaczOdzaznacz()
            {
                stan = !stan;
                stan_następny = !stan_następny;
            }
        }
        public Komórka[,] siatka;
        private int szerokość;
        private int wysokość;
        private int wielkość_pola;
        private Form adres;
        private ComboBox lista_wzorców;

        public Automat(Form adres_formy, ComboBox wzory)
        {
            adres = adres_formy;
            lista_wzorców = wzory;
        }
        
        public Automat(int sze, int wys, int wielkość, Form adres_formy, ComboBox wzory)
        {
            szerokość = sze;
            wysokość = wys;
            wielkość_pola = wielkość;
            adres = adres_formy;
            lista_wzorców = wzory;
            siatka = new Komórka[szerokość, wysokość];
            for (int i = 0; i < szerokość; i++)
            {
                for (int j = 0; j < wysokość; j++)
                {
                    siatka[i, j].pole = new PictureBox();
                    siatka[i, j].pole.Parent = adres;
                    siatka[i, j].pole.Width = wielkość;
                    siatka[i, j].pole.Height = wielkość;
                    siatka[i, j].pole.Left = wielkość * i;
                    siatka[i, j].pole.Top = 100 + wielkość * j;
                    siatka[i, j].pole.BackColor = Color.White;
                    siatka[i, j].pole.BorderStyle = BorderStyle.FixedSingle;
                    siatka[i, j].pole.Click += pictureBox1_Click;
                    siatka[i, j].pole.Tag = i * wysokość + j;
                    siatka[i, j].stan = false;
                    siatka[i, j].stan_następny = false;
                }
            }
        }

        public void Usuń()
        {
            for (int i = 0; i < szerokość; i++)
            {
                for (int j = 0; j < wysokość; j++)
                {
                    siatka[i, j].pole.Dispose();
                }
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            PictureBox tmp;
            tmp = (PictureBox)sender;
            if (tmp.BackColor == Color.White)
                tmp.BackColor = Color.Black;
            else
                tmp.BackColor = Color.White;
            int poz_x = (int)tmp.Tag / wysokość;
            int poz_y = (int)tmp.Tag % wysokość;
            siatka[poz_x, poz_y].ZaznaczOdzaznacz();
        }

        public void Zapisz()
        {
            if (!lista_wzorców.Items.Contains(lista_wzorców.Text))
                lista_wzorców.Items.Add(lista_wzorców.Text);
            StreamWriter sw = new StreamWriter(Directory.GetCurrentDirectory() + "\\lista.in");
            sw.WriteLine(lista_wzorców.Items.Count);
            for (int i = 0; i < lista_wzorców.Items.Count; i++)
            {
                sw.WriteLine(lista_wzorców.Items[i].ToString());
            }
            sw.Close();
            sw = new StreamWriter(Directory.GetCurrentDirectory() + "\\wzory\\" + lista_wzorców.Text + ".wzr");
            sw.WriteLine(szerokość);
            sw.WriteLine(wysokość);
            for (int i = 0; i < szerokość; i++)
            {
                for (int j = 0; j < wysokość; j++)
                {
                    if (siatka[i, j].stan == false)
                        sw.Write(0);
                    else
                        sw.Write(1);
                }
                sw.WriteLine();
            }
            sw.Close();
        }

        public void Wczytaj()
        {
            StreamReader sr = new StreamReader(Directory.GetCurrentDirectory() + "\\wzory\\" + lista_wzorców.Text + ".wzr");
            Usuń();
            szerokość = int.Parse(sr.ReadLine());
            wysokość = int.Parse(sr.ReadLine());
            int wielkość_x = adres.Width / szerokość;
            int wielkość_y = (adres.Height - 150) / wysokość;
            if (wielkość_x < wielkość_y)
                wielkość_pola = wielkość_x;
            else
                wielkość_pola = wielkość_y;
            siatka = new Komórka[szerokość, wysokość];
            int stan_komórki;
            for (int i = 0; i < szerokość; i++)
            {
                for (int j = 0; j < wysokość; j++)
                {
                    siatka[i, j].pole = new PictureBox();
                    siatka[i, j].pole.Parent = adres;
                    siatka[i, j].pole.Width = wielkość_pola;
                    siatka[i, j].pole.Height = wielkość_pola;
                    siatka[i, j].pole.Left = wielkość_pola * i;
                    siatka[i, j].pole.Top = 100 + wielkość_pola * j;
                    siatka[i, j].pole.BackColor = Color.White;
                    siatka[i, j].pole.BorderStyle = BorderStyle.FixedSingle;
                    siatka[i, j].pole.Click += pictureBox1_Click;
                    siatka[i, j].pole.Tag = i * wysokość + j;
                    stan_komórki = sr.Read();
                    if (stan_komórki == 49)
                    {
                        siatka[i, j].ZaznaczOdzaznacz();
                        siatka[i, j].pole.BackColor = Color.Black;
                        siatka[i, j].stan_następny = true;
                    }
                    else
                    {
                        siatka[i, j].stan_następny = false;
                        siatka[i, j].stan = false;
                    }
                }
                sr.ReadLine();
            }
            sr.Close();
        }

        public void Krok()
        {
            int sąsiedzi;
            for (int i = 0; i < szerokość; i++)
            {
                for (int j = 0; j < wysokość; j++)
                {
                    sąsiedzi = Ile_sąsiadów(i, j);
                    if (siatka[i, j].stan == false && sąsiedzi == 3)
                    {
                        siatka[i, j].stan_następny = true;
                    }
                    else if (siatka[i, j].stan == true && (sąsiedzi < 2 || sąsiedzi > 3))
                    {
                        siatka[i, j].stan_następny = false;
                    }
                }
            }
            for (int i = 0; i < szerokość; i++)
            {
                for (int j = 0; j < wysokość; j++)
                {
                    if (siatka[i, j].stan != siatka[i, j].stan_następny)
                    {
                        siatka[i, j].ZaznaczOdzaznacz();
                        if (siatka[i, j].pole.BackColor == Color.White)
                            siatka[i, j].pole.BackColor = Color.Black;
                        else
                            siatka[i, j].pole.BackColor = Color.White;
                        siatka[i, j].stan_następny = siatka[i, j].stan;
                    }
                }
            }
        }

        public int Ile_sąsiadów(int x, int y)
        {
            int liczba_żywych_sąsiadów = 0;
            if (siatka[x, (y + 1)%wysokość].stan == true)
                liczba_żywych_sąsiadów++;
            if (siatka[(x + 1)%szerokość, y].stan == true)
                liczba_żywych_sąsiadów++;
            if (siatka[(x + 1) % szerokość, (y + 1) % wysokość].stan == true)
                liczba_żywych_sąsiadów++;
            if (siatka[(x + szerokość - 1) % szerokość, y].stan == true)
                liczba_żywych_sąsiadów++;
            if (siatka[x, (y + wysokość - 1) % wysokość].stan == true)
                liczba_żywych_sąsiadów++;
            if (siatka[(x + szerokość - 1) % szerokość, (y + wysokość - 1) % wysokość].stan == true)
                liczba_żywych_sąsiadów++;
            if (siatka[(x + 1) % szerokość, (y + wysokość - 1) % wysokość].stan == true)
                liczba_żywych_sąsiadów++;
            if (siatka[(x + szerokość - 1) % szerokość, (y + 1) % wysokość].stan == true)
                liczba_żywych_sąsiadów++;
            return liczba_żywych_sąsiadów;
        }
    }
}
