﻿namespace TagEditor;

public class PromptConfig
{
    public string ApiPromptGenRandomSystemPrompt1 { get; set; } = @"あなたはStable Diffusion用のプロンプトを生成するためのアシスタントです";

    public string ApiPromptGenRandomUserPrompt1 { get; set; } = @"次のタグの一覧から適切なタグを複数ピックアップして生成したプロンプトに簡潔な説明文(日本語)を追加してください。そしてそれを3つ以上複数生成しててください。

例:
1girl, solo, black_hair, swimsuit, ponytail
水着を着た黒髪でポニーテールの女性


タグの一覧:
```
{tags}
```
";

    public string ApiPromptGenGenerateSystemPrompt1 { get; set; } = @"あなたはStable Diffusion用のプロンプトを生成するためのアシスタントです";
    public string ApiPromptGenGenerateUserPrompt1 { get; set; } = @"元プロンプトに対して要望に沿った新しいプロンプト候補と簡潔な説明文(日本語)を生成してください。そしてそれを3つ以上複数生成しててください。

元プロンプト:`{beforePrompt}`
要望: `{request}`

例:
1girl, solo, black_hair, swimsuit, ponytail
水着を着た黒髪でポニーテールの女性
";
    public string ApiPromptGenCompletionTagsSystemPrompt1 { get; set; } = @"あなたはStable Diffusion用のプロンプトを生成するためのアシスタントです";
    public string ApiPromptGenCompletionTagsUserPrompt1 { get; set; } = @"次のプロンプトに追加されるタグの候補をいくつか提示してください。

`{prompt}`

候補のタグは外観、ポーズ、構図、場所などカテゴリごとに分類してください。

例:
```json
[
    {
        ""category"": ""外観"",
        ""tags"": [
            ""blonde_hair"",
            ""blue_eyes"",
            ""fair_skin""
        ]
    },
    {
        ""category"": ""ポーズ"",
        ""tags"": [
            ""hand_up"",
            ""arm_up"",
            ""signature""
        ]
    },
```
";

    public string ApiPromptGenTitleSystemPrompt1 { get; set; } = @"あなたはStable Diffusion用のプロンプトを生成するためのアシスタントです";
    public string ApiPromptGenTitleUserPrompt1 { get; set; } = @"プロンプトに対して簡潔な説明文(日本語)を生成してください。

プロンプト:`{prompt}`

例:
1girl, solo, black_hair, swimsuit, ponytail
水着を着た黒髪でポニーテールの女性
";
}
