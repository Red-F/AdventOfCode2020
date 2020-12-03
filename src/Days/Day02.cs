using System.Linq;

namespace AdventOfCode.Days
{
    [Day(2020, 2)]
    public class Day02 : BaseDay
    {
        public override string PartOne(string input)
        {
            // parse lines into tuples with relevant values
            var items = input.ParseStrings(ParseOneString);
            // count number of valid tuples
            return items.Count(ValidatePassword1).ToString();
        }

        public override string PartTwo(string input)
        {
            // parse lines into tuples with relevant values
            var items = input.ParseStrings(ParseOneString);
            // count number of valid tuples
            return items.Count(ValidatePassword2).ToString();
        }

        private static (int minimum, int maximum, char requiredCharacter, string password) ParseOneString(string arg)
        {
            // split each line into:
            // 1
            // 3
            // a
            //
            // abcde
            var parts = arg.Split(new[] {'-', ' ', ':'});
            // Construct result tuple
            return (int.Parse(parts[0]), int.Parse(parts[1]), parts[2][0], parts[4]);
        }

        private static bool ValidatePassword1(
            (int minimum, int maximum, char requiredCharacter, string password) valueTuple)
        {
            var (minimum, maximum, requiredCharacter, password) = valueTuple;
            // count the number of required characters present in the password
            var requiredCharacterCount = password.Count(c => c == requiredCharacter);
            // check
            return requiredCharacterCount >= minimum && requiredCharacterCount <= maximum;
        }

        private static bool ValidatePassword2(
            (int minimum, int maximum, char requiredCharacter, string password) valueTuple)
        {
            var (minimum, maximum, requiredCharacter, password) = valueTuple;
            // count the number of required characters present in the password
            var requiredCharacterCount =
                password[minimum - 1] == requiredCharacter ? 1 : 0;
            requiredCharacterCount +=
                password[maximum - 1] == requiredCharacter ? 1 : 0;
            // check
            return requiredCharacterCount == 1;
        }
    }
}