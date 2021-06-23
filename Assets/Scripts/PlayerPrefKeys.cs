public static class PlayerPrefKeys
{
    public static string HasDoneTutorial = "HasDoneTutorial";
    
    /// <summary>
    /// Contains the player name used for multiplayer
    /// </summary>
    public static string PlayerName = "PlayerName";

    /// <summary>
    /// Contains the FOV value wanted by the user
    /// </summary>
    public static string FOV = "FOV";
    
    /// <summary>
    /// Contains the firebase userId of the connected user
    /// </summary>
    public static string LoggedUserId = "LoggedUserId";
    
    /// <summary>
    /// Contains the firebase idToken of the connected user
    /// </summary>
    public static string LoggedUserToken = "LoggedUserToken";
    
    /// <summary>
    /// Contains the firebase idToken of the connected user
    /// </summary>
    public static string LoggedUserExpiration = "LoggedUserExpiration";
    
    /// <summary>
    /// Contains the firebase refreshToken of the connected user
    /// </summary>
    public static string LoggedUserRefreshToken = "LoggedUserRefreshToken";
    
    /// <summary>
    /// Last state of the save credential preference. 1 if true, else 0
    /// </summary>
    public static string SaveCredentials = "SaveCredentials";
}