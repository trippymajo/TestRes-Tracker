namespace TrtParserService.ParserCore
{
    [Flags]
    public enum ParserExtension
    {
        Unknown = 0,
        Trx = 1 << 0,
        //JUnit = 1 << 1,
        //NUnit = 1 << 2,
        //XUnit = 1 << 3,
        //Xml = 1 << 4,
    }
}
