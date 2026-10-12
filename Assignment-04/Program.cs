using BankApp.Classes;

try
{
    var b = new BankAccount("Savings", 300);
    b.Deposit(200);
    b.Withdraw(100);
    Console.WriteLine(b.Name);
    Console.WriteLine(b.Balance);
    Console.WriteLine(b.HasPositiveBalance);
}
catch (ArgumentException a)
{
    Console.WriteLine($"error: {a.Message}");
}
