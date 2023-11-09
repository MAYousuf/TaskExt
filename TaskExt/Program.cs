// See https://aka.ms/new-console-template for more information
using System.Runtime.CompilerServices;
using TaskExt;

Console.WriteLine("Hello, World!");
Console.WriteLine(TimeProvider.System.GetLocalNow());
var tasks = await (GetData(), GetData(), GetData2());

Console.WriteLine(TimeProvider.System.GetLocalNow());

//Console.ReadLine();
async Task<int> GetData()
{
    await Task.Delay(2000);
    throw new Exception("test exception");
    return 0;
}

async Task<int> GetData2()
{
    await Task.Delay(2000);
    throw new ArgumentException("ArgumentException");
    return 0;
}
