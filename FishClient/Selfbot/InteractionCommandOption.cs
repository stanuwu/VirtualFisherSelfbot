namespace FishClient.Selfbot;

public class InteractionCommandOption
{
    public InteractionCommandOption(int type, string name, string value)
    {
        Type = type;
        Name = name;
        Value = value;
    }

    public int Type { get; }
    public string Name { get; }
    public string Value { get; }
}