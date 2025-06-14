using Microsoft.ML;
using System;
using System.IO;

namespace AgenciaDeViajes.ML
{
    public class SentimientoPredictionService
    {
        private readonly MLContext _mlContext;
        private readonly ITransformer _modelo;

        public SentimientoPredictionService(string modeloPath)
        {
            _mlContext = new MLContext();

            if (!File.Exists(modeloPath))
                throw new FileNotFoundException("El archivo del modelo ML.NET no se encontró.", modeloPath);

            _modelo = _mlContext.Model.Load(modeloPath, out _);
        }

        public bool PredecirSentimiento(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto))
                return false; // Considera lo que tenga sentido para ti

            var predEngine = _mlContext.Model.CreatePredictionEngine<SentimientoData, SentimientoPrediction>(_modelo);

            var input = new SentimientoData { Texto = texto };

            var resultado = predEngine.Predict(input);

            return resultado.Prediccion;
        }
    }
}
