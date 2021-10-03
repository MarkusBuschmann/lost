using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace lost
{
    public partial class Form1 : Form
    {
        public enum Kartenposition
        {
            Stapel,
            MeineHand,
            GegnerHand,
            MeineReihe,
            GegnerReihe,
            Abwurf
        }
        public enum Farben
        {
            Gelb,
            Blau,
            Weiß,
            Grün,
            Rot
        }

        public enum Move2Make
        {
            Abwerfen = 0,
            Anlegen = 1
        }
        const int Gelb = 0;
        const int Blau = 1;
        const int Weiß = 2;
        const int Grün = 3;
        const int Rot = 4;

        Kartenposition[,] Karte = new Kartenposition[5,14];

        int StapelSize = 52;
        int[] gelbAblagePos = new int[14];
        int[] blauAblagePos = new int[14];
        int[] weißAblagePos = new int[14];
        int[] grünAblagePos = new int[14];
        int[] rotAblagePos = new int[14];

        int anzMeineHand = 0;
        int anzGegnerHand = 0;
        Label[] MeineHandkarten = new Label[8];
        Label[] GegnerHandkarten = new Label[8];
        Label[] MeinPunkteStapel = new Label[5];
        Label[] GegnerPunkteStapel = new Label[5];
        Label[] Abwurfstapel = new Label[5];
        Random rnd;

        public Form1()
        {
            InitializeComponent();
            MeineHandkarten[0] = label1;
            MeineHandkarten[1] = label2;
            MeineHandkarten[2] = label3;
            MeineHandkarten[3] = label4;
            MeineHandkarten[4] = label5;
            MeineHandkarten[5] = label6;
            MeineHandkarten[6] = label7;
            MeineHandkarten[7] = label8;

            GegnerHandkarten[0] = label9;
            GegnerHandkarten[1] = label10;
            GegnerHandkarten[2] = label11;
            GegnerHandkarten[3] = label12;
            GegnerHandkarten[4] = label13;
            GegnerHandkarten[5] = label14;
            GegnerHandkarten[6] = label15;
            GegnerHandkarten[7] = label16;

            MeinPunkteStapel[0] = label21;
            MeinPunkteStapel[1] = label20;
            MeinPunkteStapel[2] = label19;
            MeinPunkteStapel[3] = label18;
            MeinPunkteStapel[4] = label17;

            GegnerPunkteStapel[0] = label26;
            GegnerPunkteStapel[1] = label25;
            GegnerPunkteStapel[2] = label24;
            GegnerPunkteStapel[3] = label23;
            GegnerPunkteStapel[4] = label22;

            Abwurfstapel[0] = label31;
            Abwurfstapel[1] = label30;
            Abwurfstapel[2] = label29;
            Abwurfstapel[3] = label28;
            Abwurfstapel[4] = label27;
            rnd = new Random();
        }
        private void GetCards(Kartenposition Kartenposition)
        {
            for (int i = 0; i < 8; i++)
            {
                bool drawn = false;
                while (!drawn)
                {
                    var color = rnd.Next(5);
                    var value = rnd.Next(1, 13);
                    if (Karte[color, value] == Kartenposition.Stapel)
                    {
                        drawn = true;
                        Karte[color, value] = Kartenposition;
                        Debug.WriteLine($"Hand: {Kartenposition} - Farbe: {color} - Wert: {value} ");
                        if (Kartenposition == Kartenposition.MeineHand)
                        {
                            MeineHandkarten[anzMeineHand].Text = $"{value}";
                            MeineHandkarten[anzMeineHand].Tag = color;
                            MeineHandkarten[anzMeineHand].BackColor = DetermineBackColor(color);
                            anzMeineHand++;
                        }
                        else
                        {
                            GegnerHandkarten[anzGegnerHand].Text = $"{value}";
                            GegnerHandkarten[anzGegnerHand].Tag = color;
                            GegnerHandkarten[anzGegnerHand].BackColor = DetermineBackColor(color);
                            anzGegnerHand++;
                        }
                        
                        StapelSize--;
                        labelNachziehstapel.Text = StapelSize.ToString();
                    }
                }
            }
        }

        private void NächsterZug()
        {
            //Welche Karte soll gespielt werden
            var card2Play = rnd.Next(8);
            //Gibt es die gezogene Karte überhaupt noch
            while (MeineHandkarten[card2Play].Text == "")
            {
                card2Play = rnd.Next(8);
            }
            var farbe = (int)MeineHandkarten[card2Play].Tag;
            var move2Make = rnd.Next(2);
            if (move2Make == (int)Move2Make.Abwerfen)
            {
                Karte[farbe, (int.Parse(MeineHandkarten[card2Play].Text))] = Kartenposition.Abwurf;
                Abwurfstapel[farbe].Text = MeineHandkarten[card2Play].Text;
                MeineHandkarten[card2Play].Text = "";
            }
            else
            {
                if (IstZugGültig(MeineHandkarten[card2Play], MeinPunkteStapel[farbe]))
                {
                    Karte[farbe, (int.Parse(MeineHandkarten[card2Play].Text))] = Kartenposition.MeineReihe;
                    MeinPunkteStapel[farbe].Text = MeineHandkarten[card2Play].Text;
                    MeineHandkarten[card2Play].Text = "";
                }
                else
                {
                    NächsterZug();
                }
            }
            KarteNachziehen();
        }

        private void KarteNachziehen()
        {
            //Abwurfstapel
        }

        private bool IstZugGültig(Label von, Label nach)        
        {
            if (nach.Text == "" ||
                int.Parse(nach.Text) > 10 ||
                ((int.Parse(von.Text) > int.Parse(nach.Text)) && int.Parse(von.Text) <= 10) ||
                ((int.Parse(von.Text) < int.Parse(nach.Text)) && int.Parse(von.Text) > 10))
            {
                return true;
            }
            else
                return false;
        }

        private Color DetermineBackColor(int color)
        {
            return color == 0 ? MeinPunkteStapel[0].BackColor : color == 1 ? MeinPunkteStapel[1].BackColor :
                                color == 2 ? MeinPunkteStapel[2].BackColor : color == 3 ? MeinPunkteStapel[3].BackColor : MeinPunkteStapel[4].BackColor;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            anzMeineHand = 0;
            anzGegnerHand = 0;
            StapelSize = 52;
            for (int j = 0; j < 5; j++)
            {
                for (int i = 1; i < 14; i++)
                {
                    Karte[j, i] = Kartenposition.Stapel;
                }
            }            
            Debug.WriteLine(Environment.NewLine + Environment.NewLine);
            Debug.WriteLine("========================================================");
            GetCards(Kartenposition.MeineHand);
            GetCards(Kartenposition.GegnerHand);
        }

        private void buttonNextMove_Click(object sender, EventArgs e)
        {
            NächsterZug();
        }
    }
}
