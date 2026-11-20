namespace ConnectFour;

public class GameState
{
    public const int Rows = 6;
    public const int Columns = 7;

    // board[row, col]: 0 = vacío, 1 = Jugador 1, 2 = Jugador 2
    private readonly int[,] _board = new int[Rows, Columns];
    private readonly int[] _columnHeights = new int[Columns];

    public int PlayerTurn { get; private set; } = 1;
    public int CurrentTurn { get; private set; } = 0;
    public int Player1WinStreak { get; private set; }
    public int Player2WinStreak { get; private set; }
    private bool _gameEnded;

    public enum WinState
    {
        None,
        Player1_Wins,
        Player2_Wins,
        Tie
    }

    public void ResetBoard()
    {
        Array.Clear(_board, 0, _board.Length);
        Array.Clear(_columnHeights, 0, _columnHeights.Length);
        PlayerTurn = 1;
        CurrentTurn = 0;
        _gameEnded = false;
    }

    // Devuelve la fila (1 a 6, acorde a las clases .drop1 .. .drop6 del CSS) donde cayó la ficha
    public int PlayPiece(byte col)
    {
        if (col >= Columns)
            throw new ArgumentException("Invalid column.", nameof(col));

        if (_columnHeights[col] >= Rows)
            throw new ArgumentException("Column is full.", nameof(col));

        // rowIndex 0 = arriba, 5 = abajo
        int rowIndex = Rows - 1 - _columnHeights[col];
        _board[rowIndex, col] = PlayerTurn;
        _columnHeights[col]++;

        CurrentTurn++;
        PlayerTurn = PlayerTurn == 1 ? 2 : 1;

        // drop1 es la fila superior, drop6 la fila inferior
        return rowIndex + 1;
    }

    public WinState CheckForWin()
    {
        // Revisar horizontal, vertical y diagonales
        for (int r = 0; r < Rows; r++)
        {
            for (int c = 0; c < Columns; c++)
            {
                int player = _board[r, c];
                if (player == 0) continue;

                if (HasFourInLine(r, c, 0, 1, player) ||   // Horizontal
                    HasFourInLine(r, c, 1, 0, player) ||   // Vertical
                    HasFourInLine(r, c, 1, 1, player) ||   // Diagonal abajo-derecha
                    HasFourInLine(r, c, 1, -1, player))    // Diagonal abajo-izquierda
                {
                    return RecordGameResult(player == 1 ? WinState.Player1_Wins : WinState.Player2_Wins);
                }
            }
        }

        if (CurrentTurn >= Rows * Columns)
            return RecordGameResult(WinState.Tie);

        return WinState.None;
    }

    private WinState RecordGameResult(WinState result)
    {
        if (!_gameEnded)
        {
            switch (result)
            {
                case WinState.Player1_Wins:
                    Player1WinStreak++;
                    Player2WinStreak = 0;
                    break;
                case WinState.Player2_Wins:
                    Player2WinStreak++;
                    Player1WinStreak = 0;
                    break;
                case WinState.Tie:
                    Player1WinStreak = 0;
                    Player2WinStreak = 0;
                    break;
            }

            _gameEnded = true;
        }

        return result;
    }

    private bool HasFourInLine(int startRow, int startCol, int dRow, int dCol, int player)
    {
        for (int step = 1; step < 4; step++)
        {
            int r = startRow + dRow * step;
            int c = startCol + dCol * step;

            if (r < 0 || r >= Rows || c < 0 || c >= Columns || _board[r, c] != player)
                return false;
        }
        return true;
    }
}