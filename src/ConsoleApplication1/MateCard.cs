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
                { AbilityTypeEnum.FileA, "Din modstanders udfordring løses ved at placere en brik på A-linien"},
                { AbilityTypeEnum.FileB, "Din modstanders udfordring løses ved at placere en brik på B-linien"},
                { AbilityTypeEnum.FileC, "Din modstanders udfordring løses ved at placere en brik på C-linien"},
                { AbilityTypeEnum.FileD, "Din modstanders udfordring løses ved at placere en brik på D-linien"},
                { AbilityTypeEnum.FileE, "Din modstanders udfordring løses ved at placere en brik på E-linien"},
                { AbilityTypeEnum.FileF, "Din modstanders udfordring løses ved at placere en brik på F-linien"},
                { AbilityTypeEnum.FileG, "Din modstanders udfordring løses ved at placere en brik på G-linien"},
                { AbilityTypeEnum.FileH, "Din modstanders udfordring løses ved at placere en brik på H-linien"},
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
        FileA,
        FileB,
        FileC,
        FileD,
        FileE,
        FileF,
        FileG,
        FileH,
        PieceIsPawn,
        PieceIsRook,
        PieceIsKnight,
        PieceIsBishop,
        PieceIsQueen,
    };
}