using Microsoft.Data.Sqlite;

DB db = new DB();

db.CreateNew();

Console.WriteLine($"Currently accessing tasks");

foreach (var line in db.GetAll())
{
    Console.WriteLine(line);
}

Console.WriteLine("Delete database? y/n");
bool deleteDB = Console.ReadLine() == "y";

//Cleanup
if(deleteDB) db.Delete();

class Menu
{
    public int DisplayMainMenu()
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
            if(int.TryParse(Console.ReadLine(), out menuChoice))
            {
                if(menuChoice > 5 || menuChoice < 1)
                {
                    validInput = false;
                    Console.WriteLine("Error: Option outside range");
                    Console.WriteLine();
                }
            } else
            {
                Console.WriteLine("Please use 1-5 to make your selection");
                Console.WriteLine();
            }
        } while (validInput);

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
                    name TEXT NOT NULL
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
    public void Delete()
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
                SELECT * FROM tasks;
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
