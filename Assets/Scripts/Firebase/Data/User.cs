using System;
using System.Collections.Generic;

namespace Firebase.Data
{
    public enum Status
    {
        BANNED,
        ADMIN,
        INACTIVE,
        ACTIVE
    }
    
    public class User
    {
        private string ID;
        private string username;
        private string email;
        private string iconUrl;
        private DateTimeOffset birthdate;
        private List<string> likedNews;
        private List<string> matchHistory;
        private int elo;
        private DateTimeOffset registerDate;
        private Status status;
        private List<string> items;
        private int level;

        public User(string ID, string username,
            string email,
            string iconUrl,
            DateTimeOffset birthdate, 
            List<string> likedNews, 
            List<string> matchHistory,
            int elo,
            DateTimeOffset registerDate,
            Status status,
            List<string> items,
            int level)
        {
            this.ID = ID;
            this.username = username;
            this.iconUrl = iconUrl;
            this.birthdate = birthdate;
            this.likedNews = likedNews;
            this.matchHistory = matchHistory;
            this.elo = elo;
            this.email = email;
            this.registerDate = registerDate;
            this.status = status;
            this.items = items;
            this.level = level;
        }

        public string IconUrl => iconUrl;
        
        public string _ID => ID;
        
        public int Level => level;

        public string Username => username;

        public string Email => email;

        public DateTimeOffset Birthdate => birthdate;

        public List<string> LikedNews => likedNews;

        public List<string> MatchHistory => matchHistory;

        public int Elo => elo;

        public DateTimeOffset RegisterDate => registerDate;

        public Status Status => status;

        public List<string> Items => items;
    }
}