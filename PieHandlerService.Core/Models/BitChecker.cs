namespace PieHandlerService.Core.Models;

/// <summary>
/// Used to check bit values of an arbitrary number
/// </summary>
public sealed class BitChecker
{
    private bool[] BoolRepresentation { get; } = new bool[0];

    public BitChecker(string hexRepresentation)
    {
        if (!string.IsNullOrWhiteSpace(hexRepresentation))
        {
            BoolRepresentation =
                string.Join(string.Empty, hexRepresentation
                        .Select(c => Convert.ToString(Convert.ToInt32(c.ToString(), 16), 2)
                            .PadLeft(4, '0')))
                    .Reverse()
                    .Select(x => x == '1').ToArray();
        }
    }

    public bool IsTrue(int index)
    {
        return
            index < BoolRepresentation.Length &&
            BoolRepresentation[index];
    }

    public bool IsEmpty()
    {
        return Length == 0;
    }

    public int Length => BoolRepresentation.Length;
}