﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WizBooklat.Models
{
    public class Branch
    {
        public int BranchId { get; set; }
        public string Name { get; set; }

        public virtual List<Book> Books { get; set; }
    }

    public class BookTemplate
    {
        public int BookTemplateId { get; set; }
        public string Title { get; set; }
        
        public string Description { get; set; }

        public string ImageLocation { get; set; }

        // Book or Other Resources
        public sbyte Type { get; set; }

        // number of days this book can be loaned; use constant
        public int LoanPeriod { get; set; }
    
        // ISBN key
        public string ISBN { get; set; }

        // open library key
        public string OLKey { get; set; }

        // specific publish date for this book
        public DateTime? PublishDate { get; set; }

        // number of stocks added upon creation
        public int InitialQuantity { get; set; }

        // number of stocks currently distributed
        public int ActualQuantity { get; set; }

        public string Genres { get; set; } // delimit with comma
        public string Authors { get; set; } // delimit with comma

        public virtual List<BookGenre> BookGenres { get; set; }
        public virtual List<BookAuthor> BookAuthors { get; set; }

        public virtual List<Review> Reviews { get; set; }

        public bool IsFeatured { get; set; }
        public DateTime DateCreated { get; set; }
    }
    
    public class Review
    {
        public int ReviewId { get; set; }

        // Over 5 only
        public sbyte Rate { get; set; }

        public string Comment { get; set; }

        public string UserId { get; set; }

        public string FirstName { get; set; }

        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }

        public DateTime DateCreated { get; set; }
    }

    public class Book
    {
        public int BookId { get; set; }

        public string InternalCode { get; set; }

        public int BranchId { get; set; }
        [ForeignKey("BranchId")]
        public virtual Branch Branch { get; set; }

        public int BookTemplateId { get; set; }
        [ForeignKey("BookTemplateId")]
        public virtual BookTemplate BookTemplate { get; set; }

        // use constant
        public sbyte BookStatus { get; set; }
    }

    public class Loan
    {
        public int LoanId { get; set; }

        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }

        public int BookId { get; set; }
        [ForeignKey("BookId")]
        public virtual Book Book { get; set; }
        
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public bool IsLate { get; set; }
        public bool IsDamaged { get; set; }
        public DateTime? LostReported { get; set; }
        public DateTime? LostConfirmed { get; set; }
        public DateTime DateCreated { get; set; }
    }

    public class Genre
    {
        public int GenreId { get; set; }
        public string Name { get; set; }
        public DateTime DateCreated { get; set; }

        public List<BookGenre> BookGenres { get; set; }
    }

    public class BookGenre
    {
        public int BookGenreId { get; set; }
        public int BookTemplateId { get; set; }
        [ForeignKey("BookTemplateId")]
        public virtual BookTemplate BookTemplate { get; set; }

        public int GenreId { get; set; }
        [ForeignKey("GenreId")]
        public virtual Genre Genre { get; set; }
        public DateTime DateCreated { get; set; }
    }

    public class Author
    {
        public int AuthorId { get; set; }
        public string Name { get; set; }
        public string AuthorKey { get; set; }
        public DateTime DateCreated { get; set; }

        public List<BookAuthor> BookAuthors { get; set; }
    }

    public class BookAuthor
    {
        public int BookAuthorId { get; set; }
        public int BookTemplateId { get; set; }
        [ForeignKey("BookTemplateId")]
        public virtual BookTemplate BookTemplate { get; set; }

        public int AuthorId { get; set; }
        [ForeignKey("AuthorId")]
        public virtual Author Author { get; set; }
        public DateTime DateCreated { get; set; }
    }

    public class Visit
    {
        public int VisitId { get; set; }
        public int BranchId { get; set; }
        [ForeignKey("BranchId")]
        public virtual Branch Branch { get; set; }
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }
        public DateTime DateCreated { get; set; }
    }

    public class PointHistory
    {
        public int PointHistoryId { get; set; }
        public int Points { get; set; }

        // Add or Minus
        public sbyte Type { get; set; }
        public string Description { get; set; }

        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }

        public DateTime DateCreated { get; set; }
    }

    public class Rank
    {
        public int RankId { get; set; }
        public string Name { get; set; }
        public int Points { get; set; }
        public string Color { get; set; }
        public virtual List<Reward> Rewards { get; set; }
    }

    public class Reward
    {
        public int RewardId { get; set; }
        public string Title { get; set; }
        public string ImageLocation { get; set; }
        public string Description { get; set; }
        public int RankId { get; set; }
        [ForeignKey("RankId")]
        public virtual Rank Rank { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime DateCreated { get; set; }
    }

    public class Claim
    {
        public int ClaimId { get; set; }

        public int RewardId { get; set; }
        [ForeignKey("RewardId")]
        public virtual Reward Reward { get; set; }

        public string Code { get; set; }
        public DateTime DateCreated { get; set; }
    }

    public class Setting
    {
        public int SettingId { get; set; }
        public string Code { get; set; }
        public string Value { get; set; }
        public DateTime DateCreated { get; set; }
    }
}