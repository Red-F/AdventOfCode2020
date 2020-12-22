using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode.Days
{
    [Day(2020, 22)]
    public class Day22 : BaseDay
    {
        public override string PartOne(string input) => PlayGameOne(ParseDecks(input)).ToString();

        public override string PartTwo(string input) => PlayGameTwo(ParseDecks(input)).score.ToString();

        private static (int player, long score) PlayGameTwo(Queue<int>[] decks)
        {
            var game = new HashSet<string>(); 
            int winner;
            while (decks[0].Count > 0 && decks[1].Count > 0)
            {
                var hash = Hash(decks);
                if (game.Contains(hash)) return (0, CalculateScore(decks[0]));
                game.Add(hash);

                var player0 = decks[0].Dequeue();
                var player1 = decks[1].Dequeue();

                if (decks[0].Count >= player0 && decks[1].Count >= player1)
                    (winner, _) = PlayGameTwo(new Queue<int>[]
                        {new(decks[0].ToArray()[..player0]), new(decks[1].ToArray()[..player1])});
                else winner = player0 > player1 ? 0 : 1;

                if (winner == 0)
                {
                    decks[0].Enqueue(player0);
                    decks[0].Enqueue(player1);
                }
                else
                {
                    decks[1].Enqueue(player1);
                    decks[1].Enqueue(player0);
                }
            }

            winner = decks[0].Count > 0 ? 0 : 1;
            return (winner, CalculateScore(decks[winner]));
        }

        private static string Hash(Queue<int>[] decks) => string.Concat(
            string.Join(',', decks[0].ToArray().Select(x => x.ToString())), "+",
            string.Join(',', decks[1].ToArray().Select(x => x.ToString())));

        private static long PlayGameOne(Queue<int>[] decks)
        {
            while (decks[0].Count > 0 && decks[1].Count > 0)
            {
                var player0 = decks[0].Dequeue();
                var player1 = decks[1].Dequeue();
                if (player0 > player1)
                {
                    decks[0].Enqueue(player0);
                    decks[0].Enqueue(player1);
                }
                else
                {
                    decks[1].Enqueue(player1);
                    decks[1].Enqueue(player0);
                }
            }

            var winningDeck = decks[0].Count == 0 ? decks[1] : decks[0];
            return CalculateScore(winningDeck);
        }

        private static int CalculateScore(Queue<int> winningDeck)
        {
            var multiplier = winningDeck.Count;
            var rc = winningDeck.Sum(x => x * multiplier--);
            return rc;
        }

        private static Queue<int>[] ParseDecks(string input)
        {
            var groups = input.LineGroups(Environment.NewLine + Environment.NewLine).ToArray();
            var rc = new Queue<int>[2];
            for (var i = 0; i < 2; i++) rc[i] = new Queue<int>(groups[i].Lines().Skip(1).Select(int.Parse));
            return rc;
        }
    }
}
