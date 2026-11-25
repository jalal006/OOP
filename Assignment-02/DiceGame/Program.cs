Random random = new Random();

int myRoll = random.Next(1, 7);
int computerRoll = random.Next(1, 7);

while (myRoll == computerRoll)
{
    Console.WriteLine("Your roll: " + myRoll);
    Console.WriteLine("Computer roll: " + computerRoll);
    Console.WriteLine("Tie! Rolling again...");
    Console.WriteLine();

    myRoll = random.Next(1, 7);
    computerRoll = random.Next(1, 7);
}

Console.WriteLine("Your roll: " + myRoll);
Console.WriteLine("Computer roll: " + computerRoll);

if (myRoll > computerRoll)
{
    Console.WriteLine("You win!");
}
else
{
    Console.WriteLine("Computer wins!");
}
