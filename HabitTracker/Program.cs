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
