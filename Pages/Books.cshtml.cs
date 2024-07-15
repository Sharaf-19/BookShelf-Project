using Book_store.Pages.Admin.Books;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace Book_store.Pages
{
    [BindProperties(SupportsGet = true)]
    public class BooksModel : PageModel
    {
        public string? Search {  get; set; }
        public string PriceRange { get; set; } = "any";
        public string PageRange { get; set; } = "any";
        public string Category { get; set; } = "any";

        public List<BookInfo> listBooks = new List<BookInfo>(); 
        public void OnGet()
        {
            try
            {
                string connectionString = "Data Source=.\\sqlexpress;Initial Catalog=Bookshelf;Integrated Security=True;Encrypt=False";

                using(SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string sql = "SELECT * FROM books";
                    sql += " WHERE (title LIKE @search OR authors LIKE @search)";

                    if (PriceRange.Equals("0_50"))
                    {
                        sql += " AND price <= 50";
                    }
                    else if (PriceRange.Equals("50_100"))
                    {
                        sql += " AND price >= 50 AND price <=100";
                    }
                    else if (PriceRange.Equals("above100"))
                    {
                        sql += " AND price >=100";
                    }
                    
                    
                    if (PageRange.Equals("0_100"))
                    {
                        sql += " AND num_pages <= 100";
                    }
                    else if (PageRange.Equals("100_299"))
                    {
                        sql += " AND num_pages >= 100 AND price <=299";
                    }
                    else if (PageRange.Equals("above300"))
                    {
                        sql += " AND num_pages >=300";
                    }

                    if (!Category.Equals("any")){
                        sql += " AND category=@category";
                    }


                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@search", "%" + Search + "%");
                        command.Parameters.AddWithValue("@category", Category);

                        using(SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                BookInfo bookInfo = new BookInfo();

                                bookInfo.Id = reader.GetInt32(0);
                                bookInfo.Title = reader.GetString(1);
                                bookInfo.Authors = reader.GetString(2);
                                bookInfo.Isbn = reader.GetString(3);
                                bookInfo.NumPages = reader.GetInt32(4);
                                bookInfo.Price = reader.GetDecimal(5);
                                bookInfo.Category = reader.GetString(6);
                                bookInfo.Description = reader.GetString(7);
                                bookInfo.ImageFileName = reader.GetString(8);
                                bookInfo.CreatedAt = reader.GetDateTime(9).ToString("MM/dd/yyyy");

                                listBooks.Add(bookInfo);
                            }
                        }   
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
