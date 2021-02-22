namespace Firebase.Data
{
    public class News
    {
        private string author;
        private string content;
        private int likes;
        private string previewImage;
        private string title;
        private string url;

        public News(string author, string title, string content, string previewImage, string url, int likes)
        {
            this.author = author;
            this.title = title;
            this.content = content;
            this.previewImage = previewImage;
            this.url = url;
            this.likes = likes;
        }

        public string Author => author;

        public string Content => content;

        public string PreviewImage => previewImage;

        public int Likes => likes;

        public string Title => title;

        public string URL => url;
    }
}