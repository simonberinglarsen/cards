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