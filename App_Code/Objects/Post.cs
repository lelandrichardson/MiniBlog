using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using MiniBlog.Includes.Data;

namespace MiniBlog.Objects
{
    public class Post
    {
        #region Database Fields

        public int Id { get; set; }
        public int AuthorId { get; set; }
        public string TitleSlug { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime LastModified { get; set; }
        public DateTime? DatePublished { get; set; }
        public char Status { get; set; }
        public bool AllowComments { get; set; }
        public bool IncludeSocialButtons { get; set; }
        public char PostType { get; set; }
        private string tags;
        public string ImageUrl { get; set; }

        #endregion

        private List<string> _tags; 
        public List<string> Tags
        {
            get { return _tags ?? (_tags = (tags ?? "").Split(new[]{'|'},StringSplitOptions.RemoveEmptyEntries).ToList()); }
        } 

        public static class Type
        {
            public const char Blog = 'B';
            public const char Page = 'P';
        }

        public static class PostStatus
        {
            public const char Active = 'A';
            public const char Draft = 'D';
        }

        #region Factory Methods 

        public static Post Get(int id)
        {
            Post p;
            using(var db = Db.GetOpenConnection())
            {
                p = db.Query<Post>("dbo.spu_Post_Get", new { id }).FirstOrDefault();
            }

            return p;
        }

        public static Post GetFromSlug(string slug)
        {
            Post p;
            using (var db = Db.GetOpenConnection())
            {
                p = db.Query<Post>("dbo.spu_Post_GetFromSlug", new { slug }).FirstOrDefault();
            }

            return p;
        }

        public static List<Post> ListRecent(int pageNumber = 1, int perPage = 5)
        {
            List<Post> d;
            using(var db = Db.GetOpenConnection())
            {
                d = db.Query<Post>("dbo.spu_Post_ListRecent",
                         new
                             {
                                 pageNumber,
                                 perPage
                             }, commandType: CommandType.StoredProcedure).ToList();
            }
            return d;
        }

        public static List<Post> ListFromTag(string tag, int pageNumber = 1, int perPage = 5)
        {
            List<Post> d;
            using (var db = Db.GetOpenConnection())
            {
                d = db.Query<Post>("dbo.spu_Post_ListFromTag",
                         new
                         {
                             term = tag,
                             pageNumber,
                             perPage
                         }, commandType: CommandType.StoredProcedure).ToList();
            }
            return d;
        }

        public static List<Post> ListArchiveMonth(int year, int month, int pageNumber = 1, int perPage = 5)
        {
            List<Post> d; 
            using (var db = Db.GetOpenConnection())
            {
                d = db.Query<Post>("dbo.spu_Post_ListFromArchiveMonth",
                         new
                         {
                             year,
                             month,
                             pageNumber,
                             perPage
                         }, commandType: CommandType.StoredProcedure).ToList();
            }
            return d;
        }

        #endregion
    }
}