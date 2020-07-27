using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WizBooklat.Models.ViewModels
{
    public class Publisher
    {
        public string name { get; set; }

    }

    public class Identifiers
    {
        public List<string> lccn { get; set; }
        public List<string> openlibrary { get; set; }
        public List<string> isbn_10 { get; set; }
        public List<string> oclc { get; set; }
        public List<string> goodreads { get; set; }
        public List<string> project_gutenberg { get; set; }
        public List<string> librarything { get; set; }

    }

    public class Classifications
    {
        public List<string> dewey_decimal_class { get; set; }
        public List<string> lc_classifications { get; set; }

    }

    public class Cover
    {
        public string small { get; set; }
        public string large { get; set; }
        public string medium { get; set; }

    }

    public class SubjectPlace
    {
        public string url { get; set; }
        public string name { get; set; }

    }

    public class Subject
    {
        public string url { get; set; }
        public string name { get; set; }

    }

    public class SubjectPeople
    {
        public string url { get; set; }
        public string name { get; set; }

    }

    public class Author
    {
        public string url { get; set; }
        public string name { get; set; }

    }

    public class PublishPlace
    {
        public string name { get; set; }

    }

    public class SubjectTime
    {
        public string url { get; set; }
        public string name { get; set; }

    }

    public class BookData
    {
        public List<Publisher> publishers { get; set; }
        public string pagination { get; set; }
        public Identifiers identifiers { get; set; }
        public Classifications classifications { get; set; }
        public string title { get; set; }
        public string url { get; set; }
        public string notes { get; set; }
        public int number_of_pages { get; set; }
        public Cover cover { get; set; }
        public List<SubjectPlace> subject_places { get; set; }
        public List<Subject> subjects { get; set; }
        public List<SubjectPeople> subject_people { get; set; }
        public string key { get; set; }
        public List<Author> authors { get; set; }
        public string publish_date { get; set; }
        public string by_statement { get; set; }
        public List<PublishPlace> publish_places { get; set; }
        public List<SubjectTime> subject_times { get; set; }

    }

    public class OpenLibraryBookData
    {
        public string ISBN { get; set; }
        public BookData BookData { get; set; }
    }
}