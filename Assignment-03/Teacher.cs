class Teacher
{
    public string Name;
    public string Subject;
    public int YearsOfExperience;

    public Teacher(string name, string subject, int yearsOfExperience)
    {
        Name = name;
        Subject = subject;
        YearsOfExperience = yearsOfExperience;
    }

    public void ShowProfile()
    {
        Console.WriteLine();
        Console.WriteLine("Name: " + Name);
        Console.WriteLine("Subject: " + Subject);
        Console.WriteLine("Years of experience: " + YearsOfExperience);
    }
}
