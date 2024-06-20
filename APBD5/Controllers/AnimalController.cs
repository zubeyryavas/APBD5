using APBD5.Model;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

[Route("api/[controller]")]
[ApiController]
public class AnimalsController : ControllerBase
{
    private const string ConnectionString = "Server=localhost;Database=APBD5;User Id=SA; Password=Admin@123;";
    private const string POST_QUERY ="INSERT INTO Animal (IdAnimal, Name, Description, Category, Area) VALUES (@IdAnimal, @Name, @Description, @Category, @Area)";
    private const string DELETE_QUERY = "DELETE FROM Animal WHERE IdAnimal = @IdAnimal";
    private const string GET_QUERY = "SELECT * FROM Animal ORDER BY {0}";
    private const string PUT_QUERY ="UPDATE Animal SET Name = @Name, Description = @Description, Category = @Category, Area = @Area WHERE IdAnimal = @IdAnimal";


    [HttpGet]
    public async Task<IActionResult> GetAnimals([FromQuery] string orderBy = "name")
    {
        var validColumns = new List<string> {"name", "description", "category", "area" };
        if (!validColumns.Contains(orderBy.ToLower()))
        {
            return BadRequest("Invalid orderBy parameter.");
        }
  
        var animals = new List<Animal>();

        using (var connection = new SqlConnection(ConnectionString))
        {
            await connection.OpenAsync();
            using (var command = new SqlCommand(string.Format(GET_QUERY, orderBy), connection))
            {
                var dr = await command.ExecuteReaderAsync();
                while (await dr.ReadAsync())
                {
                    animals.Add(new Animal
                    {
                        IdAnimal = (int)dr["IdAnimal"],
                        Name = dr["Name"].ToString(),
                        Description = dr["Description"].ToString(),
                        Category = dr["Category"].ToString(),
                        Area = dr["Area"].ToString()
                    });
                }
            }
        }

        return Ok(animals);
    }

    [HttpPost]
    public async Task<IActionResult> AddAnimal([FromBody] Animal newAnimal)
    {
        if (newAnimal == null)
        {
            return BadRequest("Invalid animal data.");
        }


        using (var connection = new SqlConnection(ConnectionString))
        {
            await connection.OpenAsync();
            using (var com = new SqlCommand(POST_QUERY, connection))
            {
                com.Parameters.AddWithValue("@IdAnimal",newAnimal.IdAnimal);
                com.Parameters.AddWithValue("@Name", newAnimal.Name);
                com.Parameters.AddWithValue("@Description", (object)newAnimal.Description ?? DBNull.Value);
                com.Parameters.AddWithValue("@Category", newAnimal.Category);
                com.Parameters.AddWithValue("@Area", newAnimal.Area);

                await com.ExecuteNonQueryAsync();
            }
        }

        return CreatedAtAction(nameof(GetAnimals), new { id = newAnimal.IdAnimal }, newAnimal);
    }

    [HttpPut("{idAnimal}")]
    public async Task<IActionResult> UpdateAnimal(int idAnimal, [FromBody] Animal animal)
    {
        if (animal == null || animal.IdAnimal != idAnimal)
        {
            return BadRequest("Invalid animal data.");
        }

        using (var con = new SqlConnection(ConnectionString))
        {
            await con.OpenAsync();
            using (var com = new SqlCommand(PUT_QUERY, con))
            {

                com.Parameters.AddWithValue("@IdAnimal", animal.IdAnimal);
                com.Parameters.AddWithValue("@Name", animal.Name);
                com.Parameters.AddWithValue("@Description", (object)animal.Description ?? DBNull.Value);
                com.Parameters.AddWithValue("@Category", animal.Category);
                com.Parameters.AddWithValue("@Area", animal.Area);

                var rowsAffected = await com.ExecuteNonQueryAsync();
                if (rowsAffected == 0)
                {
                    return NotFound();
                }
            }
        }

        return NoContent();
    }

    [HttpDelete("{idAnimal}")]
    public async Task<IActionResult> DeleteAnimal(int idAnimal)
    {

        using (var con = new SqlConnection(ConnectionString))
        {
            await con.OpenAsync();
            using (var com = new SqlCommand(DELETE_QUERY, con))
            {
                com.Parameters.AddWithValue("@IdAnimal", idAnimal);

                var rowsDeleted = await com.ExecuteNonQueryAsync();
                if (rowsDeleted == 0)
                {
                    return NotFound();
                }
            }
        }

        return NoContent();
    }
}