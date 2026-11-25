# Student Task Manager

A C#/.NET console application created for the **Independent Challenges** in the OOP tutorial.

Assignment source: https://github.com/icebeam7/oop-tutorials/blob/main/00-introduction/07-challenges.md

## Implemented challenges

### 1. Input validation
Menu input and task-number input use `int.TryParse`, so invalid text does not crash the application.

### 2. Prevent empty tasks
New tasks are checked with `string.IsNullOrWhiteSpace` before they are added.

### 3. Search tasks
The menu includes a search feature that performs a case-insensitive search through the task list.

### 4. LINQ
The project uses LINQ methods including:

- `Where(...)`
- `Any(...)`
- `Count(...)`
- `OrderBy(...)`

### 5. Persist tasks
Tasks are stored in `tasks.json` with `System.Text.Json` and loaded again when the application starts.

## Menu

```text
Student Task Manager
====================
1. Add task
2. View tasks
3. Remove task
4. Search tasks
5. Show task count
0. Exit
```

The tutorial previously used option 4 for task count, while the challenge asks for option 4 to be search. This solution keeps **Search tasks** as option 4 and moves **Show task count** to option 5.

## Run

Requirements: .NET 8 SDK or newer.

```bash
dotnet run
```

The application creates `tasks.json` automatically after tasks are added or removed.
