﻿using OnlineNotes.Data;
using OnlineNotes.Models.Enums;
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

        public string FakeGenerateQuiz(string noteTitle) // DEPRECATED
        {
            switch (noteTitle) // Note, that these quizzes were really generated by chatgpt api previously
            {
                case "Stork":
                    return "Q1. What is the scientific name for the family to which storks belong?\n" +
                           " a. Ciconiiformes. b. Ciconiidae. c. Herons.\n" +
                           "Correct Answer for question Q1 is: b.\n\n" +
                           "Q2. What is the process known as that herons, spoonbills, and ibises use to clean off fish slime?\n" +
                           " a. Clattering. b. Migrating. c. Powdering down.\n" +
                           "Correct Answer for question Q2 is: c.\n\n" +
                           "Q3. What type of habitats do storks prefer to live in?\n" +
                           " a. Fish-ridden. b. Drier. c. Wetter.\n" +
                           "Correct Answer for question Q3 is: b.\n\n" +
                           "Q4. What is the term used to refer to groups of storks?\n" +
                           " a. Musters. b. Phalanx. c. Flocks.\n" +
                           "Correct Answer for question Q4 is: a.";
                case "Overview of helpers":
                    return "Q1. What is the main advantage of using a helper?\n" +
                           "a. Easier to maintain code. b. Easier to read code. c. Easier to create code.\n" +
                           "Correct Answer for question Q1 is: a.\n\n" +
                           "Q2. What is the recommended way to create a note item in an ASP NET Web Page?\n" +
                           "a. Using an existing helper. b. Creating a new helper. c. Using a <div> element.\n" +
                           "Correct Answer for question Q2 is: b.\n\n" +
                           "Q3. Where can you find a list of the built-in helpers in ASP NET Web Pages?\n" +
                           "a. ASP NET API Quick Reference. b. ASP NET Web Pages documentation. c. Online tutorial websites.\n" +
                           "Correct Answer for question Q3 is: a.\n\n" +
                           "Q4. How can you make your codes simpler and easier to read when using a helper?\n" +
                           "a. Write shorter and concise codes. b. Insert a single line of code anywhere you need it. c. Put the codes in different pages.\n" +
                           "Correct Answer for question Q4 is: b.";
                case "What is Neuroscience?":
                    return "Q1: What is the study of neuroscience also known as?\n" +
                           " a. Neural Science. b. Biology. c. Mathematics.\n" +
                           "Correct Answer for question Q1 is: a.\n\n" +
                           "Q2: What does neuroscience focus on?\n" +
                           " a. Cognitive functions. b. Neurological disorders. c. Computer science.\n" +
                           "Correct Answer for question Q2 is: a.\n\n" +
                           "Q3: What is the difference between neuroscience and neurobiology?\n" +
                           " a. Neuroscience is wider in scope. b. Neurobiology looks at the biology of the nervous system. c. Neuroscience is more interdisciplinary.\n" +
                           "Correct Answer for question Q3 is: b.\n\n" +
                           "Q4: What aspects of the nervous system do neuroscientists study?\n" +
                           " a. Molecular. b. Computational. c. All of the above.\n" +
                           "Correct Answer for question Q4 is: c.\n\n" +
                           "Q5: What fields are neuroscientists involved in today?\n" +
                           " a. Mathematics. b. Medicine. c. Psychology.\n" +
                           "Correct Answer for question Q5 is: c.";
                default:
                    return "not implemented";
            }
        }

        public async Task<string> GenerateQuiz(string noteContents, QuizDifficulty difficulty, int questionsCount) // Should work fine by now
        {
            try
            {
                string prompt = $"Generate a quiz based on this text: {noteContents}\n";
                prompt += $"Difficulty: {difficulty}\n";
                prompt += $"Number of Questions: {questionsCount}\n";
                prompt += $"Each question should have 'a', 'b', 'c' question options.\n";
                Random random = new Random();
                for (int i = 1; i <= questionsCount; i++)
                {
                    prompt += $"Q{i}. (write a question here)\n";

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
                    prompt += $" a. [{options[0]}]. b. [{options[1]}]. c. [{options[2]}].\n";

                    // Add the correct answer to the prompt
                    prompt += $"Correct Answer for question Q{i} is: {correctAnswer}.\n\n";
                }
                prompt += "###";
                CompletionRequest request = new()
                {
                    Prompt = prompt,
                    Model = OpenAI_API.Models.Model.DavinciText,
                    MaxTokens = 2048,
                    Temperature = 0.9
                };

                string generatedQuiz;
                var result = await _api.Completions.CreateCompletionAsync(request);
                generatedQuiz = result.ToString();

                return generatedQuiz;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in GenerateQuiz: {ErrorMessage}", ex.Message);
                return "";
            }
        }
    }
}
