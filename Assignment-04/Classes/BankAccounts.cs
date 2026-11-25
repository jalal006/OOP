namespace BankApp.Classes;

public class BankAccount
{
    private string name;

    public string Name
    {
        get { return name; }
        set
        {
            if (!string.IsNullOrWhiteSpace(value))
                name = value;
            else
                throw new ArgumentException(nameof(value), "Name cannot be empty");
        }
    }

    private decimal balance;

    public decimal Balance
    {
        get => balance;
        set
        {
            if (value >= 0)
                balance = value;
            else
                throw new ArgumentOutOfRangeException(nameof(value), "Balance Cannot Be Negative");
        }
    }

    public bool HasPositiveBalance => (balance > 0);

    public string Number { get; set; }

    public BankAccount(string name, decimal balance)
    {
        Name = name;
        Balance = balance;
    }

    public BankAccount() : this("personal", 100)
    {
    }

    public void Deposit(decimal amount)
    {
        if (amount > 0)
            balance += amount;
    }

    public void Withdraw(decimal amount)
    {
        if (amount > 0)
            if (amount <= balance)
                balance -= amount;
    }
}
