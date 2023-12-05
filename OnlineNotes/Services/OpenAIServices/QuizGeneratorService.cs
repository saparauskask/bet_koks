using OnlineNotes.Data;
using OnlineNotes.Models.Enums;
using OpenAI_API;

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

                for (int i = 1; i <= questionsCount; i++)
                {
                    prompt += $"Q{i}. What is ";
                    prompt += $"\n a. [Option 1]. b. [Option 2] c. [Option 3]\n";
                }

                prompt += "###";

                var result = await _api.Completions.CreateCompletionAsync(prompt, temperature: 0.9);

               
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
