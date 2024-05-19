using Microsoft.Data.Sqlite;
using System.Globalization;
using static System.Runtime.InteropServices.JavaScript.JSType;

DB db = new();
db.CreateNew(); // Only runs on start if no tasks.db exists
DateTime today = DateTime.UtcNow.Date;
//Console.WriteLine(today.ToString("yyyy-MM-dd"));
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
            Menu.ViewAllRecords(db);
            break;
        case 3:
            // Insert Record
            Menu.InsertRecord(db);
            break;
        case 4:
            // Delete Record
            Menu.DeleteRecord(db);
            break;
        case 5:
            // Update Record
            Menu.UpdateRecord(db);
            break;
        case 6:
            // Delete database
            Menu.DeleteAllRecords(db);
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

    public static void InsertRecord(DB db)
    {
        Console.Clear();
        string description = Menu.GetValidDescription(db);
        
        // Get amount
        Console.Clear();
        Console.WriteLine($"\t{description}\n");
        int amount = Menu.GetValidAmount();

        // Get date
        Console.Clear();
        Console.WriteLine($"\t{description} done {amount} time{(amount == 1 ? "" : "s")}\n");
        string date = Menu.GetValidDate();

        //Insert new record
        Console.Clear();
        try
        {
            db.InsertRecord(description, amount, date);
            Console.WriteLine("Habit successfully logged");
        } catch(Exception e)
        {
            Console.Error.WriteLine("Error has occured"); // This is shite I know
        }



    }

    public static string GetValidDescription(DB db)
    {

        Console.WriteLine("Would you like to use a pre-existing habit? y/n");
        string? response = Console.ReadLine();

        string? description;
        if (!string.IsNullOrEmpty(response) && response.ToLower() == "y")
        {
            // Display all unique record inputs
            List<string> records = db.GetUniqueRecords();
            if (records.Count > 0)
            {
                for (int i = 0; i < records.Count; i++)
                    Console.WriteLine($"{i + 1}:\t{records[i]}");
                string? recordResponse = Console.ReadLine();
                int chosenRecord;
                while (!int.TryParse(recordResponse, out chosenRecord) || chosenRecord < 1 || chosenRecord > records.Count)
                {
                    Console.WriteLine("Invalid response");
                    recordResponse = Console.ReadLine();
                }
                description = records[chosenRecord - 1];
            }
            else
            {
                Console.WriteLine("No records found");
                description = GetDescription();
            }
        }
        else
        {
            // Get new record entry
            description = GetDescription();
        }

        return description;
    }

    public static int GetValidAmount() 
    {
        int amount;
        Console.WriteLine("Please enter amount of times habit was done");
        string? amountResponse = Console.ReadLine();
        while (!int.TryParse(amountResponse, out amount))
        {
            Console.WriteLine("Invalid response. Please enter amount of times habit was done");
            amountResponse = Console.ReadLine();
        }

        return amount; 
    }

    public static string GetValidDate()
    {
        string? date;
        Console.WriteLine("Please enter date(YYYY-MM-DD) of habit or leave blank for today");
        string? dateResponse = Console.ReadLine();
        DateTime rawDate = new();
        while (!string.IsNullOrEmpty(dateResponse) && !DateTime.TryParse(dateResponse, out rawDate))
        {
            Console.WriteLine("Invalid date, please try again");
            dateResponse = Console.ReadLine();
        }
        if (string.IsNullOrEmpty(dateResponse))
            date = DateTime.Today.ToString("yyyy-MM-dd");
        else
            date = rawDate.ToString("yyyy-MM-dd");

        return date;
    }

    public static void ViewAllRecords(DB db)
    {
        Console.Clear();
        Console.WriteLine("\nDisplaying all records from most recent to oldest:\n");
        List<string[]> records = db.GetAll();
        if (records.Count > 0)
            foreach (var record in records)
                Console.WriteLine($"{record[0]} performed {record[1]} time{(record[1] == "1" ? "" : "s")} on {record[2]}");
        else
            Console.WriteLine("No records available");
    }

    static int SelectFromRecords(DB db, string action)
    {
        Console.Clear();
        Console.WriteLine($"\nPlease select record to {action}:\n");
        List<string[]> records = db.GetAll();
        if (records.Count > 0)
        {
            for (int i = 0; i < records.Count; i++)
                Console.WriteLine($"{i + 1}:\t{records[i][0]} performed {records[i][1]} time{(records[i][1] == "1" ? "" : "s")} on {records[i][2]}");
            string? response = Console.ReadLine();
            int recordIndex;
            while(!int.TryParse(response, out recordIndex) || recordIndex < 1 || recordIndex > records.Count)
            {
                Console.WriteLine("Invalid choice, please try again: ");
                response = Console.ReadLine();
            }
            return int.Parse(records[recordIndex - 1][3]);
        }
        else
        {
            Console.Clear();
            Console.WriteLine("No records available");
            return 0;
        }
    }

    public static void UpdateRecord(DB db)
    {
        int idToUpdate = SelectFromRecords(db, "update");
        if(idToUpdate != 0)
        {
            string[] record = db.GetSingleRecord(idToUpdate);
            Console.Clear();
            Console.WriteLine("Please choose which parts to update seperated by commas:\n");
            Console.WriteLine($"1. Description:\t{record[0]}");
            Console.WriteLine($"2. Amount: {record[1]}");
            Console.WriteLine($"3. Date: {record[2]}");
            string? response;
            List<int> options = new List<int>();
            bool responseValid = false;
            int s; // for testing parsed strings
            do
            {
                response = Console.ReadLine();
                if(!string.IsNullOrEmpty(response))
                {
                    if (response.Contains(","))
                    {
                        string[] responses = response.Split(",");
                        if (responses.All(str => int.TryParse(str, out s) && s >= 1 && s <= 3))
                        {
                            foreach (var option in responses)
                            {
                                options.Add(int.Parse(option));
                            }
                            responseValid = true;
                        }
                    } else if(int.TryParse(response, out s) && s >= 1 && s <= 3)
                    {
                        options.Add(int.Parse(response));
                        responseValid = true;
                    }
                }
                if (!responseValid) Console.WriteLine("Invalid response");
            } while (!responseValid);
            options.Sort();
            // get updated records
            string newDescription = "";
            int newAmount = 0;
            string newDate = "";
            foreach(var option in options)
            {
                switch(option)
                {
                    case 1:
                        newDescription = Menu.GetValidDescription(db);
                        break;
                    case 2:
                        newAmount = Menu.GetValidAmount();
                        break;
                    case 3:
                        newDate = Menu.GetValidDate();
                        break;
                }
            }
            db.UpdateRecord(idToUpdate, options, newDescription, newAmount, newDate);
            Console.Clear();
            Console.WriteLine("Record updated successfully");
        }

    }

    public static void DeleteRecord(DB db)
    {
        int idToDelete = SelectFromRecords(db, "delete");
        if(idToDelete != 0)
        {
            db.DeleteRecord(idToDelete);
            Console.Clear();
            Console.WriteLine("\nRecord deleted");
        }
        
    }

    static string GetDescription()
    {
        Console.WriteLine("\nPlease enter a short description for the habit:");
        string? description = Console.ReadLine();
        while(string.IsNullOrEmpty(description))
        {
            Console.WriteLine("Description cannot be null, please enter a short description for the habit:");
            description = Console.ReadLine();
        }


        return description;
    }

    public static void DeleteAllRecords(DB db)
    {
        db.DeleteAll();
        Console.Clear();
        Console.WriteLine("\nAll records deleted\n");
        Console.WriteLine("Re-populate with starter data? y/n");
        string? repopulateResponse = Console.ReadLine();
        if (!string.IsNullOrEmpty(repopulateResponse)) repopulateResponse.Trim().ToLower();
        if (repopulateResponse == "y")
        {
            db.CreateNew();
            Console.WriteLine("\nDatabase reset to starter data\n");
        }
        else
        {
            db.CreateNew(false);
            Console.WriteLine("\nDatabase initialized with no data\n");
        }
    }
}
class DB
{
    public void CreateNew(bool withStarterData = true)
    {
        if(!File.Exists("tasks.db")){
            using (var connection = new SqliteConnection("Data Source=tasks.db"))
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
                ";
                command.ExecuteNonQuery();

                if (withStarterData)
                {
                    command.CommandText =
                    @"
                    INSERT INTO tasks
                           (description,                    amount, date)
                    VALUES  ('drink water',                  3,      '2024-05-18'),
                            ('walk a mile',                  5,      '2024-05-18'),
                            ('make fake records for app',    2,      '2024-05-18'),
                            ('drink water',                  2,      '2024-05-17'),
                            ('make fake records for app',    1,      '2024-05-17');
                    ";
                    command.ExecuteNonQuery();
                }
                
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

        using (var connection = new SqliteConnection("Data Source=tasks.db"))
        {
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText =
            @"
                SELECT description, amount, date, id FROM tasks
                ORDER BY date DESC;
            ";

            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    results.Add([reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(3)]);
                }
            }
        }

        return results;
    }

    public List<string> GetUniqueRecords()
    {
        List<string> results = new List<string>();

        using (var connection = new SqliteConnection("Data Source=tasks.db"))
        {
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText =
            @"
                SELECT DISTINCT description FROM tasks;
            ";

            using(var reader = command.ExecuteReader())
            {
                while(reader.Read())
                {
                    results.Add(reader.GetString(0));
                }
            }
        }

        return results;
    }

    public void InsertRecord(string description, int amount, string date)
    {
        using (var connection = new SqliteConnection("Data Source=tasks.db"))
        {
            connection.Open();

            var command = connection.CreateCommand();
            
            
            command.CommandText =
            @"
                INSERT INTO tasks
                        (description, amount, date)
                VALUES  ($description, $amount, $date);
            ";
            command.Parameters.AddWithValue("$description", description);
            command.Parameters.AddWithValue("$amount", amount);
            command.Parameters.AddWithValue("$date", date);
            command.ExecuteNonQuery();

        }
    }

    public void UpdateRecord(int id, List<int> options, string description, int amount, string date)
    {
        using (var connection = new SqliteConnection("Data Source=tasks.db"))
        {
            connection.Open();

            var command = connection.CreateCommand();


            command.CommandText =
            @"
                UPDATE tasks
                SET ";
            
            for(int i = 0; i < options.Count; i++)
            {
                if (i > 0) command.CommandText += @", ";
                switch(options[i])
                {
                    case 1:
                        command.CommandText += @"description = $description";
                        command.Parameters.AddWithValue("$description", description);
                        break;
                    case 2:
                        command.CommandText += @"amount = $amount";
                        command.Parameters.AddWithValue("$amount", amount);
                        break;
                    case 3:
                        command.CommandText += @"date = $date";
                        command.Parameters.AddWithValue("$date", date);
                        break;
                }
            }
            command.CommandText +=
            @"
                WHERE id = $id;
            ";
            command.Parameters.AddWithValue("$id", id);
            command.ExecuteNonQuery();

        }
    }

    public void DeleteRecord(int id)
    {
        using (var connection = new SqliteConnection("Data Source=tasks.db"))
        {
            connection.Open();

            var command = connection.CreateCommand();


            command.CommandText =
            @"
                DELETE FROM tasks
                WHERE id = $id;
            ";
            command.Parameters.AddWithValue("$id", id);
            command.ExecuteNonQuery();

        }
    }

    public string[] GetSingleRecord(int id)
    {
        string[] result = new string[3];
        using (var connection = new SqliteConnection("Data Source=tasks.db"))
        {
            connection.Open();

            var command = connection.CreateCommand();

            command.CommandText =
            @"
                SELECT description, amount, date FROM tasks
                WHERE id = $id;
            ";
            command.Parameters.AddWithValue("$id", id);
            using(var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    result[0] = reader.GetString(0);
                    result[1] = reader.GetString(1);
                    result[2] = reader.GetString(2);
                }
            }
        }

        return result;
    }
}
