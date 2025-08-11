using System;
using System.Collections.Generic;

// Record for transaction
public record Transaction(int Id, DateTime Date, decimal Amount, string Category);

// Interface
public interface ITransactionProcessor
{
    void Process(Transaction transaction);
}

// Implementations
public class BankTransferProcessor : ITransactionProcessor
{
    public void Process(Transaction transaction) =>
        Console.WriteLine($"[Bank Transfer] {transaction.Category}: {transaction.Amount:C} processed.");
}

public class MobileMoneyProcessor : ITransactionProcessor
{
    public void Process(Transaction transaction) =>
        Console.WriteLine($"[Mobile Money] {transaction.Category}: {transaction.Amount:C} processed.");
}

public class CryptoWalletProcessor : ITransactionProcessor
{
    public void Process(Transaction transaction) =>
        Console.WriteLine($"[Crypto Wallet] {transaction.Category}: {transaction.Amount:C} processed.");
}

// Base Account
public class Account
{
    public string AccountNumber { get; }
    public decimal Balance { get; protected set; }

    public Account(string accountNumber, decimal initialBalance)
    {
        AccountNumber = accountNumber;
        Balance = initialBalance;
    }

    public virtual void ApplyTransaction(Transaction transaction)
    {
        Balance -= transaction.Amount;
    }
}

// Sealed SavingsAccount
public sealed class SavingsAccount : Account
{
    public SavingsAccount(string accNo, decimal bal) : base(accNo, bal) { }

    public override void ApplyTransaction(Transaction transaction)
    {
        if (transaction.Amount > Balance)
            Console.WriteLine("Insufficient funds");
        else
        {
            Balance -= transaction.Amount;
            Console.WriteLine($"Transaction applied. New balance: {Balance:C}");
        }
    }
}

// App
public class FinanceApp
{
    private List<Transaction> _transactions = new();

    public void Run()
    {
        var account = new SavingsAccount("123456", 1000m);

        var t1 = new Transaction(1, DateTime.Now, 100m, "Groceries");
        var t2 = new Transaction(2, DateTime.Now, 200m, "Utilities");
        var t3 = new Transaction(3, DateTime.Now, 50m, "Entertainment");

        new MobileMoneyProcessor().Process(t1);
        account.ApplyTransaction(t1);

        new BankTransferProcessor().Process(t2);
        account.ApplyTransaction(t2);

        new CryptoWalletProcessor().Process(t3);
        account.ApplyTransaction(t3);

        _transactions.AddRange(new[] { t1, t2, t3 });
    }
}

class Program
{
    static void Main()
    {
        new FinanceApp().Run();
    }
}
