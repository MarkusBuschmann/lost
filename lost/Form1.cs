using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;
using Microsoft.VisualBasic;
using System.Threading;

namespace lost
{
    public partial class Form1 : Form
    {       
        public enum Move2Make
        {
            Abwerfen = 0,
            Anlegen = 1
        }

        public bool stopGameLoop = false;
        public int anzSpiele = 0;
        public bool MeinZug = false;
        private bool Spielläuft = true;
        private bool weiter = false;
        Karte[] Nachziehstapel = new Karte[65];
        Karte[] MeineHand = new Karte[8];
        Karte[] GegnerHand = new Karte[8];
        Karte[,] MeinePunkte = new Karte[5, 13];
        Karte[,] GegnerPunkte = new Karte[5, 13];
        Karte[,] Ablage = new Karte[5, 13];

        Label[] MeineHandLabel = new Label[8];
        Label[] GegnerHandLabel = new Label[8];
        Label[] MeinePunkteLabel = new Label[5];
        Label[] GegnerPunkteLabel = new Label[5];
        Label[] AblageLabel = new Label[5];

        int meineMaxPunkte = 0;
        int gegnerMaxPunkte = 0;

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

        private void MixAllCards2Nachziehstapel()
        {
            ResetCards();
            for (int i = 0; i < 65; i++)
            {
                while (true)
                {
                    var farbe = (Farbe)rnd.Next(0,5);
                    var wert = rnd.Next(1, 14);
                    if (wert > 11)
                        wert = 11;

                    //if (Nachziehstapel.Any(x => x != null && (x.Wert == 11 && Nachziehstapel.Count(y => y.Farbe == farbe && y.Wert == 11) == 3 || 
                    //x.Farbe == farbe && x.Wert == wert)))
                    if (wert == 11 && Nachziehstapel.Count(x => x != null && x.Farbe == farbe && x.Wert == 11) == 3 ||
                     wert < 11 && Nachziehstapel.Any(x => x != null && x.Farbe == farbe && x.Wert == wert))
                    {
                        //continue;
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
        //private bool HatNachziehStapelKarten()
        //{
        //    return Nachziehstapel.Any(x => x != null);
        //}

        private Karte ZieheObersteVomNachziehstapel()
        {
            for (int i = 64; i >= 0; i--)
            {
                if (Nachziehstapel[i] != null)
                {
                    var k = Nachziehstapel[i];
                    Nachziehstapel[i] = null;
                    if (i == 0)
                    {
                        Spielläuft = false;
                    }
                    return k;
                }
            }
            Spielläuft = false;
            return null;
        }

        private Karte ErmittleObersteVonPunkteStapel(int farbe)
        {
            Karte[,] PunkteStapel;
            if (MeinZug)
            {
                PunkteStapel = MeinePunkte;
            }
            else
            {
                PunkteStapel = GegnerPunkte;
            }

            for (int i = 12; i >= 0; i--)
            {
                if (PunkteStapel[farbe,i] != null)
                {
                    return PunkteStapel[farbe, i]; ;
                }
            }
            return null;
        }

        private Karte ZieheObersteVonIrgendeinerAblage()
        {
            //Kein Ablagestapel enthält eine Karte
            if (Ablage[0, 0] == null && Ablage[1, 0] == null && Ablage[2, 0] == null && Ablage[3, 0] == null && Ablage[4, 0] == null)
            {
                return null;
            }

            var farbenSchonProbiert = new bool[5];
            Karte k = null;
            while (k == null)
            {
                int farbe = rnd.Next(5);
                farbenSchonProbiert[farbe] = true;
                for (int pos = 12; pos >= 0; pos--)
                {
                    if (Ablage[farbe, pos] != null)
                    {
                        //Nur ziehen, wenn anlegbar
                        var oberste = ErmittleObersteVonPunkteStapel(farbe);
                        if (oberste != null && DarfKarteAufPunkte(Ablage[farbe, pos].Wert, oberste.Wert))
                        { 
                            k = Ablage[farbe, pos];
                            Ablage[farbe, pos] = null;
                            return k;
                        }
                    }
                }
                if (farbenSchonProbiert.All(x => x == true))
                {
                    return null;
                }
            }
            return k;
        }

        private void GetStartingCards(bool onlyGegner)
        {
            //Debug.Write("Meine Hand: ");
            if (!onlyGegner)
            {
                for (int i = 0; i < 8; i++)
                {
                    MeineHand[i] = ZieheObersteVomNachziehstapel();
                    //Debug.Write($"{MeineHand[i].Farbe}{MeineHand[i].Wert} ");
                }
            }
            //Debug.WriteLine("");
            //Debug.Write("Gegnerhand: ");
            for (int i = 0; i < 8; i++)
            {
                GegnerHand[i] = ZieheObersteVomNachziehstapel();
                //Debug.Write($"{GegnerHand[i].Farbe}{GegnerHand[i].Wert} ");
            }
            //Debug.WriteLine("");
        }

        private void LegeKarteAufAblage(Karte karte)
        {
            for (int i = 0; i <= 12; i++)
            {
                if (Ablage[(int)karte.Farbe,i] == null)
                {
                    Ablage[(int)karte.Farbe,i] = karte;
                    break;
                }
            }
        }
        private void LegeKarteAufNachziehstapel(Karte karte)
        {
            for (int i = 0; i <= 65; i++)
            {
                if (Nachziehstapel[i] == null)
                {
                    Nachziehstapel[i] = karte;
                    break;
                }
            }
        }

        private void LegeKarteAufPunkte(Karte karte)
        {
            Karte[,] Punktestapel;
            if (MeinZug)
            {
                Punktestapel = MeinePunkte;
            }
            else
            {
                Punktestapel = GegnerPunkte;
            }

            for (int i = 0; i < 13; i++)
            {
                if (Punktestapel[(int)karte.Farbe, i] == null)
                {
                    Punktestapel[(int)karte.Farbe, i] = karte;
                    break;
                }
            }
        }

        private int ErmittleGesamtpunkteAufHandEinerFarbe(Farbe farbe)
        {
            var i = 0;

            Karte[] Hand;
            if (MeinZug)
            {
                Hand = MeineHand;
            }
            else
            {
                Hand = GegnerHand;
            }
            i = Hand.Where(x => x.Farbe == farbe && x.Wert < 11).Sum(x => x.Wert);
            return i;
        }

        private int ErmittleGesamtpunkteAufPunktestapelEinerFarbe(Farbe farbe)
        {
            var i = 0;

            Karte[,] PunkteStapel;
            if (MeinZug)
            {
                PunkteStapel = MeinePunkte;
            }
            else
            {
                PunkteStapel = GegnerPunkte;
            }
            for (int j = 0; j < 13; j++)
            {
                if (PunkteStapel[(int)farbe, j] != null)
                {
                    i = i + PunkteStapel[(int)farbe, j].Wert;
                }
            }
            return i;
        }

        private bool ErmittleObPunkteSchonBegonnen(Farbe farbe)
        {

            Karte[,] PunkteStapel;
            if (MeinZug)
            {
                PunkteStapel = MeinePunkte;
            }
            else
            {
                PunkteStapel = GegnerPunkte;
            }
            for (int j = 0; j < 13; j++)
            {
                if (PunkteStapel[(int)farbe, j] != null)
                {
                    return true;
                }
            }
            return false;
        }

        private void NächsterZug(Karte[] kartenhand,int? kartenPos,int? nächsteWohin, bool letztesDrittel = false)
        {
            int c;
            ShowRichtextBox($"Meinzug: {MeinZug}");
            if (kartenPos == null)
            {
                //Welche Karte soll gespielt werden
                c = rnd.Next(8);
                //Gibt es die gezogene Karte überhaupt noch
                while (kartenhand[c] == null)
                {
                    c = rnd.Next(8);
                }
            }
            else
            {
                c = (int)kartenPos;
            }
            var nextAction = rnd.Next(2);
            //0 = Punkte,1 = Ablage
            if (nächsteWohin == 0 || nächsteWohin == null)
            {
                var oberste = ErmittleObersteVonPunkteStapel((int)kartenhand[c].Farbe);
                if ((oberste == null || DarfKarteAufPunkte(kartenhand[c].Wert, oberste.Wert)) &&
                    (ErmittleObPunkteSchonBegonnen(kartenhand[c].Farbe) ||
                    (MeinZug && ErmittleGesamtpunkteAufHandEinerFarbe(kartenhand[c].Farbe) >= int.Parse(textBoxMinSum.Text) ||
                    !MeinZug && ErmittleGesamtpunkteAufHandEinerFarbe(kartenhand[c].Farbe) >= int.Parse(textBoxMinSumGegner.Text))
                    && Nachziehstapel.Count(x => x != null) > 30)
                    )
                {
                    //Ermittle im 1. und 2. Drittel immer die niedrigste bzw die Verdoppler zuerst
                    if (!letztesDrittel)
                    {
                        for (int i = 0; i < 8; i++)
                        {
                            //Verdoppler
                            if (i != c && 
                                kartenhand[i].Farbe == kartenhand[c].Farbe && 
                                (oberste == null || DarfKarteAufPunkte(kartenhand[i].Wert, oberste.Wert)) &&
                                (kartenhand[i].Wert > 10))
                            {
                                c = i;
                                break;
                            }
                            //Kleinere Karte derselben Farbe nehmen
                            if (i != c &&
                                kartenhand[i].Farbe == kartenhand[c].Farbe &&
                                (oberste == null || DarfKarteAufPunkte(kartenhand[i].Wert, oberste.Wert)) &&
                                kartenhand[i].Wert < kartenhand[c].Wert)
                            {
                                ShowRichtextBox($"c = i, c:{c} i:{i}", true);
                                c = i;
                            }
                        }
                    }
                    LegeKarteAufPunkte(kartenhand[c]);
                    ShowRichtextBox($"Lege Karte auf Punkte {kartenhand[c].Farbe} {kartenhand[c].Wert}",true);
                    kartenhand[c] = null;
                }
                else
                {
                    LegeKarteAufAblage(kartenhand[c]);
                    ShowRichtextBox($"ABLAGE {kartenhand[c].Farbe} {kartenhand[c].Wert}",true);
                    kartenhand[c] = null;
                }
            }
            else
            {
                LegeKarteAufAblage(kartenhand[c]);
                ShowRichtextBox($"ABLAGE {kartenhand[c].Farbe} {kartenhand[c].Wert}",true);
                kartenhand[c] = null;
            }

            nextAction = rnd.Next(2);
            if (nextAction == 0)
            {
                kartenhand[c] = ZieheObersteVonIrgendeinerAblage();
                if (kartenhand[c] == null)
                {
                    kartenhand[c] = ZieheObersteVomNachziehstapel();
                    if (kartenhand[c] == null)
                    {
                        ShowRichtextBox("SPIELENDE",true);
                    }
                    else
                    {
                        ShowRichtextBox($"Ziehe von Nachziehstapel {kartenhand[c].Farbe} {kartenhand[c].Wert}",true);
                    }
                }
                else
                {
                    ShowRichtextBox($"Ziehe von ABLAGE {kartenhand[c].Farbe} {kartenhand[c].Wert}",true);
                }
            }
            else
            {
                kartenhand[c] = ZieheObersteVomNachziehstapel();
                if (kartenhand[c] == null)
                {
                    ShowRichtextBox("SPIELENDE",true);
                }
                else
                {
                    ShowRichtextBox($"Ziehe von Nachziehstapel {kartenhand[c].Farbe} {kartenhand[c].Wert}",true);
                }
            }
            if (checkBoxWaitAfterMoves.Checked)
            {
                if (checkBoxMyMoves.Checked && MeinZug || !checkBoxMyMoves.Checked)
                {
                    //ShowCards();
                    //Application.DoEvents();
                    //while (!weiter)
                    //{
                    //    Thread.Sleep(200);
                    //    Application.DoEvents();
                    //}
                    //weiter = false;
                }
            }
        }

        private void ShowRichtextBox(string s,bool add = false)
        {
            if (checkBoxShowOutput.Checked)
            {
                if (add)
                {
                    richTextBox1.Text = richTextBox1.Text + Environment.NewLine + s;
                }
                else
                {
                    richTextBox1.Text = s;
                }
                Application.DoEvents();
            }
        }
        private bool DarfKarteAufPunkte(int wertKarte, int wertPunkte)
        {
            if (wertPunkte > 10 ||
                ((wertKarte > wertPunkte) && wertKarte <= 10) ||
                ((wertKarte < wertPunkte) && wertKarte > 10))
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
        private void button_newgame(object sender, EventArgs e)
        {
            ResetCards();
            MixAllCards2Nachziehstapel();
            //GetStartingCards();
            ShowCards();
        }

        private void BerechnePunkte(ref int meinePunkteInDiesemTyp, ref int gegnerPunkteInDiesemTyp)
        {
            int meinePunkteDiesesSpiel = 0;
            for (int i = 0; i < 5; i++)
            {
                int sum = 0;
                int anz = 0;
                int anzVerdoppler = 0;
                for (int j = 0; j < 13; j++)
                {
                    if (MeinePunkte[i, j] != null)
                    {
                        if (MeinePunkte[i, j].Wert <= 10)
                        {
                            sum = sum + MeinePunkte[i, j].Wert;
                        }
                        if (MeinePunkte[i, j].Wert > 10)
                        {
                            anzVerdoppler++;
                        }
                        anz++;
                    }
                }
                if (anz > 0)
                {
                    var erg = (sum - 20 + (anz >= 8 ? 20 : 0)) * (1 + anzVerdoppler);
                    meinePunkteDiesesSpiel = meinePunkteDiesesSpiel + erg;
                }
            }
            //if (meinePunktelokal > 40)
            //{
            //    stopGameLoop = true;
            //}
            int gegnerPunkteDiesesSpiel = 0;
            for (int i = 0; i < 5; i++)
            {
                int sum = 0;
                int anz = 0;
                int anzVerdoppler = 0;
                for (int j = 0; j < 13; j++)
                {
                    if (GegnerPunkte[i, j] != null)
                    {
                        if (GegnerPunkte[i, j].Wert <= 10)
                        {
                            sum = sum + GegnerPunkte[i, j].Wert;
                        }
                        if (GegnerPunkte[i, j].Wert > 10)
                        {
                            anzVerdoppler++;
                        }
                        anz++;
                    }
                }
                if (anz > 0)
                {
                    var erg = (sum - 20 + (anz >= 8 ? 20 : 0)) * (1 + anzVerdoppler);
                    gegnerPunkteDiesesSpiel = gegnerPunkteDiesesSpiel + erg;
                }
            }
            //if (gegnerPunktelokal > 40)
            //{
            //    stopGameLoop = true;
            //}

            meinePunkteInDiesemTyp = meinePunkteInDiesemTyp + meinePunkteDiesesSpiel;
            gegnerPunkteInDiesemTyp = gegnerPunkteInDiesemTyp + gegnerPunkteDiesesSpiel;
            if (meinePunkteDiesesSpiel > meineMaxPunkte)
            {
                meineMaxPunkte = meinePunkteDiesesSpiel;
            }
            if (gegnerPunkteDiesesSpiel > gegnerMaxPunkte)
            {
                gegnerMaxPunkte = gegnerPunkteDiesesSpiel;
            }
            labelTitle.Text = $"Ich: {meinePunkteDiesesSpiel}, Gegner: {gegnerPunkteDiesesSpiel}";
        }

        private void buttonNextMove_Click(object sender, EventArgs e)
        {
            //MeinZug = false;
            //Spielläuft = true;
            //while (Spielläuft)
            //{
                MeinZug = !MeinZug;
                if (MeinZug)
                {
                    //NächsterZug(MeineHand);
                }
                else
                {
                    //NächsterZug(GegnerHand);
                }
            //}
            ShowCards();
            //BerechnePunkte(ref _i, ref _j);
        }

        private void ResetCards()
        {
            for (int i = 0; i < 65; i++)
            {
                Nachziehstapel[i] = null;
            }
            for (int i = 0; i < 8; i++)
            {
                MeineHand[i] = null;
                GegnerHand[i] = null;
            }

            for (int i = 0; i <= 4; i++)
            {
                for (int j = 0; j <= 12; j++)
                {
                    Ablage[i,j] = null;
                    MeinePunkte[i, j] = null;
                    GegnerPunkte[i, j] = null;
                }
            }
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
            //for (int i = 64; i >= 0 ; i--)
            //{
            //    if (Nachziehstapel[i] != null)
            //    {
            //        NachziehstapelLabel.Text = Nachziehstapel[i].Wert.ToString();
            //        NachziehstapelLabel.BackColor = DetermineBackColor((int)Nachziehstapel[i].Farbe);
            //        break;
            //    }
            //    else
            //    {
            //        NachziehstapelLabel.Text = "";
            //        NachziehstapelLabel.BackColor = Color.Gray;
            //    }
            //}
            labelStack.Text = "";
            for (int i = 0; i < 65; i++)
            {
                if (Nachziehstapel[i] != null)
                {
                    labelStack.Text = labelStack.Text + Nachziehstapel[i].Farbe.ToString() + Nachziehstapel[i].Wert.ToString() + Environment.NewLine;
                }
            }

            for (int i = 0; i < 5; i++)
            {
                AblageLabel[i].Text = "";
                for (int j = 0; j < 13; j++)
                {
                    if (Ablage[i, j] != null)
                    {
                        AblageLabel[i].Text = AblageLabel[i].Text + Ablage[i, j].Wert.ToString() + ",";
                    }
                }
                if (AblageLabel[i].Text.Length > 1)
                {
                    AblageLabel[i].Text = AblageLabel[i].Text.Substring(0, AblageLabel[i].Text.Length - 1);
                }
            }
            for (int i = 0; i < 5; i++)
            {
                MeinePunkteLabel[i].Text = "";
                for (int j = 0; j < 13; j++)
                {
                    if (MeinePunkte[i, j] != null)
                    {
                        MeinePunkteLabel[i].Text = MeinePunkteLabel[i].Text + MeinePunkte[i, j].Wert.ToString() + "," ;
                    }
                    if (MeinePunkte[i, 0] == null)
                    {
                        MeinePunkteLabel[i].Text = "";
                    }
                }
                if (MeinePunkteLabel[i].Text.Length > 1)
                {
                    MeinePunkteLabel[i].Text = MeinePunkteLabel[i].Text.Substring(0, MeinePunkteLabel[i].Text.Length - 1);
                }
            }
            for (int i = 0; i < 5; i++)
            {
                GegnerPunkteLabel[i].Text = "";
                for (int j = 0; j < 13; j++)
                {
                    if (GegnerPunkte[i, j] != null)
                    {
                        GegnerPunkteLabel[i].Text = GegnerPunkteLabel[i].Text + GegnerPunkte[i, j].Wert.ToString() + ",";
                    }
                    if (GegnerPunkte[i, 0] == null)
                    {
                        GegnerPunkteLabel[i].Text = "";
                    }
                }
                if (GegnerPunkteLabel[i].Text.Length > 1)
                {
                    GegnerPunkteLabel[i].Text = GegnerPunkteLabel[i].Text.Substring(0, GegnerPunkteLabel[i].Text.Length - 1);
                }
            }
            //BerechnePunkte();
        }

        private void StartGameFromThisPoint(object sender, EventArgs e)
        {
            var runde = 0;
            var anzSpieleJeFall = int.Parse(textBoxLoops.Text);
            var meineGesPunkte = 0;
            var gegnerGesPunkte = 0;
            DefinedStartingHand();
            ShowCards();
            Dictionary<string, double> ausgabeDict = new Dictionary<string, double>();
            richTextBox1.Text = $"{anzSpieleJeFall} Spiele je Fall" + Environment.NewLine + Environment.NewLine;
            Application.DoEvents();
            for (int? j = 0; j < 2; j++)
            {
                for (int? i = 0; i < 8; i++)
                {
                    labelType.Text = $"{runde * anzSpieleJeFall} Spiele simuliert";
                    Application.DoEvents();
                    runde++;
                    anzSpiele = 0;
                    var meinePunkteInDiesemTyp = 0;
                    var gegnerPunkteInDiesemTyp = 0;
                    meineMaxPunkte = 0;
                    gegnerMaxPunkte = 0;
                    while (anzSpiele <= anzSpieleJeFall)
                    {
                        anzSpiele++;
                        DefinedStartingHand();
                        var ersterZug = true;
                        //ResetCards();
                        //MixAllCards2Nachziehstapel();
                        //GetStartingCards();
                        GetStartingCards(true); //nur für Gegner
                        MeinZug = !checkBoxIStart.Checked;

                        Spielläuft = true;
                        while (Spielläuft)
                        {
                            var letztesDrittel = false;
                            MeinZug = !MeinZug;
                            if (MeinZug)
                            {
                                if (Nachziehstapel.Count(x => x != null) < 20)
                                {
                                    letztesDrittel = true;
                                }
                                if (ersterZug)
                                {
                                    NächsterZug(MeineHand, i, j); //sukzessive mit diesen Kombinationen beginnen
                                    ersterZug = false;
                                }
                                else
                                {
                                    NächsterZug(MeineHand, null,null, letztesDrittel); //Zufallszug
                                }
                            }
                            else
                            {
                                NächsterZug(GegnerHand,null,null, letztesDrittel);//Zufallszug
                            }
                        }
                        BerechnePunkte(ref meinePunkteInDiesemTyp, ref gegnerPunkteInDiesemTyp);
                    }

                    var s = $"Karte {i + 1} auf {(j == 0 ? "Punkte" : "Ablage")} ; Diff: {(double)meinePunkteInDiesemTyp / anzSpiele - (double)gegnerPunkteInDiesemTyp / anzSpiele:0.#}, " +
                        $"Ich: {(double)meinePunkteInDiesemTyp / anzSpiele:0.#}, Gegner: {(double)gegnerPunkteInDiesemTyp / anzSpiele:0.#}"
                        + $"; Meine Max: {meineMaxPunkte}, GegnerMax: {gegnerMaxPunkte}" + Environment.NewLine;
                    ausgabeDict.Add(s, (double)meinePunkteInDiesemTyp / anzSpiele - (double)gegnerPunkteInDiesemTyp / anzSpiele);
                    meineGesPunkte = meineGesPunkte + meinePunkteInDiesemTyp;
                    gegnerGesPunkte = gegnerGesPunkte + gegnerPunkteInDiesemTyp;
                    Application.DoEvents();
                }
            }
            foreach (var a in ausgabeDict)
            {
                if (a.Value == ausgabeDict.Min(y => y.Value))
                {
                    richTextBox1.SelectionColor = Color.Red;
                    //richTextBox1.SelectionFont = new Font(richTextBox1.Font, FontStyle.Bold); 
                    richTextBox1.AppendText(a.Key);
                }
                else if (a.Value == ausgabeDict.Max(y => y.Value))
                {
                    richTextBox1.SelectionColor = Color.Green;
                    richTextBox1.AppendText(a.Key);
                }
                else
                {
                    richTextBox1.SelectionColor = DefaultForeColor;
                    richTextBox1.AppendText(a.Key);
                }
                if (a.Key.StartsWith("Karte 8"))
                {
                    richTextBox1.AppendText(Environment.NewLine);
                }
            }
            richTextBox1.AppendText (Environment.NewLine +  $"Gesamtdurchschnitt: Mein: {(meineGesPunkte / 16.0 / anzSpieleJeFall):0.#} Gegner: {(gegnerGesPunkte / 16.0 / anzSpieleJeFall):0.#}");
        }

        private static DialogResult ShowInputDialog(ref string input, int x, int y)
        {
            System.Drawing.Size size = new System.Drawing.Size(200, 70);
            Form inputBox = new Form();

            inputBox.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            inputBox.ClientSize = size;
            inputBox.Text = "Karten";

            System.Windows.Forms.TextBox textBox = new TextBox();
            textBox.Size = new System.Drawing.Size(size.Width - 10, 23);
            textBox.Location = new System.Drawing.Point(5, 5);
            textBox.Text = input;
            textBox.SelectionStart = input.Length;
            inputBox.Controls.Add(textBox);

            Button okButton = new Button();
            okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            okButton.Name = "okButton";
            okButton.Size = new System.Drawing.Size(75, 23);
            okButton.Text = "&OK";
            okButton.Location = new System.Drawing.Point(size.Width - 80 - 80, 39);
            inputBox.Controls.Add(okButton);

            Button cancelButton = new Button();
            cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            cancelButton.Name = "cancelButton";
            cancelButton.Size = new System.Drawing.Size(75, 23);
            cancelButton.Text = "&Abbrechen";
            cancelButton.Location = new System.Drawing.Point(size.Width - 80, 39);
            inputBox.Controls.Add(cancelButton);

            inputBox.AcceptButton = okButton;
            inputBox.CancelButton = cancelButton;

            inputBox.StartPosition = FormStartPosition.Manual;
            inputBox.Location = new Point(x, y);
            DialogResult result = inputBox.ShowDialog();
            input = textBox.Text;
            return result;
        }

        private List<Karte> LegeKarten(Farbe? farbeClick)
        {
            var farbekurz = farbeClick.ToString().ToLower();
            if (farbekurz.Length > 0)
            {
                if (farbekurz == "grün")
                {
                    farbekurz = "ü";
                }
                else
                {
                    farbekurz = farbekurz.Substring(0, 1);
                }
            }

            //string input = Interaction.InputBox("Karte", "Karte eingeben", farbekurz, Cursor.Position.X, Cursor.Position.Y - 100);
            string input= farbekurz;
            if (ShowInputDialog (ref input, Cursor.Position.X, Cursor.Position.Y - 100) == DialogResult.Cancel)
            {
                return null;
            }            

            if (input.Length < 2)
            {
                return null;
            }
            var k = new List<Karte>();
            var farbe = input.Substring(0, 1);
            Farbe f  ;
            if (farbe == "g")
            {
                f = Farbe.Gelb;
            }
            else if (farbe == "b")
            {
                f = Farbe.Blau;
            }
            else if (farbe == "w")
            {
                f = Farbe.Weiß;
            }
            else if (farbe == "ü")
            {
                f = Farbe.Grün;
            }
            else if (farbe == "r")
            {
                f = Farbe.Rot;
            }
            else
            {
                return null;
            }
            var wert = input.Substring(1);
            if (wert.Length < 1)
            {
                return null;
            }
            var sp = wert.Split(',');
            foreach (var sps in sp)
            {
                if (int.TryParse(sps, out int w))
                {
                    for (int n = 0; n < 65; n++)
                    {
                        if (Nachziehstapel[n] != null && Nachziehstapel[n].Wert == w && Nachziehstapel[n].Farbe == f)
                        {
                            k.Add(new Karte() { Wert = w, Farbe = f });
                            Nachziehstapel[n] = null;
                            break;
                        }
                    }                    
                }
                else
                {
                    return null;
                }
            }
            return k;
        }

        private void labelMeineHandkartenClick(object sender, EventArgs e)
        {
            var l = (Label)sender;
            var ea = (MouseEventArgs)e;
            if (ea.Button == MouseButtons.Right)
            {
                for (int i = 0; i < 8; i++)
                {
                    if (MeineHandLabel[i] == l)
                    {
                        LegeKarteAufNachziehstapel(MeineHand[i]);
                        MeineHand[i] = null;
                        ShowCards();
                        break;
                    }
                }
            }
            var k = LegeKarten(null);
            if (k != null && k.Count == 1)
            {
                for (int i = 0; i < 8; i++)
                {
                    if (MeineHandLabel[i] == l)
                    {
                        MeineHand[i] = k[0];
                        break;
                    }
                }
                ShowCards();
            }
            else
            {
                return;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            DefinedStartingHand();
        }

        private void DefinedStartingHand()
        {
            ResetCards();

            MixAllCards2Nachziehstapel();

            MeineHand[0] = new Karte() { Farbe = Farbe.Weiß, Wert = 11 };
            MeineHand[1] = new Karte() { Farbe = Farbe.Weiß, Wert = 2 };
            MeineHand[2] = new Karte() { Farbe = Farbe.Weiß, Wert = 7 };
            MeineHand[3] = new Karte() { Farbe = Farbe.Weiß, Wert = 8 };
            MeineHand[4] = new Karte() { Farbe = Farbe.Weiß, Wert = 9 };
            MeineHand[5] = new Karte() { Farbe = Farbe.Grün, Wert = 2 };
            MeineHand[6] = new Karte() { Farbe = Farbe.Grün, Wert = 4 };
            MeineHand[7] = new Karte() { Farbe = Farbe.Grün, Wert = 10 };

            //MeineHand[0] = new Karte() { Farbe = Farbe.Gelb, Wert = 9 };
            //MeineHand[1] = new Karte() { Farbe = Farbe.Blau, Wert = 11 };
            //MeineHand[2] = new Karte() { Farbe = Farbe.Weiß, Wert = 7 };
            //MeineHand[3] = new Karte() { Farbe = Farbe.Weiß, Wert = 8 };
            //MeineHand[4] = new Karte() { Farbe = Farbe.Weiß, Wert = 9 };
            //MeineHand[5] = new Karte() { Farbe = Farbe.Grün, Wert = 4 };
            //MeineHand[6] = new Karte() { Farbe = Farbe.Rot, Wert = 11 };
            //MeineHand[7] = new Karte() { Farbe = Farbe.Rot, Wert = 2 };

            //MeineHand[0] = new Karte() { Farbe = Farbe.Gelb, Wert = 3 };
            //MeineHand[1] = new Karte() { Farbe = Farbe.Gelb, Wert = 6 };
            //MeineHand[2] = new Karte() { Farbe = Farbe.Gelb, Wert = 10 };
            //MeineHand[3] = new Karte() { Farbe = Farbe.Blau, Wert = 4 };
            //MeineHand[4] = new Karte() { Farbe = Farbe.Blau, Wert = 7 };
            //MeineHand[5] = new Karte() { Farbe = Farbe.Grün, Wert = 11 };
            //MeineHand[6] = new Karte() { Farbe = Farbe.Grün, Wert = 5 };
            //MeineHand[7] = new Karte() { Farbe = Farbe.Rot, Wert = 2 };

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 65; j++)
                {
                    if (Nachziehstapel[j] != null && Nachziehstapel[j].Farbe == MeineHand[i].Farbe && Nachziehstapel[j].Wert == MeineHand[i].Wert)
                    {
                        Nachziehstapel[j] = null;
                    }
                }
            }
            //ShowCards();
        }
        private void labelPunkteClick(MouseEventArgs me, Label lbl, Farbe f)
        {
            if (me.Button == MouseButtons.Right)
            {
                if (lbl.Text.Length >= 2)
                {
                    var cardValues = lbl.Text.Split(',');
                    foreach (var cardValue in cardValues)
                    {
                        if (!string.IsNullOrEmpty(cardValue))
                        {
                            var k = new Karte()
                            {
                                Farbe = f,
                                Wert = int.Parse(cardValue)
                            };
                            LegeKarteAufNachziehstapel(k);
                        }
                    }
                    for (int i = 0; i < 13; i++)
                    {
                        MeinePunkte[(int)f, i] = null;
                    }
                    ShowCards();
                }
            }
            LegeKartenAufPunktestapel(f);
        }

        private void label21_Click(object sender, EventArgs e)
        {
            MeinZug = true;
            var me = (MouseEventArgs)e;
            var lbl = (Label)sender;
            var f = Farbe.Gelb;
            labelPunkteClick(me, lbl, f);
        }

        private void label20_Click(object sender, EventArgs e)
        {
            MeinZug = true;
            var me = (MouseEventArgs)e;
            var lbl = (Label)sender;
            var f = Farbe.Blau;
            labelPunkteClick(me, lbl, f);
        }

        private void LegeKartenAufPunktestapel(Farbe f)
        {
            var k = LegeKarten(f);
            if (k != null)
            {
                foreach (var ks in k)
                {
                    LegeKarteAufPunkte(ks);
                }
                ShowCards();
            }
        }

        private void label19_Click(object sender, EventArgs e)
        {
            MeinZug = true;
            var me = (MouseEventArgs)e;
            var lbl = (Label)sender;
            var f = Farbe.Weiß;
            labelPunkteClick(me, lbl, f);
        }

        private void label18_Click(object sender, EventArgs e)
        {
            MeinZug = true;
            var me = (MouseEventArgs)e;
            var lbl = (Label)sender;
            var f = Farbe.Grün;
            labelPunkteClick(me, lbl, f);
        }

        private void label17_Click(object sender, EventArgs e)
        {
            MeinZug = true;
            var me = (MouseEventArgs)e;
            var lbl = (Label)sender;
            var f = Farbe.Rot;
            labelPunkteClick(me, lbl, f);
        }

        private void label26_Click(object sender, EventArgs e)
        {
            MeinZug = false;
            LegeKartenAufPunktestapel(Farbe.Gelb);
        }

        private void label25_Click(object sender, EventArgs e)
        {
            MeinZug = false;
            LegeKartenAufPunktestapel(Farbe.Blau);
        }

        private void label24_Click(object sender, EventArgs e)
        {
            MeinZug = false;
            LegeKartenAufPunktestapel(Farbe.Weiß);
        }

        private void label23_Click(object sender, EventArgs e)
        {
            MeinZug = false;
            LegeKartenAufPunktestapel(Farbe.Grün);
        }

        private void label22_Click(object sender, EventArgs e)
        {
            MeinZug = false;
            LegeKartenAufPunktestapel(Farbe.Rot);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            weiter = true;
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
