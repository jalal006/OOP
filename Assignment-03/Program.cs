List<Teacher> teachers = new List<Teacher>();

bool running = true;

while (running)
{
    Console.WriteLine();
    Console.WriteLine("Teacher System");
    Console.WriteLine("==============");
    Console.WriteLine("1. Add teacher");
    Console.WriteLine("2. Show teachers");
    Console.WriteLine("3. Count teachers");
    Console.WriteLine("4. Average years of experience");
    Console.WriteLine("0. Exit");
    Console.Write("Choose: ");

    int option = int.Parse(Console.ReadLine());

    switch (option)
    {
        case 1:
            Console.Write("Name: ");
            string name = Console.ReadLine();

            Console.Write("Subject: ");
            string subject = Console.ReadLine();

            Console.Write("Years of experience: ");
            int years = int.Parse(Console.ReadLine());

            Teacher newTeacher = new Teacher(name, subject, years);
            teachers.Add(newTeacher);

            Console.WriteLine("Teacher added.");
            break;

        case 2:
            if (teachers.Count == 0)
            {
                Console.WriteLine("No teachers available.");
            }
            else
            {
                foreach (Teacher teacher in teachers)
                {
                    teacher.ShowProfile();
                }
            }
            break;

        case 3:
            Console.WriteLine("Total teachers: " + teachers.Count);
            break;

        case 4:
            if (teachers.Count == 0)
            {
                Console.WriteLine("No teachers available.");
            }
            else
            {
                double totalYears = 0;

                foreach (Teacher teacher in teachers)
                {
                    totalYears += teacher.YearsOfExperience;
                }

                double average = totalYears / teachers.Count;

                Console.WriteLine("Average years of experience: " + average);
            }
            break;

        case 0:
            running = false;
            break;

        default:
            Console.WriteLine("Invalid option.");
            break;
    }
}
