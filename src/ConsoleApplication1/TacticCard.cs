namespace ConsoleApplication1
{
    public class TacticCard
    {
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public string SolutionText { get; set; }
        public string PuzzleText { get; set; }
        public CardData Data { get; set; }
        public string CornerText { get; set; }
    }

    public class CardData
    {
        public int FullMovesToMate { get; set; }
        public string WinningMoveLan { get; set; }
        public string WinningMoveSan { get; internal set; }
        public string Fen { get; set; }
        public bool WhiteToMove { get; set; }
        public SolutionType Solution { get; set; }
        public char WinningPieceUpper { get; set; }
        public bool IsCapture { get; set; }
        public int ScoreCP { get; internal set; }
        public int ImprovementCP { get; internal set; }
        public ScoreType ScoreType { get; set; }
        public bool MultipleSolutions { get; set; }
    }




}