using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;

namespace lost
{
    public partial class Form1 : Form
    {       
        public enum Move2Make
        {
            Abwerfen = 0,
            Anlegen = 1
        }

        Karte[] Nachziehstapel = new Karte[65];
        Karte[] MeineHand = new Karte[8];
        Karte[] GegnerHand = new Karte[8];
        Karte[,] MeinePunkte = new Karte[5, 14];
        Karte[,] GegnerPunkte = new Karte[5, 14];
        Karte[,] Ablage = new Karte[5, 14];

        Label[] MeineHandLabel = new Label[8];
        Label[] GegnerHandLabel = new Label[8];
        Label[] MeinePunkteLabel = new Label[5];
        Label[] GegnerPunkteLabel = new Label[5];
        Label[] AblageLabel = new Label[5];

        Random rnd;

        public Form1()
        {
            InitializeComponent();
            MeineHandLabel[0] = label1;
            MeineHandLabel[1] = label2;
            MeineHandLabel[2] = label3;
            MeineHandLabel[3] = label4;
            MeineHandLabel[4] = label5;
            MeineHandLabel[5] = label6;
            MeineHandLabel[6] = label7;
            MeineHandLabel[7] = label8;

            GegnerHandLabel[0] = label9;
            GegnerHandLabel[1] = label10;
            GegnerHandLabel[2] = label11;
            GegnerHandLabel[3] = label12;
            GegnerHandLabel[4] = label13;
            GegnerHandLabel[5] = label14;
            GegnerHandLabel[6] = label15;
            GegnerHandLabel[7] = label16;

            MeinePunkteLabel[0] = label21;
            MeinePunkteLabel[1] = label20;
            MeinePunkteLabel[2] = label19;
            MeinePunkteLabel[3] = label18;
            MeinePunkteLabel[4] = label17;

            GegnerPunkteLabel[0] = label26;
            GegnerPunkteLabel[1] = label25;
            GegnerPunkteLabel[2] = label24;
            GegnerPunkteLabel[3] = label23;
            GegnerPunkteLabel[4] = label22;

            AblageLabel[0] = label31;
            AblageLabel[1] = label30;
            AblageLabel[2] = label29;
            AblageLabel[3] = label28;
            AblageLabel[4] = label27;

            rnd = new Random();
        }

        private void MixAllCards()
        {
            for (int i = 0; i < 65; i++)
            {
                while (true)
                {
                    var farbe = (Farbe)rnd.Next(0,5);
                    var wert = rnd.Next(1, 14);
                    if (Nachziehstapel.Any(x => x != null && x.Farbe == farbe && x.Wert == wert))
                    {
                        continue;
                    }
                    else
                    {
                        Nachziehstapel[i] = new Karte()
                        {
                            Farbe = farbe,
                            Wert = wert
                        };
                        break;
                    }
                }
            }
        }

        private Karte ZieheObersteVomNachziehstapel()
        {
            for (int i = 64; i >= 0; i--)
            {
                if (Nachziehstapel[i] != null)
                {
                    var k = Nachziehstapel[i];
                    Nachziehstapel[i] = null;
                    return k;
                }
            }
            return null;
        }

        private Karte ErmittleObersteVonMeinePunkte(int farbe)
        {
            for (int i = 13; i >= 0; i--)
            {
                if (MeinePunkte[farbe,i] != null)
                {
                    return MeinePunkte[farbe, i]; ;
                }
            }
            return null;
        }

        private void ZieheObersteVonIrgendeinemAblageStapel()
        {

        }

        private void GetStartingCards()
        {
            for (int i = 0; i < 8; i++)
            {
                MeineHand[i] = ZieheObersteVomNachziehstapel();
            }
            for (int i = 0; i < 8; i++)
            {
                GegnerHand[i] = ZieheObersteVomNachziehstapel();
            }
        }

        private void LegeKarteAufAblage(Karte karte)
        {
            for (int i = 0; i <= 13; i++)
            {
                if (Ablage[(int)karte.Farbe,i] == null)
                {
                    Ablage[(int)karte.Farbe,i] = karte;
                    break;
                }
            }
        }

        private void LegeKarteAufMeinePunkte(Karte karte)
        {
            for (int i = 0; i <= 13; i++)
            {
                if (MeinePunkte[(int)karte.Farbe, i] == null)
                {
                    MeinePunkte[(int)karte.Farbe, i] = karte;
                    break;
                }
            }
        }

        private void NächsterZug()
        {
            //Welche Karte soll gespielt werden
            var c = rnd.Next(8);
            //c = 6;
            //Gibt es die gezogene Karte überhaupt noch
            while (MeineHand[c] == null)
            {
                c = rnd.Next(8);
            }
            var move2Make = rnd.Next(2);
            //move2Make = 1;
            if (move2Make == (int)Move2Make.Abwerfen)
            {
                LegeKarteAufAblage(MeineHand[c]);
                MeineHand[c] = null;
            }
            else
            {
                if (IstZugGültig(MeineHand[c])) 
                {
                    LegeKarteAufMeinePunkte(MeineHand[c]);
                    MeineHand[c] = null;
                    
                }
                else
                {
                    NächsterZug();
                }
            }
            //Next:
            //If Random 0,1 = 0 
            //  ZieheObersteVonIrgendeinemAblageStapel();
            //else
            //  ZieheObersteVomNachziehstapel


            ShowCards();
        }

        private bool IstZugGültig(Karte karte)        
        {
            var oberste = ErmittleObersteVonMeinePunkte((int)karte.Farbe);
            if (oberste == null ||
                oberste.Wert > 10 ||
                ((karte.Wert > oberste.Wert) && karte.Wert <= 10) ||
                ((karte.Wert < oberste.Wert) && karte.Wert > 10))
            {
                return true;
            }
            else
                return false;
        }

        private Color DetermineBackColor(int color)
        {
            return color == 0 ? MeinePunkteLabel[0].BackColor : color == 1 ? MeinePunkteLabel[1].BackColor :
                                color == 2 ? MeinePunkteLabel[2].BackColor : color == 3 ? MeinePunkteLabel[3].BackColor : MeinePunkteLabel[4].BackColor;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            MixAllCards();
            GetStartingCards();
            ShowCards();
        }

        private void buttonNextMove_Click(object sender, EventArgs e)
        {
            NächsterZug();
        }
        
        private void ShowCards()
        {
            for (int i = 0; i < 8; i++)
            {
                if (MeineHand[i] != null)
                {
                    MeineHandLabel[i].Text = MeineHand[i].Wert.ToString();
                    MeineHandLabel[i].BackColor = DetermineBackColor((int)MeineHand[i].Farbe);
                }
                else
                {
                    MeineHandLabel[i].Text = "";
                    MeineHandLabel[i].BackColor = Color.Gray;
                }
            }
            for (int i = 0; i < 8; i++)
            {
                if (GegnerHand[i] != null)
                {
                    GegnerHandLabel[i].Text = GegnerHand[i].Wert.ToString() ;
                    GegnerHandLabel[i].BackColor = DetermineBackColor((int)GegnerHand[i].Farbe);
                }
                else
                {
                    GegnerHandLabel[i].Text = "";
                    GegnerHandLabel[i].BackColor = Color.Gray;
                }
            }
            for (int i = 64; i >= 0 ; i--)
            {
                if (Nachziehstapel[i] != null)
                {
                    NachziehstapelLabel.Text = Nachziehstapel[i].Wert.ToString();
                    NachziehstapelLabel.BackColor = DetermineBackColor((int)Nachziehstapel[i].Farbe);
                    break;
                }
            }
            for (int i = 0; i < 5; i++)
            {
                for (int j = 13; j >= 0; j--)
                {
                    if (Ablage[i, j] != null)
                    {
                        AblageLabel[i].Text = Ablage[i, j].Wert.ToString();
                        break;
                    }
                }
            }
            for (int i = 0; i < 5; i++)
            {
                for (int j = 13; j >= 0; j--)
                {
                    if (MeinePunkte[i, j] != null)
                    {
                        MeinePunkteLabel[i].Text = MeinePunkte[i, j].Wert.ToString();
                        break;
                    }
                }
            }
        }
    }
    public class Karte
    {
        public Farbe Farbe { get; set; }
        public int Wert { get; set; }
    }
    public enum Farbe
    {
        Gelb,
        Blau,
        Weiß,
        Grün,
        Rot
    }

}
