using Microsoft.ML.Data;

namespace AgenciaDeViajes.ML
{
    public class SentimientoPrediction
    {
        // Columna predicha (positivo o negativo)
        [ColumnName("PredictedLabel")]
        public bool Prediccion { get; set; }

        // Probabilidad de la predicción
        public float Probability { get; set; }

        // Puntaje de la predicción
        public float Score { get; set; }
    }
}
