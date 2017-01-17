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
    }

    public class Ability
    {
        public AbilityTypeEnum AbilityType { get; set; }
        public string Text { get; set; }


        private static Dictionary<AbilityTypeEnum, string> texts = new Dictionary<AbilityTypeEnum, string>()
            {
                { AbilityTypeEnum.FileAOrRank1, "Din modstanders udfordring l�ses ved at placere en brik p� A-linien eller 1. r�kke."},
                { AbilityTypeEnum.FileBOrRank2, "Din modstanders udfordring l�ses ved at placere en brik p� B-linien eller 2. r�kke."},
                { AbilityTypeEnum.FileCOrRank3, "Din modstanders udfordring l�ses ved at placere en brik p� C-linien eller 3. r�kke."},
                { AbilityTypeEnum.FileDOrRank4, "Din modstanders udfordring l�ses ved at placere en brik p� D-linien eller 4. r�kke."},
                { AbilityTypeEnum.FileEOrRank5, "Din modstanders udfordring l�ses ved at placere en brik p� E-linien eller 5. r�kke."},
                { AbilityTypeEnum.FileFOrRank6, "Din modstanders udfordring l�ses ved at placere en brik p� F-linien eller 6. r�kke."},
                { AbilityTypeEnum.FileGOrRank7, "Din modstanders udfordring l�ses ved at placere en brik p� G-linien eller 7. r�kke."},
                { AbilityTypeEnum.FileHOrRank8, "Din modstanders udfordring l�ses ved at placere en brik p� H-linien eller 8. r�kke."},
                { AbilityTypeEnum.DiagonalA1ToA8, "Din modstanders udfordring l�ses ved at placere en brik et sted p� diagonalen fra A1 til H8."},
                { AbilityTypeEnum.DiagonalH1ToH8, "Din modstanders udfordring l�ses ved at placere en brik et sted p� diagonalen fra H1 til A8."},
                { AbilityTypeEnum.PieceIsPawn, "Din modstanders udfordring l�ses ved at flytte en bonde."},
                { AbilityTypeEnum.PieceIsRook, "Din modstanders udfordring l�ses ved at flytte et t�rn."},
                { AbilityTypeEnum.PieceIsKnight, "Din modstanders udfordring l�ses ved at flytte en springer."},
                { AbilityTypeEnum.PieceIsBishop, "Din modstanders udfordring l�ses ved at flytte en l�ber."},
                { AbilityTypeEnum.PieceIsQueen, "Din modstanders udfordring l�ses ved at flytte en dronning."},
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