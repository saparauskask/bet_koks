using OnlineNotes.Data;
using OnlineNotes.Models.Enums;
using OnlineNotes.Services.QuizzesServices;
using OpenAI_API;
using OpenAI_API.Completions;

namespace OnlineNotes.Services.OpenAIServices
{
    public class QuizGeneratorService : IQuizGeneratorService
    {
        private readonly OpenAIAPI _api;
        private readonly ILogger<QuizGeneratorService> _logger;

        public QuizGeneratorService(ILogger<QuizGeneratorService> logger)
        {
            var apiKey = FileRepository.ReadApiKey();
            _api = new OpenAIAPI(apiKey?.Key);
            _logger = logger;
        }

        public async Task<string> GenerateQuiz(string noteContents, QuizDifficulty difficulty, int questionsCount)
        {
            //try
            //{
            //    string prompt = $"Generate a quiz based on this text: {noteContents}\n";
            //    prompt += "{";
            //    prompt += $"Difficulty: {difficulty}\n";
            //    prompt += $"Number of Questions: {questionsCount}\n";
            //    prompt += $"Each question should have 'a', 'b', 'c' question options.";
            //    prompt += "} \n\n\n";
            //    prompt += "{";

            //    Random random = new Random();
            //    for (int i = 1; i <= questionsCount; i++)
            //    {
            //        prompt += $"Question{i}. (write a question here)\n";

            //        // Generate random options
            //        string option1 = "Option 1";
            //        string option2 = "Option 2";
            //        string option3 = "Option 3";

            //        // Randomly shuffle the options
            //        string[] options = { option1, option2, option3 };
            //        options = options.OrderBy(x => random.Next()).ToArray();

            //        // Select the correct answer randomly
            //        char correctAnswer = (char)('a' + Array.IndexOf(options, option2));

            //        // Display the shuffled options
            //        prompt += $" a. [{options[0]}]. b. [{options[1]}] c. [{options[2]}].\n";

            //        // Add the correct answer to the prompt
            //        prompt += $"Correct Answer for question Q{i} is: {correctAnswer}.\n\n";
            //    }
            //    prompt += "}";
            //    CompletionRequest request = new()
            //    {
            //        Prompt = prompt,
            //        Model = OpenAI_API.Models.Model.DavinciText,
            //        MaxTokens = 2048,
            //        Temperature = 0.9
            //    };

            //    string generatedQuiz;
            //    int counter = 0;
            //    do
            //    { // probably there is a more efficient solution to the problem of empty string being returned
            //        var result = await _api.Completions.CreateCompletionAsync(request);
            //        generatedQuiz = result.ToString();
            //        ++counter;
            //    } while (string.IsNullOrEmpty(generatedQuiz) || counter <=10);

            //    return generatedQuiz;
            //}
            //catch (Exception ex)
            //{
            //    _logger.LogError(ex, "An error occurred in GenerateQuiz: {ErrorMessage}", ex.Message);
            //    return "Something went wrong, the quiz could not be generated. Either your submitted note was to long or ChatGPT was unable to generate the quiz";
            //}
            return "This method for generating quizzes is not implemented";
        }
    }
}
