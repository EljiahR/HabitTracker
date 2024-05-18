using Microsoft.Data.Sqlite;

DB db = new();
db.CreateNew(); // Only runs on start if no tasks.db exists
DateTime today = DateTime.UtcNow.Date;
Console.WriteLine(today.ToString("yyyy-MM-dd"));
int menuSelection;
do
{
    menuSelection = Menu.GetMenuChoice();

    switch (menuSelection)
    {
        case 1:
            // Close Application
            Console.Clear();
            Console.WriteLine("\nGoodbye!");
            break;
        case 2:
            // View All Records
            Console.Clear();
            Console.WriteLine("\nDisplaying all records from most recent to oldest:\n");
            foreach(var record in db.GetAll())
                Console.WriteLine($"{record[0]} performed {record[1]} time{(record[1] == "1" ? "" : "s")} on {record[2]}");
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
            Console.Clear();
            Console.WriteLine("\nAll records deleted");
            break;
    }
} while (menuSelection != 1);


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
        Console.WriteLine("\nMain Menu\n");
        Console.WriteLine("Select from the following:\n");
        bool validInput = true;
        int menuChoice;
        do
        {
            Console.WriteLine("1. Close Application");
            Console.WriteLine("2. View All Records");
            Console.WriteLine("3. Insert Record");
            Console.WriteLine("4. Delete Record");
            Console.WriteLine("5. Update Record");
            Console.WriteLine("\n6. Delete All Records");
            if(int.TryParse(Console.ReadLine(), out menuChoice))
            {
                if (menuChoice > 6 || menuChoice < 1)
                {
                    validInput = false;
                    Console.WriteLine("Error: Option outside range\n");
                }
                else validInput = true;
            } else
            {
                Console.WriteLine("Please use 1-6 to make your selection\n");
            }
        } while (!validInput);

        return menuChoice;
    }

    public static void GetRecordInfo()
    {
        Console.WriteLine("Would you like to use a pre-existing habit? y/n");
        string? response = Console.ReadLine();
        if(response != null && response.ToLower() == "y")
        {
            // Display all unique record inputs
        }
        else
        {
            // Get new record entry
        }
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
                       (description,                    amount, date)
                VALUES ('drink water',                  3,      '2024-05-18'),
                       ('walk a mile',                  5,      '2024-05-18'),
                       ('make fake records for app',    2,      '2024-05-18');
            ";
                command.ExecuteNonQuery();
            }
            Console.WriteLine($"Database tasks created successfully");
        }
        
    }
    public void DeleteAll()
    {
        SqliteConnection.ClearAllPools();
        File.Delete("tasks.db");
    }

    public List<string[]> GetAll()
    {
        List<string[]> results = new();

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
                    results.Add([reader.GetString(0), reader.GetString(1), reader.GetString(2)]);
                }
            }
        }

        return results;
    }
}
