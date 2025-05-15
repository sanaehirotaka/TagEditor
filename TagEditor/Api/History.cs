using Microsoft.AspNetCore.Mvc;

namespace TagEditor.Api
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class History(Storage storage) : ControllerBase
    {
        [HttpGet]
        [Produces("application/json")]
        public IEnumerable<PromptHistory> Get()
        {
            var histories = storage.Get<PromptHistories>() ?? new PromptHistories();
            return histories.Histories;
        }

        [HttpPost]
        [Produces("application/json")]
        public IEnumerable<PromptHistory> Push([FromForm] string? Title, [FromForm] string? Prompt, [FromForm] string? Negative)
        {
            var histories = storage.Get<PromptHistories>() ?? new PromptHistories();

            histories.Histories.Add(new()
            {
                Title = Title,
                Prompt = Prompt,
                Negative = Negative
            });
            if (histories.Histories.Count > 100)
            {
                histories.Histories = histories.Histories.TakeLast(100).ToList();
            }
            storage.Set(histories);

            return histories.Histories;
        }

        [HttpPost]
        [Produces("application/json")]
        public IEnumerable<PromptHistory> Clear()
        {
            storage.Set(new PromptHistories());

            return [];
        }
    }

    public class PromptHistories
    {
        public List<PromptHistory> Histories { get; set; } = [];
    }

    public class PromptHistory
    {
        public string? Title { get; set; }

        public string? Prompt { get; set; }

        public string? Negative { get; set; }
    }
}
