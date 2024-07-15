using Book_store.MyHelpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;

namespace Book_store.Pages.Admin.Books
{
    [RequireAuth(RequiredRole = "admin")]
    public class CreateModel : PageModel
    {
        [BindProperty]
        [Required(ErrorMessage = "The title is required")]
        [MaxLength(100, ErrorMessage ="The Title cannot exceed 100 characters")]
        public string Title { get; set; } = "";
        [BindProperty]
        [Required(ErrorMessage = "The Author is required")]
        [MaxLength(255, ErrorMessage = "The Authors cannot exceed 255 characters")]
        public string Authors { get; set; } = "";
        [BindProperty]
        [Required(ErrorMessage = "The ISBN is required")]
        [MaxLength(20, ErrorMessage = "The ISBN cannot exceed 20 characters")]
        public string ISBN { get; set; } = "";
        [BindProperty]
        [Required(ErrorMessage = "The Number of pages is required")]
        [Range(1, 1500, ErrorMessage = "The Number of Pages must be in the range of 1 to 1500")]
        public int NumPages { get; set; }
        [BindProperty]
        [Required(ErrorMessage = "The Price is required")]
        public decimal Price { get; set; }
        [BindProperty, Required]
        public string Category { get; set; } = "";
        [BindProperty]
        [MaxLength(1000, ErrorMessage = "The Description cannot exceed 1000 characters")]
        public string? Description { get; set; } = "";
        [BindProperty]
        [Required(ErrorMessage = "The Image file is required")]
        public IFormFile ImageFile { get; set; }

        public string errorMessage = "";
        public string successMessage = "";

        private IWebHostEnvironment webHostEnvironment;
        public CreateModel (IWebHostEnvironment env)
        {
            webHostEnvironment = env;
        }


        public void OnGet()
        {
        }

        public void OnPost() 
        {
            if (!ModelState.IsValid)
            {
                errorMessage = "Data validation failed";
                return;
            }

            //successfull data validation

            if (Description == null) Description = "";

            //save the image file on the server
            string newFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            newFileName += Path.GetExtension(ImageFile.FileName);

            string iamgeFolder = webHostEnvironment.WebRootPath + "/images/books/";
            
            string imageFullPath = Path.Combine(iamgeFolder, newFileName);

            using (var stream = System.IO.File.Create(imageFullPath))
            {
                ImageFile.CopyTo(stream);
            }


            //save the new book in the database
            try
            {
                string connectionString = "Data Source=.\\sqlexpress;Initial Catalog=Bookshelf;Integrated Security=True;Encrypt=False";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "INSERT INTO books " +
                        "(title, authors, isbn, num_pages, price, category, description, image_filename) VALUES " +
                        "(@title, @authors, @isbn, @num_pages, @price, @category, @description, @image_filename);";

                    using (SqlCommand command = new SqlCommand(sql, connection)) 
                    {
                        command.Parameters.AddWithValue("@title", Title);
                        command.Parameters.AddWithValue("@authors", Authors);
                        command.Parameters.AddWithValue("@isbn", ISBN);
                        command.Parameters.AddWithValue("@num_pages", NumPages);
                        command.Parameters.AddWithValue("@price", Price);
                        command.Parameters.AddWithValue("@category", Category);
                        command.Parameters.AddWithValue("@description", Description);
                        command.Parameters.AddWithValue("@image_filename", newFileName);

                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return;
            }

            successMessage = "Data saved successfully";
            Response.Redirect("/Admin/Books/Index");
        }    
    }
}
