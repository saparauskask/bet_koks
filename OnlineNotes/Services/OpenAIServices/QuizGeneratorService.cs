using OnlineNotes.Data;
using OnlineNotes.Models.Enums;
using OpenAI_API;
using OpenAI_API.Completions;

namespace OnlineNotes.Services.OpenAIServices
{
    public class QuizGeneratorService : IQuizGeneratorService
    {
        private readonly OpenAIAPI _api;

        public QuizGeneratorService()
        {
            var apiKey = FileRepository.ReadApiKey();
            _api = new OpenAIAPI(apiKey?.Key);
        }

        public async Task<string> GenerateQuiz(string noteContents, QuizDifficulty difficulty, int questionsCount)
        {
            try
            {
                string prompt = $"Generate a quiz based on the following options:\n";
                prompt += $"Note Contents: {noteContents}\n";
                prompt += $"Difficulty: {difficulty}\n";
                prompt += $"Number of Questions: {questionsCount}\n";
                prompt += $"Each question should have 'a', 'b', 'c' options\n";

                Random random = new Random();
                for (int i = 1; i <= questionsCount; i++)
                {
                    prompt += $"Q{i}.\n";

                    // Generate random options
                    string option1 = "Option 1";
                    string option2 = "Option 2";
                    string option3 = "Option 3";

                    // Randomly shuffle the options
                    string[] options = { option1, option2, option3 };
                    options = options.OrderBy(x => random.Next()).ToArray();

                    // Select the correct answer randomly
                    char correctAnswer = (char)('a' + Array.IndexOf(options, option2));

                    // Display the shuffled options
                    prompt += $" a. [{options[0]}]. b. [{options[1]}] c. [{options[2]}]\n";

                    // Add the correct answer to the prompt
                    prompt += $"Correct Answer for Q{i}: {correctAnswer}\n";
                }

                prompt += "###";
                CompletionRequest request = new CompletionRequest();
                request.Prompt = prompt;
                request.Model = OpenAI_API.Models.Model.DavinciText;
                request.MaxTokens = 1024;
                request.Temperature = 0.7;

                var result = await _api.Completions.CreateCompletionAsync(request);

               
                return result.ToString();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return "Something went wrong, the quiz could not be generated";
            }
        }
    }
}
