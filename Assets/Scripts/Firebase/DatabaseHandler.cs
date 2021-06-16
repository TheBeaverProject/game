using System.Collections.Generic;
using Firebase.Data;
using Proyecto26;
using UnityEngine;

namespace Firebase
{
    public class DatabaseHandler : MonoBehaviour
    {
        protected const string ProjectId = "beaver-ea0ea";
        protected const string DatabaseId = "(default)";
        protected const string DatabaseUrl = "https://firestore.googleapis.com/v1";
        
        public delegate void SuccessCallback(bool success);
        
        public delegate void GetUserCallback(User user);
        
        /// <summary>
        /// Retrieves a user from Firestore with the associated userId
        /// </summary>
        /// <param name="userId">id of the user's document in Firestore</param>
        /// <param name="callback">Callback containing the retrieved data</param>
        /// <returns>Object containing the user's data</returns>
        public static void GetUserById(string userId, GetUserCallback callback)
        {
            RestClient.Get($"{DatabaseUrl}/projects/{ProjectId}/databases/{DatabaseId}/documents/users/{userId}")
                .Then(userRes =>
                {
                    var userResponseJson = userRes.Text;

                    var firebaseUserDocument = FirebaseUserDocument.FromJson(userResponseJson);

                    var likedNews = new List<string>();

                    if (firebaseUserDocument?.Fields?.LikedNews?.ArrayValue?.Values != null)
                    {
                        foreach (var value in firebaseUserDocument?.Fields?.LikedNews?.ArrayValue?.Values)
                        {
                            likedNews.Add(value.StringValue);
                        }
                    }
                    
                    var matchHistory = new List<string>();

                    if (firebaseUserDocument?.Fields?.MatchHistory?.ArrayValue?.Values != null)
                    {
                        foreach (var value in firebaseUserDocument?.Fields?.MatchHistory?.ArrayValue?.Values)
                        {
                            matchHistory.Add(value.StringValue);
                        }
                    }

                    callback(new User(
                        userId,
                        firebaseUserDocument.Fields.Username.StringValue,
                        firebaseUserDocument.Fields.Email.StringValue,
                        firebaseUserDocument.Fields?.IconUrl?.StringValue != null ? firebaseUserDocument.Fields.IconUrl.StringValue : "https://thebeaverproject.tk/logo192.png",
                        firebaseUserDocument.Fields.Birthdate.TimestampValue,
                        likedNews,
                        matchHistory,
                        (int) firebaseUserDocument.Fields.Elo.IntegerValue,
                        firebaseUserDocument.Fields.RegisterDate.TimestampValue,
                        (Status) firebaseUserDocument.Fields.Status.IntegerValue,
                        null,
                        (int) firebaseUserDocument.Fields.Level.IntegerValue
                    ));
                })
                .Catch(err =>
                {
                    Debug.LogErrorFormat($"Firebase.DatabaseHandler: Exception when trying to retrieve the user {userId} from the database: {err}");
                    callback(null);
                });
        }

        public delegate void GetAllNewsCallback(List<News> newsList);

        public static void GetAllNews(GetAllNewsCallback callback)
        {
            
            RestClient.Get($"{DatabaseUrl}/projects/{ProjectId}/databases/{DatabaseId}/documents/news")
                .Then(userRes =>
                {
                    var userResponseJson = userRes.Text;

                    var firebaseNewsDocumentAll = FirebaseNewsDocumentAll.FromJson(userResponseJson);

                    var newsList = new List<News>();

                    foreach (var firebaseNewsDocument in firebaseNewsDocumentAll.Documents)
                    {
                        var previewImage = firebaseNewsDocument.Fields.PreviewImage?.StringValue != null
                            ? firebaseNewsDocument.Fields.PreviewImage.StringValue
                            : "";
                        
                        newsList.Add(new News(
                            firebaseNewsDocument.Fields.Author.StringValue,
                            firebaseNewsDocument.Fields.Title.StringValue,
                            firebaseNewsDocument.Fields.Content.StringValue,
                            previewImage,
                            firebaseNewsDocument.Fields.Url.StringValue,
                            (int) firebaseNewsDocument.Fields.Likes.IntegerValue));
                    }
                    
                    callback(newsList);
                })
                .Catch(err =>
                {
                    Debug.LogErrorFormat($"Firebase.DatabaseHandler: Exception when trying to get the news: {err}");
                    callback(null);
                });
        }
    }
}