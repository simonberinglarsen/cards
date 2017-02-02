using System;
using System.Collections.Generic;

namespace ConsoleApplication1
{
    public class Translator
    {
        public SolutionType SolutionType { get; set; }
        public string Text { get; set; }


        private static readonly Dictionary<SolutionType, string> SolutionTypeTexts = new Dictionary<SolutionType, string>()
        {
            { SolutionType.FileABCD, "SOLUTION_ABCD" },
            { SolutionType.FileABEF, "SOLUTION_ABEF" },
            { SolutionType.FileABGH, "SOLUTION_ABGH" },
            { SolutionType.FileCDEF, "SOLUTION_CDEF" },
            { SolutionType.FileCDGH, "SOLUTION_CDGH" },
            { SolutionType.FileEFGH, "SOLUTION_EFGH" },
            { SolutionType.FileAll, "SOLUTION_ALL" },
        };

        private static readonly Dictionary<SolutionType, string> CornerTexts = new Dictionary<SolutionType, string>()
        {
            { SolutionType.FileABCD, "ABCD" },
            { SolutionType.FileABEF, "ABEF" },
            { SolutionType.FileABGH, "ABGH" },
            { SolutionType.FileCDEF, "CDEF" },
            { SolutionType.FileCDGH, "CDGH" },
            { SolutionType.FileEFGH, "EFGH" },
            { SolutionType.FileAll, "****" },
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

        internal static string PuzzleTextFromWhiteToMove(bool whiteToMove)
        {
            if (whiteToMove)
                return "MATE_1_WHITE";
            else
                return "MATE_1_BLACK";
        }

        public static string SolutionTypeToText(SolutionType type)
        {
            return SolutionTypeTexts[type];
        }

        public static string TitleFromCardno(int i)
        {

            string[] parts = FideInfo[i].Split(new char[] { ';' });
            string[] nameParts = parts[0].Split(new char[] { ',' });
            string title = $"#{i + 1} " + nameParts[0];
            return title;

        }

        public static string NameFromCardno(int i)
        {

            string[] parts = FideInfo[i].Split(new char[] { ';' });
            string[] nameParts = parts[0].Split(new char[] { ',' });
            string title = $"{nameParts[1]} {nameParts[0]}";
            return title;
        }

        public static string SubtitleFromCardno(int i)
        {
            string[] parts = FideInfo[i].Split(new char[] { ';' });
            string[] nameParts = parts[0].Split(new char[] { ',' });
            string fullname = $"{nameParts[1]} {nameParts[0]}";
            return $"GM {fullname}, Rating: {parts[1]}";
        }

        public static string SolutionTypeToCornerText(SolutionType solutionType)
        {
            return CornerTexts[solutionType];
        }
    }

    public enum SolutionType
    {
        FileABCD,
        FileABEF,
        FileABGH,
        FileCDEF,
        FileCDGH,
        FileEFGH,
        FileAll,
    };

    public static class StringContainer
    {
        public enum LanguageEnum
        {
            DK,
            EN
        };

        public static LanguageEnum Language { get; set; }

        public static Dictionary<string, string> CurrentLanguage
        {
            get
            {
                return Texts[Language];
            }
        }

        private static Dictionary<LanguageEnum, Dictionary<string, string>> Texts = new Dictionary<LanguageEnum, Dictionary<string, string>>()
        {
            {
                LanguageEnum.EN,
                    new Dictionary<string, string>() {
                        { "MATE_1_WHITE", "Mate in 1. White to move."},
                        { "MATE_1_BLACK", "Mate in 1. Black to move."},
                        { "SOLUTION_ABCD", "Move to the A-, B-, C- or D-file."},
                        { "SOLUTION_ABEF", "Move to the A-, B-, E- or F-file."},
                        { "SOLUTION_ABGH", "Move to the A-, B-, G- or H-file."},
                        { "SOLUTION_CDEF", "Move to the C-, D-, E- or F-file."},
                        { "SOLUTION_CDGH", "Move to the C-, D-, G- or H-file."},
                        { "SOLUTION_EFGH", "Move to the E-, F-, G- or H-file."},
                        { "SOLUTION_ALL", "This card solves ALL puzzles!" },
                        { "SOLUTION_LABEL", "Solution:" },
                        { "PUZZLE_LABEL", "Puzzle:"},
                        { "FRONTCARD.SOLUTIONS", "Solutions"},
                        { "FRONTCARD.CHESSNOTATION", "how to read chess notation..."},

                    }
            },
            {
                LanguageEnum.DK,
                    new Dictionary<string, string>() {
                        { "MATE_1_WHITE", "Mat i 1. Hvid tr�kker."},
                        { "MATE_1_BLACK", "Mat i 1. Sort tr�kker."},
                        { "SOLUTION_ABCD", "Flyt til A-, B-, C- eller D-linien."},
                        { "SOLUTION_ABEF", "Flyt til A-, B-, E- eller F-linien."},
                        { "SOLUTION_ABGH", "Flyt til A-, B-, G- eller H-linien."},
                        { "SOLUTION_CDEF", "Flyt til C-, D-, E- eller F-linien."},
                        { "SOLUTION_CDGH", "Flyt til C-, D-, G- eller H-linien."},
                        { "SOLUTION_EFGH", "Flyt til E-, F-, G- eller H-linien."},
                        { "SOLUTION_ALL", "Kortet l�ser ALLE udfordringer!" },
                        { "SOLUTION_LABEL", "L�sning:" },
                        { "PUZZLE_LABEL", "Opgave:"},
                        { "FRONTCARD.SOLUTIONS", "L�sninger"},
                        { "FRONTCARD.CHESSNOTATION", "s�dan l�ses skak-notation..."},

                    }
            }

        };
    }
}