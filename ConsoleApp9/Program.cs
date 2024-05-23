using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace ChessGame
{
    class PawnPromotion
    {
        public static char PromotePawn(List<char> choosePieces)
        {
            Console.WriteLine("Выберите фигуру, на которую вы хотите превратить пешку:");


            for (int i = 0; i < choosePieces.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {choosePieces[i]}");
            }

            int choice = 0;
            while (true)
            {
                Console.Write("Введите номер фигуры: ");
                if (int.TryParse(Console.ReadLine(), out choice) && choice >= 1 && choice <= 5)
                {

                    break;
                }
                else
                {
                    Console.WriteLine("Неверный ввод. Попробуйте еще раз.");
                }
            }


            return choosePieces[choice - 1];
        }

    }
    class Field
    {

        public int len_X;
        public int len_Y;
        public char[,] board;
        public Dictionary<char, int> capturedWhitePieces = new Dictionary<char, int>();
        public Dictionary<char, int> capturedBlackPieces = new Dictionary<char, int>();

        public Field(int len_X = 8, int len_Y = 8)
        {
            this.len_X = len_X;
            this.len_Y = len_Y;
            this.board = new char[len_X, len_Y];
            capturedWhitePieces.Add('P', 0); capturedBlackPieces.Add('p', 0);
            capturedWhitePieces.Add('N', 0); capturedBlackPieces.Add('n', 0);
            capturedWhitePieces.Add('B', 0); capturedBlackPieces.Add('b', 0);
            capturedWhitePieces.Add('R', 0); capturedBlackPieces.Add('r', 0);
            capturedWhitePieces.Add('Q', 0); capturedBlackPieces.Add('q', 0);


        }

        public void Print()
        {
            Console.Clear();
            Console.WriteLine("  a b c d e f g h");
            Console.WriteLine("  --------------");
            for (int i = 0; i < 8; i++)
            {
                Console.Write(8 - i + "|");
                for (int j = 0; j < 8; j++)
                {
                    if (char.IsLower(this.board[i, j]))
                    {
                        Console.ForegroundColor = ConsoleColor.DarkBlue;
                    }
                    Console.Write(this.board[i, j] + " ");
                    Console.ResetColor();
                }
                Console.WriteLine("|" + (8 - i));
            }
            Console.WriteLine("  ---------------");
            Console.WriteLine("  a b c d e f g h");
            Console.WriteLine("Captured White Pieces:");
            foreach (var pair in capturedWhitePieces)
            {
                Console.WriteLine($"{pair.Key}: {pair.Value}");
            }
            Console.WriteLine("Captured Black Pieces:");
            foreach (var pair in capturedBlackPieces)
            {
                Console.WriteLine($"{pair.Key}: {pair.Value}");
            }
        }
    }

    class PieceLogic
    {
        public static bool CanMoveBluePiece(char piece, bool isPlayer1Turn)
        {
            bool isBlue = char.IsLower(piece);

            return (isPlayer1Turn && !isBlue) || (!isPlayer1Turn && isBlue);
        }

        public static bool MovePiece(char[,] board, int startX, int startY, int destX, int destY, Dictionary<char, int> capturedWhitePieces, Dictionary<char, int> capturedBlackPieces, bool isPlayer1Turn)
        {
            char piece = board[startX, startY];
            if (piece == ' ') return false;

            if (!CanMoveBluePiece(piece, isPlayer1Turn))
            {
                Console.WriteLine("Вы не можете ходить этой фигурой. Попробуйте еще раз.");
                return false;
            }

            if (!IsValidMove(piece, startX, startY, destX, destY, board))
                return false;

            if (!IsDestColorValid(board, destX, destY, char.IsUpper(piece)))
            {
                return false;
            }

            char destPiece = board[destX, destY];
            if (char.IsUpper(destPiece) || char.IsUpper(destPiece))
            {
                char capturedPieceKey = Char.ToUpper(destPiece);
                if (capturedWhitePieces.ContainsKey(capturedPieceKey))
                {
                    capturedWhitePieces[capturedPieceKey]++;
                }
                else
                {
                    capturedWhitePieces[capturedPieceKey] = 1;
                }
            }

            char destPiec = board[destX, destY];
            if (char.IsLower(destPiec) || char.IsLower(destPiec))
            {
                char capturedPieceKey = Char.ToLower(destPiec);
                if (capturedBlackPieces.ContainsKey(capturedPieceKey))
                {
                    capturedBlackPieces[capturedPieceKey]++;
                }
                else
                {
                    capturedBlackPieces[capturedPieceKey] = 1;
                }
            }

            if (char.ToLower(piece) == 'p' && (destX == 0 || destX == 7))
            {
                char promotedPiece;
                if (char.IsUpper(piece))
                {
                    promotedPiece = Char.ToUpper(PawnPromotion.PromotePawn(capturedWhitePieces.Keys.ToList()));
                }
                else
                {
                    promotedPiece = Char.ToLower(PawnPromotion.PromotePawn(capturedBlackPieces.Keys.ToList()));
                }
                board[destX, destY] = promotedPiece;
            }
            else
            {
                board[destX, destY] = piece;
            }



            board[startX, startY] = ' ';

            return true;
        }


        public static bool IsDestColorValid(char[,] board, int destX, int destY, bool isPlayer1Turn)
        {
            char destPiece = board[destX, destY];
            bool isWhite = char.IsUpper(destPiece);
            bool isBlack = char.IsLower(destPiece);

            return (isPlayer1Turn && isBlack) || (!isPlayer1Turn && isWhite) || destPiece == ' ';
        }


        private static bool IsValidMove(char piece, int startX, int startY, int destX, int destY, char[,] board)
        {
            try
            {
                char startPiece = board[startX, startY];

                bool isWhite = char.IsUpper(startPiece);
                bool isBlack = char.IsLower(startPiece);

                // Проверка для пешки
                if (char.ToLower(piece) == 'p')
                {


                    // Проверка для белых пешек
                    if (isWhite)
                    {

                        // Диагональные атаки для белых пешек
                        if (startX - destX == 1 && Math.Abs(startY - destY) == 1 && char.IsLower(board[destX, destY]))
                        {
                            return true;
                        }
                        // Обычный ход для белых пешек
                        if (startX == 6 && destX == 4 && startY == destY && board[5, startY] == ' ' && board[4, startY] == ' ')
                        {
                            return true;
                        }
                        if (isWhite && startX - destX == 1 && startY == destY && board[destX, destY] == ' ')
                        {
                            return true;
                        }
                        // если перед тобой фигура, то ты не можешь через нее пройти
                        if (isWhite && startY == destY && board[destX, destY] != ' ')
                        {
                            return false;
                        }
                    }
                    // Проверка для черных пешек
                    else if (isBlack)
                    {
                        // Диагональные атаки для черных пешек
                        if (destX - startX == 1 && Math.Abs(startY - destY) == 1 && char.IsUpper(board[destX, destY]))
                        {
                            return true;
                        }
                        // Обычный ход для черных пешек
                        if (startX == 1 && destX == 3 && startY == destY && board[2, startY] == ' ' && board[3, startY] == ' ')
                        {
                            return true;
                        }
                        // если перед тобой фигура, то ты не можешь через нее пройти
                        if (isBlack && startY == destY && board[destX, destY] != ' ')
                        {
                            return false;
                        }
                        else if (isBlack && destX - startX == 1 && startY == destY && board[destX, destY] == ' ')
                        {
                            return true;
                        }
                    }
                }
                // Проверка для коня
                else if (char.ToLower(piece) == 'n')
                {
                    // Ход конем - L-образный ход
                    int dx = Math.Abs(startX - destX);
                    int dy = Math.Abs(startY - destY);
                    return (dx == 2 && dy == 1) || (dx == 1 && dy == 2);
                }
                // Проверка для слона
                else if (char.ToLower(piece) == 'b')
                {
                    // Слон может двигаться по диагонали

                    int dx = destX - startX;
                    int dy = destY - startY;
                    int stepX = dx > 0 ? 1 : -1;
                    int stepY = dy > 0 ? 1 : -1;

                    for (int x = startX + stepX, y = startY + stepY; x != destX; x += stepX, y += stepY)
                    {
                        if (board[x, y] != ' ')
                        {
                            return false;
                        }
                    }
                    return dx == dy;


                }
                // Проверка для ладьи
                else if (char.ToLower(piece) == 'r')
                {
                    // Ладья может двигаться по вертикали или горизонтали
                    if (startX == destX)
                    {
                        int step = startY < destY ? 1 : -1;
                        for (int y = startY + step; y != destY; y += step)
                        {
                            if (board[startX, y] != ' ')
                            {
                                return false;
                            }
                        }
                    }
                    else if (startY == destY)
                    {
                        int step = startX < destX ? 1 : -1;
                        for (int x = startX + step; x != destX; x += step)
                        {
                            if (board[x, startY] != ' ')
                            {
                                return false;
                            }
                        }
                    }
                    return startX == destX || startY == destY;


                }
                // Проверка для ферзя
                else if (char.ToLower(piece) == 'q')
                {
                    // Ферзь может двигаться по вертикали, горизонтали или диагонали
                    int dx = Math.Abs(destX - startX);
                    int dy = Math.Abs(destY - startY);


                    int stepX = dx > 0 ? 1 : -1;
                    int stepY = dy > 0 ? 1 : -1;

                    if (dx == dy)
                    {
                        for (int x = startX + stepX, y = startY + stepY; x != destX; x += stepX, y += stepY)
                        {
                            if (board[x, y] != ' ')
                            {
                                return false;
                            }
                        }
                        return true;
                    }


                    else if (startX == destX || startY == destY)
                    {
                        if (startX == destX) // Горизонт
                        {
                            int step = (startY < destY) ? 1 : -1;
                            for (int y = startY + step; y != destY; y += step)
                            {
                                if (board[startX, y] != ' ')
                                {
                                    return false;
                                }
                            }
                        }
                        else // Вертикаль
                        {
                            int step = (startX < destX) ? 1 : -1;
                            for (int x = startX + step; x != destX; x += step)
                            {
                                if (board[x, startY] != ' ')
                                {
                                    return false;
                                }
                            }
                        }
                        return true;
                    }

                    return true;

                }
                // Проверка для короля
                else if (char.ToLower(piece) == 'k')
                {
                    // Король может двигаться на одну клетку в любом направлении
                    int dx = Math.Abs(startX - destX);
                    int dy = Math.Abs(startY - destY);
                    return dx <= 1 && dy <= 1;
                }

                return false;
            }
            catch
            {
                Console.WriteLine("Неверный выбор позиции");
                return false;
            }
        }



    }



    internal class Program
    {
        static bool isPlayer1Turn = true;

        static Dictionary<char, int> columnMap = new Dictionary<char, int>
        {
            {'a', 0}, {'b', 1}, {'c', 2}, {'d', 3}, {'e', 4}, {'f', 5}, {'g', 6}, {'h', 7}
        };

        static void InitializeBoard(char[,] board)
        {

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    board[i, j] = ' ';
                }
            }

            // Черные фигуры
            board[0, 0] = 'r'; // Ладья
            board[0, 1] = 'n'; // Конь
            board[0, 2] = 'b'; // Слон
            board[0, 3] = 'q'; // Ферзь
            board[0, 4] = 'k'; // Король
            board[0, 5] = 'b'; // Слон
            board[0, 6] = 'n'; // Конь
            board[0, 7] = 'r'; // Ладья
            for (int j = 0; j < 8; j++)
            {
                board[1, j] = 'p'; // Пешка
            }

            // Белые фигуры
            board[7, 0] = 'R'; // Ладья
            board[7, 1] = 'N'; // Конь
            board[7, 2] = 'B'; // Слон
            board[7, 3] = 'Q'; // Ферзь
            board[7, 4] = 'K'; // Король
            board[7, 5] = 'B'; // Слон
            board[7, 6] = 'N'; // Конь
            board[7, 7] = 'R'; // Ладья
            for (int j = 0; j < 8; j++)
            {
                board[6, j] = 'P'; // Пешка
            }
        }

        static bool IsGameOver(char[,] board)
        {
            bool whiteKingCaptured = true;
            bool blackKingCaptured = true;

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (board[i, j] == 'K')
                    {
                        whiteKingCaptured = false;
                    }
                    else if (board[i, j] == 'k')
                    {
                        blackKingCaptured = false;
                    }
                }
            }

            return whiteKingCaptured || blackKingCaptured;
        }


        static void Main(string[] args)
        {

            char[,] board = new char[8, 8];
            InitializeBoard(board);

            Field field = new Field();


            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    field.board[i, j] = board[i, j];
                }
            }


            field.Print();

            // ходы игроков
            while (true)
            {

                Console.WriteLine((isPlayer1Turn ? "Игрок 1" : "Игрок 2") + ", выберите фигуру, которой хотите сделать ход (например, a2):");
                string piecePosition = Console.ReadLine();

                char pieceColumn = piecePosition[0];
                int pieceRow = 8 - int.Parse(piecePosition.Substring(1, 1));
                try
                {
                    int pieceX = pieceRow;
                    int pieceY = columnMap[pieceColumn];

                    Console.WriteLine("Введите конечную позицию для выбранной фигуры (например, a4):");
                    string destPosition = Console.ReadLine();

                    char destColumn = destPosition[0];
                    int destRow = 8 - int.Parse(destPosition.Substring(1, 1));

                    int destX = destRow;
                    int destY = columnMap[destColumn];



                    if (PieceLogic.MovePiece(field.board, pieceX, pieceY, destX, destY, field.capturedWhitePieces, field.capturedBlackPieces, isPlayer1Turn))
                    {
                        field.Print();
                        isPlayer1Turn = !isPlayer1Turn;
                    }
                    else
                    {
                        Console.WriteLine("Неверный ход! Попробуй еще раз.");
                    }

                    if (IsGameOver(field.board))
                    {
                        Console.WriteLine((isPlayer1Turn ? "Игрок 2" : "Игрок 1") + " выиграл!");
                        break;
                    }

                }
                catch (Exception e) { Console.WriteLine("Неверный ход"); continue; }

            }



        }
    }
}