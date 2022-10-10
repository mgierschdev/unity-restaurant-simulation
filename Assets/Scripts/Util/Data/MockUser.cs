using System;

// Mock User to use agains the database
public class MockUser
{
    public string FirstName = "First Name";
    public string SecondName = "Second Name";
    public string ID = "3ASOD139FB10V1AS";
    public string Auth = "google play";
    public DateTime LastLogin;
    public DateTime SignInDate;

    public MockUser()
    {
        this.SignInDate = new DateTime(2022, 0, 0, 0, 0, 0);
        this.LastLogin = new DateTime(2022, 0, 0, 0, 0, 1);
        ID = GenerateID();
    }

    private string GenerateID()
    {
        return Guid.NewGuid().ToString();
    }
}