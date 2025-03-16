using Microsoft.ML.Data;

namespace TalkVN.Application.MachineLearning.Models
{
    public class PostPredictionModel
    {
        [ColumnName("Score")]
        public float Score { get; set; }
    }
}
