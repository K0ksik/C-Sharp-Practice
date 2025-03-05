using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
class RegAuth
{
	private static List<RegAuth> Users = new List<RegAuth>();
	private string Name { get; set; }
	private string Password { get; set; }
	public RegAuth(string name, string password)
	{
		this.Name = name;
		this.Password = password;
	}

	public static async Task<RegAuth> Register()
	{
		Console.WriteLine("Введите имя: ");
		string name = Console.ReadLine();

		Console.WriteLine("Введите пароль: ");
		string password = Console.ReadLine();

		var newUser = new RegAuth(name, password);
		Users.Add(newUser);

		await FileAct.WriteFile("C:\\Users\\sora1\\Documents\\Practice3\\Users.txt", $"{name}:{password}");

		return newUser;
	}

	public static async Task<RegAuth> Authorization()
	{
		Console.WriteLine("Введите имя: ");
		string name = Console.ReadLine();

		Console.WriteLine("Введите пароль: ");
		string password = Console.ReadLine();

		foreach (var user in Users)
		{
			if (user.Name == name && user.Password == password)
			{
				return user;
			}
		}
		return null;
	}
	public static async Task LoadUsers()
	{
		var lines = await FileAct.ReadFile("C:\\Users\\sora1\\Documents\\Practice3\\Users.txt");
		foreach (var line in lines)
		{
			string[] parts = line.Split(':');
			if (parts.Length == 2)
			{
				Users.Add(new RegAuth(parts[0], parts[1]));
			}
		}
	}
}

class UserTasks
{
	private static List<UserTasks> MyTasks = new List<UserTasks>();
	public int Id { get; set; }
	public static int nextId = 1;
	public string Title { get; set; }
	public string Description { get; set; }
	public string Priority { get; set; }
	public string Status { get; set; }

	public UserTasks(int id, string title, string description, string priority, string status)
	{
		this.Id = id;
		this.Title = title;
		this.Description = description;
		this.Priority = priority;
		this.Status = status;
	}

	public static async Task<UserTasks> AddTask()
	{
		Console.WriteLine("Введите заголовок: ");
		string title = Console.ReadLine();
		Console.WriteLine("Введите описание: ");
		string description = Console.ReadLine();
		Console.WriteLine("Введите приоритет(низкий, средний, высокий): ");
		string priority = Console.ReadLine();
		Console.WriteLine("Введите статус(недоступна, в процессе, завершена): ");
		string status = Console.ReadLine();

		var newTask = new UserTasks(nextId++, title, description, priority, status);
		MyTasks.Add(newTask);

		await FileAct.SaveAll(MyTasks);
		return newTask;
	}

	public static async Task<UserTasks> EditTask()
	{
		Console.WriteLine("Введите Id задачи: ");
		if (!int.TryParse(Console.ReadLine(), out int taskId))
		{
			Console.WriteLine("Неверный id ");
			return null;
		}
		UserTasks taskEdit = null;
		foreach (var task in MyTasks)
		{
			if (task.Id == taskId)
			{
				taskEdit = task;
				break;
			}
		}
		if (taskEdit == null)
		{
			Console.WriteLine("Задача не найдена...");
			return null;
		}
		Console.WriteLine("Введите заголовок: ");
		taskEdit.Title = Console.ReadLine();
		Console.WriteLine("Введите описание: ");
		taskEdit.Description = Console.ReadLine();
		Console.WriteLine("Введите приоритет(низкий, средний, высокий): ");
		taskEdit.Priority = Console.ReadLine();
		Console.WriteLine("Введите статус(недоступна, в процессе, завершена): ");
		taskEdit.Status = Console.ReadLine();

		await FileAct.SaveAll(MyTasks);
		return taskEdit;
	}

	public static async Task<UserTasks> DeleteTask()
	{
		Console.WriteLine("Введите Id задачи: ");
		if (!int.TryParse(Console.ReadLine(), out int taskId))
		{
			Console.WriteLine("Неверное id");
			return null;
		}

		UserTasks taskDelete = null;
		foreach (var task in MyTasks)
		{
			if (task.Id == taskId)
			{
				taskDelete = task;
				break;
			}
		}
		if (taskDelete == null)
		{
			Console.WriteLine("Задача не найдена...");
			return null;
		}
		MyTasks.Remove(taskDelete);

		await FileAct.SaveAll(MyTasks);
		return taskDelete;
	}

	public static async Task LoadTasks()
	{
		var lines = await FileAct.ReadFile("C:\\Users\\sora1\\Documents\\Practice3\\Tasks.txt");
		foreach (var line in lines)
		{
			string[] parts = line.Split(':');
			if (parts.Length == 5)
			{
				var newTask = new UserTasks(nextId++,(parts[0]), parts[1], parts[2], parts[3]);
				MyTasks.Add(newTask);
				if (int.Parse(parts[0]) >= nextId)
				{
					nextId = int.Parse(parts[0]) + 1;
				}
			}
		}
	}
}

class FileAct
{
	public static async Task SaveAll(List<UserTasks> tasks)
	{
		using (StreamWriter sw = new StreamWriter("C:\\Users\\sora1\\Documents\\Practice3\\Tasks.txt", false, System.Text.Encoding.Default))
		{
			foreach (var task in tasks)
			{
				await sw.WriteLineAsync($"{task.Id}:{task.Title}:{task.Description}:{task.Priority}:{task.Status}");
			}
		}
	}
	public static async Task WriteFile(string filePath, string info)
	{
		using (StreamWriter sw = new StreamWriter(filePath, true, System.Text.Encoding.Default))
		{
			await sw.WriteLineAsync(info);
		}
	}
	public static async Task<List<string>> ReadFile(string filePath)
	{
		if (System.IO.File.Exists(filePath))
		{
			var info = await System.IO.File.ReadAllLinesAsync(filePath);
			return new List<string>(info);
		}
		return new List<string>();
	}
}

class Practice3
{
	static async Task Main(string[] args)
	{
		string FileList = "C:\\Users\\sora1\\Documents\\Practice3";
		Directory.CreateDirectory(FileList);

		await Task.WhenAll(RegAuth.LoadUsers(), UserTasks.LoadTasks());

		RegAuth UserNow = null;

		while (true)
		{
			if (UserNow == null)
			{
				Console.WriteLine("1. Регистрация");
				Console.WriteLine("2. Авторизация");
				Console.WriteLine("3. Выйти");

				string choice = Console.ReadLine();

				switch (choice)
				{
					case "1":
						await RegAuth.Register();
						break;

					case "2":
						UserNow = await RegAuth.Authorization();
						if (UserNow != null)
						{
							Console.WriteLine("Вы вошли");
						}
						else
						{
							Console.WriteLine("Неверное имя или пароль");
						}
						break;

					case "3":
						return;

					default:
						Console.WriteLine("Неверный выбор");
						break;
				}
			}
			else
			{
				Console.WriteLine("Выберите действие:");
				Console.WriteLine("1. Добавить задачу");
				Console.WriteLine("2. Редактировать задачу");
				Console.WriteLine("3. Удалить задачу");
				Console.WriteLine("4. Выйти");
				string choice = Console.ReadLine();

				switch (choice)
				{
					case "1":
						var newTask = await UserTasks.AddTask();
						if (newTask != null)
						{
							Console.WriteLine("Задача добавлена");
						}
						break;

					case "2":
						var editedTask = await UserTasks.EditTask();
						if (editedTask != null)
						{
							Console.WriteLine("Задача изменена");
						}
						break;

					case "3":
						var deletedTask = await UserTasks.DeleteTask();
						if (deletedTask != null)
						{
							Console.WriteLine("Задача удалена");
						}
						break;
					case "4":
						UserNow = null;
						break;

					default:
						Console.WriteLine("Неверный выбор");
						break;
				}
			}
		}
	}
}

