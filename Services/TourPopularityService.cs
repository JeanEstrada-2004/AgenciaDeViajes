using Microsoft.ML;
using Microsoft.ML.Data;
using System.IO;

namespace AgenciaDeViajes.Services
{
    public class TourPopularityService
    {
        private readonly PredictionEngine<TourDataInput, TourPredictionOutput> _predictionEngine;

        public TourPopularityService()
        {
            var mlContext = new MLContext();

            var modelPath = Path.Combine(Directory.GetCurrentDirectory(), "modelo_tours.zip");
            ITransformer mlModel = mlContext.Model.Load(modelPath, out var modelInputSchema);

            _predictionEngine = mlContext.Model.CreatePredictionEngine<TourDataInput, TourPredictionOutput>(mlModel);
        }

        public TourPredictionOutput PredecirPopularidad(int destinoId, float cantidadBoletos, float precioTotal)
        {
            var input = new TourDataInput
            {
                DestinoId = destinoId,
                CantidadBoletos = cantidadBoletos,
                PrecioTotal = precioTotal
            };

            return _predictionEngine.Predict(input);
        }
    }
    

    public class TourDataInput
    {
        public float DestinoId { get; set; }
        public float CantidadBoletos { get; set; }
        public float PrecioTotal { get; set; }
    }

    public class TourPredictionOutput
    {
        [ColumnName("PredictedLabel")]
        public bool EsPopular { get; set; }
        public float Probability { get; set; }
        public float Score { get; set; }
    }
}
