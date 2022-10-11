using System;
using System.Collections.Generic;

// Mock User to use agains the database
public class MockUser
{
    public string FirstName = "First Name";
    public string SecondName = "Second Name";
    public string ID;
    public string Auth = "google play";
    public DateTime LastLogin;
    public DateTime SignInDate;

    public MockUser()
    {
        this.SignInDate = DateTime.Now;
        this.LastLogin = DateTime.Now;
        ID = GenerateID();
    }

    private string GenerateID()
    {
        return Guid.NewGuid().ToString();
    }

    public Dictionary<string, object> GetUserAsMap(){
        return new Dictionary<string, object>{
            {"Name", new List<object>(){FirstName, SecondName}},
            {"Auth", Auth},
            {"LastLogin", LastLogin},
            {"CreatedAt", SignInDate}
        };
    }
}