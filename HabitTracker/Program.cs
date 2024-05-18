using Microsoft.Data.Sqlite;

int menuSelection = Menu.GetMenuChoice();
DB db = new();

switch(menuSelection)
{
    case 1:
        // Close Application
        Console.WriteLine("Goodbye!");
        break;
    case 2:
        // View All Records
        break;
    case 3:
        // Insert Record
        break;
    case 4:
        // Delete Record
        break;
    case 5:
        // Update Record
        break;
    case 6:
        // Delete database
        db.DeleteAll();
        break;
}

//DB db = new DB();

//db.CreateNew();

//Console.WriteLine($"Currently accessing tasks");

//foreach (var line in db.GetAll())
//{
//    Console.WriteLine(line);
//}

//Console.WriteLine("Delete database? y/n");
//bool deleteDB = Console.ReadLine() == "y";

////Cleanup
//if(deleteDB) db.Delete();

class Menu
{
    public static int GetMenuChoice()
    {
        Console.WriteLine();
        Console.WriteLine("Main Menu");
        Console.WriteLine();
        Console.WriteLine("Select from the following:");
        Console.WriteLine();
        bool validInput = true;
        int menuChoice;
        do
        {
            Console.WriteLine("1. Close Application");
            Console.WriteLine("2. View All Records");
            Console.WriteLine("3. Insert Record");
            Console.WriteLine("4. Delete Record");
            Console.WriteLine("5. Update Record");
            Console.WriteLine();
            Console.WriteLine("6. Delete All Records");
            if(int.TryParse(Console.ReadLine(), out menuChoice))
            {
                if (menuChoice > 6 || menuChoice < 1)
                {
                    validInput = false;
                    Console.WriteLine("Error: Option outside range");
                    Console.WriteLine();
                }
                else validInput = true;
            } else
            {
                Console.WriteLine("Please use 1-6 to make your selection");
                Console.WriteLine();
            }
        } while (!validInput);

        return menuChoice;
    }
}
class DB
{
    public void CreateNew()
    {
        if(!File.Exists("tasks.db")){
            using (var connection = new SqliteConnection($"Data Source=tasks.db"))
            {
                connection.Open();

                var command = connection.CreateCommand();
                command.CommandText =
                @"
                CREATE TABLE tasks (
                    id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                    description TEXT NOT NULL,
                    amount INTEGER NOT NULL,
                    date DATE NOT NULL
                );

                INSERT INTO tasks
                VALUES (1, 'Tim'),
                       (2, 'Tom'),
                       (3, 'Tum');
            ";
                command.ExecuteNonQuery();
            }
            Console.WriteLine($"Database tasks created successfully");
        }
        else
        {
            Console.WriteLine("Database already exists");
        }
    }
    public void DeleteAll()
    {
        SqliteConnection.ClearAllPools();
        File.Delete("tasks.db");
    }

    public List<string> GetAll()
    {
        List<string> results = new();

        using (var connection = new SqliteConnection($"Data Source=tasks.db"))
        {
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText =
            @"
                SELECT description, amount, date FROM tasks
                ORDER BY date DESC;
            ";

            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    results.Add(reader.GetString(1));
                }
            }
        }

        return results;
    }
}
