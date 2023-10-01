using CsvHelper;
using CsvHelper.Configuration;
using PasswordManager.Model;
using System.Globalization;
using System;
using System.CommandLine;

internal class Program
{
    static async Task<int> Main(string[] args)
    {
        var WorkDir = new Option<string>(
            name: "--workdir",
            description: "csv Path"
            );

        var Username = new Option<string>(
            name: "--username",
            description: "Username of the user"
            );

        var Password = new Option<string>(
            name: "--password",
            description: "Password of the user"
            );

        var Email = new Option<string>(
            name: "--email",
            description: "Email of the user"
            );

        var Firstname = new Option<string>(
            name: "--firstname",
            description: "Firstname of the user"
            );

        var Lastname = new Option<string>(
            name: "--lastname",
            description: "Lastname of the user"
            );

        var Userid = new Option<string>(
            name: "--user_id",
            description: "user id of the user"
            );

        var Website = new Option<string>(
            name: "--website",
            description: "website of the user"
            );

        var root = new RootCommand("This is the Root")
        {  WorkDir  };

        var login = new Command("login", "Login user")
        {
           Username,Password,WorkDir
        };

        var register = new Command("register","Signup new user")
        {
            Username, Password,Email, Firstname, Lastname
        };

        var list = new Command("list","list user's password")
        {
            Username, Password
        };

        root.AddCommand(login);
        root.AddCommand(register);
        login.AddCommand(list);

        login.SetHandler((user, pass, dir) => {

            if (Login(user, pass, dir))
                Console.WriteLine("logged in");
            else
                Console.WriteLine("Wrong username or password!");

        },Username,Password,WorkDir);

        register.SetHandler((user, pass, email, first, last, dir) => {
           Signup(user, pass, email, first, last, dir);
        }, Username, Password, Email, Firstname, Lastname, WorkDir);

        list.SetHandler((user, pass, web, dir) => {
            if (web == null) web = "";
            ListPassowrds(user!, dir!).ForEach(us => { Console.WriteLine(us.DecryptPassword()+ " " + us.Username + " " + us.Website); });

        }, Username, Password, Website, WorkDir);

        return await root.InvokeAsync(args);
    }

     static bool Login(string username, string password, string dir)
    {
        using (StreamReader streamRead = new(Path.Join(dir, "user.csv")))
        {
            using CsvReader reader = new(streamRead, CultureInfo.InvariantCulture);
            var user = reader.GetRecords<User>().ToList().Find(us => us.Username == username);
            if (user != null)
            {
                
                if (user.DecryptPassword() == password)
                {
                    return true;
                }
            }
            return false;
        }
    }

    static void Signup(string user, string pass, string email, string first, string last, string dir)
    {
        using (StreamReader streamReader = new(Path.Join(dir, "user.csv")))
        {
            using CsvReader reader = new(streamReader, CultureInfo.InvariantCulture);
            if (reader.GetRecords<User>().ToList()?.Find( us => us.Email == email)!= null || reader.GetRecords<User>().ToList()?.Find(us => us.Username == user) != null)
            { Console.WriteLine("Existing user"); 
            
               streamReader.Close();
            }
            else
            {
                streamReader.Close();

                using (StreamWriter stream = new(Path.Join(dir, "user.csv"), append: true))
                {
                    CsvConfiguration config = new CsvConfiguration(CultureInfo.InvariantCulture)
                    {
                        HasHeaderRecord = false
                    };
                    using CsvWriter writer = new(stream, config);
                    if (first == null) first = "";
                    if (last == null) last = "";

                    var newUser = new User()
                    {
                        Username = user,
                        Email = email,
                        Password = pass,
                        Firstname = first,
                        Lastname = last,
                    };
                    newUser.EncryptPassword();
                    writer.WriteRecord(newUser);
                    stream.WriteLineAsync();
                    Console.WriteLine("Thank you for your registration");

                }
            }

        }
       
    }

    static List<Vault> ListPassowrds(string username, string dir)
    {
        using (StreamReader stream = new(Path.Join(dir, "vault.csv")))
        {
        Vault.UserCsvPath = dir;
            using CsvReader reader = new(stream, CultureInfo.InvariantCulture);
            return reader.GetRecords<Vault>().ToList().FindAll(us => us.UserId == username);
        }
    }
}