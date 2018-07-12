namespace FakeVerifit
{
    /// <summary>
    /// AudioscanAPI 3.0
    /// </summary>
    public interface IFakeVerifitCommandCreator
    {
        IFakeVerifitCommand Create(string command);
    }
}