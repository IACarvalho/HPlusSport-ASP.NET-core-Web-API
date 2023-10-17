using HPlusSport.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HPlusSport.API.Controllers; // A partir do .Net 6 pode usar o pacote sem um bloco de código

[Route("api/[controller]")]
[ApiController]
public class ProductsController : ControllerBase
{

    private readonly ShopContext _context;

    public ProductsController(ShopContext context)
    {
        _context = context;
        _context.Database.EnsureCreated();
    }

    [HttpGet]
    public async Task<ActionResult> GetAllProducts()
    {
        return Ok(await _context.Products.ToListAsync());
    }

    [HttpGet("{id}")] // Basicamente cria uma variavel que recebe um valor da URL, mas tem que ser declarada na assinatura do metodo
    public async Task<ActionResult> GetProduct(int id) // ActionResult é o retorno mais genérico para usar em uma resposta
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null)
        {
            return NotFound(); // retorna status 404 com erro not found
        }
        return Ok(product); // retorna o status 200 com o item resultado
    }

    [HttpPost]
    public async Task<ActionResult<Product>> PostProduct(Product product)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest();
        }

        _context.Products.Add(product);
        await _context.SaveChangesAsync();
        return CreatedAtAction(
            "GetProduct",
            new { id = product.Id },
            product
        );
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> PutProduct(int id, [FromBody] Product product)
    {

        if (product.Id != id)
        {
            return BadRequest();
        }

        _context.Entry(product).State = EntityState.Modified;
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!_context.Products.Any(p => p.Id == id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<Product>> DeleteProduct(int id)
    {
        var product = await _context.Products.FindAsync(id);

        if (product == null) return NotFound();

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();

        return product;
    }

    // Chalenge
    [HttpPost]
    [Route("Delete")]
    public async Task<ActionResult<List<Product>>> DeleteSeveralProducts([FromBody] List<int> ids)
    {
        var products = new List<Product>();

        foreach (int id in ids)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                products.Add(product);

            }
            else return NotFound();
        }

        _context.Products.RemoveRange(products);
        await _context.SaveChangesAsync();
        

        return products;
    }
}