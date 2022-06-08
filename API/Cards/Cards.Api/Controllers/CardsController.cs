using Cards.Api.Data;
using Cards.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cards.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CardsController : Controller
    {
        private readonly CardsDbContext cardsDbContext;

        public CardsController(CardsDbContext cardsDbContext)
        {
            this.cardsDbContext = cardsDbContext;
        }

        [HttpGet]
        public async Task <IActionResult> GetAllCards()
        {
            var cards = await cardsDbContext.Cards.ToListAsync();

            return Ok(cards);
        }

        [HttpGet]
        [Route("{id:guid}")]
        public async Task<IActionResult> GetCardById([FromRoute] Guid id)
        {
            var card = await cardsDbContext.Cards.FirstOrDefaultAsync(card => card.Id == id);
            if(card != null)
            {
                return Ok(card);
            }

            return NotFound("Card Not Found !");
        }

        [HttpPost]
        public async Task<IActionResult> AddCard([FromBody] Card card)
        {
            card.Id = Guid.NewGuid();

            await cardsDbContext.Cards.AddAsync(card);
            await cardsDbContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCardById), new {id = card.Id }, card);
        }

        [HttpPut]
        [Route("{id:guid}")]
        public async Task<IActionResult> UpdateCard([FromRoute] Guid id, [FromBody] Card card)
        {
            var existingCard = await cardsDbContext.Cards.FirstOrDefaultAsync(card => card.Id == id);
            if(existingCard != null)
            {
                existingCard.CardholderName = card.CardholderName;
                existingCard.CardNumber = card.CardNumber;
                existingCard.ExpiryMonth = card.ExpiryMonth;
                existingCard.ExpiryYear = card.ExpiryYear;
                existingCard.CVC = card.CVC;
                await cardsDbContext.SaveChangesAsync();

                return Ok(existingCard);
            }
            
            return NotFound("Card Not Found !");
        }

        [HttpDelete]
        [Route("{id:guid}")]
        public async Task<IActionResult> DeleteCard([FromRoute] Guid id)
        {
            var existingCard = await cardsDbContext.Cards.FirstOrDefaultAsync(card => card.Id == id);
            if(existingCard != null)
            {
                cardsDbContext.Cards.Remove(existingCard);
                await cardsDbContext.SaveChangesAsync();
            
                return Ok(existingCard);
            }
            
            return NotFound("Card Not Found !");
        }
    }
}
