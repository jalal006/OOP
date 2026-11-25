const int MaxTasks = 10;
const string FileName = "tasks.txt";

List<string> tasks = new List<string>();

// Load saved tasks when the program starts
if (File.Exists(FileName))
{
    tasks.AddRange(File.ReadAllLines(FileName));
}

bool running = true;

while (running)
{
    Console.WriteLine();
    Console.WriteLine("Student Task Manager");
    Console.WriteLine("====================");
    Console.WriteLine("1. Add task");
    Console.WriteLine("2. View tasks");
    Console.WriteLine("3. Remove task");
    Console.WriteLine("4. Search tasks");
    Console.WriteLine("5. Show task count");
    Console.WriteLine("0. Exit");
    Console.Write("Choose: ");

    bool validNumber = int.TryParse(Console.ReadLine(), out int option);

    if (!validNumber)
    {
        Console.WriteLine("Invalid option. Please enter a number.");
        continue;
    }

    switch (option)
    {
        case 1:
            AddTask(tasks);
            break;

        case 2:
            ShowTasks(tasks);
            break;

        case 3:
            RemoveTask(tasks);
            break;

        case 4:
            SearchTasks(tasks);
            break;

        case 5:
            ShowTaskCount(tasks);
            break;

        case 0:
            running = false;
            Console.WriteLine("Goodbye!");
            break;

        default:
            Console.WriteLine("Invalid option.");
            break;
    }
}

void AddTask(List<string> tasks)
{
    if (tasks.Count >= MaxTasks)
    {
        Console.WriteLine("Maximum number of tasks reached.");
        return;
    }

    Console.Write("Enter task: ");
    string task = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(task))
    {
        Console.WriteLine("Task cannot be empty.");
        return;
    }

    tasks.Add(task);
    File.WriteAllLines(FileName, tasks);

    Console.WriteLine("Task added.");
}

void ShowTasks(List<string> tasks)
{
    if (tasks.Count == 0)
    {
        Console.WriteLine("No tasks available.");
        return;
    }

    for (int i = 0; i < tasks.Count; i++)
    {
        Console.WriteLine($"{i + 1}. {tasks[i]}");
    }
}

void RemoveTask(List<string> tasks)
{
    if (tasks.Count == 0)
    {
        Console.WriteLine("No tasks available.");
        return;
    }

    ShowTasks(tasks);

    Console.Write("Task to remove: ");
    bool validNumber = int.TryParse(Console.ReadLine(), out int taskNumber);

    if (!validNumber)
    {
        Console.WriteLine("Invalid task number.");
        return;
    }

    if (taskNumber < 1 || taskNumber > tasks.Count)
    {
        Console.WriteLine("Task does not exist.");
        return;
    }

    tasks.RemoveAt(taskNumber - 1);
    File.WriteAllLines(FileName, tasks);

    Console.WriteLine("Task removed.");
}

void SearchTasks(List<string> tasks)
{
    Console.Write("Search: ");
    string search = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(search))
    {
        Console.WriteLine("Search cannot be empty.");
        return;
    }

    bool found = false;

    for (int i = 0; i < tasks.Count; i++)
    {
        if (tasks[i].Contains(search, StringComparison.OrdinalIgnoreCase))
        {
            Console.WriteLine($"{i + 1}. {tasks[i]}");
            found = true;
        }
    }

    if (!found)
    {
        Console.WriteLine("No matching tasks found.");
    }
}

void ShowTaskCount(List<string> tasks)
{
    Console.WriteLine($"You currently have {tasks.Count} tasks.");
    Console.WriteLine($"Maximum allowed: {MaxTasks}");
    Console.WriteLine($"Remaining capacity: {MaxTasks - tasks.Count}");
}
