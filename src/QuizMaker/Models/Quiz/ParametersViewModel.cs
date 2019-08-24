using System.Text.Json.Serialization;

namespace QuizMaker.Models.Quiz
{
    public class ParametersViewModel
    {
        /// <summary>
        /// Question can be answered multiple times.
        /// </summary>
        [JsonPropertyName("allowMultipleResponses")]
        public bool AllowMultipleResponses { get; set; }

        /// <summary>
        /// Questions options can be multiselected (checkbox)
        /// or single selected (radiobutton).
        /// </summary>
        [JsonPropertyName("multiSelect")]
        public bool MultiSelect { get; set; }

        /// <summary>
        /// Randomize order of the questions options.
        /// </summary>
        [JsonPropertyName("randomizeOrder")]
        public bool RandomizeOrder { get; set; }
    }
}
