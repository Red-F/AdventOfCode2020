using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace AdventOfCode.Days
{
    [Day(2020, 25)]
    public class Day25 : BaseDay
    {
        public override string PartOne(string input) =>
            ReverseEngineerEncryptionKey(input.Longs().ToArray()).ToString();

        public override string PartTwo(string input) => "unused!";

        private static long ReverseEngineerEncryptionKey(IReadOnlyList<long> publicKeys)
        {
            var cardPk = publicKeys[0];
            var doorPk = publicKeys[1];
            var nrOfCardLoops = ReverseEngineerLoops(7, cardPk);
            var nrOfDoorLoops = ReverseEngineerLoops(7, doorPk);
            var encKeyDoor = Transform(cardPk, 1, nrOfDoorLoops);
            var encKeyCard = Transform(doorPk, 1, nrOfCardLoops);
            Debug.Assert(encKeyCard == encKeyDoor);
            return encKeyCard;
        }

        private static long ReverseEngineerLoops(long subjectNr, in long pk)
        {
            var v = 1L;
            var loopCount = 0L;
            while (v != pk)
            {
                v = Transform(subjectNr, v);
                loopCount++;
            }
            return loopCount;
        }

        private static long Transform(long subjectNr, long v, long count = 1)
        {
            for (var i = 0; i < count; i++) v = (v * subjectNr) % 20201227;
            return v;
        }
    }
}