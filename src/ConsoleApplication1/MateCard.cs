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
    }

    public class Ability
    {
        public AbilityTypeEnum AbilityType { get; set; }
        public string Text { get; set; }


        private static Dictionary<AbilityTypeEnum, string> texts = new Dictionary<AbilityTypeEnum, string>()
            {
                { AbilityTypeEnum.FileAOrRank1, ""},
                { AbilityTypeEnum.FileBOrRank2, ""},
                { AbilityTypeEnum.FileCOrRank3, ""},
                { AbilityTypeEnum.FileDOrRank4, ""},
                { AbilityTypeEnum.FileEOrRank5, ""},
                { AbilityTypeEnum.FileFOrRank6, ""},
                { AbilityTypeEnum.FileGOrRank7, ""},
                { AbilityTypeEnum.FileHOrRank8, ""},
                { AbilityTypeEnum.DiagonalA1ToA8, ""},
                { AbilityTypeEnum.DiagonalH1ToH8, ""},
                { AbilityTypeEnum.PieceIsPawn, ""},
                { AbilityTypeEnum.PieceIsRook, ""},
                { AbilityTypeEnum.PieceIsKnight, ""},
                { AbilityTypeEnum.PieceIsBishop, ""},
                { AbilityTypeEnum.PieceIsQueen, ""}
            };

        public static Ability Create(AbilityTypeEnum type)
        {
            var a = new Ability();
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