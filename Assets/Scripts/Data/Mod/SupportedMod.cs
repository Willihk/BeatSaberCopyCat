using MessagePack;

[MessagePackObject]
public struct SupportedMod
{
    [Key("Name")]
    public string ModName;
    [Key("Supported")]
    public byte Supported;
}
