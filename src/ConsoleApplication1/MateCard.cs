using System.Collections.Generic;

namespace ConsoleApplication1
{
    public class MateCard
    {
        public int FullMoves { get; set; }
        public string WinningMoveLan { get; set; }
        public string WinningMoveSan { get; internal set; }
        public string Fen { get; set; }
        public bool WhiteToMove { get; set; }
        public Ability Ability { get; set; }
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public char WinningPieceUpper { get; set; }
        public bool IsCapture { get; set; }
    }

    public class Ability
    {
        public AbilityTypeEnum AbilityType { get; set; }
        public string Text { get; set; }


        private static Dictionary<AbilityTypeEnum, string> texts = new Dictionary<AbilityTypeEnum, string>()
            {
                { AbilityTypeEnum.FileAOrRank1, "Din modstanders udfordring løses ved at placere en brik på A-linien eller 1. række."},
                { AbilityTypeEnum.FileBOrRank2, "Din modstanders udfordring løses ved at placere en brik på B-linien eller 2. række."},
                { AbilityTypeEnum.FileCOrRank3, "Din modstanders udfordring løses ved at placere en brik på C-linien eller 3. række."},
                { AbilityTypeEnum.FileDOrRank4, "Din modstanders udfordring løses ved at placere en brik på D-linien eller 4. række."},
                { AbilityTypeEnum.FileEOrRank5, "Din modstanders udfordring løses ved at placere en brik på E-linien eller 5. række."},
                { AbilityTypeEnum.FileFOrRank6, "Din modstanders udfordring løses ved at placere en brik på F-linien eller 6. række."},
                { AbilityTypeEnum.FileGOrRank7, "Din modstanders udfordring løses ved at placere en brik på G-linien eller 7. række."},
                { AbilityTypeEnum.FileHOrRank8, "Din modstanders udfordring løses ved at placere en brik på H-linien eller 8. række."},
                { AbilityTypeEnum.DiagonalA1ToA8, "Din modstanders udfordring løses ved at placere en brik et sted på diagonalen fra A1 til H8."},
                { AbilityTypeEnum.DiagonalH1ToH8, "Din modstanders udfordring løses ved at placere en brik et sted på diagonalen fra H1 til A8."},
                { AbilityTypeEnum.PieceIsPawn, "Din modstanders udfordring løses ved at flytte en bonde."},
                { AbilityTypeEnum.PieceIsRook, "Din modstanders udfordring løses ved at flytte et tårn."},
                { AbilityTypeEnum.PieceIsKnight, "Din modstanders udfordring løses ved at flytte en springer."},
                { AbilityTypeEnum.PieceIsBishop, "Din modstanders udfordring løses ved at flytte en løber."},
                { AbilityTypeEnum.PieceIsQueen, "Din modstanders udfordring løses ved at flytte en dronning."},
            };

        public static Ability Create(AbilityTypeEnum type)
        {
            var a = new Ability();
            a.AbilityType = type;
            a.Text = texts[type];
            return a;
        }
    }

    public enum AbilityTypeEnum
    {
        FileAOrRank1,
        FileBOrRank2,
        FileCOrRank3,
        FileDOrRank4,
        FileEOrRank5,
        FileFOrRank6,
        FileGOrRank7,
        FileHOrRank8,
        DiagonalA1ToA8,
        DiagonalH1ToH8,
        PieceIsPawn,
        PieceIsRook,
        PieceIsKnight,
        PieceIsBishop,
        PieceIsQueen,
    };
}