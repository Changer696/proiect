using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class Login
{
    private const string CREDENTIALS_FILE = "employees.txt";
    private Dictionary<string, EmployeeCredential> credentials = new Dictionary<string, EmployeeCredential>();

    public class EmployeeCredential
    {
        public string EmployeeId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
    }

    public Login()
    {
        LoadCredentials();
    }

    /// <summary>
    /// Loads employee credentials from the employees.txt file
    /// File format: employeeId;username;password;role
    /// </summary>
    private void LoadCredentials()
    {
        try
        {
            if (!File.Exists(CREDENTIALS_FILE))
            {
                Console.WriteLine($"Warning: {CREDENTIALS_FILE} not found. Creating default file...");
                CreateDefaultCredentialsFile();
            }

            credentials.Clear();
            string[] lines = File.ReadAllLines(CREDENTIALS_FILE);

            foreach (string line in lines)
            {
                if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#"))
                    continue;

                string[] parts = line.Split(';');
                if (parts.Length != 4)
                {
                    Console.WriteLine($"Warning: Invalid line format: {line}");
                    continue;
                }

                var credential = new EmployeeCredential
                {
                    EmployeeId = parts[0].Trim(),
                    Username = parts[1].Trim(),
                    Password = parts[2].Trim(),
                    Role = parts[3].Trim()
                };

                credentials[credential.Username] = credential;
            }

            Console.WriteLine($"Loaded {credentials.Count} employee credentials.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading credentials: {ex.Message}");
        }
    }

    /// <summary>
    /// Creates a default credentials file with sample data
    /// </summary>
    private void CreateDefaultCredentialsFile()
    {
        try
        {
            string[] defaultCredentials = new string[]
            {
                "# Smart Factory - Employee Credentials",
                "# Format: employeeId;username;password;role",
                "1;director;pass123;Director",
                "2;manager;pass123;ProductionManager",
                "3;tech1;pass123;Technician"
            };

            File.WriteAllLines(CREDENTIALS_FILE, defaultCredentials);
            Console.WriteLine($"Default credentials file created: {CREDENTIALS_FILE}");
            LoadCredentials();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating credentials file: {ex.Message}");
        }
    }

    /// <summary>
    /// Authenticates a user with username and password
    /// </summary>
    /// <param name="username">The username to authenticate</param>
    /// <param name="password">The password to verify</param>
    /// <returns>The employee credential if authentication is successful, null otherwise</returns>
    public EmployeeCredential Authenticate(string username, string password)
    {
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            return null;
        }

        if (credentials.ContainsKey(username))
        {
            EmployeeCredential credential = credentials[username];
            if (credential.Password == password)
            {
                return credential;
            }
        }

        return null;
    }

    /// <summary>
    /// Displays the login prompt and authenticates the user
    /// </summary>
    /// <returns>The authenticated employee credential, or null if authentication fails</returns>
    public EmployeeCredential PromptLogin()
    {
        Console.WriteLine("\n========== SMART FACTORY LOGIN ==========");
        Console.Write("Username: ");
        string username = Console.ReadLine();

        Console.Write("Password: ");
        string password = Console.ReadLine();

        EmployeeCredential credential = Authenticate(username, password);

        if (credential != null)
        {
            Console.WriteLine($"\nWelcome {username}! Role: {credential.Role}\n");
            return credential;
        }
        else
        {
            Console.WriteLine("\nInvalid username or password!\n");
            return null;
        }
    }

    
    public EmployeeCredential LoginWithAttempts(int maxAttempts = 3)
    {
        for (int attempt = 1; attempt <= maxAttempts; attempt++)
        {
            EmployeeCredential credential = PromptLogin();
            if (credential != null)
            {
                return credential;
            }

            if (attempt < maxAttempts)
            {
                Console.WriteLine($"Attempt {attempt} failed. {maxAttempts - attempt} attempts remaining.\n");
            }
        }

        Console.WriteLine("Login failed after maximum attempts. Exiting...");
        return null;
    }
}
