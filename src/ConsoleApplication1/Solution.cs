using System.Collections.Generic;

namespace ConsoleApplication1
{
    public class Translator
    {
        public SolutionType SolutionType { get; set; }
        public string Text { get; set; }


        private static readonly Dictionary<SolutionType, string> SolutionTypeTexts = new Dictionary<SolutionType, string>()
        {
            { SolutionType.FileA, "Flyt til A-linien." },
            { SolutionType.FileB, "Flyt til B-linien." },
            { SolutionType.FileC, "Flyt til C-linien." },
            { SolutionType.FileD, "Flyt til D-linien." },
            { SolutionType.FileE, "Flyt til E-linien." },
            { SolutionType.FileF, "Flyt til F-linien." },
            { SolutionType.FileG, "Flyt til G-linien." },
            { SolutionType.FileH, "Flyt til H-linien." },
            { SolutionType.Rank1234, "Flyt til r�kke 1, 2, 3 eller 4." },
            { SolutionType.Rank5678, "Flyt til r�kke 5, 6, 7 eller 8." },
            { SolutionType.FileAbcd, "Flyt til A-, B-, C- eller D-linien." },
            { SolutionType.FileEfgh, "Flyt til E-, F-, G- eller H-linien." },
            { SolutionType.PieceIsPawn, "Flyt en bonde." },
            { SolutionType.PieceIsRook, "Flyt et t�rn." },
            { SolutionType.PieceIsKnight, "Flyt en springer." },
            { SolutionType.PieceIsBishop, "Flyt en l�ber." },
            { SolutionType.PieceIsQueen, "Flyt en dronning." },
        };

        private static readonly string[] FideInfo = new string[] {
            "Carlsen, Magnus;2840;1990",
            "Caruana, Fabiano;2827;1992",
            "Kramnik, Vladimir;2811;1975",
            "So, Wesley;2808;1993",
            "Vachier-Lagrave, Maxime;2796;1990",
            "Anand, Viswanathan;2786;1969",
            "Nakamura, Hikaru;2785;1987",
            "Karjakin, Sergey;2785;1990",
            "Aronian, Levon;2780;1982",
            "Giri, Anish;2773;1994",
            "Nepomniachtchi, Ian;2767;1990",
            "Harikrishna, P.;2766;1986",
            "Mamedyarov, Shakhriyar;2766;1985",
            "Ding, Liren;2760;1992",
            "Eljanov, Pavel;2755;1983",
            "Ivanchuk, Vassily;2752;1969",
            "Adams, Michael;2751;1971",
            "Wojtaszek, Radoslaw;2750;1987",
            "Svidler, Peter;2748;1976",
            "Grischuk, Alexander;2742;1983",
            "Topalov, Veselin;2739;1975",
            "Dominguez Perez, Leinier;2739;1983",
            "Yu, Yangyi;2738;1994",
            "Andreikin, Dmitry;2736;1990",
            "Navara, David;2735;1985",
            "Vitiugov, Nikita;2724;1987",
            "Inarkiev, Ernesto;2723;1985",
            "Gelfand, Boris;2721;1968",
            "Li, Chao b;2720;1989",
            "Le, Quang Liem;2718;1991",
            "Malakhov, Vladimir;2715;1980",
            "Bu, Xiangzhi;2711;1985",
            "Tomashevsky, Evgeny;2711;1987",
            "Radjabov, Teimour;2710;1987",
            "Jakovenko, Dmitry;2709;1983",
            "Vallejo Pons, Francisco;2709;1982",
            "Ponomariov, Ruslan;2709;1983",
            "Wei, Yi;2706;1999",
            "Wang, Yue;2706;1987",
            "Rapport, Richard;2702;1996",
            "Naiditsch, Arkadij;2702;1985",
            "Kryvoruchko, Yuriy;2701;1986",
            "Jobava, Baadur;2701;1983",
            "Matlakov, Maxim;2701;1991",
            "Kasimdzhanov, Rustam;2699;1979",
            "Almasi, Zoltan;2698;1976",
            "Ragger, Markus;2697;1988",
            "Bacrot, Etienne;2695;1983",
            "Van Wely, Loek;2695;1972",
            "Rodshtein, Maxim;2693;1989",
            "Leko, Peter;2693;1979",
            "Korobov, Anton;2689;1985",
            "Cheparinov, Ivan;2689;1986",
            "Mamedov, Rauf;2688;1988",
            "Safarli, Eltaj;2686;1992",
            "Kovalenko, Igor;2684;1988",
            "Duda, Jan-Krzysztof;2684;1998",
            "Shirov, Alexei;2683;1972",
            "Rublevsky, Sergei;2681;1974",
            "Nisipeanu, Liviu-Dieter;2680;1976",
            "Areshchenko, Alexander;2679;1986",
            "Najer, Evgeniy;2679;1977",
            "Zvjaginsev, Vadim;2679;1976",
            "Movsesian, Sergei;2676;1978",
            "Short, Nigel D;2675;1965",
            "Morozevich, Alexander;2675;1977",
            "Akopian, Vladimir;2675;1971",
            "Robson, Ray;2675;1994",
            "Ni, Hua;2674;1983",
            "Shankland, Samuel L;2674;1991",
            "Vidit, Santosh Gujrathi;2673;1994",
            "Sjugirov, Sanan;2673;1993",
            "Sadler, Matthew D;2672;1974",
            "Riazantsev, Alexander;2671;1985",
            "Wang, Hao;2670;1989",
            "Rakhmanov, Aleksandr;2670;1989",
            "Negi, Parimarjan;2670;1993",
            "Smirin, Ilia;2667;1968",
            "Onischuk, Alexander;2667;1975",
            "Xiong, Jeffery;2667;2000",
            "Sargissian, Gabriel;2667;1983",
            "Amin, Bassem;2666;1988",
            "Kamsky, Gata;2666;1974",
            "Bareev, Evgeny;2666;1966",
            "Jones, Gawain C B;2665;1987",
            "Markus, Robert;2665;1983",
            "Motylev, Alexander;2663;1979",
            "Zhigalko, Sergei;2662;1989",
            "Laznicka, Viktor;2662;1988",
            "Moiseenko, Alexander;2661;1980",
            "Dubov, Daniil;2661;1996",
            "Sasikiran, Krishnan;2661;1981",
            "Fressinet, Laurent;2660;1981",
            "Ipatov, Alexander;2660;1993",
            "Tkachiev, Vladislav;2660;1973",
            "Fedoseev, Vladimir;2658;1995",
            "Ganguly, Surya Shekhar;2657;1983",
            "Salem, A.R. Saleh;2656;1993",
            "Howell, David W L;2655;1990",
            "Cordova, Emilio;2655;1991",
            "Artemiev, Vladislav;2655;1998",
            };

        public static string SolutionTypeToText(SolutionType type)
        {
            return SolutionTypeTexts[type];
        }

        public static string TitleFromCardno(int i)
        {
            
            string[] parts = FideInfo[i].Split(new char[] { ';' });
            string[] nameParts = parts[0].Split(new char[] {','});
            string title = $"#{i + 1} " + nameParts[0];
            return title;

        }

        public static string SubtitleFromCardno(int i)
        {
            string[] parts = FideInfo[i].Split(new char[] { ';' });
            string[] nameParts = parts[0].Split(new char[] { ',' });
            string fullname = $"{nameParts[1]} {nameParts[0]}";
            return $"GM {fullname}, Rating: {parts[1]}";
        }
    }

    public enum SolutionType
    {
        FileA,
        FileB,
        FileC,
        FileD,
        FileE,
        FileF,
        FileG,
        FileH,
        FileAbcd,
        FileEfgh,
        Rank1234,
        Rank5678,
        PieceIsPawn,
        PieceIsRook,
        PieceIsKnight,
        PieceIsBishop,
        PieceIsQueen,
    };
}