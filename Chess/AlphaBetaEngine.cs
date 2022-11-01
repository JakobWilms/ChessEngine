using Chess.Book;
using static Chess.CDisplay;
using static Chess.CMoveGeneration;

namespace Chess;

public abstract class AlphaBetaEngine : Engine
{
    //private readonly Random _random;
    private readonly TranspositionTable _tt;

    //private bool _betaCutoff;
    private int _depth, _nodes;
    private long _stopTime;
    private bool _timeOver;
    private CMove? _bestMove;

    protected AlphaBetaEngine()
    {
        //_random = new Random();
        _tt = new TranspositionTable();
    }

    public abstract int Evaluate(CBoard board);

    protected override CMove? FindMove(CBookEntry? entry = null)
    {
        //return RootSearch(Instance.Board);
        CMove? bookMove = entry?.RandomMove(Instance.Board);
        return bookMove ?? AlphaBetaRoot(Instance.Board.Copy(), 4, null);
    }


    private CMove? RootSearch(CBoard board)
    {
        _bestMove = null;
        _nodes = 0;
        _depth = 0;
        _stopTime = DateTimeOffset.Now.ToUnixTimeMilliseconds() + 10000;
        _timeOver = false;
        for (;; _depth++)
        {
            if (_timeOver) break;
            Instance.Window.DepthTextBlock.Text = _depth.ToString();
            CMove? move = AlphaBetaRoot(board, _depth, _bestMove);
            if (move != null) _bestMove = move;
        }

        return _bestMove;
    }

    private CMove? AlphaBetaRoot(CBoard board, int depthLeft, CMove? bestMove)
    {
        int alpha = MinValue, bestScore = int.MinValue;
        //List<CMove> bestMoves = new List<CMove>();
        CMove? ourBestMove = null;
        CMove?[] moves = Sort(MoveGen(board), bestMove);
        for (var index = 0; index < moves.Length; index++)
        {
            var move = moves[index];
            if (move == null) break;
            int score = int.MinValue;
            //_betaCutoff = false;
            move.Make(board);
            Console.Write($"{move.ToSan(moves)}: ");

            PositionInformation? information = _tt.Get(board.GetHash());
            if (information != null)
            {
                if (information.Depth >= depthLeft)
                {
                    score = information.Score;
                }
            }
            if (score == int.MinValue)
            {
                score = -AlphaBeta(MinValue, -alpha, depthLeft, board);
                //if (score == int.MinValue) return null;
                _tt.Set(board.GetHash(), new PositionInformation(null, depthLeft, score, NodeType.All));
            }

            Console.Write($"s {score}; ");
            move.Unmake(board);
            if (score <= bestScore) continue;
            bestScore = score;
            alpha = score;
            ourBestMove = move;
        }

        _tt.Set(board.GetHash(),
            new PositionInformation(ourBestMove, depthLeft, bestScore, NodeType.Pv));

        Console.WriteLine();
        return ourBestMove;
/*
        return bestMoves.Count switch
        {
            0 => null,
            1 => bestMoves[0],
            _ => bestMoves[_random.Next(bestMoves.Count)]
        };
*/
    }

    private int AlphaBeta(int alpha, int beta, int depthLeft, CBoard board)
    {
        _nodes++;
        if (depthLeft == 0) return Evaluate(board);
        CMove?[] moves = Sort(MoveGen(board), null);
        if (MoveGenCount(moves) == 0) return board.InCheck(board.ToMove) ? MinValue : 0;
        // if ((++_nodes & 0xfff) == 0)
        // {
        //     Instance.Window.NodesTextBlock.Text = _nodes.ToString();
        //     if (!_timeOver)
        //         _timeOver = DateTimeOffset.Now.ToUnixTimeMilliseconds() >= _stopTime;
        // }

        //if (_timeOver) return int.MinValue;
        for (var index = 0; index < moves.Length; index++)
        {
            var move = moves[index];
            if (move == null) break;
            int score = int.MinValue;
            move.Make(board);
            PositionInformation? information = _tt.Get(board.GetHash());
             if (information != null)
             {
                 if (information.Depth >= depthLeft)
                 {
                     score = information.Score;
                 }
             }

            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (score == int.MinValue)
            {
                score = -AlphaBeta(-beta, -alpha, depthLeft - 1, board);
                _tt.Set(board.GetHash(), new PositionInformation(null, depthLeft, score, NodeType.All));
            }

            move.Unmake(board);
            // if (score > beta)
            //     return beta;
            // else if (score == beta)
            // {
            //     _betaCutoff = true;
            //     return beta;
            // }
            if (score >= beta) return beta;
            if (score > alpha) alpha = score;
        }

        //Console.Write($"a {alpha}, ");
        return alpha;
    }
}