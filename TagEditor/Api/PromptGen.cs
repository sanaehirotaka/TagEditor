using Microsoft.AspNetCore.Mvc;

namespace TagEditor.Api
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class PromptGen(ChatCompletionService chatCompletionService, Storage storage) : ControllerBase
    {
        [HttpGet]
        [Produces("application/json")]
        public async Task<List<Prompt>> Random()
        {
            var promptConfig = storage.Get<PromptConfig>() ?? new PromptConfig();
            var chat = chatCompletionService.StartResponseSchema<List<Prompt>>();
            chat.AddSystemMessage(promptConfig.ApiPromptGenRandomSystemPrompt1);
            chat.AddUserMessage(promptConfig.ApiPromptGenRandomUserPrompt1.Replace("{tags}", string.Join("\r\n", new BuildinTags().RandomSelectTag(120).Select(tag => tag.Text))));
           return await chat.GetResponse() ?? [];
        }

        [HttpPost]
        [Produces("application/json")]
        public async Task<List<Prompt>> Generate([FromForm] string? BeforePrompt, [FromForm] string? Request)
        {
            var promptConfig = storage.Get<PromptConfig>() ?? new PromptConfig();
            var chat = chatCompletionService.StartResponseSchema<List<Prompt>>();
            chat.AddSystemMessage(promptConfig.ApiPromptGenGenerateSystemPrompt1);
            chat.AddUserMessage(promptConfig.ApiPromptGenGenerateUserPrompt1.Replace("{beforePrompt}", BeforePrompt ?? "").Replace("{request}", Request ?? ""));
            return await chat.GetResponse() ?? [];
        }

        [HttpPost]
        [Produces("application/json")]
        public async Task<List<TagCategory>> CompletionTags([FromForm] string? Prompt)
        {
            var promptConfig = storage.Get<PromptConfig>() ?? new PromptConfig();
            var chat = chatCompletionService.StartResponseSchema<List<TagCategory>>();
            chat.AddSystemMessage(promptConfig.ApiPromptGenCompletionTagsSystemPrompt1);
            chat.AddUserMessage(promptConfig.ApiPromptGenCompletionTagsUserPrompt1.Replace("{prompt}", Prompt ?? ""));
            return await chat.GetResponse() ?? [];
        }

        [HttpPost]
        [Produces("application/json")]
        public async Task<Prompt?> Title([FromForm] string? Prompt)
        {
            var promptConfig = storage.Get<PromptConfig>() ?? new PromptConfig();
            var chat = chatCompletionService.StartResponseSchema<Prompt>();
            chat.AddSystemMessage(promptConfig.ApiPromptGenTitleSystemPrompt1);
            chat.AddUserMessage(promptConfig.ApiPromptGenTitleUserPrompt1.Replace("{prompt}", Prompt ?? ""));
            return await chat.GetResponse();
        }
    }
    public class Prompt
    {
        public string PromptText { get; set; } = default!;

        public string Description { get; set; } = default!;
    }


    public class TagCategory
    {
        public string Category { get; set; } = default!;

        public List<string> Tags { get; set; } = [];
    }
}
