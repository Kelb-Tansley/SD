namespace SD.Core.Shared.Entity
{
    public class UserPreferences
    {
        public string UserName { get; set; } = "DefaultUser";
        public WindowStates WindowStates { get; set; } = new WindowStates();
    }
}
