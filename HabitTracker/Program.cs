using Microsoft.Data.Sqlite;
File.Delete("tasks.db");

if (!File.Exists("tasks.db"))
using (var connection = new SqliteConnection("Data Source=tasks.db"))
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

using (var connection = new SqliteConnection("Data Source=tasks.db"))
{
    connection.Open();

    var command = connection.CreateCommand();
    command.CommandText =
    @"
        SELECT name FROM tasks;
    ";

    using(var reader = command.ExecuteReader())
    {
        while (reader.Read())
        {
            Console.WriteLine(reader.GetString(0));
        }
    }

}
SqliteConnection.ClearAllPools();

Console.WriteLine("Delete database? y/n");
bool deleteDB = Console.ReadLine() == "y";

//Cleanup
if(deleteDB) File.Delete("tasks.db");
