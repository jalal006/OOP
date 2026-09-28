List<double> grades = new List<double>();

for (int i = 0; i < 5; i++)
{
    Console.Write("Enter grade " + (i + 1) + ": ");
    double grade = Convert.ToDouble(Console.ReadLine());
    grades.Add(grade);
}

double maximum = grades.Max();
double minimum = grades.Min();
double average = grades.Average();

Console.WriteLine();
Console.WriteLine("Maximum grade: " + maximum);
Console.WriteLine("Minimum grade: " + minimum);
Console.WriteLine("Average grade: " + average);
