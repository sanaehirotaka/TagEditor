using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.Google;
using System.Text.Json;

namespace TagEditor;

/// <summary>
/// Semantic Kernel を使用してチャット補完サービスを初期化し、管理するクラス。
/// </summary>
public class ChatCompletionService
{
    private static readonly string API_KEY = Environment.GetEnvironmentVariable("GEMINI_API_KEY")!;

    private static readonly string MODEL = Environment.GetEnvironmentVariable("GEMINI_MODEL")!;

    private readonly Kernel kernel;

    /// <summary>
    /// ChatCompletionService クラスの新しいインスタンスを初期化します。
    /// Kernel を構築し、Google AI Gemini コネクタを登録します。
    /// </summary>
    public ChatCompletionService()
    {
        kernel = Kernel.CreateBuilder()
            .AddGoogleAIGeminiChatCompletion(MODEL, API_KEY)
            .Build();
    }

    /// <summary>
    /// 新しいチャットセッションを開始します。
    /// </summary>
    /// <returns>新しい ChatTimeLime オブジェクト。これを使用してチャットのやり取りを行います。</returns>
    public ChatTimeLime StartChat()
    {
        return new ChatTimeLime(kernel);
    }

    public ResponseSchema<T> StartResponseSchema<T>()
    {
        return new ResponseSchema<T>(kernel);
    }

    public class ResponseSchema<T>(Kernel kernel) : ChatTimeLime(kernel)
    {
        public async Task<T?> GetResponse()
        {
            return JsonSerializer.Deserialize<T>(await GetChatMessageContentsAsync());
        }

        protected override GeminiPromptExecutionSettings GetPromptExecutionSettings()
        {
            var settings = base.GetPromptExecutionSettings();
            settings.ResponseMimeType = "application/json";
            settings.ResponseSchema = typeof(T);
            return settings;
        }
    }

    /// <summary>
    /// 個々のチャットセッションの状態（会話履歴）と、そのセッション内での AI との対話を管理する内部クラス。
    /// </summary>
    /// <param name="kernel">このセッションで使用する Semantic Kernel オブジェクト。</param>
    public class ChatTimeLime(Kernel kernel)
    {
        // このチャットセッションの会話履歴を保持するプロパティ。
        // ユーザーとアシスタントのメッセージが時系列で格納される。
        public ChatHistory History { get; } = [];

        public void AddSystemMessage(string content) => History.AddSystemMessage(content);

        public void AddUserMessage(string content) => History.AddUserMessage(content);

        /// <summary>
        /// 現在のチャット履歴に基づいて AI に応答を生成させ、履歴に追加し、応答テキストを返します。
        /// ストリーミング応答の場合、応答が完了するまで複数回 AI を呼び出す場合があります。
        /// </summary>
        /// <returns>AI によって生成された応答のテキスト。</returns>
        public async Task<string> GetChatMessageContentsAsync()
        {
            // Kernel から登録されているチャット補完サービス（Google Gemini コネクタ）を取得
            var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();

            // 今回の AI 応答のみを一時的に収集するための履歴オブジェクト
            var complations = new ChatHistory(); // 変数名 'complations' は 'completions' のタイポの可能性あり

            // AI の応答が完了するまでループ（ストリーミング応答などを考慮）
            // 応答メッセージのメタデータに FinishReason="stop" が含まれるまで繰り返す
            while (!complations.Any(mbox => "stop" == mbox?.Metadata?["FinishReason"]?.ToString()?.ToLower()))
            {
                // 現在の全履歴を渡して AI に応答生成をリクエスト
                // Gemini 固有の実行設定オブジェクトを使用
                // この呼び出しは、応答のチャンクを返す可能性がある
                var completion = await chatCompletionService.GetChatMessageContentsAsync(History, GetPromptExecutionSettings(), kernel);

                History.AddRange(completion);
                complations.AddRange(completion);
            }

            return complations.Single(content => content.Role == AuthorRole.Assistant).Content!;
        }

        protected virtual GeminiPromptExecutionSettings GetPromptExecutionSettings() => new()
        {
            ToolCallBehavior = GeminiToolCallBehavior.AutoInvokeKernelFunctions,
            SafetySettings = [
                new(GeminiSafetyCategory.Harassment, GeminiSafetyThreshold.BlockNone),
                new(GeminiSafetyCategory.DangerousContent, GeminiSafetyThreshold.BlockNone),
                new(GeminiSafetyCategory.SexuallyExplicit, GeminiSafetyThreshold.BlockNone),
            ],
        };
    }
}
