using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            Ablage
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

        public Form1()
        {
            InitializeComponent();
        }
        private void GetCards(Random rnd, Kartenposition Kartenposition)
        {
            for (int i = 0; i < 8; i++)
            {
                bool drawn = false;
                while (!drawn)
                {
                    var color = rnd.Next(5);
                    var value = rnd.Next(1, 14);
                    if (Karte[color, value] == Kartenposition.Stapel)
                    {
                        drawn = true;
                        Karte[color, value] = Kartenposition;
                        Debug.WriteLine($"Hand: {Kartenposition} - Farbe: {color} - Wert: {value} ");
                        anzMeineHand++;
                        StapelSize--;
                    }
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            for (int j = 0; j < 5; j++)
            {
                for (int i = 1; i < 14; i++)
                {
                    Karte[j, i] = Kartenposition.Stapel;
                }
            }
            Random rnd = new Random();
            Debug.WriteLine(Environment.NewLine + Environment.NewLine);
            Debug.WriteLine("========================================================");
            GetCards(rnd, Kartenposition.MeineHand);
            GetCards(rnd, Kartenposition.GegnerHand);
        }
    }
}
