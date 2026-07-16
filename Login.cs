using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SmartFactorySimple;

public class Login
{
    private readonly string CREDENTIALS_FILE;
    private readonly Dictionary<string, EmployeeCredential> credentials = [];

    public class EmployeeCredential
    {
        public string EmployeeId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
    }

    public Login()
    {
        // Set credentials file path in the project directory
        CREDENTIALS_FILE = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "employees.txt");
        LoadCredentials();
    }

   
    private void LoadCredentials()
    {
        try
        {
            if (!File.Exists(CREDENTIALS_FILE))
            {
                Console.WriteLine(Messages.FileNotFound(CREDENTIALS_FILE));
                return;
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
                    Console.WriteLine(Messages.InvalidCredentialLine(line));
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

            Console.WriteLine(Messages.CredentialsLoaded(credentials.Count));
        }
        catch (Exception ex)
        {
            Console.WriteLine(Messages.CredentialsLoadFailed(ex.Message));
        }
    }

    
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

    
    public EmployeeCredential PromptLogin()
    {
        Console.WriteLine(Messages.LoginTitle);
        Console.Write(Messages.UsernamePrompt);
        string username = Console.ReadLine();

        Console.Write(Messages.PasswordPrompt);
        string password = ReadPassword();

        EmployeeCredential credential = Authenticate(username, password);

        if (credential != null)
        {
            Console.WriteLine(Messages.Welcome(username, credential.Role));
                Logging.Log(credential.Username, Messages.SuccessfulLoginLog());
            return credential;
        }
        else
        {
            Console.WriteLine(Messages.InvalidCredentials);
            var userForLog = string.IsNullOrWhiteSpace(username) ? Messages.UnknownUser : username;
            Logging.Log(userForLog, Messages.FailedLoginAttemptLog());
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
                Console.WriteLine(Messages.LoginAttemptFailed(attempt, maxAttempts - attempt));
            }
        }

        Console.WriteLine(Messages.LoginFailed);
        return null;
    }

   
    public bool SaveEmployeeCredential(string employeeId, string username, string password, string role)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(employeeId) || string.IsNullOrWhiteSpace(username) || 
                string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(role))
            {
                Console.WriteLine(Messages.CredentialFieldsRequired());
                return false;
            }

            if (credentials.ContainsKey(username))
            {
                Console.WriteLine(Messages.UsernameAlreadyExists(username));
                return false;
            }

            string credentialLine = $"{employeeId};{username};{password};{role}";
            
            // Append to file
            File.AppendAllText(CREDENTIALS_FILE, credentialLine + Environment.NewLine);
            
            // Update in-memory dictionary
            var credential = new EmployeeCredential
            {
                EmployeeId = employeeId,
                Username = username,
                Password = password,
                Role = role
            };
            credentials[username] = credential;

            Console.WriteLine(Messages.CredentialsSaved(username, role));
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine(Messages.CredentialsSaveFailed(ex.Message));
            return false;
        }
    }

    // Read password from console while masking input with '*'
    public string ReadPassword()
    {
        var pwd = new System.Text.StringBuilder();
        ConsoleKeyInfo key;
        while (true)
        {
            key = Console.ReadKey(intercept: true);
            if (key.Key == ConsoleKey.Enter)
            {
                Console.WriteLine();
                break;
            }
            else if (key.Key == ConsoleKey.Backspace)
            {
                if (pwd.Length > 0)
                {
                    pwd.Length--;
                    // move cursor back, write space, move back again
                    Console.Write(Messages.PasswordMaskBackspace);
                }
            }
            else if (!char.IsControl(key.KeyChar))
            {
                pwd.Append(key.KeyChar);
                    Console.Write(Messages.PasswordMaskCharacter);
            }
        }

        return pwd.ToString();
    }
}
