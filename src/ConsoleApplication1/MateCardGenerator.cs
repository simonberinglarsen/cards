using System.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace ConsoleApplication1
{
    internal class MateCardGenerator
    {
        private StockFishProxy _stockFish = new StockFishProxy();
        List<PgnParser> _pgnList = new List<PgnParser>();
        public MateCardGenerator(string inputfile)
        {
            // read pgns
            string allPgns = File.ReadAllText(inputfile);
            // chop it
            string[] pgns = PgnParser.ChopIt(allPgns);
            foreach (var pgn in pgns)
            {
                PgnParser parser = new PgnParser(pgn);
                parser.Parse();
                _pgnList.Add(parser);
            }
        }

        internal MateCard[] GenerateMateIn(bool useCached)
        {
            string jsonFilename = "matecards.json";
            if (useCached && File.Exists(jsonFilename))
            {
                return JsonConvert.DeserializeObject<MateCard[]>(File.ReadAllText(jsonFilename));
            }
            List<MateCard> allMates = new List<MateCard>();
            foreach (var pgn in _pgnList.Skip(0))
            {

                // find if we have a mate in 1
                string notation = pgn.Game;
                string[] moves = notation.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                Engine e = new Engine();
                StringBuilder allMovesInLan = new StringBuilder();
                for (int i = 0; i < moves.Length; i++)
                {
                    if (_pgnList.IndexOf(pgn) == 19 && i == 106)
                    {
                        int debugfail = 1;
                    }
                    // get move
                    string currentMove = moves[i].Replace("+", "").Replace("#", "");
                    if (currentMove.IndexOf("1/2-1/2") >= 0
                        || currentMove.IndexOf("1-0") >= 0
                        || currentMove.IndexOf("0-1") >= 0)
                    {
                        // game over
                        break;
                    }
                    if (i % 3 == 0)
                        continue;
                    var generatedMoves = e.GenerateMoves();
                    var generatedMovesAsSan = e.PrintAsSan(generatedMoves);
                    var moveIndex = generatedMovesAsSan.ToList().IndexOf(currentMove);
                    string moveAsLan = e.PrintMove(generatedMoves[moveIndex]);

                    // ask stockfish if mate in X
                    MateCard mateCard = _stockFish.FindMate(allMovesInLan, 3);
                    if (mateCard != null)
                    {
                        int eval = e.Evaluate();
                        // only take puzzles having close to equal material
                        if (Math.Abs(eval) <= 100)
                        {
                            // find shortnotation
                            var generatedMovesAsLan = e.PrintMoves(generatedMoves).ToList();
                            var winningMoveIndex = generatedMovesAsLan.ToList().IndexOf(mateCard.WinningMoveLan);
                            string winningMoveSan = generatedMovesAsSan[winningMoveIndex];
                            mateCard.WinningMoveSan = winningMoveSan;
                            mateCard.WinningPieceUpper = char.ToUpper((char)generatedMoves[moveIndex][5]);
                            mateCard.IsCapture = winningMoveSan.Contains("x");
                            mateCard.Fen = e.PrintFen();
                            mateCard.WhiteToMove = e.ActiveColorIsWhite;
                            allMates.Add(mateCard);
                        }

                        System.Diagnostics.Debug.WriteLine($"{(_pgnList.IndexOf(pgn) * 100.0 / _pgnList.Count),3:0.#}% complete, game#{_pgnList.IndexOf(pgn)}/{_pgnList.Count}, mates found: {allMates.Count} (last mate in {((allMates.Count != 0) ? allMates.Last().FullMoves.ToString() : " - ")})");
                    }

                    // do move and continue searching for mate
                    e.DoMove(generatedMoves[moveIndex]);
                    allMovesInLan.Append(moveAsLan + " ");

                    // validate position with stockfish
                    var fen1 = e.PrintFen();
                    var fen2 = _stockFish.FenAfterMoves(allMovesInLan.ToString());
                    if (fen1 != fen2)
                    {
                        // error
                        throw new Exception("oh no! seems to be a bug in engine!?!?");
                    }
                }
            }
            File.WriteAllText(jsonFilename, JsonConvert.SerializeObject(allMates.ToArray()));
            return allMates.ToArray();
        }

        public void PostProcess(MateCard[] mateCards)
        {
            Dictionary<AbilityTypeEnum, int> setup = new Dictionary<AbilityTypeEnum, int>()
            {
                { AbilityTypeEnum.FileAOrRank1, 4},
                { AbilityTypeEnum.FileBOrRank2, 4},
                { AbilityTypeEnum.FileCOrRank3, 4},
                { AbilityTypeEnum.FileDOrRank4, 4},
                { AbilityTypeEnum.FileEOrRank5, 4},
                { AbilityTypeEnum.FileFOrRank6, 4},
                { AbilityTypeEnum.FileGOrRank7, 4},
                { AbilityTypeEnum.FileHOrRank8, 4},
                { AbilityTypeEnum.DiagonalA1ToA8, 2},
                { AbilityTypeEnum.DiagonalH1ToH8, 2},
                { AbilityTypeEnum.PieceIsPawn, 2},
                { AbilityTypeEnum.PieceIsRook, 4},
                { AbilityTypeEnum.PieceIsKnight, 4},
                { AbilityTypeEnum.PieceIsBishop, 4},
                { AbilityTypeEnum.PieceIsQueen, 4}
            };
            // verify 
            var sum = setup.Sum(s => s.Value);
            if (sum != 54) throw new Exception("Wrong deck size!");
            if (mateCards.Length != 54) throw new Exception("Wrong deck size!");
            List<Ability> abilities = new List<Ability>();
            foreach (var keyPair in setup)
            {
                for (int i = 0; i < keyPair.Value; i++)
                    abilities.Add(Ability.Create(keyPair.Key));
            }
            abilities.ForEach(a => mateCards[abilities.IndexOf(a)].Ability = a);

            // add titles
            for (int i = 0; i < 54; i++)
            {
                string[] fideInfoParts = fideInfo[i].Split(new char[] { ';' });
                string[] nameParts = fideInfoParts[0].Split(new char[] { ',' });
                string fullname = nameParts[1].Trim() + " " + nameParts[0].Trim();
                mateCards[i].Title = nameParts[0] + $" ({i + 1})";
                mateCards[i].Subtitle = $"{fullname} (Rating: {fideInfoParts[1]})";
            }
        }

        string[] fideInfo = new string[] {
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
    }

}